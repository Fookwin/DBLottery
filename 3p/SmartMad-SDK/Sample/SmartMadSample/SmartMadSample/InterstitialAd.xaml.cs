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
    public partial class InterstitialAd : PhoneApplicationPage
    {
        SMAdInterstitial interstitialAd = null;
        public InterstitialAd()
        {
            InitializeComponent();
            InitInterstitialAd();
        }


        private void PrintLog(string category, string log)
        {
            if (string.IsNullOrEmpty(log))
                return;

            System.Diagnostics.Debug.WriteLine("[{0}] : {1}", category, log);
        }

        private void InitInterstitialAd()
        {
            interstitialAd = new SMAdInterstitial("AdSpaceId");
            interstitialAd.AdSize = SMAdInterstitialSizeType.AD_INTERSTITIAL_MEASURE_AUTO;
            interstitialAd.AdInterstitialAnimationType =
                SMAdInterstitialAnimationType.INTERSTITIAL_ANIMATION_TYPE_POPUP;
            interstitialAd.AdInterstitialClicked += 
                new EventHandler(interstitialAd_AdInterstitialClicked);
            interstitialAd.AdInterstitialDismissScreen += 
                new EventHandler(interstitialAd_AdInterstitialDismissScreen);
            interstitialAd.AdInterstitialPresentingScreen += 
                new EventHandler(interstitialAd_AdInterstitialPresentingScreen);
            interstitialAd.AdInterstitialLeaveApplication += 
                new EventHandler(interstitialAd_AdInterstitialLeaveApplication);
            interstitialAd.AdInterstitialReceived += 
                new EventHandler(interstitialAd_AdInterstitialReceived);
            interstitialAd.AdInterstitialReceiveFailed += 
                new EventHandler<SMAdEventCode>(interstitialAd_AdInterstitialReceiveFailed);
            interstitialAd.RequestAd();
        }

        private void interstitialAd_AdInterstitialClicked(object sender, EventArgs e)
        {
            PrintLog("InterstitialAd", "interstitialAd_AdInterstitialClicked");
        }

        private void interstitialAd_AdInterstitialDismissScreen(object sender, EventArgs e)
        {
            PrintLog("InterstitialAd", "interstitialAd_AdInterstitialDismissScreen");
        }

        private void interstitialAd_AdInterstitialPresentingScreen(object sender, EventArgs e)
        {
            PrintLog("InterstitialAd", "interstitialAd_AdInterstitialPresentingScreen");
        }

        private void interstitialAd_AdInterstitialLeaveApplication(object sender, EventArgs e)
        {
            PrintLog("InterstitialAd", "interstitialAd_AdInterstitialLeaveApplication");
        }

        private void interstitialAd_AdInterstitialReceived(object sender, EventArgs e)
        {
            PrintLog("InterstitialAd", "interstitialAd_AdInterstitialReceived");
            interstitialAd.Show();
        }

        private void interstitialAd_AdInterstitialReceiveFailed(object sender, SMAdEventCode errcode)
        {
            PrintLog("InterstitialAd", "interstitialAd_AdInterstitialReceiveFailed error code = " + errcode.EventCode.ToString());
        }
    }
}