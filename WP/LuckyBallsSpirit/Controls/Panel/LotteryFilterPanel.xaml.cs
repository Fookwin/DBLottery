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
    public sealed partial class LotteryFilterPanel : UserControl
    {
        public delegate void BeginEditEventHandler();
        public event BeginEditEventHandler OnBeginEdit = null;

        public delegate void EndEditEventHandler(bool canceled, RefreshActionEnum action);
        public event EndEditEventHandler OnEndEdit = null;

        private List<Constraint> _constraints = null;
        private Constraint _editingConstraint = null;

        private FilterPanelBase _activePanel = null;
        private bool _initialized = false;

        public LotteryFilterPanel()
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

        public object GetCachedDataContext()
        {
            if (_activePanel != null)
                return _activePanel.Constraint;

            return null;
        }

        public void SetCachedDataContext(object cachedData)
        {
            if (cachedData != null)
            {
                BeginEdit(cachedData as Constraint);
            }
        }

        public void ShowNumberDescription(int infoIndex)
        {
            Ctrl_RedNumSetFilters.ShowNumberDescription(infoIndex);
        }

        public void OnNavigatedTo(NavigationContext context)
        {
            if (_activePanel != null)
                _activePanel.OnNavigatedTo(context);
        }

        private void RefreshResultList()
        {
            this.Dispatcher.BeginInvoke(() =>
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

        private void BT_CommitEditing_Click(object sender, RoutedEventArgs e)
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

            _editingConstraint = null; // clean cache.

            // inform that the filters have been changed.
            EndEdit(false, _editingConstraint == null ? RefreshActionEnum.FilterOnlyAction 
                    : RefreshActionEnum.RecalcualteAction); // rebuild only when editing.
        }

        private void BT_CancelEditing_Click(object sender, RoutedEventArgs e)
        {
            if (_editingConstraint != null)
            {
                _constraints.Add(_editingConstraint);

                RefreshResultList();

                _editingConstraint = null;
            }

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

            EndEdit(true, RefreshActionEnum.DoNothing);
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

                EndEdit(false, RefreshActionEnum.RecalcualteAction);
            }
        }

        private void SwitchFilterMode(Constraint con)
        {
            SchemeAttributeConstraint condiCon = con as SchemeAttributeConstraint;
            if (condiCon != null)
            {
                _activePanel = Ctrl_CondidtionFilters;
                Ctrl_CondidtionFilters.Visibility = Visibility.Visible;
                Ctrl_RedNumSetFilters.Visibility = Visibility.Collapsed;
                Ctrl_HistoryFilter.Visibility = Visibility.Collapsed;
                RB_ConditionFilters.IsChecked = true;
            }
            else if (con as RedNumSetConstraint != null)
            {
                _activePanel = Ctrl_RedNumSetFilters;
                Ctrl_CondidtionFilters.Visibility = Visibility.Collapsed;
                Ctrl_RedNumSetFilters.Visibility = Visibility.Visible;
                Ctrl_HistoryFilter.Visibility = Visibility.Collapsed;
                RB_RedNumSetFilters.IsChecked = true;
            }
            else
            {
                _activePanel = Ctrl_HistoryFilter;
                Ctrl_CondidtionFilters.Visibility = Visibility.Collapsed;
                Ctrl_RedNumSetFilters.Visibility = Visibility.Collapsed;
                Ctrl_HistoryFilter.Visibility = Visibility.Visible;
                RB_HistoryFilter.IsChecked = true;
            }

            _activePanel.SetConstraint(con);
        }        

        private void RB_ConditionFilters_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized && (_activePanel == null || _activePanel != Ctrl_CondidtionFilters))
            {
                BeginEdit(Constraint.CreateConstraint(ConstraintTypeEnum.SchemeAttributeConstraintType));
            }
        }
        private void RB_RedNumSetFilters_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized && (_activePanel == null || _activePanel != Ctrl_RedNumSetFilters))
            {
                BeginEdit(Constraint.CreateConstraint(ConstraintTypeEnum.RedNumSetConstraintType));
            }
        }

        private void RB_HistoryFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized && (_activePanel == null || _activePanel != Ctrl_HistoryFilter))
            {
                BeginEdit(Constraint.CreateConstraint(ConstraintTypeEnum.HistoryDuplicateConstraintType));
            }
        }

        private void BeginEdit(Constraint constraint)
        {
            bool bNotify = _activePanel == null; // notify only for editing happen in first time.

            // switch the panel
            SwitchFilterMode(constraint);

            // hide the result list panel and show the selector editng panel.
            Border_ResultPanel.Visibility = Visibility.Collapsed;
            FV_FilterControls.Visibility = Visibility.Visible;
            BottomControlPanel.Visibility = Visibility.Visible;

            // inform that the selectors has been modified.
            if (bNotify && OnBeginEdit != null)
            {
                OnBeginEdit();
            }
        }

        private void EndEdit(bool canceled, RefreshActionEnum action)
        {
            Border_ResultPanel.Visibility = Visibility.Visible;
            FV_FilterControls.Visibility = Visibility.Collapsed;
            BottomControlPanel.Visibility = Visibility.Collapsed;

            // inform that the selectors has been modified.
            if (OnEndEdit != null)
            {
                OnEndEdit(canceled, action);
            }
        }

        private void OnResultItem_Tap(object sender, GestureEventArgs e)
        {
            // don't prevent the clicking event from the delete image button.
            Image originalImage = e.OriginalSource as Image;
            if (originalImage != null)
                return;

            var btn = sender as Border;
            Constraint selected = btn.DataContext as Constraint;
            if (selected != null)
            {
                // select this item.
                LV_SelectedFilters.SelectedItem = selected;

                _editingConstraint = selected;

                _constraints.Remove(_editingConstraint);

                RefreshResultList();

                // move to edit.
                BeginEdit(_editingConstraint.Clone());

                RefreshResultList();
            }
        }
    }
}
