package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class Record 
{
	private String _deviceID = "";
	private int[] _bonus = new int[6];
	private int _cost = 0;
	private int _prize = 0;
	private int _issue = 0;
	
	public final String getDeviceID()
    {
		return _deviceID;
    }
	
	public final void setDeviceID(String value)
	{
		_deviceID = value;
	}

	public final int[] getBonus()
    {
		return _bonus;
    }
	
	public final void setBonus(int[] value)
	{
		_bonus = value;
	}

	public final int getCost()
    {
		return _cost;
    }
	
	public final void setCost(int value)
	{
		_cost = value;
	}

	public final int getPrize()
    {
		return _prize;
    }
	
	public final void setPrize(int value)
	{
		_prize = value;
	}

	public final int getIssue()
    {
		return _issue;
    }
	
	public final void setIssue(int value)
	{
		_issue = value;
	}

    public String getBonusString()
    {
    	String bonus = "";
        for (int count : getBonus())
        {
            bonus += Integer.toString(count) + " ";
        }

        return bonus.trim();
    }

    public void Read(DBXmlNode node)
    {
        setIssue(Integer.parseInt(node.GetAttribute("IS")));
        setCost(Integer.parseInt(node.GetAttribute("CT")));
        setPrize(Integer.parseInt(node.GetAttribute("PZ")));
        setDeviceID(node.GetAttribute("ID"));

        String bonus = node.GetAttribute("BN");
        String[] subStrings = bonus.split(" ", -1);
        for (int index = 0; index < subStrings.length; index++)
        {
            _bonus[index] = Integer.parseInt(subStrings[index]);
        }
    }

    public void Write(DBXmlNode node)
    {
        node.SetAttribute("IS", Integer.toString(getIssue()));
        node.SetAttribute("CT", Integer.toString(getCost()));
        node.SetAttribute("PZ", Integer.toString(getPrize()));
        node.SetAttribute("ID", getDeviceID());

        String bonus = getBonusString();
        node.SetAttribute("BN", bonus);
    }
}
