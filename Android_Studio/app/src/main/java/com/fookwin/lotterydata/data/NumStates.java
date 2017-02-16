package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class NumStates
{
	public NumState[] m_RedNumStates = new NumState[33];
	public NumState[] m_BlueNumStates = new NumState[16];

	public final boolean ReadFromXml(DBXmlNode node)
	{
		return true;
	}

	public final boolean SaveToXml(DBXmlNode node)
	{
		return true;
	}
}