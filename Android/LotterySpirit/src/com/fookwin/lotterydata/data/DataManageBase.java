package com.fookwin.lotterydata.data;

import com.fookwin.LotterySpirit;

import android.content.Context;

public abstract class DataManageBase
{
	protected static DataManageBase _Instance = null;
	protected History _History = null;
	protected MatrixTable _matrixTable = null;

	public static DataManageBase Instance()
	{
		if (_Instance == null)
		{
			throw new RuntimeException("Data Manager has not been instantiated yet.");
		}

		return _Instance;
	}

	public final History getHistory()
	{
		if (_History == null)
		{
			return null;
		}

		return _History;
	}

	public final MatrixTable getMatrixTable()
	{
		return _matrixTable;
	}
	
	public final Context getAppContext()
	{
		return LotterySpirit.getInstance();
	}
}