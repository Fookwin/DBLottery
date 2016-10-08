package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.DBXmlNode;

public class RedNumSetConstraint extends Constraint
{
	private Set _selectSet = new Set();
	private Set _hitLimits = new Set();

	@Override
	public ConstraintTypeEnum GetConstraintType()
	{
		return ConstraintTypeEnum.RedNumSetConstraintType;
	}

	@Override
	public boolean Meet(Scheme num, int refIssueIndex)
	{
		int[] numbers = _selectSet.getNumbers();

		int iHit = 0;
		for (int test : numbers)
		{
			if (num.Contains(test))
			{
				iHit++;
			}
		}

		return _hitLimits.Contains(iHit);
	}

	@Override
	public Constraint clone()
	{
		RedNumSetConstraint tempVar = new RedNumSetConstraint();
		tempVar._selectSet = new Set(this._selectSet);
		tempVar._hitLimits = new Set(this._hitLimits);
		return tempVar;
	}

	public final Set getSelectSet()
	{
		return _selectSet;
	}
	public final void setSelectSet(Set value)
	{
		_selectSet = value;
	}

	public final Set getHitLimits()
	{
		return _hitLimits;
	}

	public final void setHitLimits(Set value)
	{
		_hitLimits = value;
	}

	@Override
	protected String GetDisplayExpression()
	{
		return "[号码组过滤] " + "红球" + "(" + _selectSet.getDisplayExpression() + ")" + "出现" + "(" + _hitLimits.getDisplayExpression() + ")" +"次";
	}

	@Override
	public void ReadFromXml(DBXmlNode node)
	{
		String selection = node.GetAttribute("SelectSet");
		_selectSet.Parse(selection);

		String limits = node.GetAttribute("HitLimits");
		_hitLimits.Parse(limits);
	}

	@Override
	public void WriteToXml(DBXmlNode node)
	{
		node.SetAttribute("SelectSet", _selectSet.toString());
		node.SetAttribute("HitLimits", _hitLimits.toString());
	}

	@Override
	public String HasError()
	{
		if (_selectSet.getCount() == 0)
		{
			return "请选择至少一个号码";
		}

		if (_hitLimits.getCount() == 0)
		{
			return "请选择所选号码出现的个数";
		}

		int[] values = _hitLimits.getNumbers();
		for (int val : values)
		{
			if (val > _selectSet.getCount())
			{
				return "出现个数超过了号码个数！";
			}
		}

		return "";
	}
}