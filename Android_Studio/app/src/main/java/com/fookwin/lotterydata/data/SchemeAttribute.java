package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class SchemeAttribute
{
	private java.util.ArrayList<SchemeAttributeValueStatus> _valueStates;
	private boolean _needParseValues = false;
	private DBXmlNode _xmlValueNode = null;
	private int _helpID = -1;
	private double _maxProtentialEnergy = -1.0;
	
	private String privateKey;
	public final String getKey()
	{
		return privateKey;
	}
	public final void setKey(String value)
	{
		privateKey = value;
	}

	private String privateDisplayName;
	public final String getDisplayName()
	{
		return privateDisplayName;
	}
	public final void setDisplayName(String value)
	{
		privateDisplayName = value;
	}

	private Region privateValidRegion;
	public final Region getValidRegion()
	{
		return privateValidRegion;
	}
	public final void setValidRegion(Region value)
	{
		privateValidRegion = value;
	}

	private String privateDescription;
	public final String getDescription()
	{
		return privateDescription;
	}
	public final void setDescription(String value)
	{
		privateDescription = value;
	}
	
	public final int getHelpID()
	{
		return _helpID;
	}
	public final void setHelpID(int value)
	{
		_helpID = value;
	}
	
	public final double getMaxProtentialEnergy()
	{
		if (_maxProtentialEnergy >= 0)
			return _maxProtentialEnergy;
		
		for (SchemeAttributeValueStatus state : getValueStates())
		{
			if (state.getProtentialEnergy() > _maxProtentialEnergy)
			{
				_maxProtentialEnergy = state.getProtentialEnergy();
			}
		}
		
		return _maxProtentialEnergy;
	}

	public final java.util.ArrayList<SchemeAttributeValueStatus> getValueStates()
	{
		if (_needParseValues)
		{
			ParseValues();
		}

		return _valueStates;
	}
	public final void setValueStates(java.util.ArrayList<SchemeAttributeValueStatus> value)
	{
		_valueStates = value;
	}

	public final void ReadFromTemplate(DBXmlNode node)
	{
		setKey(node.Name());
		setDisplayName(node.GetAttribute("Display"));
		setValidRegion(new Region(node.GetAttribute("Region")));
		setDescription(node.GetAttribute("Description"));
		
		String temp = node.GetAttribute("HelpID");
		if (temp != "")
		{
			setHelpID(Integer.parseInt(temp));
		}

		if (_valueStates == null)
		{
			_valueStates = new java.util.ArrayList<SchemeAttributeValueStatus>();
		}

		_valueStates.clear();

		for (DBXmlNode stateNode : node.ChildNodes())
		{
			SchemeAttributeValueStatus state = new SchemeAttributeValueStatus(this);
			state.ReadFromTemplate(stateNode);

			_valueStates.add(state);
		}
	}

	public final void SaveAsTemplate(DBXmlNode node)
	{
		node.SetAttribute("Display", getDisplayName());
		node.SetAttribute("Region", getValidRegion().toString());
		node.SetAttribute("Description", getDescription());
		node.SetAttribute("HelpID", Integer.toString(getHelpID()));

		for (SchemeAttributeValueStatus state : _valueStates)
		{
			DBXmlNode stateNode = node.AddChild("State");
			state.SaveAsTemplate(stateNode);
		}
	}

	public final void ReadValueFromXml(DBXmlNode node)
	{
		_xmlValueNode = node;
		_needParseValues = true;
	}

	private void ParseValues()
	{
		int index = 0;
		for (DBXmlNode stateNode : _xmlValueNode.ChildNodes())
		{
			SchemeAttributeValueStatus state = _valueStates.get(index++);
			state.ReadValueFromXml(stateNode);
		}

		_needParseValues = false;
	}

	public final void SaveValueToXml(DBXmlNode node)
	{
		for (SchemeAttributeValueStatus state : getValueStates())
		{
			DBXmlNode stateNode = node.AddChild("State");
			state.SaveValueToXml(stateNode);
		}
	}

	public final SchemeAttribute clone()
	{
		SchemeAttribute tempVar = new SchemeAttribute();
		tempVar.setKey(this.getKey());
		tempVar.setDisplayName(this.getDisplayName());
		tempVar.setDescription(this.getDescription());
		tempVar.setValidRegion(this.getValidRegion());
		tempVar.setValueStates(new java.util.ArrayList<SchemeAttributeValueStatus>());
		tempVar.setHelpID(this.getHelpID());
		SchemeAttribute copy = tempVar;

		for (SchemeAttributeValueStatus state : getValueStates())
		{
			copy.getValueStates().add(state.clone());
		}

		return copy;
	}
}