package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.AttributeUtil;
import com.fookwin.lotterydata.util.DBXmlNode;

public class SchemeAttributeConstraint extends Constraint
{
	private String _attributeKey = "";
	private String _attributeName = "";
	protected Set _validNumbers = new Set();

	public SchemeAttributeConstraint()
	{
	}

	public SchemeAttributeConstraint(String attributeKey, String attributeName, Set set)
	{
		 _validNumbers = set;
		_attributeKey = attributeKey;
		_attributeName = attributeName;
	}

	@Override
	public ConstraintTypeEnum GetConstraintType()
	{
		return ConstraintTypeEnum.SchemeAttributeConstraintType;
	}
	
	public final void setAttribute(String key, String name)
	{
		_attributeKey = key;
		_attributeName = name;
	}
	
	public final String getAttributeName()
	{
		return _attributeName;
	}

	public final String getAttributeKey()
	{
		return _attributeKey;
	}

	public final Set getValues()
	{
		return _validNumbers;
	}
	public final void setValues(Set value)
	{
		_validNumbers = value;
	}

	@Override
	public Constraint clone()
	{
	   return new SchemeAttributeConstraint(_attributeKey, _attributeName, new Set(_validNumbers));
	}

	@Override
	protected String GetDisplayExpression()
	{
		// Get the corresponding attribute.
		SchemeAttribute template = AttributeUtil.GetSchemeAttribute(_attributeKey);
		if (template == null)
			return "";

		String valueExp = "";
		for (SchemeAttributeValueStatus value : template.getValueStates())
		{
			if (_validNumbers.Contains(value.getValueRegion()))
			{
				if (!valueExp.equals(""))
				{
					valueExp += ", ";
				}
				valueExp += value.getValueExpression();
			}
		}

		return "[属性过滤] " + _attributeName + " = [" + valueExp + "]";
	}

	@Override
	public boolean Meet(Scheme num, int refIssueIndex)
	{
		return _validNumbers.Contains(num.Attribute(_attributeKey, refIssueIndex));
	}

	@Override
	public void ReadFromXml(DBXmlNode node)
	{
		if (_validNumbers == null)
		{
			_validNumbers = new Set();
		}

		_attributeKey = node.GetAttribute("Attribute");
		_attributeName = node.GetAttribute("AttributeName");

		DBXmlNode numberNode = node.FirstChildNode("Values");
		_validNumbers.Parse(numberNode.GetAttribute("Expression"));
	}

	@Override
	public void WriteToXml(DBXmlNode node)
	{
		node.SetAttribute("Attribute", _attributeKey);
		node.SetAttribute("AttributeName", _attributeName);

		DBXmlNode numberNode = node.AddChild("Values");
		numberNode.SetAttribute("Expression", _validNumbers.toString());
	}

	@Override
	public String HasError()
	{
		if (_attributeKey.length() == 0)
		{
			return "请选择一个属性";
		}

		if (_validNumbers.getCount() == 0)
		{
			return "请选择至少一个属性值";
		}

		return "";
	}
}