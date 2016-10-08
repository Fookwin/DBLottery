using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LuckyBallsData.Util;
using LuckyBallsData.Selection;
using LuckyBallSpirit.DataModel;
using LuckyBallsData.Configration;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class ConditionFilterPanel : UserControl, FilterPanelBase
    {
        private SchemeAttributes _attributes = null;
        private List<AttributeGroup> _attributesInGroups = null;
        
        private bool _inialized = false;

        public ConditionFilterPanel()
        {
            // get the attribute categories.
            _attributes = DataModel.LBDataManager.GetInstance().LastAttributes;

            this.InitializeComponent();
            this.Loaded += delegate
            {
                // Update filter controls.
                AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;
                TB_HitPropLowLimit.Value = option.HitProbability_LowLimit;
                TB_OmissionLowLimit.Value = option.ImmediateOmission_LowLimit;
                TB_ProtentialLowLimit.Value = option.ProtentialEnergy_LowLimit;
                TB_MaxDeviationLimit.Value = option.MaxDeviation_LowLimit;

                _inialized = true;

                UpdateAttributeCategories(true);
            };
        }

        private void LB_Filter_Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_inialized)
            {
                AttributeItem attri = LB_Filter_Categories.SelectedItem as AttributeItem;
                if (attri != null)
                {
                    UpdateRegeSelection(attri, null);
                }
            }
        }

        private void UpdateRegeSelection(AttributeItem attri, Set selected)
        {
            Ctrl_RangeSelection.ItemsSource = attri.Values;
 
            Ctrl_RangeSelection.SelectedItems.Clear();

            if (selected != null)
            {
                foreach (AttributeValue value in attri.Values)
                {
                    if (selected.Contains(value.Source.ValueRegion))
                    {
                        Ctrl_RangeSelection.SelectedItems.Add(value);
                    }
                }
            }
        }

        public Constraint Constraint
        {
            get
            {
                return BuildConstraint();
            }
        }

        public void SetConstraint(Constraint _constr)
        {
            SchemeAttributeConstraint con = _constr as SchemeAttributeConstraint;
            if (con == null)
                throw new Exception("Must be a SchemeAttributeConstraint");

            LB_Filter_Categories.SelectedItem = null;
            Ctrl_RangeSelection.ItemsSource = null;

            if (con.AttributeKey.Length > 0)
            {
                // Switch to full mode.
                if (!semanticZoom.IsZoomedInViewActive)
                {
                    semanticZoom.ToggleActiveView();
                }                

                // find the attribute by key.            
                List<AttributeGroup> all = AttributeGroups(false);

                foreach (AttributeGroup group in all)
                {
                    foreach (AttributeItem attri in group.Attributes)
                    {
                        if (attri.Source.Key == con.AttributeKey)
                        {
                            if (!LB_Filter_Categories.Items.Contains(attri))
                            {
                                // We are in filtered mode, switch to full mode.
                                EnableFilterMode(false);
                            }

                            // Selecte the category.
                            LB_Filter_Categories.SelectedItem = attri;
                            LB_Filter_Categories.ScrollIntoView(attri);

                            // Select the values.
                            UpdateRegeSelection(attri, con.Values);

                            break;
                        }
                    }
                }
            }
        }

        private Constraint BuildConstraint()
        {
            AttributeItem attri = LB_Filter_Categories.SelectedItem as AttributeItem;
            if (attri != null)
            {
                Set values = new Set();

                foreach (AttributeValue value in Ctrl_RangeSelection.SelectedItems)
                {
                    values.Add(value.Source.ValueRegion);
                }

                return new SchemeAttributeConstraint(attri.Source.Key, attri.Source.DisplayName, values);
            }
            else
                return new SchemeAttributeConstraint(); // new an empty.
        }

        private List<AttributeGroup> AttributeGroups(bool rebuild)
        {
            if (_attributesInGroups != null && !rebuild)
                return _attributesInGroups;

            if (_attributesInGroups == null)
            {
                _attributesInGroups = new List<AttributeGroup>();
            }

            _attributesInGroups.Clear();

            AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;

            foreach (KeyValuePair<string, SchemeAttributeCategory> pair in _attributes.Categories)
            {
                AttributeGroup group = new AttributeGroup() { DisplayName = pair.Value.DisplayName, Attributes = new List<AttributeItem>() };

                foreach (KeyValuePair<string, SchemeAttribute> attriPair in pair.Value.Attributes)
                {
                    AttributeItem attri = new AttributeItem()
                    {
                        DisplayName = attriPair.Value.DisplayName,
                        Source = attriPair.Value,
                        Values = new List<AttributeValue>(),
                        MaxScore = 0.0
                    };

                    bool bHighlighted = false, bRecommend = false;
                    foreach (SchemeAttributeValueStatus valueState in attriPair.Value.ValueStates)
                    {
                        AttributeValue value = new AttributeValue() { DisplayName = valueState.ValueExpression, Source = valueState, Score=valueState.ProtentialEnergy.ToString("f1") };

                        if (option.Passed(valueState))
                        {
                            // higlight the value if it passed the filtering.
                            value.Highlight = true;
                            bHighlighted = true;

                            // show recommend if the energy is larger than the threshold.
                            if (valueState.ProtentialEnergy >= option.RecommendThreshold)
                                bRecommend = true;
                        }
                        else
                        {
                            value.Highlight = false;
                        }

                        if (attri.MaxScore < valueState.ProtentialEnergy)
                            attri.MaxScore = valueState.ProtentialEnergy;
                            
                        attri.Values.Add(value);
                    }

                    // recommend or not?
                    attri.Filtered = bHighlighted;
                    attri.Recommended = bRecommend;

                    group.Attributes.Add(attri);
                }
                _attributesInGroups.Add(group);
            }

            return _attributesInGroups;
        }

        private List<AttributeGroup> FilteredAttributes(bool rebuild)
        {
            List<AttributeGroup> all = AttributeGroups(rebuild);

            List<AttributeItem> result = new List<AttributeItem>();
            foreach (AttributeGroup group in all)
            {
                foreach (AttributeItem attri in group.Attributes)
                {
                    if (attri.Filtered)
                    {
                        result.Add(attri);
                    }
                }
            }

            // Create one group.
            List<AttributeGroup> groups = new List<AttributeGroup>();
            groups.Add(new AttributeGroup() { Attributes = result, DisplayName = "异常属性" });

            result.Sort();
            result.Reverse();

            return groups;
        }

        private void CB_ShowFilterOnly_Click(object sender, RoutedEventArgs e)
        {
            if (_inialized)
            {
                EnableFilterMode(CB_ShowFilterOnly.IsChecked != null && CB_ShowFilterOnly.IsChecked.Value);
            }
        }

        private void EnableFilterMode(bool bEnable)
        { 
            if (CB_ShowFilterOnly.IsChecked != bEnable)
                CB_ShowFilterOnly.IsChecked = bEnable;

            TB_HitPropLowLimit.IsEnabled = bEnable;
            TB_OmissionLowLimit.IsEnabled = bEnable;
            TB_ProtentialLowLimit.IsEnabled = bEnable;
            TB_MaxDeviationLimit.IsEnabled = bEnable;

            UpdateAttributeCategories(false);
        }

        private void TB_HitPropLowLimit_TextChanged(object sender, ValueChangeArgs e)
        {
            AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;

            if (TB_HitPropLowLimit.Value != option.HitProbability_LowLimit)
            {
                option.HitProbability_LowLimit = TB_HitPropLowLimit.Value;
                UpdateAttributeCategories(true);
            }
        }

        private void TB_OmissionLowLimit_TextChanged(object sender, ValueChangeArgs e)
        {
            AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;

            if (TB_OmissionLowLimit.Value != option.ImmediateOmission_LowLimit)
            {
                option.ImmediateOmission_LowLimit = (int)TB_OmissionLowLimit.Value;
                UpdateAttributeCategories(true);
            }
        }

        private void TB_ProtentialLowLimit_TextChanged(object sender, ValueChangeArgs e)
        {
            AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;

            if (TB_ProtentialLowLimit.Value != option.ProtentialEnergy_LowLimit)
            {
                option.ProtentialEnergy_LowLimit = TB_ProtentialLowLimit.Value;
                UpdateAttributeCategories(true);
            }
        }


        private void TB_MaxDeviationLimit_TextChanged(object sender, ValueChangeArgs e)
        {
            AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;

            if (TB_MaxDeviationLimit.Value != option.MaxDeviation_LowLimit)
            {
                option.MaxDeviation_LowLimit = TB_MaxDeviationLimit.Value;
                UpdateAttributeCategories(true);
            }
        }

        private void BT_ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (semanticZoom.IsZoomedInViewActive)
            {
                semanticZoom.ToggleActiveView();
            }
        }

        private void Contract_Button_Click(object sender, RoutedEventArgs e)
        {
            if (semanticZoom.IsZoomedInViewActive)
            {
                semanticZoom.ToggleActiveView();
            }
        }

        private void UpdateAttributeCategories(bool rebuild)
        {
            if (CB_ShowFilterOnly.IsChecked == true)
            {
                cvsAttributeCategories.Source = FilteredAttributes(rebuild);
                (semanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvsAttributeCategories.View.CollectionGroups;
            }
            else
            {
                cvsAttributeCategories.Source = null;
                cvsAttributeCategories.Source = AttributeGroups(rebuild);
                (semanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvsAttributeCategories.View.CollectionGroups;
            }
        }

        private void CB_ShowFilterOnly_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CB_ShowFilterOnly_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
