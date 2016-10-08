using System;
using System.IO;
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

namespace LuckyBallsServer.Pages
{
    /// <summary>
    /// Interaction logic for UploadToCloudPage.xaml
    /// </summary>
    public partial class UploadToCloudPage : Page
    {
        private class ActionItem
        {
            public String Name
            {
                get;
                set;
            }

            public String Source
            {
                get;
                set;
            }

            public String TargetContainer
            {
                get;
                set;
            }

            public String TargetBlob
            {
                get;
                set;
            }

            public String Status
            {
                get;
                set;
            }

            public String Error
            {
                get;
                set;
            }
        }

        private List<ActionItem> _actions = new List<ActionItem>();

        public UploadToCloudPage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                // build action items.
                List<ActionItem> _actions = BuildActionItems();

                ActionItemList.ItemsSource = _actions;
            };
        }

        private List<ActionItem> BuildActionItems()
        {
            // sql
            _actions.Add(new ActionItem() { Name = "SQL Query", Source = "Z:/ReleaseData/SQL.sql", TargetContainer="", TargetBlob="", Status = "Ready"});

            // latest release info
            _actions.Add(new ActionItem() { Name = "Release Info", Source = "Z:/ReleaseData/ReleaseInformation.xml", TargetContainer = "dblotterydata", TargetBlob = "ReleaseInformation.xml", Status = "Ready" });

            // latest attribute
            _actions.Add(new ActionItem() { Name = "Attribute Info", Source = "Z:/ReleaseData/LatestAttributes.xml", TargetContainer = "dblotterydata", TargetBlob = "LatestAttributes.xml", Status = "Ready" });

            // windows tile notification
            _actions.Add(new ActionItem() { Name = "Default Notification", Source = "Z:/ReleaseData/Notifications/Tile_Default.xml", TargetContainer = "dbnotification", TargetBlob = "Tile_Default.xml", Status = "Ready" });
            _actions.Add(new ActionItem() { Name = "Attribute Notification", Source = "Z:/ReleaseData/Notifications/Tile_attribute_analysis.xml", TargetContainer = "dbnotification", TargetBlob = "Tile_attribute_analysis.xml", Status = "Ready" });
            _actions.Add(new ActionItem() { Name = "Detail Notification", Source = "Z:/ReleaseData/Notifications/Tile_issue_details.xml", TargetContainer = "dbnotification", TargetBlob = "Tile_issue_details.xml", Status = "Ready" });
            _actions.Add(new ActionItem() { Name = "Analysis Notification", Source = "Z:/ReleaseData/Notifications/Tile_num_analysis.xml", TargetContainer = "dbnotification", TargetBlob = "Tile_num_analysis.xml", Status = "Ready" });

            // help
            _actions.Add(new ActionItem() { Name = "Help", Source = "Z:/Help.xml", TargetContainer = "dblotterydata", TargetBlob = "Help.xml", Status = "Ready" });

            // attribute template
            _actions.Add(new ActionItem() { Name = "Attributes Template", Source = "Z:/Attributes/AttributesTemplate.xml", TargetContainer = "dblotterydata", TargetBlob = "AttributesTemplate.xml", Status = "Ready" });

            // data version
            _actions.Add(new ActionItem() { Name = "Data Version", Source = "Z:/ReleaseData/Version.xml", TargetContainer = "dblotterydata", TargetBlob = "Version.xml", Status = "Ready" });

            return _actions;
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ActionItem item = btn.DataContext as ActionItem;

            string result = "";
            if (item.Name == "SQL Query")
            {
                result = LBDataManager.GetDataMgr().UpdateSQLOnCloud();
            }
            else
            {
                result = LBDataManager.GetDataMgr().UpdateFileOnCloud(item.Source, item.TargetContainer, item.TargetBlob);
            }

            if (result == "Done")
            {
                item.Status = "Success";
                item.Error = "";
            }
            else
            {
                item.Status = "Fail";
                item.Error = result;
            }

            // update the list.
            ActionItemList.ItemsSource = null;
            ActionItemList.ItemsSource = _actions;
        }

        private void ActionItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActionItem item = ActionItemList.SelectedItem as ActionItem;
            if (item != null)
            {
                string content = File.ReadAllText(item.Source);

                ContentView.Text = content;
                ErrorView.Text = item.Error;
            }
        }
    }
}
