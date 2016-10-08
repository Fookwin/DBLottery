package com.fookwin.lotteryspirit.data;

import com.fookwin.lotterydata.data.NumberState;

public class NumberStateInfo
{
	private int privateNumber;
	public final int getNumber()
	{
		return privateNumber;
	}
	public final void setNumber(int value)
	{
		privateNumber = value;
	}

	private NumberState privateState;
	public final NumberState getState()
	{
		return privateState;
	}
	public final void setState(NumberState value)
	{
		privateState = value;
	}

	private boolean privateIncluded;
	public final boolean getIncluded()
	{
		return privateIncluded;
	}
	public final void setIncluded(boolean value)
	{
		privateIncluded = value;
	}

	private boolean privateExcluded;
	public final boolean getExcluded()
	{
		return privateExcluded;
	}
	public final void setExcluded(boolean value)
	{
		privateExcluded = value;
	}
}