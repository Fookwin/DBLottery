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
using LuckyBallsData.Selection;
using LuckyBallsSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class FilterHistoryPanel : UserControl, FilterPanelBase
    {
        private HistoryDuplicateConstraint _constraint = new HistoryDuplicateConstraint();

        public FilterHistoryPanel()
        {
            this.InitializeComponent();
            this.Loaded += delegate
            {
                // Init test issue count combo box.
                CB_TestIssueCount.Values = new List<string> {
                    "所有开奖", "最近10期", "最近30期", "最近50期", "最近100期", "最近200期", "最近500期", "最近1000期"
                };
                CB_TestIssueCount.ValueIndex = 0;

                CB_FilterCondition.Values = new List<string> {
                    "六个红球相同", "五个以上红球相同", "四个以上红球相同", "三个以上红球相同", "两个以上红球相同"
                };
                CB_FilterCondition.ValueIndex = 0;

                CB_TestIssueCount.ValueChanged += TestIssueCount_ValueChanged;
                CB_FilterCondition.ValueChanged += FilterCondition_ValueChanged;
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
                this.Dispatcher.BeginInvoke(() =>
                {
                    CB_TestIssueCount.ValueIndex = RefCountToIndex(_constraint.ReferenceCount);
                    CB_FilterCondition.ValueIndex = 6 - _constraint.ExcludeCondition;
                });
            }
        }

        public void OnNavigatedTo(NavigationContext context)
        {

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

        private void TestIssueCount_ValueChanged()
        {
            _constraint.ReferenceCount = IndexToRefCount(CB_TestIssueCount.ValueIndex);
        }

        private void FilterCondition_ValueChanged()
        {
            _constraint.ExcludeCondition = 6 - CB_FilterCondition.ValueIndex;
        }
    }
}
