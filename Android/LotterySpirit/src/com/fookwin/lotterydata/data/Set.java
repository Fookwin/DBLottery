package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.StringFormater;

public class Set
{
	protected java.util.ArrayList<Region> m_ValidNumbers = new java.util.ArrayList<Region>();

	public Set()
	{
	}

	public Set(Region ragion)
	{
		Reset(ragion);
	}

	public Set(int[] nums)
	{
		Reset(nums);
	}

	public Set(String str)
	{
		Parse(str);
	}

	public Set(Set from)
	{
		Reset(from);
	}

	public final void Parse(String str)
	{
		Reset(str);
	}

	@Override
	public String toString()
	{
		String str = "";
		for (Region condi : m_ValidNumbers)
		{
			if (str.length() > 0)
			{
				str += ",";
			}

			str += condi.toString();
		}

		return str;
	}

	public final void Reset(Set from)
	{
		m_ValidNumbers.clear();

		for (Region item : from.m_ValidNumbers)
		{
			m_ValidNumbers.add(item);
		}
	}

	public final void Reset(Region ragion)
	{
		m_ValidNumbers.clear();
		m_ValidNumbers.add(ragion);
	}

	// the numbers should be ordered from smaller to larger and no duplication.
	public final void Reset(int[] nums)
	{
		m_ValidNumbers.clear();

		Region tempRagion = null;
		for (int num : nums)
		{
			if (tempRagion == null)
			{
				// init temp.
				tempRagion = new Region(num);
			}
			else
			{
				int comp = tempRagion.CompareTo(num);
				if (comp >= 0)
				{
					Clear();
					throw new RuntimeException("Input nums should be sorted by numeric order and no duplication.");
				}

				// be connected with the ragion?
				if (num == tempRagion.getMax() + 1)
				{
					tempRagion.Extend(0, 1);
				}
				else
				{
					// submit existing and adding a new ragion.
					m_ValidNumbers.add(tempRagion);

					// reset temp
					tempRagion = new Region(num);
				}
			}
		}

		// submit last one.
		m_ValidNumbers.add(tempRagion);
	}

	public final void Reset(String str)
	{
		m_ValidNumbers.clear();

		int iStart = 0, iEnd = -1, iLength = str.length();
		while (iStart < iLength)
		{
			iEnd = str.indexOf(',', iStart);
			if (iEnd < 0)
			{
				iEnd = iLength;
			}

			Add(new Region(str.substring(iStart, iEnd)));

			iStart = iEnd + 1;
		}
	}

	public final void Add(int iNum)
	{
		Add(new Region(iNum));
	}

	public final void Add(Set nums)
	{
		for (Region rg : nums.getRagions())
		{
			Add(rg);
		}
	}

	public final void Add(Region ragion)
	{
		int InsertAt = 0;
		Region temp = ragion.clone();
		int count = m_ValidNumbers.size();
		for (int i = count - 1; i >= 0; --i)
		{
			int iComp = m_ValidNumbers.get(i).CompareTo(temp);

			// try to merge with this.
			if (temp.Merge(m_ValidNumbers.get(i)))
			{
				// Remove current region.
				m_ValidNumbers.remove(i);
			}
			else if (iComp < 0)
			{
				// insert here.
				InsertAt = i + 1;
				break; // end.
			}
		}

		// Insert ...
		m_ValidNumbers.add(InsertAt, temp);
	}

	public final void Remove(int iNum)
	{
		int count = m_ValidNumbers.size();
		for (int i = 0; i < count; ++i)
		{
			int comp = m_ValidNumbers.get(i).CompareTo(iNum);
			if (comp > 0)
			{
				return; // not included.
			}

			if (comp == 0)
			{
				// Split this ragion by the number.
				int tempMin = m_ValidNumbers.get(i).getMin();
				int tempMax = m_ValidNumbers.get(i).getMax();

				// Remove this ragion.
				m_ValidNumbers.remove(i);

				// Part 1.
				if (iNum > tempMin)
				{
					m_ValidNumbers.add(i++, new Region(tempMin, iNum - 1));
				}

				// Part 2.
				if (iNum < tempMax)
				{
					m_ValidNumbers.add(i++, new Region(iNum + 1, tempMax));
				}

				break; // end.
			}
		}
	}

	public final void Clear()
	{
		m_ValidNumbers.clear();
	}

	public final boolean Contains(int iNum)
	{
		for (Region condi : m_ValidNumbers)
		{
			int iComp = condi.CompareTo(iNum);
			if (iComp == 0) // just it...
			{
				return true;
			}

			if (iComp > 0) // not in...
			{
				return false;
			}
		}

		return false;
	}

	public final boolean Contains(Region test)
	{
		for (Region condi : m_ValidNumbers)
		{
			int iComp = condi.CompareTo(test);
			if (iComp == 0 && condi.getMin() <= test.getMin() && condi.getMax() >= test.getMax())
			{
				return true;
			}

			if (iComp > 0) // not in...
			{
				return false;
			}
		}

		return false;
	}

	public final Set Intersection(Set compare)
	{
		Set result = new Set();

		for (int num : getNumbers())
		{
			if (compare.Contains(num))
			{
				result.Add(num);
			}
		}

		return result;
	}

	public final int getCount()
	{
		int count = 0;
		for (Region condi : m_ValidNumbers)
		{
			count += condi.getStep() + 1;
		}

		return count;
	}

	public final java.util.ArrayList<Region> getRagions()
	{
		return m_ValidNumbers;
	}

	public final int[] getNumbers()
	{
		int index = 0;
		int[] nums = new int[getCount()];
		for (Region rg : m_ValidNumbers)
		{
			for (int num : rg.getNumbers())
			{
				nums[index++] = num;
			}
		}

		return nums;
	}

	public final String getDisplayExpression()
	{
		int[] nums = getNumbers();
		String str = "";
		int count = nums.length;
		for (int i = 0; i < count; ++ i)
		{
			str += StringFormater.padLeft(Integer.toString(nums[i]), 2, '0');
			if (i != count - 1)
			{
				str += ",";
			}
		}

		return str;
	}
}