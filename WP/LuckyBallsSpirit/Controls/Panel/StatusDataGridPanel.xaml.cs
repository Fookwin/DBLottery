using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// 
//
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using LuckyBallsSpirit.DataModel;
using LuckyBallsData.Statistics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class StatusDataGridPanel : UserControl
    {
        private bool _initialized = false;

        public StatusDataGridPanel()
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
            List<Lottery> lots = LuckyBallsSpirit.DataModel.LBDataManager.Instance().History.Lotteries;
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
            this.Dispatcher.BeginInvoke(() =>
            {
                DataGridContentHeader.Update(options, optionChangeArg);
                DataGridContentColumns.Update(options, optionChangeArg);

                WaitingProgressRing.Visibility = Visibility.Collapsed;
            }); 
        }

        private void FixedColumnsScroll_ViewChanged()
        {
            if (DataGridContentColumns.VerticalOffset != DataGridFixedColumns.VerticalOffset)
                DataGridContentColumns.VerticalOffset = DataGridFixedColumns.VerticalOffset;
        }

        private void ContentViewer_ViewChanged()
        {
            if (DataGridContentColumns.VerticalOffset != DataGridFixedColumns.VerticalOffset)
                DataGridFixedColumns.VerticalOffset = DataGridContentColumns.VerticalOffset;
        }
    }
}
