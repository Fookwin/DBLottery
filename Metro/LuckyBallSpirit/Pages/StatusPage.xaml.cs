using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LuckyBallSpirit.DataModel;
using LuckyBallsData.Statistics;
using LuckyBallSpirit.Controls;
using LuckyBallSpirit.ViewModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace LuckyBallSpirit.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class StatusPage : LuckyBallSpirit.Common.LayoutAwarePage
    {
        private bool _initialized = false;
        private static StatusOptions _option = new StatusOptions(); // keep the option in global to remember the status.

        public StatusPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            this.Loaded += delegate
            {
                // Initilize the foot panel.
                pageFootPanel.LaunchTimeDown();

                // show float panel.
                LotterySelectionFloatPanel.Instance().Show(LotterySelectionFloatPanel.LSDisplayModeEnum.Experss);

                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate()
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

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            pageFootPanel.ActiveCommand = PageFootPanel.PageIndexEnum.StatusPage;
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
