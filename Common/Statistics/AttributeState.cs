using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Util;

namespace LuckyBallsData.Statistics
{
    public class AttributeStates
    {
        Dictionary<string, int> _states = new Dictionary<string, int>();

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
