using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LuckyBallsData.Util;
using LuckyBallsData.Management;
using System.Windows.Threading;
using System.Threading;

namespace LuckyBallsServer.Pages
{
    /// <summary>
    /// Interaction logic for UserManagementPage.xaml
    /// </summary>
    public partial class UserManagementPage : Page
    {
        public UserManagementPage()
        {
            InitializeComponent();
        }

        private void UpdateDataList()
        {
            int platform = PlatformCombo.SelectedIndex + 1;

            DateTime current = DateTime.UtcNow;

            int activeUserIn7days = 0;
            int activeUserIn1month = 0;
            var users = ServerUtil.GetUsers(platform);

            foreach (User item in users)
            {
                int days = (current - item.LastLoginDate).Days;
                if (days < 30)
                {
                    activeUserIn1month++;
                    if (days < 7)
                    {
                        activeUserIn7days++;
                    }
                }
            }

            string summary = "[ " + PlatformCombo.Text + " ]: 共 " + users.Count.ToString() + " 注册用户, 其中 ";
            summary += activeUserIn7days.ToString() + " 用户近一周内登陆过，";
            summary += activeUserIn1month.ToString() + " 用户近一月内登陆过";

            UserSummary.Text = summary;

            DisplayGrid.ItemsSource = null;
            DisplayGrid.ItemsSource = users;
        }

        private void Button_Click_PushWNS(object sender, RoutedEventArgs e)
        {
            PushWNSButton.IsEnabled = false;

            int platform = PlatformCombo.SelectedIndex + 1;
            string title = NotificationTitle.Text;
            string content = NotificationContent.Text;

            string message = LBDataManager.GetDataMgr().FormatNotification(platform, title, content);
            if (message == "")
            {
                MessageBox.Show("Input message is invalid.");
            }
            else 
            {
                ServerUtil.PushNotification(platform, message);
                MessageBox.Show("Pushed!");
            }

            PushWNSButton.IsEnabled = true;            
        }
        
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            if (PlatformCombo.SelectedIndex >= 0 && PlatformCombo.SelectedIndex < 3)
            {
                PushWNSButton.IsEnabled = true;
                DisplayDetailsButton.IsEnabled = true;
            }
            else
            {
                PushWNSButton.IsEnabled = false;
                DisplayDetailsButton.IsEnabled = false;
            }
        }

        private void TemplateCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                UpdateNotificationMessage((NotificationTemplateEnum)TemplateCombo.SelectedIndex);
            }
        }

        private void UpdateNotificationMessage(NotificationTemplateEnum template)
        {
            string title = "";
            List<string> content = new List<string>();
            LBDataManager.GetDataMgr().GetNotificationFromTemplate(template, ref title, ref content);

            // update ui.
            NotificationTitle.Text = title;
            NotificationContent.ItemsSource = content;
            NotificationContent.SelectedIndex = 0;
        }

        private void Handle_Login_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LBDataManager.GetDataMgr().ProcessLoginMessages();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            MessageBox.Show("Done");
        }

        private void DisplayDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayDetailsButton.IsEnabled = false;

            UpdateDataList();
        }
    }
}
