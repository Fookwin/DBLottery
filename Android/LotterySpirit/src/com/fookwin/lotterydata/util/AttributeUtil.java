package com.fookwin.lotterydata.util;

import com.fookwin.lotterydata.data.SchemeAttribute;
import com.fookwin.lotterydata.data.SchemeAttributeCategory;
import com.fookwin.lotterydata.data.SchemeAttributes;

public final class AttributeUtil
{
	private static final int[] _redMatrixRow = new int[] { 4, 4, 3, 3, 3, 4, 5, 5, 5, 5, 4, 3, 2, 2, 2, 2, 2, 3, 4, 5, 6, 6, 6, 6, 6, 6, 5, 4, 3, 2, 1, 1, 1 };
	private static final int[] _redMatrixColumn = new int[] { 4, 3, 3, 4, 5, 5, 5, 4, 3, 2, 2, 2, 2, 3, 4, 5, 6, 6, 6, 6, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1, 1, 2, 3 };
	private static final int[] _blueMatrixRow = new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4 };
	private static final int[] _blueMatrixColumn = new int[] { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 };
	private static java.util.ArrayList<String> _matrix_012 = new java.util.ArrayList<String>(java.util.Arrays.asList(new String[] {"0-0-6","0-1-5", "0-2-4", "0-3-3", "0-4-2", "0-5-1", "0-6-0", "1-0-5", "1-1-4", "1-2-3", "1-3-2", "1-4-1", "1-5-0", "2-0-4", "2-1-3", "2-2-2", "2-3-1", "2-4-0", "3-0-3", "3-1-2", "3-2-1", "3-3-0", "4-0-2", "4-1-1", "4-2-0", "5-0-1", "5-1-0", "6-0-0" }));

	private static java.util.ArrayList<String> _attributeKeys = null;
	private static SchemeAttributes _attributeTemplate = null;

	public static java.util.ArrayList<String> AttributeKeys()
	{
		if (_attributeKeys == null)
		{
			_attributeKeys = new java.util.ArrayList<String>();
			for (java.util.Map.Entry<String, SchemeAttributeCategory> cat : GetAttributesTemplate().getCategories().entrySet())
			{
				for (java.util.Map.Entry<String, SchemeAttribute> attri : cat.getValue().getAttributes().entrySet())
				{
					_attributeKeys.add(attri.getKey());
				}
			}
		}

		return _attributeKeys;
	}

	public static SchemeAttributes GetAttributesTemplate()
	{
		return _attributeTemplate;
	}

	public static void SetAttributesTemplate(SchemeAttributes template)
	{
		_attributeTemplate = template;
	}

	public static SchemeAttribute GetSchemeAttribute(String attriKey)
	{
		if (_attributeTemplate == null)
		{
			throw new RuntimeException("Attribute Template was not initialized.");
		}

		return _attributeTemplate.Attribute(attriKey);
	}

	public static int RedRow(int red)
	{
		return _redMatrixRow[red - 1];
	}

	public static int RedColumn(int red)
	{
		return _redMatrixColumn[red - 1];
	}

	public static int BlueRow(int red)
	{
		return _blueMatrixRow[red - 1];
	}

	public static int BlueColumn(int red)
	{
		return _blueMatrixColumn[red - 1];
	}

	public static int IndexOfSum(int sum)
	{
		return sum > 140 ? 8 : (sum < 71 ? 0 : (sum - 61) / 10);
	}

	public static int IndexOf3Div(int countIn0, int countIn1, int countIn2)
	{
		String exp = Integer.toString(countIn0) + "-" + Integer.toString(countIn1) + "-" + Integer.toString(countIn2);
		return _matrix_012.indexOf(exp) + 1;
	}

	public static int IndexOf5Xing(int red)
	{
		String sRed = StringFormater.padLeft(Integer.toString(red), 2, '0');
		if ((new String("09 10 21 22 33")).contains(sRed))
		{
			return 1;
		}
		else if ((new String("03 04 15 16 27 28")).contains(sRed))
		{
			return 2;
		}
		else if ((new String("01 12 13 24 25")).contains(sRed))
		{
			return 3;
		}
		else if ((new String("06 07 18 19 30 31")).contains(sRed))
		{
			return 4;
		}
		else //"02 05 08 11 14 17 20 23 26 29 32"
		{
			return 5;
		}
	}

	public static String IndexOf3DivToString(int index)
	{
		return _matrix_012.get(index - 1);
	}

	public static boolean IsPrime(int num)
	{
		if (num == 1 || num == 2 || num == 3 || num == 5 || num == 7 || num == 11 || num == 13 || num == 17 || num == 19 || num == 23 || num == 29 || num == 31)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}