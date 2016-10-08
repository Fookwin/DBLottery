using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LuckyBallsSpirit.DataModel;
using LuckyBallsSpirit.Controls;

namespace LuckyBallsSpirit.Pages
{
    public partial class LotterySelectionPage : PhoneApplicationPage
    {
        private bool _initialized = false;

        private int m_nSomethingInEditing = 0;
        private List<string> m_extendInformations = new List<string> { "显示遗漏", "显示冷热", "显示胆杀" };

        public LotterySelectionPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!_initialized)
            {
                if (LBDataManager.GetInstance().PendingPurchase != null)
                {
                    InitControls();

                    // show float panel.
                    OperationMenuControl.SetTarget(LBDataManager.GetInstance().PendingPurchase);
                    OperationMenuControl.Show();

                    Panel_NumberSelection.OnBeginEdit += delegate
                    {
                        ++m_nSomethingInEditing;

                        this.Dispatcher.BeginInvoke(() =>
                        {
                            // hide the operation menu control.
                            OperationMenuControl.Visibility = Visibility.Collapsed;
                        });

                    };

                    Panel_NumberSelection.OnEndEdit += delegate(bool canceled, RefreshActionEnum action)
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            if (--m_nSomethingInEditing <= 0)
                            {
                                // hide the operation menu control.
                                OperationMenuControl.Visibility = Visibility.Visible;
                            }

                            if (!canceled)
                            {
                                OperationMenuControl.Refresh(action);
                            }
                        });

                    };

                    Panel_SchemeFilters.OnBeginEdit += delegate
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            ++m_nSomethingInEditing;

                            // hide the operation menu control.
                            OperationMenuControl.Visibility = Visibility.Collapsed;
                        });
                    };

                    Panel_SchemeFilters.OnEndEdit += delegate(bool canceled, RefreshActionEnum action)
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            if (--m_nSomethingInEditing <= 0)
                            {
                                // hide the operation menu control.
                                OperationMenuControl.Visibility = Visibility.Visible;
                            }

                            if (!canceled)
                            {
                                OperationMenuControl.Refresh(action);
                            }
                        });
                    };

                    pageHeaderPanel.OnExtendMenuClicked += delegate
                    {
                        ValuePickerFlyoutCombo picker = ValuePickerFlyoutCombo.UniqueInstance;
                        picker.Show("显示号码信息", m_extendInformations, false, true, new List<int>() { m_extendInformations.IndexOf(pageHeaderPanel.ExtendMenuText) });
                        picker.ValueConfirmed += delegate(List<string> selected)
                        {
                            if (selected.Count > 0)
                                ChangeNumberExtendInfo(selected[0]);
                        };
                    };
                }

                _initialized = true;
            }
            else
            {
                // any infomration from previous page.
                if (Pivot_Panel.SelectedIndex == 1)
                {
                    Panel_SchemeFilters.OnNavigatedTo(this.NavigationContext);
                }
            }
        }

        private void InitControls()
        {
            // Get the new pending purchase and update controls.
            LBDataManager.GetInstance().PendingPurchase.GetSource().ContinueWith(res =>
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    Panel_NumberSelection.SetTarget(res.Result.Selectors);
                    Panel_SchemeFilters.SetTarget(res.Result.Filters);
                });
            });
        }

        private void ChangeNumberExtendInfo(string infoName)
        {
            pageHeaderPanel.ExtendMenuText = infoName;

            int infoIndex = m_extendInformations.IndexOf(infoName);
            Panel_NumberSelection.ShowNumberDescription(infoIndex);
            Panel_SchemeFilters.ShowNumberDescription(infoIndex);
        }
    }
}
