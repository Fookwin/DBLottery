using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LuckyBallsSpirit.Controls
{
    public partial class ValuePickerFlyoutCombo : UserControl
    {
        public delegate void ValueConfirmedEventHandler(List<string> selected);
        public event ValueConfirmedEventHandler ValueConfirmed = null;

        private static ValuePickerFlyoutCombo _uniqueInstance = null;

        private bool m_bCloseOnTap = true;

        public static ValuePickerFlyoutCombo UniqueInstance
        {
            get
            {
                if (_uniqueInstance == null)
                    _uniqueInstance = new ValuePickerFlyoutCombo();

                return _uniqueInstance;
            }
        }

        private ValuePickerFlyoutCombo()
        {
            InitializeComponent();
        }

        public void Show(string title, List<string> values, bool multipleSelection, bool closeOnTap, List<int> selectedItems)
        {
            // add close button.

            Title.Text = title;
            ExpandList.ItemsSource = values;
            m_bCloseOnTap = !multipleSelection && closeOnTap; // for mutiple selection, don't allow close on tap.

            OKButton.Visibility = m_bCloseOnTap ? Visibility.Collapsed : Visibility.Visible;

            if (multipleSelection)
            {
                ExpandList.SelectionMode = SelectionMode.Multiple;

                ExpandList.SelectedItems.Clear();
                foreach (int index in selectedItems)
                {
                    ExpandList.SelectedItems.Add(index);
                }
            }
            else
            {
                ExpandList.SelectionMode = SelectionMode.Single;
                if (selectedItems.Count > 0)
                    ExpandList.SelectedIndex = selectedItems[0];
            }

            PhoneApplicationFrame appFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            double ScreenW = appFrame.ActualWidth;
            double ScreenH = appFrame.ActualHeight;

            OuterBorder.Height = ScreenH;
            OuterBorder.Width = ScreenW;

            int maxLength = 0;
            foreach (string str in values)
            {
                if (str.Length > maxLength)
                    maxLength = str.Length;
            }

            double panelW = Math.Max(maxLength * 30 + 20, 200);
            double panelH = Math.Min(values.Count * 30 + 60, 400);

            //InnerBorder.Margin = new Thickness((ScreenW - panelW) / 2, (ScreenH - panelH) / 2, 0, 0);

            flyoutPopup.IsOpen = true;
            ShowPopupStory.Begin();
        }

        public void Hide()
        {
            flyoutPopup.Opacity = 0;
            flyoutPopup.IsOpen = false;

            ValueConfirmed = null; // disconnect with all handler.
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValueConfirmed != null)
            {
                ValueConfirmed(ExpandList.SelectionMode == SelectionMode.Single ? 
                    new List<string>() { ExpandList.SelectedItem as string } : ExpandList.SelectedItems as List<string>);
            }

            Hide();
        }

        private void ExpandList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (m_bCloseOnTap)
            {
                if (ValueConfirmed != null)
                {
                    ValueConfirmed(new List<string>() { ExpandList.SelectedItem as string });
                }

                Hide();
            }
        }

        private void CloseButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Hide();
        }
    }
}
