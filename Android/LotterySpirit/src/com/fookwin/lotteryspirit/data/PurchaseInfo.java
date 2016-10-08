package com.fookwin.lotteryspirit.data;

import java.text.ParseException;

import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotterydata.util.DataUtil;

public class PurchaseInfo
{
	public enum PurchaseStatusEnum
	{
		unverified,
		verifying,
		verified				
	}
	
	public int ID = 0;
	public int Issue = 0;
	public String CreateAt = null;
	public String ReleaseAt = null;
	public int Buy = 0;
	public int Win = -2;	
	
	private Purchase _source = null;
	
	public PurchaseStatusEnum getStatus()
	{
		if (Win == -2)
			return PurchaseStatusEnum.unverified;
		else if (Win == -1)
			return PurchaseStatusEnum.verifying;
		else
			return PurchaseStatusEnum.verified;
	}
	
	public boolean verify() throws ParseException
	{
		if (Win >= 0)
			return true;
		
        Lottery lot = LBDataManager.GetInstance().getHistory().getByIssue(Issue);
        if (lot == null) 
        {
            return false;
        }
        
        // calculate it only when the bonus information is ready.
        boolean bBonusReleased = lot.BonusAmount(6) > 0;
        
        Win = 0;
        Purchase target = getSource();
        
        boolean bGetWin = false, bVerified = true;
        for (Scheme item : target.getSelection())
        {
            int prize = lot.CheckPrize(item);
            if (prize > 0)
            {
            	bGetWin = true;
            	
            	if (prize < 3 && !bBonusReleased)
            	{
            		bVerified = false;
            		break; // we don't have bonus info for top 2 prizes, waiting for next.
            	}
            	
            	Win += lot.BonusMoney(prize);
            }
        }
        
        if (bGetWin && !bVerified)
        	Win = -1; // calculate the win bonus next time.
        
		return true;
	}
	
	public Purchase getSource()
	{
		if (_source == null)
		{
			// read from disk.
			_source = DataUtil.ReadPurchase(ID);
		}
		return _source;
	}
	
	public void setSource(Purchase src)
	{
		_source = src;
	}
}