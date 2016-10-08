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

namespace IMAdViewFromXAML
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InterstitialAd : Page
    {

        IMAdInterstitial interstitial = new IMAdInterstitial("YOUR-APP-ID");
        IMAdRequest req = new IMAdRequest();

        public InterstitialAd()
        {
            this.InitializeComponent();
            Init();
        }


        private void Init()
        {
            interstitial.OnLeaveApplication += interstitial_LeaveApplication;
            interstitial.OnAdRequestLoaded += interstitial_AdRequestLoaded;
            interstitial.OnShowAdScreen += interstitial_ShowFullAdScreen;
            interstitial.OnAdRequestFailed += interstitial_AdRequestFailed;
            interstitial.OnDismissAdScreen += interstitial_DismissFullAdScreen;
        }

        private void interstitial_DismissFullAdScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Dismiss Full Screen");
        }

        private void interstitial_AdRequestFailed(object sender, IMAdInterstitialErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Ad request Failed {0}",e.ErrorDescription));
        }

        private void interstitial_ShowFullAdScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ShowFullScreen");
        }

        private void interstitial_AdRequestLoaded(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Intersitial Ad loaded");
            if (interstitial.State == States.READY)
                interstitial.ShowAd();
            else
                System.Diagnostics.Debug.WriteLine("Ad is not in Ready state");
        }

        private void interstitial_LeaveApplication(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Leave application");
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Frame.Navigate(typeof(InterstitialAd));
        }

        private void btnLoadInterstitialAd_Click_1(object sender, RoutedEventArgs e)
        {
            SDKUtility.LogLevel = LogLevels.IMLogLevelDebug;
             req.Age = 25;
            interstitial.LoadNewAd(req);

        }

        private void btnShowInterstitialAd_Click_1(object sender, RoutedEventArgs e)
        {
            
            interstitial.ShowAd();
        }

        private void btnGoback_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }
    }
}
