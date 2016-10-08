using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LuckyBallSpirit.ViewModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls.ToolControl.StatusDataGrid
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

        private void SetItemTemplate(string contentTemplate)
        {
            string str = "<DataTemplate ";
            str += "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" ";
            str += "xmlns:listCtrls=\"using:LuckyBallSpirit.Controls.ToolControl.StatusDataGrid\" ";
            str += ">";
            str += contentTemplate;
            str += "</DataTemplate>";

            DataTemplate gridTemp = (DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(str);
            BorderContainer.Child = gridTemp.LoadContent() as UIElement;
        }

        private string BuildRedDivisionTemplate(int divCount)
        {
            string header = "<Grid HorizontalAlignment=\"Center\">";

            string numRowContent = "";

            string rowDef = "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                rowDef += "<RowDefinition Height=\"25\"/>";
            }
            rowDef += "</Grid.RowDefinitions>";

            string colDef = "<Grid.ColumnDefinitions>";
            for (int i = 1; i <= 33; ++i)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                // build the number row.
                string num = i.ToString().PadLeft(2, '0');
                numRowContent += "<listCtrls:DGCell Grid.Row=\"2\" Grid.Column=\"" + (i - 1).ToString() + "\" ";
                numRowContent += "CellContent=\"" + num + "\" ";
                numRowContent += "TextColor=\"White\" ";
                numRowContent += "BackgroundColor=\"DarkRed\" ";
                numRowContent += "/>";
            }
            colDef += "</Grid.ColumnDefinitions>";

            // build the first row.
            string firstRow = "<listCtrls:DGCell Grid.Row=\"0\" Grid.Column=\"0\" Grid.ColumnSpan=\"33\" ";
            firstRow += "CellContent=\"红球分区\" ";
            firstRow += "TextColor=\"White\" ";
            firstRow += "BackgroundColor=\"DarkRed\" ";
            firstRow += "/>";

            // build the second row.
            string divRow = "";
            int column = 0;
            int colSpan = divCount == 3 ? 11 : (divCount == 4 ? 8 : (divCount == 7 ? 5 : 3));
            for (int i = 1; i <= divCount; ++i)
            {
                string divContent = "<listCtrls:DGCell Grid.Row=\"1\" ";
                divContent += "Grid.Column=\"" + column.ToString() + "\" Grid.ColumnSpan=\"" + colSpan.ToString() + "\" ";
                divContent += "CellContent=\"" + i.ToString() + "分区\" ";
                divContent += "TextColor=\"White\" ";
                divContent += "BackgroundColor=\"DarkRed\" ";
                divContent += "/>";

                divRow += divContent;

                column += colSpan;

                if (divCount == 4 && i == 2)
                {
                    // add the middle column for 17.
                    string midDivContent = "<listCtrls:DGCell Grid.Row=\"1\" ";
                    midDivContent += "Grid.Column=\"" + column.ToString() + "\" ";
                    midDivContent += "CellContent=\"中\" ";
                    midDivContent += "TextColor=\"White\" ";
                    midDivContent += "BackgroundColor=\"DarkRed\" ";
                    midDivContent += "/>";

                    divRow += midDivContent;

                    column++;
                }

                if (divCount == 7 && i == 7)
                {
                    colSpan = 3;
                }
            }

            string foot = "</Grid>";
            return header + rowDef + colDef + firstRow + divRow + numRowContent + foot;
        }

        private string BuildRedPositionTemplate(int pos)
        {
            string[] numNames = DGRedPositionViewModel.GetRedGroupInfo(pos); 

            string[] propNames =  
            {                
                "奇", "偶", "质", "合", "0", "1", "2", "金", "木", "水", "火", "土"
            };

            string header = "<Grid HorizontalAlignment=\"Center\">";

            string rowDef = "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                rowDef += "<RowDefinition Height=\"25\"/>";
            }
            rowDef += "</Grid.RowDefinitions>";

            // build the first row.
            string firstRow = "<listCtrls:DGCell Grid.Row=\"0\" Grid.Column=\"0\" Grid.ColumnSpan=\"34\" ";
            firstRow += "CellContent=\"红球分区走势\" ";
            firstRow += "TextColor=\"White\" ";
            firstRow += "BackgroundColor=\"DarkRed\" ";
            firstRow += "/>";

            string colDef = "<Grid.ColumnDefinitions>";
            string secondRowContent = "";

            int colIndex = 0;
            foreach (string num in numNames)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                // build the number row.
                secondRowContent += "<listCtrls:DGCell Grid.Row=\"1\" Grid.RowSpan=\"2\" Grid.Column=\"" + colIndex.ToString() + "\" ";
                secondRowContent += "CellContent=\"" + num + "\" ";
                secondRowContent += "TextColor=\"White\" ";
                secondRowContent += "BackgroundColor=\"DarkRed\" ";
                secondRowContent += "/>";

                ++ colIndex;
            }

            foreach (string prop in propNames)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                // build the number row.
                secondRowContent += "<listCtrls:DGCell Grid.Row=\"2\" Grid.Column=\"" + colIndex.ToString() + "\" ";
                secondRowContent += "CellContent=\"" + prop + "\" ";
                secondRowContent += "TextColor=\"White\" ";
                secondRowContent += "BackgroundColor=\"DarkRed\" ";
                secondRowContent += "/>";

                ++colIndex;
            }

            // build the second row.
            string secondPropRow = "";

            string contentFormat = "<listCtrls:DGCell Grid.Row=\"1\" ";
            contentFormat += "Grid.Column=\"{0}\" Grid.ColumnSpan=\"{1}\" ";
            contentFormat += "CellContent=\"{2}\" ";
            contentFormat += "TextColor=\"White\" ";
            contentFormat += "BackgroundColor=\"DarkRed\" ";
            contentFormat += "/>";

            // Add columns.
            int propColumIndex = numNames.Count();
            secondPropRow += string.Format(contentFormat, propColumIndex, 2, "奇偶");
            secondPropRow += string.Format(contentFormat, propColumIndex + 2, 2, "质合");
            secondPropRow += string.Format(contentFormat, propColumIndex + 4, 3, "除3余数");
            secondPropRow += string.Format(contentFormat, propColumIndex + 7, 5, "五行");

            colDef += "</Grid.ColumnDefinitions>";

            string foot = "</Grid>";
            return header + rowDef + colDef + firstRow + secondRowContent + secondPropRow + foot;
        }

        private string BuildBlueGeneralTemplate()
        {
            string[] columnNames =  
            {
                "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", 
                "奇", "偶", "大", "小", "质", "合", "0", "1", "2", "金", "木", "水", "火", "土"
            };

            string header = "<Grid HorizontalAlignment=\"Center\">";

            string numRowContent = "";

            string rowDef = "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                rowDef += "<RowDefinition Height=\"25\"/>";
            }
            rowDef += "</Grid.RowDefinitions>";

            string colDef = "<Grid.ColumnDefinitions>";
            for (int i = 0; i < 30; ++i)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                // build the number row.
                numRowContent += "<listCtrls:DGCell Grid.Row=\"2\" Grid.Column=\"" + i.ToString() + "\" ";
                numRowContent += "CellContent=\"" + columnNames[i] + "\" ";
                numRowContent += "TextColor=\"White\" ";
                numRowContent += "BackgroundColor=\"DarkRed\" ";
                numRowContent += "/>";
            }
            colDef += "</Grid.ColumnDefinitions>";

            // build the first row.
            string firstRow = "<listCtrls:DGCell Grid.Row=\"0\" Grid.Column=\"0\" Grid.ColumnSpan=\"34\" ";
            firstRow += "CellContent=\"篮球综合\" ";
            firstRow += "TextColor=\"White\" ";
            firstRow += "BackgroundColor=\"DarkRed\" ";
            firstRow += "/>";

            // build the second row.
            string secondRow = "";

            string contentFormat = "<listCtrls:DGCell Grid.Row=\"1\" ";
            contentFormat += "Grid.Column=\"{0}\" Grid.ColumnSpan=\"{1}\" ";
            contentFormat += "CellContent=\"{2}\" ";
            contentFormat += "TextColor=\"White\" ";
            contentFormat += "BackgroundColor=\"DarkRed\" ";
            contentFormat += "/>";

            // Add columns.
            secondRow += string.Format(contentFormat, 0, 4, "1分区");
            secondRow += string.Format(contentFormat, 4, 4, "2分区");
            secondRow += string.Format(contentFormat, 8, 4, "3分区");
            secondRow += string.Format(contentFormat, 12, 4, "4分区");
            secondRow += string.Format(contentFormat, 16, 2, "奇偶");
            secondRow += string.Format(contentFormat, 18, 2, "大小");
            secondRow += string.Format(contentFormat, 20, 2, "质合");
            secondRow += string.Format(contentFormat, 22, 3, "除3余数");
            secondRow += string.Format(contentFormat, 25, 5, "五行");

            string foot = "</Grid>";
            return header + rowDef + colDef + firstRow + secondRow + numRowContent + foot;
        }

        private string BuildBlueSpanTemplate()
        {
            string[] columnNames =  
            {
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", 
                "奇", "偶", "大", "小", "质", "合", "0", "1", "2", "金", "木", "水", "火", "土"
            };

            string header = "<Grid HorizontalAlignment=\"Center\">";

            string numRowContent = "";

            string rowDef = "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                rowDef += "<RowDefinition Height=\"25\"/>";
            }
            rowDef += "</Grid.RowDefinitions>";

            string colDef = "<Grid.ColumnDefinitions>";
            for (int i = 0; i < 30; ++i)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                // build the number row.
                numRowContent += "<listCtrls:DGCell Grid.Row=\"2\" Grid.Column=\"" + i.ToString() + "\" ";
                numRowContent += "CellContent=\"" + columnNames[i] + "\" ";
                numRowContent += "TextColor=\"White\" ";
                numRowContent += "BackgroundColor=\"DarkRed\" ";
                numRowContent += "/>";
            }
            colDef += "</Grid.ColumnDefinitions>";

            // build the first row.
            string firstRow = "<listCtrls:DGCell Grid.Row=\"0\" Grid.Column=\"0\" Grid.ColumnSpan=\"34\" ";
            firstRow += "CellContent=\"篮球跨度\" ";
            firstRow += "TextColor=\"White\" ";
            firstRow += "BackgroundColor=\"DarkRed\" ";
            firstRow += "/>";

            // build the second row.
            string secondRow = "";

            string contentFormat = "<listCtrls:DGCell Grid.Row=\"1\" ";
            contentFormat += "Grid.Column=\"{0}\" Grid.ColumnSpan=\"{1}\" ";
            contentFormat += "CellContent=\"{2}\" ";
            contentFormat += "TextColor=\"White\" ";
            contentFormat += "BackgroundColor=\"DarkRed\" ";
            contentFormat += "/>";

            // Add columns.
            secondRow += string.Format(contentFormat, 0, 16, "跨度分布");
            secondRow += string.Format(contentFormat, 16, 2, "奇偶");
            secondRow += string.Format(contentFormat, 18, 2, "大小");
            secondRow += string.Format(contentFormat, 20, 2, "质合");
            secondRow += string.Format(contentFormat, 22, 3, "除3余数");
            secondRow += string.Format(contentFormat, 25, 5, "五行");

            string foot = "</Grid>";
            return header + rowDef + colDef + firstRow + secondRow + numRowContent + foot;
        }

        private string BuildRedGeneralTemplate(StatusViewOption viewOp)
        {
            string[] columnNames =  
            {
                "奇", "偶", "大", "小", "质", "合", "0", "1", "2", "小", "中", "大"
            };

            string contentFormat = "<listCtrls:DGCell Grid.Row=\"{0}\" Grid.RowSpan=\"{1}\" ";
            contentFormat += "Grid.Column=\"{2}\" Grid.ColumnSpan=\"{3}\" ";
            contentFormat += "CellContent=\"{4}\" ";
            contentFormat += "TextColor=\"White\" ";
            contentFormat += "BackgroundColor=\"DarkRed\" ";
            contentFormat += "/>";


            string header = "<Grid HorizontalAlignment=\"Center\">";
            
            string rowDef = "<Grid.RowDefinitions>";
            for (int i = 0; i < 3; ++i)
            {
                rowDef += "<RowDefinition Height=\"25\"/>";
            }
            rowDef += "</Grid.RowDefinitions>";

            // build the first row.
            string firstRow = "<listCtrls:DGCell Grid.Row=\"0\" Grid.Column=\"0\" Grid.ColumnSpan=\"14\" ";
            firstRow += "CellContent=\"红球综合\" ";
            firstRow += "TextColor=\"White\" ";
            firstRow += "BackgroundColor=\"DarkRed\" ";
            firstRow += "/>";

            string colDef = "<Grid.ColumnDefinitions>";

            // build the second row.
            string secondRow = "";
            colDef += "<ColumnDefinition Width=\"50\"/>";
            secondRow += string.Format(contentFormat, 1, 2, 0, 1, "和值");

            colDef += "<ColumnDefinition Width=\"25\"/>";
            secondRow += string.Format(contentFormat, 1, 2, 1, 1, "连号");

            secondRow += string.Format(contentFormat, 1, 1, 2, 2, "奇偶");
            secondRow += string.Format(contentFormat, 1, 1, 4, 2, "大小");
            secondRow += string.Format(contentFormat, 1, 1, 6, 2, "质合");
            secondRow += string.Format(contentFormat, 1, 1, 8, 3, "除3余数");
            secondRow += string.Format(contentFormat, 1, 1, 11, 3, "三分区");

            // build the bottom row.
            string bottomRow = "";

            int index = 2;
            foreach (string propName in columnNames)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";
                bottomRow += string.Format(contentFormat, 2, 1, index++, 1, propName);
            }

            // Build the extend columns according to the view options.
            string extFirstRow = "", extBottomRow = "";
            string[] extendColumnNames = null;
            string extendCategoryName = GetExtendColumnInfo(viewOp, out extendColumnNames);
            if (extendColumnNames != null)
            {
                int extendColumnCount = extendColumnNames.Count();

                extFirstRow = "<listCtrls:DGCell Grid.Row=\"0\" Grid.Column=\"14\" Grid.ColumnSpan=\"" + extendColumnCount.ToString() + "\" ";
                extFirstRow += "CellContent=\"" + extendCategoryName + "\" ";
                extFirstRow += "TextColor=\"White\" ";
                extFirstRow += "BackgroundColor=\"DarkRed\" ";
                extFirstRow += "/>";
                
                int extIndex = 14;
                foreach (string extColName in extendColumnNames)
                {
                    colDef += "<ColumnDefinition Width=\"50\"/>";
                    extBottomRow += string.Format(contentFormat, 1, 2, extIndex++, 1, extColName);
                }
            }

            colDef += "</Grid.ColumnDefinitions>";

            string foot = "</Grid>";
            return header + rowDef + colDef + firstRow + extFirstRow + secondRow + extBottomRow + bottomRow + foot;
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
