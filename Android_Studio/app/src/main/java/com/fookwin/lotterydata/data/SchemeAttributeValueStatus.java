package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class SchemeAttributeValueStatus implements Comparable<SchemeAttributeValueStatus>
{	
	private SchemeAttribute _parent;
	private int _hitCount = 0;
	private double _hitProbability = 0.0;
	private double _idealProbility = -1.0;
	private double _averageOmission = 0.0;
	private int _maxOmission = 0;
	private int _immediateOmission = 0;
	private double _protentialEnergy = 0.0;

	@Override
	public final int compareTo(SchemeAttributeValueStatus other)
	{
		// If other is not a valid object reference, this instance is greater. 
		if (other == null)
		{
			return 1;
		}

		// The temperature comparison depends on the comparison of  
		// the underlying Double values.
		return Double.compare(_protentialEnergy, other._protentialEnergy);
	}


	public final String getDisplayName()
	{
		return _parent.getDisplayName() + " = [" + getValueExpression() + "]";
	}

	private Region privateValueRegion;
	public final Region getValueRegion()
	{
		return privateValueRegion;
	}
	public final void setValueRegion(Region value)
	{
		privateValueRegion = value;
	}

	private String privateValueExpression;
	public final String getValueExpression()
	{
		return privateValueExpression;
	}
	public final void setValueExpression(String value)
	{
		privateValueExpression = value;
	}

	public final SchemeAttribute getParent()
	{
		return _parent;
	}
	public final void setParent(SchemeAttribute value)
	{
		_parent = value;
	}

	public final int getHitCount()
	{
		return _hitCount;
	}
	public final void setHitCount(int value)
	{
		_hitCount = value;
	}

	public final double getHitProbability()
	{
		return _hitProbability;
	}
	public final void setHitProbability(double value)
	{
		_hitProbability = value;
	}

	public final double getIdealProbility()
	{
		return _idealProbility;
	}
	public final void setIdealProbility(double value)
	{
		_idealProbility = value;
	}

	public final double getAverageOmission()
	{
		return _averageOmission;
	}
	public final void setAverageOmission(double value)
	{
		_averageOmission = value;
	}

	public final int getMaxOmission()
	{
		return _maxOmission;
	}
	public final void setMaxOmission(int value)
	{
		_maxOmission = value;
	}

	public final int getImmediateOmission()
	{
		return _immediateOmission;
	}
	public final void setImmediateOmission(int value)
	{
		_immediateOmission = value;
	}

	public final double getProtentialEnergy()
	{
		return _protentialEnergy;
	}
	public final void setProtentialEnergy(double value)
	{
		_protentialEnergy = value;
	}

	public SchemeAttributeValueStatus()
	{
		_parent = null;
	}

	public SchemeAttributeValueStatus(SchemeAttribute parent_Attri)
	{
		_parent = parent_Attri;
	}

	public final void ReadValueFromXml(DBXmlNode node)
	{
		String value = node.GetAttribute("Value");
		String[] data = value.split("[,]", -1);
		if (data.length != 7)
		{
			throw new RuntimeException("Attribute Value Count is not correct.");
		}

		int index = 0;
		setHitCount(Integer.parseInt(data[index++]));
		setHitProbability(Double.parseDouble(data[index++]));
		setIdealProbility(Double.parseDouble(data[index++]));
		setAverageOmission(Double.parseDouble(data[index++]));
		setMaxOmission(Integer.parseInt(data[index++]));
		setImmediateOmission(Integer.parseInt(data[index++]));
		setProtentialEnergy(Double.parseDouble(data[index++]));
	}

	public final void SaveValueToXml(DBXmlNode node)
	{
		String value = "";

		value += Integer.toString(getHitCount()) + ",";
		value += String.format("%0.1f", getHitProbability()) + ",";
		value += String.format("%0.1f", getIdealProbility()) + ",";
		value += String.format("%0.1f", getAverageOmission()) + ",";
		value += Integer.toString(getMaxOmission()) + ",";
		value += Integer.toString(getImmediateOmission()) + ",";
		value += String.format("%0.1f", getProtentialEnergy());

		node.SetAttribute("Value", value);
	}

	public final void ReadFromTemplate(DBXmlNode node)
	{
		setValueExpression(node.GetAttribute("Expression"));
		setValueRegion(new Region(node.GetAttribute("Region")));
	}

	public final void SaveAsTemplate(DBXmlNode node)
	{
		node.SetAttribute("Expression", getValueExpression());
		node.SetAttribute("Region", getValueRegion().toString());
	}

	public final SchemeAttributeValueStatus clone()
	{
		SchemeAttributeValueStatus tempVar = new SchemeAttributeValueStatus(_parent);
		tempVar.setValueExpression(this.getValueExpression());
		tempVar.setValueRegion(new Region(this.getValueRegion().getMin(), this.getValueRegion().getMax()));
		tempVar.setHitCount(this.getHitCount());
		tempVar.setHitProbability(this.getHitProbability());
		tempVar.setIdealProbility(this.getIdealProbility());
		tempVar.setAverageOmission(this.getAverageOmission());
		tempVar.setMaxOmission(this.getMaxOmission());
		tempVar.setImmediateOmission(this.getImmediateOmission());
		tempVar.setProtentialEnergy(this.getProtentialEnergy());
		return tempVar;
	}
}