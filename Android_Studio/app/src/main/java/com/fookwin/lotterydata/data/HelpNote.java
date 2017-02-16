package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class HelpNote 
{
	private int _id = -1;
	private String _content = "";
	
	public final int getID()
    {
        return _id;
    }
	
	public final void setID(int value)
	{
		_id = value;
	}

	public final String getContent()
    {
        return _content;
    }
	
	public final void setContent(String value)
	{
		_content = value;
	}

    public void SaveToXml(DBXmlNode node)
    {
        node.SetAttribute("ID", Integer.toString(getID()));
        node.SetAttribute("Text", getContent());
    }

    public void ReadFromXml(DBXmlNode node)
    {
        setID(Integer.parseInt(node.GetAttribute("ID")));
        setContent(node.GetAttribute("Text"));
    }
}
