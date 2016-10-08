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
using CN.SmartMad.Ads.WindowsPhone.WPF;

namespace Test
{
    public partial class AdTestPage : PhoneApplicationPage
    {
        public AdTestPage()
        {
            InitializeComponent();
        }

        private void PrintLog(string category, string log)
        {
            if (string.IsNullOrEmpty(log))
                return;

            System.Diagnostics.Debug.WriteLine("[{0}] : {1}", category, log);
        }

        private void ad1_AdBannerClicked(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad1_AdBannerClicked");
        }

        private void ad1_AdBannerWillExpand(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad1_AdBannerWillExpand");
        }

        private void ad1_AdBannerExpandClosed(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad1_AdBannerExpandClosed");
        }

        private void ad1_AdBannerReceived(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad1_AdBannerReceived");
        }

        private void ad1_AdBannerReceiveFailed(object sender, SMAdEventCode code)
        {
            PrintLog("BannerAd", "ad1_AdBannerReceiveFailed" + code.EventCode.ToString());
        }

        private void ad1_AdBannerPresentingScreen(object sender, SMAdEventCode code)
        {
            PrintLog("BannerAd", "ad1_AdBannerPresentingScreen" + code.EventCode.ToString());
        }

        private void ad1_AdBannerLeaveApplication(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad1_AdBannerLeaveApplication");
        }

        private void ad1_AppWillSuspendForAd(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad1_AppWillSuspendForAd");
        }

        private void ad1_AppWillResumeFromAd(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad1_AppWillResumeFromAd");
        }

        private void ad1_AdBannerDismissScreen(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad1_AdBannerDismissScreen");
        }

    }
}