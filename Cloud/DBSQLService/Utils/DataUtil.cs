using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSQLService
{
    public class DataUtil
    {
        public static DBLotteryModel BuildLotteryModel(Basic basic, Detail detail)
        {
            DBLotteryModel lot = new DBLotteryModel();
            lot.Issue = basic.Issue;
            lot.Scheme = basic.Red1.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red2.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red3.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red4.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red5.ToString().PadLeft(2, '0');
            lot.Scheme += " " + basic.Red6.ToString().PadLeft(2, '0');
            lot.Scheme += "+" + basic.Blue.ToString().PadLeft(2, '0');

            lot.Bet = detail.BetAmount;
            lot.Pool = detail.PoolAmount;
            lot.Details = detail.More;
            lot.Date = detail.Date;

            lot.Bonus = new List<int>
            {
                detail.Prize1Count,
                Convert.ToInt32(detail.Prize1Bonus),
                detail.Prize2Count,
                Convert.ToInt32(detail.Prize2Bonus),
                detail.Prize3Count,
                3000,
                detail.Prize4Count,
                200,
                detail.Prize5Count,
                10,
                detail.Prize6Count,
                5,
            };

            return lot;
        }
    }
}