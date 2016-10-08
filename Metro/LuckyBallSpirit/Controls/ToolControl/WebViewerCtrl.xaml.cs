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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class WebViewerCtrl : UserControl
    {
        public WebViewerCtrl()
        {
            this.InitializeComponent();
        }

        public void SetUri(Uri path)
        {
            WV_WebSite.Source = path;
            TB_URI.Text = path.ToString();
            PR_Loading.Visibility = Visibility.Visible;
        }

        private void WV_WebSite_LoadCompleted(object sender, NavigationEventArgs e)
        {
            PR_Loading.Visibility = Visibility.Collapsed;
            TB_URI.Text = WV_WebSite.Source.ToString();
        }

        private void WV_WebSite_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {

        }

        private void WV_WebSite_ScriptNotify(object sender, NotifyEventArgs e)
        {

        }
    }
}
