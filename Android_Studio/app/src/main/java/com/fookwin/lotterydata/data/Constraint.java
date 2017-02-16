package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public abstract class Constraint
{
	public Constraint()
	{
	}

	public boolean Meet(Scheme lucyNum, int refIssueIndex)
	{
		return false;
	}

	public final String getDisplayExpression()
	{
		return GetDisplayExpression();
	}

	public abstract Constraint clone();
	public abstract void ReadFromXml(DBXmlNode node);
	public abstract void WriteToXml(DBXmlNode node);
	public abstract ConstraintTypeEnum GetConstraintType();
	protected abstract String GetDisplayExpression();
	public abstract String HasError();

	public static Constraint CreateConstraint(ConstraintTypeEnum type)
	{
		switch (type)
		{
			case RedNumSetConstraintType:
				return new RedNumSetConstraint();
			case SchemeAttributeConstraintType:
				return new SchemeAttributeConstraint();
			case HistoryDuplicateConstraintType:
				return new HistoryDuplicateConstraint();
			default:
				throw new RuntimeException("Unsupported constraint type.");
		}
	}
}