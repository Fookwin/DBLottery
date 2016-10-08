using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LuckyBallsData.Statistics;
using LuckyBallsSpirit.DataModel;

namespace LuckyBallsSpirit.Pages
{
    public partial class LotteryDetailPage : PhoneApplicationPage
    {
        private List<Lottery> _history = null;
        private int currentIndex = -1;
        private int maxIndex = -1;

        public LotteryDetailPage()
        {
            InitializeComponent();

            _history = LBDataManager.Instance().History.Lotteries;
            maxIndex = _history.Count;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string index = "";
            if (NavigationContext.QueryString.TryGetValue("index", out index))
            {
                currentIndex = Convert.ToInt32(index);

                ShowDetail(currentIndex);
            }
        }

        private void ShowDetail(int issueIndex)
        {
            // get the lottery.
            Lottery lot = _history[issueIndex];
            LotteryDetailPanel.Target = lot;

            // update the title.
            pageHeaderPanel.Title = "第" + lot.Issue.ToString() + "期";
        }
    }
}