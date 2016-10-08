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
using LuckyBallsSpirit.DataModel;
using LuckyBallsData.Statistics;
using LuckyBallsSpirit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LuckyBallsSpirit.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class LotteryStatusPage : PhoneApplicationPage
    {
        private bool _initialized = false;
        private static StatusOptions _option = new StatusOptions(); // keep the option in global to remember the status.

        public LotteryStatusPage()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {                
                this.Dispatcher.BeginInvoke(() =>
                {
                    // init the active category.
                    switch (_option.Category)
                    {
                        case StatusCategory.RedGeneral:
                            RB_CAT_RedGen.IsChecked = true;
                            break;
                        case StatusCategory.RedDivision:
                            RB_CAT_RedDiv.IsChecked = true;
                            break;
                        case StatusCategory.RedPosition:
                            RB_CAT_RedPos.IsChecked = true;
                            break;
                        case StatusCategory.BlueGeneral:
                            RB_CAT_BlueGen.IsChecked = true;
                            break;
                        case StatusCategory.BlueSpan:
                            RB_CAT_BlueDiff.IsChecked = true;
                            break;
                    }

                    SetCategory(_option.Category, true);
                    StatusOptionPanel.OnOptionsChangeEventHandler += OnOptionsChanged;

                    _initialized = true;
                });            
            };
        }

        private void Categories_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_initialized)
            {
                if (RB_CAT_RedGen.IsChecked.Value)
                    SetCategory(StatusCategory.RedGeneral, false);
                else if (RB_CAT_RedDiv.IsChecked.Value)
                    SetCategory(StatusCategory.RedDivision, false);
                else if (RB_CAT_RedPos.IsChecked.Value)
                    SetCategory(StatusCategory.RedPosition, false);
                else if (RB_CAT_BlueGen.IsChecked.Value)
                    SetCategory(StatusCategory.BlueGeneral, false);
                else if (RB_CAT_BlueDiff.IsChecked.Value)
                    SetCategory(StatusCategory.BlueSpan, false);
            }
        }

        public void SetCategory(StatusCategory category, bool init)
        {
            // Switch the option panel.
            StatusOptionPanel.SwitchPanel(category, init ? _option : null);

            _option.Category = category;
            if (!init)
            {
                // Get the option from the panel.
                _option.SubCategory = StatusOptionPanel.SubCategory;
                _option.ViewOption = StatusOptionPanel.ViewOption;
                _option.ViewIssueCount = StatusOptionPanel.ViewIssueCount;
            }

            // Switch the content panel.
            StatusGridPanel.Update(_option, OptionChangeEnum.CategoryChanged);
        }

        public void OnOptionsChanged(OptionChangeEnum arg)
        {
            // update the option.
            switch (arg)
            {
                case OptionChangeEnum.SubCategoryChanged:
                    _option.SubCategory = StatusOptionPanel.SubCategory;
                    break;
                case OptionChangeEnum.ViewOptionChanged:
                    _option.ViewOption = StatusOptionPanel.ViewOption;
                    break;
                case OptionChangeEnum.ViewCountChanged:
                    _option.ViewIssueCount = StatusOptionPanel.ViewIssueCount;
                    break;
            }

            // Switch the content panel.
            StatusGridPanel.Update(_option, arg);
        }
    }
}
