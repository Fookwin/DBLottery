using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Windows.UI.Xaml.Media.Animation;

using LuckyBallSpirit.DataModel;
using LuckyBallSpirit.Pages;

using LuckyBallsData.Statistics;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class LotterySelectionFloatPanel : UserControl
    {
        public enum RefreshActionEnum
        {
            RecalcualteAction = 0,
            FilterOnlyAction = 1,
            RefreshCountOnlyAction = 2
        }

        public enum LSDisplayModeEnum
        {
            Normal = 0,
            Experss = 1,
            Extend = 2
        }

        private static LotterySelectionFloatPanel _sInstance = null;

        public static LotterySelectionFloatPanel Instance()
        {
            if (_sInstance == null)
            {
                _sInstance = new LotterySelectionFloatPanel();
                _sInstance.SetTarget(LBDataManager.GetInstance().PendingPurchase); // add the pending purchase as default.
            }

            return _sInstance;
        }

        // temporary data for select count changing.
        private DispatcherTimer _countChangeTimer;
        private Int32 _countChangeDuration = 10;
        private double _countCurrent = 0;
        private double _countChangeStep = 1;

        private LSDisplayModeEnum _displayMode = LSDisplayModeEnum.Experss;
        private bool _firstTime = true;
        private PurchaseInfo _pendingPurchase = null;
        private Purchase _targetPurchase = null;

        private bool _prevCalculateFailed = false;

        public delegate void EditingTargetCreatedEventsHandler();
        public event EditingTargetCreatedEventsHandler EditingTargetCreated = null;

        public delegate void SelectionChangedEventHandler();
        public event SelectionChangedEventHandler SelectionChanged = null;

        public LotterySelectionFloatPanel()
        {
            this.InitializeComponent();
        }

        public void Show(LSDisplayModeEnum dispMode)
        {
            if (_displayMode != dispMode)
            {
                _displayMode = dispMode;

                if (dispMode == LSDisplayModeEnum.Extend)
                {         
                    DetailControlsPanel.Visibility = Visibility.Visible; 
                }
                else
                {
                    DetailControlsPanel.Visibility = Visibility.Collapsed;
                }
            }
            
            // Update the controls and make the panel visible.
            Update();

            if (_firstTime)
            {
                // put the icon to the left bottom.
                double ScreenW = Window.Current.Bounds.Width;
                double ScreenH = Window.Current.Bounds.Height;
                MainPanel.HorizontalOffset = 5;
                MainPanel.VerticalOffset = ScreenH - 145;

                _firstTime = false;
            }

            MainPanel.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            MainPanel.Visibility = Visibility.Collapsed;
        }

        public void Refresh(RefreshActionEnum action)
        {
            _pendingPurchase.Dirty = true;

            if (action == RefreshActionEnum.RecalcualteAction || _prevCalculateFailed)
            {
                // Recalculate the initive scheme selection.
                try
                {
                    _targetPurchase.Selection.Clear();
                    foreach (SchemeSelector selector in _targetPurchase.Selectors)
                    {
                        _targetPurchase.Selection.AddRange(selector.GetResult());
                    }
                }
                catch (OutOfMemoryException)
                {
                    MessageCenter.AddMessage(":( 没有足够内存完成此次计算！", DataModel.MessageType.Error,
                        DataModel.MessagePriority.Immediate, 3);

                    _prevCalculateFailed = true; // set the flag for the rebuild next time.

                    return;
                }
            }

            if (action == RefreshActionEnum.RecalcualteAction ||
                action == RefreshActionEnum.FilterOnlyAction ||
                _prevCalculateFailed)
            {
                // Re-filtering the scheme selection.
                List<Scheme> candidates = _targetPurchase.Selection;
                SelectUtil.Filter(ref candidates, _targetPurchase.Filters);
            }

            // Inform the selection list to refresh.
            if (SelectionChanged != null && action != RefreshActionEnum.RefreshCountOnlyAction)
            {
                SelectionChanged();
            }

            // Update UI.
            Update();

            _prevCalculateFailed = false;
        }

        public void SetTarget(PurchaseInfo target)
        {
            if (_pendingPurchase != target && target != null)            
            {
                _pendingPurchase = target;
                _targetPurchase = null;

                _pendingPurchase.GetSource().ContinueWith(res =>
                {
                    _targetPurchase = res.Result;

                    // Inform the editor that the solution changed.
                    if (EditingTargetCreated != null)
                        EditingTargetCreated();

                    Update();
                });
            }
        }

        private void Update()
        {
            if (_pendingPurchase != null && _targetPurchase != null)
            {
                bool bEmpty = _targetPurchase.Selection == null || _targetPurchase.Selection.Count == 0;

                // Update the background color of the icon.
                Color bkClr = bEmpty ? Colors.DimGray : Colors.DarkRed;

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    SolidColorBrush brush = ToggleIconBorder.Background as SolidColorBrush;
                    brush.Color = bkClr;

                    // Update save button
                    BT_Save.IsEnabled = _pendingPurchase.Dirty && !bEmpty;

                    BT_New.IsEnabled = !bEmpty;
                    BT_Buy.IsEnabled = !bEmpty;

                    // Update the count.
                    RefreshCount();

                    BorderFlashStoryBoard.Begin();
                });
            }
        }

        private void RefreshCount()
        {
            _countCurrent = Convert.ToInt32(SelectCount.Text);
            int newCount = _pendingPurchase.SchemeCount;
            if (_countCurrent != newCount)
            {
                // Set timer.
                if (_countChangeTimer == null)
                {
                    _countChangeTimer = new DispatcherTimer();
                    _countChangeTimer.Interval = new TimeSpan(100);
                    _countChangeTimer.Tick += timer_Tick;
                }

                double change = newCount - _countCurrent;
                _countChangeStep = change / 10;
                _countChangeDuration = 10;

                _countChangeTimer.Start();
            }
        }

        private void timer_Tick(object sender, object e)
        {
            if (TimeCountDown())
            {
                _countCurrent += _countChangeStep;

                int count = (int)Math.Round(_countCurrent);
                SelectCount.Text = count.ToString();
            }
            else
            {
                _countChangeTimer.Stop();
            }
        }

        public bool TimeCountDown()
        {
            if (_countChangeDuration <= 0)
                return false;
            else
            {
                _countChangeDuration --;
                return true;
            }
        }

        // Events 
        //
        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            if (_pendingPurchase.Dirty)
            {
                // Save the purchase information.
                _pendingPurchase.Save().ContinueWith(res =>
                {
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        string msg = "方案" + _pendingPurchase.Index.ToString() + "已保存";
                        MessageCenter.AddMessage(msg, MessageType.Information, MessagePriority.Immediate, 3);
                    });
                });

                BT_Save.IsEnabled = false;
            }
        }

        private void BT_New_Click(object sender, RoutedEventArgs e)
        {
            if (_pendingPurchase.Dirty)
            {
                Controls.MessageBox.Show("是否保存当期的方案？", "提示", Controls.MessageBoxButton.YesNoCancel).ContinueWith(res =>
                {
                    if (res.Result == Controls.MessageBoxResult.Yes)
                    {
                        _pendingPurchase.Save().ContinueWith(tk =>
                        {
                            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                BT_Save.IsEnabled = false;

                                string msg = "方案" + _pendingPurchase.Index.ToString() + "已保存";
                                MessageCenter.AddMessage(msg, MessageType.Information, MessagePriority.Immediate, 3);
                            });
                        });
                    }
                    else if (res.Result == Controls.MessageBoxResult.Cancel)
                    {
                        return; // do nothing.
                    }                

                    // Add new purchase.
                    CreatePendingPurchase();
                });
            }
            else
            {
                // Add new purchase.
                CreatePendingPurchase();
            }
        }

        private void CreatePendingPurchase()
        {
            // Add new purchase.
            LBDataManager.GetInstance().AddEmptyPurchase().ContinueWith(tk =>
            {
                SetTarget(tk.Result);
            });
        }

        private void BT_Buy_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (_pendingPurchase.Dirty)
            {
                // Save the order before buying.
                _pendingPurchase.Save().ContinueWith(tk =>
                {
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        string msg = "方案" + _pendingPurchase.Index.ToString() + "已保存";
                        MessageCenter.AddMessage(msg, MessageType.Information, MessagePriority.Immediate, 3);

                        rootFrame.Navigate(typeof(PurchasePage), _targetPurchase.Selection);
                    });
                });

                BT_Save.IsEnabled = false;
            }
            else
            {
                rootFrame.Navigate(typeof(PurchasePage), _targetPurchase.Selection);
            }
        }

        private void BT_ToggleIcon_Click(object sender, RoutedEventArgs e)
        {
            if (_displayMode == LSDisplayModeEnum.Experss)
            {
                // navigate to selection page.
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(SelectionPage));
            }
            else
            {
                _displayMode = (_displayMode == LSDisplayModeEnum.Extend) ? LSDisplayModeEnum.Normal : LSDisplayModeEnum.Extend;
                DetailControlsPanel.Visibility = (_displayMode == LSDisplayModeEnum.Extend) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
