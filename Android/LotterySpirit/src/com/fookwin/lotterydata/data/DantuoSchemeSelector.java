package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.SelectUtil;

public class DantuoSchemeSelector extends SchemeSelector
{
	private Set selected_tuos = new Set();
	private Set selected_dans = new Set();
	private Set selected_blues = new Set();

	public final Set getSelectedTuos()
	{
		return selected_tuos;
	}
	public final void setSelectedTuos(Set value)
	{
		selected_tuos = value;
	}

	public final Set getSelectedDans()
	{
		return selected_dans;
	}
	public final void setSelectedDans(Set value)
	{
		selected_dans = value;
	}

	public final Set getSelectedBlues()
	{
		return selected_blues;
	}
	public final void setSelectedBlues(Set value)
	{
		selected_blues = value;
	}

	public DantuoSchemeSelector()
	{
	}

	@Override
	public SchemeSelectorTypeEnum GetSelectorType()
	{
		return SchemeSelectorTypeEnum.DantuoSelectorType;
	}

	@Override
	protected String GetExpression()
	{
		String expression = "";
		if (selected_dans.getCount() > 0)
		{
			expression = "(" + selected_dans.toString() + ")";
		}

		return expression + getSelectedTuos().toString() + "+" + getSelectedBlues().toString();
	}

	@Override
	public void ParseExpression(String exp)
	{
		int separator1 = exp.indexOf(')');
		if (separator1 > 0)
		{
			selected_dans = new Set(exp.substring(1, separator1));
		}

		int separator2 = exp.indexOf('+');
		setSelectedTuos(new Set(exp.substring(separator1 + 1, separator1 + 1 + separator2 - separator1 - 1)));
		setSelectedBlues(new Set(exp.substring(separator2 + 1, separator2 + 1 + exp.length() - separator2 - 1)));
	}

	@Override
	public int GetSchemeCount()
	{
		int tuoCount = getSelectedTuos().getCount();
		if (tuoCount + selected_dans.getCount() < 6)
		{
			return 0;
		}

		int blueCount = getSelectedBlues().getCount();
		if (blueCount <= 0)
		{
			return 0;
		}

		int danCount = selected_dans.getCount();
		if (danCount == 0)
		{
			final int devide = 720;

			int total = 1;
			for (int i = 0; i < 6; ++i)
			{
				total *= tuoCount - i;
			}

			total /= devide;

			return total * blueCount;
		}
		else
		{
			int total = 1;
			for (int i = 0; i < (6 - danCount); ++i)
			{
				total *= tuoCount - i;
			}

			int devide = 1;
			for (int i = 6 - danCount; i > 0; --i)
			{
				devide *= i;
			}

			total /= devide;

			return total * blueCount;
		}
	}

	@Override
	public java.util.ArrayList<Scheme> GetResult()
	{
		return SelectUtil.CalculateSchemeSelection(selected_dans, selected_tuos, getSelectedBlues());
	}

	@Override
	public SchemeSelector clone()
	{
		DantuoSchemeSelector tempVar = new DantuoSchemeSelector();
		tempVar.selected_tuos = new Set(this.selected_tuos);
		tempVar.selected_dans = new Set(this.selected_dans);
		tempVar.selected_blues = new Set(this.selected_blues);
		SchemeSelector copy = tempVar;

		return copy;
	}

	@Override
	public void Reset()
	{
		selected_tuos.Clear();
		selected_dans.Clear();
		selected_blues.Clear();
	}

	@Override
	protected String GetDisplayExpression()
	{
		String display = "[胆拖] ";
		display += "[" + Integer.toString(GetSchemeCount()) + "注] ";
		display += "(" + selected_dans.getDisplayExpression() + ") ";
		display += selected_tuos.getDisplayExpression();
		display += " : " + selected_blues.getDisplayExpression();
		return display;
	}

	@Override
	public String HasError()
	{
		if (selected_dans.getCount() + selected_tuos.getCount() < 6)
		{
			return "至少需要选择6个红球";
		}

		if (selected_dans.getCount() > 5)
		{
			return "最多只能选5个胆号红球";
		}

		if (selected_blues.getCount() < 1)
		{
			return "至少需要选择1个蓝球";
		}

		return "";
	}
}