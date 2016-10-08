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
using LuckyBallsData.Statistics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls.ToolControl.StatusDataGrid
{
    public sealed partial class DGIndexPanel : UserControl
    {
        public delegate void ViewChangeEventHandler(object sender, ScrollViewerViewChangedEventArgs e);
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
                    _listScrollViewer.ViewChanged += OnViewChanged;
                }
            };
        }

        public void SetContent(List<Lottery> _content)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
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

        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (OnViewChangeEventHandler != null)
                OnViewChangeEventHandler(sender, e);
        }
    }
}
