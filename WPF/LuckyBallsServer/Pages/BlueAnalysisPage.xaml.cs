using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
using LuckyBallsData.Statistics;
using System.Windows.Threading;
using System.Threading;

namespace LuckyBallsServer
{
    /// <summary>
    /// Interaction logic for BlueAnalysisPage.xaml
    /// </summary>
    public partial class BlueAnalysisPage : Page
    {
        private Dictionary<int, BlueRadomAnalyser> _analyzer = new Dictionary<int, BlueRadomAnalyser>();
        private List<Lottery> _baseTests = null;

        public BlueAnalysisPage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                _baseTests = LBDataManager.Instance().History.Lotteries.ToList();
                _baseTests.Reverse();

                StartFrom.ItemsSource = _baseTests;
                StartFrom.SelectedIndex = 0;

                Test_Threshold.ItemsSource = new int[] { 360, 390, 420, 450 };
                Test_Threshold.SelectedIndex = 0;
            };
        }

        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            int threshold = Convert.ToInt32(Test_Threshold.SelectedItem);
            Lottery selected = StartFrom.SelectedItem as Lottery;            

            // Get analylser.
            BlueRadomAnalyser oAnalyser = null;
            if (_analyzer.ContainsKey(selected.Issue))
            {
                oAnalyser = _analyzer[selected.Issue];

                Cal_Button.IsEnabled = false;

                Thread calThread = new Thread(() =>
                {
                    var result = oAnalyser.Calculate(threshold);
                    Dispatcher.Invoke(() =>
                    {
                        List_Candidates.DataContext = BuildSequenceTable(result);
                        Max_Omission.Text = oAnalyser.MaxOmission().ToString();
                    });
                });

                calThread.SetApartmentState(ApartmentState.STA);
                calThread.Start();
            }
        }

        private DataTable BuildMatrixDataTable(int[,] data)
        {
            DataTable table = new DataTable();
            Type intType = System.Type.GetType("System.Int32");

            table.Columns.Add(CreateColumn("Num", intType, true, false));

            for (int i = 1; i <= 16; ++i)
            {
                table.Columns.Add(CreateColumn("P_" + i.ToString(), intType, false, false));
            }

            DataRow row;
            for (int num = 0; num < 16; ++num)
            {
                row = table.NewRow();
                row["Num"] = num + 1;

                for (int pos = 1; pos <= 16; ++pos)
                {
                    row["P_" + pos.ToString()] = data[num, pos - 1];
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private DataTable BuildSequenceTable(List<BlueRadomAnalyser.Sequence> list)
        {
            DataTable table = new DataTable();
            Type intType = System.Type.GetType("System.Int32");

            table.Columns.Add(CreateColumn("Index", intType, true, false));
            table.Columns.Add(CreateColumn("Score", intType, false, false));

            for (int i = 1; i <= 16; ++i)
            {
                table.Columns.Add(CreateColumn("N+" + i.ToString(), intType, false, false));
            }

            DataRow row;
            for (int i = 0; i < list.Count; ++i)
            {     
                row = table.NewRow();
                row["Index"] = i + 1;
                row["Score"] = list[i].Score;

                for (int j = 1; j <= 16; ++j)
                {
                    row["N+" + j.ToString()] = list[i].NumAt(16 - j);
                }

                table.Rows.Add(row);
            }            

            return table;
        }

        private DataColumn CreateColumn(string text, Type datatype, bool unique, bool writable)
        {
            DataColumn column = new DataColumn();
            column.DataType = datatype;
            column.ColumnName = text;
            column.ReadOnly = !writable;
            column.Unique = unique;

            return column;
        }

        private void StartFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // update controls...
            int threshold = Convert.ToInt32(Test_Threshold.SelectedItem);
            Lottery selected = StartFrom.SelectedItem as Lottery;

            // Get analylser.
            BlueRadomAnalyser oAnalyser = null;
            if (_analyzer.ContainsKey(selected.Issue))
            {
                oAnalyser = _analyzer[selected.Issue];
            }
            else
            {
                int index = StartFrom.SelectedIndex;

                // Get the list for the testing.
                List<Lottery> tests = _baseTests.GetRange(index, _baseTests.Count - index);

                oAnalyser = new BlueRadomAnalyser();
                oAnalyser.Init(tests);

                _analyzer.Add(selected.Issue, oAnalyser);
            }

            //Num_Matrix.DataContext = BuildMatrixDataTable(oAnalyser.GetMatrix());

            var result = oAnalyser.GetResult(threshold);
            if (result == null && threshold > 300)
            {
                result = oAnalyser.Calculate(threshold);
            }

            if (result != null)
            {
                List_Candidates.DataContext = BuildSequenceTable(result);
                Cal_Button.IsEnabled = false;

                Max_Omission.Text = oAnalyser.MaxOmission().ToString();
            }
            else
            {
                List_Candidates.DataContext = null;
                Cal_Button.IsEnabled = true;
            }
        }

        private void Test_Threshold_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int threshold = Convert.ToInt32(Test_Threshold.SelectedItem);
            Lottery selected = StartFrom.SelectedItem as Lottery;

            // Get analylser.
            BlueRadomAnalyser oAnalyser = null;
            if (_analyzer.ContainsKey(selected.Issue))
            {
                oAnalyser = _analyzer[selected.Issue];

                var result = oAnalyser.GetResult(threshold);
                if (result != null)
                {
                    List_Candidates.DataContext = BuildSequenceTable(result);
                    Cal_Button.IsEnabled = false;

                    Max_Omission.Text = oAnalyser.MaxOmission().ToString();
                }
                else
                {
                    List_Candidates.DataContext = null;
                    Cal_Button.IsEnabled = true;
                }
            }
        }
    }
}
