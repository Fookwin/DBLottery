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
using LuckyBallSpirit.Pages;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class PageHeaderPanel : UserControl
    {
        public PageHeaderPanel()
        {
            this.InitializeComponent();
        }

        // ShowBackButton property
        private static void OnShowBackButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageHeaderPanel ctrl = d as PageHeaderPanel;
            if (ctrl != null)
            {
                bool? val = e.NewValue as bool?;
                ctrl.UpdateShowBackButton(val.Value);
            }
        }

        private void UpdateShowBackButton(bool value)
        {
            backButton.IsEnabled = value;
            backButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty ShowBackButtonProperty = DependencyProperty.Register(
                        "ShowBackButton", typeof(Boolean), typeof(PageHeaderPanel),
                        new PropertyMetadata(true,
                        new PropertyChangedCallback(OnShowBackButtonPropertyChanged)));

        public bool ShowBackButton
        {
            get { return (bool)GetValue(ShowBackButtonProperty); }
            set { SetValue(ShowBackButtonProperty, value); }
        }

        // PageTitle property
        private static void OnPageTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageHeaderPanel ctrl = d as PageHeaderPanel;
            if (ctrl != null)
            {
                string val = e.NewValue as string;
                ctrl.UpdatePageTitle(val);
            }
        }

        private void UpdatePageTitle(string value)
        {
            pageTitle.Text = value;
        }

        public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register(
                        "PageTitle", typeof(string), typeof(PageHeaderPanel),
                        new PropertyMetadata(false,
                        new PropertyChangedCallback(OnPageTitlePropertyChanged)));

        public string PageTitle
        {
            get { return (string)GetValue(PageTitleProperty); }
            set { SetValue(PageTitleProperty, value); }
        }

        // SubTitle property
        private static void OnSubTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageHeaderPanel ctrl = d as PageHeaderPanel;
            if (ctrl != null)
            {
                string val = e.NewValue as string;
                ctrl.UpdateSubTitle(val);
            }
        }

        private void UpdateSubTitle(string value)
        {
            subTitle.Text = value;
        }

        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
                        "SubTitle", typeof(string), typeof(PageHeaderPanel),
                        new PropertyMetadata(false,
                        new PropertyChangedCallback(OnSubTitlePropertyChanged)));

        public string SubTitle
        {
            get { return (string)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        // ShowBackButton property
        private static void OnShowSettingButtonProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageHeaderPanel ctrl = d as PageHeaderPanel;
            if (ctrl != null)
            {
                bool? val = e.NewValue as bool?;
                ctrl.UpdateShowSettingButton(val.Value);
            }
        }

        private void UpdateShowSettingButton(bool value)
        {
            RB_GoToSettingsPage.IsEnabled = value;
            RB_GoToSettingsPage.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty ShowSettingButtonProperty = DependencyProperty.Register(
                        "ShowSettingButton", typeof(Boolean), typeof(PageHeaderPanel),
                        new PropertyMetadata(true,
                        new PropertyChangedCallback(OnShowSettingButtonProperty)));

        public bool ShowSettingButton
        {
            get { return (bool)GetValue(ShowSettingButtonProperty); }
            set { SetValue(ShowSettingButtonProperty, value); }
        }

        // Events...
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame currentFram = Window.Current.Content as Frame;
            if (currentFram != null)
                currentFram.GoBack();
        }

        private void RB_GoToSettingsPage_Click(object sender, RoutedEventArgs e)
        {
            Frame currentFram = Window.Current.Content as Frame;
            currentFram.Navigate(typeof(SettingPage));
        }
    }
}
