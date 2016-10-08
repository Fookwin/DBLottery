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
using LuckyBallsData.Selection;
using LuckyBallSpirit.DataModel;
using LuckyBallsData.Configration;
using LuckyBallSpirit.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LuckyBallSpirit.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class AttributePage : LuckyBallSpirit.Common.LayoutAwarePage
    {
        private SchemeAttributes _attributes = null;
        private List<AttributeGroup> _attributesInGroups = null;
        private SchemeAttributeValueStatus _preSelectedStatus = null;

        private bool _inialized = false;

        public AttributePage()
        {
            // get the attribute categories.
            _attributes = DataModel.LBDataManager.GetInstance().LastAttributes;

            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            this.Loaded += delegate
            {
                // Initilize the foot panel.
                pageFootPanel.LaunchTimeDown();

                // show float panel.
                LotterySelectionFloatPanel.Instance().Show(LotterySelectionFloatPanel.LSDisplayModeEnum.Experss);

                // Update filter controls.
                AttributeFilterOption option = LBDataManager.GetInstance().AttributeFilterOption;
                TB_HitPropLowLimit.Value = option.HitProbability_LowLimit;
                TB_OmissionLowLimit.Value = option.ImmediateOmission_LowLimit;
                TB_ProtentialLowLimit.Value = option.ProtentialEnergy_LowLimit;
                TB_MaxDeviationLimit.Value = option.MaxDeviation_LowLimit;

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {                    
                    UpdateAttributeCategories(true);
                });

                _inialized = true;
            };
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            SchemeAttributeValueStatus status = navigationParameter as SchemeAttributeValueStatus;
            if (status != null)
            {
                _preSelectedStatus = status;
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            pageFootPanel.ActiveCommand = PageFootPanel.PageIndexEnum.FilterPage;
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
                    AttributeItem attri = new AttributeItem() { DisplayName = attriPair.Value.DisplayName, 
                        Source = attriPair.Value, Values = new List<AttributeValue>(), MaxScore = 0.0};

                    bool bHighlighted = false, bRecommend = false;
                    foreach (SchemeAttributeValueStatus valueState in attriPair.Value.ValueStates)
                    {
                        AttributeValue value = new AttributeValue() { DisplayName = valueState.ValueExpression, Source = valueState };

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

        private List<AttributeGroup> RecommendedAttributes(bool rebuild)
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

        private void LB_Filter_Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_inialized)
            {
                AttributeItem attri = LB_Filter_Categories.SelectedItem as AttributeItem;
                if (attri != null)
                {
                    HelpIcon.HelpID = attri.Source.HelpID;
                    AttributeName.Text = attri.DisplayName;
                    LV_AttributeDetail.DataContext = attri;
                    LV_AttributeDetail.ItemsSource = attri.Values;
                }
            }
        }

        private void CB_ShowFilterOnly_Click(object sender, RoutedEventArgs e)
        {
            if (_inialized)
            {
                UpdateAttributeCategories(false);
            }
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

        private void UpdateAttributeCategories(bool rebuild)
        {
            if (CB_ShowFilterOnly.IsChecked == true)
            {
                cvsAttributeCategories.Source = RecommendedAttributes(rebuild);
                (semanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvsAttributeCategories.View.CollectionGroups;
            }
            else
            {
                cvsAttributeCategories.Source = null;
                cvsAttributeCategories.Source = AttributeGroups(rebuild);
                (semanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvsAttributeCategories.View.CollectionGroups;
            }

            if (_preSelectedStatus != null)
            {
                foreach (AttributeItem item in LB_Filter_Categories.Items)
                {
                    if (item.DisplayName == _preSelectedStatus.Parent.DisplayName)
                    {
                        LB_Filter_Categories.SelectedItem = item;

                        LV_AttributeDetail.DataContext = item;
                        LV_AttributeDetail.ItemsSource = item.Values;

                        break;
                    }
                }

                _preSelectedStatus = null;
            }
        }

        private void Contract_Button_Click(object sender, RoutedEventArgs e)
        {
            if (semanticZoom.IsZoomedInViewActive)
            {
                semanticZoom.ToggleActiveView();
            }
        }
    }
}
