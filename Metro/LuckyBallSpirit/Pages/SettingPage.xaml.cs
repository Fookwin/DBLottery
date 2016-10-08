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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LuckyBallSpirit.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : LuckyBallSpirit.Common.LayoutAwarePage
    {
        public SettingPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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
            string command = navigationParameter as string;
            if (command != null)
            {
                if (command == "Donation")
                {
                    FeedbackView.Visibility = Visibility.Collapsed;
                    SystemSettingView.Visibility = Visibility.Collapsed;
                    DonationView.Visibility = Visibility.Visible;
                    AboutView.Visibility = Visibility.Collapsed;
                }
            }
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

        private void Command_List_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource == SuggestionCmmand)
            {
                FeedbackView.Visibility = Visibility.Visible;
                SystemSettingView.Visibility = Visibility.Collapsed;
                DonationView.Visibility = Visibility.Collapsed;
                AboutView.Visibility = Visibility.Collapsed;
            }
            else if (e.OriginalSource == SystemCmmand)
            {
                FeedbackView.Visibility = Visibility.Collapsed;
                SystemSettingView.Visibility = Visibility.Visible;
                DonationView.Visibility = Visibility.Collapsed;
                AboutView.Visibility = Visibility.Collapsed;
            }
            else if (e.OriginalSource == DonateCmmand)
            {
                FeedbackView.Visibility = Visibility.Collapsed;
                SystemSettingView.Visibility = Visibility.Collapsed;
                DonationView.Visibility = Visibility.Visible;
                AboutView.Visibility = Visibility.Collapsed;
            }
            else if (e.OriginalSource == AboutCmmand)
            {
                FeedbackView.Visibility = Visibility.Collapsed;
                SystemSettingView.Visibility = Visibility.Collapsed;
                DonationView.Visibility = Visibility.Collapsed;
                AboutView.Visibility = Visibility.Visible;
            }
        }

        private void CleanLocalCacheButton_Click(object sender, RoutedEventArgs e)
        {

        }       
    }
}
