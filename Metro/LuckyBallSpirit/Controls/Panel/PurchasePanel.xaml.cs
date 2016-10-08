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
using LuckyBallSpirit.Pages;
using LuckyBallSpirit.DataModel;
using LuckyBallsData.Selection;
using LuckyBallsData.Statistics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class PurchasePanel : UserControl
    {
        public delegate void PurchaseSelectedEventHandler(PurchaseInfo selected);
        public event PurchaseSelectedEventHandler PurchaseSelected;

        public delegate void PurchaseDeletedEventHandler(PurchaseInfo selected);
        public event PurchaseDeletedEventHandler PurchaseDeleted;

        private PurchaseBucketInfo _targetBucket = null;
        private bool _selectionEnabled = false;

        public PurchasePanel()
        {
            this.InitializeComponent();
        }

        public bool SelectionEnabled
        {
            get
            {
                return _selectionEnabled;
            }
            set
            {
                if (_selectionEnabled != value)
                {
                    _selectionEnabled = value;

                    UpdateControlsStatus();
                }
            }
        }

        public PurchaseBucketInfo Target
        {
            get
            {
                return _targetBucket;
            }
            set
            {
                _targetBucket = value;

                Refresh();
            }
        }

        public void Refresh()
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                // Re-init the ui controls.
                LV_RecentPurchases.ItemsSource = null;
                TB_Red1.Text = "?";
                TB_Red2.Text = "?";
                TB_Red3.Text = "?";
                TB_Red4.Text = "?";
                TB_Red5.Text = "?";
                TB_Red6.Text = "?";
                TB_Blue.Text = "?";
                TB_Issue.Text = "?";
                TB_Cost.Text = "0";
                TB_Earning.Text = "0";

                WinBackgroundImage.Visibility = Visibility.Collapsed;
                BT_Donate.Visibility = Visibility.Collapsed;

                if (_targetBucket != null)
                {
                    // Refresh the context.
                    TB_Issue.Text = _targetBucket.Issue.ToString();
                    TB_Cost.Text = _targetBucket.Cost.ToString();
                    TB_Earning.Text = _targetBucket.Earning < 0 ? "统计中" : _targetBucket.Earning.ToString();

                    if (_targetBucket.Earning > 0)
                    {
                        WinBackgroundImage.Visibility = Visibility.Visible;
                        BT_Donate.Visibility = Visibility.Visible;
                    }

                    // refresh the lottery result.
                    Lottery lot = LuckyBallsData.DataManageBase.Instance().History.LotteryInIssue(_targetBucket.Issue);
                    if (lot != null)
                    {
                        int[] reds = lot.Scheme.GetRedNums();
                        TB_Red1.Text = reds[0].ToString().PadLeft(2, '0');
                        TB_Red2.Text = reds[1].ToString().PadLeft(2, '0');
                        TB_Red3.Text = reds[2].ToString().PadLeft(2, '0');
                        TB_Red4.Text = reds[3].ToString().PadLeft(2, '0');
                        TB_Red5.Text = reds[4].ToString().PadLeft(2, '0');
                        TB_Red6.Text = reds[5].ToString().PadLeft(2, '0');
                        TB_Blue.Text = lot.Scheme.Blue.ToString().PadLeft(2, '0');
                    }

                    // Don't add the pending purchase.
                    List<PurchaseInfo> purchaseList = _targetBucket.Orders.ToList();
                    purchaseList.Remove(LBDataManager.GetInstance().PendingPurchase);
                    LV_RecentPurchases.ItemsSource = purchaseList;               
                }
            });
        }

        private void UpdateControlsStatus()
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                PurchaseInfo _selectedPurchase = LV_RecentPurchases.SelectedItem as PurchaseInfo;
                if (_selectionEnabled && _selectedPurchase != null)
                {
                    BT_DeleteSelectedPurchase.Visibility = Visibility.Visible;
                    BT_EditSelectedPurchase.Visibility = _targetBucket.Released ? Visibility.Collapsed : Visibility.Visible;
                    BT_ReuseSelectedPurchase.Visibility = _targetBucket.Released ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    BT_DeleteSelectedPurchase.Visibility = Visibility.Collapsed;
                    BT_EditSelectedPurchase.Visibility = Visibility.Collapsed;
                    BT_ReuseSelectedPurchase.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void LV_RecentPurchases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PurchaseSelected != null)
            {
                PurchaseSelected(LV_RecentPurchases.SelectedItem as PurchaseInfo);

                UpdateControlsStatus();
            }
        } 

        public void SelectItem(PurchaseInfo item)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                LV_RecentPurchases.SelectedItem = item;
            });
        }

        private void BT_DeleteSelectedPurchase_Click(object sender, RoutedEventArgs e)
        {
            PurchaseInfo _selectedPurchase = LV_RecentPurchases.SelectedItem as PurchaseInfo;
            if (_selectedPurchase != null)
            {
                LBDataManager.GetInstance().PurchaseManager.DeletePurchase(_selectedPurchase).ContinueWith(tk =>
                {
                    if (PurchaseDeleted != null)
                    {
                        PurchaseDeleted(_selectedPurchase);
                    }
                });
            }
        }

        private void BT_EditSelectedPurchase_Click(object sender, RoutedEventArgs e)
        {
            PurchaseInfo _selectedPurchase = LV_RecentPurchases.SelectedItem as PurchaseInfo;
            if (_selectedPurchase != null)
            {
                LBDataManager.GetInstance().EditExistPurchase(_selectedPurchase);

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    LotterySelectionFloatPanel.Instance().SetTarget(_selectedPurchase);

                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(SelectionPage));
                });
            }
        }

        private void BT_ReuseSelectedPurchase_Click(object sender, RoutedEventArgs e)
        {
            PurchaseInfo _selectedPurchase = LV_RecentPurchases.SelectedItem as PurchaseInfo;
            if (_selectedPurchase != null)
            {
                _selectedPurchase.GetSource().ContinueWith(res =>
                {
                    LBDataManager.GetInstance().ReuseExistPurchase(res.Result).ContinueWith(newTarget =>
                    {
                        this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                        {
                            LotterySelectionFloatPanel.Instance().SetTarget(newTarget.Result);

                            Frame rootFrame = Window.Current.Content as Frame;
                            rootFrame.Navigate(typeof(SelectionPage));
                        });
                    });
                });
            }
        }

        private void BT_Donate_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SettingPage), "Donation");
        }
    }
}
