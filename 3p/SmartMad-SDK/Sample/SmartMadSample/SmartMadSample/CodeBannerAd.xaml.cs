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

namespace SmartMadSDKTest2
{
    public partial class CodeBannerAd : PhoneApplicationPage
    {
        SMAdBannerView ad2 = null;
        public CodeBannerAd()
        {
            InitializeComponent();
            InitBannerAd();
        }

        private void PrintLog(string category, string log)
        {
            if(string.IsNullOrEmpty(log))
                return;

            System.Diagnostics.Debug.WriteLine("[{0}] : {1}", category, log);
        }

        private void InitBannerAd()
        {
            if (null == ad2)
            {
                ad2 = new SMAdBannerView("AdSpaceId");
                ad2.AdBannerAnimationType =
                    SMAdBannerAnimationType.BANNER_ANIMATION_TYPE_FADEINOUT;

                ad2.AdBannerClicked += new EventHandler(ad2_AdBannerClicked);
                ad2.AdBannerWillExpand += new EventHandler(ad2_AdBannerWillExpand);
                ad2.AdBannerExpandClosed += new EventHandler(ad2_AdBannerExpandClosed);
                ad2.AdBannerReceived += new EventHandler(ad2_AdBannerReceived);
                ad2.AdBannerReceiveFailed += 
                    new EventHandler<SMAdEventCode>(ad2_AdBannerReceiveFailed);
                ad2.AdBannerPresentingScreen += 
                    new EventHandler<SMAdEventCode>(ad2_AdBannerPresentingScreen);
                ad2.AdBannerLeaveApplication += new EventHandler(ad2_AdBannerLeaveApplication);
                ad2.AppWillSuspendForAd += new EventHandler(ad2_AppWillSuspendForAd);
                ad2.AppWillResumeFromAd += new EventHandler(ad2_AppWillResumeFromAd);
                ad2.AdBannerDismissScreen += new EventHandler(ad2_AdBannerDismissScreen);

                ad2.VerticalAlignment = VerticalAlignment.Top;
                ContentPanel.Children.Add(ad2);
            }

        }

        private void ad2_AdBannerClicked(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad2_AdBannerClicked");
        }

        private void ad2_AdBannerWillExpand(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad2_AdBannerWillExpand");
        }

        private void ad2_AdBannerExpandClosed(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad2_AdBannerExpandClosed");
        }

        private void ad2_AdBannerReceived(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad2_AdBannerReceived");
        }

        private void ad2_AdBannerReceiveFailed(object sender, SMAdEventCode code)
        {
            PrintLog("BannerAd", "ad2_AdBannerReceiveFailed" + code.EventCode.ToString());
        }

        private void ad2_AdBannerPresentingScreen(object sender, SMAdEventCode code)
        {
            PrintLog("BannerAd", "ad2_AdBannerPresentingScreen" + code.EventCode.ToString());
        }

        private void ad2_AdBannerLeaveApplication(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad2_AdBannerLeaveApplication");
        }

        private void ad2_AppWillSuspendForAd(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad2_AppWillSuspendForAd");
        }

        private void ad2_AppWillResumeFromAd(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad2_AppWillResumeFromAd");
        }

        private void ad2_AdBannerDismissScreen(object sender, EventArgs e)
        {
            PrintLog("BannerAd", "ad2_AdBannerDismissScreen");
        }
    }
}