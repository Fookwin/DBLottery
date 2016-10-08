using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using LuckyBallSpirit.Pages;
using LuckyBallsData.Util;
using LuckyBallsData.Selection;
using LuckyBallSpirit.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LuckyBallSpirit
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : LuckyBallSpirit.Common.LayoutAwarePage
    {
        private static bool? _firstTime = null;

        private bool bLatestLotteryInfoInitialized = false;
        private bool bHistoryInfoInitialized = false;
        private bool bLotteryAttributesInitalized = false;
        private bool bPurchaseInfoInitalized = false;
        private bool bTimeCountDownCtrlInitalized = false;        

        public MainPage()
        {
            if (_firstTime == null)
                _firstTime = true;
            else
                _firstTime = false;

            this.InitializeComponent();
            this.Loaded += delegate 
            {
                Initialize();                
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

        private void Initialize()
        {
            LBDataManager mgr = LBDataManager.GetInstance();
            if (mgr.Initialized)
            {
                RefreshControls();
            }
            else
            {
                RefreshControls(); // call it once to make valid controls updated.

                WaitingProgress.Visibility = Visibility.Visible; // start waiting progress.

                mgr.OnDataInitalized += DataInitializedHandler;

                mgr.Initialize();                
            }
        }

        private void DataInitializedHandler(string message, int progress)
        {
            if (progress >= 0)
            {
                RefreshControls();
            }

            // Update progress...
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                ProgressMessage.Text = message;
                ProgressBar.Value = progress < 0 ? 0 : progress;

                if (progress < 0)
                    RetryButton.Visibility = Visibility.Visible;
            });
        }

        private void RefreshControls()
        {
            LBDataManager dataMgr = LBDataManager.GetInstance();

            // Initialize latest lottery information.
            if (!bLatestLotteryInfoInitialized &&
                dataMgr.GetLatestLotteryInfo() != null &&
                dataMgr.GetLatestLotteryStateInfo() != null)
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    LotteryDetailPanel.DataContext = dataMgr.GetLatestLotteryInfo();
                    LotteryStatusPanel.SetContext(dataMgr.GetLatestLotteryStateInfo());

                    string message = "已更新到第" + dataMgr.ReleaseInfo.CurrentIssue.ToString() + "期";
                    pageHeaderPanel.SubTitle = message;

                    DataUtil.SyncContext context = dataMgr.GetSyncContext();
                    if (_firstTime != null && _firstTime.Value && context.LocalVersion.LatestIssue != context.CloudVersion.LatestIssue)
                    {
                        // show the message only when the data is just updated.
                        MessageCenter.AddMessage(message, MessageType.Information, MessagePriority.Immediate, 2);
                    }

                    LotteryDetailPanel.Visibility = Visibility.Visible;
                    LotteryStatusPanel.Visibility = Visibility.Visible;                   
                });

                bLatestLotteryInfoInitialized = true;
            }

            // Update history information.
            if (!bHistoryInfoInitialized && dataMgr.History != null)
            {
                bHistoryInfoInitialized = true;
            }

            // Update lottery attributes.
            if (!bLotteryAttributesInitalized && dataMgr.LastAttributes != null)
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    CT_RecommenedAttributes.SetContext(dataMgr.GetRecommendedConditions());

                    // Show the filter option.
                    CT_RecommenedAttributes.Visibility = Visibility.Visible;
                });
                
                bLotteryAttributesInitalized = true;
            }

            // Update purchase bucket.
            if (!bPurchaseInfoInitalized && dataMgr.PurchaseManager != null && dataMgr.PendingPurchase != null)
            {
                PurchaseBucketInfo lastBucket = dataMgr.PurchaseManager.PurchaseBucket(dataMgr.ReleaseInfo.CurrentIssue);
                if (lastBucket == null)
                {
                    // Create a temporary bucket.
                    lastBucket = new PurchaseBucketInfo(dataMgr.ReleaseInfo.CurrentIssue);
                }

                // initialize it before using.
                lastBucket.Initialize().ContinueWith(tk =>
                {
                    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                    {
                        if (_firstTime == true && lastBucket.JustVerified && lastBucket.Earning > 0)
                        {
                            MessageCenter.AddMessage("恭喜！您有彩票中奖啦！", MessageType.Information, MessagePriority.Immediate, 2);
                        }

                        CT_Purchase.Target = lastBucket;
                        CT_Purchase.PurchaseSelected += OnViewPurchasItem;
                    });
                });
                
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    // show float panel.
                    LotterySelectionFloatPanel.Instance().Show(LotterySelectionFloatPanel.LSDisplayModeEnum.Experss);

                    CT_Purchase.Visibility = Visibility.Visible;
                });

                bPurchaseInfoInitalized = true;
            }

            // Start time count down controls.
            if (!bTimeCountDownCtrlInitalized && dataMgr.ReleaseInfo != null)
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    pageFootPanel.LaunchTimeDown();
                });

                bTimeCountDownCtrlInitalized = true;
            }

            if (bLatestLotteryInfoInitialized &&
                bHistoryInfoInitialized &&
                bLotteryAttributesInitalized &&
                bPurchaseInfoInitalized &&
                bTimeCountDownCtrlInitalized)
            {
                // Once everything completed, enable the page navigation.
                WaitingProgress.Visibility = Visibility.Collapsed;
                pageFootPanel.Visibility = Visibility.Visible;
            }
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            RetryButton.Visibility = Visibility.Collapsed;

            LBDataManager mgr = LBDataManager.GetInstance();
            mgr.Initialize();
        }


        private void OnViewPurchasItem(PurchaseInfo selected)
        {
            var param = new PurchaseHistoryPage.PurchaseItem() 
            { 
                Bucket = CT_Purchase.Target,
                Purchase = selected
            };

            Frame.Navigate(typeof(PurchaseHistoryPage), param);
        }
    }
}
