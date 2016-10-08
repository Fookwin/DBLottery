using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using CN.SmartMad.Ads.WindowsPhone.WPF;

namespace SlXnaAppSample
{
    public partial class GamePage : PhoneApplicationPage
    {
        ContentManager contentManager;
        GameTimer timer;
        SpriteBatch spriteBatch;

        Texture2D texture;
        Vector2 spritePosition;
        Vector2 spriteSpeed = new Vector2(100.0f, 100.0f);

        // A variety of rectangle colors
        Texture2D redTexture;
        Texture2D greenTexture;
        Texture2D blueTexture;

        // For rendering the XAML onto a texture
        UIElementRenderer elementRenderer;

        public GamePage()
        {
            InitializeComponent();

            // 从应用程序中获取内容管理器
            contentManager = (Application.Current as App).Content;

            // 为此页面创建计时器
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            SMAdManager.SetApplicationId("ApplicationId");
            SMAdManager.SetDebugMode(true); //设置为true取Debug广告，设置为false取正式广告；默认值为false
            //SMAdManager.SetAdRefreshInterval(30);

            SMAdBannerView banner = new SMAdBannerView();
            banner.AdSpaceId = "ApplicationId";
            gridAd.Children.Add(banner);
            banner.AdBannerClicked += banner_AdBannerClicked;
            banner.AdBannerDismissScreen += banner_AdBannerDismissScreen;
            banner.AdBannerExpandClosed += banner_AdBannerExpandClosed;
            banner.AdBannerLeaveApplication += banner_AdBannerLeaveApplication;
            banner.AdBannerPresentingScreen += banner_AdBannerPresentingScreen;
            banner.AdBannerReceived += banner_AdBannerReceived;
            banner.AdBannerReceiveFailed += banner_AdBannerReceiveFailed;
            banner.AdBannerWillExpand += banner_AdBannerWillExpand;
            banner.AppWillResumeFromAd += banner_AppWillResumeFromAd;
            banner.AppWillSuspendForAd += banner_AppWillSuspendForAd;


            //SMAdInterstitial interstitial = new SMAdInterstitial();
            //interstitial.AdSpaceId = "ApplicationId";
            //interstitial.RequestAd();
            //interstitial.AdInterstitialReceived += interstitial_AdInterstitialReceived;
            //interstitial.AdInterstitialClicked += interstitial_AdInterstitialClicked;
            //interstitial.AdInterstitialDismissScreen += interstitial_AdInterstitialDismissScreen;
            //interstitial.AdInterstitialPresentingScreen += interstitial_AdInterstitialPresentingScreen;
            //interstitial.AdInterstitialLeaveApplication += interstitial_AdInterstitialLeaveApplication;
            //interstitial.AdInterstitialReceiveFailed += interstitial_AdInterstitialReceiveFailed;

            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            if (ActualWidth > 0 && ActualHeight > 0 && elementRenderer == null)
            {
                elementRenderer = new UIElementRenderer(this, (int)ActualWidth, (int)ActualHeight);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 设置图形设备的共享模式以启用 XNA 呈现
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // 创建可用来绘制纹理的新 SpriteBatch。
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // TODO: 使用 this.content 在此处加载游戏内容
            if (null == texture)
            {
                redTexture = contentManager.Load<Texture2D>("redRect");
                greenTexture = contentManager.Load<Texture2D>("greenRect");
                blueTexture = contentManager.Load<Texture2D>("blueRect");

                // Start with the red rectangle
                texture = redTexture;
            }

            spritePosition.X = 0;
            spritePosition.Y = 0;


            // 启动计时器
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // 停止计时器
            timer.Stop();

            // 设置图形设备的共享模式以禁用 XNA 呈现
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 允许页面运行逻辑，如更新环境、
        /// 检查冲突、收集输入和播放音频。
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: 在此处添加更新逻辑
            spritePosition += spriteSpeed * (float)e.ElapsedTime.TotalSeconds;

            int MinX = 0;
            int MinY = 72;
            int MaxX = SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width - texture.Width;
            int MaxY = SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - texture.Height;

            // Check for bounce.
            if (spritePosition.X > MaxX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = MaxX;
            }
            else if (spritePosition.X < MinX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = MinX;
            }

            if (spritePosition.Y > MaxY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = MaxY;
            }
            else if (spritePosition.Y < MinY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = MinY;
            }
        }

        /// <summary>
        /// 允许页面绘制自身。
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: 在此处添加绘制代码

            elementRenderer.Render();
            spriteBatch.Begin();
            spriteBatch.Draw(texture, spritePosition, Color.White);
            spriteBatch.Draw(elementRenderer.Texture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        #region
        private void ColorPanelToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Visibility.Visible == ColorPanel.Visibility)
            {
                ColorPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ColorPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void redButton_Click(object sender, RoutedEventArgs e)
        {
            texture = redTexture;
        }

        private void greenButton_Click(object sender, RoutedEventArgs e)
        {
            texture = greenTexture;
        }

        private void blueButton_Click(object sender, RoutedEventArgs e)
        {
            texture = blueTexture;
        }
        #endregion


        #region "BannerEvent"
        void banner_AdBannerWillExpand(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AdBannerWillExpand");
        }

        void banner_AdBannerReceiveFailed(object sender, SMAdEventCode e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AdBannerReceiveFailed");
        }

        void banner_AdBannerReceived(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AdBannerReceived");
        }

        void banner_AdBannerPresentingScreen(object sender, SMAdEventCode e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AdBannerPresentingScreen");
        }

        void banner_AdBannerLeaveApplication(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AdBannerLeaveApplication");
        }

        void banner_AdBannerExpandClosed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AdBannerExpandClosed");
        }

        void banner_AdBannerDismissScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AdBannerDismissScreen");
        }

        void banner_AdBannerClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AdBannerClicked");
        }

        void banner_AppWillSuspendForAd(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AppWillSuspendForAd");

            // 响应接口，暂停游戏
            timer.Stop();
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);
        }

        void banner_AppWillResumeFromAd(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("------banner_AppWillResumeFromAd");

            // 响应接口，恢复游戏
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);
            timer.Start();
        }
        #endregion

        #region "InterstitialEvent"
        void interstitial_AdInterstitialReceiveFailed(object sender, SMAdEventCode e)
        {
            System.Diagnostics.Debug.WriteLine("======interstitial_AdInterstitialReceiveFailed");
        }

        void interstitial_AdInterstitialLeaveApplication(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======interstitial_AdInterstitialLeaveApplication");
        }

        void interstitial_AdInterstitialPresentingScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======interstitial_AdInterstitialPresentingScreen");

            // 响应接口，暂停游戏
            timer.Stop();
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);
        }

        void interstitial_AdInterstitialDismissScreen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======interstitial_AdInterstitialDismissScreen");

            // 响应接口，恢复游戏
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);
            timer.Start();
        }

        void interstitial_AdInterstitialClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======interstitial_AdInterstitialClicked");
        }

        void interstitial_AdInterstitialReceived(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======interstitial_AdInterstitialReceived");
            SMAdInterstitial interstitial = (SMAdInterstitial)sender;
            interstitial.Show();
        }
        #endregion
    }
}