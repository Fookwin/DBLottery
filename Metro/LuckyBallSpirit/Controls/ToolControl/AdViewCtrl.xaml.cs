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
//using LeadBolt.Windows8.AppAd;
//using LeadBolt.Windows8.AppAd.Listener;
//using InMobi.W8.AdSDK;
//using JiuYouWin8Ad;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class AdViewCtrl : UserControl
    {
        //private AdController lbAdController;
        //private AdListener lbAdListener;
        private static bool _bUserStopped = false;

        public AdViewCtrl()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                if (!_bUserStopped)
                {
                    //lbAdListener = new AdListener();
                    //lbAdListener.AdFailed += delegate
                    //{
                    //    AdCtrl.Visibility = Visibility.Collapsed;
                    //};

                    //lbAdListener.AdLoaded += delegate
                    //{
                    //    AdCtrl.Visibility = Visibility.Visible;
                    //};

                    //lbAdController = new AdController(AdCtrl, "690042240", lbAdListener);
                    //lbAdController.LoadAd();
                }                
            }; 
        }

        private void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!_bUserStopped)
            {
                //lbAdController.DestroyAd();

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    AdCtrl.Visibility = Visibility.Collapsed;
                });

                _bUserStopped = true;
            }
        }
    }
}
