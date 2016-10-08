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
using LuckyBallsData.Statistics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls.StatusDataGrid
{
    public sealed partial class StatusDataGrid : UserControl
    {
        private bool _initialized = false;

        public StatusDataGrid()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                DataGridFixedColumns.OnViewChangeEventHandler += FixedColumnsScroll_ViewChanged;
                DataGridContentColumns.OnViewChangeEventHandler += ContentViewer_ViewChanged;
            };
        }

        private void RebuildContext(int displayCount)
        {
            List<Lottery> lots = LuckyBallSpirit.DataModel.LBDataManager.Instance().History.Lotteries;
            int startIndex = lots.Count - displayCount;
            var dispLots = lots.GetRange(startIndex, displayCount);

            DataGridFixedColumns.SetContent(dispLots);
            DataGridContentColumns.SetContent(dispLots);

            _initialized = true;
        }

        public void Update(StatusOptions options, OptionChangeEnum optionChangeArg)
        {
            if (!_initialized || optionChangeArg == OptionChangeEnum.ViewCountChanged)
                RebuildContext(options.ViewIssueCount);

            WaitingProgressRing.Visibility = Visibility.Visible;
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
            {
                DataGridContentHeader.Update(options, optionChangeArg);
                DataGridContentColumns.Update(options, optionChangeArg);

                WaitingProgressRing.Visibility = Visibility.Collapsed;
            }); 
        }

        private void FixedColumnsScroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (DataGridContentColumns.VerticalOffset != DataGridFixedColumns.VerticalOffset)
                DataGridContentColumns.VerticalOffset = DataGridFixedColumns.VerticalOffset;
        }

        private void ContentViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (DataGridContentColumns.VerticalOffset != DataGridFixedColumns.VerticalOffset)
                DataGridFixedColumns.VerticalOffset = DataGridContentColumns.VerticalOffset;
        }
    }
}
