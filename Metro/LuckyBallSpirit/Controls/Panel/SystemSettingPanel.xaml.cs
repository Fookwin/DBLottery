using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LuckyBallSpirit.DataModel;
using LuckyBallsData.Configration;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class SystemSettingPanel : UserControl
    {
        public SystemSettingPanel()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                // show float panel.
                LotterySelectionFloatPanel.Instance().Show(LotterySelectionFloatPanel.LSDisplayModeEnum.Experss);

                AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;
                if (option != null)
                {
                    TB_HitPropLowLimit.Value = option.HitProbability_LowLimit;
                    TB_OmissionLowLimit.Value = option.ImmediateOmission_LowLimit;
                    TB_ProtentialLowLimit.Value = option.ProtentialEnergy_LowLimit;
                    TB_MaxDeviationLimit.Value = option.MaxDeviation_LowLimit;
                    TB_RecommendThreshold.Value = option.RecommendThreshold;
                }
            };
        }

        private void CleanLocalCacheButton_Click(object sender, RoutedEventArgs e)
        {
            LBDataManager.GetInstance().CleanLocalCache();
            MessageBox.Show("本地数据已清除， 下次启动会将重新加载最新数据。");
        }

        private void TB_ValueChanged(object sender, ValueChangeArgs e)
        {
            EditFilterButton.IsEnabled = true;
        }

        private void EditFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // make the change.
            AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;
            option.HitProbability_LowLimit = TB_HitPropLowLimit.Value;
            option.ImmediateOmission_LowLimit = (int)TB_OmissionLowLimit.Value;
            option.ProtentialEnergy_LowLimit = TB_ProtentialLowLimit.Value;
            option.MaxDeviation_LowLimit = TB_MaxDeviationLimit.Value;
            option.RecommendThreshold = TB_RecommendThreshold.Value;

            // save to local.
            LBDataManager.GetInstance().SaveAttributeFilterOption();
            EditFilterButton.IsEnabled = false;
        }

        private void SetDefaultFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // make default and save to local.
            AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;
            option.AsDefault();
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                TB_HitPropLowLimit.Value = option.HitProbability_LowLimit;
                TB_OmissionLowLimit.Value = option.ImmediateOmission_LowLimit;
                TB_ProtentialLowLimit.Value = option.ProtentialEnergy_LowLimit;
                TB_MaxDeviationLimit.Value = option.MaxDeviation_LowLimit;
                TB_RecommendThreshold.Value = option.RecommendThreshold;
            });

            LBDataManager.GetInstance().SaveAttributeFilterOption();
            EditFilterButton.IsEnabled = false;
        }
    }
}
