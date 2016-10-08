using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LuckyBallsData.Selection;

namespace LuckyBallsSpirit
{
    public partial class AttributeDetailPage : PhoneApplicationPage
    {
        private SchemeAttributes _attributes = null;
        private static Set _selected = null;

        public AttributeDetailPage()
        {
            InitializeComponent();

            // get the attribute categories.
            _attributes = DataModel.LBDataManager.GetInstance().LastAttributes;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _selected = null;
            string attributeKey = "";
            if (NavigationContext.QueryString.TryGetValue("attiName", out attributeKey))
            {
                string selectedString = null;
                if (NavigationContext.QueryString.TryGetValue("sel", out selectedString))
                {
                    _selected = new Set(selectedString);
                }

                ShowDetail(attributeKey, _selected != null);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // fill the selection
            if (_selected != null)
            {
                foreach (SchemeAttributeValueStatus value in LV_AttributeDetail.SelectedItems)
                {
                    _selected.Add(value.ValueRegion);
                }
            }

            base.OnNavigatedFrom(e);
        }

        public static Set GetSelected()
        {
            return _selected;
        }

        private void ShowDetail(string attriKey, bool selectMode)
        {
            SchemeAttribute attri = _attributes.Attribute(attriKey);
            if (attri != null)
            {
                LV_AttributeDetail.ItemsSource = attri.ValueStates;
                AttributeLable.Text = attri.DisplayName;
                pageHeaderPanel.Title = attri.DisplayName;

                if (selectMode)
                {
                    LV_AttributeDetail.SelectionMode = SelectionMode.Multiple;
                    
                    if (_selected.Count > 0)
                    {
                        foreach (SchemeAttributeValueStatus value in attri.ValueStates)
                        {
                            if (_selected.Contains(value.ValueRegion))
                            {
                                LV_AttributeDetail.SelectedItems.Add(value);
                            }
                        }
                    }
                }
            }
        }
    }
}