using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LuckyBallsSpirit.DataModel;

namespace LuckyBallsSpirit.Controls
{
    public partial class LotteryStatusPanel : UserControl
    {
        public LotteryStatusPanel()
        {
            InitializeComponent();
        }

        internal void SetContext(LotteryStateInfo info)
        {
            RedHotNumPanel.Title = "热";
            RedHotNumPanel.Discription = "近期频出号码";
            RedHotNumPanel.TitleColor = ColorPicker.OrangeRed;
            RedHotNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.RedsStateInfo, true, NumStatusType.HotNumber);

            RedCoolNumPanel.Title = "冷";
            RedCoolNumPanel.Discription = "长期未出号码";
            RedCoolNumPanel.TitleColor = ColorPicker.SkyBlue;
            RedCoolNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.RedsStateInfo, true, NumStatusType.CoolNumber);

            RedIncludeNumPanel.Title = "胆";
            RedIncludeNumPanel.Discription = "下期推荐胆号";
            RedIncludeNumPanel.TitleColor = ColorPicker.LimeGreen;
            RedIncludeNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.RedsStateInfo, true, NumStatusType.RecommendedNumber);

            RedExcludeNumPanel.Title = "杀";
            RedExcludeNumPanel.Discription = "下期推荐杀号";
            RedExcludeNumPanel.TitleColor = ColorPicker.Gray;
            RedExcludeNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.RedsStateInfo, true, NumStatusType.KilledNumber);

            BlueHotNumPanel.Title = "热";
            BlueHotNumPanel.Discription = "近期频出号码";
            BlueHotNumPanel.TitleColor = ColorPicker.OrangeRed;
            BlueHotNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.BluesStateInfo, false, NumStatusType.HotNumber);

            BlueCoolNumPanel.Title = "冷";
            BlueCoolNumPanel.Discription = "长期未出号码";
            BlueCoolNumPanel.TitleColor = ColorPicker.SkyBlue;
            BlueCoolNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.BluesStateInfo, false, NumStatusType.CoolNumber);

            BlueIncludeNumPanel.Title = "胆";
            BlueIncludeNumPanel.Discription = "下期推荐胆号";
            BlueIncludeNumPanel.TitleColor = ColorPicker.LimeGreen;
            BlueIncludeNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.BluesStateInfo, false, NumStatusType.RecommendedNumber);

            BlueExcludeNumPanel.Title = "杀";
            BlueExcludeNumPanel.Discription = "下期推荐杀号";
            BlueExcludeNumPanel.TitleColor = ColorPicker.Gray;
            BlueExcludeNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.BluesStateInfo, false, NumStatusType.KilledNumber);
        }
    }
}
