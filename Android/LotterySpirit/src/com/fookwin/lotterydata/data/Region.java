package com.fookwin.lotterydata.data;

public class Region
{
	public Region()
	{
		m_iMin = 0;
		m_iStep = 0;
	}

	public Region(int single)
	{
		m_iMin = single;
		m_iStep = 0;
	}

	public Region(int iMin, int iMax)
	{
		Reset(iMin, iMax);
	}

	public Region(String str)
	{
		Parse(str);
	}
	
	public Region clone()
	{
		Region rg = new Region();
		rg.m_iMin = this.m_iMin;
		rg.m_iStep = this.m_iStep;
		
		return rg;
	}

	public final boolean equals(Region other)
	{
		if (this.m_iMin == other.m_iMin && this.m_iStep == other.m_iStep)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public final void Reset(int iMin, int iMax)
	{
		if (iMin >= 0 && iMax >= iMin)
		{
			m_iMin = iMin;
			m_iStep = iMax - iMin;
		}
		else
		{
			throw new RuntimeException("Both Min and Step must not be less than zero.");
		}
	}

	public final void Reset(Region other)
	{
		Reset(other.getMin(), other.getMax());
	}

	// Result:  1 - larger than the specific num or ragion.
	//          -1 - smaller than the specific num or ragion
	//          0 - contains the number or cross with the specific ragion 
	public final int CompareTo(int iNum)
	{
		return CompareTo(new Region(iNum));
	}

	public final int CompareTo(Region other)
	{
		if (m_iMin > other.m_iMin + other.m_iStep)
		{
			return 1;
		}
		else if (m_iMin + m_iStep < other.m_iMin)
		{
			return -1;
		}
		else
		{
			return 0;
		}
	}

	@Override
	public String toString()
	{
		String str;
		if (m_iStep == 0)
		{
			str = Integer.toString(m_iMin);
		}
		else
		{
			str = Integer.toString(m_iMin) + "~" + Integer.toString(m_iMin + m_iStep);
		}
		return str;
	}

	public final int[] getNumbers()
	{
		int[] nums = new int[m_iStep + 1];
		for (int i = 0; i <= m_iStep; ++i)
		{
			nums[i] = i + m_iMin;
		}

		return nums;
	}

	public final void Parse(String str)
	{
		int iBreak = str.indexOf('~');
		if (iBreak < 0)
		{
			// a single number.
			m_iMin = Integer.parseInt(str);
			m_iStep = 0;
		}
		else
		{
			String strMin = str.substring(0, iBreak);
			String strMax = str.substring(iBreak + 1, iBreak + 1 + str.length() - iBreak - 1);
			m_iMin = Integer.parseInt(strMin);
			m_iStep = Integer.parseInt(strMax) - m_iMin;
			if (m_iStep < 0 || m_iMin < 0)
			{
				throw new RuntimeException("Input string could not be parsed as a valid ragion.");
			}
		}
	}

	public final int getMin()
	{
		return m_iMin;
	}

	public final int getMax()
	{
		return m_iMin + m_iStep;
	}

	public final int getStep()
	{
		return m_iStep;
	}

	public final void Extend(int head, int tail)
	{
		m_iMin -= head;
		m_iStep += head;
		m_iStep += tail;
	}

	public final boolean Merge(Region other)
	{
		if (m_iMin > other.getMax() + 1 || getMax() + 1 < other.getMin())
		{
			return false;
		}

		int tempMax = getMax();

		m_iMin = Math.min(getMin(), other.getMin());
		m_iStep = Math.max(tempMax, other.getMax()) - m_iMin;

		return true;
	}

	public final boolean Contains(int iNum)
	{
		return m_iMin <= iNum && getMax() >= iNum;
	}

	private int m_iMin;
	private int m_iStep; // iStep == 0, means it represents a single number.
}