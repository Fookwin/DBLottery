using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Util;
using LuckyBallsData.Selection;

namespace LuckyBallsData.Configration
{
    public class AttributeFilterOption
    {
        public double HitProbability_LowLimit = 10.0;
        public int ImmediateOmission_LowLimit = 1;
        public double ProtentialEnergy_LowLimit = 5.0;
        public double MaxDeviation_LowLimit = 5.0;
        public double RecommendThreshold = 8.0;

        public void AsDefault()
        {
            HitProbability_LowLimit = 10.0;
            ImmediateOmission_LowLimit = 1;
            ProtentialEnergy_LowLimit = 5.0;
            MaxDeviation_LowLimit = 5.0;
            RecommendThreshold = 8.0;
        }

        public bool Passed(SchemeAttributeValueStatus state)
        {
            return state.HitProbability > HitProbability_LowLimit &&
                state.ImmediateOmission > ImmediateOmission_LowLimit &&
                state.ProtentialEnergy > ProtentialEnergy_LowLimit &&
                ((double)state.MaxOmission / state.AverageOmission) > MaxDeviation_LowLimit;
        }

        public void Read(DBXmlNode node)
        {
            HitProbability_LowLimit = Convert.ToDouble(node.GetAttribute("HitPorb"));
            ImmediateOmission_LowLimit = Convert.ToInt32(node.GetAttribute("CurOmis"));
            ProtentialEnergy_LowLimit = Convert.ToDouble(node.GetAttribute("ProtEng"));
            MaxDeviation_LowLimit = Convert.ToDouble(node.GetAttribute("MaxDev"));
            RecommendThreshold = Convert.ToDouble(node.GetAttribute("Threshold"));
        }

        public void Save(DBXmlNode node)
        {
            node.SetAttribute("HitPorb", HitProbability_LowLimit.ToString());
            node.SetAttribute("CurOmis", ImmediateOmission_LowLimit.ToString());
            node.SetAttribute("ProtEng", ProtentialEnergy_LowLimit.ToString());
            node.SetAttribute("MaxDev", MaxDeviation_LowLimit.ToString());
            node.SetAttribute("Threshold", RecommendThreshold.ToString());
        }
    }
}