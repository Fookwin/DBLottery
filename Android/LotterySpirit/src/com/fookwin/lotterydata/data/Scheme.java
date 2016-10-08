package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.AttributeUtil;
import com.fookwin.lotterydata.util.StringFormater;

public class Scheme
{
	public static int[][] indexMatrix = new int[][] {
        {201376,0,0,0,0},  //1
        {169911,31465,0,0,0},
        {142506,27405,4060,0,0},
        {118755,23751,3654,406,0},
        {98280,20475,3276,378,28}, //5
        {80730,17550,2925,351,27},
        {65780,14950,2600,325,26},
        {53130,12650,2300,300,25},
        {42504,10626,2024,276,24},
        {33649,8855,1771,253,23}, //10
        {26334,7315,1540,231,22},
        {20349,5985,1330,210,21},
        {15504,4845,1140,190,20},
        {11628,3876,969,171,19},
        {8568,3060,816,153,18}, //15
        {6188,2380,680,136,17},
        {4368,1820,560,120,16},
        {3003,1365,455,105,15},
        {2002,1001,364,91,14},
        {1287,715,286,78,13}, //20
        {792,495,220,66,12},
        {462,330,165,55,11},
        {252,210,120,45,10},
        {126,126,84,36,9},
        {56,70,56,28,8}, // 25
        {21,35,35,21,7},
        {6,15,20,15,6},
        {1,5,10,10,5},
        {0,1,4,6,4},
        {0,0,1,3,3}, // 30
        {0,0,0,1,2},
        {0,0,0,0,1},
        {0,0,0,0,0},
    };

    public static int[] getNumbersFromIndex(int index)
    {
        int[] numbers = new int[6];
        int pre_num = 0;
        for (int pos = 0; pos < 5; ++pos)
        {
            for (int num = pre_num; num < 33; ++num)
            {
                if (index > indexMatrix[num][pos])
                {
                    index -= indexMatrix[num][pos];
                }
                else
                {
                    numbers[pos] = num + 1;
                    pre_num = numbers[pos];
                    break;
                }
            }
        }

        numbers[5] = index + numbers[4]; // the last one.

        return numbers;
    }

    public static int getIndexFromNumbers(int[] numbers)
    {
        int index = 0, prv_num = 0;
        for (int pos = 0; pos < 6; ++pos)
        {
            int test_num = numbers[pos];

            if (pos == 5)
                index += test_num - prv_num;
            else
            {
                for (int num = prv_num + 1; num <= 33; ++num)
                {
                    if (num >= test_num)
                        break;

                    index += indexMatrix[num - 1][pos];
                }
            }

            prv_num = test_num;
        }

        return index;
    }
    
    public static Scheme parseFromString(String exp)
    {
        return new Scheme(exp);
    }
    
    public static Scheme parseFromIdentifier(String str)
    {
		String[] subs = str.split("\\+");
		if (subs.length != 2)
			return null;
		
        int[] numbers = getNumbersFromIndex(Integer.parseInt(subs[0]));
        int blue = Byte.parseByte(subs[1]);
        return new Scheme(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5], blue);
    }
    
	// Data...
	protected int _hightBits = 0; // for the reds except 33.
	protected byte _lowBit = 0; // for the blue and red 33.

	public Scheme()
	{
	}

	public Scheme(int highBits, byte lowBits)
	{
		_hightBits = highBits;
		_lowBit = lowBits;
	}
	
	public Scheme(String text)
	{
		String[] nums = text.split("[ +,]", -1);
		if (nums.length == 7)
		{
			int init = (int)1;
			for (int i = 0; i < 5; ++i)
			{
				_hightBits |= init << (Integer.parseInt(nums[i]) - 1);
			}

			int red6 = Integer.parseInt(nums[5]);
			if (red6 != 33)
			{
				_hightBits |= init << (red6 - 1);
			}

			_lowBit |= Byte.parseByte(nums[6]);
			if (red6 == 33)
			{
				_lowBit |= (byte)128;
			}
		}
	}

	public Scheme(Scheme from)
	{
		_hightBits = from._hightBits;
		_lowBit = from._lowBit;
	}

	public Scheme(int red1, int red2, int red3, int red4, int red5, int red6, int blue)
	{
		init(red1, red2, red3, red4, red5, red6, blue);
	}
	
	private void init(int red1, int red2, int red3, int red4, int red5, int red6, int blue)
	{
		// High bits...
		int init = (int)1;
		_hightBits |= init << (red1 - 1);
		_hightBits |= init << (red2 - 1);
		_hightBits |= init << (red3 - 1);
		_hightBits |= init << (red4 - 1);
		_hightBits |= init << (red5 - 1);
		if (red6 != 33)
		{
			_hightBits |= init << (red6 - 1);
		}


		// Low bits...
		_lowBit = (byte)blue;

		if (red6 == 33)
		{
			_lowBit |= (byte)128;
		}
	}

	public final int[] GetRedNums()
	{
		int[] _redNums = new int[6];

		int index = 0;
		int init = (int)1;
		for (int i = 0; i < 32; ++i)
		{
			if ((_hightBits & (init << i)) != 0)
			{
				_redNums[index++] = i + 1;

				if (index == 6)
				{
					break;
				}
			}
		}

		if (index == 5)
		{
			_redNums[5] = 33;
		}

		return _redNums;
	}

	public final boolean IsValid()
	{
		if (getBlue() > 16 || getBlue() < 1)
		{
			return false;
		}

		int[] reds = GetRedNums();

		int previous = 0;
		for (int num : reds)
		{
			if (num <= previous || num > 33)
			{
				return false;
			}

			previous = num;
		}

		return true;
	}
	
	public final int getIndex()
	{
		int[] reds = GetRedNums();
		return getIndexFromNumbers(reds);
	}

	public final int getBlue()
	{
		return (int)(_lowBit & (byte)127);
	}

	public final String getBlueExp()
	{
		return StringFormater.padLeft(Integer.toString(getBlue()), 2, '0');
	}
	
	public final String getRedExp(int index)
	{
		return StringFormater.padLeft(Integer.toString(Red(index)), 2, '0');
	}

	// Properties...
	public final int getContinuity()
	{
		int temp = _hightBits & (_hightBits >> 1);

		int conti = 0;
		while (temp != 0)
		{
			++conti;

			temp &= temp - 1;
		}

		if (Contain33() && (_hightBits & ((int)1 << 31)) != 0)
		{
			++conti;
		}

		return conti;
	}

	public final int getSum()
	{
		int[] reds = GetRedNums();

		int _iSum = 0;
		for (int num : reds)
		{
			_iSum += num;
		}

		return _iSum;
	}

	public final int getEvenNumCount()
	{
		int temp = _hightBits & 0xAAAAAAAA;

		int even = 0;
		while (temp != 0)
		{
			++even;

			temp &= temp - 1;
		}

		return even;
	}

	public final int getPrimeNumCount()
	{
		int init = 1;

		int mask = 1;
		mask |= init << 1;
		mask |= init << 2;
		mask |= init << 4;
		mask |= init << 6;
		mask |= init << 10;
		mask |= init << 12;
		mask |= init << 16;
		mask |= init << 18;
		mask |= init << 22;
		mask |= init << 28;
		mask |= init << 30;

		int temp = _hightBits & mask;

		int prime = 0;
		while (temp != 0)
		{
			++prime;

			temp &= temp - 1;
		}

		return prime;
	}

	public final int getSmallNumCount()
	{
		int temp = _hightBits & (((int)1 << 16) - 1);

		int small = 0;
		while (temp != 0)
		{
			++small;

			temp &= temp - 1;
		}

		return small;
	}

	public final int getOddSum()
	{
		int[] reds = GetRedNums();

		int _iOddSum = 0;
		for (int num : reds)
		{
			if (num%2 == 1)
			{
				_iOddSum += num;
			}
		}

		return _iOddSum;
	}

	public final int getEvenSum()
	{
		int[] reds = GetRedNums();

		int _iEvenSum = 0;
		for (int num : reds)
		{
			if (num % 2 == 0)
			{
				_iEvenSum += num;
			}
		}

		return _iEvenSum;
	}

	public final int getPrimeSum()
	{
		int[] reds = GetRedNums();

		int _iPrimeSum = 0;
		for (int num : reds)
		{
			if (AttributeUtil.IsPrime(num))
			{
				_iPrimeSum += num;
			}
		}

		return _iPrimeSum;
	}

	public final int getCompositSum()
	{
		int[] reds = GetRedNums();

		int _iCompositSum = 0;
		for (int num : reds)
		{
			if (!AttributeUtil.IsPrime(num))
			{
				_iCompositSum += num;
			}
		}

		return _iCompositSum;
	}

	public final boolean Contains(int num)
	{
		if (num == 33)
		{
			return Contain33();
		}

		return (_hightBits & ((int)1 << (num - 1))) != 0 ? true : false;
	}

	public final int Similarity(Scheme other)
	{
		int temp = _hightBits & other._hightBits;

		int siml = 0;
		while (temp != 0)
		{
			++siml;

			temp &= temp - 1;
		}

		if (Contain33() && other.Contain33())
		{
			++siml;
		}

		return siml;
	}

	private boolean Contain33()
	{
		return (_lowBit & 128) != 0;
	}

	public final int Red(int index) //0~5
	{
		int[] reds = GetRedNums();
		return reds[index];
	}

	public final String getRedsExp()
	{
		String str = "";

		int[] reds = GetRedNums();
		for (int red : reds)
		{
			str += StringFormater.padLeft(Integer.toString(red), 2, '0') + " ";
		}

		return StringFormater.trimEnd(str, ' ');
	}

	public final String getDisplayExpression()
	{
		String str = "";

		int[] reds = GetRedNums();
		for (int red : reds)
		{
			str += StringFormater.padLeft(Integer.toString(red), 2, '0') + " ";
		}

		return str + "+ " + StringFormater.padLeft(Integer.toString(getBlue()), 2, '0');
	}
	
	public final String getVerifiedExpression(Scheme compare)
	{
		String result = "";
		int[] reds = GetRedNums();
		for (int i = 0; i < 6; ++i)
		{
			String num = StringFormater.padLeft(Integer.toString(reds[i]), 2, '0');			
			if(compare.Contains(reds[i]))
			{
				result += "<font color='red'>" + num + " </font>";
			}
			else
			{
				result += num + " ";
			}
		}

		String blueNum = StringFormater.padLeft(Integer.toString(getBlue()), 2, '0');
		if(compare.getBlue() == getBlue())
		{
			result += "+ " + "<font color='blue'>" + blueNum + "</font>";
		}
		else
		{
			result += "+ " + blueNum;
		}		
		
		return result;
	}

	public final long getRedBits()
	{
		return _hightBits;
	}

	public final int GetRemainBy3(int[] output)
	{
		output[0] = 0;
		output[1] = 0;
		output[2] = 0;
		int[] reds = GetRedNums();
		for (int red : reds)
		{
			switch (red % 3)
			{
				case 0:
					output[0]++;
					break;
				case 1:
					output[1]++;
					break;
				case 2:
					output[2]++;
					break;
			}
		}

		return AttributeUtil.IndexOf3Div(output[0], output[1], output[2]);

	}

	// small-middle-big index
	public final int GetSmallMiddleBig(int[] output)
	{
		output[0] = 0;
		output[1] = 0;
		output[2] = 0;
		int[] reds = GetRedNums();
		for (int red : reds)
		{
			if (red <= 11)
			{
				output[0]++;
			}
			else if (red <= 22)
			{
				output[1]++;
			}
			else
			{
				output[2]++;
			}
		}

		return AttributeUtil.IndexOf3Div(output[0], output[1], output[2]);
	}

	@Override
	public String toString()
	{
		String str= "";

		int[] reds = GetRedNums();
		for (int red : reds)
		{
			str += StringFormater.padLeft(Integer.toString(red), 2, '0') + " ";
		}

		return StringFormater.trimEnd(str) + "+" + StringFormater.padLeft(Integer.toString(getBlue()), 2, '0');
	}

	public final String toString(String format)
	{
		if (format.length() == 0)
		{
			return toString();
		}

		String red_sep = " ", blue_sep = " ";

		char[] chars = format.toCharArray();
		if (chars.length == 2)
		{
			red_sep = Character.toString(chars[0]);
			blue_sep = Character.toString(chars[1]);
		}
		else
		{
			// Not supported format, switch to default.
			return toString();
		}

		String str = "";
		int[] reds = GetRedNums();
		for (int red : reds)
		{
			if (!str.equals(""))
			{
				str += red_sep;
			}

			str += StringFormater.padLeft(Integer.toString(red), 2, '0');
		}

		return str + blue_sep + StringFormater.padLeft(Integer.toString(getBlue()), 2, '0');
	}

	/** 
	 Get the value on the specific attribute.
	 
	 @param key attribute name
	 @param previousIssueIndex the index of the previous issue to be referenced by some attributes calcuation
	 @return 
	*/
	public final int Attribute(String key, int previousIssueIndex)
	{
		if (key.contains("Red_InPos_"))
		{
			int pos = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));
			return Red(pos - 1);
		}
		else if (key.contains("Red_Mantissa_InPos_"))
		{
			int pos = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));
			return Red(pos - 1) % 10;
		}
		else if (key.equals("Red_Mantissa_Max"))
		{
			int[] reds = GetRedNums();
			return Math.max(reds[0] % 10, Math.max(reds[1] % 10, Math.max(reds[2] % 10, Math.max(reds[3] % 10, Math.max(reds[4] % 10, reds[5] % 10)))));
		}
		else if (key.equals("Red_Mantissa_Min"))
		{
			int[] reds = GetRedNums();
			return Math.min(reds[0] % 10, Math.min(reds[1] % 10, Math.min(reds[2] % 10, Math.min(reds[3] % 10, Math.min(reds[4] % 10, reds[5] % 10)))));
		}
		else if (key.contains("Red_Repeat_Previous_"))
		{
			int step = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			if (previousIssueIndex - step >= 0)
			{
				Lottery refLot = DataManageBase.Instance().getHistory().getByIndex(previousIssueIndex - step);
				return refLot.getScheme().Similarity(this);
			}
			else
			{
				return 0;
			}
		}
		else if (key.equals("Red_Sum"))
		{
			return getSum();
		}
		else if (key.equals("Red_Sum_Mantissa"))
		{
			return getSum()%10;
		}
		else if (key.equals("Red_Odd_Sum"))
		{
			return getOddSum();
		}
		else if (key.equals("Red_Odd_Sum_Mantissa"))
		{
			return getOddSum()%10;
		}
		else if (key.equals("Red_Even_Sum"))
		{
			return getEvenSum();
		}
		else if (key.equals("Red_Even_Sum_Mantissa"))
		{
			return getEvenSum()%10;
		}
		else if (key.equals("Red_Composite_Sum"))
		{
			return getCompositSum();
		}
		else if (key.equals("Red_Composite_Sum_Mantissa"))
		{
			return getCompositSum()%10;
		}
		else if (key.equals("Red_Primary_Sum"))
		{
			return getPrimeSum();
		}
		else if (key.equals("Red_Primary_Sum_Mantissa"))
		{
			return getPrimeSum()%10;
		}
		else if (key.equals("Red_BigSmall"))
		{
			return 6-getSmallNumCount();
		}
		else if (key.equals("Red_OddEven"))
		{
			return 6-getEvenNumCount();
		}
		else if (key.equals("Red_PrimaryComposite"))
		{
			return getPrimeNumCount();
		}
		else if (key.contains("Red_Remainder_Devide"))
		{
			int div = Integer.parseInt(key.substring(key.length() - 3, key.length() - 3 + 1));
			int rmd = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int iHit = 0;
			int[] reds = GetRedNums();
			for (int red : reds)
			{
				if (rmd == red % div)
				{
					iHit++;
				}
			}
			return iHit;
		}
		else if (key.contains("Red_Sum_Pos"))
		{
			int pos1 = Integer.parseInt(key.substring(key.length() - 2, key.length() - 2 + 1));
			int pos2 = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int[] reds = GetRedNums();
			return reds[pos2-1] + reds[pos1-1];
		}
		else if (key.contains("Red_Diff_Pos"))
		{
			int pos1 = Integer.parseInt(key.substring(key.length() - 2, key.length() - 2 + 1));
			int pos2 = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int[] reds = GetRedNums();
			return reds[pos2 - 1] - reds[pos1 - 1];
		}
		else if (key.contains("Red_Zone_In"))
		{
			int div = Integer.parseInt(key.substring(key.length() - 5, key.length() - 5 + 2));
			int rmd = Integer.parseInt(key.substring(key.length() - 2, key.length() - 2 + 2));

			int iHit = 0;
			int[] reds = GetRedNums();
			for (int red : reds)
			{
				int divAt = 0;
				if (div == 3)
				{
					divAt = (red - 1) / 11 + 1;
				}
				else if (div == 4)
				{
					divAt = red < 17 ? (red - 1) / 8 + 1 : (red - 2) / 8 + 1;
				}
				else if (div == 7)
				{
					divAt = (red - 1) / 5 + 1;
				}
				else if (div == 11)
				{
					divAt = (red - 1) / 3 + 1;
				}

				if (rmd == divAt)
				{
					iHit++;
				}
			}
			return iHit;
		}
		else if (key.contains("Red_Row_"))
		{
			int row = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int iHit = 0;
			int[] reds = GetRedNums();
			for (int red : reds)
			{
				if (row == AttributeUtil.RedRow(red))
				{
					iHit++;
				}
			}

			return iHit;
		}
		else if (key.contains("Red_Column_"))
		{
			int col = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int iHit = 0;
			int[] reds = GetRedNums();
			for (int red : reds)
			{
				if (col == AttributeUtil.RedColumn(red))
				{
					iHit++;
				}
			}

			return iHit;
		}
		else if (key.contains("Red_Omission_"))
		{
			int num = Integer.parseInt(key.substring(key.length() - 2, key.length() - 2 + 2));
			return Contains(num) ? 1: 0;
		}
		else if (key.equals("Blue_Omission"))
		{
			return getBlue();
		}
		else if (key.equals("Blue_Amplitude"))
		{
			if (previousIssueIndex >= 0)
			{
				Lottery refLot = DataManageBase.Instance().getHistory().getByIndex(previousIssueIndex);
				return Math.abs(getBlue() - refLot.getScheme().getBlue());
			}
			else
			{
				return getBlue(); // treat the bule of prevouse as zero.
			}
		}
		else if (key.equals("Blue_Rows"))
		{
			return AttributeUtil.BlueRow(getBlue());
		}
		else if (key.equals("Blue_Columns"))
		{
			return AttributeUtil.BlueColumn(getBlue());
		}
		else if (key.equals("Blue_Mantissa_Omission"))
		{
			return getBlue()%10;
		}
		else if (key.contains("Blue_Mantissa_Repeat_Previous_"))
		{
			int step = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			if (previousIssueIndex - step >= 0)
			{
				Lottery refLot = DataManageBase.Instance().getHistory().getByIndex(previousIssueIndex - step);
				return refLot.getScheme().getBlue()%10 == getBlue()%10 ? 1 : 0;
			}
			else
			{
				return 0;
			}
		}
		else if (key.equals("Red_012"))
		{
			int[] temp = new int[] { 0,0,0 };
			int tempVar = GetRemainBy3(temp);
			return tempVar;
		}
		else if (key.equals("Red_SmallMidBig"))
		{
			int countIn0 = 0, countIn1 = 0, countIn2 = 0;
			int[] reds = GetRedNums();
			for (int red : reds)
			{
				switch ((red - 1) / 11)
				{
					case 0:
						countIn0++;
						break;
					case 1:
						countIn1++;
						break;
					case 2:
						countIn2++;
						break;
				}
			}

			return AttributeUtil.IndexOf3Div(countIn0, countIn1, countIn2);
		}
		else if (key.equals("Red_FuGeZhong"))
		{
			if (previousIssueIndex > 1)
			{
				Lottery lotPre1 = DataManageBase.Instance().getHistory().getByIndex(previousIssueIndex);
				Lottery lotPre2 = DataManageBase.Instance().getHistory().getByIndex(previousIssueIndex-1);

				int countIn0 = 0, countIn1 = 0, countIn2 = 0;
				int[] reds = GetRedNums();
				for (int red : reds)
				{
					if (lotPre1.getScheme().Contains(red))
					{
						countIn0++;
					}
					else if (lotPre2.getScheme().Contains(red))
					{
						countIn1++;
					}
					else
					{
						countIn2++;
					}
				}

				return AttributeUtil.IndexOf3Div(countIn0, countIn1, countIn2);
			}
			else
			{
				return AttributeUtil.IndexOf3Div(0, 0, 6);
			}
		}
		else if (key.contains("Red_5Xing_"))
		{
			int pos = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));
			return AttributeUtil.IndexOf5Xing(Red(pos - 1));
		}
		else if (key.contains("Red_PC_Pos"))
		{
			int pos1 = Integer.parseInt(key.substring(key.length() - 2, key.length() - 2 + 1));
			int pos2 = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int[] reds = GetRedNums();
			int A1 = AttributeUtil.IsPrime(reds[pos1 - 1]) ? 0 : 1;
			int A2 = AttributeUtil.IsPrime(reds[pos2 - 1]) ? 0 : 1;
			return A1 * 2 + A2 + 1;
		}
		else if (key.contains("Red_WX_Pos"))
		{
			int pos1 = Integer.parseInt(key.substring(key.length() - 2, key.length() - 2 + 1));
			int pos2 = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int[] reds = GetRedNums();
			int A1 = AttributeUtil.IndexOf5Xing(reds[pos1 - 1]);
			int A2 = AttributeUtil.IndexOf5Xing(reds[pos2 - 1]);
			return (A1 - 1) * 5 + A2;
		}
		else if (key.contains("Red_012_Pos"))
		{
			int pos1 = Integer.parseInt(key.substring(key.length() - 2, key.length() - 2 + 1));
			int pos2 = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int[] reds = GetRedNums();
			int A1 = reds[pos1 - 1]%3;
			int A2 = reds[pos2 - 1]%3;
			return A1 * 3 + A2 + 1;
		}
		else if (key.contains("Red_OE_Pos"))
		{
			int pos1 = Integer.parseInt(key.substring(key.length() - 2, key.length() - 2 + 1));
			int pos2 = Integer.parseInt(key.substring(key.length() - 1, key.length() - 1 + 1));

			int[] reds = GetRedNums();
			int A1 = (reds[pos1 - 1] + 1) % 2;
			int A2 = (reds[pos2 - 1] + 1) % 2;
			return A1 * 2 + A2 + 1;
		}
		else
		{
			throw new RuntimeException("The attribute is not supported");
		}
	}
}