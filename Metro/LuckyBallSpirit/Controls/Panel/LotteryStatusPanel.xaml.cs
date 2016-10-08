using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LuckyBallsData.Statistics;
using LuckyBallSpirit.DataModel;
using LuckyBallSpirit.ViewModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class LotteryStatusPanel : UserControl
    {
        public LotteryStatusPanel()
        {
            this.InitializeComponent();
        }

        internal void SetContext(LotteryStateInfo info)
        {
            RedHotNumPanel.Title = "热";
            RedHotNumPanel.Discription = "近期频出号码";
            RedHotNumPanel.TitleColor = Colors.OrangeRed;            
            RedHotNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.RedsStateInfo, true, NumStatusType.HotNumber);

            RedCoolNumPanel.Title = "冷";
            RedCoolNumPanel.Discription = "长期未出号码";
            RedCoolNumPanel.TitleColor = Colors.SkyBlue;
            RedCoolNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.RedsStateInfo, true, NumStatusType.CoolNumber);

            RedIncludeNumPanel.Title = "胆";
            RedIncludeNumPanel.Discription = "下期推荐胆号";
            RedIncludeNumPanel.TitleColor = Colors.LimeGreen;
            RedIncludeNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.RedsStateInfo, true, NumStatusType.RecommendedNumber);

            RedExcludeNumPanel.Title = "杀";
            RedExcludeNumPanel.Discription = "下期推荐杀号";
            RedExcludeNumPanel.TitleColor = Colors.Gray;
            RedExcludeNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.RedsStateInfo, true, NumStatusType.KilledNumber);

            BlueHotNumPanel.Title = "热";
            BlueHotNumPanel.Discription = "近期频出号码";
            BlueHotNumPanel.TitleColor = Colors.OrangeRed;
            BlueHotNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.BluesStateInfo, false, NumStatusType.HotNumber);

            BlueCoolNumPanel.Title = "冷";
            BlueCoolNumPanel.Discription = "长期未出号码";
            BlueCoolNumPanel.TitleColor = Colors.SkyBlue;
            BlueCoolNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.BluesStateInfo, false, NumStatusType.CoolNumber);

            BlueIncludeNumPanel.Title = "胆";
            BlueIncludeNumPanel.Discription = "下期推荐胆号";
            BlueIncludeNumPanel.TitleColor = Colors.LimeGreen;
            BlueIncludeNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.BluesStateInfo, false, NumStatusType.RecommendedNumber);

            BlueExcludeNumPanel.Title = "杀";
            BlueExcludeNumPanel.Discription = "下期推荐杀号";
            BlueExcludeNumPanel.TitleColor = Colors.Gray;
            BlueExcludeNumPanel.Numbers = ConvertStatusToBallGroupHelper.GetNumbers(info.BluesStateInfo, false, NumStatusType.KilledNumber);
        }
    }
}
