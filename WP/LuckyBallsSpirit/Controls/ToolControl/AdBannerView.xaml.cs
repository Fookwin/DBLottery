using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.AdMediator.Core.Events;

namespace LuckyBallsSpirit.Controls
{
    public partial class AdBannerView : UserControl
    {

        public AdBannerView()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                AdMediatorCtrl.AdSdkEvent += new EventHandler<AdSdkEventArgs>(AdMediatorCtr_AdSdkEvent);
                AdMediatorCtrl.AdSdkError += new EventHandler<AdFailedEventArgs>(AdMediatorCtr_AdError);
                AdMediatorCtrl.AdMediatorFilled += new EventHandler<AdSdkEventArgs>(AdMediatorCtrl_AdFilled);
                AdMediatorCtrl.AdMediatorError += new EventHandler<AdMediatorFailedEventArgs>(AdMediatorCtrl_AdMediatorError);
            };
        }

        void AdMediatorCtr_AdSdkEvent(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
        {
            Debug.WriteLine("AdSdk event {0} by {1}", e.EventName, e.Name);
        }

        void AdMediatorCtrl_AdFilled(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
        {
            AdPanel.Visibility = Visibility.Visible;
        }

        void AdMediatorCtrl_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
        {
            Debug.WriteLine("AdMediatorError:" + e.Error + " " + e.ErrorCode);

        }

        void AdMediatorCtr_AdError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
        {
            Debug.WriteLine("AdSdkError by {0} ErrorCode: {1} ErrorDescription: {2} Error: {3}", e.Name, e.ErrorCode, e.ErrorDescription, e.Error);
        }
    }
}
