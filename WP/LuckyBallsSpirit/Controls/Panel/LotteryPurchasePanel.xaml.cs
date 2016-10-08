using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LuckyBallsData.Selection;
using LuckyBallsData.Statistics;
using LuckyBallsSpirit.DataModel;

namespace LuckyBallsSpirit.Controls
{
    public partial class LotteryPurchasePanel : UserControl
    {
       public delegate void PurchaseSelectedEventHandler(PurchaseInfo selected);
        public event PurchaseSelectedEventHandler PurchaseSelected;

        public delegate void PurchaseDeletedEventHandler(PurchaseInfo selected);
        public event PurchaseDeletedEventHandler PurchaseDeleted;

        private PurchaseBucketInfo _targetBucket = null;
        private bool _selectionEnabled = false;

        public LotteryPurchasePanel()
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
            this.Dispatcher.BeginInvoke(() =>
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

                if (_targetBucket != null)
                {
                    // Refresh the context.
                    TB_Issue.Text = _targetBucket.Issue.ToString();
                    TB_Cost.Text = _targetBucket.Cost.ToString();
                    TB_Earning.Text = _targetBucket.Earning.ToString();

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
            this.Dispatcher.BeginInvoke(() =>
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
            this.Dispatcher.BeginInvoke(() =>
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

                this.Dispatcher.BeginInvoke(() =>
                {
                    //LotterySelectionFloatPanel.Instance().SetTarget(_selectedPurchase);

                    //Frame rootFrame = Window.Current.Content as Frame;
                    //rootFrame.Navigate(typeof(SelectionPage));
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
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            //LotterySelectionFloatPanel.Instance().SetTarget(newTarget.Result);

                            //Frame rootFrame = Window.Current.Content as Frame;
                            //rootFrame.Navigate(typeof(SelectionPage));
                        });
                    });
                });
            }
        }
    }
}
