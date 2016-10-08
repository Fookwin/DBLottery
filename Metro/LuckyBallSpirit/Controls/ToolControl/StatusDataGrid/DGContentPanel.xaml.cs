using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
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
using LuckyBallsData.Statistics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls.ToolControl.StatusDataGrid
{
    public sealed partial class DGContentPanel : UserControl
    {
        public delegate void ViewChangeEventHandler(object sender, ScrollViewerViewChangedEventArgs e);
        public event ViewChangeEventHandler OnViewChangeEventHandler;

        private ScrollViewer _listScrollViewer = null;
        private List<Lottery> _basicContent = null;

        // cached data...
        private DGViewModelBase _viewModel = null;

        public DGContentPanel()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                _listScrollViewer = DGUtil.FindScrollViewer(ListBoxCtrl);
                if (_listScrollViewer != null)
                {
                    _listScrollViewer.ViewChanged += OnViewChanged;
                }                
            };
        }

        public void SetContent(List<Lottery> content)
        {
            // Reset the cached content so that the UI could be refreshed later.
            _basicContent = content;
            _viewModel = null;
        }

        public void Update(StatusOptions options, OptionChangeEnum optionChangeArg)
        {
            switch (options.Category)
            {
                case StatusCategory.RedDivision:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged)
                        {
                            SetItemTemplate(BuildGridXamlRedDivision());
                            _viewModel = null; // rebuild the view model.
                        }

                        bool bResetSource = false;
                        if (_viewModel == null)
                        {
                            _viewModel = new DGRedDivisionViewModel();
                            _viewModel.Init(_basicContent);
                            bResetSource = true;
                        }

                        _viewModel.SetOptions(options);

                        if (bResetSource)
                        {
                            ObservableCollection<DGRedDivisionRowModel> redData = 
                                _viewModel.GetViewData() as ObservableCollection<DGRedDivisionRowModel>;
                            ListBoxCtrl.ItemsSource = redData;
                            ListBoxCtrl.ScrollIntoView(redData.Last());
                        }

                        break;
                    }
                case StatusCategory.BlueGeneral:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged)
                        {
                            SetItemTemplate(BuildGridXamlBlue());
                            _viewModel = null; // rebuild the view model.
                        }

                        bool bResetSource = false;
                        if (_viewModel == null)
                        {
                            _viewModel = new DGBlueGeneralViewModel();
                            _viewModel.Init(_basicContent);
                            bResetSource = true;
                        }

                        _viewModel.SetOptions(options);

                        if (bResetSource)
                        {
                            ObservableCollection<DGBlueNumberRowModel> blueData =
                                _viewModel.GetViewData() as ObservableCollection<DGBlueNumberRowModel>;

                            ListBoxCtrl.ItemsSource = blueData;
                            ListBoxCtrl.ScrollIntoView(blueData.Last());
                        }
                        break;
                    }
                case StatusCategory.BlueSpan:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged)
                        {
                            SetItemTemplate(BuildGridXamlBlue());
                            _viewModel = null; // rebuild the view model.
                        }

                        bool bResetSource = false;
                        if (_viewModel == null)
                        {
                            _viewModel = new DGBlueSpanViewModel();
                            _viewModel.Init(_basicContent);
                            bResetSource = true;
                        }

                        _viewModel.SetOptions(options);

                        if (bResetSource)
                        {
                            ObservableCollection<DGBlueNumberRowModel> blueData =
                                _viewModel.GetViewData() as ObservableCollection<DGBlueNumberRowModel>;

                            ListBoxCtrl.ItemsSource = blueData;
                            ListBoxCtrl.ScrollIntoView(blueData.Last());
                        }
                        break;
                    }
                case StatusCategory.RedGeneral:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged)
                        {
                            SetItemTemplate(BuildGridXamlRedGeneral());
                            _viewModel = null; // rebuild the view model.
                        }

                        bool bResetSource = false;
                        if (_viewModel == null)
                        {
                            _viewModel = new DGRedGeneralViewModel();
                            _viewModel.Init(_basicContent);
                            bResetSource = true;
                        }

                        _viewModel.SetOptions(options);

                        if (bResetSource)
                        {
                            ObservableCollection<DGRedGeneralRowModel> data =
                                _viewModel.GetViewData() as ObservableCollection<DGRedGeneralRowModel>;

                            ListBoxCtrl.ItemsSource = data;
                            ListBoxCtrl.ScrollIntoView(data.Last());
                        }
                        break;
                    }
                case StatusCategory.RedPosition:
                    {
                        if (optionChangeArg == OptionChangeEnum.CategoryChanged)
                        {
                            SetItemTemplate(BuildGridXamlRedPosition());
                            _viewModel = null; // rebuild the view model.
                        }

                        bool bResetSource = false;
                        if (_viewModel == null)
                        {
                            _viewModel = new DGRedPositionViewModel();
                            _viewModel.Init(_basicContent);
                            bResetSource = true;
                        }

                        _viewModel.SetOptions(options);

                        if (bResetSource)
                        {
                            ObservableCollection<DGRedPositionRowModel> data =
                                _viewModel.GetViewData() as ObservableCollection<DGRedPositionRowModel>;

                            ListBoxCtrl.ItemsSource = data;
                            ListBoxCtrl.ScrollIntoView(data.Last());
                        }
                        break;
                    }
            }
        }

        public double VerticalOffset
        {
            get
            {
                return _listScrollViewer.VerticalOffset;
            }

            set
            {
                _listScrollViewer.ScrollToVerticalOffset(value);
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return _listScrollViewer.HorizontalOffset;
            }

            set
            {
                _listScrollViewer.ScrollToHorizontalOffset(value);
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

            DataTemplate template = (DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(str);
            ListBoxCtrl.ItemTemplate = template;
        }

        private string BuildGridXamlRedDivision()
        {
            string header = "<Grid Height=\"25\">";

            string content = "";

            string colDef = "<Grid.ColumnDefinitions>";
            for (int i = 1; i <= 33; ++i)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                string cellObj = "Cell_" + i.ToString().PadLeft(2, '0');
                content += "<listCtrls:DGCell Grid.Column=\"" + (i - 1).ToString() + "\" ";
                content += "CellContent=\"{Binding " + cellObj + ".Text}\" ";
                content += "CircleVisibility=\"{Binding " + cellObj + ".ShowAsBall}\" ";
                content += "CircleColor=\"{Binding " + cellObj + ".BallColor}\" ";
                content += "TextColor=\"{Binding " + cellObj + ".ForeColor}\" ";
                content += "BackgroundColor=\"{Binding " + cellObj + ".BackColor}\" ";
                content += "/> ";
            }
             
            colDef += "</Grid.ColumnDefinitions>";

            string foot = "</Grid>";
            return header + colDef + content + foot;
        }

        private string BuildGridXamlBlue()
        {
            string header = "<Grid Height=\"25\">";

            string content = "";
            string colDef = "<Grid.ColumnDefinitions>";
            for (int i = 1; i <= 16; ++i)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                string cellObj = "Cell_" + i.ToString().PadLeft(2, '0');
                content += "<listCtrls:DGCell Grid.Column=\"" + (i - 1).ToString() + "\" ";
                content += "CellContent=\"{Binding " + cellObj + ".Text}\" ";
                content += "CircleVisibility=\"{Binding " + cellObj + ".ShowAsBall}\" ";
                content += "CircleColor=\"{Binding " + cellObj + ".BallColor}\" ";
                content += "TextColor=\"{Binding " + cellObj + ".ForeColor}\" ";
                content += "BackgroundColor=\"{Binding " + cellObj + ".BackColor}\" ";
                content += "/>";
            }

            string propFormat = "<listCtrls:DGCell Grid.Column=\"{0}\" ";
            propFormat += "CellContent=\"{{Binding {1}.Text}}\" ";
            propFormat += "CircleVisibility=\"{{Binding {1}.ShowAsBall}}\" ";
            propFormat += "CircleColor=\"{{Binding {1}.BallColor}}\" ";
            propFormat += "TextColor=\"{{Binding  {1}.ForeColor}}\" ";
            propFormat += "BackgroundColor=\"{{Binding {1}.BackColor}}\" ";
            propFormat += "/>";

            // properties.
            string[] propNames = new string[]
            {
                "Odd", "Even", "Big", "Small", "Primary", "Composite", 
                "Remain0", "Remain1", "Remain2", 
                "WXJin", "WXMu", "WXShui", "WXHuo", "WXTu"
            };

            for (int i = 16; i < 30; ++i)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                string propName = propNames[i - 16];
                content += string.Format(propFormat, i, propName);
            }

            colDef += "</Grid.ColumnDefinitions>";

            if (true)
            {
                string line1 = "<Line Grid.Column=\"0\" Grid.ColumnSpan=\"16\" Visibility=\"{Binding HasLine1}\" ";
                line1 += "X1=\"{Binding Line1StartX}\" Y1=\"{Binding Line1StartY}\" X2=\"{Binding Line1EndX}\" Y2=\"{Binding Line1EndY}\" ";
                line1 += "Stroke=\"Gray\" StrokeThickness=\"2\" ";
                line1 += "/>";

                content += line1;

                string line2 = "<Line Grid.Column=\"0\" Grid.ColumnSpan=\"16\" Visibility=\"{Binding HasLine2}\" ";
                line2 += "X1=\"{Binding Line2StartX}\" Y1=\"{Binding Line2StartY}\" X2=\"{Binding Line2EndX}\" Y2=\"{Binding Line2EndY}\" ";
                line2 += "Stroke=\"Gray\" StrokeThickness=\"2\" ";
                line2 += "/>";

                content += line2;
            }

            string foot = "</Grid>";
            return header + colDef + content + foot;
        }

        private string BuildGridXamlRedGeneral()
        {
            string header = "<Grid Height=\"25\">";

            string content = "";
            string colDef = "<Grid.ColumnDefinitions>";

            string propFormat = "<listCtrls:DGCell Grid.Column=\"{0}\" ";
            propFormat += "CellContent=\"{{Binding {1}.Text}}\" ";
            propFormat += "CircleVisibility=\"{{Binding {1}.ShowAsBall}}\" ";
            propFormat += "CircleColor=\"{{Binding {1}.BallColor}}\" ";
            propFormat += "TextColor=\"{{Binding  {1}.ForeColor}}\" ";
            propFormat += "BackgroundColor=\"{{Binding {1}.BackColor}}\" ";
            propFormat += "/>";

            // properties.
            string[] propNames = new string[]
            {
                "Continuity", "Odd", "Even", "Big", "Small", "Primary", "Composite", "Remain0", "Remain1", "Remain2", "Div1", "Div2", "Div3"
            };

            colDef += "<ColumnDefinition Width=\"50\"/>";
            content += string.Format(propFormat, 0, "Sum");

            int index = 1;
            foreach (string propName in propNames)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";
                content += string.Format(propFormat, index ++, propName);
            }

            // build external 9 columns
            string extColFormat = "<listCtrls:DGCell Height=\"25\" Width=\"50\" Grid.Column=\"{0}\" ";
            extColFormat += "CellContent=\"{{Binding {1}.Text}}\" ";
            extColFormat += "CircleVisibility=\"{{Binding {1}.ShowAsBall}}\" ";
            extColFormat += "CircleColor=\"{{Binding {1}.BallColor}}\" ";
            extColFormat += "TextColor=\"{{Binding  {1}.ForeColor}}\" ";
            extColFormat += "BackgroundColor=\"{{Binding {1}.BackColor}}\" ";
            extColFormat += "Visible=\"{{Binding {1}.IsVisible}}\" ";
            extColFormat += "/>";

            for (int i = 1; i <= 9; ++i)
            {
                string colName = "Extend" + i.ToString();
                colDef += "<ColumnDefinition Width=\"Auto\"/>";
                content += string.Format(extColFormat, index++, colName);
            }

            colDef += "</Grid.ColumnDefinitions>";

            // Draw lines.
            string line1 = "<Line Grid.Column=\"14\" Grid.ColumnSpan=\"9\" Visibility=\"{Binding HasLine1}\" ";
            line1 += "X1=\"{Binding Line1StartX}\" Y1=\"{Binding Line1StartY}\" X2=\"{Binding Line1EndX}\" Y2=\"{Binding Line1EndY}\" ";
            line1 += "Stroke=\"Gray\" StrokeThickness=\"2\" ";
            line1 += "/>";

            content += line1;

            string line2 = "<Line Grid.Column=\"14\" Grid.ColumnSpan=\"9\" Visibility=\"{Binding HasLine2}\" ";
            line2 += "X1=\"{Binding Line2StartX}\" Y1=\"{Binding Line2StartY}\" X2=\"{Binding Line2EndX}\" Y2=\"{Binding Line2EndY}\" ";
            line2 += "Stroke=\"Gray\" StrokeThickness=\"2\" ";
            line2 += "/>";

            content += line2;

            string foot = "</Grid>";
            return header + colDef + content + foot;
        }

        private string BuildGridXamlRedPosition()
        {
            string header = "<Grid Height=\"25\">";

            string content = "";
            string colDef = "<Grid.ColumnDefinitions>";
            int groupCount = DGRedPositionViewModel.GetGroupCount();
            for (int i = 1; i <= groupCount; ++i)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                string cellObj = "Cell_" + i.ToString().PadLeft(2, '0');
                content += "<listCtrls:DGCell Grid.Column=\"" + (i - 1).ToString() + "\" ";
                content += "CellContent=\"{Binding " + cellObj + ".Text}\" ";
                content += "CircleVisibility=\"{Binding " + cellObj + ".ShowAsBall}\" ";
                content += "CircleColor=\"{Binding " + cellObj + ".BallColor}\" ";
                content += "TextColor=\"{Binding " + cellObj + ".ForeColor}\" ";
                content += "BackgroundColor=\"{Binding " + cellObj + ".BackColor}\" ";
                content += "/>";
            }

            string propFormat = "<listCtrls:DGCell Grid.Column=\"{0}\" ";
            propFormat += "CellContent=\"{{Binding {1}.Text}}\" ";
            propFormat += "CircleVisibility=\"{{Binding {1}.ShowAsBall}}\" ";
            propFormat += "CircleColor=\"{{Binding {1}.BallColor}}\" ";
            propFormat += "TextColor=\"{{Binding  {1}.ForeColor}}\" ";
            propFormat += "BackgroundColor=\"{{Binding {1}.BackColor}}\" ";
            propFormat += "/>";

            // properties.
            string[] propNames = new string[]
            {
                "Odd", "Even", "Primary", "Composite", 
                "Remain0", "Remain1", "Remain2", 
                "WXJin", "WXMu", "WXShui", "WXHuo", "WXTu"
            };

            for (int i = 0; i < 12; ++i)
            {
                colDef += "<ColumnDefinition Width=\"25\"/>";

                string propName = propNames[i];
                content += string.Format(propFormat, i + groupCount, propName);
            }

            colDef += "</Grid.ColumnDefinitions>";

            if (true)
            {
                string line1 = "<Line Grid.Column=\"0\" Grid.ColumnSpan=\"" + groupCount .ToString() + "\" Visibility=\"{Binding HasLine1}\" ";
                line1 += "X1=\"{Binding Line1StartX}\" Y1=\"{Binding Line1StartY}\" X2=\"{Binding Line1EndX}\" Y2=\"{Binding Line1EndY}\" ";
                line1 += "Stroke=\"Gray\" StrokeThickness=\"2\" ";
                line1 += "/>";

                content += line1;

                string line2 = "<Line Grid.Column=\"0\" Grid.ColumnSpan=\"" + groupCount.ToString() + "\" Visibility=\"{Binding HasLine2}\" ";
                line2 += "X1=\"{Binding Line2StartX}\" Y1=\"{Binding Line2StartY}\" X2=\"{Binding Line2EndX}\" Y2=\"{Binding Line2EndY}\" ";
                line2 += "Stroke=\"Gray\" StrokeThickness=\"2\" ";
                line2 += "/>";

                content += line2;
            }

            string foot = "</Grid>";
            return header + colDef + content + foot;
        }

        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (OnViewChangeEventHandler != null)
                OnViewChangeEventHandler(sender, e);
        }
    }
}
