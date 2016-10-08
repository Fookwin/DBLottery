using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
 

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Markup;
using LuckyBallsSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class DGContentHealder : UserControl
    {
        public DGContentHealder()
        {
            this.InitializeComponent();
        }

        public void Update(StatusOptions options, OptionChangeEnum optionChangeArg)
        {
            switch (options.Category)
            {
                case StatusCategory.RedGeneral:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged || optionChangeArg == OptionChangeEnum.ViewOptionChanged)
                        {
                            SetItemTemplate(BuildRedGeneralTemplate(options.ViewOption));
                        }
                        break;
                    }
                case StatusCategory.RedDivision:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged || optionChangeArg == OptionChangeEnum.SubCategoryChanged)
                        {
                            int divCount = DGRedDivisionViewModel.DivisionCountFromType(options.SubCategory);
                            if (divCount > 0)
                                SetItemTemplate(BuildRedDivisionTemplate(divCount));
                        }

                        break;
                    }
                case StatusCategory.RedPosition:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged || optionChangeArg == OptionChangeEnum.SubCategoryChanged)
                        {
                            int pos = DGRedPositionViewModel.RedPositionFromType(options.SubCategory);
                            if (pos > 0)
                                SetItemTemplate(BuildRedPositionTemplate(pos));
                        }

                        break;
                    }
                case StatusCategory.BlueGeneral:
                    {         
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged)
                        {
                            SetItemTemplate(BuildBlueGeneralTemplate());
                        }
                        break;
                    }
                case StatusCategory.BlueSpan:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged)
                        {
                            SetItemTemplate(BuildBlueSpanTemplate());
                        }
                        break;
                    }
            }
        }

        private void SetItemTemplate(Grid contentTemplate)
        {
            BorderContainer.Child = contentTemplate;
        }

        private Grid BuildRedDivisionTemplate(int divCount)
        {
            // Build the grid from xaml.
            string gridTemplate = "<Grid HorizontalAlignment=\"Center\" ";
            gridTemplate += "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">";

            gridTemplate += "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                gridTemplate += "<RowDefinition Height=\"25\"/>";
            }
            gridTemplate += "</Grid.RowDefinitions>";

            gridTemplate += "<Grid.ColumnDefinitions>";
            for (int i = 1; i <= 33; ++i)
            {
                gridTemplate += "<ColumnDefinition Width=\"25\"/>";
            }
            gridTemplate += "</Grid.ColumnDefinitions>";

            gridTemplate += "</Grid>";

            Grid grid = XamlReader.Load(gridTemplate) as Grid;

            // build the first row.
            AddCellToGrid(grid, "红球分区", 0, 0, 1, 33);

            // build the second row.
            int column = 0;
            int colSpan = divCount == 3 ? 11 : (divCount == 4 ? 8 : (divCount == 7 ? 5 : 3));
            for (int i = 1; i <= divCount; ++i)
            {
                AddCellToGrid(grid, i.ToString() + "分区", 1, column, 1, colSpan);

                column += colSpan;

                if (divCount == 4 && i == 2)
                {
                    // add the middle column for 17.
                    AddCellToGrid(grid, "中", 1, column, 1, 1);

                    column++;
                }

                if (divCount == 7 && i == 7)
                {
                    colSpan = 3;
                }
            }           

            // Build number coloumns.
            for (int i = 1; i <= 33; ++i)
            {
                AddCellToGrid(grid, i.ToString().PadLeft(2, '0'), 2, i - 1, 1, 1);
            }

            return grid;
        }

        private Grid BuildRedPositionTemplate(int pos)
        {
            // Preparing...
            string[] numNames = DGRedPositionViewModel.GetRedGroupInfo(pos);

            string[] propNames =  
            {                
                "奇", "偶", "质", "合", "0", "1", "2", "金", "木", "水", "火", "土"
            };

            int columnCount = numNames.Count() + propNames.Count();

            // Build the grid from xaml.
            string gridTemplate = "<Grid HorizontalAlignment=\"Center\" ";
            gridTemplate += "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">";

            gridTemplate += "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                gridTemplate += "<RowDefinition Height=\"25\"/>";
            }
            gridTemplate += "</Grid.RowDefinitions>";

            gridTemplate += "<Grid.ColumnDefinitions>";
            for (int i = 0; i < columnCount; ++i)
            {
                gridTemplate += "<ColumnDefinition Width=\"25\"/>";
            }
            gridTemplate += "</Grid.ColumnDefinitions>";

            gridTemplate += "</Grid>";

            Grid grid = XamlReader.Load(gridTemplate) as Grid;

            // build the first row.
            AddCellToGrid(grid, "红球分区走势", 0, 0, 1, 34);

            // build number column.
            int colIndex = 0;
            foreach (string num in numNames)
            {
                AddCellToGrid(grid, num, 1, colIndex, 2, 1);
                ++ colIndex;
            }

            foreach (string prop in propNames)
            {
                AddCellToGrid(grid, prop, 2, colIndex, 1, 1);
                ++colIndex;
            }

            // build the second row.
            string contentFormat = "<local:DGCell Grid.Row=\"1\" ";
            contentFormat += "Grid.Column=\"{0}\" Grid.ColumnSpan=\"{1}\" ";
            contentFormat += "CellContent=\"{2}\" ";
            contentFormat += "TextColor=\"White\" ";
            contentFormat += "BackgroundColor=\"DarkRed\" ";
            contentFormat += "/>";

            colIndex = numNames.Count(); // reset column index.
            AddCellToGrid(grid, "奇偶", 1, colIndex, 1, 2);
            AddCellToGrid(grid, "质合", 1, colIndex + 2, 1, 2);
            AddCellToGrid(grid, "除3余数", 1, colIndex + 4, 1, 3);
            AddCellToGrid(grid, "五行", 1, colIndex + 7, 1, 5);

            return grid;
        }

        private Grid BuildBlueGeneralTemplate()
        {
            string[] columnNames =  
            {
                "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", 
                "奇", "偶", "大", "小", "质", "合", "0", "1", "2", "金", "木", "水", "火", "土"
            };

            // Build the grid from xaml.
            string gridTemplate = "<Grid HorizontalAlignment=\"Center\" ";
            gridTemplate += "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">";

            gridTemplate += "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                gridTemplate += "<RowDefinition Height=\"25\"/>";
            }
            gridTemplate += "</Grid.RowDefinitions>";

            gridTemplate += "<Grid.ColumnDefinitions>";
            for (int i = 0; i < 30; ++i)
            {
                gridTemplate += "<ColumnDefinition Width=\"25\"/>";
            }
            gridTemplate += "</Grid.ColumnDefinitions>";

            gridTemplate += "</Grid>";

            Grid grid = XamlReader.Load(gridTemplate) as Grid;

            // build the first row.
            AddCellToGrid(grid, "篮球综合", 0, 0, 1, 34);

            // build number column.
            for (int i = 0; i < 30; ++i)
            {
                AddCellToGrid(grid, columnNames[i], 2, i, 1, 1);
            }

            // build the second row.

            string contentFormat = "<local:DGCell Grid.Row=\"1\" ";
            contentFormat += "Grid.Column=\"{0}\" Grid.ColumnSpan=\"{1}\" ";
            contentFormat += "CellContent=\"{2}\" ";
            contentFormat += "TextColor=\"White\" ";
            contentFormat += "BackgroundColor=\"DarkRed\" ";
            contentFormat += "/>";

            // Add columns.
            AddCellToGrid(grid, "1分区", 1, 0, 1, 4);
            AddCellToGrid(grid, "2分区", 1, 4, 1, 4);
            AddCellToGrid(grid, "3分区", 1, 8, 1, 4);
            AddCellToGrid(grid, "4分区", 1, 12, 1, 4);
            AddCellToGrid(grid, "奇偶", 1, 16, 1, 2);
            AddCellToGrid(grid, "大小", 1, 18, 1, 2);
            AddCellToGrid(grid, "质合", 1, 20, 1, 2);
            AddCellToGrid(grid, "除3余数", 1, 22, 1, 3);
            AddCellToGrid(grid, "五行", 1, 25, 1, 5);

            return grid;
        }

        private Grid BuildBlueSpanTemplate()
        {
            string[] columnNames =  
            {
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", 
                "奇", "偶", "大", "小", "质", "合", "0", "1", "2", "金", "木", "水", "火", "土"
            };

            // Build the grid from xaml.
            string gridTemplate = "<Grid HorizontalAlignment=\"Center\" ";
            gridTemplate += "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">";

            gridTemplate += "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                gridTemplate += "<RowDefinition Height=\"25\"/>";
            }
            gridTemplate += "</Grid.RowDefinitions>";

            gridTemplate += "<Grid.ColumnDefinitions>";
            for (int i = 0; i < 30; ++i)
            {
                gridTemplate += "<ColumnDefinition Width=\"25\"/>";
            }
            gridTemplate += "</Grid.ColumnDefinitions>";

            gridTemplate += "</Grid>";

            Grid grid = XamlReader.Load(gridTemplate) as Grid;

            // build the first row.
            AddCellToGrid(grid, "篮球跨度", 0, 0, 1, 34);

            for (int i = 0; i < 30; ++i)
            {
                AddCellToGrid(grid, columnNames[i], 2, i, 1, 1);
            }

            // build the second row.
            AddCellToGrid(grid, "跨度分布", 1, 0, 1, 16);
            AddCellToGrid(grid, "奇偶", 1, 16, 1, 2);
            AddCellToGrid(grid, "大小", 1, 18, 1, 2);
            AddCellToGrid(grid, "质合", 1, 20, 1, 2);
            AddCellToGrid(grid, "除3余数", 1, 22, 1, 3);
            AddCellToGrid(grid, "五行", 1, 25, 1, 5);

            return grid;
        }

        private Grid BuildRedGeneralTemplate(StatusViewOption viewOp)
        {
            string[] columnNames =  
            {
                "奇", "偶", "大", "小", "质", "合", "0", "1", "2", "小", "中", "大"
            };

            string[] extendColumnNames = null;
            string extendCategoryName = GetExtendColumnInfo(viewOp, out extendColumnNames);

            // Build the grid from xaml.
            string gridTemplate = "<Grid HorizontalAlignment=\"Center\" ";
            gridTemplate += "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">";

            gridTemplate += "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                gridTemplate += "<RowDefinition Height=\"25\"/>";
            }
            gridTemplate += "</Grid.RowDefinitions>";

            gridTemplate += "<Grid.ColumnDefinitions>";
            gridTemplate += "<ColumnDefinition Width=\"50\"/>";
            gridTemplate += "<ColumnDefinition Width=\"25\"/>";

            foreach (string propName in columnNames)
            {
                gridTemplate += "<ColumnDefinition Width=\"25\"/>";
            }

            if (extendColumnNames != null)
            {
                foreach (string extColName in extendColumnNames)
                {
                    gridTemplate += "<ColumnDefinition Width=\"50\"/>";
                }
            }
            gridTemplate += "</Grid.ColumnDefinitions>";

            gridTemplate += "</Grid>";

            Grid grid = XamlReader.Load(gridTemplate) as Grid;

            // build the first row.
            AddCellToGrid(grid, "红球综合", 0, 0, 1, 14);

            // build the second row.
            AddCellToGrid(grid, "和值", 1, 0, 2, 1);
            AddCellToGrid(grid, "连号", 1, 1, 2, 1);
            AddCellToGrid(grid, "奇偶", 1, 2, 1, 2);
            AddCellToGrid(grid, "大小", 1, 4, 1, 2);
            AddCellToGrid(grid, "质合", 1, 6, 1, 2);
            AddCellToGrid(grid, "除3余数", 1, 8, 1, 3);
            AddCellToGrid(grid, "三分区", 1, 11, 1, 3);

            // build the bottom row.
            int index = 2;
            foreach (string propName in columnNames)
            {
                AddCellToGrid(grid, propName, 2, index++, 1, 1);
            }

            // Build the extend columns according to the view options.
            if (extendColumnNames != null)
            {
                int extendColumnCount = extendColumnNames.Count();
                AddCellToGrid(grid, extendCategoryName, 0, 14, 1, extendColumnCount);
                
                int extIndex = 14;
                foreach (string extColName in extendColumnNames)
                {
                    AddCellToGrid(grid, extColName, 1, extIndex++, 2, 1);
                }
            }

            return grid;
        }

        private void AddCellToGrid(Grid target, string text, int row, int col, int rowSpan, int colSpan)
        {
            DGCell cell = new DGCell() { CellContent = text, BackgroundColor = ColorPicker.DarkRed, TextColor = ColorPicker.White };
            Grid.SetRow(cell, row);
            Grid.SetRowSpan(cell, rowSpan);
            Grid.SetColumn(cell, col);
            Grid.SetColumnSpan(cell, colSpan);
            target.Children.Add(cell);
        }

        private string GetExtendColumnInfo(StatusViewOption viewOp, out string[] columnNames)
        {
            switch (viewOp)
            {
                case StatusViewOption.RedSumDetail:
                    {
                        columnNames = new string[] { "小于 71", "71 ~ 80", "81 ~ 90", "91 ~ 100", "101 ~ 110", "111 ~ 120", "121 ~ 130", "131 ~ 140", "大于 140" };
                        return "和值分布";
                    }
                case StatusViewOption.RedContinuityDetail:
                    {
                        columnNames = new string[] { "0个", "1个", "2个", "3个", "4个", "5个"};
                        return "连号数分布";
                    }
                case StatusViewOption.RedEvenOddDetail:
                    {
                        columnNames = new string[] { "0-6", "1-5", "2-4", "3-3", "4-2", "5-1", "6-0" };
                        return "奇偶数分布 (奇-偶)";
                    }
                case StatusViewOption.RedPrimaryCompositeDetail:
                    {
                        columnNames = new string[] { "0-6", "1-5", "2-4", "3-3", "4-2", "5-1", "6-0" };
                        return "质合数分布 (质-合)";
                    }
                case StatusViewOption.RedBigSmallDetail:
                    {
                        columnNames = new string[] { "0-6", "1-5", "2-4", "3-3", "4-2", "5-1", "6-0" };
                        return "大小数分布 (大-小)";
                    }
                case StatusViewOption.RedRemain0Detail:
                    {
                        columnNames = new string[] { "0个", "1个", "2个", "3个", "4个", "5个", "6个" };
                        return "除3余0个数分布";
                    }
                case StatusViewOption.RedRemain1Detail:
                    {
                        columnNames = new string[] { "0个", "1个", "2个", "3个", "4个", "5个", "6个" };
                        return "除3余1个数分布";
                    }
                case StatusViewOption.RedRemain2Detail:
                    {
                        columnNames = new string[] { "0个", "1个", "2个", "3个", "4个", "5个", "6个" };
                        return "除3余2个数分布";
                    }
                case StatusViewOption.RedDiv1Detail:
                    {
                        columnNames = new string[] { "0个", "1个", "2个", "3个", "4个", "5个", "6个" };
                        return "3分区1区个数分布";
                    }
                case StatusViewOption.RedDiv2Detail:
                    {
                        columnNames = new string[] { "0个", "1个", "2个", "3个", "4个", "5个", "6个" };
                        return "3分区2区个数分布";
                    }
                case StatusViewOption.RedDiv3Detail:
                    {
                        columnNames = new string[] { "0个", "1个", "2个", "3个", "4个", "5个", "6个" };
                        return "3分区3区个数分布";
                    }
            }

            columnNames = null;
            return "";
        }
    }
}
