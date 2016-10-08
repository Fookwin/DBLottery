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

namespace AdChina.WindowsPhone.CodeSample
{
    public partial class MainPage : PhoneApplicationPage, IAdListener
    {
        private BannerAdView _adview;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_adview == null)
            {
                //创建AdView展示控件
                _adview = new BannerAdView("69327");
                _adview.Width = 480;
                _adview.Height = 72;
                _adview.Margin = new Thickness(-12, 88, 0, 0);

                //设置监听接口,this所指的类实现了IAdListener接口
                _adview.SetAdListener(this);

                //启动
                _adview.Start();
            }
        }

        public void OnReceiveAd(BannerAdView view)
        {
            Dispatcher.BeginInvoke(
                () =>
                {
                    if (_adview != null && !this.ContentPanel.Children.Contains(_adview))
                        this.ContentPanel.Children.Add(_adview);
                });
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

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {

        } 
    }
}