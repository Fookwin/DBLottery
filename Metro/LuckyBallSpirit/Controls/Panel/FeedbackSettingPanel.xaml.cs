using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class FeedbackSettingPanel : UserControl
    {
        public FeedbackSettingPanel()
        {
            this.InitializeComponent();
        }

        private void TB_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            bool bShowDimText = tb.Text == "" || tb.Text == GetDimText(tb);
            if (bShowDimText)
            {
                tb.Text = GetDimText(tb);
                tb.Foreground = new SolidColorBrush(Colors.LightGray);
            }

            UpdateCommitButton();
        }

        private void TB_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (GetDimText(tb) == tb.Text)
            {
                tb.Text = "";
                tb.Foreground = new SolidColorBrush(Colors.DimGray);
            }
        }

        private string GetDimText(TextBox tb)
        {
            if (tb == TB_Name)
                return "怎样称呼您？";
            else if (tb == TB_Email)
                return "怎样联系您？";
            else if (tb == TB_Phone)
                return "跪求~";
            else if (tb == TB_Content)
                return "吐槽吧~";

            return "";
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            // get feedback.
            string name = TB_Name.Text;
            string email = TB_Email.Text;
            string phone = TB_Phone.Text;
            string content = TB_Content.Text;

            LBDataManager.GetInstance().PostFeedback(name, email, phone, content);
            MessageBox.Show("感谢您的支持，我们会认真考虑您的宝贵意见！");
        }

        private void UpdateCommitButton()
        {
            string text = TB_Name.Text;
            if (text == "" || text == GetDimText(TB_Name))
            {
                CommitButton.IsEnabled = false;
                return;
            }

            text = TB_Email.Text;
            if (text == "" || text == GetDimText(TB_Email))
            {
                CommitButton.IsEnabled = false;
                return;
            }

            text = TB_Content.Text;
            if (text == "" || text == GetDimText(TB_Content) || text.Length > 300)
            {
                CommitButton.IsEnabled = false;
                return;
            }

            CommitButton.IsEnabled = true;
        }
    }
}
