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
using AdChina.WindowsPhone.Base;
using AdChina.WindowsPhone.Views;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;

namespace AdChina.WindowsPhone.XAMLSample
{
    public partial class MainPage : PhoneApplicationPage, IAdListener
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (adview1 != null) adview1.SetAdListener(this);
        }

        public void OnReceiveAd(BannerAdView view)
        {
            Display("OnReceiveAd");
        }

        public void OnFailedToReceiveAd(BannerAdView view)
        {
            Display("OnFailedToReceiveAd");
        }

        public void OnRefreshAd(BannerAdView view)
        {
            Display("OnRefreshAd");
        }

        public void OnFailedToRefreshAd(BannerAdView view)
        {
            Display("OnFailedToRefreshAd");
        }

        public void OnClickBanner(BannerAdView view)
        {
            Display("OnClickBanner");
        }

        public void OnClickActionButton(BannerAdView view)
        {
            Display("OnClickActionButton");
        }

        private void Display(string message)
        {
            Dispatcher.BeginInvoke(
                  () =>
                  {
                      ToastPrompt toast = new ToastPrompt(); //实例化 

                      toast.Title = "ToastPrompt"; //设置标题 
                      toast.Message = message; //设置正文消息 
                      toast.Show();
                  });
        }
    }
}