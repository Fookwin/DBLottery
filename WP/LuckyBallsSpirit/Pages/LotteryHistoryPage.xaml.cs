using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LuckyBallsSpirit.DataModel;
using LuckyBallsData.Statistics;

namespace LuckyBallsSpirit.Pages
{
    public partial class LotteryHistoryPage : PhoneApplicationPage
    {
        private int lotteryCount = -1;
        public LotteryHistoryPage()
        {
            InitializeComponent();
            
            this.Loaded += delegate
            {
                var list = LBDataManager.Instance().History.Lotteries.ToList();
                list.Reverse();

                lotteryCount = list.Count;

                LotteryListCtrl.ItemsSource = list;
            };
        }

        private void LotteryListCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LotteryListCtrl.SelectedIndex >= 0)
            {
                int lotIndex = lotteryCount - LotteryListCtrl.SelectedIndex - 1;

                // switch to detail page.
                Uri target = new Uri("/Pages/LotteryDetailPage.xaml?index=" + lotIndex.ToString(), UriKind.Relative);
                NavigationService.Navigate(target);
            }
        }
    }
}