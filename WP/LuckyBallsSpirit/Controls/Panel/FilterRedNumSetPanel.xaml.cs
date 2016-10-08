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
using LuckyBallsSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class FilterRedNumSetPanel : UserControl, FilterPanelBase
    {
        private RedNumSetConstraint _constraint = new RedNumSetConstraint();

        public FilterRedNumSetPanel()
        {
            this.InitializeComponent();
            this.Loaded += delegate
            {
                Panel_RedSelection.SelectMode = NumButton.SelectModeEnum.DuplexSelectable;
                Panel_RedSelection.SelectionChanged += OnRedSelectionChanged;

                Ctrl_RangeSelection.ItemsSource = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
            };
        }
        
        public Constraint Constraint
        {
            get
            {
                RedNumSetConstraint copy = new RedNumSetConstraint();
                copy.SelectSet.Reset(_constraint.SelectSet);
                copy.HitLimits.Reset(_constraint.HitLimits);

                return copy;
            }
        }

        public void SetConstraint(Constraint _constr)
        {
            RedNumSetConstraint constraint = _constr as RedNumSetConstraint;
            if (constraint != null)
            {
                Ctrl_RangeSelection.SelectedItems.Clear();

                // Selected the items within the select set.
                foreach (int val in constraint.HitLimits.Numbers)
                {
                    Ctrl_RangeSelection.SelectedItems.Add(val);
                }

                Panel_RedSelection.SetNumsSelectionState(constraint.SelectSet, NumButton.SelectStatusEnum.Selected, true);

                _constraint = constraint;
            }           
        }

        public void OnNavigatedTo(NavigationContext context)
        {

        }

        public void ShowNumberDescription(int infoIndex)
        {
            Panel_RedSelection.NumDescriptionType = infoIndex;
        }
        
        private void OnRedSelectionChanged(object sender, NumButton.NumStateChangeArg e)
        {
            if (e.number <= 0)
                return;

            bool bSelected = e.toState == NumButton.SelectStatusEnum.Selected;

            if (bSelected)
                _constraint.SelectSet.Add(e.number);
            else
                _constraint.SelectSet.Remove(e.number);
        }

        private void SelectAll(bool bUnselect)
        {
            _constraint.SelectSet.Clear();
            if (!bUnselect)
                _constraint.SelectSet.Reset(new Region(1, 33));

            Panel_RedSelection.SetNumsSelectionState(_constraint.SelectSet, NumButton.SelectStatusEnum.Selected, true);
        }

        private void BT_SelectAllRed_Click(object sender, RoutedEventArgs e)
        {
            SelectAll(false);
        }

        private void BT_ClearAllRed_Click(object sender, RoutedEventArgs e)
        {
            SelectAll(true);
        }

        private void Ctrl_RangeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _constraint.HitLimits.Clear();

            foreach (int sel in Ctrl_RangeSelection.SelectedItems)
            {
                _constraint.HitLimits.Add(sel);
            }
        }
    }
}
