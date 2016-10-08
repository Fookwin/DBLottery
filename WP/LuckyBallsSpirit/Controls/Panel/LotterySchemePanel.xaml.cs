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
using LuckyBallsData.Util;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class SchemeSelectionPanel : UserControl
    {
        private List<Scheme> _selection = null;
        public delegate void SelectionChangedEventHandler(RefreshActionEnum action);
        public event SelectionChangedEventHandler SelectionChanged;
        private bool _intitialized = false;
        private int _remainCount = 0;

        public SchemeSelectionPanel()
        {
            this.InitializeComponent();
            this.Loaded += delegate
            {
                _intitialized = true;
            };
        }

        public void SetTarget(List<Scheme> target)
        {
            _selection = target;

            Refresh();
        }

        public void Refresh()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                LV_SelectedSchemes.ItemsSource = null;
                LV_SelectedSchemes.ItemsSource = _selection;

                if ((_remainCount == 0 || _remainCount >= _selection.Count) && _selection.Count > 0)
                {
                    _remainCount = (_selection.Count - 1) / 2 + 1;
                    TB_RemainCount.Value = _remainCount;
                    TB_RemainCount.LowLimit = 1;
                    TB_RemainCount.HighLimit = _selection.Count - 1;
                }

                if (_remainCount < _selection.Count)
                    BT_RadomRemain.IsEnabled = true;

                BT_Rollback.IsEnabled = false;
            });
        }

        private void BT_DeleteScheme_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Scheme selected = btn.DataContext as Scheme;
            if (selected != null)
            {
                _selection.Remove(selected);

                this.Dispatcher.BeginInvoke(() =>
                {
                    LV_SelectedSchemes.ItemsSource = null;
                    LV_SelectedSchemes.ItemsSource = _selection;
                });

                // inform that the selection has been modified.
                if (SelectionChanged != null)
                {
                    SelectionChanged(RefreshActionEnum.RefreshCountOnlyAction);
                }

                BT_Rollback.IsEnabled = true;
            }
        }

        private void BT_RadomRemain_Click(object sender, RoutedEventArgs e)
        {
            SelectUtil.RadomRemain(ref _selection, _remainCount);

            this.Dispatcher.BeginInvoke(() =>
            {
                LV_SelectedSchemes.ItemsSource = null;
                LV_SelectedSchemes.ItemsSource = _selection;
            });

            // inform that the selection has been modified.
            if (SelectionChanged != null)
            {
                SelectionChanged(RefreshActionEnum.RefreshCountOnlyAction);
            }

            BT_Rollback.IsEnabled = true;
        }

        private void TB_RemainCount_TextChanged(object sender, ValueChangeArgs e)
        {
            if (_intitialized)
            {
                int remainCount = Convert.ToInt32(Math.Round(TB_RemainCount.Value));

                if (remainCount <= 0 || remainCount >= _selection.Count)
                {
                    BT_RadomRemain.IsEnabled = false;
                }
                else
                {
                    _remainCount = remainCount;
                    BT_RadomRemain.IsEnabled = true;
                }
            }
        }

        private void BT_Rollback_Click(object sender, RoutedEventArgs e)
        {
            // inform that the selection has been modified.
            if (SelectionChanged != null)
            {
                SelectionChanged(RefreshActionEnum.RecalcualteAction);
            }
        }     
    }
}
