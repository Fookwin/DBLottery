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
using LuckyBallSpirit.Controls;
using LuckyBallsData.Util;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class StandardSelectionPanel : UserControl, ISelectionPanelBase
    {
        private StandardSchemeSelector selector = new StandardSchemeSelector();
        private bool _initialized = false;

        public StandardSelectionPanel()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                RB_RedState_YL.IsChecked = true;
                RB_BlueState_YL.IsChecked = true;

                Panel_RedSelection.SelectMode = NumButton.SelectModeEnum.DuplexSelectable;
                Panel_BlueSelection.SelectMode = NumButton.SelectModeEnum.DuplexSelectable;

                Panel_RedSelection.NumDescriptionType = 0;
                Panel_BlueSelection.NumDescriptionType = 0;

                Panel_RedSelection.SelectionChanged += OnRedSelectionChanged;
                Panel_BlueSelection.SelectionChanged += OnBlueSelectionChanged;

                // init combo boxes.
                int[] redRandomRange = new int[20];
                for (int i = 1; i <= 20; ++ i)
                {
                    redRandomRange[i- 1] = i;
                }
                CB_RedRandomCount.ItemsSource = redRandomRange;

                int[] blueRandomRange = new int[10];
                for (int i = 1; i <= 10; ++i)
                {
                    blueRandomRange[i - 1] = i;
                }
                CB_BlueRandomCount.ItemsSource = blueRandomRange;

                _initialized = true;
            };
        }

        public SchemeSelector Selector
        {
            get
            {
                return selector;
            }
        }

        public void SetSelector(SchemeSelector _selector)
        {
            StandardSchemeSelector standardSel = _selector as StandardSchemeSelector;
            if (standardSel == null)
                throw new Exception("Input selector does not match with this selection panel.");

            selector = standardSel;

            UpdateControls();
        }

        public void SetVisiblity(bool visible)
        {
            this.Visibility = visible ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void UpdateSummary()
        {
            TB_RedCount.Text = selector.SelectedReds.Count.ToString();
            TB_BlueCount.Text = selector.SelectedBlues.Count.ToString();
            TB_SelectedCount.Text = selector.GetSchemeCount().ToString();
        }

        private void OnRedSelectionChanged(object sender, NumButton.NumStateChangeArg e)
        {
            if (e.number <= 0)
                return;
            
            bool bSelected = e.toState == NumButton.SelectStatusEnum.Selected;

            if (bSelected)
                selector.SelectedReds.Add(e.number);
            else
                selector.SelectedReds.Remove(e.number);

            UpdateMatrixFilterCtrl();

            // Update Summary.
            UpdateSummary();
        }

        private void OnBlueSelectionChanged(object sender, NumButton.NumStateChangeArg e)
        {
            if (e.number <= 0)
                return;

            bool bSelected = e.toState == NumButton.SelectStatusEnum.Selected;

            if (bSelected)
                selector.SelectedBlues.Add(e.number);
            else
                selector.SelectedBlues.Remove(e.number);

            if (selector.SelectedBlues.Count > 1)
            {
                CB_BlueAttachOption.IsEnabled = true;
            }
            else
            {
                CB_BlueAttachOption.IsEnabled = false;
            }

            // Update Summary.
            UpdateSummary();
        }

        private void SelectAll(bool blue, bool bUnselect)
        {  
            if (blue)
            {                
                selector.SelectedBlues.Clear();
                if (!bUnselect)
                    selector.SelectedBlues.Reset(new Region(1, 16));

                Panel_BlueSelection.SetNumsSelectionState(selector.SelectedBlues, NumButton.SelectStatusEnum.Selected, true);

                if (selector.SelectedBlues.Count > 1)
                {
                    CB_BlueAttachOption.IsEnabled = true;
                }
                else
                {
                    CB_BlueAttachOption.IsEnabled = false;
                }
            }
            else
            {                
                selector.SelectedReds.Clear();
                if (!bUnselect)
                    selector.SelectedReds.Reset(new Region(1, 33));

                Panel_RedSelection.SetNumsSelectionState(selector.SelectedReds, NumButton.SelectStatusEnum.Selected, true);

                UpdateMatrixFilterCtrl();
            }
        }

        private void UpdateControls()
        {
            if (selector == null)
                return;

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Panel_RedSelection.SetNumsSelectionState(selector.SelectedReds, NumButton.SelectStatusEnum.Selected, true);
                Panel_BlueSelection.SetNumsSelectionState(selector.SelectedBlues, NumButton.SelectStatusEnum.Selected, true);
                CB_BlueAttachOption.IsEnabled = selector.SelectedBlues.Count > 1;

                CB_BlueAttachOption.SelectedIndex = (int)selector.BlueConnectionType;

                CB_MatrixFilterEnabled.IsChecked = selector.ApplyMatrixFilter;
                UpdateMatrixFilterCtrl();

                // Update Summary.
                UpdateSummary();
            });
        }

        private void BT_SelectAllRed_Click(object sender, RoutedEventArgs e)
        {
            SelectAll(false, false);
            UpdateSummary();
        }

        private void BT_ClearAllRed_Click(object sender, RoutedEventArgs e)
        {
            SelectAll(false, true);
            UpdateSummary();
        }

        private void BT_RamdonRed_Click(object sender, RoutedEventArgs e)
        {
            CB_RedRandomCount.Visibility = Visibility.Visible;

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                CB_RedRandomCount.IsDropDownOpen = true;
            });
        }

        private void BT_SelectAllBlue_Click(object sender, RoutedEventArgs e)
        {
            SelectAll(true, false);
            UpdateSummary();
        }

        private void BT_ClearAllBlue_Click(object sender, RoutedEventArgs e)
        {
            SelectAll(true, true);
            UpdateSummary();
        }

        private void BT_RamdonBlue_Click(object sender, RoutedEventArgs e)
        {
            CB_BlueRandomCount.Visibility = Visibility.Visible;

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                CB_BlueRandomCount.IsDropDownOpen = true;
            });
        }

        private void CB_RedRandomCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_RedRandomCount.SelectedItem != null)
            {
                Set random = SelectUtil.RandomReds(new Set(new Region(1, 33)), CB_RedRandomCount.SelectedIndex + 1);
                Panel_RedSelection.SetNumsSelectionState(random, NumButton.SelectStatusEnum.Selected, true);

                selector.SelectedReds.Reset(random);

                // Update the matrix filter control.
                UpdateMatrixFilterCtrl();

                UpdateSummary();

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    CB_RedRandomCount.SelectedItem = null; // clean the selection.
                    CB_RedRandomCount.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void CB_BlueRandomCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_BlueRandomCount.SelectedItem != null)
            {
                Set random = SelectUtil.RandomBlues(new Set(new Region(1, 16)), CB_BlueRandomCount.SelectedIndex + 1);
                Panel_BlueSelection.SetNumsSelectionState(random, NumButton.SelectStatusEnum.Selected, true);

                selector.SelectedBlues.Reset(random);
                UpdateSummary();

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    CB_BlueRandomCount.SelectedItem = null; // clean the selection.
                    CB_BlueRandomCount.Visibility = Visibility.Collapsed;

                    if (selector.SelectedBlues.Count > 1)
                    {
                        CB_BlueAttachOption.IsEnabled = true;
                    }
                    else
                    {
                        CB_BlueAttachOption.IsEnabled = false;
                    }
                });
            }
        }

        private void RedDesc_Changed(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            string name = btn.Name;

            if (name == "RB_RedState_YL")
            {
                Panel_RedSelection.NumDescriptionType = 0;
            }
            else if (name == "RB_RedState_LR")
            {
                Panel_RedSelection.NumDescriptionType = 1;
            }
            else if (name == "RB_RedState_DS")
            {
                Panel_RedSelection.NumDescriptionType = 2;
            }
            else if (name == "RB_BlueState_YL")
            {
                Panel_BlueSelection.NumDescriptionType = 0;
            }
            else if (name == "RB_BlueState_LR")
            {
                Panel_BlueSelection.NumDescriptionType = 1;
            }
            else if (name == "RB_BlueState_DS")
            {
                Panel_BlueSelection.NumDescriptionType = 2;
            }        
        }

        private void CB_BlueAttachOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialized)
            {
                selector.BlueConnectionType = (StandardSchemeSelector.RedBlueConnectionTypeEnum)CB_BlueAttachOption.SelectedIndex;
                UpdateSummary();
            }
        }

        private void CB_MatrixFilterEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized)
            {
                selector.ApplyMatrixFilter = CB_MatrixFilterEnabled.IsChecked != null && CB_MatrixFilterEnabled.IsChecked.Value;
                UpdateSummary();
            }
        }

        private void UpdateMatrixFilterCtrl()
        {
            bool bEnable = selector.SelectedReds.Count > 7 && selector.SelectedReds.Count <= 20;

            if (CB_MatrixFilterEnabled.IsEnabled != bEnable )
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (!bEnable)
                    {
                        // Uncheck the check box and not apply the matrix filtering.
                        selector.ApplyMatrixFilter = false;
                        CB_MatrixFilterEnabled.IsChecked = false;
                    }

                    CB_MatrixFilterEnabled.IsEnabled = bEnable;
                });
            }
        }

        private void CB_RedRandomCount_LostFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                CB_RedRandomCount.Visibility = Visibility.Visible;
            });
        }

        private void CB_BlueRandomCount_LostFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                CB_BlueRandomCount.Visibility = Visibility.Visible;
            });
        }
    }
}
