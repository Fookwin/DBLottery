using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows;
using LuckyBallsSpirit.DataModel;
using Microsoft.Phone.Controls;

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class FlyoutMessageBox : UserControl
    {
        private DispatcherTimer _timer;
        private int _TotalSecond;

        public void StartTimer()
        {
            if (_timer != null && _timer.IsEnabled)
                return;

            if (_TotalSecond > 0)
            {
                // Set timer.
                if (_timer == null)
                {
                    _timer = new DispatcherTimer();
                    _timer.Interval = new TimeSpan(0, 0, 1);
                    _timer.Tick += timer_Tick;
                }
                _timer.Start();
            }
        }

        private void timer_Tick(object sender, object e)
        {
            if (!TimeCountDown())
            {
                _timer.Stop();
                Hide();
            }
        }

        public bool TimeCountDown()
        {
            if (_TotalSecond <= 0)
                return false;
            else
            {
                _TotalSecond--;
                return true;
            }
        }

        public FlyoutMessageBox()
        {
            this.InitializeComponent();
        }

        public void SetMessage(string message, int duration, MessageType type)
        {
            TB_Message.Text = message;
            _TotalSecond = duration;

            // Set image
            string imageUri = "";
            switch (type)
            {
                case MessageType.Information:
                    imageUri = "Information.png";
                    break;
                case MessageType.Error:
                    imageUri = "Error.png";
                    break;
                case MessageType.Warning:
                    imageUri = "Warning.png";
                    break;
                case MessageType.Success:
                    imageUri = "Success.png";
                    break;
            }

            // Set the image source to the selected bitmap 
            BitmapImage bitmapImage = new BitmapImage(new Uri("/Assets/" + imageUri, UriKind.Relative));
            I_Icon.Source = bitmapImage;
        }

        public void Show()
        {
            PhoneApplicationFrame appFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            double ScreenW = appFrame.ActualWidth;
            double ScreenH = appFrame.ActualHeight;
            flyoutPopup.HorizontalOffset = (ScreenW - 260)/2;

            ShowPopupStory.Begin();

            StartTimer();
        }

        public void Hide()
        {
            HidePopupStory.Begin();
        }

        private void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            // Stop the timer.
            if (_timer != null && _timer.IsEnabled)
                _timer.Stop();

            // Hide the control.
            Hide();
        }

        private void mainBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            // Pause the timer.
            _timer.Stop();
        }

        private void mainBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            // Resume the timer.
            if (_TotalSecond > 0)
                _timer.Start();
        }
    }
}
