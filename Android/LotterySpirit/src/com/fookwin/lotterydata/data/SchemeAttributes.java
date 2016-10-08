package com.fookwin.lotterydata.data;

import java.util.HashMap;

import com.fookwin.lotterydata.util.DBXmlNode;

public class SchemeAttributes
{
	private HashMap<String, SchemeAttributeCategory> _categories = new HashMap<String,SchemeAttributeCategory>();

	public SchemeAttributes()
	{
	}

	public final SchemeAttributeCategory Category(String name)
	{
		if (_categories.containsKey(name))
		{
			return _categories.get(name);
		}

		return null;
	}

	public final HashMap<String, SchemeAttributeCategory> getCategories()
	{
		return _categories;
	}

	public final SchemeAttributeCategory AddCategory(SchemeAttributeCategory cat)
	{
		if (cat != null)
		{
			_categories.put(cat.getName(), cat);
		}

		return cat;
	}

	public final SchemeAttributes clone()
	{
		SchemeAttributes copy = new SchemeAttributes();

		for (java.util.Map.Entry<String, SchemeAttributeCategory> cat : _categories.entrySet())
		{
			copy.AddCategory(cat.getValue().clone());
		}

		return copy;
	}

	public final SchemeAttribute Attribute(String category, String key)
	{
		SchemeAttributeCategory cat = Category(category);
		if (cat == null)
		{
			return null;
		}

		return cat.Attribute(key);
	}

	// Use Attribute(string category, string key) if you know the [aremt category. 
	public final SchemeAttribute Attribute(String key)
	{
		for (java.util.Map.Entry<String, SchemeAttributeCategory> cat : _categories.entrySet())
		{
			SchemeAttribute temp = cat.getValue().Attribute(key);
			if (temp != null)
			{
				return temp;
			}
		}

		return null;
	}

	public final void SaveAsTemplate(DBXmlNode node)
	{
		for (java.util.Map.Entry<String, SchemeAttributeCategory> cat : _categories.entrySet())
		{
			DBXmlNode catNode = node.AddChild(cat.getKey());
			cat.getValue().SaveAsTemplate(catNode);
		}
	}

	public final void ReadFromTemplate(DBXmlNode node)
	{
		_categories.clear();

		java.util.ArrayList<DBXmlNode> catNodes = node.ChildNodes();
		for (DBXmlNode catNode : catNodes)
		{
			SchemeAttributeCategory cat = new SchemeAttributeCategory();
			cat.ReadFromTemplate(catNode);

			_categories.put(catNode.Name(), cat);
		}
	}

	public final void SaveValueToXml(DBXmlNode node)
	{
		for (java.util.Map.Entry<String, SchemeAttributeCategory> cat : _categories.entrySet())
		{
			DBXmlNode catNode = node.AddChild(cat.getKey());
			cat.getValue().SaveValueToXml(catNode);
		}
	}

	public final void ReadValueFromXml(DBXmlNode node)
	{
		java.util.ArrayList<DBXmlNode> catNodes = node.ChildNodes();
		for (DBXmlNode catNode : catNodes)
		{
			SchemeAttributeCategory cat = Category(catNode.Name());
			cat.ReadValueFromXml(catNode);
		}
	}
}