package com.fookwin.lotterydata.data;

import java.util.HashMap;

import com.fookwin.lotterydata.util.DBXmlNode;

public class SchemeAttributeCategory
{
	private HashMap<String, SchemeAttribute> _attributes = new HashMap<String, SchemeAttribute>();

	private String privateName;
	public final String getName()
	{
		return privateName;
	}
	public final void setName(String value)
	{
		privateName = value;
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

	public final HashMap<String, SchemeAttribute> getAttributes()
	{
		return _attributes;
	}

	public final SchemeAttribute Attribute(String Key)
	{
		if (_attributes.containsKey(Key))
		{
			return _attributes.get(Key);
		}

		return null;
	}

	public final void AddAttribute(SchemeAttribute attri)
	{
		if (_attributes.containsKey(attri.getKey()))
		{
			throw new RuntimeException("The attribute with the same key already exists.");
		}

		_attributes.put(attri.getKey(), attri);
	}

	public final void RemoveAttribute(String Key)
	{
		if (_attributes.containsKey(Key))
		{
			_attributes.remove(Key);
		}
	}

	public final SchemeAttributeCategory clone()
	{
		SchemeAttributeCategory tempVar = new SchemeAttributeCategory();
		tempVar.setName(this.getName());
		tempVar.setDisplayName(this.getDisplayName());
		SchemeAttributeCategory copy = tempVar;

		for (java.util.Map.Entry<String, SchemeAttribute> pair : _attributes.entrySet())
		{
			copy.AddAttribute(pair.getValue().clone());
		}

		return copy;
	}

	public final void SaveAsTemplate(DBXmlNode node)
	{
		node.SetAttribute("Display", getDisplayName());
		for (java.util.Map.Entry<String, SchemeAttribute> pair : _attributes.entrySet())
		{
			DBXmlNode attriNode = node.AddChild(pair.getKey());
			pair.getValue().SaveAsTemplate(attriNode);
		}
	}

	public final void ReadFromTemplate(DBXmlNode node)
	{
		setName(node.Name());
		setDisplayName(node.GetAttribute("Display"));

		_attributes.clear();
		java.util.ArrayList<DBXmlNode> attiNodes = node.ChildNodes();
		for (DBXmlNode attiNode : attiNodes)
		{
			SchemeAttribute attri = new SchemeAttribute();
			attri.ReadFromTemplate(attiNode);

			_attributes.put(attiNode.Name(), attri);
		}
	}

	public final void SaveValueToXml(DBXmlNode node)
	{
		for (java.util.Map.Entry<String, SchemeAttribute> pair : _attributes.entrySet())
		{
			DBXmlNode attriNode = node.AddChild(pair.getKey());
			pair.getValue().SaveValueToXml(attriNode);
		}
	}

	public final void ReadValueFromXml(DBXmlNode node)
	{
		if (_attributes.isEmpty())
		{
			throw new RuntimeException("Attributes Not Initialized.");
		}

		java.util.ArrayList<DBXmlNode> attiNodes = node.ChildNodes();
		for (DBXmlNode attiNode : attiNodes)
		{
			SchemeAttribute attri = Attribute(attiNode.Name());
			attri.ReadValueFromXml(attiNode);
		}
	}
}