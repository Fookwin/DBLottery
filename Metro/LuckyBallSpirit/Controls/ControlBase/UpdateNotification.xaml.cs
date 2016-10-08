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
    public sealed partial class UpdateNotification : UserControl
    {
        public UpdateNotification()
        {
            this.InitializeComponent();
        }
        
        public void Show(string localVer, string newVer, string whatsNew, bool bBlock)
        {
            TB_CurrentVersion.Text = localVer;
            TB_LatestVersion.Text = newVer;
            TB_WhatNew.Text = whatsNew;
            
            double ScreenW = Window.Current.Bounds.Width;
            double ScreenH = Window.Current.Bounds.Height;
            flyoutPopup.HorizontalOffset = (ScreenW - 500) / 2;
            flyoutPopup.VerticalOffset = (ScreenH - 500) / 2;            

            ShowPopupStory.Begin();

            if (bBlock)
            {
                BT_Cancel.Visibility = Visibility.Collapsed;
                TB_HaveToUpdateNotification.Visibility = Visibility.Visible;
            }
        }        

        public void Hide()
        {
            HidePopupStory.Begin();
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void BT_OK_Click(object sender, RoutedEventArgs e)
        {
            Uri path = new Uri(@"ms-windows-store:PDP?PFN=19278ZZXMicroCreation.5103613603842_aechqbv795pbc");

            Windows.System.Launcher.LaunchUriAsync(path);
        }
    }
}
