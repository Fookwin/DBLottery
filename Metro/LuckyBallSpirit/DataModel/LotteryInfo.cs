using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Statistics;

namespace LuckyBallSpirit.DataModel
{
    public class BonusInfo
    {
        public string Name { get; set; }
        public string Count { get; set; }
        public string Bonus { get; set; }
    }

    public class LotteryInfo
    {
        public string Index { get; set; }
        public string Issue { get; set; }
        public string Date { get; set; }
        public string Red1 { get; set; }
        public string Red2 { get; set; }
        public string Red3 { get; set; }
        public string Red4 { get; set; }
        public string Red5 { get; set; }
        public string Red6 { get; set; }
        public string Blue { get; set; }
        public string Reds { get; set; }
        public string BetAmount { get; set; }
        public string PoolAmount { get; set; }
        public string More { get; set; }
        public List<BonusInfo> Winners { get; set; }

        public static LotteryInfo Create(Lottery lot)
        {
            LotteryInfo info = new LotteryInfo();

            info.Issue = lot.Issue.ToString();
            info.Date = lot.Date.ToString("yyy-MM-dd");
            info.Red1 = lot.Scheme.Red(0).ToString().PadLeft(2, '0');
            info.Red2 = lot.Scheme.Red(1).ToString().PadLeft(2, '0');
            info.Red3 = lot.Scheme.Red(2).ToString().PadLeft(2, '0');
            info.Red4 = lot.Scheme.Red(3).ToString().PadLeft(2, '0');
            info.Red5 = lot.Scheme.Red(4).ToString().PadLeft(2, '0');
            info.Red6 = lot.Scheme.Red(5).ToString().PadLeft(2, '0');
            info.Blue = lot.Scheme.Blue.ToString().PadLeft(2, '0');
            info.Reds = lot.Scheme.RedsExp;

            info.BetAmount = FormatMoney(lot.BetAmount);
            info.PoolAmount = FormatMoney(lot.PoolAmount);
            info.More = lot.MoreInfo;

            // winners...
            info.Winners = new List<BonusInfo>();

            for (int i = 1; i <= 6; ++i)
            {
                BonusInfo item = new BonusInfo();
                item.Name = i.ToString() + "等奖";
                item.Bonus = FormatMoney(lot.BonusMoney(i));
                item.Count = lot.BonusAmount(i).ToString() + "注";

                info.Winners.Add(item);
            }            

            return info;
        }

        private static string FormatMoney(int money)
        {
            string output = "";
            int yi = money / 100000000;
            if (yi > 0)
                output += yi.ToString() + "亿";

            int wan = (money % 100000000) / 10000;
            if (wan > 0)
                output += wan.ToString() + "万";

            output += (money % 10000).ToString() + "元";

            return output;
        }
    }
}
