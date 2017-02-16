package com.fookwin.lotteryspirit.data;

import android.os.Handler;

import com.fookwin.lotterydata.data.DataManageBase;

public class DataManager extends DataManageBase
{
	private static DataManager s_singleton = null;

    public Handler initializingHandler = null;
	
	public static DataManager getDataManager()
	{
		if (s_singleton == null)
		{
			s_singleton = new DataManager();
		}
		
		return s_singleton;
	}
	
	public DataManager()
	{
	
	}

	public void Initialize()
	{
	
	}
}
