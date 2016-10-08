using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Statistics;
using LuckyBallsData;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace LuckyBallsSpirit.DataModel
{
    public class NumberStateInfo
    {
        public int Number
        {
            get;
            set;
        }

        public NumberState State
        {
            get;
            set;
        }

        public bool Included
        {
            get;
            set;
        }

        public bool Excluded
        {
            get;
            set;
        }
    }

    public class RedTemperatureToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int temp = (int)value;

            return temp >= 4 ? "热" : (temp < 2 ? "冷" : "温");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BlueTemperatureToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int temp = (int)value;

            return temp >= 2 ? "热" : (temp == 0 ? "冷" : "温");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    class LotteryStateInfo
    {
        public NumberStateInfo[] RedsStateInfo = new NumberStateInfo[33];
        public NumberStateInfo[] BluesStateInfo = new NumberStateInfo[16];

        public static LotteryStateInfo Create(Lottery lot, ReleaseInfo release)
        {
            LotteryStateInfo info = new LotteryStateInfo();

            for (int i = 0; i < 33; ++i)
            {
                info.RedsStateInfo[i] = new NumberStateInfo()
                {
                    Number = i + 1,
                    State = lot.Status.RedNumStates[i],
                    Included = release.IncludedReds.Contains(i + 1),
                    Excluded = release.ExcludedReds.Contains(i + 1)
                };
            }

            for (int i = 0; i < 16; ++i)
            {
                info.BluesStateInfo[i] = new NumberStateInfo()
                {
                    Number = i + 1,
                    State = lot.Status.BlueNumStates[i],
                    Included = release.IncludedBlues.Contains(i + 1),
                    Excluded = release.ExcludedBlues.Contains(i + 1)
                };
            }

            return info;
        }
    }
}
