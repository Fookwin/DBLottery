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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class SchemeFiltersPanel : UserControl
    {
        public delegate void FilterChangedEventHandler(LotterySelectionFloatPanel.RefreshActionEnum action);
        public event FilterChangedEventHandler FilterChanged = null;

        private List<Constraint> _constraints = null;
        private Constraint _editingConstraint = null;

        private FilterPanelBase _activePanel = null;
        private bool _initialized = false;

        public SchemeFiltersPanel()
        {
            this.InitializeComponent();
            this.Loaded += delegate
            {
                _initialized = true;
            };
        }

        public void SetTarget(List<Constraint> target)
        {
            _constraints = target;

            RefreshResultList();
        }

        private void RefreshResultList()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                LV_SelectedFilters.ItemsSource = null;
                LV_SelectedFilters.ItemsSource = _constraints;

                if (_constraints == null || _constraints.Count == 0)
                {
                    ListEmptyText.Visibility = Visibility.Visible;
                }
                else
                {
                    ListEmptyText.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void BT_CommitFilter_Click(object sender, RoutedEventArgs e)
        {
            Constraint con = _activePanel.Constraint;  
          
            // Check the availability of the constraint.
            string error = con.HasError();
            if (error.Length > 0)
            {
                MessageCenter.AddMessage(error, MessageType.Warning, MessagePriority.Immediate, 3);
                return;
            }

            _constraints.Add(con);

            RefreshResultList();

            ShowResultOrSelection(true);

            if (_activePanel != null)
            {
                SchemeAttributeConstraint condiCon = _activePanel.Constraint as SchemeAttributeConstraint;
                if (condiCon != null)
                {
                    RB_ConditionFilters.IsChecked = false;
                }
                else if (_activePanel.Constraint as RedNumSetConstraint != null)
                {
                    RB_RedNumSetFilters.IsChecked = false;
                }
                else
                    RB_HistoryFilter.IsChecked = false;

                _activePanel = null;
            }           

            // inform that the filters have been changed.
            if (FilterChanged != null)
                FilterChanged(_editingConstraint == null ? LotterySelectionFloatPanel.RefreshActionEnum.FilterOnlyAction 
                    : LotterySelectionFloatPanel.RefreshActionEnum.RecalcualteAction); // rebuild only when editing.

            _editingConstraint = null; // clean cache.

            // hide help icon and make help id invalid.
            HelpIcon.Visibility = Visibility.Collapsed;
            HelpIcon.HelpID = -1;
        }

        private void BT_DismissFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_editingConstraint != null)
            {
                _constraints.Add(_editingConstraint);

                RefreshResultList();

                _editingConstraint = null;
            }

            ShowResultOrSelection(true);

            if (_activePanel != null)
            {
                SchemeAttributeConstraint condiCon = _activePanel.Constraint as SchemeAttributeConstraint;
                if (condiCon != null)
                {
                    RB_ConditionFilters.IsChecked = false;
                }
                else if (_activePanel.Constraint as RedNumSetConstraint != null)
                {
                    RB_RedNumSetFilters.IsChecked = false;
                }
                else
                    RB_HistoryFilter.IsChecked = false;
                _activePanel = null;
            }

            // hide help icon and make help id invalid.
            HelpIcon.Visibility = Visibility.Collapsed;
            HelpIcon.HelpID = -1;
        }

        private void BT_EditFilter_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Constraint selected = btn.DataContext as Constraint;
            if (selected != null)
            {
                // select this item.
                LV_SelectedFilters.SelectedItem = selected;

                _editingConstraint = selected;

                _constraints.Remove(_editingConstraint);

                RefreshResultList();

                // move to edit.
                SwitchFilterMode(_editingConstraint.Clone());

                ShowResultOrSelection(false);

                RefreshResultList();
            }
        }

        private void BT_DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Constraint selected = btn.DataContext as Constraint;
            if (selected != null)
            {
                // select this item.
                LV_SelectedFilters.SelectedItem = selected;

                _constraints.Remove(selected);

                RefreshResultList();

                // inform that the filters have been changed.
                if (FilterChanged != null)
                    FilterChanged(LotterySelectionFloatPanel.RefreshActionEnum.RecalcualteAction);
            }
        }

        private void SwitchFilterMode(Constraint con)
        {
            // display help icon and update help id.
            HelpIcon.Visibility = Visibility.Visible;            

            SchemeAttributeConstraint condiCon = con as SchemeAttributeConstraint;
            if (condiCon != null)
            {
                _activePanel = Ctrl_CondidtionFilters;
                RB_ConditionFilters.IsChecked = true;

                HelpIcon.HelpID = 35;
            }
            else if (con as RedNumSetConstraint != null)
            {
                _activePanel = Ctrl_RedNumSetFilters;
                RB_RedNumSetFilters.IsChecked = true;

                HelpIcon.HelpID = 36;
            }
            else
            {
                _activePanel = Ctrl_HistoryFilter;
                RB_HistoryFilter.IsChecked = true;

                HelpIcon.HelpID = 37;
            }

            _activePanel.SetConstraint(con);

            Ctrl_CondidtionFilters.Visibility = Ctrl_CondidtionFilters == _activePanel ? Visibility.Visible : Visibility.Collapsed;
            Ctrl_RedNumSetFilters.Visibility = Ctrl_RedNumSetFilters == _activePanel ? Visibility.Visible : Visibility.Collapsed;
            Ctrl_HistoryFilter.Visibility = Ctrl_HistoryFilter == _activePanel ? Visibility.Visible : Visibility.Collapsed;
        }        

        private void RB_ConditionFilters_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized && (_activePanel == null || _activePanel != Ctrl_CondidtionFilters))
            {
                SwitchFilterMode(Constraint.CreateConstraint(ConstraintTypeEnum.SchemeAttributeConstraintType));

                ShowResultOrSelection(false);
            }
        }
        private void RB_RedNumSetFilters_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized && (_activePanel == null || _activePanel != Ctrl_RedNumSetFilters))
            {
                SwitchFilterMode(Constraint.CreateConstraint(ConstraintTypeEnum.RedNumSetConstraintType));                

                ShowResultOrSelection(false);
            }
        }

        private void RB_HistoryFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized && (_activePanel == null || _activePanel != Ctrl_HistoryFilter))
            {
                SwitchFilterMode(Constraint.CreateConstraint(ConstraintTypeEnum.HistoryDuplicateConstraintType));

                ShowResultOrSelection(false);
            }
        }

        private void ShowResultOrSelection(bool bShowResult)
        {
            if (bShowResult)
            {
                Border_ResultPanel.Visibility = Visibility.Visible;
                FV_FilterControls.Visibility = Visibility.Collapsed;
                BT_CommitFilter.Visibility = Visibility.Collapsed;
                BT_DismissFilter.Visibility = Visibility.Collapsed;
            }
            else
            {
                Border_ResultPanel.Visibility = Visibility.Collapsed;
                FV_FilterControls.Visibility = Visibility.Visible;
                BT_CommitFilter.Visibility = Visibility.Visible;
                BT_DismissFilter.Visibility = Visibility.Visible;
            }
        }
    }
}
