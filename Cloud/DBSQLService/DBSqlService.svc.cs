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
            return DataManager.Get()._GetLotteryData(issue);
        }

        public string GetAllLotteries()
        {
            return DataManager.Get()._GetAllLotteries(false); 
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

                    if (!DataManager.Get()._GetXml(basicList[i].Issue, basicList[i], detailList[i], omissionList[i], attributeList[i], ref node))
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

                if (!DataManager.Get()._GetXml(basicList[i].Issue, basicList[i], detailList[i], omissionList[i], attributeList[i], ref node))
                    return "";
            }

            return xml.OuterXml;
        }

        public string GetDataVersion()
        {
            return DataManager.Get()._GetDataVersion();
        }

        public string GetAttributesTemplate()
        {
            return DataManager.Get()._GetAttributesTemplate();
        }

        public string GetLatestAttributes()
        {
            return DataManager.Get()._GetLatestAttributes();
        }

        public string GetLatestReleaseInfo()
        {
            return DataManager.Get()._GetLatestReleaseInfo();
        }

        public string GetMatrixTableItem(int candidateCount, int selectCount)
        {
            return DataManager.Get()._GetMatrixTableItem(candidateCount, selectCount);
        }

        public string GetHelp()
        {
            return DataManager.Get()._GetHelp();
        }

        public string GetAllLotteriesBase()
        {
            return DataManager.Get()._GetAllLotteries(true);
        }

        public string GetLotteriesBaseByIndex(int index_from, int index_to)
        {
            return DataManager.Get()._GetLotteriesBaseByIndex(index_from, index_to);
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

                    if (!DataManager.Get()._GetBaseXml(basicList[i].Issue, basicList[i], detailList[i], ref node))
                        return "";
                }
            }

            return xml.OuterXml;
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
