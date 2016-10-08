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
using LuckyBallsData.Reference;
using LuckyBallSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class HelpViewCtrl : UserControl
    {
        public HelpViewCtrl()
        {
            this.InitializeComponent();
        }

        public void OnClosingEvent(object sender)
        {
            try
            {
                flyoutPopup.IsOpen = false;
                Canvas.SetLeft(flyoutPopup, Window.Current.Bounds.Width + 10);
            }
            catch 
            {
            }
        }

        public void Show(Topic topic)
        {
            double ScreenW = Window.Current.Bounds.Width;
            double ScreenH = Window.Current.Bounds.Height;
            mainBorder.Width = 400;
            mainBorder.Height = ScreenH;

            flyoutPopup.IsOpen = true;
            Canvas.SetLeft(flyoutPopup, ScreenW-400);

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.Activated += OnWindowActivated;

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                TopicTitle.Text = topic.Title;
                Description.Text = topic.Description;
                Note_List.ItemsSource = HelpCenter.Instance().GetTopicNotes(topic);
            });            
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                Hide();
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            Hide();
        }

        public void Hide()
        {
            OnClosingEvent(this);
            Window.Current.Activated -= OnWindowActivated;
            Canvas.SetLeft(flyoutPopup, Window.Current.Bounds.Width);

            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            Window.Current.Activated -= OnWindowActivated;
        }

        private void OnPopupClosed(object sender, object e)
        {
            Hide();           
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
