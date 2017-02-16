package com.fookwin.lotteryspirit.data;

import java.text.ParseException;

import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotterydata.data.NumberState;
import com.fookwin.lotterydata.data.ReleaseInfo;

public class LotteryStateInfo
{
	public NumberStateInfo[] RedsStateInfo = new NumberStateInfo[33];
	public NumberStateInfo[] BluesStateInfo = new NumberStateInfo[16];

	public static LotteryStateInfo Create(Lottery lot, ReleaseInfo release) throws ParseException
	{
		LotteryStateInfo info = new LotteryStateInfo();

		NumberState[] redNumStates = lot.getStatus().getRedNumStates();
		
		for (int i = 0; i < 33; ++i)
		{
			NumberStateInfo tempVar = new NumberStateInfo();
			tempVar.setNumber(i + 1);
			tempVar.setState(redNumStates[i]);
			tempVar.setIncluded(release.getIncludedReds().Contains(i + 1));
			tempVar.setExcluded(release.getExcludedReds().Contains(i + 1));
			info.RedsStateInfo[i] = tempVar;
		}

		for (int i = 0; i < 16; ++i)
		{
			NumberStateInfo tempVar2 = new NumberStateInfo();
			tempVar2.setNumber(i + 1);
			tempVar2.setState(lot.getStatus().getBlueNumStates()[i]);
			tempVar2.setIncluded(release.getIncludedBlues().Contains(i + 1));
			tempVar2.setExcluded(release.getExcludedBlues().Contains(i + 1));
			info.BluesStateInfo[i] = tempVar2;
		}

		return info;
	}
}