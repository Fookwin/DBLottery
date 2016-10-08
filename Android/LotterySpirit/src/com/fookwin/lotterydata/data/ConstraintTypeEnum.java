package com.fookwin.lotterydata.data;

import android.annotation.SuppressLint;
import java.util.HashMap;

public enum ConstraintTypeEnum
{
	SchemeAttributeConstraintType(0),
	RedNumSetConstraintType(1),
	HistoryDuplicateConstraintType(2);

	private int intValue;
	private static HashMap<Integer, ConstraintTypeEnum> mappings;
	@SuppressLint("UseSparseArrays")
	private synchronized static HashMap<Integer, ConstraintTypeEnum> getMappings()
	{
		if (mappings == null)
		{
			mappings = new HashMap<Integer, ConstraintTypeEnum>();
		}
		return mappings;
	}

	private ConstraintTypeEnum(int value)
	{
		intValue = value;
		ConstraintTypeEnum.getMappings().put(value, this);
	}

	public int getValue()
	{
		return intValue;
	}

	public static ConstraintTypeEnum forValue(int value)
	{
		return getMappings().get(value);
	}
}