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
using LuckyBallsSpirit.DataModel;

namespace LuckyBallsSpirit.Controls
{
    public partial class LotteryAttributePanel : UserControl
    {
        public LotteryAttributePanel()
        {
            this.InitializeComponent();
        }

        public void SetContext(List<SchemeAttributeValueStatus> context)
        {
            LV_RecommandedAttribute.ItemsSource = context;
        }

        private void LV_RecommandedAttribute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SchemeAttributeValueStatus status = LV_RecommandedAttribute.SelectedItem as SchemeAttributeValueStatus;
            if (status != null)
            {
                // switch to detail page.
                Uri target = new Uri("/Pages/AttributeDetailPage.xaml?attiName=" + status.Parent.Key, UriKind.Relative);

                PhoneApplicationFrame appFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (appFrame.CurrentSource != target)
                {
                    appFrame.Navigate(target);
                }
            }
        }
    }
}
