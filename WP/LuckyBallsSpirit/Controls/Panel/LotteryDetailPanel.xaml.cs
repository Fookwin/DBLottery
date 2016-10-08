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
using LuckyBallsData.Statistics;

namespace LuckyBallsSpirit.Controls
{
    public partial class LotteryDetailPanel : UserControl
    {
        public LotteryDetailPanel()
        {
            InitializeComponent();
        }

        // Target property
        private static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LotteryDetailPanel ctrl = d as LotteryDetailPanel;
            if (ctrl != null)
            {
                Lottery val = e.NewValue as Lottery;
                ctrl.UpdateTarget(val);
            }
        }

        private void UpdateTarget(Lottery value)
        {
            LotteryInfo info = LotteryInfo.Create(value);

            // Update contorls.
            IssueCtrl.Text = info.Issue;
            DateCtrl.Text = info.Date;

            Red1Ctrl.Num = info.Red1;
            Red2Ctrl.Num = info.Red2;
            Red3Ctrl.Num = info.Red3;
            Red4Ctrl.Num = info.Red4;
            Red5Ctrl.Num = info.Red5;
            Red6Ctrl.Num = info.Red6;
            BlueCtrl.Num = info.Blue;

            BetCtrl.Text = info.BetAmount;
            PoolCtrl.Text = info.PoolAmount;

            WinnersCtrl.ItemsSource = info.Winners;
            MoreCtrl.Text = info.More;
        }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
                        "Target", typeof(Lottery), typeof(LotteryDetailPanel),
                        new PropertyMetadata(null,
                        new PropertyChangedCallback(OnTargetPropertyChanged)));

        public Lottery Target
        {
            get { return (Lottery)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
    }
}
