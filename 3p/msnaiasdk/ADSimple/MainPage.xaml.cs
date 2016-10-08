using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ADSimple.Resources;
using Windows.ApplicationModel;
using Windows.Phone.Management.Deployment;
using MSNADSDK.AD;
using System.Windows.Media;
using Microsoft.Phone.Info;
using System.Xml.Linq;

namespace ADSimple
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            resolution.TextWrapping = TextWrapping.Wrap;

            
        }

        private void ad_AdRequestSuccessEvent(object sender, MSNADSDK.AD.AdRequestStatesEventArgs args)
        {

        }

        private void ad_AdRequestErrorEvent(object sender, MSNADSDK.AD.AdRequestStatesEventArgs args)
        {

        }

        private void AdView_Loaded(object sender, RoutedEventArgs e)
        {
         
        }

        string defaultAppid = "143608";
        string defaultAdId = "188336";
        string defaultIntersitialAdId = "100004";
        string defaultSerectKey = "08a51b58fec34f3eac09e1b937903ba6";
        
        private void request(object sender, RoutedEventArgs e)
        {


            AdView adView =  new AdView();
            Button btn = sender as Button;

            adView.Appid = defaultAppid;
            adView.SecretKey = defaultSerectKey;

            if (btn.Name == "requestbtn2")
            {
                adView.SizeForAd = AdSize.Small;
                adView.Adid = defaultAdId;
            }
            else if (btn.Name == "requestbtn")
            {
                adView.SizeForAd = AdSize.Large;
                adView.Adid = defaultAdId;
            }
            else
            {
             
                adView.SizeForAd = AdSize.Large;
                adView.IsInterstitial = true;
                adView.Adid = defaultIntersitialAdId;
            }

            
            
            adView.TelCapability = true;
            
            adArea.Children.Clear();
            adArea.Children.Add(adView);

          
            adView.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            adView.LogOutput += adView_LogOutput;
            adView.AdRequestSuccessEvent += adView_AdRequestSuccessEvent;
            adView.AdRequestErrorEvent += adView_AdRequestErrorEvent;
            adView.AdSdkExceptionEvent += adView_AdSdkExceptionEvent;
            txt.Text = "广告请求中";


            if (btn.Name == "requestbtn3")
            {
                adView.GetInterstitialAd();
            }
            requestbtn.IsEnabled = false;
            requestbtn2.IsEnabled = false;
            requestbtn3.IsEnabled = false;
        }

        void adView_AdSdkExceptionEvent(object sender, ADExceptionEventArgs e)
        {
            
            txt.Text = "广告请求异常, " + e.SDKDescription;
        }

        void adView_LogOutput(object sender, EventArgs e)
        {
            LogEventArgs logArgs = e as LogEventArgs;

            txt.Text = logArgs.Log;
        }

        void adView_AdRequestErrorEvent(object sender, AdRequestStatesEventArgs args)
        {
            string msg = args.Msg;
            txt.Text = "广告请求失败, " + msg;
            
        }

        void adView_AdRequestSuccessEvent(object sender, AdRequestStatesEventArgs args)
        {
            
            txt.Text = "广告请求成功";
            
                
        }

        private void oriental(object sender, RoutedEventArgs e)
        {
            RotateTransform transform = new RotateTransform();
            transform.Angle = 90;
            RenderTransform = transform;
        
        }

     

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}