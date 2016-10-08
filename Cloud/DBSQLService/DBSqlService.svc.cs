using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using LuckyBallsData;
using LuckyBallsData.Util;
using DBSQLService.Data;

namespace DBSQLService
{
    public class DBSqlService : ISqlService
    {
        #region interfaces
        public string GetLotteryData(int issue)
        {
            return _GetLotteryData(issue);
        }

        public string GetAllLotteries()
        {
            return _GetAllLotteries(false); 
        }

        public int GetLotteryCount()
        {
            return DBSQLClient.Instance().GetRecordCount();
        }

        public string GetLotteriesByIssue(int issue_from, int issue_to)
        {
            List<Basic> basicList = null;
            List<Detail> detailList = null;
            List<Omission> omissionList = null;
            List<Attribute> attributeList = null;
            DBSQLClient.Instance().GetRecordList(out basicList, out detailList, out omissionList, out attributeList);

            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("History");
            xml.AppendChild(root);

            for (int i = 0; i < basicList.Count(); ++i)
            {
                if (basicList[i].Issue > issue_to)
                    break;

                if (basicList[i].Issue > issue_from)
                {
                    XmlElement node = xml.CreateElement("Lottery");
                    root.AppendChild(node);

                    if (!_GetXml(basicList[i].Issue, basicList[i], detailList[i], omissionList[i], attributeList[i], ref node))
                        return "";
                }
            }

            return xml.OuterXml;
        }

        public string GetLotteriesByIndex(int index_from, int index_to)
        {
            List<Basic> basicList = null;
            List<Detail> detailList = null;
            List<Omission> omissionList = null;
            List<Attribute> attributeList = null;
            DBSQLClient.Instance().GetRecordList(out basicList, out detailList, out omissionList, out attributeList);

            if (index_to >= basicList.Count || index_from > index_to)
                return "";

            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("History");
            xml.AppendChild(root);

            for (int i = index_from; i <= index_to; ++i)
            {
                XmlElement node = xml.CreateElement("Lottery");
                root.AppendChild(node);

                if (!_GetXml(basicList[i].Issue, basicList[i], detailList[i], omissionList[i], attributeList[i], ref node))
                    return "";
            }

            return xml.OuterXml;
        }

        public string GetDataVersion()
        {
            return _GetDataVersion();
        }

        public string GetAttributesTemplate()
        {
            return _GetAttributesTemplate();
        }

        public string GetLatestAttributes()
        {
            return _GetLatestAttributes();
        }

        public string GetLatestReleaseInfo()
        {
            return _GetLatestReleaseInfo();
        }

        public string GetMatrixTableItem(int candidateCount, int selectCount)
        {
            return _GetMatrixTableItem(candidateCount, selectCount);
        }

        public string GetHelp()
        {
            return _GetHelp();
        }

        public string GetAllLotteriesBase()
        {
            return _GetAllLotteries(true);
        }

        public string GetLotteriesBaseByIndex(int index_from, int index_to)
        {
            List<Basic> basicList = null;
            List<Detail> detailList = null;
            List<Omission> omissionList = null;
            List<Attribute> attributeList = null;
            DBSQLClient.Instance().GetRecordList(out basicList, out detailList, out omissionList, out attributeList);

            if (index_to >= basicList.Count || index_from > index_to)
                return "";

            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("History");
            xml.AppendChild(root);

            for (int i = index_from; i <= index_to; ++i)
            {
                XmlElement node = xml.CreateElement("Lottery");
                root.AppendChild(node);

                if (!_GetBaseXml(basicList[i].Issue, basicList[i], detailList[i], ref node))
                    return "";
            }

            return xml.OuterXml;
        }

        public string GetLotteriesBaseByIssue(int issue_from, int issue_to)
        {
            List<Basic> basicList = null;
            List<Detail> detailList = null;
            List<Omission> omissionList = null;
            List<Attribute> attributeList = null;
            DBSQLClient.Instance().GetRecordList(out basicList, out detailList, out omissionList, out attributeList);

            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("History");
            xml.AppendChild(root);

            for (int i = 0; i < basicList.Count(); ++i)
            {
                if (basicList[i].Issue > issue_to)
                    break;

                if (basicList[i].Issue > issue_from)
                {
                    XmlElement node = xml.CreateElement("Lottery");
                    root.AppendChild(node);

                    if (!_GetBaseXml(basicList[i].Issue, basicList[i], detailList[i], ref node))
                        return "";
                }
            }

            return xml.OuterXml;
        }

        #endregion

        #region data

        private CloudBlockBlob mBlobDataVersion = null;

        // cache data.
        private static DateTimeOffset? s_versionFileTimeStamp = null;
        private static DataVersion s_versionCache = null;

        private static string s_fullDataCache = "";
        private static string s_baseDataCache = "";
        private static string s_latestLotteryDataCache = "";
        private static string s_latestReleaseDataCache = "";
        private static string s_latestAttributeCache = "";
        private static string s_attributeTemplateCache = "";
        private static string s_helpCache = "";
        private static Dictionary<string, string> s_matrixTemplateCache = new Dictionary<string,string>();

        private static object lockObjectOnCache = new object();

        #endregion

        #region impplementations

        private void _ValidateCache()
        {
            lock (lockObjectOnCache)
            {
                if (mBlobDataVersion == null)
                {
                    mBlobDataVersion = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "Version.xml");
                }

                if (s_versionFileTimeStamp != null && s_versionFileTimeStamp.Value != null)
                {
                    mBlobDataVersion.FetchAttributes();

                    // no change on version since last change.
                    if (s_versionFileTimeStamp.Value == mBlobDataVersion.Properties.LastModified.Value)
                        return;
                }

                // read the data from the version file.
                string dataVersion;
                using (var memoryStream = new MemoryStream())
                {
                    mBlobDataVersion.DownloadToStream(memoryStream);
                    dataVersion = System.Text.Encoding.ASCII.GetString(memoryStream.ToArray());
                }

                // update the file time stamp.
                s_versionFileTimeStamp = mBlobDataVersion.Properties.LastModified;

                // parse to a temp object for comparing.
                DataVersion latestVersion = new DataVersion();

                DBXmlDocument xml = new DBXmlDocument();
                xml.Load(dataVersion);
                latestVersion.ReadFromXml(xml.Root());

                // update the cache data
                bool historyChanged = true; 
                bool latestLotDataChanged = true; 
                bool latestReleaseDataChanged = true;
                bool latestAttriChanged = true;
                bool attriTempChanged = true;
                bool matrixTempChanged = true;
                bool helpChanged = true;

                if (s_versionCache != null)
                {
                    // mark the single flag if the data version is changed.
                    historyChanged = s_versionCache.HistoryDataVersion != latestVersion.HistoryDataVersion;
                    latestLotDataChanged = s_versionCache.LatestLotteryVersion != latestVersion.LatestLotteryVersion;
                    latestReleaseDataChanged = s_versionCache.ReleaseDataVersion != latestVersion.ReleaseDataVersion;
                    latestAttriChanged = s_versionCache.AttributeDataVersion != latestVersion.AttributeDataVersion;
                    attriTempChanged = s_versionCache.AttributeTemplateVersion != latestVersion.AttributeTemplateVersion;
                    matrixTempChanged = s_versionCache.MatrixDataVersion != latestVersion.MatrixDataVersion;
                    helpChanged = s_versionCache.HelpContentVersion != latestVersion.HelpContentVersion;

                    if (s_versionCache.LatestIssue != latestVersion.LatestIssue)
                    {
                        // if the new issue released, need update latest data and the history (full) data.
                        historyChanged = true;
                        latestReleaseDataChanged = true;
                        latestAttriChanged = true;
                        latestLotDataChanged = true;
                    }
                }

                // clean the cache accordingly.
                if (historyChanged)
                {
                    s_fullDataCache = "";
                    s_baseDataCache = "";
                }

                if (latestLotDataChanged)
                    s_latestLotteryDataCache = "";

                if (latestReleaseDataChanged)
                    s_latestReleaseDataCache = "";

                if (latestAttriChanged)
                    s_latestAttributeCache = "";

                if (attriTempChanged)
                    s_attributeTemplateCache = "";

                if (matrixTempChanged)
                    s_matrixTemplateCache.Clear();

                if (helpChanged)
                    s_helpCache = "";

                // update version of the cache.
                s_versionCache = latestVersion;
            }            
        }

        private string _GetDataVersion()
        {
            _ValidateCache();

            return s_versionCache.ToString();
        }

        private string _GetLotteryData(int issue)
        {
            // must be called piror reuse any catch.
            _ValidateCache();

            // if the issue has been cached yet, return the cache.
            if (s_versionCache != null && issue == s_versionCache.LatestIssue && s_latestLotteryDataCache != "")
            {
                return s_latestLotteryDataCache;
            }

            Basic basic = null;
            Detail detail = null;
            Omission omission = null;
            Attribute attribute = null;
            if (!DBSQLClient.Instance().GetRecord(issue, out basic, out detail, out omission, out attribute))
                return "";

            XmlDocument xml = new XmlDocument();
            XmlElement node = xml.CreateElement("Lottery");
            xml.AppendChild(node);

            if (!_GetXml(issue, basic, detail, omission, attribute, ref node))
                return "";

            string output = xml.OuterXml;

            if (s_versionCache != null && issue == s_versionCache.LatestIssue)
            {
                // refresh the cache.
                s_latestLotteryDataCache = output;
            }

            return output;
        }

        private string _GetAllLotteries(bool bBaseOnly)
        {
            _ValidateCache();

            int count = DBSQLClient.Instance().GetRecordCount();
            if (bBaseOnly)
            {
                if (s_baseDataCache != "")
                    return s_baseDataCache;

                string output = GetLotteriesBaseByIndex(0, count - 1);

                // refresh cache
                s_baseDataCache = output;

                return output;
            }
            else
            {
                if (s_fullDataCache != "")
                    return s_fullDataCache;

                string output = GetLotteriesByIndex(0, count - 1);

                // refresh cache
                s_fullDataCache = output;

                return output;
            }
        }

        public string _GetAttributesTemplate()
        {
            _ValidateCache();

            if (s_attributeTemplateCache != "")
                return s_attributeTemplateCache;

            CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "AttributesTemplate.xml");

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            // refresh cache.
            s_attributeTemplateCache = text;

            return text;
        }

        public string _GetLatestAttributes()
        {
            _ValidateCache();

            if (s_latestAttributeCache != "")
                return s_latestAttributeCache;

            CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "LatestAttributes.xml");

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            // refresh cache.
            s_latestAttributeCache = text;

            return text;
        }

        public string _GetLatestReleaseInfo()
        {
            _ValidateCache();

            if (s_latestReleaseDataCache != "")
                return s_latestReleaseDataCache;

            CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "ReleaseInformation.xml");

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            // refresh cache.
            s_latestReleaseDataCache = text;

            return text;
        }

        public string _GetMatrixTableItem(int candidateCount, int selectCount)
        {
            _ValidateCache();

            // use the file name as the cache key.
            string fileName = candidateCount.ToString() + "-" + selectCount.ToString() + ".txt";

            if (s_matrixTemplateCache.ContainsKey(fileName))
            {
                return s_matrixTemplateCache[fileName];
            }
            
            CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dbmatrixtale", fileName);

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            // update the cache.
            s_matrixTemplateCache[fileName] = text;

            return text;
        }

        private bool _GetBaseXml(int issue, Basic basic, Detail detail, ref XmlElement node)
        {
            // save basic data.
            node.SetAttribute("Issue", basic.Issue.ToString());
            node.SetAttribute("Scheme", basic.Red1.ToString().PadLeft(2, '0') + " " +
                                        basic.Red2.ToString().PadLeft(2, '0') + " " +
                                        basic.Red3.ToString().PadLeft(2, '0') + " " +
                                        basic.Red4.ToString().PadLeft(2, '0') + " " +
                                        basic.Red5.ToString().PadLeft(2, '0') + " " +
                                        basic.Red6.ToString().PadLeft(2, '0') + " " +
                                        basic.Blue.ToString().PadLeft(2, '0'));

            // save release date.
            node.SetAttribute("Date", detail.Date.ToString("yyyy-MM-dd"));

            return true;
        }

        private bool _GetXml(int issue, Basic basic, Detail detail, Omission omssion, Attribute attribute, ref XmlElement node)
        {
            // save basic data.
            node.SetAttribute("Issue", basic.Issue.ToString());
            node.SetAttribute("Scheme", basic.Red1.ToString().PadLeft(2, '0') + " " +
                                        basic.Red2.ToString().PadLeft(2, '0') + " " +
                                        basic.Red3.ToString().PadLeft(2, '0') + " " +
                                        basic.Red4.ToString().PadLeft(2, '0') + " " +
                                        basic.Red5.ToString().PadLeft(2, '0') + " " +
                                        basic.Red6.ToString().PadLeft(2, '0') + " " +
                                        basic.Blue.ToString().PadLeft(2, '0'));

            // save detail data.
            node.SetAttribute("Date", detail.Date.ToString("yyyy-MM-dd"));
            node.SetAttribute("Bet", detail.BetAmount.ToString());
            node.SetAttribute("Pool", detail.PoolAmount.ToString());
            node.SetAttribute("Comments", detail.More);

            XmlElement bonusNode = node.OwnerDocument.CreateElement("Bonus");
            node.AppendChild(bonusNode);

            bonusNode.SetAttribute("PrizeCounts", detail.Prize1Count.ToString() + " " +
                                                    detail.Prize2Count.ToString() + " " +
                                                    detail.Prize3Count.ToString() + " " +
                                                    detail.Prize4Count.ToString() + " " +
                                                    detail.Prize5Count.ToString() + " " +
                                                    detail.Prize6Count.ToString());
            bonusNode.SetAttribute("Prize1Bonus", ((int)detail.Prize1Bonus).ToString());
            bonusNode.SetAttribute("Prize2Bonus", ((int)detail.Prize2Bonus).ToString());

            // save extend data.
            XmlElement statusNode = node.OwnerDocument.CreateElement("Status");
            node.AppendChild(statusNode);

            // omission.
            string[] reds_hitList = new string[33], reds_omissionList = new string[33], reds_tempList = new string[33];
            string[] blues_hitList = new string[16], blues_omissionList = new string[16], blues_tempList = new string[16];

            Type numStateType = typeof(Omission);
            System.Reflection.PropertyInfo[] properties = numStateType.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                string name = property.Name;

                if (name.Contains("Red_"))
                {
                    int num = Convert.ToInt32(name.Substring(4, 2));

                    if (name.Contains("_Hit"))
                    {
                        reds_hitList[num - 1] = property.GetValue(omssion).ToString();
                    }
                    else if (name.Contains("_Omission"))
                    {
                        reds_omissionList[num - 1] = property.GetValue(omssion).ToString();
                    }
                    else // _Temperature
                    {
                        reds_tempList[num - 1] = property.GetValue(omssion).ToString();
                    }
                }
                else if (name != "Issue") // blue
                {
                    int num = Convert.ToInt32(name.Substring(5, 2));

                    if (name.Contains("_Hit"))
                    {
                        blues_hitList[num - 1] = property.GetValue(omssion).ToString();
                    }
                    else if (name.Contains("_Omission"))
                    {
                        blues_omissionList[num - 1] = property.GetValue(omssion).ToString();
                    }
                    else // _Temperature
                    {
                        blues_tempList[num - 1] = property.GetValue(omssion).ToString();
                    }
                }
            }

            string reds_hit = "", reds_omission = "", reds_temp = "";
            string blues_hit = "", blues_omission = "", blues_temp = "";

            for (int i = 0; i < 33; ++i)
            {
                reds_hit += reds_hitList[i] + " ";
                reds_omission += reds_omissionList[i] + " ";
                reds_temp += reds_tempList[i] + " ";
            }

            for (int i = 0; i < 16; ++i)
            {
                blues_hit += blues_hitList[i] + " ";
                blues_omission += blues_omissionList[i] + " ";
                blues_temp += blues_tempList[i] + " ";
            }

            statusNode.SetAttribute("Red_HitCount", reds_hit.TrimEnd(' '));
            statusNode.SetAttribute("Red_Omission", reds_omission.TrimEnd(' '));
            statusNode.SetAttribute("Red_Temperature", reds_temp.TrimEnd(' '));

            statusNode.SetAttribute("Blue_HitCount", blues_hit.TrimEnd(' '));
            statusNode.SetAttribute("Blue_Omission", blues_omission.TrimEnd(' '));
            statusNode.SetAttribute("Blue_Temperature", blues_temp.TrimEnd(' '));

            // attributes
            string attributeValues = "";

            Type attributeType = typeof(Attribute);
            System.Reflection.PropertyInfo[] properties1 = attributeType.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties1)
            {
                if (property.Name != "Issue")
                {
                    attributeValues += property.GetValue(attribute).ToString() + " ";
                }
            }

            statusNode.SetAttribute("Attribute_Value", attributeValues.TrimEnd(' '));

            return true;
        }

        public string _GetHelp()
        {
            _ValidateCache();

            if (s_helpCache != "")
                return s_helpCache;

            CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "Help.xml");

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            // refresh cache.
            s_helpCache = text;

            return text;
        }

        #endregion

        #region obsolete

        public class UserDataItem : TableEntity
        {
            public UserDataItem(string platform, string userId, string chanelUri, string name, string lastLoginDate)
            {
                this.PartitionKey = platform;
                this.RowKey = userId;
                ChannelUri = chanelUri;
                Name = name;
                LastLogin = DateTime.Parse(lastLoginDate);
            }

            public UserDataItem()
            {
            }

            public string PlatForm
            {
                get
                {
                    return this.PartitionKey;
                }
                set
                {
                    this.PartitionKey = value;
                }
            }

            public Guid UserId
            {
                get
                {
                    return Guid.Parse(this.RowKey);
                }
                set
                {
                    this.RowKey = value.ToString();
                }
            }

            public string ChannelUri { get; set; }
            public string Name { get; set; }
            public DateTime LastLogin { get; set; }
        }


        public void GetVersion(ref int version, ref Int64 revisions, ref int latestIssue)
        {
            string versionString = GetDataVersion();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(versionString);

            XmlElement root = xml.FirstChild as XmlElement;
            if (root != null)
            {
                latestIssue = Convert.ToInt32(root.GetAttribute("LatestIssue"));

                // Set default version and revision
                version = 2;
                revisions = 0;
            }
        }

        public void RegisterUserChannel(string userId, string channelUri, string platform)
        {
            String tableName = "UserData";

            // Create the CloudTable object that represents the table. 
            CloudTable table = DBCloudStorageClient.Instance().GetTable(tableName);
            if (table == null)
            {
                throw new Exception("Table for user data does not exist.");
            }

            // Construct the query operation for all customer entities where PartitionKey=userid. 
            string queryString = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId);
            TableQuery<UserDataItem> query = new TableQuery<UserDataItem>().Where(queryString);

            IEnumerable<UserDataItem> result = table.ExecuteQuery(query);
            if (result.Count() == 1)
            {
                UserDataItem current = result.Last<UserDataItem>();
                current.ChannelUri = channelUri;

                TableOperation operation = TableOperation.Replace(current);
                table.Execute(operation);
            }
            else
            {
                // Add a new item for this user.
                UserDataItem current = new UserDataItem(platform, userId, channelUri, null, DateTime.UtcNow.ToString());

                TableOperation operation = TableOperation.Insert(current);
                table.Execute(operation);
            }
        }

        public string[] GetUserChannels(string platform)
        {
            List<string> channels = new List<string>();

            String tableName = "UserData";

            // Create the CloudTable object that represents the table. 
            CloudTable table = DBCloudStorageClient.Instance().GetTable(tableName);
            if (table == null)
            {
                throw new Exception("Table for user data does not exist.");
            }

            string queryString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, platform);
            TableQuery<UserDataItem> query = new TableQuery<UserDataItem>().Where(queryString);

            IEnumerable<UserDataItem> result = table.ExecuteQuery(query);
            foreach (UserDataItem item in result)
            {
                if (item.ChannelUri != "")
                    channels.Add(item.ChannelUri);
            }

            return channels.ToArray();
        }

        public void RememberUserLastLoginDate(string userId)
        {
            String tableName = "UserData";

            // Create the CloudTable object that represents the table. 
            CloudTable table = DBCloudStorageClient.Instance().GetTable(tableName);
            if (table == null)
            {
                throw new Exception("Table for user data does not exist.");
            }

            // Construct the query operation for all customer entities where PartitionKey=userid. 
            string queryString = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId);
            TableQuery<UserDataItem> query = new TableQuery<UserDataItem>().Where(queryString);

            IEnumerable<UserDataItem> result = table.ExecuteQuery(query);
            if (result.Count() == 1)
            {
                UserDataItem current = result.Last<UserDataItem>();
                current.LastLogin = DateTime.UtcNow;

                TableOperation operation = TableOperation.Replace(current);
                table.Execute(operation);
            }
            else
            {
                throw new Exception("User does not exist.");
            }
        }

        public void GetLatestSoftwareVersion(ref int version, ref bool schemaChanged)
        {
            CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dbsoftwareversions", "CurrentVersion.txt");

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            string[] segs = text.Split(' ');

            version = Convert.ToInt32(segs[0]);
            schemaChanged = Convert.ToBoolean(segs[1]);;
        }

        public string GetReleaseNotes(int fromVersion)
        {
            string output = "";

            CloudBlobContainer container = DBCloudStorageClient.Instance().GetBlobContainer("dbsoftwareversions");

            var blobList = container.ListBlobs();
            foreach (IListBlobItem item in blobList)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    if (blob.Name != "CurrentVersion.txt")
                    {
                        int version = Convert.ToInt32(blob.Name.Substring(0,8));
                        if (version > fromVersion)
                        {
                            string text;
                            using (var memoryStream = new MemoryStream())
                            {
                                blob.DownloadToStream(memoryStream);
                                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                            }

                            output += text + "\n";
                        }
                    }
                }
            }

            return output;
        }
        #endregion
    }
}
