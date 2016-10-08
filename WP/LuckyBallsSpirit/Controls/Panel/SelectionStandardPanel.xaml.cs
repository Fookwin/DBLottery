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
using System.Windows.Navigation;
using LuckyBallsData.Selection;
using LuckyBallsSpirit.Controls;
using LuckyBallsSpirit.DataModel;
using LuckyBallsData.Util;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class StandardSelectionPanel : UserControl, ISelectionPanelBase
    {
        private StandardSchemeSelector selector = new StandardSchemeSelector();
        private bool _initialized = false;
        private static List<string> redRandomRange = new List<string> 
        { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };
        private static List<string> blueRandomRange = new List<string>
        { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

        public StandardSelectionPanel()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                Panel_RedSelection.SelectMode = NumButton.SelectModeEnum.DuplexSelectable;
                Panel_BlueSelection.SelectMode = NumButton.SelectModeEnum.DuplexSelectable;

                Panel_RedSelection.SelectionChanged += OnRedSelectionChanged;
                Panel_BlueSelection.SelectionChanged += OnBlueSelectionChanged;

                // Init the blue-red connection way.
                CB_BlueAttachOption.Values = new List<string>() { "蓝球复式", "按注顺序选择蓝球", "按注随机选择蓝球" };
                CB_BlueAttachOption.ValueIndex = 0;
                CB_BlueAttachOption.ValueChanged += BlueAttachOption_SelectionChanged;

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
            this.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void ShowNumberDescription(int infoIndex)
        {
            Panel_RedSelection.NumDescriptionType = infoIndex;
            Panel_BlueSelection.NumDescriptionType = infoIndex;
        }

        private void UpdateSummary()
        {
            TB_RedCount.Text = selector.SelectedReds.Count.ToString();
            TB_BlueCount.Text = selector.SelectedBlues.Count.ToString();
            TB_SelectedCount.Text = selector.GetSchemeCount().ToString();
        }

        private void RandomRed()
        {
            int count = Convert.ToInt32(RamdonRedCountText.Text);

            Set random = SelectUtil.RandomReds(new Set(new Region(1, 33)), count);
            Panel_RedSelection.SetNumsSelectionState(random, NumButton.SelectStatusEnum.Selected, true);

            selector.SelectedReds.Reset(random);

            // Update the matrix filter control.
            UpdateMatrixFilterCtrl();

            UpdateSummary();
        }

        private void RandomBlue()
        {
            int count = Convert.ToInt32(RamdonBlueCountText.Text);

            Set random = SelectUtil.RandomBlues(new Set(new Region(1, 16)), count);
            Panel_BlueSelection.SetNumsSelectionState(random, NumButton.SelectStatusEnum.Selected, true);

            selector.SelectedBlues.Reset(random);
            UpdateSummary();

            if (selector.SelectedBlues.Count > 1)
            {
                CB_BlueAttachOption.IsEnabled = true;
            }
            else
            {
                CB_BlueAttachOption.IsEnabled = false;
            }
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

        private void UpdateControls()
        {
            if (selector == null)
                return;

            this.Dispatcher.BeginInvoke(() =>
            {
                Panel_RedSelection.SetNumsSelectionState(selector.SelectedReds, NumButton.SelectStatusEnum.Selected, true);
                Panel_BlueSelection.SetNumsSelectionState(selector.SelectedBlues, NumButton.SelectStatusEnum.Selected, true);
                CB_BlueAttachOption.IsEnabled = selector.SelectedBlues.Count > 1;

                CB_BlueAttachOption.ValueIndex = (int)selector.BlueConnectionType;

                CB_MatrixFilterEnabled.IsChecked = selector.ApplyMatrixFilter;
                UpdateMatrixFilterCtrl();

                // Update Summary.
                UpdateSummary();
            });
        }

        private void BT_RamdonRed_Click(object sender, RoutedEventArgs e)
        {
            RandomRed();
        }

        private void BT_RamdonBlue_Click(object sender, RoutedEventArgs e)
        {
            RandomBlue();
        }

        private void BT_RamdonBlueCount_Click(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(RamdonBlueCountText.Text);

            ValuePickerFlyoutCombo picker = ValuePickerFlyoutCombo.UniqueInstance;
            picker.Show("随机选择篮球个数", blueRandomRange, false, true, new List<int>() { count - 1 });
            picker.ValueConfirmed += BlueRandomCount_SelectionChanged;
        }

        private void BT_RamdonRedCount_Click(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(RamdonRedCountText.Text);

            ValuePickerFlyoutCombo picker = ValuePickerFlyoutCombo.UniqueInstance;
            picker.Show("随机选择红球个数", redRandomRange, false, true, new List<int>() { count - 1 });
            picker.ValueConfirmed += RedRandomCount_SelectionChanged;
        }

        private void RedRandomCount_SelectionChanged(List<string> selected)
        {
            if (selected.Count > 0)
            {
                // update the count control.
                RamdonRedCountText.Text = selected[0];

                // select randomly.
                RandomRed();
            }
        }

        private void BlueRandomCount_SelectionChanged(List<string> selected)
        {
            if (selected.Count > 0)
            {
                // update the count control.
                RamdonBlueCountText.Text = selected[0];

                // select randomly.
                RandomBlue();
            }
        }

        private void BlueAttachOption_SelectionChanged()
        {
            selector.BlueConnectionType = (StandardSchemeSelector.RedBlueConnectionTypeEnum)CB_BlueAttachOption.ValueIndex;
            UpdateSummary();
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
                this.Dispatcher.BeginInvoke(() =>
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
    }
}
