package com.fookwin.lotterydata.data;

import android.annotation.SuppressLint;
import java.util.HashMap;

public enum SchemeSelectorTypeEnum
{
	StandardSelectorType(0),
	DantuoSelectorType(1),
	RandomSelectorType(2),
	UploadSelectorType(3);

	private int intValue;
	private static HashMap<Integer, SchemeSelectorTypeEnum> mappings;
	@SuppressLint("UseSparseArrays")
	private synchronized static HashMap<Integer, SchemeSelectorTypeEnum> getMappings()
	{
		if (mappings == null)
		{
			mappings = new HashMap<Integer, SchemeSelectorTypeEnum>();
		}
		return mappings;
	}

	private SchemeSelectorTypeEnum(int value)
	{
		intValue = value;
		SchemeSelectorTypeEnum.getMappings().put(value, this);
	}

	public int getValue()
	{
		return intValue;
	}

	public static SchemeSelectorTypeEnum forValue(int value)
	{
		return getMappings().get(value);
	}
}