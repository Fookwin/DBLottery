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
using InMobi.WpSdk;

namespace ImAdViewFromXAML
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            //Set  the Loglevel to Debug, to get the maximum debug information. 
            //Before submitting live either dont use any loglevel or set it to LogLevels.IMLogLevelNone
            SDKUtility.LogLevel = LogLevels.IMLogLevelDebug;

            IMAdRequest adRequest = new IMAdRequest();
            //<Make sure the isInTestMode is set to false before going live
            adRequest.IsInTestMode = false;
            AdView.RefreshInterval = 20; 
            AdView.AnimationType = IMAdAnimationType.FLIP_FROM_LEFT;
            //Set the IMAdRequest 
            AdView.IMAdRequest = adRequest;
        }

        //Invoked when an exception is raised from IMAdView
        private void AdView_AdRequestFailed(IMAdView IMAdView, IMAdViewErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ErrorCode.ToString() + e.ErrorDescription.ToString());
        }

        //Invoked when Ad is loaded
        private void AdView_AdRequestLoaded(IMAdView IMAdView, IMAdViewSuccessEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ResponseCode.ToString() + e.ResponseDescription.ToString());
        }

        //Invoked when AD view loaded
        private void AdView_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AD view loaded");
        }

        //Invoked when the  move away from page containing IMAdView
        private void AdView_Unloaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AD view Unloaded");
        }

        //Invoked when full screen Ad displayed is closed
        private void AdView_DismissFullAdScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Full screen closed");
        }

        //Invoked when navigating out of application as Click To Action on IMAdView 
        private void AdView_LeaveApplication(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Moving out of application");
        }

        //Invoked when the full screen Ad has been opened
        private void AdView_ShowFullAdScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Displaying full screen");
        }

        //Move to page where IMAdView is configured in XAML
        private void btnxaml_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PageXMLProperty.xaml", UriKind.Relative));
        }

    }
}