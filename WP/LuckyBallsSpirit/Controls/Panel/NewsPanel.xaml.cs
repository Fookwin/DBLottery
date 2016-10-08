using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CN.SmartMad.Ads.WindowsPhone.WPF;
using LuckyBallsSpirit.DataModel;
using LuckyBallsData.Util;

namespace LuckyBallsSpirit.Controls.Panel
{
    public partial class NewsPanel : UserControl
    {
        private bool bLatestLotteryInfoInitialized = false;
        private bool bHistoryInfoInitialized = false;
        private bool bLotteryAttributesInitalized = false;
        private bool bPurchaseInfoInitalized = false;
        private bool bTimeCountDownCtrlInitalized = false;

        private SMAdBannerView _smartAdBanner = null;

        public NewsPanel()
        {
            InitializeComponent();


            this.Loaded += delegate
            {
                // initialize the ad panel
                LoadAd();
            };
        }

        private void LoadAd()
        {
            if (_smartAdBanner == null)
            {
                _smartAdBanner = new SMAdBannerView("90045926");
                _smartAdBanner.Visibility = Visibility.Collapsed;
                _smartAdBanner.AdBannerAnimationType = SMAdBannerAnimationType.BANNER_ANIMATION_TYPE_RANDOM;

                _smartAdBanner.AdBannerReceived += new EventHandler(AdView1_AdBannerReceived);
            }

            if (!AdCtrl.Children.Contains(_smartAdBanner))
                AdCtrl.Children.Add(_smartAdBanner);
        }

        private void AdView1_AdBannerReceived(object sender, EventArgs e)
        {
            _smartAdBanner.Visibility = Visibility.Visible;
            AdPanel.Visibility = Visibility.Visible;
        }

        public bool RefreshControls(bool firstTime)
        {
            LBDataManager dataMgr = LBDataManager.GetInstance();

            // Initialize latest lottery information.
            if (!bLatestLotteryInfoInitialized &&
                dataMgr.GetLatestLottery() != null &&
                dataMgr.GetLatestLotteryStateInfo() != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    LotteryDetailPanel.Target = dataMgr.GetLatestLottery();
                    CT_LatestStatus.SetContext(dataMgr.GetLatestLotteryStateInfo());

                    DataUtil.SyncContext context = dataMgr.GetSyncContext();
                    if (firstTime && context.LocalVersion.LatestIssue != context.CloudVersion.LatestIssue)
                    {
                        string message = "已更新到第" + dataMgr.ReleaseInfo.CurrentIssue.ToString() + "期";

                        // show the message only when the data is just updated.
                        MessageCenter.AddMessage(message, MessageType.Information, MessagePriority.Immediate, 2);
                    }

                    LotteryDetailPanel.Visibility = Visibility.Visible;
                    CT_LatestStatus.Visibility = Visibility.Visible;
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
                this.Dispatcher.BeginInvoke(() =>
                {
                    CT_RecommenedAttributes.SetContext(dataMgr.GetRecommendedConditions());

                    CT_RecommenedAttributes.Visibility = Visibility.Visible;
                });

                bLotteryAttributesInitalized = true;
            }

            // Update purchase bucket.
            if (!bPurchaseInfoInitalized && dataMgr.PurchaseManager != null && dataMgr.PendingPurchase != null)
            {
                bPurchaseInfoInitalized = true;
            }

            // Start time count down controls.
            if (!bTimeCountDownCtrlInitalized && dataMgr.ReleaseInfo != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    TimeCountDownPanel.Start();
                    TimeCountDownPanel.Visibility = Visibility.Visible;
                });

                bTimeCountDownCtrlInitalized = true;
            }

            if (bLatestLotteryInfoInitialized &&
                bHistoryInfoInitialized &&
                bLotteryAttributesInitalized &&
                bPurchaseInfoInitalized &&
                bTimeCountDownCtrlInitalized)
            {
                return true;
            }

            return false;
        }
    }
}
