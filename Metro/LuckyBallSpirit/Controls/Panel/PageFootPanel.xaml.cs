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
    public sealed partial class PageFootPanel : UserControl
    {
        private bool _navigationEnabled = false;
        private PageIndexEnum _activeCommand = PageIndexEnum.MainPage;

        public enum PageIndexEnum
        {
            MainPage = 0,
            HistoryPage = 1,
            StatusPage = 2,
            FilterPage = 3,
            SelectionPage = 4,
            PersonalPage = 5
        }

        public PageFootPanel()
        {
            this.InitializeComponent();
        }        

        public PageIndexEnum ActiveCommand
        {
            get
            {
                return _activeCommand;
            }
            set
            {
                _navigationEnabled = false;

                _activeCommand = value;

                switch (value)
                {
                    case PageIndexEnum.MainPage:
                        RB_GoToMainPage.IsChecked = true;
                        break;
                    case PageIndexEnum.HistoryPage:
                        RB_GoToHistoryPage.IsChecked = true;
                        break;
                    case PageIndexEnum.StatusPage:
                        RB_GoToStatusPage.IsChecked = true;
                        break;
                    case PageIndexEnum.SelectionPage:
                        RB_GoToSelectionPagePage.IsChecked = true;
                        break;
                    case PageIndexEnum.PersonalPage:
                        RB_GoToPersonalPage.IsChecked = true;
                        break;
                    case PageIndexEnum.FilterPage:
                        RB_GoToFiltersPage.IsChecked = true;
                        break;
                    default:
                        return;
                }

                _navigationEnabled = true;
            }
        }

        public void LaunchTimeDown()
        {
            CL_TimeCountDownPanel.Start();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Frame currentFram = Window.Current.Content as Frame;
            if (_navigationEnabled && currentFram != null)
            {
                RadioButton btn = sender as RadioButton;
                if (btn.Name == "RB_GoToMainPage")
                {
                    currentFram.Navigate(typeof(MainPage));
                }
                else if (btn.Name == "RB_GoToHistoryPage")
                {
                    currentFram.Navigate(typeof(HistoryPage));
                }
                else if (btn.Name == "RB_GoToStatusPage")
                {
                    currentFram.Navigate(typeof(StatusPage));
                }
                else if (btn.Name == "RB_GoToSelectionPagePage")
                {
                    currentFram.Navigate(typeof(SelectionPage));
                }
                else if (btn.Name == "RB_GoToPersonalPage")
                {
                    currentFram.Navigate(typeof(PurchaseHistoryPage));
                }
                else if (btn.Name == "RB_GoToFiltersPage")
                {
                    currentFram.Navigate(typeof(AttributePage));
                }
            }
        }
    }
}
