package com.fookwin.lotterydata.data;

import java.util.ArrayList;

import com.fookwin.lotterydata.util.SelectUtil;

public class RandomSchemeSelector extends SchemeSelector
{
	private Set included_reds = new Set();
	private Set excluded_reds = new Set();
	private Set included_blues = new Set();
	private Set excluded_blues = new Set();
	private int selectedCount = 1;
	private ArrayList<Scheme> _result = new ArrayList<Scheme>();

	public final ArrayList<Scheme> getResultList()
	{
		return _result;
	}
	
	public void random(boolean rebuild)
	{
		if (rebuild)
		{
			_result.clear();
			SelectUtil.RandomSchemes(selectedCount, this, _result);
		}
		else
		{
			int currentSize = _result.size();
			if (currentSize == selectedCount)
				return;
			
			if (currentSize < selectedCount)
			{
				SelectUtil.RandomSchemes(selectedCount - currentSize, this, _result);
			}
			else
			{
				// remove redundant from tail.
				while (currentSize > selectedCount)
				{
					_result.remove(-- currentSize);
				}
			}
		}
		
	}
	
	public final int getSelectedCount()
	{
		return selectedCount;
	}
	public final void setSelectedCount(int value)
	{
		selectedCount = value;
	}

	public final Set getIncludedReds()
	{
		return included_reds;
	}
	public final void setIncludedReds(Set value)
	{
		included_reds = value;
	}

	public final Set getExcludedReds()
	{
		return excluded_reds;
	}
	public final void setExcludedReds(Set value)
	{
		excluded_reds = value;
	}

	public final Set getIncludedBlues()
	{
		return included_blues;
	}
	public final void setIncludedBlues(Set value)
	{
		included_blues = value;
	}

	public final Set getExcludedBlues()
	{
		return excluded_blues;
	}
	public final void setExcludedBlues(Set value)
	{
		excluded_blues = value;
	}

	public RandomSchemeSelector()
	{
	}

	@Override
	public SchemeSelectorTypeEnum GetSelectorType()
	{
		return SchemeSelectorTypeEnum.RandomSelectorType;
	}

	@Override
	protected String GetExpression()
	{
		String expression = "";
		expression += String.valueOf(selectedCount);
		
		expression += "|";
		
		if (included_reds.getCount() > 0)
		{
			expression += "(" + included_reds.toString() + ")";
		}

		if (excluded_reds.getCount() > 0)
		{
			expression += "[" + excluded_reds.toString() + "]";
		}

		expression += "|";

		if (included_blues.getCount() > 0)
		{
			expression += "(" + included_blues.toString() + ")";
		}

		if (excluded_blues.getCount() > 0)
		{
			expression += "[" + excluded_blues.toString() + "]";
		}
		
		// set result.
		expression += "|";
		
		ArrayList<Scheme> result = GetResult();
		if (result.size() > 0)
		{
			expression += "{";
			
			int count = result.size();
			for (int i = 0; i < count; ++ i)
			{
				if (i > 0)
					expression += " ";
				
				Scheme sm = result.get(i);
				expression += Long.toString(sm.getIndex()) + "+" + Integer.toString(sm.getBlue());
			}
			expression += "}";
		}

		return expression;
	}

	@Override
	public void ParseExpression(String exp)
	{
		String[] segments = exp.split("\\|", -1);
		if (segments == null || segments.length != 4)
			return;
		
		selectedCount = Integer.valueOf(segments[0]);
		
		String reds = segments[1];
		if (reds.length() > 0)
		{
			int separator1 = reds.indexOf(')');
			if (separator1 > 0)
			{
				// included...
				included_reds = new Set(reds.substring(1, separator1));
			}

			int separator2 = reds.indexOf('[');
			if (separator2 > 0)
			{
				// excluded...
				excluded_reds = new Set(reds.substring(separator2 + 1, separator2 + 1 + reds.length() - separator2 - 2));
			}
		}

		String blues = segments[2];
		if (blues.length() > 0)
		{
			int separator1 = blues.indexOf(')');
			if (separator1 > 0)
			{
				// included...
				included_blues = new Set(blues.substring(1, separator1));
			}

			int separator2 = blues.indexOf('[');
			if (separator2 > 0)
			{
				// excluded...
				excluded_blues = new Set(blues.substring(separator2 + 1, separator2 + 1 + blues.length() - separator2 - 2));
			}
		}
		
		String result = segments[3];
		if (result.length() > 0)
		{
			// remove "{}";
			result = result.substring(1, result.length() - 2);

			String[] subStrings = result.split(" ");
			for (String line : subStrings)
			{
				Scheme item = Scheme.parseFromIdentifier(line);
				if (item != null)
				{
					_result.add(item);
				}
			}
		}
	}

	@Override
	public int GetSchemeCount()
	{
		if (excluded_reds.getCount() <= 27 && excluded_blues.getCount() <= 15)
		{
			return selectedCount;
		}

		return 0;
	}

	@Override
	public ArrayList<Scheme> GetResult()
	{
		random(false); // make sure the result is correct.
		
		return _result;
	}

	@Override
	public SchemeSelector clone()
	{
		RandomSchemeSelector tempVar = new RandomSchemeSelector();
		tempVar.included_reds = new Set(this.included_reds);
		tempVar.excluded_reds = new Set(this.excluded_reds);
		tempVar.included_blues = new Set(this.included_blues);
		tempVar.excluded_blues = new Set(this.excluded_blues);
		tempVar.selectedCount = this.selectedCount;
		tempVar._result.addAll(this._result);
		SchemeSelector copy = tempVar;

		return copy;
	}

	@Override
	public void Reset()
	{
		included_reds.Clear();
		excluded_reds.Clear();
		included_blues.Clear();
		excluded_blues.Clear();
		_result.clear();
		selectedCount = 1;
	}

	@Override
	protected String GetDisplayExpression()
	{
		String display = "[随机] ";
		display += "[" + Integer.toString(selectedCount) + "注] ";
		
		if (included_reds.getCount() > 0)
		{
			display += "<红胆 ";
			display +=  included_reds.toString() + ">";
		}

		if (excluded_reds.getCount() > 0)
		{
			display += "<红杀 ";
			display += excluded_reds.toString() + ">";
		}

		if (included_blues.getCount() > 0)
		{
			display += "<蓝胆 ";
			display += included_blues.toString() + ">";
		}

		if (excluded_blues.getCount() > 0)
		{
			display += "<蓝杀 ";
			display += excluded_blues.toString() + ">";
		}

		return display;
	}

	@Override
	public String HasError()
	{
		if (included_reds.getCount() > 6)
		{
			return "选为必出的红球不能超过6个";
		}

		if (excluded_reds.getCount() > 27)
		{
			return "选为不出的红球不能超过27个";
		}

		if (included_blues.getCount() > 1)
		{
			return "选为必出蓝球只能有1个";
		}

		if (excluded_blues.getCount() > 15)
		{
			return "选为不出的蓝球不能超过15个";
		}

		if(selectedCount == 0)
		{
			return "至少要选一注";
		}

		return "";
	}
}