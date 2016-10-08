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
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;

namespace LuckyBallsServer
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                LBDataManager mgr = LBDataManager.GetDataMgr();
                mgr.AutoDetectLotteriesFromWeb();

                TB_CurrentIssue.Text = mgr.IssueInLocal.ToString();
                TB_CloudIssue.Text = mgr.IssueInCloud.ToString();
                TB_LatestIssue.Text = mgr.LatestIssue.ToString();

                LV_Updates.ItemsSource = mgr.PendingIssues;

                // update the next issue number.
                LB_Issue.Content = "第" + mgr.NextIssue + "期";

                TB_History_Version.Text = mgr.DataVersion.HistoryDataVersion.ToString();
                TB_Release_Version.Text = mgr.DataVersion.ReleaseDataVersion.ToString();
                TB_Attribute_Version.Text = mgr.DataVersion.AttributeDataVersion.ToString();
                TB_Template_Version.Text = mgr.DataVersion.AttributeTemplateVersion.ToString();
                TB_Lottery_Version.Text = mgr.DataVersion.LatestLotteryVersion.ToString();
                TB_Matrix_Version.Text = mgr.DataVersion.MatrixDataVersion.ToString();
                TB_Help_Version.Text = mgr.DataVersion.HelpContentVersion.ToString();
            };
        }

        private void Launch_History_Viewer(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/HistoryPage.xaml", UriKind.Relative));
        }

        private void Launch_Num_Selector(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/AttributePage.xaml", UriKind.Relative));
        }

        private void Launch_MaxtixPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/MatrixPage.xaml", UriKind.Relative));
        }

        private void Launch_BlueAnaylsisPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/BlueAnalysisPage.xaml", UriKind.Relative));
        }

        private void Launch_UserManagePage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/UserManagementPage.xaml", UriKind.Relative));
        }

        private void Launch_SchemeAnalysisPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/SchemeAnalysisPage.xaml", UriKind.Relative));
        }

        private void Button_Click_SaveToLocal(object sender, RoutedEventArgs e)
        {
            // Update data version.
            LBDataManager.GetDataMgr().DataVersion.HistoryDataVersion = Convert.ToInt32(TB_History_Version.Text);
            LBDataManager.GetDataMgr().DataVersion.ReleaseDataVersion = Convert.ToInt32(TB_Release_Version.Text);
            LBDataManager.GetDataMgr().DataVersion.AttributeDataVersion = Convert.ToInt32(TB_Attribute_Version.Text);
            LBDataManager.GetDataMgr().DataVersion.AttributeTemplateVersion = Convert.ToInt32(TB_Template_Version.Text);
            LBDataManager.GetDataMgr().DataVersion.LatestLotteryVersion = Convert.ToInt32(TB_Lottery_Version.Text);
            LBDataManager.GetDataMgr().DataVersion.MatrixDataVersion = Convert.ToInt32(TB_Matrix_Version.Text);
            LBDataManager.GetDataMgr().DataVersion.HelpContentVersion = Convert.ToInt32(TB_Help_Version.Text);

            LBDataManager.GetDataMgr().SaveToLocal();
            MessageBox.Show("Done");
        }

        private void Button_Click_SaveToCloud(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/UploadToCloudPage.xaml", UriKind.Relative));
        }

        private void LV_Updates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UncommittedItem item = LV_Updates.SelectedItem as UncommittedItem;
            if (item != null)
            {
                TB_Issue.Text = item.Issue.ToString();
                TB_Date.Text = item.Lottery.Date.ToString("yyyy-MM-dd");
                TB_Result.Text = item.Lottery.Scheme.DisplayExpression;
                TB_Bet.Text = item.Lottery.BetAmount.ToString();
                TB_Pool.Text = item.Lottery.PoolAmount.ToString();

                // Bonus...
                TB_BounsCount_1.Text = item.Lottery.BonusAmount(1).ToString();
                TB_Bouns_1.Text = item.Lottery.BonusMoney(1).ToString();

                TB_BounsCount_2.Text = item.Lottery.BonusAmount(2).ToString();
                TB_Bouns_2.Text = item.Lottery.BonusMoney(2).ToString();

                TB_BounsCount_3.Text = item.Lottery.BonusAmount(3).ToString();
                TB_BounsCount_4.Text = item.Lottery.BonusAmount(4).ToString();
                TB_BounsCount_5.Text = item.Lottery.BonusAmount(5).ToString();
                TB_BounsCount_6.Text = item.Lottery.BonusAmount(6).ToString();

                string comments = item.Lottery.MoreInfo;

                int ind0 = comments.IndexOf("一等奖中奖地");
                int ind2 = comments.IndexOf("出球顺序");
                if (ind0 >= 0 && ind2 > ind0)
                {
                    TB_Provinces.Text = comments.Substring(ind0 + 7, ind2 - ind0 - 7).Trim();
                    TB_Order.Text = comments.Substring(ind2 + 5).Trim();
                }
                else
                {
                    TB_Provinces.Text = comments;
                    TB_Order.Text = "";
                }        


                // update recommendation.
                LuckyBallsData.ReleaseInfo release = item.ReleaseInfo;
                if (release != null)
                {
                    for (int i = 1; i <= 33; ++i)
                    {
                        string ctrlName = "Red_" + i.ToString().PadLeft(2, '0');
                        CheckBox check = this.FindName(ctrlName) as CheckBox;

                        if (release.IncludedReds.Contains(i))
                            check.IsChecked = true;
                        else if (release.ExcludedReds.Contains(i))
                            check.IsChecked = false;
                        else
                            check.IsChecked = null;
                    }

                    for (int i = 1; i <= 16; ++i)
                    {
                        string ctrlName = "Blue_" + i.ToString().PadLeft(2, '0');
                        CheckBox check = this.FindName(ctrlName) as CheckBox;

                        if (release.IncludedBlues.Contains(i))
                            check.IsChecked = true;
                        else if (release.ExcludedBlues.Contains(i))
                            check.IsChecked = false;
                        else
                            check.IsChecked = null;
                    }
                }

                TB_NextIssue.Text = release.NextIssue.ToString();
                TB_NextRelease.Text = release.NextReleaseTime.ToString("yyyy-MM-dd");

                GR_ItemPanel.Visibility = Visibility.Visible;
            }
            else
            {
                GR_ItemPanel.Visibility = Visibility.Hidden;
            }
        }

        private void BT_AddNew_Click(object sender, RoutedEventArgs e)
        {
            // check the input result.
            Scheme result = new Scheme(TB_NextResult.Text);
            if (!result.IsValid())
            {
                MessageBox.Show("Invalid input!");
                return;
            } 

            LBDataManager mgr = LBDataManager.GetDataMgr();

            // Refresh the list with autoselecting the new item.
            UncommittedItem newItem = mgr.AddNextLottery(result);
            LV_Updates.ItemsSource = null;
            LV_Updates.ItemsSource = mgr.PendingIssues;
            LV_Updates.SelectedItem = newItem;

            // update the next issue number.
            LB_Issue.Content = "第" + mgr.NextIssue + "期";
        }

        private void BT_SaveChange_Click(object sender, RoutedEventArgs e)
        {
            // Prepare the data.
            try
            {
                UncommittedItem item = LV_Updates.SelectedItem as UncommittedItem;

                DateTime time = DateTime.Parse(TB_Date.Text);
                int pool = Convert.ToInt32(TB_Pool.Text);
                int bet = Convert.ToInt32(TB_Bet.Text);
                int nextIssue = Convert.ToInt32(TB_NextIssue.Text);
                DateTime nextDate = DateTime.Parse(TB_NextRelease.Text);

                string provience = TB_Provinces.Text;
                string order = TB_Order.Text;
                string comments = "一等奖中奖地:" + provience + " 出球顺序:" + order;

                int[,] bonus = new int[6, 2];
                bonus[0, 0] = Convert.ToInt32(TB_BounsCount_1.Text);
                bonus[0, 1] = Convert.ToInt32(TB_Bouns_1.Text);

                bonus[1, 0] = Convert.ToInt32(TB_BounsCount_2.Text);
                bonus[1, 1] = Convert.ToInt32(TB_Bouns_2.Text);

                bonus[2, 0] = Convert.ToInt32(TB_BounsCount_3.Text);
                bonus[2, 1] = 3000;

                bonus[3, 0] = Convert.ToInt32(TB_BounsCount_4.Text);
                bonus[3, 1] = 200;

                bonus[4, 0] = Convert.ToInt32(TB_BounsCount_5.Text);
                bonus[4, 1] = 10;

                bonus[5, 0] = Convert.ToInt32(TB_BounsCount_6.Text);
                bonus[5, 1] = 5;

                for (int i = 1; i <= 33; ++i)
                {
                    string ctrlName = "Red_" + i.ToString().PadLeft(2, '0');
                    CheckBox check = this.FindName(ctrlName) as CheckBox;

                    if (check.IsChecked == null)
                    {
                        item.ReleaseInfo.IncludedReds.Remove(i);
                        item.ReleaseInfo.ExcludedReds.Remove(i);
                    }
                    else if (check.IsChecked == true)
                    {
                        item.ReleaseInfo.IncludedReds.Add(i);
                        item.ReleaseInfo.ExcludedReds.Remove(i);
                    }
                    else
                    {
                        item.ReleaseInfo.ExcludedReds.Add(i);
                        item.ReleaseInfo.IncludedReds.Remove(i);
                    }
                }

                for (int i = 1; i <= 16; ++i)
                {
                    string ctrlName = "Blue_" + i.ToString().PadLeft(2, '0');
                    CheckBox check = this.FindName(ctrlName) as CheckBox;

                    if (check.IsChecked == null)
                    {
                        item.ReleaseInfo.IncludedBlues.Remove(i);
                        item.ReleaseInfo.ExcludedBlues.Remove(i);
                    }
                    else if (check.IsChecked == true)
                    {
                        item.ReleaseInfo.IncludedBlues.Add(i);
                        item.ReleaseInfo.ExcludedBlues.Remove(i);
                    }
                    else
                    {
                        item.ReleaseInfo.ExcludedBlues.Add(i);
                        item.ReleaseInfo.IncludedBlues.Remove(i);
                    }
                }

                item.Lottery.SetDetail(time, bonus, bet, pool, comments);

                item.ReleaseInfo.NextIssue = nextIssue;
                item.ReleaseInfo.NextReleaseTime = nextDate.AddHours(21.25);
                item.ReleaseInfo.SellOffTime = nextDate.AddHours(19.75);                

                item.Dirty = true;

                MessageBox.Show("Done");
            }
            catch
            {
                MessageBox.Show("Invalid Inputs!");
            }
        }

        private void BN_RadomReds_Click(object sender, RoutedEventArgs e)
        {
            // Reset all.
            for (int i = 1; i <= 33; ++i)
            {
                string ctrlName = "Red_" + i.ToString().PadLeft(2, '0');
                CheckBox check = this.FindName(ctrlName) as CheckBox;
                check.IsChecked = null;
            }

            List<int> reds = new List<int>();

            int validCount = 0;
            while (validCount < 8)
            {
                Random rnd = new Random((int)DateTime.UtcNow.Ticks);
                int red = rnd.Next(1, 33);
                if (!reds.Contains(red))
                {
                    reds.Add(red);
                    validCount++;

                    string ctrlName = "Red_" + red.ToString().PadLeft(2, '0');
                    CheckBox check = this.FindName(ctrlName) as CheckBox;

                    if (validCount <= 2)
                    {
                        // select it.
                        check.IsChecked = true;
                    }
                    else
                    {
                        // unselect it.
                        check.IsChecked = false;
                    }
                }
            }
        }

        private void BN_RadomBlues_Click(object sender, RoutedEventArgs e)
        {
            // Reset all.
            for (int i = 1; i <= 16; ++i)
            {
                string ctrlName = "Blue_" + i.ToString().PadLeft(2, '0');
                CheckBox check = this.FindName(ctrlName) as CheckBox;
                check.IsChecked = null;
            }

            List<int> reds = new List<int>();

            int validCount = 0;
            while (validCount < 4)
            {
                Random rnd = new Random((int)DateTime.UtcNow.Ticks);
                int red = rnd.Next(1, 16);
                if (!reds.Contains(red))
                {
                    reds.Add(red);
                    validCount++;

                    string ctrlName = "Blue_" + red.ToString().PadLeft(2, '0');
                    CheckBox check = this.FindName(ctrlName) as CheckBox;

                    if (validCount <= 1)
                    {
                        // select it.
                        check.IsChecked = true;
                    }
                    else
                    {
                        // unselect it.
                        check.IsChecked = false;
                    }
                }
            }  
        }

        private void BT_SyncToWeb_Click(object sender, RoutedEventArgs e)
        {
            UncommittedItem item = LV_Updates.SelectedItem as UncommittedItem;
            if (item != null)
            {
                DateTime date = DateTime.Now;
                int[] reds = new int[6];
                int blue = 0;
                int[,] bonus = new int[6, 2];
                int betAmount = 0;
                int poolAmount = 0;
                string more = "None";

                if (DataUtil.SyncFromWeb(item.Issue, ref date, ref reds, ref blue, ref bonus, ref betAmount, ref poolAmount, ref more))
                {
                    TB_Bet.Text = betAmount.ToString();
                    TB_Pool.Text = poolAmount.ToString();

                    // Bonus...
                    TB_BounsCount_1.Text = bonus[0, 0].ToString();
                    TB_Bouns_1.Text = bonus[0, 1].ToString();

                    TB_BounsCount_2.Text = bonus[1, 0].ToString();
                    TB_Bouns_2.Text = bonus[1, 1].ToString();

                    TB_BounsCount_3.Text = bonus[2, 0].ToString();
                    TB_BounsCount_4.Text = bonus[3, 0].ToString();
                    TB_BounsCount_5.Text = bonus[4, 0].ToString();
                    TB_BounsCount_6.Text = bonus[5, 0].ToString();

                    string comments = more;

                    int ind0 = comments.IndexOf("一等奖中奖地");
                    int ind2 = comments.IndexOf("出球顺序");
                    if (ind0 >= 0 && ind2 > ind0)
                    {
                        string winDetails = comments.Substring(ind0 + 7, ind2 - ind0 - 7);
                        winDetails = winDetails.Replace("注", "注 ");
                        winDetails.Trim(new char[] {' ', '。'});

                        TB_Provinces.Text = winDetails;

                        string orderDetails = comments.Substring(ind2 + 5).Trim(new char[] { ' ', '。'});
 
                        int indexOfPlus = orderDetails.IndexOf('+');
                        if (indexOfPlus >= 12)
                        {
                            string originalString = orderDetails.Substring(indexOfPlus - 12, 15);
                            if (!originalString.Contains(' '))
                            {
                                orderDetails = "";
                                for (int i = 1; i < 12; ++i)
                                {
                                    if (i % 2 == 0)
                                    {
                                        orderDetails += originalString.Substring(i - 2, 2) + " ";
                                    }
                                }

                                orderDetails += originalString.Substring(10);
                            }
                        }  
                        
                        TB_Order.Text = orderDetails;
                    }
                    else
                    {
                        TB_Provinces.Text = comments;
                        TB_Order.Text = "";
                    }
                }
            }
        }

        private void Launch_Help_Builder(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Pages/HelpContentPage.xaml", UriKind.Relative));
        }
    }
}
