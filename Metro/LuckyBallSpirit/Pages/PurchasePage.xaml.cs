using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LuckyBallSpirit.DataModel;
using LuckyBallsData.Selection;
using LuckyBallsData.Statistics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Net;
using Windows.Storage.Streams;
using LuckyBallsData.Util;
using LuckyBallSpirit.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LuckyBallSpirit.Pages
{
    class SchemeItem
    {
        private Scheme _scheme = null;
        public static string sOutputFormat = "";

        public SchemeItem(Scheme native)
        {
            _scheme = native;
        }

        public override string ToString()
        {
 	         return _scheme.ToString(sOutputFormat);
        }
    }

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PurchasePage : LuckyBallSpirit.Common.LayoutAwarePage
    {
        private List<SchemeItem> _list = new List<SchemeItem>();
        private List<LotteryWebSite> _webSites = null;
        private LotteryWebSite _editingSite = null;

        public List<LotteryWebSite> WebSites
        {
            get
            {
                return _webSites;
            }
        }

        public PurchasePage()
        {
            this.InitializeComponent();
            this.Loaded += delegate
            {
                // Initilize the foot panel.
                pageFootPanel.LaunchTimeDown();

                // show float panel.
                LotterySelectionFloatPanel.Instance().Show(LotterySelectionFloatPanel.LSDisplayModeEnum.Experss);

                // Build the web sites...
                LBDataManager.GetInstance().PurchaseWebSites().ContinueWith( res =>
                {
                    _webSites = res.Result;
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        LV_WebSites.ItemsSource = _webSites;
                    });
                });                

                // Hide web viewer and customize panel.
                BR_WebViewer.Visibility = Visibility.Collapsed;
                BR_CustomOrderPanel.Visibility = Visibility.Collapsed;
            };
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            List<Scheme> list = navigationParameter as List<Scheme>;
            if (list != null)
            {
                _list.Clear();

                foreach (Scheme item in list)
                {
                    _list.Add(new SchemeItem(item));
                }

                LV_SelectedSchemes.ItemsSource = _list;
                TB_Count.Text = _list.Count.ToString();
                TB_Cost.Text = (_list.Count * 2).ToString();
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            pageFootPanel.ActiveCommand = PageFootPanel.PageIndexEnum.SelectionPage;
        }

        private void BT_Export_Click(object sender, RoutedEventArgs e)
        {
            // Save the scheme list as a text file for furture purchase.
            SaveToFile().ContinueWith(res =>
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DataPackage data = new DataPackage();
                    data.SetText(res.Result);
                    Clipboard.SetContent(data);

                    string msg = "导出到 " + res.Result + " 成功！文件路径已复制到剪贴板。";
                    MessageCenter.AddMessage(msg, MessageType.Success, MessagePriority.Immediate, 3);
                });
            });   
        }

        private void BT_CopyContent_Click(object sender, RoutedEventArgs e)
        {
            string stream = GetOutput();

            DataPackage data = new DataPackage();
            data.SetText(stream);

            Clipboard.SetContent(data);

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
            foreach (SchemeItem sm in _list)
            {
                stream += sm.ToString() + "\r\n";
            }

            return stream;
        }

        private async Task<string> SaveToFile()
        { 
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary; 
            savePicker.FileTypeChoices.Add("Text", new List<string>() { ".txt" });

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                string stream = GetOutput();                
                await FileIO.WriteTextAsync(file, stream);

                return file.Path;
            }
            else
                throw new Exception("Can't save to the target file.");
        }

        private void LV_WebSites_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectWebSite(e.ClickedItem as LotteryWebSite);
        }

        private void SelectWebSite(LotteryWebSite site)
        {
            if (site != null)
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Make it selected.
                    LV_WebSites.SelectedItem = site;

                    if (site.Uri != null)
                    {
                        // Show web panel.
                        BR_CustomOrderPanel.Visibility = Visibility.Collapsed;
                        BR_WebViewer.Visibility = Visibility.Visible;

                        // Load the panel...
                        WV_WebViewerCtl.SetUri(site.Uri);
                    }
                    else
                    {
                        BR_WebViewer.Visibility = Visibility.Collapsed;
                    }

                    // Set the format.
                    SchemeItem.sOutputFormat = site.OutputFormat;

                    // Update scheme list.
                    LV_SelectedSchemes.ItemsSource = null;
                    LV_SelectedSchemes.ItemsSource = _list;
                });
            }
        }

        private void BT_Edit_Site_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LotteryWebSite selected = btn.DataContext as LotteryWebSite;
            if (selected != null)
            {
                // select this item.
                LV_WebSites.SelectedItem = selected;

                EditWebSite(selected);
            }
        }

        private void BT_Delete_Site_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            LotteryWebSite selected = btn.DataContext as LotteryWebSite;

            if (selected == _editingSite)
            {
                // if we are editing this web site, stop the editing.
                _editingSite = null;

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    BR_CustomOrderPanel.Visibility = Visibility.Collapsed; // hide the edit panel.
                });
            }

            // Delete the site.
            _webSites.Remove(selected);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                LV_WebSites.ItemsSource = null;
                LV_WebSites.ItemsSource = _webSites;
            });

            // Save the change to file.
            DataUtil.SavePurchaseWebSites(LotteryWebSite.GetJSONString(_webSites));
        }

        private void BT_AddSite_Click(object sender, RoutedEventArgs e)
        {
            // Start edit.
            EditWebSite(new LotteryWebSite());

            // Don't select any one.
            LV_WebSites.SelectedItem = null;
        }

        private void BT_SaveSite_Click(object sender, RoutedEventArgs e)
        {
            // Save the change on the web site.
            SaveWebSite();

            // Update selected site.
            SelectWebSite(LV_WebSites.SelectedItem as LotteryWebSite);

            // Save the change to file.
            DataUtil.SavePurchaseWebSites(LotteryWebSite.GetJSONString(_webSites));
        }

        private void EditWebSite(LotteryWebSite site)
        {
            _editingSite = site;

            // Put data to UI.
            TB_SiteName.Text = site.Name;
            TB_SiteUrl.Text = site.Uri != null ? site.Uri.ToString() : "";

            TB_SiteName.IsEnabled = site.Customized;
            TB_SiteUrl.IsEnabled = site.Customized;

            SetOutputFormatUI(site.OutputFormat);

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Show configation panel.
                BR_CustomOrderPanel.Visibility = Visibility.Visible;

                BT_SaveSite.IsEnabled = IsValidWebSite();
            });
        }

        private void SaveWebSite()
        {
            if (_editingSite != null)
            {
                // Saving data..
                if (_editingSite.Customized)
                {
                    _editingSite.Name = TB_SiteName.Text;
                    _editingSite.Uri = new Uri(TB_SiteUrl.Text);
                }
                _editingSite.OutputFormat = GetOutputFormatFromUI();

                if (!_webSites.Contains(_editingSite))
                {
                    // add this site to list if it is a newly created. Make it at first.
                    _webSites.Insert(0, _editingSite);

                    // and make sure it is selected.
                    LV_WebSites.SelectedItem = _editingSite;
                }

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Refresh the site list.
                    LV_WebSites.ItemsSource = null;
                    LV_WebSites.ItemsSource = _webSites;

                    BR_CustomOrderPanel.Visibility = Visibility.Collapsed; // hide the edit panel.
                }); 

                _editingSite = null;
            }
        }

        private bool IsValidWebSite()
        {
            return TB_SiteName.Text != "" && (!TB_SiteUrl.IsEnabled || TB_SiteUrl.Text != "");
        }

        private void TB_SiteName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update ok button.
            BT_SaveSite.IsEnabled = IsValidWebSite();
        }

        private void TB_SiteUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update ok button.
            BT_SaveSite.IsEnabled = IsValidWebSite();
        }
    }
}