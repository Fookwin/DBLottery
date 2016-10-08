package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class HelpTopic
{
	private int _id = -1;
	private String _title = "";
	private String _description = "";
	private String _notes = "";
	
	public final int getID()
    {
        return _id;
    }
	
	public final void setID(int value)
	{
		_id = value;
	}

	public final String getTitle()
    {
        return _title;
    }
	
	public final void setTitle(String value)
	{
		_title = value;
	}

	public final String getDescription()
    {
        return _description;
    }
	
	public final void setDescription(String value)
	{
		_description = value;
	}

	public final String getNotes()
    {
        return _notes;
    }
	
	public final void setNotes(String value)
	{
		_notes = value;
	}

    public int[] getNoteIDs()
    {
        if (_notes == "")
            return null;

        String[] ids = _notes.split(" ", -1);

        int[] output = new int[ids.length];

        int index = 0;
        for (String id : ids)
        {
            output[index++] = Integer.parseInt(id);
        }
        return output;
    }

    public void SaveToXml(DBXmlNode node)
    {
        node.SetAttribute("ID", Integer.toString(getID()));
        node.SetAttribute("Description", getDescription());
        node.SetAttribute("Title", getTitle());
        node.SetAttribute("Notes", getNotes());
    }

    public void ReadFromXml(DBXmlNode node)
    {
        setID(Integer.parseInt(node.GetAttribute("ID")));
        setDescription(node.GetAttribute("Description"));
        setTitle(node.GetAttribute("Title"));
        setNotes(node.GetAttribute("Notes"));
    }
}
