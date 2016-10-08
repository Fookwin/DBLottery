using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using LuckyBallsData.Util;
using LuckyBallsData.Selection;
using LuckyBallsSpirit.DataModel;

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class FilterConditionPanel : UserControl, FilterPanelBase
    {
        private SchemeAttributes _attributes = null;
        private ObservableCollection<AttributeGroup> _attributesInGroups = null;
        
        private bool _inialized = false;
        private AttributeItem _selectedAttir = null;
        private Set _selectedAttriVals = null;
        private AttributeItem _emptySelectedattri = new AttributeItem()
        {
            DisplayName = "请从下面列表选择一个属性",
            Description = ""
        };
        private bool _waitingForValueSelection = false;

        public FilterConditionPanel()
        {
            // get the attribute categories.
            _attributes = DataModel.LBDataManager.GetInstance().LastAttributes;

            this.InitializeComponent();
            this.Loaded += delegate
            {
                _inialized = true;

                UpdateAttributeCategories();

                UpdateSelectedItem();
            };
        }

        public void OnNavigatedTo(NavigationContext context)
        {
            if (_waitingForValueSelection && _selectedAttir != null)
            {
                _selectedAttriVals = AttributeDetailPage.GetSelected();
            }

            UpdateSelectedItem();

            _waitingForValueSelection = false;
        }

        private void UpdateSelectedItem()
        {
            if (_selectedAttir != null)
            {
                string valueExp = "";
                foreach (SchemeAttributeValueStatus value in _selectedAttir.Source.ValueStates)
                {
                    if (_selectedAttriVals.Contains(value.ValueRegion))
                    {
                        if (valueExp != "")
                            valueExp += ", ";
                        valueExp += value.ValueExpression;
                    }
                }

                SelectedFilter.DataContext = new AttributeItem()
                {
                    DisplayName = _selectedAttir.DisplayName,
                    Description = "属性值 = " + "[" + valueExp + "]"
                };
            }
            else
            {
                SelectedFilter.DataContext = _emptySelectedattri;
            }
        }

        private void LB_Filter_Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_inialized)
            {
                AttributeItem attri = e.AddedItems[0] as AttributeItem;
                if (attri != null)
                {
                    if (attri != _selectedAttir)
                    {
                        _selectedAttir = attri;
                        _selectedAttriVals = new Set();
                    }

                    ShowAttributeValueSelection(_selectedAttir, _selectedAttriVals);
                }
            }
        }

        private void ShowAttributeValueSelection(AttributeItem attri, Set selected)
        {
            // switch to detail page.
            string uriStr = "/Pages/AttributeDetailPage.xaml?attiName=" + attri.Key;
            if (selected != null)
            {
                uriStr += "&sel=" + selected.ToString();
            }
            Uri target = new Uri(uriStr, UriKind.Relative);
            App.RootFrame.Navigate(target);

            _waitingForValueSelection = true;
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

            // clear previous data.
            LB_Filter_Groups.ItemsSource = null;

            // rebuild the list.
            UpdateAttributeCategories();

            _selectedAttir = null;
            _selectedAttriVals = null;

            if (con.AttributeKey.Length > 0)
            {
                // find the attribute by key.            
                ObservableCollection<AttributeGroup> all = AttributeGroups();

                foreach (AttributeGroup group in all)
                {
                    foreach (AttributeItem attri in group.Attributes)
                    {
                        if (attri.Source.Key == con.AttributeKey)
                        {
                            _selectedAttir = attri;
                            _selectedAttriVals = con.Values;

                            break;
                        }
                    }
                }
            }

            UpdateSelectedItem();
        }

        private Constraint BuildConstraint()
        {
            if (_selectedAttir != null)
            {
                Set values = _selectedAttriVals == null ? new Set() : _selectedAttriVals;

                return new SchemeAttributeConstraint(_selectedAttir.Source.Key, _selectedAttir.Source.DisplayName, _selectedAttriVals);
            }
            else
                return new SchemeAttributeConstraint(); // new an empty.
        }

        private ObservableCollection<AttributeGroup> AttributeGroups()
        {
            if (_attributesInGroups != null)
                return _attributesInGroups;

            if (_attributesInGroups == null)
            {
                _attributesInGroups = new ObservableCollection<AttributeGroup>();
            }

            _attributesInGroups.Clear();

            FilterOption option = LBDataManager.GetInstance().FilterOption;

            foreach (KeyValuePair<string, SchemeAttributeCategory> pair in _attributes.Categories)
            {
                AttributeGroup group = new AttributeGroup() { DisplayName = pair.Value.DisplayName, Attributes = new List<AttributeItem>() };

                foreach (KeyValuePair<string, SchemeAttribute> attriPair in pair.Value.Attributes)
                {
                    AttributeItem attri = new AttributeItem()
                    {
                        Key = attriPair.Value.Key,
                        DisplayName = attriPair.Value.DisplayName, 
                        Source = attriPair.Value
                    };

                    // recommend or not?
                    SchemeAttributeValueStatus maxEngryAttriVal = null;
                    attri.Recommended = option.Recommended(attriPair.Value, out maxEngryAttriVal);
                    attri.Description = "最大偏离值[" + maxEngryAttriVal.ProtentialEnergy.ToString() + "] : " + maxEngryAttriVal.DisplayName;
                    attri.MaxEnegine = maxEngryAttriVal.ProtentialEnergy;
                    group.Attributes.Add(attri);
                }
                _attributesInGroups.Add(group);
            }

            return _attributesInGroups;
        }

        private ObservableCollection<AttributeGroup> RecommendedAttributes()
        {
            ObservableCollection<AttributeGroup> all = AttributeGroups();

            FilterOption option = LBDataManager.GetInstance().FilterOption;

            List<AttributeItem> result = new List<AttributeItem>();
            foreach (AttributeGroup group in all)
            {
                foreach (AttributeItem attri in group.Attributes)
                {
                    if (option.Passed(attri.Source))
                    {
                        result.Add(attri);
                    }
                }
            }
            result.Sort();

            // Create one group.
            ObservableCollection<AttributeGroup> groups = new ObservableCollection<AttributeGroup>();
            groups.Add(new AttributeGroup() { Attributes = result, DisplayName = "推荐属性", Expanded = true });

            return groups;
        }

        private void CB_ShowFilterOnly_Checked(object sender, RoutedEventArgs e)
        {
            if (_inialized)
            {
                UpdateAttributeCategories();
            }
        }

        private void CB_ShowFilterOnlyAll_Checked(object sender, RoutedEventArgs e)
        {
            if (_inialized)
            {
                UpdateAttributeCategories();
            }
        }

        private void UpdateAttributeCategories()
        {
            if (CB_ShowFilterOnly.IsChecked == true)
            {
                LB_Filter_Groups.ItemsSource = RecommendedAttributes();
            }
            else
            {
                LB_Filter_Groups.ItemsSource = AttributeGroups();
            }
        }

        private void Contract_Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            AttributeGroup sel = btn.DataContext as AttributeGroup;
            sel.Expanded = !sel.Expanded;
        }

        private void SelectedFilter_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_selectedAttir != null)
            {
                ShowAttributeValueSelection(_selectedAttir, _selectedAttriVals);
            }
        }
    }
}
