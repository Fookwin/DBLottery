using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Windows.Media.Animation;

using Microsoft.Phone.Controls;

using LuckyBallsSpirit.DataModel;
using LuckyBallsSpirit.Pages;
using LuckyBallsSpirit.Controls;

using LuckyBallsData.Statistics;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public enum RefreshActionEnum
    {
        DoNothing = 0,
        RecalcualteAction = 1,
        FilterOnlyAction = 2,
        RefreshCountOnlyAction = 3
    }

    public sealed partial class LotterySelectionFloatPanel : UserControl
    {
        // temporary data for select count changing.
        private DispatcherTimer _countChangeTimer;
        private Int32 _countChangeDuration = 10;
        private double _countCurrent = 0;
        private double _countChangeStep = 1;

        private PurchaseInfo _pendingPurchase = null;
        private Purchase _targetPurchase = null;

        private bool _prevCalculateFailed = false;

        public LotterySelectionFloatPanel()
        {
            this.InitializeComponent();
        }

        public void Show()
        {
            // Update the controls and make the panel visible.
            Update();
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

                    Update();
                });
            }
        }

        private void Update()
        {
            if (_pendingPurchase != null && _targetPurchase != null)
            {
                bool bEmpty = _targetPurchase.Selection == null || _targetPurchase.Selection.Count == 0;

                this.Dispatcher.BeginInvoke(() =>
                {
                    // Update save button
                    BT_Save.IsEnabled = _pendingPurchase.Dirty && !bEmpty;

                    BT_New.IsEnabled = !bEmpty;
                    BT_Buy.IsEnabled = !bEmpty;

                    // Update the count.
                    RefreshCount();
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
                    this.Dispatcher.BeginInvoke(() =>
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
                if (MessageBox.Show("是否保存当期的方案？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    _pendingPurchase.Save().ContinueWith(tk =>
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            BT_Save.IsEnabled = false;

                            string msg = "方案" + _pendingPurchase.Index.ToString() + "已保存";
                            MessageCenter.AddMessage(msg, MessageType.Information, MessagePriority.Immediate, 3);
                        });
                    });
                }
                else
                {
                    return; // do nothing.
                }                

                    // Add new purchase.
                CreatePendingPurchase();
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
            Uri target = new Uri("/Pages/LotteryExportPage.xaml", UriKind.Relative);
            PhoneApplicationFrame appFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            //if (_pendingPurchase.Dirty)
            //{
            //    // Save the order before buying.
            //    _pendingPurchase.Save().ContinueWith(tk =>
            //    {
            //        this.Dispatcher.BeginInvoke(() =>
            //        {
            //            string msg = "方案" + _pendingPurchase.Index.ToString() + "已保存";
            //            MessageCenter.AddMessage(msg, MessageType.Information, MessagePriority.Immediate, 3);

            //            appFrame.Navigate(target);
            //        });
            //    });

            //    BT_Save.IsEnabled = false;
            //}
            //else
            //{
                appFrame.Navigate(target);
            //}
        }
    }
}
