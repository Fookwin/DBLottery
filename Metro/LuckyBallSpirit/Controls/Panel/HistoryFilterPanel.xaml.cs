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
using LuckyBallsData.Selection;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class HistoryFilterPanel : UserControl, FilterPanelBase
    {
        private bool _initilized = false;
        private HistoryDuplicateConstraint _constraint = new HistoryDuplicateConstraint();

        public HistoryFilterPanel()
        {
            this.InitializeComponent();
            this.Loaded += delegate
            {
                _initilized = true;
            };
        }

        public Constraint Constraint
        {
            get
            {
                return _constraint;
            }
        }

        public void SetConstraint(Constraint _constr)
        {
            _constraint = _constr as HistoryDuplicateConstraint;
            if (_constraint != null)
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
                {
                    CB_TestIssueCount.SelectedIndex = RefCountToIndex(_constraint.ReferenceCount);
                    CB_FilterCondition.SelectedIndex = 6 - _constraint.ExcludeCondition;
                });
            }
        }

        private int RefCountToIndex(int count)
        {
            if (count < 0)
                return 0; // select all;

            if (count == 10)
                return 1;
            else if (count == 30)
                return 2;
            else if (count == 50)
                return 3;
            else if (count == 100)
                return 4;
            else if (count == 200)
                return 5;
            else if (count == 500)
                return 6;
            else if (count == 1000)
                return 7;
            else
                return 0;
        }

        private int IndexToRefCount(int index)
        {
            switch (index)
            {
                case 0:
                    return -1;
                case 1:
                    return 10;
                case 2:
                    return 30;
                case 3:
                    return 50;
                case 4:
                    return 100;
                case 5:
                    return 200;
                case 6:
                    return 500;
                case 7:
                    return 1000;
                default:
                    return -1;
            }
        }

        private void CB_TestIssueCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initilized)
            {
                _constraint.ReferenceCount = IndexToRefCount(CB_TestIssueCount.SelectedIndex);
            }
        }

        private void CB_FilterCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initilized)
            {
                _constraint.ExcludeCondition = 6 - CB_FilterCondition.SelectedIndex;
            }
        }
    }
}
