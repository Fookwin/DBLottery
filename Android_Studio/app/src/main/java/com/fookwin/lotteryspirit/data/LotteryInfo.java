package com.fookwin.lotteryspirit.data;

import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

import com.fookwin.lotterydata.data.Lottery;

public class LotteryInfo
{	
    public String Index;
    public String Issue;
    public String Date;
    public String Red1;
    public String Red2;
    public String Red3;
    public String Red4;
    public String Red5;
    public String Red6;
    public String Blue;
    public String Reds;
    public String BetAmount;
    public String PoolAmount;
    public String More;
    public List<BonusInfo> Winners;

    public static LotteryInfo Create(Lottery lot) throws ParseException
    {
        LotteryInfo info = new LotteryInfo();

        info.Issue = Integer.toString(lot.getIssue());
        info.Date = lot.getDateExp();
        info.Red1 = lot.getScheme().getRedExp(0);
        info.Red2 = lot.getScheme().getRedExp(1);
        info.Red3 = lot.getScheme().getRedExp(2);
        info.Red4 = lot.getScheme().getRedExp(3);
        info.Red5 = lot.getScheme().getRedExp(4);
        info.Red6 = lot.getScheme().getRedExp(5);
        info.Blue = lot.getScheme().getBlueExp();
        info.Reds = lot.getScheme().getRedsExp();

        info.BetAmount = FormatMoney(lot.getBetAmount());
        info.PoolAmount = FormatMoney(lot.getPoolAmount());
        info.More = lot.getMoreInfo();

        // winners...
        info.Winners = new ArrayList<BonusInfo>();

        for (int i = 1; i <= 6; ++i)
        {
            BonusInfo item = new BonusInfo();
            item.Name = Integer.toString(i) + "µÈ½±";
            item.Bonus = FormatMoney(lot.BonusMoney(i));
            item.Count = Integer.toString(lot.BonusAmount(i)) + "×¢";

            info.Winners.add(item);
        }            

        return info;
    }

    private static String FormatMoney(int money)
    {
    	String output = "";
        int yi = money / 100000000;
        if (yi > 0)
            output += Integer.toString(yi) + "ÒÚ";

        int wan = (money % 100000000) / 10000;
        if (wan > 0)
            output += Integer.toString(wan) + "Íò";

        output += Integer.toString((money % 10000)) + "Ôª";

        return output;
    }
}

