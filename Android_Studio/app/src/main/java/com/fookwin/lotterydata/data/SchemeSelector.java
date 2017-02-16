package com.fookwin.lotterydata.data;

import java.util.ArrayList;

public abstract class SchemeSelector
{
	public final String getExpression()
	{
		return GetExpression();
	}

	public final String getDisplayExpression()
	{
		return GetDisplayExpression();
	}

	public abstract SchemeSelectorTypeEnum GetSelectorType();
	protected abstract String GetExpression();
	public abstract void ParseExpression(String exp);
	protected abstract String GetDisplayExpression();
	public abstract ArrayList<Scheme> GetResult();
	public abstract int GetSchemeCount();
	public abstract SchemeSelector clone();
	public abstract void Reset();
	public abstract String HasError();

	public static SchemeSelector CreateSelector(SchemeSelectorTypeEnum type)
	{
		switch (type)
		{
			case StandardSelectorType:
				return new StandardSchemeSelector();
			case DantuoSelectorType:
				return new DantuoSchemeSelector();
			case RandomSelectorType:
				return new RandomSchemeSelector();
			case UploadSelectorType:
			default:
				throw new RuntimeException("Unsupported selector type.");
		}
	}
}