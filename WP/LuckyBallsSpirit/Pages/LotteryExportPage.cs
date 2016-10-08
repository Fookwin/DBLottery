using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LuckyBallsSpirit.DataModel;
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;

namespace LuckyBallsSpirit.Pages
{
    public partial class LotteryExportPage : PhoneApplicationPage
    {
        private bool _initialized = false;
        private ObservableCollection<Scheme> _list = new ObservableCollection<Scheme>();
        public string outputFormat = "";
        private Scheme _sample = new Scheme(01, 02, 03, 04, 05, 06, 07);

        public LotteryExportPage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                // get the list of schemes.
                PurchaseInfo pendingPurchase = LBDataManager.GetInstance().PendingPurchase;
                if (pendingPurchase != null)
                {
                    pendingPurchase.GetSource().ContinueWith(res =>
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            RefreshFromData(res.Result.Selection);
                        });
                    });
                }

                OutputFormatSample.Text = _sample.ToString(outputFormat);
                _initialized = true;
            };
        }

        private void RefreshFromData(List<Scheme> data)
        {
            LV_SelectedSchemes.ItemsSource = null;

            _list.Clear();
            foreach (Scheme item in data)
            {
                _list.Add(item);
            }

            LV_SelectedSchemes.ItemsSource = _list;

            UpdateCountAndCost();
        }

        private void UpdateCountAndCost()
        {
            TB_Count.Text = _list.Count.ToString();
            TB_Cost.Text = (_list.Count * 2).ToString();
        }

        private void BT_CopyContent_Click(object sender, RoutedEventArgs e)
        {
            string stream = GetOutput();
            Clipboard.SetText(stream);

            MessageCenter.AddMessage("选号已复制到剪贴板", MessageType.Success, MessagePriority.Immediate, 3);
        }

        private string GetOutputFormatFromUI()
        {
            string red_sep = " ", blue_sep = " ";

            if (RB_Red_Comma.IsChecked != null && RB_Red_Comma.IsChecked.Value)
                red_sep = ",";
            else if (RB_Red_Dot.IsChecked != null && RB_Red_Dot.IsChecked.Value)
                red_sep = ".";
            else if (RB_Red_Strigula.IsChecked != null && RB_Red_Strigula.IsChecked.Value)
                red_sep = "-";
            else if (RB_Red_Colon.IsChecked != null && RB_Red_Colon.IsChecked.Value)
                red_sep = ":";
            else if (RB_Red_Plus.IsChecked != null && RB_Red_Plus.IsChecked.Value)
                red_sep = "+";

            if (RB_Blue_Comma.IsChecked != null && RB_Blue_Comma.IsChecked.Value)
                blue_sep = ",";
            else if (RB_Blue_Dot.IsChecked != null && RB_Blue_Dot.IsChecked.Value)
                blue_sep = ".";
            else if (RB_Blue_Strigula.IsChecked != null && RB_Blue_Strigula.IsChecked.Value)
                blue_sep = "-";
            else if (RB_Blue_Colon.IsChecked != null && RB_Blue_Colon.IsChecked.Value)
                blue_sep = ":";
            else if (RB_Blue_Plus.IsChecked != null && RB_Blue_Plus.IsChecked.Value)
                blue_sep = "+";

            return red_sep + blue_sep;
        }

        private void SetOutputFormatUI(string format)
        {
            string red_sep = " ", blue_sep = " ";

            char[] chars = format.ToCharArray();
            if (chars.Count() == 2)
            {
                red_sep = chars[0].ToString();
                blue_sep = chars[1].ToString();
            }

            if (red_sep == ",")
                RB_Red_Comma.IsChecked = true;
            else if (red_sep == ".")
                RB_Red_Dot.IsChecked = true;
            else if (red_sep == "-")
                RB_Red_Strigula.IsChecked = true;
            else if (red_sep == ":")
                RB_Red_Colon.IsChecked = true;
            else if (red_sep == "+")
                RB_Red_Plus.IsChecked = true;
            else
                RB_Red_Space.IsChecked = true;

            if (blue_sep == ",")
                RB_Blue_Comma.IsChecked = true;
            else if (blue_sep == ".")
                RB_Blue_Dot.IsChecked = true;
            else if (blue_sep == "-")
                RB_Blue_Strigula.IsChecked = true;
            else if (blue_sep == ":")
                RB_Blue_Colon.IsChecked = true;
            else if (blue_sep == "+")
                RB_Blue_Plus.IsChecked = true;
            else
                RB_Blue_Space.IsChecked = true;
        }

        private string GetOutput()
        {
            string stream = "";
            foreach (Scheme sm in _list)
            {
                stream += sm.ToString(outputFormat) + "\r\n";
            }

            return stream;
        }

        private void ExpandMoreBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BR_CustomOrderPanel.Visibility == Visibility.Collapsed)
            {
                BR_CustomOrderPanel.Visibility = Visibility.Visible;
            }
            else
            {
                BR_CustomOrderPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void RB_Output_Option_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized)
            {
                // Update the output format.
                outputFormat = GetOutputFormatFromUI();

                // update the sample string.
                OutputFormatSample.Text = _sample.ToString(outputFormat);
            }
        }

        private void BT_DeleteScheme_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Scheme selected = btn.DataContext as Scheme;
            if (selected != null)
            {
                _list.Remove(selected);

                UpdateCountAndCost();

                // inform that the selection has been modified.
                BT_Rollback.IsEnabled = true;
            }
        }

        private void BT_Rollback_Click(object sender, RoutedEventArgs e)
        {
        }        
    }
}