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
using LuckyBallsData.Util;
using LuckyBallsSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class SelectionDanTuoPanel : UserControl, ISelectionPanelBase
    {
        private DantuoSchemeSelector selector = new DantuoSchemeSelector();
        private static List<string> redRandomRange = new List<string> 
        { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };
        private static List<string> blueRandomRange = new List<string> 
        { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

        public SelectionDanTuoPanel()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                Panel_RedSelection.SelectMode = NumButton.SelectModeEnum.TriplexSelectableWithHighlight;
                Panel_BlueSelection.SelectMode = NumButton.SelectModeEnum.DuplexSelectable;

                Panel_RedSelection.SelectionChanged += OnRedSelectionChanged;
                Panel_BlueSelection.SelectionChanged += OnBlueSelectionChanged;

                UpdateControls();
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
            DantuoSchemeSelector standardSel = _selector as DantuoSchemeSelector;
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
            TB_RedCount.Text = (selector.SelectedTuos.Count + selector.SelectedDans.Count).ToString();
            TB_DanCount.Text = selector.SelectedDans.Count.ToString();
            TB_BlueCount.Text = selector.SelectedBlues.Count.ToString();
            TB_SelectedCount.Text = selector.GetSchemeCount().ToString();
        }

        private void OnRedSelectionChanged(object sender, NumButton.NumStateChangeArg e)
        {
            if (e.number <= 0)
                return;

            if (e.toState == NumButton.SelectStatusEnum.NotSelected)
            {
                selector.SelectedTuos.Remove(e.number);
                selector.SelectedDans.Remove(e.number);
            }
            else if (e.toState == NumButton.SelectStatusEnum.Selected)
            {
                selector.SelectedTuos.Add(e.number);
            }
            else if (e.toState == NumButton.SelectStatusEnum.Highlighted)
            {
                selector.SelectedTuos.Remove(e.number);
                selector.SelectedDans.Add(e.number);
            }

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

            // Update Summary.
            UpdateSummary();
        }

        private void UpdateControls()
        {
            if (selector == null)
                return;

            Panel_RedSelection.SetNumsSelectionState(selector.SelectedTuos, NumButton.SelectStatusEnum.Selected, true);
            Panel_RedSelection.SetNumsSelectionState(selector.SelectedDans, NumButton.SelectStatusEnum.Highlighted, false);
            Panel_BlueSelection.SetNumsSelectionState(selector.SelectedBlues, NumButton.SelectStatusEnum.Selected, true);

            // Update Summary.
            UpdateSummary();
        }

        private void RandomRed()
        {
            int toSelectCount = Convert.ToInt32(RamdonRedCountText.Text);
            int[] danNums = selector.SelectedDans.Numbers;
            if (toSelectCount > danNums.Count())
            {
                Set candidates = new Set(new Region(1, 33));
                for (int i = 0; i < danNums.Count(); ++i)
                {
                    candidates.Remove(danNums[i]);
                }

                Set random = SelectUtil.RandomReds(candidates, toSelectCount - danNums.Count());
                selector.SelectedTuos.Reset(random);

                Panel_RedSelection.SetNumsSelectionState(selector.SelectedTuos, NumButton.SelectStatusEnum.Selected, true);
                Panel_RedSelection.SetNumsSelectionState(selector.SelectedDans, NumButton.SelectStatusEnum.Highlighted, false);

                UpdateSummary();
            }
        }

        private void RandomBlue()
        {
            int count = Convert.ToInt32(RamdonBlueCountText.Text);

            Set random = SelectUtil.RandomBlues(new Set(new Region(1, 16)), count);
            Panel_BlueSelection.SetNumsSelectionState(random, NumButton.SelectStatusEnum.Selected, true);

            selector.SelectedBlues.Reset(random);
            UpdateSummary();
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
    }
}
