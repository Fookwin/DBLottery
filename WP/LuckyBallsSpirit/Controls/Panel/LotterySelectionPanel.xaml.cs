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
using LuckyBallsData.Statistics;
using LuckyBallsSpirit.DataModel;
using LuckyBallsSpirit.Controls;

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class LotterySelectionPanel : UserControl
    {
        private List<SchemeSelector> committedSelectors = null;
        private SchemeSelector _editingSelector = null;

        private List<Scheme> selectedSchemes = new List<Scheme>();

        private bool bInitialized = false;
        private ISelectionPanelBase activePanel = null;

        public delegate void BeginEditEventHandler();
        public event BeginEditEventHandler OnBeginEdit = null;

        public delegate void EndEditEventHandler(bool canceled, RefreshActionEnum action);
        public event EndEditEventHandler OnEndEdit = null;

        public LotterySelectionPanel()
        {
            this.InitializeComponent();
            this.Loaded += delegate 
            {
                bInitialized = true; 
            };
        }

        public void SetTarget(List<SchemeSelector> target)
        {
            committedSelectors = target;

            RefreshResultList();
        }

        public object GetCachedDataContext()
        {
            if (activePanel != null)
                return activePanel.Selector;

            return null;
        }

        public void SetCachedDataContext(object cachedData)
        {
            if (cachedData != null)
            {
                BeginEdit(cachedData as SchemeSelector);
            }
        }

        public void ShowNumberDescription(int infoIndex)
        {
            Panel_StandardSelection.ShowNumberDescription(infoIndex);
            Panel_DantuoSelection.ShowNumberDescription(infoIndex);
            Panel_RandomSelection.ShowNumberDescription(infoIndex);
        }

        private void RefreshResultList()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                LV_Selection.ItemsSource = null;
                LV_Selection.ItemsSource = committedSelectors;

                if (committedSelectors == null || committedSelectors.Count == 0)
                {
                    ListEmptyText.Visibility = Visibility.Visible;
                }
                else
                {
                    ListEmptyText.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void SwitchSelectMode(SchemeSelector selector)
        {
            if (!bInitialized)
                return;

            activePanel = GetTargetSelectionPanel(selector.GetSelectorType());
            activePanel.SetSelector(selector);

            GetTargetRadioButton(selector.GetSelectorType()).IsChecked = true;

            // Enable the target panel.
            switch (selector.GetSelectorType())
            {
                case SchemeSelectorTypeEnum.StandardSelectorType:
                    {
                        Panel_StandardSelection.Visibility = Visibility.Visible;
                        Panel_DantuoSelection.Visibility = Visibility.Collapsed;
                        Panel_RandomSelection.Visibility = Visibility.Collapsed;
                        break;
                    }
                case SchemeSelectorTypeEnum.DantuoSelectorType:
                    {
                        Panel_StandardSelection.Visibility = Visibility.Collapsed;
                        Panel_DantuoSelection.Visibility = Visibility.Visible;
                        Panel_RandomSelection.Visibility = Visibility.Collapsed;
                        break;
                    }
                case SchemeSelectorTypeEnum.RandomSelectorType:
                    {
                        Panel_StandardSelection.Visibility = Visibility.Collapsed;
                        Panel_DantuoSelection.Visibility = Visibility.Collapsed;
                        Panel_RandomSelection.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

        private ISelectionPanelBase GetTargetSelectionPanel(SchemeSelectorTypeEnum type)
        {
            switch (type)
            {
                case SchemeSelectorTypeEnum.StandardSelectorType:
                    return Panel_StandardSelection;
                case SchemeSelectorTypeEnum.DantuoSelectorType:
                    return Panel_DantuoSelection;  
                case SchemeSelectorTypeEnum.RandomSelectorType:
                    return Panel_RandomSelection;
                default:
                    return null;
            }
        }

        private RadioButton GetTargetRadioButton(SchemeSelectorTypeEnum type)
        {
            switch (type)
            {
                case SchemeSelectorTypeEnum.StandardSelectorType:
                    return RB_StandardSelector;
                case SchemeSelectorTypeEnum.DantuoSelectorType:
                    return RB_DanTuoSelector;
                case SchemeSelectorTypeEnum.RandomSelectorType:
                    return RB_RandomSelector;
                default:
                    return null;
            }
        }

        private void BT_CommitEditing_Click(object sender, RoutedEventArgs e)
        {
            if (activePanel != null)
            {
                // commit the current selector.
                SchemeSelector candidate = activePanel.Selector;

                // Check the availability of the selector.
                string error = candidate.HasError();
                if (error.Length > 0)
                {
                    MessageCenter.AddMessage(error, MessageType.Warning, MessagePriority.Immediate, 3);
                    return;
                }

                if (candidate.GetSchemeCount() > 0)
                {
                    foreach (SchemeSelector item in candidate.BasicSelectors)
                    {
                        committedSelectors.Add(item);
                    }

                    RefreshResultList();

                    // clean the cache.
                    _editingSelector = null;
                }

                EndEdit(false, RefreshActionEnum.RecalcualteAction);

                if (activePanel != null)
                {
                    GetTargetRadioButton(activePanel.Selector.GetSelectorType()).IsChecked = false;
                    activePanel = null;
                }              
            }
        }

        private void BT_CancelEditing_Click(object sender, RoutedEventArgs e)
        {
            // Resume the editing selector.
            if (_editingSelector != null)
            {
                // Remove the selector temporarily.
                committedSelectors.Add(_editingSelector);

                RefreshResultList();

                _editingSelector = null;
            }

            if (activePanel != null)
            {
                GetTargetRadioButton(activePanel.Selector.GetSelectorType()).IsChecked = false;
                activePanel = null;
            }

            EndEdit(true, RefreshActionEnum.DoNothing);
        }

        private void RB_StandardSelector_Checked(object sender, RoutedEventArgs e)
        {
            if (bInitialized && (activePanel == null || activePanel.Selector.GetSelectorType() != SchemeSelectorTypeEnum.StandardSelectorType))
            {
                BeginEdit(SchemeSelector.CreateSelector(SchemeSelectorTypeEnum.StandardSelectorType));
            }
        }

        private void RB_DanTuoSelector_Checked(object sender, RoutedEventArgs e)
        {
            if (bInitialized && (activePanel == null || activePanel.Selector.GetSelectorType() != SchemeSelectorTypeEnum.DantuoSelectorType))
            {
                BeginEdit(SchemeSelector.CreateSelector(SchemeSelectorTypeEnum.DantuoSelectorType));
            }
        }

        private void RB_RandomSelector_Checked(object sender, RoutedEventArgs e)
        {
            if (bInitialized && (activePanel == null || activePanel.Selector.GetSelectorType() != SchemeSelectorTypeEnum.RandomSelectorType))
            {
                BeginEdit(SchemeSelector.CreateSelector(SchemeSelectorTypeEnum.RandomSelectorType));
            }
        }

        private void BT_DeleteSelector_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            SchemeSelector selected = btn.DataContext as SchemeSelector;
            if (selected != null)
            {
                committedSelectors.Remove(selected);

                RefreshResultList();

                EndEdit(false, RefreshActionEnum.RecalcualteAction);
            }
        }

        private void BeginEdit(SchemeSelector selector)
        {
            bool bNotify = activePanel == null; // notify only for editing happen in first time.

            // switch the panel
            SwitchSelectMode(selector);

            // hide the result list panel and show the selector editng panel.
            Border_ResultPanel.Visibility = Visibility.Collapsed;
            FV_SelectionControls.Visibility = Visibility.Visible;
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
            FV_SelectionControls.Visibility = Visibility.Collapsed;
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
            SchemeSelector selected = btn.DataContext as SchemeSelector;
            if (selected != null)
            {
                // select this item.
                LV_Selection.SelectedItem = selected;

                _editingSelector = selected;

                // Remove the selector temporarily.
                committedSelectors.Remove(_editingSelector);

                RefreshResultList();

                // move to edit.
                BeginEdit(_editingSelector.Clone()); // edit a copy.
            }
        }
    }
}
