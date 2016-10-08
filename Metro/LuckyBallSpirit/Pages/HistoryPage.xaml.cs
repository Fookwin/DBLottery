using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using LuckyBallsData.Statistics;
using LuckyBallSpirit.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LuckyBallSpirit.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class HistoryPage : LuckyBallSpirit.Common.LayoutAwarePage
    {
        public HistoryPage()
        {
            this.InitializeComponent();
            this.Loaded += delegate
            {
                // Initilize the foot panel.
                pageFootPanel.LaunchTimeDown();

                // show float panel.
                LotterySelectionFloatPanel.Instance().Show(LotterySelectionFloatPanel.LSDisplayModeEnum.Experss);

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    List<Lottery> reversedList = LBDataManager.GetInstance().History.Lotteries.ToList();
                    reversedList.Reverse();

                    Lottery_List.ItemsSource = reversedList;
                });
            };
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            pageFootPanel.ActiveCommand = PageFootPanel.PageIndexEnum.HistoryPage;
        }

        private void Lottery_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lottery_List.SelectedItem != null)
            {
                Lottery selected = Lottery_List.SelectedItem as Lottery;
                if (selected != null)
                {
                    BR_DetailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    BR_DetailPanel.DataContext = LotteryInfo.Create(selected);
                }
            }
            else
            {
                BR_DetailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                BR_DetailPanel.DataContext = null;
            }
        }

        private void BT_Go_Click(object sender, RoutedEventArgs e)
        {
            int specified = -1;
            try
            {
                specified = Convert.ToInt32(TB_SpecifiedIssue.Text);
            }
            catch
            {
                specified = -1;
            }

            Lottery lot = LBDataManager.GetInstance().History.LotteryInIssue(specified);
            if (lot != null)
            {
                Lottery_List.SelectedItem = lot;
                Lottery_List.ScrollIntoView(lot);
            }
        }

        private void TB_SpecifiedIssue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_SpecifiedIssue.Text == "")
            {
                TB_SpecifiedIssue.Text = "输入期号";
                TB_SpecifiedIssue.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
            }
        }

        private void TB_SpecifiedIssue_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TB_SpecifiedIssue.Text == "输入期号")
            {
                TB_SpecifiedIssue.Text = "";
                TB_SpecifiedIssue.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            }
        }
    }
}
