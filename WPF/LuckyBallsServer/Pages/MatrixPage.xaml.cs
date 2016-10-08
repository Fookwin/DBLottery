using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LuckyBallsServer
{
    class ProgressState
    {
        public string ThreadID
        {
            get;set;
        }

        public string Message
        {
            get; set;
        }

        public double Progress
        {
            get;set;
        }
    }

    /// <summary>
    /// Interaction logic for MatrixPage.xaml
    /// </summary>
    public partial class MatrixPage : Page
    {
        private DataTable _table = null;
        private delegate void ThreadDelegate();
        private bool skip = false;
        private Dictionary<string, ProgressState> _progressStates = new Dictionary<string, ProgressState>();
        private LuckyBallsData.Filters.MatrixCell selectedCell = null;
        private int selectedRow = 0;
        private int selectedCol = 0;

        DispatcherTimer timer = new DispatcherTimer();

        public MatrixPage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                InitTable();

                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = TimeSpan.FromSeconds(2);
            };
        }

        private void InitTable()
        {
            _table = LuckyBallsServer.LBDataManager.GetDataMgr().InitMatrixTable();
            DG_Matrix.DataContext = _table;
        }

        private void DG_Matrix_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGridCellInfo selected = DG_Matrix.CurrentCell;
            if (selected != null && selected.IsValid)
            {
                int col = selected.Column.DisplayIndex - 1;

                DataRowView dvr = (DataRowView)selected.Item;
                DataRow dataRow = dvr.Row;

                System.Reflection.FieldInfo fieldInfo = dataRow.GetType().GetField("_rowID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                Int64 rowID = (Int64)fieldInfo.GetValue(dataRow) - 1;

                int solutionCount = -1;
                selectedRow = (int)rowID + 6;
                selectedCol = col + 2;
                selectedCell = LuckyBallsServer.LBDataManager.GetDataMgr().MatrixTable.GetCell(selectedRow, selectedCol);
                if (selectedCell != null)
                {
                    LV_Template.DataContext = selectedCell.Template;
                    solutionCount = selectedCell.Template.Count;                    
                }

                BT_Verify.IsEnabled = solutionCount > 0;
                Selected_Cell_Name.Text = selectedRow.ToString() + " 选 " + selectedCol.ToString() + " [" + solutionCount.ToString() + "]";
            }
            else
            {
                selectedRow = -1;
                selectedCol = -1;
                selectedCell = null;

                BT_Verify.IsEnabled = false;
                Selected_Cell_Name.Text = "";
            }
        }

        private void Button_Click_Calculate(object sender, RoutedEventArgs e)
        {
            if (selectedRow < 0 || selectedCol < 0)
                return;

            // clearn the progress list.
            _progressStates.Clear();
            LV_Progress.ItemsSource = null;

            // Start timer.
            timer.Start();

            int start = Convert.ToInt32(TB_TestStart.Text);
            int algorithm = CB_Algorithm.SelectedIndex;

            MatrixTableBuilder builder = new MatrixTableBuilder(LuckyBallsServer.LBDataManager.GetDataMgr().MatrixTable);
            builder.MatrixProgressHandler += UpdateProgress;

            Thread calThread = new Thread(() =>
            {
                DateTime startTime = DateTime.Now;
                int iRes = builder.BuildMarixCell(selectedRow, selectedCol, start, algorithm);

                TimeSpan duration = DateTime.Now - startTime;
                timer.Stop();

                Dispatcher.Invoke(() =>
                {
                    _table = LuckyBallsServer.LBDataManager.GetDataMgr().InitMatrixTable();
                    DG_Matrix.DataContext = _table;
                });

                MessageBox.Show((iRes > 0 ? "Found Solution with Count " + iRes.ToString() : "No Solution Found!") + " Duration: "+ duration.ToString());
            });

            calThread.SetApartmentState(ApartmentState.STA);
            calThread.Start(); 
        }

        private int UpdateProgress(string threadID, string message, double progress)
        {
            lock (_progressStates)
            {
                if (_progressStates.ContainsKey(threadID))
                {
                    if (progress < 0)
                    {
                        _progressStates.Remove(threadID);
                    }
                    else
                    {
                        _progressStates[threadID].Message = message;
                        _progressStates[threadID].Progress = progress;
                    }
                }
                else if (progress >= 0)
                {
                    _progressStates.Add(threadID, new ProgressState() { Message = message, ThreadID = threadID, Progress = progress });
                }
            }

            return skip ? -1 : 0;
        }

        private void Button_Click_Skip(object sender, RoutedEventArgs e)
        {
            skip = true;
        }

        private void Button_Click_Verify(object sender, RoutedEventArgs e)
        {
            if (selectedCell != null)
            {
                bool valid = BuildMatrixUtil.ValidateSolution(selectedRow, selectedCol, selectedCell.Template);
                MessageBox.Show(valid ? "valid" : "invalid");
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            // update the controls.
            Dispatcher.Invoke(() =>
            {
                lock (_progressStates)
                {
                    LV_Progress.ItemsSource = null;
                    LV_Progress.ItemsSource = _progressStates.ToList();
                }
            }, DispatcherPriority.Normal);
        }
    }
}
