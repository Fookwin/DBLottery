using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
 

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Navigation;
using LuckyBallsData.Statistics;
using LuckyBallsSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class DGContentPanel : UserControl
    {
        public delegate void ViewChangeEventHandler();
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
                    ScrollBar vBar = ((FrameworkElement)VisualTreeHelper.GetChild(_listScrollViewer, 0)).FindName("VerticalScrollBar") as ScrollBar;
                    vBar.ValueChanged += OnViewChanged;
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
                            ListBoxCtrl.ItemTemplate = this.Resources["RedDivisionTemplate"] as DataTemplate;
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
                            ListBoxCtrl.ItemTemplate = this.Resources["BlueGeneralTemplate"] as DataTemplate;
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
                            ListBoxCtrl.ItemTemplate = this.Resources["BlueSpanTemplate"] as DataTemplate;
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
                            ListBoxCtrl.ItemTemplate = this.Resources["RedGeneralTemplate"] as DataTemplate;
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
                            ListBoxCtrl.ItemTemplate = this.Resources["RedPositionTemplate"] as DataTemplate;
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
                if (_listScrollViewer.VerticalOffset != value)
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
                if (_listScrollViewer.HorizontalOffset != value)
                    _listScrollViewer.ScrollToHorizontalOffset(value);
            }
        }

        private void OnViewChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OnViewChangeEventHandler != null)
                OnViewChangeEventHandler();
        }
    }
}
