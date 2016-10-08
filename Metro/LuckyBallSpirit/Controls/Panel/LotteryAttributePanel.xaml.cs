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
using LuckyBallSpirit.DataModel;
using LuckyBallsData.Selection;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class LotteryAttributePanel : UserControl
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
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Pages.AttributePage), status);
            }
        }
    }
}
