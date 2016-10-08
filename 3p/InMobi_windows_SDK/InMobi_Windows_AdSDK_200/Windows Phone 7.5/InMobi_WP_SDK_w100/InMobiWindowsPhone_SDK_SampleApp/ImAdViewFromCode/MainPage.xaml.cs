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

namespace ImAdViewFromCode
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            CreateAd();
            
        }
        //Create the Ad at runtime and add to the container
        private void CreateAd()
        {
            SDKUtility.LogLevel = LogLevels.IMLogLevelDebug;
            
            IMAdView AdView = new IMAdView();
            AdView.DisplayText = "InMobi";
            AdView.AdSize = IMAdView.INMOBI_AD_UNIT_320X50;
            
            AdView.Height = 50;
            AdView.Width = 315;
           
            //Subscribe for IMAdView events
            AdView.AdRequestFailed += new AdRequestErrorHandler(AdView_AdRequestFailed);
            AdView.AdRequestLoaded += new AdRequestSuccessHandler(AdView_AdRequestLoaded);
            AdView.DismissFullAdScreen += new EventHandler(AdView_DismissFullAdScreen);
            AdView.LeaveApplication += new EventHandler(AdView_LeaveApplication);
            AdView.ShowFullAdScreen += new EventHandler(AdView_ShowFullAdScreen);

            
            //Set the AppId. Provide you AppId
            AdView.AppId = "Your_App_Id";
            AdView.RefreshInterval = 20;
            AdView.AnimationType = IMAdAnimationType.SLIDE_IN_LEFT;
            IMAdRequest imAdRequest = new IMAdRequest();

            //Before releasing to MarketPlace, set IsInTestMode to false or don't set this property
            imAdRequest.IsInTestMode = false;
            AdView.LoadNewAd(imAdRequest);

            //Add IMAdView to Container
            stackContainer.Children.Add(AdView);
            
        }

        //Invoked when the full screen Ad has been opened
        void AdView_ShowFullAdScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Displaying full screen");
        }

        //Invoked when navigating out of application as Click To Action on IMAdView 
        void AdView_LeaveApplication(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Moving out of application");
        }

        //Invoked when full screen Ad displayed is closed
        void AdView_DismissFullAdScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Full screen closed");
        }


        //Invoked when Ad is loaded
        void AdView_AdRequestLoaded(IMAdView IMAdView, IMAdViewSuccessEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ResponseCode.ToString() + e.ResponseDescription.ToString());
        }

        //Invoked when an exception is raised from IMAdView
        void AdView_AdRequestFailed(IMAdView IMAdView, IMAdViewErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ErrorCode.ToString() + e.ErrorDescription.ToString());
        }


    }
}