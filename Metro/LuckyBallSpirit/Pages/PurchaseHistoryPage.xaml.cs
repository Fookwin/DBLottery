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
using LuckyBallsData.Selection;
using LuckyBallSpirit.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LuckyBallSpirit.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PurchaseHistoryPage : LuckyBallSpirit.Common.LayoutAwarePage
    {
        public class PurchaseItem
        {
            public PurchaseBucketInfo Bucket = null;
            public PurchaseInfo Purchase = null;
        }

        private bool _initialized = false;
        private PurchaseBucketInfo _selectedBucket = null;
        private PurchaseInfo _selectedPurchase = null;

        public PurchaseHistoryPage()
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
                    Dictionary<int, PurchaseBucketInfo> allPurchases = LBDataManager.GetInstance().PurchaseManager.AllPurchases;
                    Issue_List.ItemsSource = allPurchases;
                    CL_PurchaseBucket.PurchaseSelected += OnPurchaseSelected;
                    CL_PurchaseBucket.PurchaseDeleted += OnPurchaseDeleted;
                    CL_PurchaseBucket.SelectionEnabled = true;
                    CL_PurchaseBucket.Visibility = Visibility.Collapsed;
                    CL_PurchaseDetail.Visibility = Visibility.Collapsed;

                    if (_selectedBucket != null)
                    {
                        int index = 0;
                        foreach (KeyValuePair<int, PurchaseBucketInfo> pair in allPurchases)
                        {
                            if (pair.Key == _selectedBucket.Issue)
                            {
                                // select this bucket and update the purchase info.
                                Issue_List.SelectedIndex = index;
                                SelectPurchaseBucket(pair.Value, _selectedPurchase);

                                break;
                            }

                            index++;
                        }
                    }

                    _initialized = true;
                });

                if (LBDataManager.GetInstance().PendingPurchase != null)
                {
                    CL_PurchaseDetail.OnFilterAdded += ResuedFilters;
                    CL_PurchaseDetail.OnSelectorAdded += ResuedSelectors;
                }
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
            PurchaseItem item = navigationParameter as PurchaseItem;
            if (item != null)
            {
                _selectedPurchase = item.Purchase;
                _selectedBucket = item.Bucket;
            }
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

            pageFootPanel.ActiveCommand = PageFootPanel.PageIndexEnum.PersonalPage;
        }

        private void ResuedFilters()
        {
            LotterySelectionFloatPanel.Instance().Refresh(LotterySelectionFloatPanel.RefreshActionEnum.FilterOnlyAction);
        }

        private void ResuedSelectors()
        {
            LotterySelectionFloatPanel.Instance().Refresh(LotterySelectionFloatPanel.RefreshActionEnum.RecalcualteAction);
        }

        private void Issue_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialized)
            {
                KeyValuePair<int, PurchaseBucketInfo>? selected = Issue_List.SelectedItem as KeyValuePair<int, PurchaseBucketInfo>?;
                SelectPurchaseBucket(selected != null ? selected.Value.Value : null, null);
            }
        }

        private void SelectPurchaseBucket(PurchaseBucketInfo bucket, PurchaseInfo preSelInfo)
        {
            if (bucket != null)
            {
                // initialize it before using.
                bucket.Initialize().ContinueWith(res =>
                {
                    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                    {
                        CL_PurchaseBucket.Visibility = Visibility.Visible;
                        CL_PurchaseBucket.Target = bucket;
                        BT_DeleteSelected.Visibility = Visibility.Visible;

                        if (preSelInfo != null)
                        {
                            CL_PurchaseBucket.SelectItem(preSelInfo);
                        }
                    });

                    _selectedBucket = bucket;
                    _selectedPurchase = preSelInfo;
                });
            }
            else
            {
                _selectedBucket = null;
                _selectedPurchase = null;
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    CL_PurchaseBucket.Visibility = Visibility.Collapsed;
                    CL_PurchaseDetail.Visibility = Visibility.Collapsed;
                    BT_DeleteSelected.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void OnPurchaseSelected(PurchaseInfo selected)
        {
            if (_initialized)
            {
                _selectedPurchase = selected;

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    if (selected != null)
                    {
                        CL_PurchaseDetail.Visibility = Visibility.Visible;
                        CL_PurchaseDetail.SetPurcase(selected);
                    }
                    else
                    {
                        CL_PurchaseDetail.Visibility = Visibility.Collapsed;
                    }
                });
            }
        }

        private void BT_CleanAll_Click(object sender, RoutedEventArgs e)
        {
            LBDataManager.GetInstance().PurchaseManager.DeleteAll();

            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                SelectPurchaseBucket(null, null);

                Issue_List.ItemsSource = null;
            });
        }

        private void BT_DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<int, PurchaseBucketInfo>? selected = Issue_List.SelectedItem as KeyValuePair<int, PurchaseBucketInfo>?;
            if (selected != null)
            {
                LBDataManager.GetInstance().PurchaseManager.DeleteBucket(selected.Value.Value);

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    SelectPurchaseBucket(null, null);

                    Issue_List.ItemsSource = null;
                    Issue_List.ItemsSource = LBDataManager.GetInstance().PurchaseManager.AllPurchases;
                });
            }
        }

        private void OnPurchaseDeleted(PurchaseInfo selected)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                CL_PurchaseBucket.Refresh();
            });
        }
    }
}
