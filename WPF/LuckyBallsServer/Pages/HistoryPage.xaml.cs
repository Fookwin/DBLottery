using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
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
using System.Data;
using System.ComponentModel;


namespace LuckyBallsServer
{
    /// <summary>
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : Page
    {
        private int CountToShow = 30;
        private DataTable DataToShow;
        private delegate void ThreadDelegate();

        class ColumnInfo
        {
            public string Identifier { get; set; }
            public string DisplayName { get;set;}
            public bool Visible { get; set; }
            public int IndexInGrid { get; set; }
        };

        class ColumnCategory
        {
            public ColumnCategory(string id, string display)
            {
                Identifier = id;
                DisplayName = display;
                Columns = new List<ColumnInfo>();
            }

            public string Identifier {get; set;}
            public string DisplayName { get;set;}
            public List<ColumnInfo> Columns { get; set; }
        };

        private List<ColumnCategory> ColumnCategories = new List<ColumnCategory>();

        public HistoryPage()
        {
            InitializeComponent();

            InitData();

            InitCtrls();
        }

        private void InitData()
        {
            // columns.
            //

            // basic columns.
            ColumnCategory basicCy = new ColumnCategory("ID_Basic", "基本信息");
            basicCy.Columns.Add(new ColumnInfo { Identifier = "Index", DisplayName = "序号", Visible = true });
            basicCy.Columns.Add(new ColumnInfo { Identifier = "Issue", DisplayName = "期号", Visible = true });
            for (int i = 1; i <= 6; ++i)
            {
                basicCy.Columns.Add(new ColumnInfo { Identifier = "Red" + i.ToString(), DisplayName = "红" + i.ToString(), Visible = true });
            }
            basicCy.Columns.Add(new ColumnInfo { Identifier = "Blue", DisplayName = "蓝", Visible = true });
            ColumnCategories.Add(basicCy);

            ColumnCategory redAttriCy = new ColumnCategory("ID_Red_Attributes", "红球状态");
            redAttriCy.Columns.Add(new ColumnInfo { Identifier = "Continuity", DisplayName = "连续", Visible = true });
            redAttriCy.Columns.Add(new ColumnInfo { Identifier = "EvenCount", DisplayName = "偶数", Visible = true });
            redAttriCy.Columns.Add(new ColumnInfo { Identifier = "PrimeNumCount", DisplayName = "质合", Visible = true });
            redAttriCy.Columns.Add(new ColumnInfo { Identifier = "SmallNumCount", DisplayName = "小数", Visible = true });
            redAttriCy.Columns.Add(new ColumnInfo { Identifier = "Sum", DisplayName = "和值", Visible = true });
            redAttriCy.Columns.Add(new ColumnInfo { Identifier = "TotalOmission", DisplayName = "遗漏", Visible = true });
            redAttriCy.Columns.Add(new ColumnInfo { Identifier = "RepeatPreivous", DisplayName = "重复", Visible = true });
            ColumnCategories.Add(redAttriCy);

            ColumnCategory misCy = new ColumnCategory("ID_Omission", "数据遗漏");
            for (int i = 1; i <= 33; ++i)
            {
                misCy.Columns.Add(new ColumnInfo { Identifier = "Mis_Red" + i.ToString(), DisplayName = "红遗" + i.ToString(), Visible = false });
            }

            for (int i = 1; i <= 16; ++i)
            {
                misCy.Columns.Add(new ColumnInfo { Identifier = "Mis_Blue" + i.ToString(), DisplayName = "蓝遗" + i.ToString(), Visible = false });
            }
            ColumnCategories.Add(misCy);


            // data grid...
            //

            int index = 0;
            foreach (ColumnCategory cy in ColumnCategories)
            {
                foreach (ColumnInfo info in cy.Columns)
                {
                    DataGridTextColumn newCol = new DataGridTextColumn();
                    newCol.Header = info.DisplayName;
                    newCol.Binding = new Binding(info.Identifier);
                    newCol.Visibility = info.Visible ? Visibility.Visible : Visibility.Hidden;

                    DisplayGrid.Columns.Add(newCol);

                    info.IndexInGrid = index++;
                }
            }
        }

        private void InitCtrls()
        {
            // Init cout to show.
            CountToShowCtrl.ItemsSource = new string[6] { "30", "50", "100", "500", "1000", "全部" };

            // Inti colomn filer.
            List<string> categories = new List<string>();
            foreach (ColumnCategory cy in ColumnCategories)
            {
                categories.Add(cy.DisplayName);
            }
            categories.Add("全部");

            ColumnCategoryCtrl.ItemsSource = categories;
            ListColumnsInCategory("基本信息");
        }

        private void ListColumnsInCategory(string categoryName)
        {
            bool bShowAll = categoryName == "全部";

            List<ColumnInfo> columns = new List<ColumnInfo>();
            foreach (ColumnCategory cy in ColumnCategories)
            {
                if (bShowAll)
                {
                    columns.AddRange(cy.Columns);
                }
                else if (cy.DisplayName == categoryName)
                {
                    ColumnFilterCtrl.DataContext = cy.Columns;
                    return;
                }
            }

            ColumnFilterCtrl.DataContext = columns;
        }

        private void Update()
        {
            ThreadDelegate backWorkDel = new ThreadDelegate(ProcessUpdate);
            backWorkDel.BeginInvoke(null, null);
        }

        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            Update();
        }
        
        private void ProcessUpdate()
        {
            ThreadDelegate InitDel = delegate()
            {
                //WaitProgress.Visibility = System.Windows.Visibility.Visible;
                LoadingText.Visibility = System.Windows.Visibility.Visible;
                DisplayGrid.Visibility = System.Windows.Visibility.Hidden;
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, InitDel);

            DataToShow = LBDataManager.GetDataMgr().QueryHistory(CountToShow);

            ThreadDelegate CompleteDel = delegate()
            {
                DisplayGrid.DataContext = DataToShow;
                //WaitProgress.Visibility = System.Windows.Visibility.Hidden;
                LoadingText.Visibility = System.Windows.Visibility.Hidden;
                DisplayGrid.Visibility = System.Windows.Visibility.Visible;
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, CompleteDel);            
        }

        private void CountToShowCtrl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            string sel = CountToShowCtrl.SelectedValue.ToString();
            if (CountToShow > 0 && sel == CountToShow.ToString())
                return;

            if (CountToShow < 0 && sel == "全部")
                return;

            if (sel == "全部")
                CountToShow = -1;
            else
                CountToShow = Convert.ToInt32(sel);

            Update();
        }

        private void ColumnCategoryCtrl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            string sel = ColumnCategoryCtrl.SelectedValue.ToString();

            ListColumnsInCategory(sel);
        }

        private void chBox_Checked_1(object sender, RoutedEventArgs e)
        {
            CheckBox sel = e.OriginalSource as CheckBox;
            TextBlock content = sel.Content as TextBlock;

            SetColumeVisibility(content.Text, true); 
        }

        private void chBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            CheckBox sel = e.OriginalSource as CheckBox;
            TextBlock content = sel.Content as TextBlock;

            SetColumeVisibility(content.Text, false);
        }

        private void SetColumeVisibility(string name, bool bVisible)
        {
            foreach (ColumnCategory cy in ColumnCategories)
            {
                foreach (ColumnInfo info in cy.Columns)
                {
                    if (info.DisplayName == name)
                    {
                        info.Visible = bVisible;

                        // update the column of the data grid.
                        DisplayGrid.Columns[info.IndexInGrid].Visibility = bVisible ? Visibility.Visible : Visibility.Hidden;
                        return;
                    }
                }
            }
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

        private void Button_Click_QuckTest(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(LuckyBallsData.Util.DataUtil.Test(LBDataManager.GetDataMgr().GetHistory()).ToString());
        }
    }
}
