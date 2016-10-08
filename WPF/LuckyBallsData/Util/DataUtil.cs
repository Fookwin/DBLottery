using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Data;
using System.IO;
using LuckyBallsData.Statistics;

namespace LuckyBallsData.Util
{
    public class DataUtil
    {
        private const int sCurrentVersion = 2;

        public static int Version
        {
            get
            {
                return sCurrentVersion;
            }
        }

        #region History_Data
        public static bool ReadFromXml(History history, string source)
        {
            if (source == "")
                return false;

            DBXmlDocument xml = new DBXmlDocument();

            try
            {
                xml.Load(source);
            }
            catch (Exception e)
            {
                string str = e.Message;
            }

            DBXmlNode root = xml.Root();

            // update version information.
            int version = Convert.ToInt32(root.GetAttribute("Version"));

            history.ReadFromXml(root, History.InsertOption.eReplaceExists);            
  
            history.SetVersionInfo(sCurrentVersion);

            return true;
        }

        public static bool SaveToXml(History history, string target)
        {
            DBXmlDocument xml = new DBXmlDocument();
            DBXmlNode root = xml.AddRoot("History");

            if (!history.SaveToXml(ref root))
                return false;

            // save the version information.
            root.SetAttribute("Version", history.Version.ToString());
            root.SetAttribute("LastIssue", history.LastIssue.ToString());

            xml.Save(target);

            return true;
        }       

        public static Lottery AddLottory(History history, int issue, Scheme nums, bool bSyncDetailsFromWeb)
        {
            int count = history.Count;

            Lottery lot = new Lottery(history, issue, nums);

            if (bSyncDetailsFromWeb)
            {
                DateTime date = DateTime.Today;
                int[] reds = new int[6];
                int blue = 0;
                int[,] bonus = new int[6, 2];
                int betAmount = 0;
                int poolAmount = 0;
                string more = "None";

                try
                {
                    // sync more data from web.
                    if (!SyncFromWeb(issue, ref date, ref reds, ref blue, ref bonus, ref betAmount, ref poolAmount, ref more))
                    {
                        more = "Error: Fail to read data from web.";
                    }
                }
                catch (Exception e)
                {
                    more = "Error: " + e.Message;
                }

                lot.SetDetail(date, bonus, betAmount, poolAmount, more);
            }

            if (count > 0)
            {
                if (!AnalyzeLotteryStatus(lot, history.LastLottery, count - 1))
                    return null;
            }
            

            return history.AddLottery(lot);
        }

        public static bool SyncFromWeb(int issue, ref DateTime date, ref int[] reds, ref int blue,
            ref int[,] bonus, ref int betAmount, ref int poolAmount, ref string more)
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

                date = DateTime.Parse(output);

                // red numbers.
                token = "蓝色球";
                currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                if (currentIndex < token.Length)
                    return false;
                searchIndex = currentIndex;

                for (int i = 0; i < 6; ++i)
                {
                    token = "<font color=red>";
                    currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                    if (currentIndex < token.Length)
                        return false;

                    readCount = 2;
                    output = webData.Substring(currentIndex, readCount);
                    searchIndex = currentIndex + readCount;

                    reds[i] = Convert.ToInt32(output.TrimStart('0'));
                }

                // Blue
                token = "<font color=blue>";
                currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                if (currentIndex < token.Length)
                    return false;

                readCount = 2;
                output = webData.Substring(currentIndex, readCount);
                searchIndex = currentIndex + readCount;

                blue = Convert.ToInt32(output.TrimStart('0'));

                // bonus
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
                    bonus[i / 2, i % 2] = Convert.ToInt32(output.Replace(",", ""));
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

                betAmount = Convert.ToInt32(output.Replace(",", ""));

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

                poolAmount = Convert.ToInt32(output.Replace(",", ""));

                // comments.
                currentIndex = searchIndex + 5;
                searchIndex = currentIndex;

                // find the end.
                //token = "本期红色球出球顺序";
                //endIndex = webData.IndexOf(token, searchIndex);
                //if (endIndex < 0)
                //{
                //    token = "本期红球出球顺序";
                //    endIndex = webData.IndexOf(token, searchIndex);
                //    if (endIndex < 0)
                //    {
                //        token = "红球出球顺序";
                //        endIndex = webData.IndexOf(token, searchIndex);
                //        if (endIndex < 0)
                //        {
                //            token = "出球顺序";
                //            endIndex = webData.IndexOf(token, searchIndex);
                //            if (endIndex < 0)
                //            {
                token = "</td></tr></table>";
                endIndex = webData.IndexOf(token, searchIndex);
                //            }
                //        }
                //    }
                //}

                if (endIndex > 0)
                {
                    readCount = endIndex - currentIndex;
                    output = webData.Substring(currentIndex, readCount).Replace("\r", "").Replace("\n", "").Replace(" ", "");
                    searchIndex = currentIndex + readCount;

                    if (output != "")
                        more = output;
                }

                //// order.
                //currentIndex = webData.IndexOf(token, searchIndex) + token.Length;
                //if (currentIndex < token.Length)
                //return false;

                //searchIndex = currentIndex;

                //// find the end.
                //token = "元";
                //endIndex = webData.IndexOf(token, searchIndex) - 1;

                //readCount = endIndex - currentIndex;
                //output = webData.Substring(currentIndex, readCount);
                //searchIndex = currentIndex + readCount;

                //int betAmount = Convert.ToInt32(output);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static bool AnalyzeLotteryStatus(Lottery lot, Lottery previous, int previousIssueIndex)
        {
            Status curStatus = lot.Status;

            if (previous != null)
            {
                Status preStatus = previous.Status;

                // Red numbers...
                for (int i = 1; i <= 33; i++)
                {
                    if (curStatus.RedNumStates[i - 1] == null)
                        curStatus.RedNumStates[i - 1] = new NumberState();

                    if (lot.Scheme.Contains(i))
                    {
                        curStatus.RedNumStates[i - 1].HitCount = preStatus.RedNumStates[i - 1].HitCount + 1;
                        curStatus.RedNumStates[i - 1].Omission = 0;                        
                    }
                    else
                    {
                        curStatus.RedNumStates[i - 1].HitCount = preStatus.RedNumStates[i - 1].HitCount;
                        curStatus.RedNumStates[i - 1].Omission = preStatus.RedNumStates[i - 1].Omission + 1;
                    }

                    if (previousIssueIndex - 9 >= 0)
                    {
                        Lottery refLot = DataManageBase.Instance().History.Lotteries[previousIssueIndex - 9];
                        curStatus.RedNumStates[i - 1].Temperature = curStatus.RedNumStates[i - 1].HitCount - refLot.Status.RedNumStates[i - 1].HitCount;
                    }
                    else
                    {
                        curStatus.RedNumStates[i - 1].Temperature = 5;
                    }
                }

                // Blue numbers.
                for (int i = 1; i <= 16; i++)
                {
                    if (curStatus.BlueNumStates[i - 1] == null)
                        curStatus.BlueNumStates[i - 1] = new NumberState();

                    if (lot.Scheme.Blue == i)
                    {
                        curStatus.BlueNumStates[i - 1].HitCount = preStatus.BlueNumStates[i - 1].HitCount + 1;
                        curStatus.BlueNumStates[i - 1].Omission = 0;
                    }
                    else
                    {
                        curStatus.BlueNumStates[i - 1].HitCount = preStatus.BlueNumStates[i - 1].HitCount;
                        curStatus.BlueNumStates[i - 1].Omission = preStatus.BlueNumStates[i - 1].Omission + 1;
                    }

                    if (previousIssueIndex - 9 >= 0)
                    {
                        Lottery refLot = DataManageBase.Instance().History.Lotteries[previousIssueIndex - 9];
                        curStatus.BlueNumStates[i - 1].Temperature = curStatus.BlueNumStates[i - 1].HitCount - refLot.Status.BlueNumStates[i - 1].HitCount;
                    }
                    else
                    {
                        curStatus.BlueNumStates[i - 1].Temperature = 5;
                    }
                }
            }

            // Attribute values.
            foreach (KeyValuePair<string, AttributeState> state in curStatus.AttributeStates)
            {
                state.Value.Value = lot.Scheme.Attribute(state.Key, previousIssueIndex);
            }

            return true;
        }

        public static void CalculateNextReleaseNumberAndTime(int currentIssue, DateTime releaseDate, ref int nextIssue, 
            ref DateTime sellOfftime, ref DateTime nextReleaseDate)
        {
            int lastIssue = currentIssue;

            nextIssue = lastIssue + 1;
            if (releaseDate.Month == 12 && (releaseDate.Day > 29 || (releaseDate.Day == 29 && releaseDate.DayOfWeek == DayOfWeek.Thursday)))
            {
                nextIssue = (releaseDate.Year + 1) * 1000 + 1;
            }

            DateTime nextDate = new DateTime(releaseDate.Year, releaseDate.Month, releaseDate.Day);
            sellOfftime = nextDate.AddDays(nextDate.DayOfWeek == DayOfWeek.Thursday ? 3 : 2).AddHours(19.5);
            nextReleaseDate = sellOfftime.AddHours(2);
        }

        public static void AppendLoginInfo(string item, string strLogFile)
        {
            using (StreamWriter writer = File.AppendText(strLogFile))
            {
                writer.NewLine = item;
                writer.WriteLine();
            }
        }

        #endregion 

        #region SQLHelper

        private static string sqlQuery_addBasic = "INSERT INTO [dbo].[Basic] ([Issue], [Red1], [Red2], [Red3], [Red4], [Red5], [Red6], [Blue]) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})";
        private static string sqlQuery_addDetail = "INSERT INTO [dbo].[Detail] ([Issue], [Date], [Prize1Bonus], [Prize2Bonus], [Prize1Count], [Prize2Count], [Prize3Count], [Prize4Count], [Prize5Count], [Prize6Count], [BetAmount], [PoolAmount], [More]) VALUES ({0}, N'{1}', CAST({2} AS Money), CAST({3} AS Money), {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, N'{12}')";
        private static string sqlQuery_addOmission = "";
        private static string sqlQuery_addAttributes = "";

        private static string SqlDeleteDetail(int issue)
        {
            return "DELETE from [dbo].[Detail] WHERE [Issue]='" + issue.ToString() + "'";
        }

        private static string SqlAddAttributes()
        {
            if (sqlQuery_addAttributes == "")
            {
                sqlQuery_addAttributes = "INSERT INTO [dbo].[Attributes] ([Issue], ";

                string columns = "", rows = "";
                List<string> keys = AttributeUtil.AttributeKeys();
                int index = 0;
                rows += "{" + index.ToString() + "}, ";
                index++;

                foreach(string key in keys)
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

        private static string SqlAddOmission()
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

        public static void LatestIssueInCloud(ref int version, ref int issue)
        {
            LuckyBallsData.ServiceReference1.SqlServiceClient client = new LuckyBallsData.ServiceReference1.SqlServiceClient();

            // Check the version in cloud to determine if an update need to be performed.
            Int64 revisionCloud = 0;
            client.GetVersion(ref version, ref revisionCloud, ref issue);
        }  

        public static string GetSQLQuery(Lottery lot, bool modifying)
        {
            string query = "";

            if (!modifying)
            {
                int[] reds = lot.Scheme.GetRedNums();

                object[] basic_values = new object[] 
                {                
                    lot.Issue,
                    reds[0], 
                    reds[1], 
                    reds[2], 
                    reds[3], 
                    reds[4], 
                    reds[5],
                    lot.Scheme.Blue
                };
                query += string.Format(sqlQuery_addBasic, basic_values) + "\n";

                object[] omission_values = new object[148];
                omission_values[0] = lot.Issue;

                int index = 1;
                for (int i = 1; i <= 33; ++i)
                {
                    omission_values[index++] = lot.Status.RedNumStates[i - 1].HitCount;
                    omission_values[index++] = lot.Status.RedNumStates[i - 1].Omission;
                    omission_values[index++] = lot.Status.RedNumStates[i - 1].Temperature;
                }

                for (int i = 1; i <= 16; ++i)
                {
                    omission_values[index++] = lot.Status.BlueNumStates[i - 1].HitCount;
                    omission_values[index++] = lot.Status.BlueNumStates[i - 1].Omission;
                    omission_values[index++] = lot.Status.BlueNumStates[i - 1].Temperature;
                }
                query += string.Format(SqlAddOmission(), omission_values) + "\n";


                index = 1;
                int attriCount = lot.Status.AttributeStates.Count;
                object[] attribute_values = new object[attriCount + 1];
                attribute_values[0] = lot.Issue;
                foreach (KeyValuePair<string, AttributeState> state in lot.Status.AttributeStates)
                {
                    attribute_values[index++] = state.Value.Value;
                }

                query += string.Format(SqlAddAttributes(), attribute_values) + "\n";
            }
            else
            {
                // delete the existing detail row. For now, just allow modify the details.
                query += SqlDeleteDetail(lot.Issue) + "\n";
            }

            object[] detail_values = new object[]
            {
                lot.Issue,
                lot.Date.ToString("yyyy-MM-dd"),
                lot.BonusMoney(1),
                lot.BonusMoney(2),
                lot.BonusAmount(1),
                lot.BonusAmount(2),
                lot.BonusAmount(3),
                lot.BonusAmount(4),
                lot.BonusAmount(5),
                lot.BonusAmount(6),
                lot.BetAmount,
                lot.PoolAmount,
                lot.MoreInfo
            };
            query += string.Format(sqlQuery_addDetail, detail_values) + "\n";

            return query;
        }

        #endregion

        #region Obsolete_Code
        //////////////////////////////////////////////////////LEGACY CODE/////////////////////////////////////////////////////

        public static double Test(History history)
        {
            const int testCount = 10000;
            Random[] testers = new Random[testCount];
            int[] behaviors = new int[testCount];

            for (int i = 1; i <= testCount; ++i)
            {
                testers[i - 1] = new Random(i);
            }

            const int testFrom = 80;
            int iHit = 0;
            int believeWho = -1;
            int index = 0;
            foreach (Lottery lot in history.Lotteries)
            {
                // Get test num.
                int worst = 1000000, who = 0;
                int[] blues = new int[testCount];
                for (int i = 0; i < testCount; ++i)
                {
                    blues[i] = testers[i].Next(1, 16);
                    if (blues[i] == lot.Scheme.Blue)
                    {
                        behaviors[i]++;
                    }

                    if (behaviors[i] < worst)
                    {
                        worst = behaviors[i];
                        who = i;
                    }
                }

                if (++index >= testFrom)
                {
                    //if (believeWho < 0)
                    //{
                    //    believeWho = who;
                    //    continue;
                    //}

                    if (blues[believeWho] == lot.Scheme.Blue)
                    {
                        iHit++;
                        //believeWho = who;
                    }
                }

                believeWho = who;
            }

            return (double)iHit / (history.Count - testFrom);
        }

        public static bool Save(History history, string strFullFileName)
        {
            // Get data in string array.
            string data = "";

            int index = 1, count = history.Lotteries.Count;
            foreach (Lottery result in history.Lotteries)
            {
                string line = result.Issue.ToString() + " ";
                for (int i = 0; i < 6; ++i)
                    line += result.Scheme.Red(i).ToString().PadLeft(2, '0') + " ";

                line += result.Scheme.Blue.ToString().PadLeft(2, '0');

                data += line;
                if (index ++ != count)
                 data += "\r\n";
            }

            File.WriteAllText(strFullFileName, data);

            return false;
        }
       
#endregion
    }
}
