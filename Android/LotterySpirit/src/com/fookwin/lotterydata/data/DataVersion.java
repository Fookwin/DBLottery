package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class DataVersion
{
	private int _latestIssue = 0;
	private int _historyDataVersion = 1;
	private int _attributeDataVersion = 1;
	private int _attributeTemplateVersion = 1;
	private int _releaseDataVersion = 1;
	private int _latestLotteryVersion = 1;
	private int _matrixDataVersion = 1;
	private int _helpContentVersion = 0;

	public final int getLatestIssue()
	{
		return _latestIssue;
	}
	public final void setLatestIssue(int value)
	{
		_latestIssue = value;
	}

	public final int getMatrixDataVersion()
	{
		return _matrixDataVersion;
	}
	public final void setMatrixDataVersion(int value)
	{
		_matrixDataVersion = value;
	}

	public final int getLatestLotteryVersion()
	{
		return _latestLotteryVersion;
	}
	public final void setLatestLotteryVersion(int value)
	{
		_latestLotteryVersion = value;
	}

	public final int getHistoryDataVersion()
	{
		return _historyDataVersion;
	}
	public final void setHistoryDataVersion(int value)
	{
		_historyDataVersion = value;
	}

	public final int getReleaseDataVersion()
	{
		return _releaseDataVersion;
	}
	public final void setReleaseDataVersion(int value)
	{
		_releaseDataVersion = value;
	}

	public final int getAttributeDataVersion()
	{
		return _attributeDataVersion;
	}
	public final void setAttributeDataVersion(int value)
	{
		_attributeDataVersion = value;
	}

	public final int getAttributeTemplateVersion()
	{
		return _attributeTemplateVersion;
	}
	public final void setAttributeTemplateVersion(int value)
	{
		_attributeTemplateVersion = value;
	}
	
	public final int getHelpContentVersion()
	{
		return _helpContentVersion;
	}
	public final void setHelpContentVersion(int value)
	{
		_helpContentVersion = value;
	}

	public final void SaveToXml(DBXmlNode node)
	{
		node.SetAttribute("LatestIssue", Integer.toString(getLatestIssue()));
		node.SetAttribute("History", Integer.toString(getHistoryDataVersion()));
		node.SetAttribute("Release", Integer.toString(getReleaseDataVersion()));
		node.SetAttribute("Attributes", Integer.toString(getAttributeDataVersion()));
		node.SetAttribute("AttributeTemplate", Integer.toString(getAttributeTemplateVersion()));
		node.SetAttribute("LatestLottery", Integer.toString(getLatestLotteryVersion()));
		node.SetAttribute("Matrix", Integer.toString(getMatrixDataVersion()));
		node.SetAttribute("Help", Integer.toString(getHelpContentVersion()));
    }

	public final void ReadFromXml(DBXmlNode node)
	{
		setLatestIssue(Integer.parseInt(node.GetAttribute("LatestIssue")));

		String temp = node.GetAttribute("History");
		if (!temp.equals(""))
		{
			setHistoryDataVersion(Integer.parseInt(temp));
		}

		temp = node.GetAttribute("Release");
		if (!temp.equals(""))
		{
			setReleaseDataVersion(Integer.parseInt(temp));
		}

		temp = node.GetAttribute("Attributes");
		if (!temp.equals(""))
		{
			setAttributeDataVersion(Integer.parseInt(temp));
		}

		temp = node.GetAttribute("AttributeTemplate");
		if (!temp.equals(""))
		{
			setAttributeTemplateVersion(Integer.parseInt(temp));
		}

		temp = node.GetAttribute("LatestLottery");
		if (!temp.equals(""))
		{
			setLatestLotteryVersion(Integer.parseInt(temp));
		}

		temp = node.GetAttribute("Matrix");
		if (!temp.equals(""))
		{
			setMatrixDataVersion(Integer.parseInt(temp));
		}
		
		temp = node.GetAttribute("Help");
		if (!temp.equals(""))
		{
			setHelpContentVersion(Integer.parseInt(temp));
		}
	}
}