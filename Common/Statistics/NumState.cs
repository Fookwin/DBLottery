using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Util;

namespace LuckyBallsData.Statistics
{
    public struct NumState
    {
	    public int m_iHit;
        public int m_iOmission;
        public int m_iMaxOmission;
        public int m_iAverageOmission;
    }

    public class NumStates
    {
        public NumState[] m_RedNumStates = new NumState[33];
        public NumState[] m_BlueNumStates = new NumState[16];

        public bool ReadFromXml(DBXmlNode node)
        {
            return true;
        }

        public bool SaveToXml(DBXmlNode node)
        {
            return true;
        }
    }
}
