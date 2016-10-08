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
using LuckyBallsData.Statistics;
using LuckyBallSpirit.DataModel;
using LuckyBallSpirit.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class NumSelectionPanel : UserControl
    {
        private List<SchemeSelector> committedSelectors = null;
        private SchemeSelector _editingSelector = null;

        private List<Scheme> selectedSchemes = new List<Scheme>();

        private bool bInitialized = false;
        private ISelectionPanelBase activePanel = null;

        public delegate void SelectorChangedEventHandler(LotterySelectionFloatPanel.RefreshActionEnum action);
        public event SelectorChangedEventHandler SelectorChanged = null;
        
        public NumSelectionPanel()
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

        private void RefreshResultList()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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

        private void SwitchSelectMode(SchemeSelectorTypeEnum mode)
        {
            if (!bInitialized)
                return;

            activePanel = GetTargetSelectionPanel(mode);
            MakeActivePanelVisible();

            activePanel.SetSelector(SchemeSelector.CreateSelector(mode)); // create a new selector for creation.
            GetTargetRadioButton(mode).IsChecked = true;

            // display help icon and update help id.
            HelpIcon.Visibility = Visibility.Visible;
            HelpIcon.HelpID = GetHelpID(mode);
        }

        public void SwitchSelectMode(SchemeSelector selector)
        {
            if (!bInitialized)
                return;

            activePanel = GetTargetSelectionPanel(selector.GetSelectorType());
            MakeActivePanelVisible();

            activePanel.SetSelector(selector);
            GetTargetRadioButton(selector.GetSelectorType()).IsChecked = true;
        }

        private void MakeActivePanelVisible()
        {
            Panel_StandardSelection.Visibility = Panel_StandardSelection == activePanel ? Visibility.Visible : Visibility.Collapsed;
            Panel_DantuoSelection.Visibility = Panel_DantuoSelection == activePanel ? Visibility.Visible : Visibility.Collapsed;
            Panel_RandomSelection.Visibility = Panel_RandomSelection == activePanel ? Visibility.Visible : Visibility.Collapsed;
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

        private int GetHelpID(SchemeSelectorTypeEnum type)
        {
            switch (type)
            {
                case SchemeSelectorTypeEnum.StandardSelectorType:
                    return 32;
                case SchemeSelectorTypeEnum.DantuoSelectorType:
                    return 33;
                case SchemeSelectorTypeEnum.RandomSelectorType:
                    return 34;
                default:
                    return -1;
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

                    // inform that the selectors has been modified.
                    if (SelectorChanged != null)
                    {
                        SelectorChanged(LotterySelectionFloatPanel.RefreshActionEnum.RecalcualteAction);
                    }
                }

                ShowResultOrSelection(true);

                if (activePanel != null)
                {
                    GetTargetRadioButton(activePanel.Selector.GetSelectorType()).IsChecked = false;
                    activePanel = null;
                }

                // hide help icon and make help id invalid.
                HelpIcon.Visibility = Visibility.Collapsed;
                HelpIcon.HelpID = -1;
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

            ShowResultOrSelection(true);

            if (activePanel != null)
            {
                GetTargetRadioButton(activePanel.Selector.GetSelectorType()).IsChecked = false;
                activePanel = null;
            }

            // hide help icon and make help id invalid.
            HelpIcon.Visibility = Visibility.Collapsed;
            HelpIcon.HelpID = -1;
        }

        private void RB_StandardSelector_Checked(object sender, RoutedEventArgs e)
        {
            if (bInitialized && (activePanel == null || activePanel.Selector.GetSelectorType() != SchemeSelectorTypeEnum.StandardSelectorType))
            {
                SwitchSelectMode(SchemeSelectorTypeEnum.StandardSelectorType);

                ShowResultOrSelection(false);
            }
        }

        private void RB_DanTuoSelector_Checked(object sender, RoutedEventArgs e)
        {
            if (bInitialized && (activePanel == null || activePanel.Selector.GetSelectorType() != SchemeSelectorTypeEnum.DantuoSelectorType))
            {
                SwitchSelectMode(SchemeSelectorTypeEnum.DantuoSelectorType);

                ShowResultOrSelection(false);
            }
        }

        private void RB_RandomSelector_Checked(object sender, RoutedEventArgs e)
        {
            if (bInitialized && (activePanel == null || activePanel.Selector.GetSelectorType() != SchemeSelectorTypeEnum.RandomSelectorType))
            {
                SwitchSelectMode(SchemeSelectorTypeEnum.RandomSelectorType);

                ShowResultOrSelection(false);
            }
        }

        private void BT_EditSelector_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
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
                SwitchSelectMode(_editingSelector.Clone()); // edit a copy.

                ShowResultOrSelection(false);
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

                // inform that the selectors has been modified.
                if (SelectorChanged != null)
                {
                    SelectorChanged(LotterySelectionFloatPanel.RefreshActionEnum.RecalcualteAction);
                }
            }
        }        

        private void ShowResultOrSelection(bool bShowResult)
        {
            if (bShowResult)
            {
                Border_ResultPanel.Visibility = Visibility.Visible;
                FV_SelectionControls.Visibility = Visibility.Collapsed;
                BT_CommitEditing.Visibility = Visibility.Collapsed;
                BT_CancelEditing.Visibility = Visibility.Collapsed;
            }
            else
            {
                Border_ResultPanel.Visibility = Visibility.Collapsed;
                FV_SelectionControls.Visibility = Visibility.Visible;
                BT_CommitEditing.Visibility = Visibility.Visible;
                BT_CancelEditing.Visibility = Visibility.Visible;
            }
        }
    }
}
