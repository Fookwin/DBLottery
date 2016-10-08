using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LuckyBallsSpirit.DataModel;
using LuckyBallsData.Statistics;
using LuckyBallsData.Selection;
using LuckyBallsSpirit.Controls;

namespace LuckyBallsSpirit.Pages
{
    public partial class LotteryAttributePage : PhoneApplicationPage
    {
        private SchemeAttributes _attributes = null;
        private ObservableCollection<AttributeGroup> _attributesInGroups = null;
        
        private bool _inialized = false;

        public LotteryAttributePage()
        {
            // get the attribute categories.
            _attributes = DataModel.LBDataManager.GetInstance().LastAttributes;

            this.InitializeComponent();
            this.Loaded += delegate
            {
                _inialized = true;

                LB_Filter_Groups.ItemsSource = AttributeGroups();
                UpdateFilteredAttributes();
            };
        }

        private void LB_Filter_Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_inialized)
            {
                if (e.AddedItems.Count > 0)
                {
                    AttributeItem attri = e.AddedItems[0] as AttributeItem;
                    if (attri != null)
                    {
                        // switch to detail page.
                        Uri target = new Uri("/Pages/AttributeDetailPage.xaml?attiName=" + attri.Key, UriKind.Relative);
                        NavigationService.Navigate(target);
                    }

                    ListBox sourceLB = sender as ListBox;
                    if (sourceLB != null)
                        sourceLB.SelectedIndex = -1;
                }
            }
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

        private List<AttributeItem> FilteredAttributes()
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

            return result;
        }

        private void UpdateFilteredAttributes()
        {
            LB_Filtered_Attribute_List.ItemsSource = FilteredAttributes();
        }

        private void Contract_Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            AttributeGroup sel = btn.DataContext as AttributeGroup;
            sel.Expanded = !sel.Expanded;
        }
    }
}