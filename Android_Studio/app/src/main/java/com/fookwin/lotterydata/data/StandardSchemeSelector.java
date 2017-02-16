package com.fookwin.lotterydata.data;

import java.util.ArrayList;
import java.util.HashMap;

import android.annotation.SuppressLint;
import com.fookwin.lotterydata.util.SelectUtil;

public class StandardSchemeSelector extends SchemeSelector
{
	public enum RedBlueConnectionTypeEnum
	{
		Duplicate(0),
		OneToOneInOrder(1),
		OneToOneInRandom(2);

		private int intValue;
		private static HashMap<Integer, RedBlueConnectionTypeEnum> mappings;
		@SuppressLint("UseSparseArrays")
		private synchronized static HashMap<Integer, RedBlueConnectionTypeEnum> getMappings()
		{
			if (mappings == null)
			{
				mappings = new HashMap<Integer, RedBlueConnectionTypeEnum>();
			}
			return mappings;
		}

		private RedBlueConnectionTypeEnum(int value)
		{
			intValue = value;
			RedBlueConnectionTypeEnum.getMappings().put(value, this);
		}

		public int getValue()
		{
			return intValue;
		}

		public static RedBlueConnectionTypeEnum forValue(int value)
		{
			return getMappings().get(value);
		}
	}

	private Set selected_reds = new Set();
	private Set selected_blues = new Set();
	private boolean applyMatrixFilter = false;
	private RedBlueConnectionTypeEnum connectType = RedBlueConnectionTypeEnum.Duplicate;

	public final static StandardSchemeSelector createFrom(Scheme from)
	{
		StandardSchemeSelector simpleSelector = new StandardSchemeSelector();
		simpleSelector.setSelectedBlues(new Set(new Region(from.getBlue(), from.getBlue())));
		Set reds = new Set();
		for (int red : from.GetRedNums())
			reds.Add(red);
		
		simpleSelector.setSelectedReds(reds);
		
		return simpleSelector;
	}
	
	public final Set getSelectedReds()
	{
		return selected_reds;
	}
	public final void setSelectedReds(Set value)
	{
		selected_reds = value;
	}

	public final Set getSelectedBlues()
	{
		return selected_blues;
	}
	public final void setSelectedBlues(Set value)
	{
		selected_blues = value;
	}

	public final boolean getApplyMatrixFilter()
	{
		return applyMatrixFilter;
	}
	public final void setApplyMatrixFilter(boolean value)
	{
		applyMatrixFilter = value;
	}

	public final RedBlueConnectionTypeEnum getBlueConnectionType()
	{
		return connectType;
	}
	public final void setBlueConnectionType(RedBlueConnectionTypeEnum value)
	{
		connectType = value;
	}

	public StandardSchemeSelector()
	{
	}

	@Override
	public SchemeSelectorTypeEnum GetSelectorType()
	{
		return SchemeSelectorTypeEnum.StandardSelectorType;
	}

	@Override
	protected String GetExpression()
	{
		String exp = getSelectedReds().toString() + "+" + getSelectedBlues().toString();
		exp += "+" + Boolean.toString(applyMatrixFilter) + "+" + Integer.toString(connectType.getValue());
		return exp;
	}

	@Override
	public void ParseExpression(String exp)
	{
		String[] sub = exp.split("[+]", -1);
		setSelectedReds(new Set(sub[0]));
		setSelectedBlues(new Set(sub[1]));
		if (sub.length == 4)
		{
			applyMatrixFilter = Boolean.parseBoolean(sub[2]);
			connectType = RedBlueConnectionTypeEnum.forValue(Integer.parseInt(sub[3]));
		}
	}

	@Override
	protected String GetDisplayExpression()
	{
		String display = selected_reds.getCount() != 6 ? "[复式] " : "[单式] ";
		display += "[" + Integer.toString(this.GetSchemeCount()) + "注] ";
		display += selected_reds.getDisplayExpression();
		display += " : " + selected_blues.getDisplayExpression();

		switch (connectType)
		{
			case Duplicate:
				display += " <蓝球复式>";
				break;
			case OneToOneInOrder:
				display += " <顺序选择>";
				break;
			case OneToOneInRandom:
				display += " <随机选择>";
				break;
		}

		if (applyMatrixFilter)
		{
			display += " [选6中5缩水]";
		}

		return display;
	}

	@Override
	public int GetSchemeCount()
	{
		int redCount = getSelectedReds().getCount();
		if (redCount < 6)
		{
			return 0;
		}

		int blueCount = getSelectedBlues().getCount();
		if (blueCount <= 0)
		{
			return 0;
		}

		int count = 0;
		if (applyMatrixFilter)
		{
			count = DataManageBase.Instance().getMatrixTable().GetCellItemCount(redCount, 6);
		}
		else
		{
			final int devide = 720;

			count = 1;
			for (int i = 0; i < 6; ++i)
			{
				count *= redCount - i;
			}

			count /= devide;
		}

		return connectType == RedBlueConnectionTypeEnum.Duplicate ? count * blueCount : count;
	}

	@Override
	public ArrayList<Scheme> GetResult()
	{
		return SelectUtil.CalculateSchemeSelection(selected_reds, getSelectedBlues(), connectType != RedBlueConnectionTypeEnum.Duplicate, connectType == RedBlueConnectionTypeEnum.OneToOneInRandom, applyMatrixFilter);
	}

	@Override
	public SchemeSelector clone()
	{
		StandardSchemeSelector tempVar = new StandardSchemeSelector();
		tempVar.selected_reds = new Set(this.selected_reds);
		tempVar.selected_blues = new Set(this.selected_blues);
		tempVar.connectType = this.getBlueConnectionType();
		tempVar.applyMatrixFilter = this.applyMatrixFilter;
		SchemeSelector copy = tempVar;

		return copy;
	}

	@Override
	public void Reset()
	{
		selected_reds.Clear();
		selected_blues.Clear();
		applyMatrixFilter = false;
		connectType = RedBlueConnectionTypeEnum.Duplicate;
	}

	@Override
	public String HasError()
	{
		if (selected_reds.getCount() < 6)
		{
			return "至少需要选择6个红球";
		}

		if (selected_blues.getCount() < 1)
		{
			return "至少需要选择1个蓝球";
		}

		return "";
	}
}