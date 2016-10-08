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
    public sealed partial class RandomSelectionPanel : UserControl, ISelectionPanelBase 
    {
        private RandomSchemeSelector selector = new RandomSchemeSelector();

        public RandomSelectionPanel()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                Panel_RedSelection.SelectMode = NumButton.SelectModeEnum.TriplexSelectableWithGrayed;
                Panel_BlueSelection.SelectMode = NumButton.SelectModeEnum.TriplexSelectableWithGrayed;

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
            RandomSchemeSelector standardSel = _selector as RandomSchemeSelector;
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

        private void OnRedSelectionChanged(object sender, NumButton.NumStateChangeArg e)
        {
            if (e.number <= 0)
                return;

            if (e.toState == NumButton.SelectStatusEnum.NotSelected)
            {
                selector.IncludedReds.Remove(e.number);
                selector.ExcludedReds.Remove(e.number);
            }
            else if (e.toState == NumButton.SelectStatusEnum.Selected)
            {
                selector.IncludedReds.Add(e.number);
            }
            else if (e.toState == NumButton.SelectStatusEnum.Grayed)
            {
                selector.ExcludedReds.Add(e.number);
                selector.IncludedReds.Remove(e.number);
            }
        }

        private void OnBlueSelectionChanged(object sender, NumButton.NumStateChangeArg e)
        {
            if (e.number <= 0)
                return;

            if (e.toState == NumButton.SelectStatusEnum.NotSelected)
            {
                selector.IncludedBlues.Remove(e.number);
                selector.ExcludedBlues.Remove(e.number);
            }
            else if (e.toState == NumButton.SelectStatusEnum.Selected)
            {
                selector.IncludedBlues.Add(e.number);
            }
            else if (e.toState == NumButton.SelectStatusEnum.Grayed)
            {
                selector.ExcludedBlues.Add(e.number);
                selector.IncludedBlues.Remove(e.number);
            }            
        }

        private void UnselectAll(bool blue)
        {
            if (blue)
            {
                selector.IncludedBlues.Clear();
                selector.ExcludedBlues.Clear();

                Panel_BlueSelection.SetNumsSelectionState(new Set(new Region(1, 16)), NumButton.SelectStatusEnum.NotSelected, true);
            }
            else
            {
                selector.IncludedReds.Clear();
                selector.ExcludedReds.Clear();

                Panel_RedSelection.SetNumsSelectionState(new Set(new Region(1, 33)), NumButton.SelectStatusEnum.NotSelected, true);
            }
        }

        private void UpdateControls()
        {
            if (selector == null)
                return;

            Panel_RedSelection.SetNumsSelectionState(selector.IncludedReds, NumButton.SelectStatusEnum.Selected, true);
            Panel_RedSelection.SetNumsSelectionState(selector.ExcludedReds, NumButton.SelectStatusEnum.Grayed, false);
            Panel_BlueSelection.SetNumsSelectionState(selector.IncludedBlues, NumButton.SelectStatusEnum.Selected, true);
            Panel_BlueSelection.SetNumsSelectionState(selector.ExcludedBlues, NumButton.SelectStatusEnum.Grayed, false);
        }        

        private void ValueEditorCtrl_ValueChanged(object sender, ValueChangeArgs e)
        {
            int input = selector.SelectedCount;

            input = Convert.ToInt32(Math.Round(SchemeCountValueEditor.Value));

            selector.SelectedCount = input;
        }
    }
}
