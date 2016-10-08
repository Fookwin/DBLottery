using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LuckyBallsSpirit.Controls.Panel
{
    public partial class AnalysisToolPanel : UserControl
    {
        public AnalysisToolPanel()
        {
            InitializeComponent();
        }

        private void GoToHistoryPage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Uri target = new Uri("/Pages/LotteryHistoryPage.xaml", UriKind.Relative);
            if (App.RootFrame.CurrentSource != target)
            {
                App.RootFrame.Navigate(target);
            }
        }

        private void GoToDiagramPage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Uri target = new Uri("/Pages/LotteryStatusPage.xaml", UriKind.Relative);
            if (App.RootFrame.CurrentSource != target)
            {
                App.RootFrame.Navigate(target);
            }
        }

        private void GoToAttribtuePage_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Uri target = new Uri("/Pages/LotteryAttributePage.xaml", UriKind.Relative);
            if (App.RootFrame.CurrentSource != target)
            {
                App.RootFrame.Navigate(target);
            }
        }
    }
}
