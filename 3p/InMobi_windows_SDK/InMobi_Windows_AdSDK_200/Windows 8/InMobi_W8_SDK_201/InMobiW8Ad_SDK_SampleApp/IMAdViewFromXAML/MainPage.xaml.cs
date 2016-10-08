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
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void AdView_AdRequestLoaded(object IMAdview, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Ad loaded");
        }

      

        private void AdView_AdRequestFailed(object sender, IMAdViewErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("AdView1_AdRequestFailes_1 ErroCode:{0} ErrorDescription:{1}",
               e.ErrorCode, e.ErrorDescription));
            
        }

        private void btnLoadAd_Click_1(object sender, RoutedEventArgs e)
        {
            IMAdRequest imAdRequest = new IMAdRequest();
             SDKUtility.LogLevel = LogLevels.IMLogLevelDebug;
            AdView1.IMAdRequest = imAdRequest;
            AdView1.LoadNewAd();
        }

        private void btnViewInterstitialAd_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InterstitialAd));
        }

       

       

       
    }
}
