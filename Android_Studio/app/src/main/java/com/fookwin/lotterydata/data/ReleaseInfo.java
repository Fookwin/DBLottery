package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

public class ReleaseInfo
{
	protected int _currentIssue = -1;
	protected int _nextIssue = -1;
	protected Date _nextReleaseTime = null;
	protected Date _sellOffTime = null;
	protected Set _includedReds = new Set();
	protected Set _includedBlues = new Set();
	protected Set _excludedReds = new Set();
	protected Set _excludedBlues = new Set();

	public final int getCurrentIssue()
	{
		return _currentIssue;
	}
	public final void setCurrentIssue(int value)
	{
		_currentIssue = value;
	}

	public final int getNextIssue()
	{
		return _nextIssue;
	}
	public final void setNextIssue(int value)
	{
		_nextIssue = value;
	}

	public final Date getNextReleaseTime()
	{
		return _nextReleaseTime;
	}
	public final void setNextReleaseTime(java.util.Date value)
	{
		_nextReleaseTime = value;
	}

	public final Date getSellOffTime()
	{
		return _sellOffTime;
	}
	public final void setSellOffTime(java.util.Date value)
	{
		_sellOffTime = value;
	}

	public final Set getIncludedReds()
	{
		return _includedReds;
	}
	public final Set getIncludedBlues()
	{
		return _includedBlues;
	}
	public final Set getExcludedReds()
	{
		return _excludedReds;
	}
	public final Set getExcludedBlues()
	{
		return _excludedBlues;
	}

	public final void Read(DBXmlNode node)
	{
		_currentIssue = Integer.parseInt(node.GetAttribute("Issue"));
		_nextIssue = Integer.parseInt(node.GetAttribute("Next"));
		
		try {
			SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yyyy HH:mm:ss");
			_nextReleaseTime = sdf.parse(node.GetAttribute("NextDate"));
			_sellOffTime = sdf.parse(node.GetAttribute("OffTime"));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		_includedReds.Parse(node.GetAttribute("Included_Reds"));
		_includedBlues.Parse(node.GetAttribute("Included_Blues"));
		_excludedReds.Parse(node.GetAttribute("Excluded_Reds"));
		_excludedBlues.Parse(node.GetAttribute("Excluded_Blues"));
	}
}