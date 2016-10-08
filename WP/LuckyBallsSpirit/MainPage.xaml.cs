using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using LuckyBallsSpirit.DataModel;
using LuckyBallsData.Util;
using LuckyBallsSpirit.Controls;

namespace LuckyBallsSpirit
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static bool? _firstTime = null;

        private UIElement _selectedFootItem = null;
        private LuckyBallsSpirit.Controls.Panel.NewsPanel _newsPanel = null;
        private LuckyBallsSpirit.Controls.Panel.AnalysisToolPanel _analysisPanel = null;

        // Constructor
        public MainPage()
        {
            if (_firstTime == null)
                _firstTime = true;
            else
                _firstTime = false;

            InitializeComponent();

            this.Loaded += delegate
            {
                pageHeaderPanel.OnExtendMenuClicked += delegate
                {
                    ValuePickerFlyoutCombo picker = ValuePickerFlyoutCombo.UniqueInstance;
                    picker.Show("设置", new List<string>{"建议意见", "清除本地数据"}, false, true, new List<int>());
                    picker.ValueConfirmed += OnExtendCommandClicked;
                };
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_selectedFootItem == null)
            {
                // Add content.
                if (_newsPanel == null)
                    _newsPanel = new LuckyBallsSpirit.Controls.Panel.NewsPanel();
                ScrollViewContainer.Content = _newsPanel;

                Initialize();

                PressFootItem(GoToHomePageBtn, false);
                PressFootItem(GoToAnalysisPageBtn, true);
                PressFootItem(GoToSelectionPageBtn, true);

                _selectedFootItem = GoToHomePageBtn;
                pageHeaderPanel.Title = "最新开奖";
            }
        }

        private void OnExtendCommandClicked(List<string> selected)
        {
            if (selected.Count > 0)
            {
                string selectedValue = selected[0];
                if (selectedValue == "建议意见")
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri("http://t.qq.com/fuyingluckyballs", UriKind.Absolute);
                    webBrowserTask.Show();
                }
                else
                {
                    LBDataManager.GetInstance().CleanLocalCache().ContinueWith(res =>
                    {
                        // Update progress...
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("本地数据已清除， 下次启动会将重新加载最新数据。");
                        });
                    });
                }
            }
        }

        private void Initialize()
        {
            LBDataManager mgr = LBDataManager.GetInstance();
            if (mgr.Initialized)
            {
                _newsPanel.RefreshControls(_firstTime != null && _firstTime.Value);
            }
            else
            {
                _newsPanel.RefreshControls(_firstTime != null && _firstTime.Value); // call it once to make valid controls updated.

                WaitingProgress.Visibility = Visibility.Visible; // start waiting progress.

                mgr.OnDataInitalized += DataInitializedHandler;

                mgr.Initialize();
            }
        }

        private void DataInitializedHandler(string message, int progress)
        {
            if (progress >= 0)
            {
                if (_newsPanel.RefreshControls(_firstTime != null && _firstTime.Value))
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        // Once everything completed, enable the page navigation.
                        WaitingProgress.Visibility = Visibility.Collapsed;
                        pageFootPanel.Visibility = Visibility.Visible;
                    });
                }
            }

            // Update progress...
            this.Dispatcher.BeginInvoke(() =>
            {
                ProgressMessage.Text = message;
                ProgressBar.Value = progress < 0 ? 0 : progress;

                if (progress < 0)
                    RetryButton.Visibility = Visibility.Visible;
            });
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            RetryButton.Visibility = Visibility.Collapsed;

            LBDataManager mgr = LBDataManager.GetInstance();
            mgr.Initialize();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (MessageBox.Show("确定要推出福盈双色球？", "退出", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void GoToHomePageBtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_selectedFootItem != GoToHomePageBtn)
            {
                PressFootItem(_selectedFootItem, true);
                PressFootItem(GoToHomePageBtn, false);
                _selectedFootItem = GoToHomePageBtn;

                if (_newsPanel == null)
                    _newsPanel = new LuckyBallsSpirit.Controls.Panel.NewsPanel();
                ScrollViewContainer.Content = _newsPanel;

                pageHeaderPanel.Title = "最新开奖";
            }
        }

        private void GoToAnalysisPageBtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_selectedFootItem != GoToAnalysisPageBtn)
            {
                PressFootItem(_selectedFootItem, true);
                PressFootItem(GoToAnalysisPageBtn, false);
                _selectedFootItem = GoToAnalysisPageBtn;
                
                if (_analysisPanel == null)
                    _analysisPanel =new LuckyBallsSpirit.Controls.Panel.AnalysisToolPanel();
                ScrollViewContainer.Content = _analysisPanel;

                pageHeaderPanel.Title = "数据分析";
            }
        }

        private void GoToSelectionPageBtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_selectedFootItem != GoToSelectionPageBtn)
            {
                //PressFootItem(_selectedFootItem, true);
                //PressFootItem(GoToSelectionPageBtn, false);
                //_selectedFootItem = GoToSelectionPageBtn;

                Uri target = new Uri("/Pages/LotterySelectionPage.xaml", UriKind.Relative);
                if (App.RootFrame.CurrentSource != target)
                {
                    App.RootFrame.Navigate(target);
                }
            }
        }

        private void PressFootItem(UIElement targetItem, bool unpress)
        {
            if (targetItem == GoToHomePageBtn)
            {
                Uri imgUri = new Uri(unpress ? "/Assets/icon_news_grey.png" : "/Assets/icon_news_red.png", UriKind.Relative);
                NewsIcon.Source = new System.Windows.Media.Imaging.BitmapImage(imgUri);
                NewsText.Foreground = new System.Windows.Media.SolidColorBrush(unpress ? ColorPicker.Gray : ColorPicker.DarkRed);
            }
            else if (targetItem == GoToAnalysisPageBtn)
            {
                Uri imgUri = new Uri(unpress ? "/Assets/icon_analysis_grey.png" : "/Assets/icon_analysis_red.png", UriKind.Relative);
                AnalysisIcon.Source = new System.Windows.Media.Imaging.BitmapImage(imgUri);
                AnalysisText.Foreground = new System.Windows.Media.SolidColorBrush(unpress ? ColorPicker.Gray : ColorPicker.DarkRed);
            }
            else if (targetItem == GoToSelectionPageBtn)
            {
                Uri imgUri = new Uri(unpress ? "/Assets/icon_selection_grey.png" : "/Assets/icon_selection_red.png", UriKind.Relative);
                SelectionIcon.Source = new System.Windows.Media.Imaging.BitmapImage(imgUri);
                SelectionText.Foreground = new System.Windows.Media.SolidColorBrush(unpress ? ColorPicker.Gray : ColorPicker.DarkRed);
            }
        }
    }
}