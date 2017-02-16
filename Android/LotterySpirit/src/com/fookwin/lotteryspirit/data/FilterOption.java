package com.fookwin.lotteryspirit.data;

import com.fookwin.lotterydata.data.SchemeAttribute;
import com.fookwin.lotterydata.data.SchemeAttributeValueStatus;
import com.fookwin.lotterydata.util.DBXmlNode;

public class FilterOption
{
    public double HitProbability_LowLimit = 10.0;
    public int ImmediateOmission_LowLimit = 2;
    public double ProtentialEnergy_LowLimit = 5.0;
    public double MaxDeviation_LowLimit = 5.0;
    public double thresoldToRecommend = 8.0;

    public boolean passed(SchemeAttributeValueStatus state)
    {
        return state.getHitProbability() > HitProbability_LowLimit &&
            state.getImmediateOmission() > ImmediateOmission_LowLimit &&
            state.getProtentialEnergy() > ProtentialEnergy_LowLimit &&
            (double)state.getMaxOmission() / state.getAverageOmission() > MaxDeviation_LowLimit;
    }
    
    public boolean passed(SchemeAttribute attri)
    {
		for (SchemeAttributeValueStatus state : attri.getValueStates())
		{
			if (passed(state))
			{
				return true;
			}
		}
		
		return false;
    }
    
    public void asDefault()
    {
		HitProbability_LowLimit = 10.0;
		ImmediateOmission_LowLimit = 2;
		ProtentialEnergy_LowLimit = 5.0;
		MaxDeviation_LowLimit = 5.0;
		thresoldToRecommend = 8.0;
    }
    
    public boolean recommend(double score)
    {
    	return score > thresoldToRecommend;
    }

    public boolean recommend(SchemeAttributeValueStatus state)
    {
    	return state.getProtentialEnergy() > thresoldToRecommend;
    }

    public void Read(DBXmlNode node)
    {
        HitProbability_LowLimit = Double.parseDouble(node.GetAttribute("HitPorb"));
        ImmediateOmission_LowLimit = Integer.parseInt(node.GetAttribute("CurOmis"));
        ProtentialEnergy_LowLimit = Double.parseDouble(node.GetAttribute("ProtEng"));
        MaxDeviation_LowLimit = Double.parseDouble(node.GetAttribute("MaxDev"));
        thresoldToRecommend = Double.parseDouble(node.GetAttribute("Threshold"));
    }

    public void Save(DBXmlNode node)
    {
        node.SetAttribute("HitPorb", Double.toString(HitProbability_LowLimit));
        node.SetAttribute("CurOmis", Integer.toString(ImmediateOmission_LowLimit));
        node.SetAttribute("ProtEng", Double.toString(ProtentialEnergy_LowLimit));
        node.SetAttribute("MaxDev", Double.toString(MaxDeviation_LowLimit));
        node.SetAttribute("Threshold", Double.toString(thresoldToRecommend));
    }
}
