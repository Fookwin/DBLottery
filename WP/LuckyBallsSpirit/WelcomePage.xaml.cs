using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LuckyBallsSpirit
{
    public partial class WelcomePage : PhoneApplicationPage
    {
        public WelcomePage()
        {
            InitializeComponent();
            Loaded += delegate
            {
                // get screen size.
                PhoneApplicationFrame appFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                double ScreenW = appFrame.ActualWidth;
                double ScreenH = appFrame.ActualHeight;

                LogoImage.Width = ScreenW * 3 / 5;
                LogoImage.Height = ScreenW * 3 / 5;

                //LogoImage.Margin = new Thickness(0, 0, 0, ScreenH / 4);

                ShowPopupStory.Begin();
                ShowPopupStory.Completed += delegate
                {
                    System.Threading.Thread.Sleep(1000);

                    Uri target = new Uri("/MainPage.xaml", UriKind.Relative);
                    NavigationService.Navigate(target);
                    NavigationService.RemoveBackEntry();
                };
            };
        }
    }
}