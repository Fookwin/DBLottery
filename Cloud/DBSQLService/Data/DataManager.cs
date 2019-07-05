using DBSQLService.Data;
using LuckyBallsData;
using LuckyBallsData.Util;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace DBSQLService
{
    public class DataManager
    {
        private CloudBlockBlob mBlobDataVersion = null;

        // cache data.
        private DateTimeOffset? s_versionFileTimeStamp = null;
        private DataVersion s_versionCache = null;

        private string s_fullDataCache = "";
        private string s_baseDataCache = "";
        private string s_latestLotteryDataCache = "";
        private string s_latestReleaseDataCache = "";
        private string s_latestAttributeCache = "";
        private string s_attributeTemplateCache = "";
        private string s_helpCache = "";
        private Dictionary<string, string> s_matrixTemplateCache = new Dictionary<string, string>();

        private object lockObjectOnCache = new object();

        private DataManager()
        {
        }

        private static DataManager s_singleton = null;

        public static DataManager Get()
        {
            if (s_singleton == null)
                s_singleton = new DataManager();

            return s_singleton;
        }

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

        public string _GetDataVersion()
        {
            _ValidateCache();

            return s_versionCache.ToString();
        }

        public string _GetLotteryData(int issue)
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

        public string _GetLotteriesBaseByIndex(int index_from, int index_to)
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

        public string _GetAllLotteries(bool bBaseOnly)
        {
            _ValidateCache();

            int count = DBSQLClient.Instance().GetRecordCount();
            if (bBaseOnly)
            {
                if (s_baseDataCache != "")
                    return s_baseDataCache;

                string output = _GetLotteriesBaseByIndex(0, count - 1);

                // refresh cache
                s_baseDataCache = output;

                return output;
            }
            else
            {
                if (s_fullDataCache != "")
                    return s_fullDataCache;

                string output = _GetLotteriesBaseByIndex(0, count - 1);

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

        public bool _GetBaseXml(int issue, Basic basic, Detail detail, ref XmlElement node)
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

        public bool _GetXml(int issue, Basic basic, Detail detail, Omission omssion, Attribute attribute, ref XmlElement node)
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

    }
}