using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Util;

namespace LuckyBallsData.Statistics
{
    public class Lottery
    {
        private DBXmlNode _xmlNodeCache = null;
        protected History _parent = null;

        protected int _issue = -1;
        protected DateTime? _releaseDate = null;
        protected int[,] _bonusResult = null;
        protected int _betAmount = -1;
        protected int _poolAmount = -1;
        protected string _moreInfo = null;
        protected Scheme _scheme = null;
        protected Status _status = null;

        public Lottery(History history)
        {
            _parent = history;
        }

        public Lottery(History hisory, int issue, Scheme scheme)
        {
            _issue = issue;
            _scheme = scheme;
            _parent = hisory; 
        }

        public void SetDetail(DateTime date, int[,] bonus, int betAmount, int poolAmount, string moreInfo)
        {
            _releaseDate = date;
            _bonusResult = bonus;
            _betAmount = betAmount;
            _poolAmount = poolAmount;
            _moreInfo = moreInfo;
        }

        public DateTime Date
        {
            get
            {
                if (_releaseDate == null)
                {
                    if (_xmlNodeCache != null)
                        _releaseDate = DateTime.Parse(_xmlNodeCache.GetAttribute("Date"));
                    else
                        _releaseDate = new DateTime();
                }

                return _releaseDate.Value;
            }
        }

        public string DateExp
        {
            get
            {
                return Date.ToString("yyyy-MM-dd");
            }
        }

        public int BetAmount
        {
            get
            {
                if (_betAmount < 0 && _xmlNodeCache != null)
                    _betAmount = Convert.ToInt32(_xmlNodeCache.GetAttribute("Bet"));

                return _betAmount;
            }
        }

        public int PoolAmount
        {
            get
            {
                if (_poolAmount < 0 && _xmlNodeCache != null)
                    _poolAmount = Convert.ToInt32(_xmlNodeCache.GetAttribute("Pool"));

                return _poolAmount;
            }
        }

        public string MoreInfo
        {
            get
            {
                if (_moreInfo == null)
                {
                    if (_xmlNodeCache != null)
                        _moreInfo = _xmlNodeCache.GetAttribute("Comments");
                    else
                        _moreInfo = "";
                }

                return _moreInfo;
            }
        }

        public int BonusAmount(int prizeLevel)
        {
            if (prizeLevel < 1 || prizeLevel > 6)
                return -1;

            if (_bonusResult == null)
            {
                ParseBonus();
            }

            return _bonusResult[prizeLevel - 1, 0];
        }

        public int BonusMoney(int prizeLevel)
        {
            if (prizeLevel < 1 || prizeLevel > 6)
                return -1;

            if (_bonusResult == null)
            {
                ParseBonus();
            }

            return _bonusResult[prizeLevel - 1, 1];
        }

        public Scheme Scheme
        {
            get
            {
                if (_scheme == null)
                {
                    if (_xmlNodeCache != null)
                        _scheme = new Scheme(_xmlNodeCache.GetAttribute("Scheme"));
                    else
                        _scheme = new Scheme();                    
                }

                return _scheme;
            }
        }

        public Status Status
        {
            get
            {
                if (_status == null)
                {
                    _status = new Status();

                    if (_xmlNodeCache != null)
                    {
                        DBXmlNode childNode = _xmlNodeCache.FirstChildNode("Status");
                        _status.ReadFromXml(childNode);
                    }
                }

                return _status;
            }
        }

        public int Issue
        {
            get
            {
                if (_issue < 0 && _xmlNodeCache != null)
                    _issue = Convert.ToInt32(_xmlNodeCache.GetAttribute("Issue"));

                return _issue;
            }
        }

        public int CheckPrize(Scheme test)
        {
            int redHit = test.Similarity(Scheme);
            bool blueHit = test.Blue == Scheme.Blue;

            switch (redHit)
            {
                case 0:
                case 1:
                case 2:
                    return blueHit ? 6 : -1;
                case 3:
                    return blueHit ? 5 : -1;
                case 4:
                    return blueHit ? 4 : 5;
                case 5:
                    return blueHit ? 3 : 4;
                case 6:
                    return blueHit ? 1 : 2;
                default:
                    return -1;
            }
        }        

        public bool ReadFromXml(DBXmlNode node)
        {
            _xmlNodeCache = node;

            // clear current data.
            _issue = -1;
            _releaseDate = null;
            _bonusResult = null;
            _betAmount = -1;
            _poolAmount = -1;
            _moreInfo = null;
            _scheme = null;
            _status = null;

            return true; // delay parsing the data.
        }

        public bool SaveToXml(DBXmlNode node)
        {
            node.SetAttribute("Issue", Issue.ToString());
            node.SetAttribute("Scheme", Scheme.ToString());
            node.SetAttribute("Date", Date.ToString("yyyy-MM-dd"));
            node.SetAttribute("Bet", BetAmount.ToString());
            node.SetAttribute("Pool", PoolAmount.ToString());
            node.SetAttribute("Comments", MoreInfo.ToString());

            DBXmlNode bonusNode = node.AddChild("Bonus");

            if (_bonusResult == null)
            {
                // parse it first if empty.
                ParseBonus();
            }

            string bonusCounts = "";
            for (int i = 0; i < 6; ++i)
            {
                bonusCounts += _bonusResult[i, 0].ToString();
                if (i != 5)
                    bonusCounts += " ";
            }

            bonusNode.SetAttribute("PrizeCounts", bonusCounts);
            bonusNode.SetAttribute("Prize1Bonus", _bonusResult[0, 1].ToString());
            bonusNode.SetAttribute("Prize2Bonus", _bonusResult[1, 1].ToString());

            DBXmlNode statusNode = node.AddChild("Status");
            Status.SaveToXml(statusNode);

            return true;
        } 

        private void ParseBonus()
        {
            _bonusResult = new int[6, 2] { { 0, 0 }, { 0, 0 }, { 0, 3000 }, { 0, 200 }, { 0, 10 }, { 0, 5 } };

            if (_xmlNodeCache != null)
            {
                DBXmlNode childNode = _xmlNodeCache.FirstChildNode("Bonus");

                string[] bonusCounts = childNode.GetAttribute("PrizeCounts").Split(' ');
                if (bonusCounts.Count() == 6)
                {
                    _bonusResult[0, 0] = Convert.ToInt32(bonusCounts[0]);
                    _bonusResult[1, 0] = Convert.ToInt32(bonusCounts[1]);
                    _bonusResult[2, 0] = Convert.ToInt32(bonusCounts[2]);
                    _bonusResult[3, 0] = Convert.ToInt32(bonusCounts[3]);
                    _bonusResult[4, 0] = Convert.ToInt32(bonusCounts[4]);
                    _bonusResult[5, 0] = Convert.ToInt32(bonusCounts[5]);
                }

                _bonusResult[0, 1] = Convert.ToInt32(childNode.GetAttribute("Prize1Bonus"));
                _bonusResult[1, 1] = Convert.ToInt32(childNode.GetAttribute("Prize2Bonus"));
            }
        }

        public string BetAmountExp
        {
            get
            {
                return FormatMoney(BetAmount);
            }
        }

        public string PoolAmountExp
        {
            get
            {
                return FormatMoney(PoolAmount);
            }
        }

        public static string FormatMoney(int money)
        {
            string output = "";
            int yi = money / 100000000;
            if (yi > 0)
                output += yi.ToString() + "亿";

            int wan = (money % 100000000) / 10000;
            if (wan > 0)
                output += wan.ToString() + "万";

            output += (money % 10000).ToString() + "元";

            return output;
        }
    }
}
