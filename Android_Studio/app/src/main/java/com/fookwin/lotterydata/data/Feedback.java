package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class Feedback 
{
	private int _plateform = -1;
	private String _deviceID = "";
	private int _localVersion = -1;
	private String _name = "";
	private String _email = "";
	private String _phone = "";
	private String _content = "";
	
	public final int getPlateform()
    {
		return _plateform;
    }
	
	public final void setPlateform(int value)
	{
		_plateform = value;
	}	
	
	public final String getDeviceID()
    {
		return _deviceID;
    }
	
	public final void setDeviceID(String value)
	{
		_deviceID = value;
	}
	
	public final int getLocalVersion()
    {
		return _localVersion;
    }
	
	public final void setLocalVersion(int value)
	{
		_localVersion = value;
	}
	
	public final String getName()
    {
		return _name;
    }
	
	public final void setName(String value)
	{
		_name= value;
	}

	public final String getEmail()
    {
		return _email;
    }
	
	public final void setEmail(String value)
	{
		_email = value;
	}

	public final String getPhone()
    {
		return _phone;
    }
	
	public final void setPhone(String value)
	{
		_phone = value;
	}

	public final String getContent()
    {
		return _content;
    }
	
	public final void setContent(String value)
	{
		_content = value;
	}

    public void Read(DBXmlNode node)
    {
        setPlateform(Integer.parseInt(node.GetAttribute("PL")));
        setLocalVersion(Integer.parseInt(node.GetAttribute("VR")));
        setDeviceID(node.GetAttribute("ID"));
        setName(node.GetAttribute("NM"));
        setEmail(node.GetAttribute("EM"));
        setPhone(node.GetAttribute("PH"));
        setContent(node.GetAttribute("CT"));
    }

    public void Write(DBXmlNode node)
    {
        node.SetAttribute("PL", Integer.toString(getPlateform()));
        node.SetAttribute("VR", Integer.toString(getLocalVersion()));
        node.SetAttribute("ID", getDeviceID());
        node.SetAttribute("NM", getName());
        node.SetAttribute("EM", getEmail());
        node.SetAttribute("PH", getPhone());
        node.SetAttribute("CT", getContent());
    }
}
