package com.fookwin.lotterydata.data;

import com.fookwin.lotterydata.util.StringFormater;

public class MatrixItemByte
{
	public static long[] _rBits = new long[] { 1, 2,4,8,16,32,64,128,256,512, 1024,2048,4096,8192,16384,32768, 65536,131072,262144, 524288, 1048576,2097152,4194304,8388608, 16777216,33554432,67108864,134217728,268435456, 536870912,1073741824, 2147483648L,4294967296L };

	private long _set = 0;
	private int _bitSize = 0;

	public MatrixItemByte(int size)
	{
		_bitSize = size;
	}

	public MatrixItemByte(int size, long copyFrom)
	{
		_bitSize = size;
		_set = copyFrom;
	}

	public MatrixItemByte(String values)
	{
		String[] vals = values.split("[ ,]", -1);
		for (int i = 0; i < vals.length; ++i)
		{
			_set |= _rBits[Integer.parseInt(vals[i].trim()) - 1];
		}

		_bitSize = vals.length;
	}

	public final MatrixItemByte clone()
	{
		MatrixItemByte copy = new MatrixItemByte();
		copy._set = _set;
		copy._bitSize = _bitSize;

		return copy;
	}

	public final long getBits()
	{
		return _set;
	}

	public final int getSize()
	{
		return _bitSize;
	}

	public final void Add(int num)
	{
		_set |= _rBits[num - 1];
	}

	@Override
	public String toString()
	{
		String str = "";

		int index = 1;
		for (long bit : _rBits)
		{
			if ((_set | bit) == _set)
			{
				if (!str.equals(""))
				{
					str += " ";
				}

				str += StringFormater.padLeft(Integer.toString(index), 2, '0');
			}

			++index;
		}

		return str;
	}

	public final Integer[] getIndices()
	{
		java.util.ArrayList<Integer> output = new java.util.ArrayList<Integer>();
		int index = 1;
		for (long bit : _rBits)
		{
			if ((_set | bit) == _set)
			{
				output.add(index);
			}

			++index;
		}
		
		return (Integer[])output.toArray(new Integer[output.size()]);
	}

	public final Integer[] Instance(int[] candidates)
	{
		Integer[] indices = getIndices();
		// replace with the corresponding candidate at the same position.
		for (int i = 0; i < indices.length; ++i)
		{
			indices[i] = candidates[indices[i] - 1];
		}

		return indices;
	}

	public final int Intersection(MatrixItemByte compareTo)
	{
		long comp = _set & compareTo._set;
		int hit = 0;
		while (comp != 0)
		{
			++hit;

			comp = comp & (comp - 1);
		}

		return hit;
	}

	private MatrixItemByte()
	{

	}
}