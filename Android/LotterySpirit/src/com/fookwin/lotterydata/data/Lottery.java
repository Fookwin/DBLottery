package com.fookwin.lotterydata.data;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

import org.json.JSONException;

import android.annotation.SuppressLint;
import android.database.Cursor;

import com.fookwin.lotterydata.util.DBXmlDocument;
import com.fookwin.lotterydata.util.DBXmlNode;
import com.fookwin.lotterydata.util.DataUtil;

public class Lottery
{
	@SuppressLint("SimpleDateFormat")
	private static SimpleDateFormat s_dataFormat = new SimpleDateFormat("yyyy-MM-dd"); 

	private boolean _isDetailsReady = false;
	
	protected int _issue = -1;
	protected Scheme _scheme = null;
	
	protected Date _releaseDate = null;
	protected int[][] _bonusResult = null;
	protected int _betAmount = -1;
	protected int _poolAmount = -1;
	protected String _moreInfo = null;
	protected Status _status = null;

	public Lottery()
	{
	}
	
	public final static Lottery create(Cursor cr) throws ParseException, JSONException
	{
    	Lottery item = new Lottery();				
		item._issue = Integer.parseInt(cr.getString(cr.getColumnIndex("Issue")));
		item._releaseDate = s_dataFormat.parse(cr.getString(cr.getColumnIndex("ReleaseAt")));
		item._scheme = Scheme.parseFromString(cr.getString(cr.getColumnIndex("Scheme")));
		
		return item;
	}
	
	public final Date getDate()
	{
		return _releaseDate;
	}

	public final String getDateExp()
	{
		return s_dataFormat.format(getDate());
	}

	public final int getBetAmount() throws ParseException
	{
		if (!initDetails())
			return -1;

		return _betAmount;
	}

	public final int getPoolAmount() throws ParseException
	{
		if (!initDetails())
			return -1;

		return _poolAmount;
	}

	public final String getMoreInfo() throws ParseException
	{
		if (!initDetails())
			return "";

		return _moreInfo;
	}

	public final int BonusAmount(int prizeLevel) throws ParseException
	{
		if (prizeLevel < 1 || prizeLevel > 6)
		{
			return -1;
		}

		if (!initDetails())
			return -1;

		return _bonusResult[prizeLevel - 1][0];
	}

	public final int BonusMoney(int prizeLevel) throws ParseException
	{
		if (prizeLevel < 1 || prizeLevel > 6)
		{
			return -1;
		}

		if (!initDetails())
			return -1;

		return _bonusResult[prizeLevel - 1][1];
	}

	public final Scheme getScheme()
	{
		return _scheme;
	}

	public final Status getStatus() throws ParseException
	{
		if (!initDetails())
			return null;
		
		return _status;
	}

	public final int getIssue()
	{
		return _issue;
	}

    public final int CheckPrize(Scheme test)
    {
        int redHit = test.Similarity(getScheme());
        boolean blueHit = test.getBlue() == getScheme().getBlue();

        switch (redHit)
        {
            case 0:
            case 1:
            case 2:
                return blueHit ? 6 : -1;
            case 3:
                return blueHit ? 5 : -1;
            case 4:
                return blueHit ? 4 : 5;
            case 5:
                return blueHit ? 3 : 4;
            case 6:
                return blueHit ? 1 : 2;
            default:
                return -1;
        }
    } 
    
	public final boolean ReadFromXml(DBXmlNode node, boolean bParseBaseDataOnly) throws ParseException
	{
		// clear current data.
		_issue = -1;
		_releaseDate = null;
		_bonusResult = null;
		_betAmount = -1;
		_poolAmount = -1;
		_moreInfo = null;
		_scheme = null;
		_status = null;
		
		_issue = Integer.parseInt(node.GetAttribute("Issue"));
		_scheme = new Scheme(node.GetAttribute("Scheme"));
		_releaseDate = s_dataFormat.parse(node.GetAttribute("Date"));
		
		if (!bParseBaseDataOnly)
		{
			_moreInfo = node.GetAttribute("Comments");			
			_poolAmount = Integer.parseInt(node.GetAttribute("Pool"));
			_betAmount = Integer.parseInt(node.GetAttribute("Bet"));
			
			_status = new Status();
			DBXmlNode statusNode = node.FirstChildNode("Status");
			_status.ReadFromXml(statusNode);
			
			_bonusResult = new int[][] { { 0, 0 }, { 0, 0 }, { 0, 3000 }, { 0, 200 }, { 0, 10 }, { 0, 5 } };

			DBXmlNode bonusNode = node.FirstChildNode("Bonus");
			String[] bonusCounts = bonusNode.GetAttribute("PrizeCounts").split("[ ]", -1);
			if (bonusCounts.length == 6)
			{
				_bonusResult[0][0] = Integer.parseInt(bonusCounts[0]);
				_bonusResult[1][0] = Integer.parseInt(bonusCounts[1]);
				_bonusResult[2][0] = Integer.parseInt(bonusCounts[2]);
				_bonusResult[3][0] = Integer.parseInt(bonusCounts[3]);
				_bonusResult[4][0] = Integer.parseInt(bonusCounts[4]);
				_bonusResult[5][0] = Integer.parseInt(bonusCounts[5]);
			}
			_bonusResult[0][1] = Integer.parseInt(bonusNode.GetAttribute("Prize1Bonus"));
			_bonusResult[1][1] = Integer.parseInt(bonusNode.GetAttribute("Prize2Bonus"));
			
			_isDetailsReady = true;
		}

		return true; // delay parsing the data.
	}

	public final String getBonusString() throws ParseException
	{
		if (!initDetails())
			return "";
		
		String str = "";
		str += Integer.toString(_bonusResult[1][0]) + " ";
		str += Integer.toString(_bonusResult[2][0]) + " ";
		str += Integer.toString(_bonusResult[3][0]) + " ";
		str += Integer.toString(_bonusResult[4][0]) + " ";
		str += Integer.toString(_bonusResult[5][0]) + " ";
		str += Integer.toString(_bonusResult[0][1]) + " ";
		str += Integer.toString(_bonusResult[1][1]);
		
		return str;
	}
	
	public final boolean initDetails() throws ParseException
	{
		if (_isDetailsReady)
			return true;
		
		DBXmlDocument xml;
		try {
			xml = DataUtil.getLotteryData(_issue);
		} catch (Exception e) {
			e.printStackTrace();
			return false;
		}
		
		DBXmlNode node = xml.Root();
		
		if (ReadFromXml(node, false))
		{		
			_isDetailsReady = true;
			return true;
		}
		
		return false;
	}
	
 	public final String getBetAmountExp() throws ParseException
	{
		return FormatMoney(getBetAmount());
	}

	public final String getPoolAmountExp() throws ParseException
	{
		return FormatMoney(getPoolAmount());
	}

	public static String FormatMoney(int money)
	{
		String output = "";
		int yi = money / 100000000;
		if (yi > 0)
		{
			output += Integer.toString(yi) + "亿";
		}

		int wan = (money % 100000000) / 10000;
		if (wan > 0)
		{
			output += Integer.toString(wan) + "万";
		}

		output += Integer.toString(money % 10000) + "元";

		return output;
	}
}