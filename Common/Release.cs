using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;
using LuckyBallsData.Selection;

namespace LuckyBallsData
{
    public class DataVersion
    {
        private int _latestIssue = 0;
        private int _historyDataVersion = 1;
        private int _attributeDataVersion = 1;
        private int _attributeTemplateVersion = 1;
        private int _releaseDataVersion = 1;
        private int _latestLotteryVersion = 1;
        private int _matrixDataVersion = 1;
        private int _helpContentVersion = 0;

        public int LatestIssue
        {
            get { return _latestIssue; }
            set { _latestIssue = value; }
        }

        public int MatrixDataVersion
        {
            get { return _matrixDataVersion; }
            set { _matrixDataVersion = value; }
        }

        public int LatestLotteryVersion
        {
            get { return _latestLotteryVersion; }
            set { _latestLotteryVersion = value; }
        }

        public int HistoryDataVersion
        {
            get { return _historyDataVersion; }
            set { _historyDataVersion = value; }
        }

        public int ReleaseDataVersion
        {
            get { return _releaseDataVersion; }
            set { _releaseDataVersion = value; }
        }

        public int AttributeDataVersion
        {
            get { return _attributeDataVersion; }
            set { _attributeDataVersion = value; }
        }

        public int AttributeTemplateVersion
        {
            get { return _attributeTemplateVersion; }
            set { _attributeTemplateVersion = value; }
        }

        public int HelpContentVersion
        {
            get { return _helpContentVersion; }
            set { _helpContentVersion = value; }
        }

        public void SaveToXml(ref DBXmlNode node)
        {
            node.SetAttribute("LatestIssue", LatestIssue.ToString());
            node.SetAttribute("History", HistoryDataVersion.ToString());
            node.SetAttribute("Release", ReleaseDataVersion.ToString());
            node.SetAttribute("Attributes", AttributeDataVersion.ToString());
            node.SetAttribute("AttributeTemplate", AttributeTemplateVersion.ToString());
            node.SetAttribute("LatestLottery", LatestLotteryVersion.ToString());
            node.SetAttribute("Matrix", MatrixDataVersion.ToString());
            node.SetAttribute("Help", HelpContentVersion.ToString());
        }

        public void ReadFromXml(DBXmlNode node)
        {
            LatestIssue = Convert.ToInt32(node.GetAttribute("LatestIssue"));

            string temp = node.GetAttribute("History");
            if (temp != "")
                HistoryDataVersion = Convert.ToInt32(temp);

            temp = node.GetAttribute("Release");
            if (temp != "")
                ReleaseDataVersion = Convert.ToInt32(temp);

            temp = node.GetAttribute("Attributes");
            if (temp != "") 
                AttributeDataVersion = Convert.ToInt32(temp);

            temp = node.GetAttribute("AttributeTemplate");
            if (temp != "") 
                AttributeTemplateVersion = Convert.ToInt32(temp);

            temp = node.GetAttribute("LatestLottery");
            if (temp != "") 
                LatestLotteryVersion = Convert.ToInt32(temp);

            temp = node.GetAttribute("Matrix");
            if (temp != "") 
                MatrixDataVersion = Convert.ToInt32(temp);

            temp = node.GetAttribute("Help");
            if (temp != "") 
                HelpContentVersion = Convert.ToInt32(temp);
        }

        public override string ToString()
        {
            DBXmlDocument xml = new DBXmlDocument();
            DBXmlNode root = xml.AddRoot("Version");

            SaveToXml(ref root);

            return xml.OuterXml();
        }
    }

    public class DBMessage
    {
        private string _title = "";
        private string _content = "";
        private Uri _link = null;
        private DateTime _date = DateTime.Today;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public Uri Link
        {
            get { return _link; }
            set { _link = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public void SaveToXml(ref DBXmlNode node)
        {
            node.SetAttribute("Title", _title);
            node.SetAttribute("Content", _content);
            node.SetAttribute("Link", _link.ToString());
            node.SetAttribute("Date", _date.ToString());
        }

        public void ReadFromXml(DBXmlNode node)
        {
            _title = node.GetAttribute("Title");
            _content = node.GetAttribute("Content");
            _link = new Uri(node.GetAttribute("Link"));
            _date = DateTime.Parse(node.GetAttribute("Date"));
        }
    }

    public class ReleaseInfo
    {
        protected int _currentIssue = -1;
        protected int _nextIssue = -1;
        protected DateTime? _nextReleaseTime = null;
        protected DateTime? _sellOffTime = null;
        protected List<DBMessage> _releaseMessages = new List<DBMessage>();
        protected List<DBMessage> _nextMessages = new List<DBMessage>();
        protected Set _includedReds = new Set();
        protected Set _includedBlues = new Set();
        protected Set _excludedReds = new Set();
        protected Set _excludedBlues = new Set();

        public int CurrentIssue
        {
            get { return _currentIssue; }
            set { _currentIssue = value; }
        }

        public int NextIssue
        {
            get { return _nextIssue; }
            set { _nextIssue = value; }
        }

        public DateTime NextReleaseTime
        {
            get { return _nextReleaseTime.Value; }
            set { _nextReleaseTime = value; }
        }

        public DateTime SellOffTime
        {
            get { return _sellOffTime.Value; }
            set { _sellOffTime = value; }
        }

        public List<DBMessage> CurrentReleaseMessages
        {
            get { return _releaseMessages; }
        }
        public List<DBMessage> NextReleaseMessages
        {
            get { return _nextMessages; }
        }

        public Set IncludedReds
        {
            get { return _includedReds; }
        }
        public Set IncludedBlues
        {
            get { return _includedBlues; }
        }
        public Set ExcludedReds
        {
            get { return _excludedReds; }
        }
        public Set ExcludedBlues
        {
            get { return _excludedBlues; }
        }

        public void Save(ref DBXmlNode node)
        {
            node.SetAttribute("Issue", _currentIssue.ToString());
            node.SetAttribute("Next", _nextIssue.ToString());
            node.SetAttribute("NextDate", _nextReleaseTime.ToString());
            node.SetAttribute("OffTime", _sellOffTime.ToString());

            DBXmlNode releaseMsgNode = node.AddChild("PostMessages");
            foreach (DBMessage msg in _releaseMessages)
            {
                DBXmlNode msgNode = releaseMsgNode.AddChild("Msg");
                msg.SaveToXml(ref msgNode);
            }

            DBXmlNode nextMsgNode = node.AddChild("PreMessages");
            foreach (DBMessage msg in _nextMessages)
            {
                DBXmlNode msgNode = nextMsgNode.AddChild("Msg");
                msg.SaveToXml(ref msgNode);
            }

            node.SetAttribute("Included_Reds", _includedReds.ToString());
            node.SetAttribute("Included_Blues", _includedBlues.ToString());
            node.SetAttribute("Excluded_Reds", _excludedReds.ToString());
            node.SetAttribute("Excluded_Blues", _excludedBlues.ToString());
        }

        public void Read(DBXmlNode node)
        {
            _currentIssue = Convert.ToInt32(node.GetAttribute("Issue"));
            _nextIssue = Convert.ToInt32(node.GetAttribute("Next"));
            _nextReleaseTime = DateTime.Parse(node.GetAttribute("NextDate"));
            _sellOffTime = DateTime.Parse(node.GetAttribute("OffTime"));

            DBXmlNode releaseMsgNode = node.FirstChildNode("PostMessages");
            List<DBXmlNode> childNodes1 = releaseMsgNode.ChildNodes();
            foreach (DBXmlNode child in childNodes1)
            {
                DBMessage msg = new DBMessage();
                msg.ReadFromXml(child);
                _releaseMessages.Add(msg);
            }

            DBXmlNode nextMsgNode = node.FirstChildNode("PreMessages");
            List<DBXmlNode> childNodes2 = nextMsgNode.ChildNodes();
            foreach (DBXmlNode child in childNodes2)
            {
                DBMessage msg = new DBMessage();
                msg.ReadFromXml(child);
                _releaseMessages.Add(msg);
            }

            _includedReds.Parse(node.GetAttribute("Included_Reds"));
            _includedBlues.Parse(node.GetAttribute("Included_Blues"));
            _excludedReds.Parse(node.GetAttribute("Excluded_Reds"));
            _excludedBlues.Parse(node.GetAttribute("Excluded_Blues"));
        }
    }
}
