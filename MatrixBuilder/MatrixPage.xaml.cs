﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace MatrixBuilder
{
    /// <summary>
    /// Interaction logic for MatrixPage.xaml
    /// </summary>
    public partial class MatrixPage : Page
    {
        MatrixTableBuilder _builder = new MatrixTableBuilder();
        private DataTable _table = null;
        private delegate void ThreadDelegate();
        private bool _userCanceled = false;
        private MatrixCell _selectedCell = null;
        private int _selectedRow = 0;
        private int _selectedCol = 0;

        DispatcherTimer _timer = new DispatcherTimer();

        public MatrixPage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                RefreshTable();

                _timer.Tick += new EventHandler(timer_Tick);
                _timer.Interval = TimeSpan.FromSeconds(2);
            };
        }

        private void RefreshTable()
        {
            _table = BuildMatrixTable(_builder.GetTable());
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
                _selectedRow = (int)rowID + 6;
                _selectedCol = col + 2;
                _selectedCell = _builder.GetTable().GetCell(_selectedRow, _selectedCol);
                if (_selectedCell != null)
                {
                    LV_Template.DataContext = _selectedCell.Template;
                    solutionCount = _selectedCell.Template.Count;                    
                }

                BT_Verify.IsEnabled = solutionCount > 0;
                Selected_Cell_Name.Text = _selectedRow.ToString() + " 选 " + _selectedCol.ToString() + " [" + solutionCount.ToString() + "] - " + _selectedCell.Status.ToString();
                TB_TestStart.Text = (solutionCount - 1).ToString();
            }
            else
            {
                _selectedRow = -1;
                _selectedCol = -1;
                _selectedCell = null;

                BT_Verify.IsEnabled = false;
                Selected_Cell_Name.Text = "";
                TB_TestStart.Text = "-1";
            }
        }

        private void Button_Click_Calculate(object sender, RoutedEventArgs e)
        {
            if (_selectedRow < 0 || _selectedCol < 0)
                return;

            // clearn the progress list.
            _userCanceled = false;
            LV_Progress.ItemsSource = null;

            // Start timer.
            _timer.Start();

            int expectedItemCount = Convert.ToInt32(TB_TestStart.Text);
            int algorithm = CB_Algorithm.SelectedIndex;

            string config = "[" + _selectedRow.ToString() + " - " + _selectedCol.ToString() + "]: Algorithm: " + CB_Algorithm.Text + " with " + expectedItemCount.ToString() + "items";
            this.WindowTitle = "Calculating " + config;

            Thread calThread = new Thread(() =>
            {
                _builder.BuildMarixCell(_selectedRow, _selectedCol, algorithm, expectedItemCount);

                _timer.Stop();

                Dispatcher.Invoke(() =>
                {
                    this.WindowTitle = "Finished " + config;
                    RefreshTable();
                });

            });

            calThread.SetApartmentState(ApartmentState.STA);
            calThread.Start();
        }

        private void Button_Click_Skip(object sender, RoutedEventArgs e)
        {
            _userCanceled = true;
        }

        private void Button_Click_Verify(object sender, RoutedEventArgs e)
        {
            if (_selectedCell != null)
            {
                bool valid = MatrixCalculatorWrapper.ValidateSolution(_selectedRow, _selectedCol, _selectedCell.Template);
                MessageBox.Show(valid ? "valid" : "invalid");
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            // update the controls.
            Dispatcher.Invoke(() =>
            {
                if (_builder != null)
                {
                    LV_Progress.ItemsSource = null;

                    if (_userCanceled)
                    {
                        _builder.Abort();
                    }
                    else
                    {
                        LV_Progress.ItemsSource = _builder.GetProgress();
                    }
                }
            }, DispatcherPriority.Normal);
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

        public DataTable BuildMatrixTable(MatrixTable _matrixTable)
        {
            DataTable table = new DataTable();

            Type intType = System.Type.GetType("System.Int32");
            Type stringType = System.Type.GetType("System.String");
            Type doubleType = System.Type.GetType("System.Double");
            table.Columns.Add(CreateColumn("选号数", intType, false, false));
            table.Columns.Add(CreateColumn("选2中1", intType, false, false));
            table.Columns.Add(CreateColumn("选3中2", intType, false, false));
            table.Columns.Add(CreateColumn("选4中3", intType, false, false));
            table.Columns.Add(CreateColumn("选5中4", intType, false, false));
            table.Columns.Add(CreateColumn("选6中5", intType, false, false));

            for (int i = 6; i <= 33; ++i)
            {
                DataRow row = table.NewRow();

                row["选号数"] = i;
                row["选2中1"] = _matrixTable.GetCellItemCount(i, 2);
                row["选3中2"] = _matrixTable.GetCellItemCount(i, 3);
                row["选4中3"] = _matrixTable.GetCellItemCount(i, 4);
                row["选5中4"] = _matrixTable.GetCellItemCount(i, 5);
                row["选6中5"] = _matrixTable.GetCellItemCount(i, 6);

                table.Rows.Add(row);
            }

            return table;
        }

        private void BT_Normalize_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCell != null)
            {
                if (_builder.Normalize(_selectedRow, _selectedCol))
                {
                    // update the list
                    LV_Template.DataContext = _selectedCell.Template;
                }
            }
        }
    }
}
