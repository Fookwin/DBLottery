using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace SmartMadSDKWindowsPhoneSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnXamlBannerAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/XamlBannerAd.xaml", UriKind.Relative));
        }

        private void btnCodeBannerAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CodeBannerAd.xaml", UriKind.Relative));
        }

        private void btnInterstitialAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/InterstitialAd.xaml", UriKind.Relative));
        }
    }
}