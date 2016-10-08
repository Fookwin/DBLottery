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
using InMobi.W8.AdSDK;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ImAdViewFromCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            CreateAd();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void CreateAd()
        {
            SDKUtility.LogLevel = LogLevels.IMLogLevelDebug;

            IMAdView AdView = new IMAdView();
            AdView.AdSize = IMAdView.INMOBI_AD_UNIT_320X50;

            AdView.Height = 50;
            AdView.Width = 320;
            
            //Subscribe for IMAdView events
            AdView.OnAdRequestFailed += AdView_AdRequestFailed;
            AdView.OnAdRequestLoaded += AdView_AdRequestLoaded;
            AdView.OnDismissAdScreen += AdView_DismissFullAdScreen;
            AdView.OnLeaveApplication += AdView_LeaveApplication;
            AdView.OnShowAdScreen += AdView_ShowFullAdScreen;

            //Set the Applcation Id
            AdView.AppId = "YOUR-APP-ID";
            AdView.RefreshInterval = 20;
            AdView.AnimationType = IMAdAnimationType.SLIDE_IN_LEFT;
            IMAdRequest imAdRequest = new IMAdRequest ();
            
           

            //Add the ImAdView to page
            container.Children.Add(AdView);

            AdView.LoadNewAd(imAdRequest);

        }

        void AdView_AdRequestLoaded(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Ad Loaded");
               
        }

        void AdView_AdRequestFailed(object sender, IMAdViewErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Ad Request failed Error Code:{0} Error Description {1}",
               e.ErrorCode, e.ErrorDescription));
        }

        void AdView_ShowFullAdScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Displaying full screen");
        }

        void AdView_LeaveApplication(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Moving out of application");
        }

        void AdView_DismissFullAdScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Full screen closed");
        }

       

        
    }
}
