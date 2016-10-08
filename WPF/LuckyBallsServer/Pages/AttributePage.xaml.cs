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
using LuckyBallsData.Selection;

namespace LuckyBallsServer
{
    /// <summary>
    /// Interaction logic for AttributePage.xaml
    /// </summary>
    public partial class AttributePage : Page
    {
        private bool showAll = true;
        private double protionalPorpCondi = 4.0;
        private double hitPorpCondi = 4.0;
        private bool initialized = false;
        private int currentIssue = -1;

        public AttributePage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {                
                List<string> issues = LBDataManager.GetDataMgr().GetIssueNumbers();
                CB_Issue.ItemsSource = issues;
                CB_Issue.SelectedIndex = issues.Count - 1;
                currentIssue = Convert.ToInt32(issues.Last());

                UpdateList();
                initialized = true;
            };
        }

        private void Attributes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (showAll)
            {
                SchemeAttributeCategory cat = Attributes.SelectedItem as SchemeAttributeCategory;
                AttributeDetails.DataContext = cat == null ? null : LBDataManager.GetDataMgr().GetAttributesInCategory(cat, null);
            }
            else
            {
                SchemeAttributeValueStatus state = Attributes.SelectedItem as SchemeAttributeValueStatus;
                AttributeDetails.DataContext = state == null ? null : LBDataManager.GetDataMgr().GetState(state);
            }
        }

        private void UpdateList()
        {
            AttributeDetails.DataContext = null;
            if (showAll)
            {
                Attributes.ItemsSource = LBDataManager.GetDataMgr().GetAttributeCatetories(currentIssue);
            }
            else
            {
                Attributes.ItemsSource = LBDataManager.GetDataMgr().GetAttributeValueStates(currentIssue, 
                    new FilterOption() { HitPropLowLimit = hitPorpCondi, ProtentialPropLowLimit = protionalPorpCondi });
            }
        }

        private void RB_ShowAll_Checked(object sender, RoutedEventArgs e)
        {
            if (initialized)
            {
                showAll = true;
                BR_Conditions.IsEnabled = false;
                UpdateList();
            }
        }

        private void RB_Filters_Checked(object sender, RoutedEventArgs e)
        {
            if (initialized)
            {
                showAll = false;
                BR_Conditions.IsEnabled = true;
                //UpdateList(); // happen when filter button clicked.
            }
        }

        private void TB_ProtionalProp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (initialized)
            {
                double val = protionalPorpCondi;
                try
                {
                    val = Convert.ToDouble(TB_ProtionalProp.Text);
                }
                catch
                {
                    TB_ProtionalProp.Text = protionalPorpCondi.ToString("0.0");
                }

                protionalPorpCondi = val;
            }
        }

        private void TB_Prop_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (initialized)
            {
                double val = hitPorpCondi;
                try
                {
                    val = Convert.ToDouble(TB_Prop.Text);
                }
                catch
                {
                    TB_Prop.Text = hitPorpCondi.ToString("0.0");
                }

                hitPorpCondi = val;
            }
        }

        private void BT_Filter_Click(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void BT_Update_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {

        }

        private void CB_Issue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialized)
            {
                currentIssue = Convert.ToInt32(CB_Issue.SelectedItem as string);
                UpdateList();
            }
        }
    }
}
