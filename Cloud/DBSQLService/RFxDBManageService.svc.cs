using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Net;
using DBSQLService.Data;
using LuckyBallsData.Statistics;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;
using DataModel;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DBSQLService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DBManagement" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DBManagement.svc or DBManagement.svc.cs at the Solution Explorer and start debugging.
    public class RFxDBManageService : IRFxDBManageService
    {
        public string PushNotification(NotificationPocket notification)
        {
            foreach (int platform in notification.Platforms)
            {
                string notifyContent = FormatNotification(platform, notification.Message.Title, notification.Message.Content);

                DBNotification.Instance().PushNotification((PlatformIndex)platform, notifyContent, new List<string>());
            }

            return "success";
        }

        public Dictionary<string, MessagePocket> GetNotificationTemplates()
        {
            // get the last release information
            CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "ReleaseInformation.xml");

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            // parse to a temp object for comparing.
            LuckyBallsData.ReleaseInfo lastRelease = new LuckyBallsData.ReleaseInfo();

            DBXmlDocument xml = new DBXmlDocument();
            xml.Load(text);
            lastRelease.Read(xml.Root());

            int issue = lastRelease.CurrentIssue;

            // get the data
            Basic basic = null;
            Detail detail = null;
            Omission omission = null;
            Attribute attribute = null;
            if (!DBSQLClient.Instance().GetRecord(issue, out basic, out detail, out omission, out attribute))
                return null;

            Status lastStatus = ExtractStatus(omission, attribute);

            Dictionary<string, MessagePocket> templates = new Dictionary<string, MessagePocket>();

            string title = "", content = "";

            //case "release":
            {
                title = "第" + issue.ToString() + "期" + " 开奖啦！";
                content = "红: " + basic.Red1.ToString().PadLeft(2, '0') + " " +
                            basic.Red2.ToString().PadLeft(2, '0') + " " +
                            basic.Red3.ToString().PadLeft(2, '0') + " " +
                            basic.Red4.ToString().PadLeft(2, '0') + " " +
                            basic.Red5.ToString().PadLeft(2, '0') + " " +
                            basic.Red6.ToString().PadLeft(2, '0') + " " +
                            " 蓝: " + basic.Blue.ToString().PadLeft(2, '0');

                templates.Add("NEW RELEASE", new MessagePocket() { Title = title, Content = content });                        
            }

            //    case "detail":
            {
                int firstPZCount = detail.Prize1Count;
                int pool = detail.PoolAmount;
                if (pool > 0)
                {
                    int expectFPZCount = pool / 5000000;

                    title = "一等奖中出 " + firstPZCount.ToString() + " 注 " + Lottery.FormatMoney((int)detail.Prize1Bonus);
                    content = "奖池 " + Lottery.FormatMoney((int)detail.PoolAmount) + ", 够开出 " + expectFPZCount.ToString() + " 个五百万！";

                    templates.Add("RELEASE DETAIL", new MessagePocket() { Title = title, Content = content });
                }
            }

            //    case "recommendation-red":
            {
                // top missed red.
                int r_top = 0, r_mis = 0;
                for (int i = 1; i <= 33; ++i)
                {
                    int redOmission = lastStatus.RedNumStates[i - 1].Omission;

                    if (r_top == 0 || redOmission > r_mis)
                    {
                        r_top = i;
                        r_mis = redOmission;
                    }
                }

                title = "第" + lastRelease.NextIssue.ToString() + "期 今晚开奖！";
                content = "参考：" + "红球 " + r_top.ToString().PadLeft(2, '0') + " 已连续 " + r_mis.ToString() + " 期未开出！";

                templates.Add("RECOMMENDED RED", new MessagePocket() { Title = title, Content = content });
            }

            //    case "recommendation-blue":
            {
                // top missed blue.
                int b_top = 0, b_mis = 0;
                for (int i = 1; i <= 16; ++i)
                {
                    int blueOmission = lastStatus.BlueNumStates[i - 1].Omission;

                    if (b_top == 0 || blueOmission > b_mis)
                    {
                        b_top = i;
                        b_mis = blueOmission;
                    }
                }

                title = "第" + lastRelease.NextIssue.ToString() + "期 今晚开奖！";
                content = "参考：" + "蓝球 " + b_top.ToString().PadLeft(2, '0') + " 已连续 " + b_mis.ToString() + " 期未开出！";

                templates.Add("RECOMMENDED BLUE", new MessagePocket() { Title = title, Content = content });
            }

            //    case "recommendation-attribute":
            {
                // top missed attribute
                SchemeAttributeValueStatus state1 = null;
                double prop1 = 0.0;
                SchemeAttributes attriSets = ReadAttributes(issue);
                foreach (KeyValuePair<string, SchemeAttributeCategory> cat in attriSets.Categories)
                {
                    foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Value.Attributes)
                    {
                        foreach (SchemeAttributeValueStatus state in attri.Value.ValueStates)
                        {
                            if (state.AverageOmission < 1.0 || state.HitProbability < 5)
                                continue; // skip the attribute happens too frequently.

                            if (state1 == null || prop1 < state.ProtentialEnergy)
                            {
                                state1 = state;
                                prop1 = state.ProtentialEnergy;
                            }
                        }
                    }
                }

                title = "第" + lastRelease.NextIssue.ToString() + "期 今晚开奖！";
                content = "参考：" + "属性 " + state1.DisplayName + " 偏离指数已达到 " + prop1.ToString() + " 倍！";

                templates.Add("RECOMMENDED ATTRIBUTE", new MessagePocket() { Title = title, Content = content });
            }

            return templates;
        }

        public DBReleaseModel GetLatestRelease()
        {
            DBReleaseModel release = new DBReleaseModel();

            int currentIssue = -1;
            // get the last release information
            {
                CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "ReleaseInformation.xml");

                string text;
                using (var memoryStream = new MemoryStream())
                {
                    blob.DownloadToStream(memoryStream);
                    text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                // parse to a temp object for comparing.
                LuckyBallsData.ReleaseInfo lastRelease = new LuckyBallsData.ReleaseInfo();

                DBXmlDocument xml = new DBXmlDocument();
                xml.Load(text);
                lastRelease.Read(xml.Root());

                currentIssue = lastRelease.CurrentIssue;
                release.NextRelease = new DBReleaseInfoModel()
                {
                    Issue = lastRelease.NextIssue,
                    CutOffTime = lastRelease.SellOffTime,
                    Date = lastRelease.NextReleaseTime
                };

                // recommendation
                release.Recommendation = new DBRecommendationModel()
                {
                    RedExcludes = new List<int>(lastRelease.ExcludedReds.Numbers),
                    RedIncludes = new List<int>(lastRelease.IncludedReds.Numbers),
                    BlueExcludes = new List<int>(lastRelease.ExcludedBlues.Numbers),
                    BlueIncludes = new List<int>(lastRelease.IncludedBlues.Numbers),
                };
            }

            // get the version information in the last update.
            {
                CloudBlockBlob mBlobDataVersion = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "Version.xml");

                // read the data from the version file.
                string dataVersion;
                using (var memoryStream = new MemoryStream())
                {
                    mBlobDataVersion.DownloadToStream(memoryStream);
                    dataVersion = System.Text.Encoding.ASCII.GetString(memoryStream.ToArray());
                }

                // parse to a temp object for comparing.
                LuckyBallsData.DataVersion latestVersion = new LuckyBallsData.DataVersion();

                DBXmlDocument xml = new DBXmlDocument();
                xml.Load(dataVersion);
                latestVersion.ReadFromXml(xml.Root());

                release.DataVersion = new DBVersionModel()
                {
                    LatestIssue = latestVersion.LatestIssue,
                    HistoryDataVersion = latestVersion.HistoryDataVersion,
                    ReleaseDataVersion = latestVersion.ReleaseDataVersion,
                    AttributeDataVersion = latestVersion.AttributeDataVersion,
                    AttributeTemplateVersion = latestVersion.AttributeTemplateVersion,
                    LatestLotteryVersion = latestVersion.LatestLotteryVersion,
                    MatrixDataVersion = latestVersion.MatrixDataVersion,
                    HelpContentVersion = latestVersion.HelpContentVersion
                };
            }

            // get the relase
            Basic basic = null;
            Detail detail = null;
            if (!DBSQLClient.Instance().GetRecordBasic(currentIssue, out basic, out detail))
                throw new Exception("Fail to obtain last record.");

            release.Lottery = buildLotteryModel(basic, detail);
            if (release.Lottery == null)
                throw new Exception("Fail to obtain last record.");

            return release;
        }

        public DBLotteryModel SyncLotteryToOffical(int issue)
        {
            DBLotteryModel lot = new DBLotteryModel() { Issue = issue };
            if (_ReadLotteryDataFromWeb(issue, ref lot))
            {
                return lot;
            }

            return null;
        }

        public DBReleaseInfoModel CalcualateNextReleaseInfo(int currentIssue, DateTime currentDate)
        {
            int nextIssue = 0;
            DateTime sellOfftime = DateTime.Now, nextReleaseDate = DateTime.Now;

            _CalculateNextReleaseNumberAndTime(currentIssue, currentDate, ref nextIssue, ref sellOfftime, ref nextReleaseDate);

            return new DBReleaseInfoModel() { Issue = nextIssue, Date = nextReleaseDate, CutOffTime = sellOfftime };
        }

        public CommitReleaseResultPocket PrecommitRelease(DBReleaseModel releaseData)
        {
            // Get latest release info
            DBReleaseModel latest = GetLatestRelease();
            if (latest.Lottery.Issue == releaseData.Lottery.Issue)
            {
                // update existing
                return _UpdateLatestRelease(releaseData);
            }
            else if (latest.NextRelease.Issue == releaseData.Lottery.Issue)
            {
                //add new release
                return _NorminateNewRelease(releaseData);
            }
            else
            {
                // not allow add/update others
                return new CommitReleaseResultPocket() { ErrorMessage = "Not allow to modify this release.", ReturnCode = 405 };
            }
        }

        public CommitReleaseResultPocket GetPendingActions()
        {
            CloudBlobContainer container = DBCloudStorageClient.Instance().GetBlobContainer("data-release-pending");
            if (container == null)
                return new CommitReleaseResultPocket() { ErrorMessage = "No pending container.", ReturnCode = 500 };

            List<string> pendingFiles = new List<string>();
            IEnumerable<IListBlobItem> blobs = container.ListBlobs();
            if (blobs != null)
            {
                foreach (IListBlobItem item in blobs)
                {
                    CloudBlockBlob blob = item as CloudBlockBlob;
                    pendingFiles.Add(blob.Name);
                }
            }

            return new CommitReleaseResultPocket()
            {
                ErrorMessage = "",
                ReturnCode = 200,
                Container = "data-release-pending",
                Files = pendingFiles
            };
        }

        public CommitReleaseResultPocket ExecutePendingActions()
        {
            CommitReleaseResultPocket actionsRes = GetPendingActions();
            if (actionsRes.Files == null || actionsRes.Files.Count == 0)
                return actionsRes;

            // execute Actions
            List<string> errFiles = new List<string>();

            bool anyError = false;
            foreach (string fileName in actionsRes.Files)
            {
                if (!ExecuteAction(actionsRes.Container, fileName))
                {
                    errFiles.Add(fileName);
                    anyError = true;
                }
            }

            return new CommitReleaseResultPocket()
            {
                ErrorMessage = anyError ? "Some action failed to execute." : "",
                ReturnCode = anyError ? 500 : 200,
                Container = "data-release-pending",
                Files = errFiles
            };
        }

        ////////////////////////////////////////////////////////////-Private-//////////////////////////////////////////////////////////////////        
        private string FormatNotification(int platform, string title, string content)
        {
            if (title == "" || content == "")
                return "";

            if (platform == 1)
            {
                //<toast>
                //    <visual>
                //        <binding template="ToastImageAndText02">
                //            <image id="1" src="https://dbdatastorage.blob.core.windows.net/dbnotification/Logo.png"/>
                //            <text id="1">Title</text>
                //            <text id="2">Content</text>
                //        </binding>
                //    </visual>
                //</toast>
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode toastNode = xml.AddRoot("toast");
                DBXmlNode visualNode = toastNode.AddChild("visual");
                DBXmlNode bindingNode = visualNode.AddChild("binding");

                bindingNode.SetAttribute("template", "ToastImageAndText02");

                DBXmlNode imageNode = bindingNode.AddChild("image");
                imageNode.SetAttribute("id", "1");
                imageNode.SetAttribute("src", "https://dbdatastorage.blob.core.windows.net/dbnotification/Logo.png");

                DBXmlNode text1Node = bindingNode.AddChild("text");
                text1Node.SetAttribute("id", "1");
                text1Node.SetValue(title);

                DBXmlNode text2Node = bindingNode.AddChild("text");
                text2Node.SetAttribute("id", "2");
                text2Node.SetValue(content);

                // Save to file.
                return xml.OuterXml();
            }
            else if (platform == 2)
            {
                //<?xml version=\"1.0\" encoding=\"utf-8\"?>
                //<wp:Notification xmlns:wp=\"WPNotification\">
                //    <wp:Toast>
                //        <wp:Text1>Title</wp:Text1>
                //        <wp:Text2>Content</wp:Text2>
                //    </wp:Toast>
                //</wp:Notification>

                return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                "<wp:Notification xmlns:wp=\"WPNotification\">" +
                                "<wp:Toast>" +
                                "<wp:Text1>" + title + " " + "</wp:Text1>" +
                                "<wp:Text2>" + content + "</wp:Text2>" +
                                "</wp:Toast>" +
                                "</wp:Notification>";
            }
            else if (platform == 3)
            {
                //"{\"title\":\"\",\"description\":\"test\"}"
                return "{\"title\":\"" + title + "\",\"description\":\"" + content + "\"}";
            }

            return "";
        }

        private CommitReleaseResultPocket _NorminateNewRelease(DBReleaseModel data)
        {
            List<string> pendingFiles = new List<string>();

            // get scheme
            Scheme newScheme = ExtractScheme(data.Lottery);

            // calculate the status for new release.
            Status newStatus = CalculateStatusForNewIssue(newScheme);

            // calculate the attributes set
            SchemeAttributes attirSet = CalculateAttributesForNewIssue(newStatus);
            if (attirSet == null)
                return new CommitReleaseResultPocket() { ErrorMessage = "Fail to calcuate the attributes.", ReturnCode = 500 };

            // Get the detail
            Detail newDetail = ExtractDetail(data.Lottery);

            // generate the SQL command lines file
            string sql = GenerateSQLQueryForNewRelease(data.Lottery.Issue, newScheme, newDetail, newStatus);
            if (string.IsNullOrEmpty(sql))
                return new CommitReleaseResultPocket() { ErrorMessage = "Fail to generate SQL for updating detail.", ReturnCode = 500 };

            // save sql to file
            {
                DBCloudStorageClient.Instance().WriteTextAsBlob("data-release-pending", "Lottery.sql", sql);
                pendingFiles.Add("Lottery.sql");
            }

            // save attributes
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Attributes");

                attirSet.SaveValueToXml(ref root);

                DBCloudStorageClient.Instance().WriteTextAsBlob("data-release-pending", "LatestAttributes.xml", xml.InnerXml());
                pendingFiles.Add("LatestAttributes.xml");
            }

            // updating version
            {
                // parse to a temp object for comparing.
                LuckyBallsData.DataVersion latestVersion = new LuckyBallsData.DataVersion()
                {
                    LatestIssue = data.DataVersion.LatestIssue,
                    HistoryDataVersion = data.DataVersion.HistoryDataVersion,
                    ReleaseDataVersion = data.DataVersion.ReleaseDataVersion,
                    AttributeDataVersion = data.DataVersion.AttributeDataVersion,
                    AttributeTemplateVersion = data.DataVersion.AttributeTemplateVersion,
                    LatestLotteryVersion = data.DataVersion.LatestLotteryVersion,
                    MatrixDataVersion = data.DataVersion.MatrixDataVersion,
                    HelpContentVersion = data.DataVersion.HelpContentVersion
                };

                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Version");
                latestVersion.SaveToXml(ref root);

                DBCloudStorageClient.Instance().WriteTextAsBlob("data-release-pending", "Version.xml", xml.InnerXml());
                pendingFiles.Add("Version.xml");
            }

            // save release information
            {
                // parse to a temp object for comparing.
                LuckyBallsData.ReleaseInfo lastRelease = new LuckyBallsData.ReleaseInfo()
                {
                    CurrentIssue = data.Lottery.Issue,
                    NextIssue = data.NextRelease.Issue,
                    SellOffTime = data.NextRelease.CutOffTime,
                    NextReleaseTime = data.NextRelease.Date
                };

                lastRelease.ExcludedReds.Reset(data.Recommendation.RedExcludes.ToArray());
                lastRelease.IncludedReds.Reset(data.Recommendation.RedIncludes.ToArray());
                lastRelease.ExcludedBlues.Reset(data.Recommendation.BlueExcludes.ToArray());
                lastRelease.IncludedBlues.Reset(data.Recommendation.BlueIncludes.ToArray());

                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Info");
                lastRelease.Save(ref root);

                DBCloudStorageClient.Instance().WriteTextAsBlob("data-release-pending", "ReleaseInformation.xml", xml.InnerXml());
                pendingFiles.Add("ReleaseInformation.xml");
            }

            return new CommitReleaseResultPocket()
            {
                ErrorMessage = "",
                ReturnCode = 200,
                Container = "data-release-pending",
                Files = pendingFiles
            };
        }

        private CommitReleaseResultPocket _UpdateLatestRelease(DBReleaseModel data)
        {
            List<string> pendingFiles = new List<string>();

            // Only details can be changed so far.
            Detail newDetail = ExtractDetail(data.Lottery);

            // update sql
            {
                // generate the SQL command lines file
                string sql = GenerateSQLQueryForDetail(newDetail, true);
                if (string.IsNullOrEmpty(sql))
                    return new CommitReleaseResultPocket() { ErrorMessage = "Fail to generate SQL for updating detail.", ReturnCode = 500 };

                DBCloudStorageClient.Instance().WriteTextAsBlob("data-release-pending", "Lottery.sql", sql);
                pendingFiles.Add("Lottery.sql");
            }

            // updating version
            {
                // parse to a temp object for comparing.
                LuckyBallsData.DataVersion latestVersion = new LuckyBallsData.DataVersion()
                {
                    LatestIssue = data.DataVersion.LatestIssue,
                    HistoryDataVersion = data.DataVersion.HistoryDataVersion,
                    ReleaseDataVersion = data.DataVersion.ReleaseDataVersion,
                    AttributeDataVersion = data.DataVersion.AttributeDataVersion,
                    AttributeTemplateVersion = data.DataVersion.AttributeTemplateVersion,
                    LatestLotteryVersion = data.DataVersion.LatestLotteryVersion,
                    MatrixDataVersion = data.DataVersion.MatrixDataVersion,
                    HelpContentVersion = data.DataVersion.HelpContentVersion
                };

                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Version");
                latestVersion.SaveToXml(ref root);

                DBCloudStorageClient.Instance().WriteTextAsBlob("data-release-pending", "Version.xml", xml.InnerXml());
                pendingFiles.Add("Version.xml");
            }

            // reload the latest issue.
            DBSQLClient.Instance().RefreshRecord(data.Lottery.Issue);

            return new CommitReleaseResultPocket()
            {
                ErrorMessage = "",
                ReturnCode = 200,
                Container = "data-release-pending",
                Files = pendingFiles
            };
        }

        private void _CalculateNextReleaseNumberAndTime(int currentIssue, DateTime releaseDate, ref int nextIssue, ref DateTime cutOfftime, ref DateTime nextReleaseDate)
        {
            int lastIssue = currentIssue;

            nextIssue = lastIssue + 1;
            if (releaseDate.Month == 12 && (releaseDate.Day > 29 || (releaseDate.Day == 29 && releaseDate.DayOfWeek == DayOfWeek.Thursday)))
            {
                nextIssue = (releaseDate.Year + 1) * 1000 + 1;
            }

            DateTime nextDate = new DateTime(releaseDate.Year, releaseDate.Month, releaseDate.Day);
            cutOfftime = nextDate.AddDays(nextDate.DayOfWeek == DayOfWeek.Thursday ? 3 : 2).AddHours(20);
            nextReleaseDate = cutOfftime.AddHours(1.25);
        }

        private bool _ReadLotteryDataFromWeb(int issue, ref DBLotteryModel lot)
        {
            try
            {
                const string rootSite = "http://www.17500.cn/ssq/details.php?issue=";

                string sourceSite = rootSite + issue.ToString();

                WebClient wc = new WebClient();
                Encoding encoding = Encoding.GetEncoding("GBK");
                string webData = encoding.GetString(wc.DownloadData(sourceSite));
                if (webData == "")
                    return false;

                int searchIndex = 0, currentIndex = 0, readCount = 0;
                string output, token;

                // date
                token = "开奖</td>";
                currentIndex = webData.IndexOf(token, searchIndex) - 10;
                if (currentIndex < 0)
                    return false;

                readCount = 10;
                output = webData.Substring(currentIndex, readCount);
                searchIndex = currentIndex + readCount;

                lot.Date = DateTime.Parse(output);

                // red numbers.
                token = "蓝色球";
                currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                if (currentIndex < token.Length)
                    return false;
                searchIndex = currentIndex;

                lot.Scheme = "";
                for (int i = 0; i < 6; ++i)
                {
                    token = "<font color=red>";
                    currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                    if (currentIndex < token.Length)
                        return false;

                    readCount = 2;
                    output = webData.Substring(currentIndex, readCount);
                    searchIndex = currentIndex + readCount;

                    if (i > 0)
                        lot.Scheme += " ";

                    lot.Scheme += (Convert.ToInt32(output.TrimStart('0'))).ToString().PadLeft(2, '0');
                }

                // Blue
                token = "<font color=blue>";
                currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                if (currentIndex < token.Length)
                    return false;

                readCount = 2;
                output = webData.Substring(currentIndex, readCount);
                searchIndex = currentIndex + readCount;

                lot.Scheme += "+" + (Convert.ToInt32(output.TrimStart('0'))).ToString().PadLeft(2, '0');

                // bonus
                lot.Bonus = new List<int>();
                int endIndex = 0;
                for (int i = 0; i < 12; ++i)
                {
                    // find the start
                    token = "<TD height=18 width=\"33%\" align=center>";
                    currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                    if (currentIndex < token.Length)
                        return false;

                    searchIndex = currentIndex;

                    // find the end.
                    token = "</TD>";
                    endIndex = webData.IndexOf(token, searchIndex);
                    if (endIndex < 0)
                        return false;

                    readCount = endIndex - currentIndex;
                    output = webData.Substring(currentIndex, readCount);
                    searchIndex = currentIndex + readCount;

                    // Get amount or money.
                    lot.Bonus.Add(Convert.ToInt32(output.Replace(",", "")));
                }

                // bet ammount.
                token = "投注总额为：";
                currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                if (currentIndex < token.Length)
                    return false;

                searchIndex = currentIndex;

                // find the end.
                token = "元";
                endIndex = webData.IndexOf(token, searchIndex);
                if (endIndex < 0)
                    return false;

                readCount = endIndex - currentIndex;
                output = webData.Substring(currentIndex, readCount);
                searchIndex = currentIndex + readCount;

                lot.Bet = Convert.ToInt32(output.Replace(",", ""));

                // pool ammount.
                token = "奖池金额为：";
                currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                if (currentIndex < token.Length)
                    return false;

                searchIndex = currentIndex;

                // find the end.
                token = "元";
                endIndex = webData.IndexOf(token, searchIndex);
                if (endIndex < 0)
                    return false;

                readCount = endIndex - currentIndex;
                output = webData.Substring(currentIndex, readCount);
                searchIndex = currentIndex + readCount;

                lot.Pool = Convert.ToInt32(output.Replace(",", ""));

                // comments.
                currentIndex = searchIndex + 5;
                searchIndex = currentIndex;

                token = "</td></tr></table>";
                endIndex = webData.IndexOf(token, searchIndex);

                if (endIndex > 0)
                {
                    readCount = endIndex - currentIndex;
                    output = webData.Substring(currentIndex, readCount).Replace("\r", "").Replace("\n", "").Replace(" ", "");
                    searchIndex = currentIndex + readCount;

                    if (output != "")
                    {
                        string comments = output;

                        int ind0 = comments.IndexOf("一等奖中奖地");
                        int ind2 = comments.IndexOf("出球顺序");
                        if (ind0 >= 0 && ind2 > ind0)
                        {
                            string winDetails = comments.Substring(ind0 + 7, ind2 - ind0 - 7);
                            winDetails = winDetails.Replace("注", "注 ");
                            winDetails = winDetails.Trim(new char[] { ' ', '。' });

                            lot.Details = "一等奖中奖地:" + winDetails + "。";

                            string orderDetails = comments.Substring(ind2 + 5).Trim(new char[] { ' ', '。' });

                            int indexOfPlus = orderDetails.IndexOf('+');
                            if (indexOfPlus >= 12)
                            {
                                string originalString = orderDetails.Substring(indexOfPlus - 12, 15);
                                if (!originalString.Contains(' '))
                                {
                                    orderDetails = "";
                                    for (int i = 1; i < 12; ++i)
                                    {
                                        if (i % 2 == 0)
                                        {
                                            orderDetails += originalString.Substring(i - 2, 2) + " ";
                                        }
                                    }

                                    orderDetails += originalString.Substring(10);
                                }
                            }

                            lot.Details += "出球顺序:" + orderDetails.TrimEnd();
                        }
                        else
                        {
                            lot.Details = comments;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        private Scheme ExtractScheme(Basic basic)
        {
            return new Scheme(basic.Red1, basic.Red2, basic.Red3, basic.Red4, basic.Red5, basic.Red6, basic.Blue);
        }

        private Scheme ExtractScheme(DBLotteryModel lot)
        {
            return new Scheme(lot.Scheme);
        }

        private Detail ExtractDetail(DBLotteryModel release)
        {
            return new Detail()
            {
                Issue = release.Issue,
                More = release.Details,
                Date = release.Date, 
                PoolAmount = release.Pool, 
                BetAmount = release.Bet,
                Prize1Count = release.Bonus[0],
                Prize1Bonus = release.Bonus[1],
                Prize2Count = release.Bonus[2],
                Prize2Bonus = release.Bonus[3],
                Prize3Count = release.Bonus[4],
                Prize4Count = release.Bonus[6],
                Prize5Count = release.Bonus[8],
                Prize6Count = release.Bonus[10]
            };       
        }

        private Status CalculateStatusForNewIssue(Scheme lotScheme)
        {
            Status curStatus = new Status();

            int currentCount = DBSQLClient.Instance().GetRecordCount();

            // get the history lists...
            List<Basic> basicList = null;
            List<Detail> detailList = null;
            List<Omission> omissionList = null;
            List<Attribute> attributeList = null;
            DBSQLClient.Instance().GetRecordsFromEnd(10, out basicList, out detailList, out omissionList, out attributeList);

            // latest status
            Status lastStatus = ExtractStatus(omissionList[9], attributeList[9]);

            // get the status for the 9th issue before latest
            Status refStatus = ExtractStatus(omissionList[0], attributeList[0]);

            // Red numbers...
            for (int i = 1; i <= 33; i++)
            {
                if (curStatus.RedNumStates[i - 1] == null)
                    curStatus.RedNumStates[i - 1] = new NumberState();

                if (lotScheme.Contains(i))
                {
                    curStatus.RedNumStates[i - 1].HitCount = lastStatus.RedNumStates[i - 1].HitCount + 1;
                    curStatus.RedNumStates[i - 1].Omission = 0;
                }
                else
                {
                    curStatus.RedNumStates[i - 1].HitCount = lastStatus.RedNumStates[i - 1].HitCount;
                    curStatus.RedNumStates[i - 1].Omission = lastStatus.RedNumStates[i - 1].Omission + 1;
                }

                curStatus.RedNumStates[i - 1].Temperature = curStatus.RedNumStates[i - 1].HitCount - refStatus.RedNumStates[i - 1].HitCount;
            }

            // Blue numbers.
            for (int i = 1; i <= 16; i++)
            {
                if (curStatus.BlueNumStates[i - 1] == null)
                    curStatus.BlueNumStates[i - 1] = new NumberState();

                if (lotScheme.Blue == i)
                {
                    curStatus.BlueNumStates[i - 1].HitCount = lastStatus.BlueNumStates[i - 1].HitCount + 1;
                    curStatus.BlueNumStates[i - 1].Omission = 0;
                }
                else
                {
                    curStatus.BlueNumStates[i - 1].HitCount = lastStatus.BlueNumStates[i - 1].HitCount;
                    curStatus.BlueNumStates[i - 1].Omission = lastStatus.BlueNumStates[i - 1].Omission + 1;
                }

                curStatus.BlueNumStates[i - 1].Temperature = curStatus.BlueNumStates[i - 1].HitCount - refStatus.BlueNumStates[i - 1].HitCount;
            }

            // Attribute values.
            foreach (KeyValuePair<string, AttributeState> state in curStatus.AttributeStates)
            {
                if (state.Key.Contains("Red_Repeat_Previous_"))
                {
                    int step = Convert.ToInt32(state.Key.Substring(state.Key.Length - 1, 1));

                    Scheme compareTo = ExtractScheme(basicList[9 - step]);
                    state.Value.Value = compareTo.Similarity(lotScheme);
                }
                else if (state.Key == "Blue_Amplitude")
                {
                    Scheme compareTo = ExtractScheme(basicList[9]);
                    state.Value.Value = Math.Abs(lotScheme.Blue - compareTo.Blue);
                }
                else if (state.Key.Contains("Blue_Mantissa_Repeat_Previous_"))
                {
                    int step = Convert.ToInt32(state.Key.Substring(state.Key.Length - 1, 1));

                    Scheme compareTo = ExtractScheme(basicList[9 - step]);
                    state.Value.Value = compareTo.Blue % 10 == lotScheme.Blue % 10 ? 1 : 0;
                }
                else if (state.Key == "Red_FuGeZhong")
                {
                    Scheme compareTo1 = ExtractScheme(basicList[9]);
                    Scheme compareTo2 = ExtractScheme(basicList[8]);

                    int countIn0 = 0, countIn1 = 0, countIn2 = 0;
                    int[] reds = lotScheme.GetRedNums();
                    foreach (int red in reds)
                    {
                        if (compareTo1.Contains(red))
                        {
                            countIn0++;
                        }
                        else if (compareTo2.Contains(red))
                        {
                            countIn1++;
                        }
                        else
                        {
                            countIn2++;
                        }
                    }

                    state.Value.Value = LuckyBallsData.Util.AttributeUtil.IndexOf3Div(countIn0, countIn1, countIn2);
                }
                else
                {
                    state.Value.Value = lotScheme.Attribute2(state.Key);
                }
            }

            return curStatus;
        }

        private SchemeAttributes CalculateAttributesForNewIssue(Status newStatus)
        {
            int currentIssue = DBSQLClient.Instance().GetLastIssue();
            int currentCount = DBSQLClient.Instance().GetRecordCount();

            // Get the previous.
            SchemeAttributes currentAttris = ReadAttributes(currentIssue);
            if (currentAttris == null)
                return null;

            // Create the attributes.
            SchemeAttributes attributeSet = currentAttris.Clone();

            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in attributeSet.Categories)
            {
                foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Value.Attributes)
                {
                    int value = newStatus.AttributeStates[attri.Key].Value;

                    // Update the states.
                    foreach (SchemeAttributeValueStatus status in attri.Value.ValueStates)
                    {
                        double totalOmission = status.AverageOmission * status.HitCount;                        

                        if (status.ValueRegion.Contains(value))
                        {
                            status.HitCount ++;
                            status.ImmediateOmission = 0;
                        }
                        else
                        {
                            status.ImmediateOmission++;
                            totalOmission++;
                        }

                        if (status.ImmediateOmission > status.MaxOmission)
                            status.MaxOmission = status.ImmediateOmission;

                        status.AverageOmission = status.HitCount == 0 ? -1.0 : totalOmission / status.HitCount;
                        status.HitProbability = ((double)status.HitCount * 100) / (currentCount + 1);

                        if (status.AverageOmission <= 0.0 || status.MaxOmission == status.AverageOmission)
                            status.ProtentialEnergy = 0;
                        else
                        {
                            //status.ProtentialEnergy = (status.ImmediateOmission - status.AverageOmission) / (status.MaxOmission - status.AverageOmission);
                            status.ProtentialEnergy = status.ImmediateOmission / status.AverageOmission;
                        }
                    }
                }
            }

            return attributeSet;
        }

        private Status ExtractStatus(Omission omssion, Attribute attribute)
        {
            Status status = new Status();

            // omission.
            Type numStateType = typeof(Omission);
            System.Reflection.PropertyInfo[] properties = numStateType.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                string name = property.Name;

                if (name.Contains("Red_"))
                {
                    int num = Convert.ToInt32(name.Substring(4, 2));
                    if (status.RedNumStates[num - 1] == null)
                    {
                        status.RedNumStates[num - 1] = new NumberState();
                    }

                    if (name.Contains("_Hit"))
                    {
                        status.RedNumStates[num - 1].HitCount = Convert.ToInt32(property.GetValue(omssion).ToString());
                    }
                    else if (name.Contains("_Omission"))
                    {
                        status.RedNumStates[num - 1].Omission = Convert.ToInt32(property.GetValue(omssion).ToString());
                    }
                    else // _Temperature
                    {
                        status.RedNumStates[num - 1].Temperature = Convert.ToInt32(property.GetValue(omssion).ToString());
                    }
                }
                else if (name != "Issue") // blue
                {
                    int num = Convert.ToInt32(name.Substring(5, 2));

                    if (status.BlueNumStates[num - 1] == null)
                    {
                        status.BlueNumStates[num - 1] = new NumberState();
                    }

                    if (name.Contains("_Hit"))
                    {
                        status.BlueNumStates[num - 1].HitCount = Convert.ToInt32(property.GetValue(omssion).ToString());
                    }
                    else if (name.Contains("_Omission"))
                    {
                        status.BlueNumStates[num - 1].Omission = Convert.ToInt32(property.GetValue(omssion).ToString());
                    }
                    else // _Temperature
                    {
                        status.BlueNumStates[num - 1].Temperature = Convert.ToInt32(property.GetValue(omssion).ToString());
                    }
                }
            }

            // attributes
            Type attributeType = typeof(Attribute);
            System.Reflection.PropertyInfo[] properties1 = attributeType.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties1)
            {
                if (property.Name != "Issue")
                {
                    if (status.AttributeStates.ContainsKey(property.Name))
                    {
                        status.AttributeStates[property.Name].Value = Convert.ToInt32(property.GetValue(attribute).ToString());
                    }
                    else
                    {
                        Console.WriteLine("not value " + property.Name);
                    }
                }
            }

            return status;
        }

        private SchemeAttributes NewAttributes()
        {
            if (AttributeUtil.GetAttributesTemplate() == null)
            {
                CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", "AttributesTemplate.xml");

                string text;
                using (var memoryStream = new MemoryStream())
                {
                    blob.DownloadToStream(memoryStream);
                    text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                // parse to a temp object for comparing.
                SchemeAttributes _template = new SchemeAttributes();

                DBXmlDocument xml = new DBXmlDocument();
                xml.Load(text);

                DBXmlNode root = xml.Root();
                _template.ReadFromTemplate(root);

                AttributeUtil.SetAttributesTemplate(_template);
            }

            return AttributeUtil.GetAttributesTemplate().Clone();
        }

        private SchemeAttributes ReadAttributes(int issue)
        {
            // get the attributes file for this issue.
            string attriString = DBCloudStorageClient.Instance().ReadBlobAsString("data-attributes", issue.ToString() + ".xml");
            if (string.IsNullOrEmpty(attriString))
                return null;

            DBXmlDocument xml = new DBXmlDocument();
            xml.Load(attriString);

            SchemeAttributes attriSet = NewAttributes();
            attriSet.ReadValueFromXml(xml.Root());

            return attriSet;
        }

        private DBLotteryModel buildLotteryModel(Basic basic, Detail detail)
        {
            DBLotteryModel lot = new DBLotteryModel();
            lot.Issue = basic.Issue;
            lot.Scheme = basic.Red1.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red2.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red3.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red4.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red5.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red6.ToString().PadLeft(2, '0');
            lot.Scheme += "+" + basic.Blue.ToString().PadLeft(2, '0');

            lot.Bet = detail.BetAmount;
            lot.Pool = detail.PoolAmount;
            lot.Details = detail.More;
            lot.Date = detail.Date;

            lot.Bonus = new List<int>
            {
                detail.Prize1Count,
                Convert.ToInt32(detail.Prize1Bonus),
                detail.Prize2Count,
                Convert.ToInt32(detail.Prize2Bonus),                
                detail.Prize3Count,
                3000,
                detail.Prize4Count,
                200,
                detail.Prize5Count,
                10,
                detail.Prize6Count,
                5,
            };

            return lot;
        }

        private bool ExecuteAction(string container, string actionFileName)
        {
            if (actionFileName.Substring(actionFileName.Length - 4) == ".xml")
            {
                // copy the file to release folder
                CloudBlockBlob srcBlob = DBCloudStorageClient.Instance().GetBlockBlob(container, actionFileName);
                CloudBlockBlob targetBlob = DBCloudStorageClient.Instance().GetBlockBlob("dblotterydata", actionFileName);

                string result = targetBlob.StartCopy(srcBlob);

                // delete the source file.
                srcBlob.Delete();

                return true;
            }
            else if (actionFileName.Substring(actionFileName.Length - 4) == ".sql")
            {
                // execute the sql
                string sqlLines = DBCloudStorageClient.Instance().ReadBlobAsString(container, actionFileName);
                if (!string.IsNullOrEmpty(sqlLines))
                {
                    string res = DBSQLClient.Instance().ExecuteSqlQueries(sqlLines);
                    if (res == "Success")
                    {
                        // delete the source file.
                        CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob(container, actionFileName);
                        blob.Delete();

                        return true;
                    }
                }

            }

            return false;
        }

        // SQL ...
        //

        private static string sqlQuery_addBasic = "INSERT INTO [dbo].[Basic] ([Issue], [Red1], [Red2], [Red3], [Red4], [Red5], [Red6], [Blue]) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})";
        private static string sqlQuery_addDetail = "INSERT INTO [dbo].[Detail] ([Issue], [Date], [Prize1Bonus], [Prize2Bonus], [Prize1Count], [Prize2Count], [Prize3Count], [Prize4Count], [Prize5Count], [Prize6Count], [BetAmount], [PoolAmount], [More]) VALUES ({0}, N'{1}', CAST({2} AS Money), CAST({3} AS Money), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, N'{12}')";
        private static string sqlQuery_addOmission = "";
        private static string sqlQuery_addAttributes = "";

        private string SqlDeleteDetail(int issue)
        {
            return "DELETE from [dbo].[Detail] WHERE [Issue]='" + issue.ToString() + "'";
        }

        private string SqlAddAttributes()
        {
            if (sqlQuery_addAttributes == "")
            {
                sqlQuery_addAttributes = "INSERT INTO [dbo].[Attributes] ([Issue], ";

                string columns = "", rows = "";
                List<string> keys = AttributeUtil.AttributeKeys();
                int index = 0;
                rows += "{" + index.ToString() + "}, ";
                index++;

                foreach (string key in keys)
                {
                    columns += "[" + key + "], ";
                    rows += "{" + index.ToString() + "}, ";
                    index++;
                }

                sqlQuery_addAttributes += columns.TrimEnd(',', ' ') + ")";
                sqlQuery_addAttributes += " VALUES (" + rows.TrimEnd(',', ' ') + ")";
            }

            return sqlQuery_addAttributes;
        }

        private string SqlAddOmission()
        {
            if (sqlQuery_addOmission == "")
            {
                sqlQuery_addOmission = "INSERT INTO [dbo].[Omission] ([Issue], ";

                string columns = "", rows = "";
                int index = 0;
                rows += "{" + index.ToString() + "}, ";
                index++;

                for (int i = 1; i <= 33; ++i)
                {
                    columns += "[" + "Red_" + i.ToString().PadLeft(2, '0') + "_Hit" + "], ";
                    columns += "[" + "Red_" + i.ToString().PadLeft(2, '0') + "_Omission" + "], ";
                    columns += "[" + "Red_" + i.ToString().PadLeft(2, '0') + "_Temperature" + "], ";

                    rows += "{" + index.ToString() + "}, ";
                    index++;
                    rows += "{" + index.ToString() + "}, ";
                    index++;
                    rows += "{" + index.ToString() + "}, ";
                    index++;
                }

                for (int i = 1; i <= 16; ++i)
                {
                    columns += "[" + "Blue_" + i.ToString().PadLeft(2, '0') + "_Hit" + "], ";
                    columns += "[" + "Blue_" + i.ToString().PadLeft(2, '0') + "_Omission" + "], ";
                    columns += "[" + "Blue_" + i.ToString().PadLeft(2, '0') + "_Temperature" + "], ";

                    rows += "{" + index.ToString() + "}, ";
                    index++;
                    rows += "{" + index.ToString() + "}, ";
                    index++;
                    rows += "{" + index.ToString() + "}, ";
                    index++;
                }

                sqlQuery_addOmission += columns.TrimEnd(',', ' ') + ")";
                sqlQuery_addOmission += " VALUES (" + rows.TrimEnd(',', ' ') + ")";
            }

            return sqlQuery_addOmission;
        }

        private string GenerateSQLQueryForNewRelease(int issue, Scheme scheme, Detail detail, Status status)
        {
            string query = "";

            int[] reds = scheme.GetRedNums();

            object[] basic_values = new object[] 
            {                
                issue,
                reds[0], 
                reds[1], 
                reds[2], 
                reds[3], 
                reds[4], 
                reds[5],
                scheme.Blue
            };
            query += string.Format(sqlQuery_addBasic, basic_values) + "\n";

            object[] omission_values = new object[148];
            omission_values[0] = issue;

            int index = 1;
            for (int i = 1; i <= 33; ++i)
            {
                omission_values[index++] = status.RedNumStates[i - 1].HitCount;
                omission_values[index++] = status.RedNumStates[i - 1].Omission;
                omission_values[index++] = status.RedNumStates[i - 1].Temperature;
            }

            for (int i = 1; i <= 16; ++i)
            {
                omission_values[index++] = status.BlueNumStates[i - 1].HitCount;
                omission_values[index++] = status.BlueNumStates[i - 1].Omission;
                omission_values[index++] = status.BlueNumStates[i - 1].Temperature;
            }
            query += string.Format(SqlAddOmission(), omission_values) + "\n";


            index = 1;
            int attriCount = status.AttributeStates.Count;
            object[] attribute_values = new object[attriCount + 1];
            attribute_values[0] = issue;
            foreach (KeyValuePair<string, AttributeState> state in status.AttributeStates)
            {
                attribute_values[index++] = state.Value.Value;
            }

            query += string.Format(SqlAddAttributes(), attribute_values) + "\n";

            query += GenerateSQLQueryForDetail(detail, false); ;

            return query;
        }

        private string GenerateSQLQueryForDetail(Detail detail, bool forUpdate)
        {
            // delete the existing detail row. For now, just allow modify the details.
            string query = forUpdate? SqlDeleteDetail(detail.Issue) + "\n" : "";

            object[] detail_values = new object[]
            {
                detail.Issue,
                detail.Date.ToString("yyyy-MM-dd"),
                detail.Prize1Bonus,
                detail.Prize2Bonus,
                detail.Prize1Count,
                detail.Prize2Count,
                detail.Prize3Count,
                detail.Prize4Count,
                detail.Prize5Count,
                detail.Prize6Count,
                detail.BetAmount,
                detail.PoolAmount,
                detail.More
            };
            query += string.Format(sqlQuery_addDetail, detail_values);

            return query;
        }
    }
}
