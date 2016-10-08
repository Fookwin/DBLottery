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
using LuckyBallsData.Statistics;
using LuckyBallsSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class DGIndexPanel : UserControl
    {
        public delegate void ViewChangeEventHandler();
        public event ViewChangeEventHandler OnViewChangeEventHandler;

        private ScrollViewer _listScrollViewer = null;

        public DGIndexPanel()
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

        public void SetContent(List<Lottery> _content)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                ListBoxCtrl.ItemsSource = _content;
                ListBoxCtrl.ScrollIntoView(_content.Last());
            });
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
