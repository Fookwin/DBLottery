using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using LuckyBallsData.Statistics;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;
using LuckyBallsData;
using LuckyBallsServer.Data;
using LuckyBallsData.Management;

namespace LuckyBallsServer
{
    public struct FilterOption
    {
        public double ProtentialPropLowLimit;
        public double HitPropLowLimit;
    }

    public enum NotificationTemplateEnum
    {
        Release = 0,
        Detail = 1,
        Recommendation = 2
    }

    class UncommittedItem
    {
        public System.Windows.Media.Color HighlightColor
        {
            get
            {
                if (SavedInCloud)
                    return System.Windows.Media.Colors.Green;
                else if (SavedInLocal)
                    return System.Windows.Media.Colors.Blue;
                else
                    return System.Windows.Media.Colors.Red;
            }
        }

        public bool SavedInLocal
        {
            get;
            set;
        }

        public bool SavedInCloud
        {
            get;
            set;
        }

        public int Issue
        {
            get;
            set;
        }
        public bool Dirty
        {
            get;
            set;
        }

        public SchemeAttributes Attributes
        {
            get;
            set;
        }

        public Lottery Lottery
        {
            get;
            set;
        }

        public ReleaseInfo ReleaseInfo
        {
            get;
            set;
        }
    }

    class LBDataManager : LuckyBallsData.DataManageBase
    {
        private AttributeManager _AttributeMgr = null;
        private List<UncommittedItem> _pendingItems = new List<UncommittedItem>();
        private int _issueInLocal = -1;
        private int _issueInCloud = -1;
        private int _issueLatest = -1;
        private int _nextIssue = -1;
        private DataVersion _dataVersion = new DataVersion();
        private HelpBuilder _helpBuilder = null;

        #region Get Methods
        private LBDataManager()
        {
        }

        public int IssueInLocal
        {
            get
            {
                return _issueInLocal;
            }
        }

        public int IssueInCloud
        {
            get
            {
                return _issueInCloud;
            }
        }

        public int LatestIssue
        {
            get
            {
                return _issueLatest;
            }
        }

        public int NextIssue
        {
            get
            {
                return _nextIssue;
            }
        }

        public DataVersion DataVersion
        {
            get
            {
                return _dataVersion;
            }
        }

        public History GetHistory()
        {
            if (_History == null)
            {
                _History = new History();

                LuckyBallsData.Util.DataUtil.ReadFromXml(_History, "Z:/History.xml");
            }

            return _History;
        }

        public AttributeManager GetAttriMgr()
        {
            return _AttributeMgr;
        }

        static public LBDataManager GetDataMgr()
        {
            if (_Instance == null)
            {
                LBDataManager mgr = new LBDataManager();
                _Instance = mgr;
                mgr.Init();
            }

            return _Instance as LBDataManager;
        }

        public List<string> GetIssueNumbers()
        {
            List<string> result = new List<string>();

            History history = GetHistory();

            foreach (Lottery lot in history.Lotteries)
            {
                result.Add(lot.Issue.ToString());
            }

            return result;
        }

        public List<UncommittedItem> PendingIssues
        {
            get
            {
                return _pendingItems;
            }            
        }

        public DataTable QueryHistory(int countToShow)
        {
            DataTable table = new DataTable();
            Type intType = System.Type.GetType("System.Int32");

            table.Columns.Add(CreateColumn("Index", intType, true, false));
            table.Columns.Add(CreateColumn("Issue", intType, true, false));

            for (int i = 1; i <= 6; ++i)
            {
                table.Columns.Add(CreateColumn("Red" + i.ToString(), intType, false, false));
            }

            table.Columns.Add(CreateColumn("Blue", intType, false, false));

            table.Columns.Add(CreateColumn("Continuity", intType, false, false));
            table.Columns.Add(CreateColumn("EvenCount", intType, false, false));
            table.Columns.Add(CreateColumn("PrimeNumCount", intType, false, false));
            table.Columns.Add(CreateColumn("SmallNumCount", intType, false, false));
            table.Columns.Add(CreateColumn("Sum", intType, false, false));
            table.Columns.Add(CreateColumn("TotalOmission", intType, false, false));
            table.Columns.Add(CreateColumn("RepeatPreivous", intType, false, false));

            for (int i = 1; i <= 33; ++i)
            {
                table.Columns.Add(CreateColumn("Mis_Red" + i.ToString(), intType, false, false));
            }

            for (int i = 1; i <= 16; ++i)
            {
                table.Columns.Add(CreateColumn("Mis_Blue" + i.ToString(), intType, false, false));
            }

            DataRow row;
            int iStart = countToShow < 0 ? 0 : _History.Lotteries.Count - countToShow;
            int index = 0;
            foreach (Lottery lot in _History.Lotteries)
            {
                if (index++ < iStart)
                    continue;

                row = table.NewRow();
                row["Index"] = index;
                row["Issue"] = lot.Issue;
                row["Red1"] = lot.Scheme.Red(0);
                row["Red2"] = lot.Scheme.Red(1);
                row["Red3"] = lot.Scheme.Red(2);
                row["Red4"] = lot.Scheme.Red(3);
                row["Red5"] = lot.Scheme.Red(4);
                row["Red6"] = lot.Scheme.Red(5);
                row["Blue"] = lot.Scheme.Blue;

                for (int i = 1; i <= 33; ++i)
                {
                    row["Mis_Red" + i.ToString()] = lot.Status.RedNumStates[i - 1].Omission;
                }

                row["Mis_Red" + lot.Scheme.Red(0).ToString()] = lot.Scheme.Red(0);
                row["Mis_Red" + lot.Scheme.Red(1).ToString()] = lot.Scheme.Red(1);
                row["Mis_Red" + lot.Scheme.Red(2).ToString()] = lot.Scheme.Red(2);
                row["Mis_Red" + lot.Scheme.Red(3).ToString()] = lot.Scheme.Red(3);
                row["Mis_Red" + lot.Scheme.Red(4).ToString()] = lot.Scheme.Red(4);
                row["Mis_Red" + lot.Scheme.Red(5).ToString()] = lot.Scheme.Red(5);

                for (int i = 1; i <= 16; ++i)
                {
                    row["Mis_Blue" + i.ToString()] = lot.Status.BlueNumStates[i - 1].Omission;
                }
                row["Mis_Blue" + lot.Scheme.Blue.ToString()] = lot.Scheme.Blue;

                table.Rows.Add(row);
            }

            return table;
        }

        #endregion

        #region Attributes Methods
        public List<SchemeAttributeCategory> GetAttributeCatetories(int issue)
        {
            List<SchemeAttributeCategory> cats = new List<SchemeAttributeCategory>();

            SchemeAttributes last = GetAttriMgr().Attributes(issue);
            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in last.Categories)
            {
                cats.Add(cat.Value);
            }

            return cats;
        }

        public List<SchemeAttributeValueStatus> GetAttributeValueStates(int issue, FilterOption filters)
        {
            List<SchemeAttributeValueStatus> result = new List<SchemeAttributeValueStatus>();

            SchemeAttributes last = GetAttriMgr().Attributes(issue);
            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in last.Categories)
            {
                foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Value.Attributes)
                {
                    foreach (SchemeAttributeValueStatus state in attri.Value.ValueStates)
                    {

                        if (state.HitProbability >= filters.HitPropLowLimit &&
                            state.ProtentialEnergy >= filters.ProtentialPropLowLimit)
                        {
                            result.Add(state);
                        }
                    }
                }
            }

            return result;
        }

        public DataTable GetState(SchemeAttributeValueStatus state)
        {
            DataTable table = BuildAttributeTable();
            AddHeader(ref table, state.Parent.DisplayName);
            AddRow(ref table, state);
            return table;
        }

        public DataTable GetAttributesInCategory(SchemeAttributeCategory cat, FilterOption? filters)
        {
            DataTable table = BuildAttributeTable();

            foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Attributes)
            {
                AddRows(ref table, attri.Value);
            }

            return table;
        }

        private DataTable BuildAttributeTable()
        {
            DataTable table = new DataTable();

            Type intType = System.Type.GetType("System.Int32");
            Type stringType = System.Type.GetType("System.String");
            Type doubleType = System.Type.GetType("System.Double");
            table.Columns.Add(CreateColumn("属性名称", stringType, false, false));
            table.Columns.Add(CreateColumn("出现次数", stringType, false, false));
            table.Columns.Add(CreateColumn("出现概率", stringType, false, false));
            table.Columns.Add(CreateColumn("理论概率", stringType, false, false));
            table.Columns.Add(CreateColumn("平均遗漏", stringType, false, false));
            table.Columns.Add(CreateColumn("最大遗漏", stringType, false, false));
            table.Columns.Add(CreateColumn("当前遗漏", stringType, false, false));
            table.Columns.Add(CreateColumn("欲出几率", stringType, false, false));

            return table;
        }

        private void AddRows(ref DataTable table, SchemeAttribute attri)
        {
            // Add a header for this attribute.
            AddHeader(ref table, attri.DisplayName);

            foreach (SchemeAttributeValueStatus state in attri.ValueStates)
            {
                AddRow(ref table, state);
            }
        }

        private void AddHeader(ref DataTable table, string displayName)
        {
            DataRow sep = table.NewRow();
            sep["属性名称"] = displayName;
            sep["出现次数"] = "出现次数";
            sep["出现概率"] = "出现概率";
            sep["理论概率"] = "理论概率";
            sep["平均遗漏"] = "平均遗漏";
            sep["最大遗漏"] = "最大遗漏";
            sep["当前遗漏"] = "当前遗漏";
            sep["欲出几率"] = "欲出几率";
            table.Rows.Add(sep);
        }

        private void AddRow(ref DataTable table, SchemeAttributeValueStatus state)
        {
            DataRow row = table.NewRow();
            row["属性名称"] = state.ValueExpression;
            row["出现次数"] = state.HitCount.ToString();
            row["出现概率"] = state.HitProbability.ToString("0.00");
            row["理论概率"] = state.IdealProbility > 0.0 ? state.IdealProbility.ToString("0.00") : "-";
            row["平均遗漏"] = state.AverageOmission > 0.0 ? state.AverageOmission.ToString("0.00") : "-";
            row["最大遗漏"] = state.MaxOmission.ToString();
            row["当前遗漏"] = state.ImmediateOmission.ToString();
            row["欲出几率"] = state.ProtentialEnergy >= 0.0 ? state.ProtentialEnergy.ToString("0.00") : "-";
            table.Rows.Add(row);
        }

        #endregion

        #region Update Methods
        private void Init()
        {
            // initialize global members.
            //

            SchemeAttributes template = InitTemplate("Z:/Attributes/AttributesTemplate.xml");
            AttributeUtil.SetAttributesTemplate(template);

            // history...
            //

            History history = GetHistory();

            // attributes
            //

            _AttributeMgr = new AttributeManager("Z:/Attributes/");
            _AttributeMgr.Init();
            
            // issue data.
            //

            _issueInLocal = history.LastIssue;
            _issueLatest = _issueInLocal;
            
            int version = DataUtil.Version;
            DataUtil.LatestIssueInCloud(ref version, ref _issueInCloud);

            // display recent 5 issues.
            List<Lottery> lotteries = GetHistory().Lotteries;
            for (int i = lotteries.Count - 6; i < lotteries.Count; ++ i)
            {
                Lottery lot = lotteries[i];

                bool bSavedInCloud = lot.Issue <= _issueInCloud;

                bool releaseInfoFound = true;
                ReleaseInfo releaseInfo = new ReleaseInfo();
                string messageFile = "Z:/Messages/" + lot.Issue.ToString() + ".xml";
                if (File.Exists(messageFile))
                {
                    DBXmlDocument xml = new DBXmlDocument();
                    xml.Load(messageFile);

                    releaseInfo.Read(xml.Root());
                }
                else
                {
                    int currentIssue = lot.Issue;
                    DateTime releaseDate = lot.Date;

                    int nextIssue = 0;
                    DateTime sellOfftime = DateTime.Now, nextReleaseDate = DateTime.Now;

                    // Get next release info.
                    DataUtil.CalculateNextReleaseNumberAndTime(currentIssue, releaseDate, ref nextIssue, ref sellOfftime, ref nextReleaseDate);

                    releaseInfo.CurrentIssue = currentIssue;
                    releaseInfo.NextIssue = nextIssue;
                    releaseInfo.NextReleaseTime = nextReleaseDate;
                    releaseInfo.SellOffTime = sellOfftime;
                            
                    releaseInfoFound = false;
                }
                        
                UncommittedItem item = new UncommittedItem()
                {
                    Issue = lot.Issue,
                    Dirty = !releaseInfoFound,
                    Lottery = lot,
                    Attributes = _AttributeMgr.Attributes(lot.Issue),
                    ReleaseInfo = releaseInfo,
                    SavedInCloud = bSavedInCloud,
                    SavedInLocal = true
                };

                _pendingItems.Add(item);
            }

            // Read the data version from disk.
            string versionFile = "Z:\\ReleaseData\\Version.xml";
            if (File.Exists(versionFile))
            {
                DBXmlDocument xml = new DBXmlDocument();
                xml.Load(versionFile);

                _dataVersion.ReadFromXml(xml.Root());
            }

            // Set the latest issue.
            _dataVersion.LatestIssue = _issueInLocal;
        }

        private SchemeAttributes InitTemplate(string templateFile)
        {
            SchemeAttributes _template = new SchemeAttributes();

            DBXmlDocument xml = new DBXmlDocument();

            try
            {
                xml.Load(templateFile);
            }
            catch (Exception e)
            {
                string str = e.Message;
            }

            DBXmlNode root = xml.Root();
            _template.ReadFromTemplate(root);

            return _template;
        }

        public void SaveToLocal()
        {
            List<string> sqlLines = new List<string>();

            bool bAnyItemChanged = false;
            foreach (UncommittedItem item in _pendingItems)
            {
                if (item.Dirty)
                {
                    // Save release info.
                    string messageFile = "Z:/Messages/" + item.Issue.ToString() + ".xml";
                    DBXmlDocument xml = new DBXmlDocument();
                    DBXmlNode root = xml.AddRoot("Info");
                    item.ReleaseInfo.Save(ref root);
                    xml.Save(messageFile);

                    // Save attributes
                    if (item.Attributes == null)
                    {
                        item.Attributes = GetAttriMgr().AddAttributesForLottery(item.Issue, item.Lottery.Scheme);
                    }

    
                    // Generate the sql query for updating the lottery, if it has been existing in sql database,
                    // delete the existing.
                    string query = DataUtil.GetSQLQuery(item.Lottery, item.Issue <= _issueInCloud);
                    if (query.Length > 0)
                    {
                        // Add to sql lines.
                        sqlLines.Add(query);
                    }                   

                    item.Dirty = false;

                    bAnyItemChanged = true;
                }
            }

            if (bAnyItemChanged)
            {
                // Save hisgory with xml file.
                LuckyBallsData.Util.DataUtil.SaveToXml(LBDataManager.GetDataMgr().GetHistory(), "Z:/History.xml");

                // Save data for history with txt file.
                LuckyBallsData.Util.DataUtil.Save(LBDataManager.GetDataMgr().GetHistory(), "Z:/History.txt");

                // Save attributes.
                LBDataManager.GetDataMgr().GetAttriMgr().SaveToLocal("Z:/");

                // Prepare the data for cloud servers.
                //

                // SQL query file.
                if (sqlLines.Count > 0)
                {
                    string fileName = "Z:/SQL/" + "Query_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".sql";
                    File.WriteAllLines(fileName, sqlLines);

                    if (File.Exists(fileName))
                        File.Copy(fileName, "Z:/ReleaseData/SQL.sql", true);
                }

                // Latest Attributes.
                GetAttriMgr().SaveLatestAttributes("Z:/ReleaseData/LatestAttributes.xml");

                // Release Information.
                string source = "Z:/Messages/" + GetHistory().LastIssue.ToString() + ".xml";
                if (File.Exists(source))
                {
                    string target = "Z:/ReleaseData/ReleaseInformation.xml";
                    File.Copy(source, target, true);
                }
            }

            // Save the data version.
            string versionFile = "Z:\\ReleaseData\\Version.xml";
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Version");
                _dataVersion.SaveToXml(ref root);

                xml.Save(versionFile);
            }

            // Update the notifications
            UpdateWindowsTileNotification();
        }

        private void UpdateWindowsTileNotification()
        {
            Lottery lot = GetHistory().LastLottery;
            string strIssue = "第" + lot.Issue.ToString() + "期";
            string strScheme = lot.Scheme.ToString();
            string betAmount = lot.BetAmount > 0 ? (lot.BetAmountExp) : "统计中...";
            string poolAmount = lot.PoolAmount > 0 ? (lot.PoolAmountExp) : "统计中...";

            int r_top1 = 0, r_top2 = 0, r_mis1 = 0, r_mis2 = 0;
            for (int i = 1; i <= 33; ++i)
            {
                int omission = lot.Status.RedNumStates[i - 1].Omission;

                if (r_top1 == 0 || omission > r_mis1)
                {
                    int pre = r_top1, preOmission = r_mis1;

                    r_top1 = i;
                    r_mis1 = omission;

                    if (pre > 0)
                    {
                        r_top2 = pre;
                        r_mis2 = preOmission;
                    }
                }
                else if (r_top2 == 0 || omission > r_mis1)
                {
                    r_top2 = i;
                    r_mis2 = omission;
                }
            }

            int b_top = 0, b_mis = 0;
            for (int i = 1; i <= 16; ++i)
            {
                int omission = lot.Status.BlueNumStates[i - 1].Omission;

                if (b_top == 0 || omission > b_mis)
                {
                    b_top = i;
                    b_mis = omission;
                }
            }

            SchemeAttributeValueStatus state1 = null, state2 = null, state3 = null;
            double prop1 = 0.0, prop2 = 0.0, prop3 = 0.0;
            SchemeAttributes last = GetAttriMgr().LatestAttributes();
            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in last.Categories)
            {
                foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Value.Attributes)
                {
                    foreach (SchemeAttributeValueStatus state in attri.Value.ValueStates)
                    {
                        if (state.AverageOmission < 1.0)
                            continue; // skip the attribute happens too frequently.

                        if (state1 == null || prop1 < state.ProtentialEnergy)
                        {
                            if (state2 != null)
                            {
                                state3 = state2;
                                prop3 = prop2;
                            }

                            if (state1 != null)
                            {
                                state2 = state1;
                                prop2 = prop1;
                            }

                            state1 = state;
                            prop1 = state.ProtentialEnergy;
                        }
                        else if (state2 == null || prop2 < state.ProtentialEnergy)
                        {
                            if (state2 != null)
                            {
                                state3 = state2;
                                prop3 = prop2;
                            }

                            state2 = state;
                            prop2 = state.ProtentialEnergy;
                        }
                        else if (state3 == null || prop3 < state.ProtentialEnergy)
                        {
                            state3 = state;
                            prop3 = state.ProtentialEnergy;
                        }
                    }
                }
            }

            #region Build Tiles - windows
            // Generate Windows tile notification - default tile
            {
                //<tile>
                //  <visual>
                //    <binding template="TileWideImageAndText02">
                //      <image id="1" src="https://dbdatastorage.blob.core.windows.net/dbnotification/WideLogo.png" alt="alt text"/>
                //      <text id="1">最新开奖 第2013110期</text>
                //      <text id="2">红:15 17 18 21 29 32 蓝:13</text>
                //    </binding>  
                //    <binding template="TileSquareImage">
                //      <image id="1" src="https://dbdatastorage.blob.core.windows.net/dbnotification/Logo.png" alt="alt text"/>
                //    </binding>
                //  </visual>
                //</tile>
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode toastNode = xml.AddRoot("tile");
                DBXmlNode visualNode = toastNode.AddChild("visual");

                {
                    DBXmlNode bindingNode = visualNode.AddChild("binding");

                    bindingNode.SetAttribute("template", "TileWideImageAndText02");

                    DBXmlNode imageNode = bindingNode.AddChild("image");
                    imageNode.SetAttribute("id", "1");
                    imageNode.SetAttribute("src", "https://dbdatastorage.blob.core.windows.net/dbnotification/WideLogo.png");
                    imageNode.SetAttribute("alt", "alt text");

                    DBXmlNode text1Node = bindingNode.AddChild("text");
                    text1Node.SetAttribute("id", "1");
                    text1Node.SetValue("最新开奖 " + strIssue);

                    DBXmlNode text2Node = bindingNode.AddChild("text");
                    text2Node.SetAttribute("id", "2");
                    text2Node.SetValue("红:" + lot.Scheme.RedsExp + " 蓝:" + lot.Scheme.BlueExp);
                }

                {
                    DBXmlNode bindingNode = visualNode.AddChild("binding");

                    bindingNode.SetAttribute("template", "TileSquareImage");

                    DBXmlNode imageNode = bindingNode.AddChild("image");
                    imageNode.SetAttribute("id", "1");
                    imageNode.SetAttribute("src", "https://dbdatastorage.blob.core.windows.net/dbnotification/Logo.png");
                    imageNode.SetAttribute("alt", "alt text");
                }

                // Save to file.
                xml.Save("Z:\\ReleaseData\\Notifications\\Tile_Default.xml");
            }

            // 3. Generate windows tile notification - detail tile
            {
                //<tile>
                //  <visual>
                //    <binding template="TileWideSmallImageAndText02">
                //      <image id="1" src="https://dbdatastorage.blob.core.windows.net/dbnotification/Draw.png" alt="alt text"/>
                //      <text id="1">第2013110期</text>
                //      <text id="2">15 17 18 21 29 32+13</text>
                //      <text id="3">[销售]3亿4186万7256元</text>
                //      <text id="4">[奖池]2亿7166万6100元</text>
                //    </binding>  
                //    <binding template="TileSquareText01">
                //      <text id="1">第2013110期</text>
                //      <text id="2">15 17 18</text>
                //      <text id="3">21 29 32</text>
                //      <text id="4">+13</text>
                //    </binding> 
                //  </visual>
                //</tile>
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode toastNode = xml.AddRoot("tile");
                DBXmlNode visualNode = toastNode.AddChild("visual");

                {
                    DBXmlNode bindingNode = visualNode.AddChild("binding");

                    bindingNode.SetAttribute("template", "TileWideSmallImageAndText02");

                    DBXmlNode imageNode = bindingNode.AddChild("image");
                    imageNode.SetAttribute("id", "1");
                    imageNode.SetAttribute("src", "https://dbdatastorage.blob.core.windows.net/dbnotification/Draw.png");
                    imageNode.SetAttribute("alt", "alt text");

                    DBXmlNode text1Node = bindingNode.AddChild("text");
                    text1Node.SetAttribute("id", "1");
                    text1Node.SetValue(strIssue);

                    DBXmlNode text2Node = bindingNode.AddChild("text");
                    text2Node.SetAttribute("id", "2");
                    text2Node.SetValue(strScheme);

                    DBXmlNode text3Node = bindingNode.AddChild("text");
                    text3Node.SetAttribute("id", "3");
                    text3Node.SetValue("[销售] " + betAmount);

                    DBXmlNode text4Node = bindingNode.AddChild("text");
                    text4Node.SetAttribute("id", "4");
                    text4Node.SetValue("[奖池] " + poolAmount);
                }

                {
                    DBXmlNode bindingNode = visualNode.AddChild("binding");

                    bindingNode.SetAttribute("template", "TileSquareText01");

                    DBXmlNode text1Node = bindingNode.AddChild("text");
                    text1Node.SetAttribute("id", "1");
                    text1Node.SetValue(strIssue);

                    DBXmlNode text2Node = bindingNode.AddChild("text");
                    text2Node.SetAttribute("id", "2");
                    text2Node.SetValue(lot.Scheme.RedsExp.Substring(0, 8));

                    DBXmlNode text3Node = bindingNode.AddChild("text");
                    text3Node.SetAttribute("id", "3");
                    text3Node.SetValue(lot.Scheme.RedsExp.Substring(9, 8));

                    DBXmlNode text4Node = bindingNode.AddChild("text");
                    text4Node.SetAttribute("id", "4");
                    text4Node.SetValue("+" + lot.Scheme.BlueExp);
                }

                // Save to file.
                xml.Save("Z:\\ReleaseData\\Notifications\\Tile_issue_details.xml");
            }

            // 4. Generate windows tile notification - attribute tile
            {
                //<tile>
                //  <visual>
                //    <binding template="TileWideSmallImageAndText02">
                //      <image id="1" src="https://dbdatastorage.blob.core.windows.net/dbnotification/Attribute_Analysis.png" alt="alt text"/>
                //      <text id="1">异常属性</text>
                //      <text id="2">[8.2] 红球03 [出现]</text>
                //      <text id="3">[7.5] 红球二六位和 [36~38]</text>
                //      <text id="4">[7.1] 红球12位012对比 [0-1]</text>
                //    </binding>  
                //    <binding template="TileSquareText01">
                //      <text id="1">异常属性</text>
                //      <text id="2">红球03 [出现]</text>
                //      <text id="3">红球二六位和 [36~38]</text>
                //      <text id="4">红球12位012对比 [0-1]</text>
                //    </binding> 
                //  </visual>
                //</tile>
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode toastNode = xml.AddRoot("tile");
                DBXmlNode visualNode = toastNode.AddChild("visual");

                {
                    DBXmlNode bindingNode = visualNode.AddChild("binding");

                    bindingNode.SetAttribute("template", "TileWideSmallImageAndText02");

                    DBXmlNode imageNode = bindingNode.AddChild("image");
                    imageNode.SetAttribute("id", "1");
                    imageNode.SetAttribute("src", "https://dbdatastorage.blob.core.windows.net/dbnotification/Attribute_Analysis.png");
                    imageNode.SetAttribute("alt", "alt text");

                    DBXmlNode text1Node = bindingNode.AddChild("text");
                    text1Node.SetAttribute("id", "1");
                    text1Node.SetValue("异常属性");

                    DBXmlNode text2Node = bindingNode.AddChild("text");
                    text2Node.SetAttribute("id", "2");
                    text2Node.SetValue("[" + prop1.ToString("f2") + "] " + state1.DisplayName);

                    DBXmlNode text3Node = bindingNode.AddChild("text");
                    text3Node.SetAttribute("id", "3");
                    text3Node.SetValue("[" + prop2.ToString("f2") + "] " + state2.DisplayName);

                    DBXmlNode text4Node = bindingNode.AddChild("text");
                    text4Node.SetAttribute("id", "4");
                    text4Node.SetValue("[" + prop3.ToString("f2") + "] " + state3.DisplayName);
                }

                {
                    DBXmlNode bindingNode = visualNode.AddChild("binding");

                    bindingNode.SetAttribute("template", "TileSquareText01");

                    DBXmlNode text1Node = bindingNode.AddChild("text");
                    text1Node.SetAttribute("id", "1");
                    text1Node.SetValue("异常属性");

                    DBXmlNode text2Node = bindingNode.AddChild("text");
                    text2Node.SetAttribute("id", "2");
                    text2Node.SetValue(state1.DisplayName);

                    DBXmlNode text3Node = bindingNode.AddChild("text");
                    text3Node.SetAttribute("id", "3");
                    text3Node.SetValue(state2.DisplayName);

                    DBXmlNode text4Node = bindingNode.AddChild("text");
                    text4Node.SetAttribute("id", "4");
                    text4Node.SetValue(state3.DisplayName);
                }

                // Save to file.
                xml.Save("Z:\\ReleaseData\\Notifications\\Tile_attribute_analysis.xml");
            }

            // 5. Generate windows tile notification - num analysis tile
            {
                //<tile>
                //  <visual>
                //    <binding template="TileWideSmallImageAndText02">
                //      <image id="1" src="https://dbdatastorage.blob.core.windows.net/dbnotification/Number_Analysis.png" alt="alt text"/>
                //      <text id="1">遗漏号码</text>
                //      <text id="2">[红33] 连续[35]期未开出</text>
                //      <text id="3">[红03] 连续[32]期未开出</text>
                //      <text id="4">[蓝04] 连续[62]期未开出</text>
                //    </binding>  
                //    <binding template="TileSquareText01">
                //      <text id="1">遗漏号码</text>
                //      <text id="2">[红33] 遗漏35期</text>
                //      <text id="3">[红03] 遗漏32期</text>
                //      <text id="4">[蓝04] 遗漏62期</text>
                //    </binding> 
                //  </visual>
                //</tile>
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode toastNode = xml.AddRoot("tile");
                DBXmlNode visualNode = toastNode.AddChild("visual");

                {
                    DBXmlNode bindingNode = visualNode.AddChild("binding");

                    bindingNode.SetAttribute("template", "TileWideSmallImageAndText02");

                    DBXmlNode imageNode = bindingNode.AddChild("image");
                    imageNode.SetAttribute("id", "1");
                    imageNode.SetAttribute("src", "https://dbdatastorage.blob.core.windows.net/dbnotification/Number_Analysis.png");
                    imageNode.SetAttribute("alt", "alt text");

                    DBXmlNode text1Node = bindingNode.AddChild("text");
                    text1Node.SetAttribute("id", "1");
                    text1Node.SetValue("遗漏号码");

                    DBXmlNode text2Node = bindingNode.AddChild("text");
                    text2Node.SetAttribute("id", "2");
                    text2Node.SetValue("[红球" + r_top1.ToString().PadLeft(2, '0') + "] 连续[" + r_mis1.ToString() + "]期未开出");

                    DBXmlNode text3Node = bindingNode.AddChild("text");
                    text3Node.SetAttribute("id", "3");
                    text3Node.SetValue("[红球" + r_top2.ToString().PadLeft(2, '0') + "] 连续[" + r_mis2.ToString() + "]期未开出");

                    DBXmlNode text4Node = bindingNode.AddChild("text");
                    text4Node.SetAttribute("id", "4");
                    text4Node.SetValue("[蓝球" + b_top.ToString().PadLeft(2, '0') + "] 连续[" + b_mis.ToString() + "]期未开出");
                }

                {
                    DBXmlNode bindingNode = visualNode.AddChild("binding");

                    bindingNode.SetAttribute("template", "TileSquareText01");

                    DBXmlNode text1Node = bindingNode.AddChild("text");
                    text1Node.SetAttribute("id", "1");
                    text1Node.SetValue("遗漏号码");

                    DBXmlNode text2Node = bindingNode.AddChild("text");
                    text2Node.SetAttribute("id", "2");
                    text2Node.SetValue("[红球" + r_top1.ToString().PadLeft(2, '0') + "] 遗漏" + r_mis1.ToString() + "期");

                    DBXmlNode text3Node = bindingNode.AddChild("text");
                    text3Node.SetAttribute("id", "3");
                    text3Node.SetValue("[红球" + r_top2.ToString().PadLeft(2, '0') + "] 遗漏" + r_mis2.ToString() + "期");

                    DBXmlNode text4Node = bindingNode.AddChild("text");
                    text4Node.SetAttribute("id", "4");
                    text4Node.SetValue("[蓝球" + b_top.ToString().PadLeft(2, '0') + "] 遗漏" + b_mis.ToString() + "期");
                }

                // Save to file.
                xml.Save("Z:\\ReleaseData\\Notifications\\Tile_num_analysis.xml");
            }

            #endregion
        }

        // notification pushing related
        public void GetNotificationFromTemplate(NotificationTemplateEnum template, ref string title, ref List<string> content)
        {
            content.Clear();

            Lottery lot = GetHistory().LastLottery;
            switch (template)
            {
                case NotificationTemplateEnum.Release:
                    {
                        title = "第" + lot.Issue.ToString() + "期" + " 开奖啦！";
                        content.Add("红: " + lot.Scheme.RedsExp + " 蓝: " + lot.Scheme.BlueExp);

                        break;
                    }
                case NotificationTemplateEnum.Detail:
                    {
                        int firstPZCount = lot.BonusAmount(1);
                        int pool = lot.PoolAmount;
                        if (pool > 0)
                        {
                            int expectFPZCount = pool / 5000000;

                            title = "一等奖中出 " + firstPZCount.ToString() + " 注 " + Lottery.FormatMoney(lot.BonusMoney(1));
                            content.Add("奖池 " + lot.PoolAmountExp + ", 够开出 " + expectFPZCount.ToString() + " 个五百万！");
                        }

                        break;
                    }
                case NotificationTemplateEnum.Recommendation:
                    {
                        // top missed red.
                        int r_top = 0, r_mis = 0;
                        for (int i = 1; i <= 33; ++i)
                        {
                            int omission = lot.Status.RedNumStates[i - 1].Omission;

                            if (r_top == 0 || omission > r_mis)
                            {
                                r_top = i;
                                r_mis = omission;
                            }
                        }

                        double red_score = ((double)(r_mis - 10) / 5.0) * 25.0;

                        // top missed blue.
                        int b_top = 0, b_mis = 0;
                        for (int i = 1; i <= 16; ++i)
                        {
                            int omission = lot.Status.BlueNumStates[i - 1].Omission;

                            if (b_top == 0 || omission > b_mis)
                            {
                                b_top = i;
                                b_mis = omission;
                            }
                        }

                        double blue_score = ((double)(b_mis - 30) / 10.0) * 25.0;

                        // top missed attribute
                        SchemeAttributeValueStatus state1 = null;
                        double prop1 = 0.0;
                        SchemeAttributes last = GetAttriMgr().LatestAttributes();
                        foreach (KeyValuePair<string, SchemeAttributeCategory> cat in last.Categories)
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

                        double attri_score = (prop1 - 7.0) * 25.0;

                        title = "第" + _nextIssue.ToString() + "期 今晚开奖！";
                        content.Add("参考：" + "红球 " + r_top.ToString().PadLeft(2, '0') + " 已连续 " + r_mis.ToString() + " 期未开出！");
                        content.Add("参考：" + "蓝球 " + b_top.ToString().PadLeft(2, '0') + " 已连续 " + b_mis.ToString() + " 期未开出！");
                        content.Add("参考：" + "属性 " + state1.DisplayName + " 偏离指数已达到 " + prop1.ToString() + " 倍！");
                            
                        break;
                    }
            }
        }

        public string FormatNotification(int platform, string title, string content)
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

                return          "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
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

        public string UpdateSQLOnCloud()
        {
            // get the sql lines from the target file.
            string fileName = "Z:/ReleaseData/SQL.sql";
            if (!File.Exists(fileName))
                return "Can't find sql file at Z:/ReleaseData/SQL.sql";
            
            string sqlLines = File.ReadAllText(fileName);
            if (sqlLines == "")
                return "Invalid SQL commands";

            return ServerUtil.ExecuteSqlQueries(sqlLines);
        }

        public string UpdateFileOnCloud(string source, string targetContainer, string targetBlob)
        {
            return ServerUtil.UploadToAzureStorage(source, targetContainer, targetBlob);
        }

        public void AutoDetectLotteriesFromWeb()
        {
            Lottery current = GetHistory().LastLottery;

            int currentIssue = current.Issue;
            DateTime releaseDate = current.Date;

            int nextIssue = 0;
            DateTime sellOfftime = DateTime.Now, nextReleaseDate = DateTime.Now;
                            
            // Get next release info.
            DataUtil.CalculateNextReleaseNumberAndTime(currentIssue, releaseDate, ref nextIssue, ref sellOfftime, ref nextReleaseDate);

            int[] reds = new int[6];
            int blue = 0;
            int[,] bonus = new int[6, 2];
            int betAmount = 0;
            int poolAmount = 0;
            string more = "None";

            while (DataUtil.SyncFromWeb(nextIssue, ref nextReleaseDate, ref reds, ref blue, ref bonus, ref betAmount, ref poolAmount, ref more))
            {
                // Add the new lotter.
                Lottery next = DataUtil.AddLottory(GetHistory(), nextIssue, new Scheme(reds[0], reds[1], reds[2], reds[3], reds[4], reds[5], blue), false);

                // Update details.
                next.SetDetail(nextReleaseDate, bonus, betAmount, poolAmount, more.Replace("\r", "").Replace("\n", "").Replace(" ", ""));

                // Refresh data.
                currentIssue = nextIssue;
                releaseDate = nextReleaseDate;
                DataUtil.CalculateNextReleaseNumberAndTime(currentIssue, releaseDate, ref nextIssue, ref sellOfftime, ref nextReleaseDate);

                UncommittedItem item = new UncommittedItem()
                {
                    Issue = currentIssue,
                    Dirty = true,
                    Lottery = next,
                    Attributes = null, // keep it now, and calcualte when saving.
                    ReleaseInfo = new ReleaseInfo()
                    {
                        CurrentIssue = currentIssue,
                        NextIssue = nextIssue,
                        NextReleaseTime = nextReleaseDate,
                        SellOffTime = sellOfftime
                    },
                    SavedInCloud = false,
                    SavedInLocal = false
                };

                _pendingItems.Add(item);

                _issueLatest = currentIssue;

                _dataVersion.LatestIssue = _issueLatest;
            }

            // update next issue.
            _nextIssue = nextIssue;
        }

        public UncommittedItem AddNextLottery(Scheme result)
        {
            Lottery current = GetHistory().LastLottery;

            int currentIssue = current.Issue;
            DateTime releaseDate = current.Date;

            int nextIssue = 0;
            DateTime sellOfftime = DateTime.Now, nextReleaseDate = DateTime.Now;

            // Get next release info.
            DataUtil.CalculateNextReleaseNumberAndTime(currentIssue, releaseDate, ref nextIssue, ref sellOfftime, ref nextReleaseDate);

            // Add the new lottery.
            Lottery next = DataUtil.AddLottory(GetHistory(), nextIssue, result, false);

            next.SetDetail(nextReleaseDate, new int[6,2], 0, 0, "");

            // Refresh data.
            currentIssue = nextIssue;
            releaseDate = nextReleaseDate;
            DataUtil.CalculateNextReleaseNumberAndTime(currentIssue, releaseDate, ref nextIssue, ref sellOfftime, ref nextReleaseDate);

            UncommittedItem item = new UncommittedItem()
            {
                Issue = currentIssue,
                Dirty = true,
                Lottery = next,
                Attributes = null,
                ReleaseInfo = new ReleaseInfo()
                {
                    CurrentIssue = currentIssue,
                    NextIssue = nextIssue,
                    NextReleaseTime = nextReleaseDate,
                    SellOffTime = sellOfftime
                },
                SavedInCloud = false,
                SavedInLocal = false
            };

            _pendingItems.Add(item);

            _issueLatest = currentIssue;
            _nextIssue = nextIssue;

            _dataVersion.LatestIssue = _issueLatest;
            
            return item;
        }
     
        public DataTable InitMatrixTable()
        {
            if (_matrixTable == null)
            {
                _matrixTable = new LuckyBallsData.Filters.MatrixTable();
                _matrixTable.Init();
                //MatrixTableBuilder builder = new MatrixTableBuilder(_matrixTable);
                //builder.Init(false);
            }

            DataTable table = new DataTable();
  
            Type intType = System.Type.GetType("System.Int32");
            Type stringType = System.Type.GetType("System.String");
            Type doubleType = System.Type.GetType("System.Double");
            table.Columns.Add(CreateColumn("选号数", intType, false, false));
            table.Columns.Add(CreateColumn("选2中1", intType, false, false));
            table.Columns.Add(CreateColumn("选3中2", intType, false, false));
            table.Columns.Add(CreateColumn("选4中3", intType, false, false));
            table.Columns.Add(CreateColumn("选5中4", intType, false, false));
            table.Columns.Add(CreateColumn("选6中5", intType, false, false));

            for (int i = 6; i <= 33; ++i)
            {
                DataRow row = table.NewRow();

                row["选号数"] = i;
                row["选2中1"] = _matrixTable.GetCellItemCount(i, 2);
                row["选3中2"] = _matrixTable.GetCellItemCount(i, 3);
                row["选4中3"] = _matrixTable.GetCellItemCount(i, 4);
                row["选5中4"] = _matrixTable.GetCellItemCount(i, 5);
                row["选6中5"] = _matrixTable.GetCellItemCount(i, 6);

                table.Rows.Add(row);
            }

            return table;
        }     

        #endregion

        #region Utilities
        private DataColumn CreateColumn(string text, Type datatype, bool unique, bool writable)
        {
            DataColumn column = new DataColumn();
            column.DataType = datatype;
            column.ColumnName = text;
            column.ReadOnly = !writable;
            column.Unique = unique;

            return column;
        }

        #endregion

        public void ProcessLoginMessages()
        {
            ServerUtil.ProcessLoginInfo(ProcessLogin);
        }

        private bool ProcessLogin(string message)
        {
            DataUtil.AppendLoginInfo(message, "Z:\\log.txt");

            return true;
        }

        #region Help related

        public HelpBuilder GetHelp()
        {
            if (_helpBuilder == null)
            {
                _helpBuilder = new HelpBuilder();

                string helpFile = "Z:/Help.xml";
                if (File.Exists(helpFile))
                {
                    DBXmlDocument xml = new DBXmlDocument();
                    xml.Load(helpFile);

                    _helpBuilder.Read(xml.Root());

                    _helpBuilder.InitNextIDs();
                }
            }

            return _helpBuilder;
        }

        public void SaveHelp()
        {
            if (_helpBuilder != null)
            {
                string helpFile = "Z:/Help.xml";
                _helpBuilder.SaveToFile(helpFile);
            }
        }

        #endregion
    }
}
