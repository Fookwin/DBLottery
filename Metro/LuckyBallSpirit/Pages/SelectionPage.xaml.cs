using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using LuckyBallsData.Filters;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;
using LuckyBallsData.Statistics;
using LuckyBallSpirit.DataModel;
using LuckyBallSpirit.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LuckyBallSpirit.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SelectionPage : LuckyBallSpirit.Common.LayoutAwarePage
    {
        public SelectionPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            this.Loaded += delegate
            {
                // Initilize the foot panel.
                pageFootPanel.LaunchTimeDown();

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    if (LBDataManager.GetInstance().PendingPurchase != null)
                    {
                        ResetControls();

                        // show float panel.
                        LotterySelectionFloatPanel floatPanel = LotterySelectionFloatPanel.Instance();
                        floatPanel.Show(LotterySelectionFloatPanel.LSDisplayModeEnum.Extend);
                        floatPanel.EditingTargetCreated += ResetControls;
                        floatPanel.SelectionChanged += RefreshResultList;

                        Panel_NumberSelection.SelectorChanged += floatPanel.Refresh;
                        Panel_SchemeFilters.FilterChanged += floatPanel.Refresh;
                        Panel_SchemeSelection.SelectionChanged += floatPanel.Refresh;
                    }
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

            pageFootPanel.ActiveCommand = PageFootPanel.PageIndexEnum.SelectionPage;
        }

        private void RefreshResultList()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Panel_SchemeSelection.Refresh();
            });
        }

        private void ResetControls()
        {
            // Get the new pending purchase and update controls.
            LBDataManager.GetInstance().PendingPurchase.GetSource().ContinueWith(res =>
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Panel_NumberSelection.SetTarget(res.Result.Selectors);
                    Panel_SchemeFilters.SetTarget(res.Result.Filters);
                    Panel_SchemeSelection.SetTarget(res.Result.Selection);
                });
            });
        }

        private void pageRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            LotterySelectionFloatPanel.Instance().EditingTargetCreated -= ResetControls;
            LotterySelectionFloatPanel.Instance().SelectionChanged -= RefreshResultList;
        }
    }
}
