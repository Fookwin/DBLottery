using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Util;

namespace LuckyBallsData.Statistics
{
    public class History
    {
        private List<Lottery> m_Lotteries = new List<Lottery>();
        private int mVersion = 0;
        private int mLastIssue = 0;

        public enum DataID
        {
            DID_INVALID = 0,

            // basic data...
            //

            // red numbers...
            DID_RED_1,
            DID_RED_2,
            DID_RED_3,
            DID_RED_4,
            DID_RED_5,
            DID_RED_6,

            DID_BLUE,
        }

        public static void GetDataRegion(DataID dataId, out int min, out int max)
        {
            min = 0;
            max = 0;
            switch (dataId)
            {
                case DataID.DID_BLUE:
                    {
                        min = 1;
                        max = 16;
                        break;
                    }
                case DataID.DID_RED_1:
                case DataID.DID_RED_2:
                case DataID.DID_RED_3:
                case DataID.DID_RED_4:
                case DataID.DID_RED_5:
                case DataID.DID_RED_6:
                    {
                        min = dataId - DataID.DID_RED_1 + 1;
                        max = 28 + (dataId - DataID.DID_RED_1);
                        break;
                    }
            }
        }

        public History()
        {
        }

        public int Version
        {
            get
            {
                return mVersion;
            }
        }

        public int LastIssue
        {
            get
            {
                return mLastIssue;
            }
        }

        public List<Lottery> Lotteries
        {
            get
            {
                return m_Lotteries;
            }
        }

        public Lottery LotteryInIndex(int index)
        {
            if (index >= 0 && index < m_Lotteries.Count())
                return m_Lotteries[index];
            else
                return null;
        }

        public int IssueToIndex(int issue)
        {
            for (int i = 0; i < m_Lotteries.Count; ++ i)
            {
                Lottery lot = m_Lotteries[i];
                if (lot.Issue == issue)
                    return i;

                if (lot.Issue > issue)
                    return -1; ;
            }

            return -1;
        }

        public Lottery LotteryInIssue(int issue)
        {
            foreach (Lottery lot in m_Lotteries)
            {
                if (lot.Issue == issue)
                    return lot;

                if (lot.Issue > issue)
                    break;
            }

            return null;
        }

        public int Count
        {
            get
            {
                return m_Lotteries.Count;
            }
        }

        public Lottery LastLottery
        {
            get
            {
                return m_Lotteries[m_Lotteries.Count - 1];
            }
        }

        public Lottery AddLottery(Lottery lot)
        {
            m_Lotteries.Add(lot);

            // update the latest issue.
            if (mLastIssue < lot.Issue)
                mLastIssue = lot.Issue;

            return lot;
        }

        public bool GetDataByID(DataID id, ref Dictionary<int, int> output, int limitStart = -1, int limitEnd = -1)
        {
            if (limitStart < limitEnd)
                return false;

            bool bRanged = limitStart >= 0;

            switch (id)
            {
                case DataID.DID_BLUE:
                    {
                        int index = 0;
                        foreach (Lottery result in m_Lotteries)
                        {
                            if (!bRanged || index >= limitStart)
                            {
                                if (bRanged && index > limitEnd)
                                    return true; // end.

                                output.Add(result.Issue, result.Scheme.Blue);
                            }
                            ++index;
                        }
                        break;
                    }
                case DataID.DID_RED_1:
                case DataID.DID_RED_2:
                case DataID.DID_RED_3:
                case DataID.DID_RED_4:
                case DataID.DID_RED_5:
                case DataID.DID_RED_6:
                    {
                        int index = 0;
                        foreach (Lottery result in m_Lotteries)
                        {
                            if (!bRanged || index >= limitStart)
                            {
                                if (bRanged && index > limitEnd)
                                    return true; // end.

                                output.Add(result.Issue, result.Scheme.Red(id - DataID.DID_RED_1));
                            }
                            ++index;
                        }
                        break;
                    }
                default:
                    return false;
            }

            return true;
        }

        public void SetVersionInfo(int version)
        {
            mVersion = version;
        }

        public enum InsertOption
        {
            eReplaceExists = 0,
            eAppendToTail = 1,
            eInsertToHead = 2
        };

        public bool ReadFromXml(DBXmlNode node, InsertOption op)
        {
            if (op == InsertOption.eReplaceExists)
            {
                m_Lotteries.Clear();
            }

            int index = op == InsertOption.eInsertToHead ? 0 : m_Lotteries.Count();
            foreach (DBXmlNode childNode in node.ChildNodes())
            {
                Lottery lot = new Lottery(this);
                lot.ReadFromXml(childNode);

                if (op == InsertOption.eInsertToHead)
                {
                    m_Lotteries.Insert(index - 1, lot);
                }
                else
                {
                    m_Lotteries.Add(lot);

                    // update the latest issue.
                    if (mLastIssue < lot.Issue)
                        mLastIssue = lot.Issue;
                }
            }

            return true;
        }

        public static Lottery ReadSingleLottery(int issue, DBXmlNode node)
        {
            foreach (DBXmlNode childNode in node.ChildNodes())
            {
                if (childNode.GetAttribute("Issue") == issue.ToString())
                {
                    Lottery lot = new Lottery(null);
                    lot.ReadFromXml(childNode);

                    return lot;
                }

            }

            return null;
        }

        public bool SaveToXml(ref DBXmlNode node)
        {
            foreach (Lottery lot in m_Lotteries)
            {
                DBXmlNode lotNode = node.AddChild("Lottery");

                lot.SaveToXml(lotNode);
            }

            return true;
        }
    }
}
