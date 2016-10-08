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
using LuckyBallsData.Selection;
using LuckyBallSpirit.DataModel;
using LuckyBallsData.Statistics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class PurchaseDetailPanel : UserControl
    {
        public delegate void PuchaseChangedEventHandler();
        public event PuchaseChangedEventHandler OnSelectorAdded = null;
        public event PuchaseChangedEventHandler OnFilterAdded = null;

        private List<VerifiedScheme> _schemeList = new List<VerifiedScheme>();
        private List<VerifiedFilter> _filterList = new List<VerifiedFilter>();
        private Purchase _pendingPurchase = null;

        public PurchaseDetailPanel()
        {
            this.InitializeComponent();

            LBDataManager.GetInstance().PendingPurchase.GetSource().ContinueWith(res =>
            {
                _pendingPurchase = res.Result;
            });
        }

        public void SetPurcase(DataModel.PurchaseInfo purchase)
        {
            _schemeList.Clear();
            _filterList.Clear();

            purchase.GetSource().ContinueWith(res =>
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    LV_NumberSelection.ItemsSource = res.Result.Selectors;

                    int refIndex = LuckyBallsData.DataManageBase.Instance().History.IssueToIndex(purchase.Issue) - 1; 
                   
                    Scheme refScheme = null;
                    Lottery lot = LuckyBallsData.DataManageBase.Instance().History.LotteryInIssue(purchase.Issue);
                    if (lot != null)
                        refScheme = lot.Scheme;

                    foreach (Scheme item in res.Result.Selection)
                    {
                        _schemeList.Add(new VerifiedScheme(item, refScheme));
                    }

                    LV_SchemeSelection.ItemsSource = null;
                    LV_SchemeSelection.ItemsSource = _schemeList;
                    
                    foreach (Constraint item in res.Result.Filters)
                    {
                        _filterList.Add(new VerifiedFilter(item, refScheme, refIndex));
                    }

                    LV_FilterSelection.ItemsSource = null;
                    LV_FilterSelection.ItemsSource = _filterList;
                });
            });
        }

        private void BT_AddNumSetToSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (SchemeSelector sel in LV_NumberSelection.SelectedItems)
            {
                _pendingPurchase.Selectors.Add(sel.Clone());
            }

            string msg = LV_NumberSelection.SelectedItems.Count.ToString() + "组选号已加入当前方案";
            MessageCenter.AddMessage(msg, DataModel.MessageType.Information, DataModel.MessagePriority.Immediate, 3);

            if (OnSelectorAdded != null)
                OnSelectorAdded();

            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                // Clear selection.
                LV_NumberSelection.SelectedItems.Clear();
            });
        }

        private void BT_AddFilterToSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (Constraint sel in LV_FilterSelection.SelectedItems)
            {
                _pendingPurchase.Filters.Add(sel.Clone());
            }

            string msg = LV_FilterSelection.SelectedItems.Count.ToString() + "组过滤条件已加入当前方案";
            MessageCenter.AddMessage(msg, DataModel.MessageType.Information, DataModel.MessagePriority.Immediate, 3);

            if (OnFilterAdded != null)
                OnFilterAdded();

            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                // Clear selection.
                LV_FilterSelection.SelectedItems.Clear();
            });
        }

        private void BT_AddSchemeToSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (VerifiedScheme sel in LV_SchemeSelection.SelectedItems)
            {
                _pendingPurchase.Selectors.Add(new StandardSchemeSelector()
                {
                    SelectedBlues = new Set(new int[] { sel.Target.Blue }),
                    SelectedReds = new Set(sel.Target.GetRedNums())
                });
            }

            string msg = LV_SchemeSelection.SelectedItems.Count.ToString() + "组号码已加入当前方案";
            MessageCenter.AddMessage(msg, DataModel.MessageType.Information, DataModel.MessagePriority.Immediate, 3);

            if (OnSelectorAdded != null)
                OnSelectorAdded();

            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                // Clear selection.
                LV_SchemeSelection.SelectedItems.Clear();
            });
        }

        private void BT_BT_SelectAllSchemes_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                LV_SchemeSelection.SelectAll();
            });
        }

        private void BT_UnselectAllSchemes_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                LV_SchemeSelection.SelectedItems.Clear();
            });            
        }

        private void LV_SchemeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BT_AddSchemeToSelection.Visibility = _pendingPurchase != null && LV_SchemeSelection.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LV_NumberSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BT_AddNumSetToSelection.Visibility = _pendingPurchase != null && LV_NumberSelection.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LV_FilterSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BT_AddFilterToSelection.Visibility = _pendingPurchase != null && LV_FilterSelection.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
