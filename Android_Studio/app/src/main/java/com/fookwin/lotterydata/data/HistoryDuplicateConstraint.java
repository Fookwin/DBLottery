package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class HistoryDuplicateConstraint extends Constraint
{
	private int _referencedIssueCount = -1; // -1 for all.
	private int _condition = 6;

	@Override
	public ConstraintTypeEnum GetConstraintType()
	{
		return ConstraintTypeEnum.HistoryDuplicateConstraintType;
	}

	@Override
	public boolean Meet(Scheme num, int refIssueIndex)
	{
		History history = DataManageBase.Instance().getHistory();

		int index = _referencedIssueCount < 0 ? 0 : refIssueIndex - _referencedIssueCount + 1;

		for (; index <= refIssueIndex; ++index)
		{
			if (num.Similarity(history.getByIndex(index).getScheme()) >= _condition)
			{
				return false;
			}
		}

		return true;
	}

	@Override
	public Constraint clone()
	{
		HistoryDuplicateConstraint tempVar = new HistoryDuplicateConstraint();
		tempVar._referencedIssueCount = this._referencedIssueCount;
		tempVar._condition = this._condition;
		return tempVar;
	}

	public final int getReferenceCount()
	{
		return _referencedIssueCount;
	}
	public final void setReferenceCount(int value)
	{
		_referencedIssueCount = value;
	}

	public final int getExcludeCondition()
	{
		return _condition;
	}

	public final void setExcludeCondition(int value)
	{
		_condition = value;
	}

	@Override
	protected String GetDisplayExpression()
	{
		String reference = _referencedIssueCount < 0 ? "[所有开奖中]" : "[最近" + Integer.toString(_referencedIssueCount) + "期]";
		return "[历史过滤] " + "排除与" + reference + "有[" + Integer.toString(_condition) + "]个以上红球相同的号码组";
	}

	@Override
	public void ReadFromXml(DBXmlNode node)
	{
		_referencedIssueCount = Integer.parseInt(node.GetAttribute("Reference"));

		_condition = Integer.parseInt(node.GetAttribute("Condition"));
	}

	@Override
	public void WriteToXml(DBXmlNode node)
	{
		node.SetAttribute("Reference", Integer.toString(_referencedIssueCount));
		node.SetAttribute("Condition", Integer.toString(_condition));
	}

	@Override
	public String HasError()
	{
		return ""; // should no error for any value.
	}
}