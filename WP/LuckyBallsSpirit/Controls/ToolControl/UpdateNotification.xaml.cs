using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
 

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class UpdateNotification : UserControl
    {
        public UpdateNotification()
        {
            this.InitializeComponent();
        }
        
        public void Show(string localVer, string newVer, string whatsNew, bool bBlock)
        {
            flyoutPopup.IsOpen = true;

            TB_CurrentVersion.Text = localVer;
            TB_LatestVersion.Text = newVer;
            TB_WhatNew.Text = whatsNew;                

            PhoneApplicationFrame appFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            double ScreenW = appFrame.ActualWidth;
            double ScreenH = appFrame.ActualHeight;
            flyoutPopup.HorizontalOffset = (ScreenW - 400) / 2;
            flyoutPopup.VerticalOffset = 230;

            if (bBlock)
            {
                BT_Cancel.Visibility = Visibility.Collapsed;
                TB_HaveToUpdateNotification.Visibility = Visibility.Visible;
            }
        }        

        public void Hide()
        {
            flyoutPopup.IsOpen = false;
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void BT_OK_Click(object sender, RoutedEventArgs e)
        {
            Uri path = new Uri("zune:navigate?appid=3abc7497-f756-4b21-8f18-7c6782d9718e");

            Windows.System.Launcher.LaunchUriAsync(path);
        }
    }
}
