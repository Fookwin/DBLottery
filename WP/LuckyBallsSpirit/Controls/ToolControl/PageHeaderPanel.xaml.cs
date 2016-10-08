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
    public partial class PageHeaderPanel : UserControl
    {
        private bool _showContextMenuButton = false;
        private string _title = "";
        private string _extendMenuText = "";

        public delegate void ExtendButtonClickedEventHandler();
        public event ExtendButtonClickedEventHandler OnExtendMenuClicked = null;

        public PageHeaderPanel()
        {
            InitializeComponent();

            Loaded += delegate
            {
                if (App.RootFrame.CanGoBack)
                {
                    BackIcon.Visibility = Visibility.Visible;
                }
            };
        }

        public bool DisplayContextMenuButton
        {
            get
            {
                return _showContextMenuButton;
            }
            set
            {
                if (_showContextMenuButton != value)
                {
                    _showContextMenuButton = value;

                    ExtendMenuBorder.Visibility = _showContextMenuButton ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        public string ExtendMenuText
        {
            get
            {
                return _extendMenuText;
            }
            set
            {
                if (_extendMenuText != value)
                {
                    _extendMenuText = value;

                    ExtendMenuButton.Content = _extendMenuText;
                }
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;

                    TitleText.Text = _title;
                }
            }
        }

        private void ExtendMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnExtendMenuClicked != null)
            {
                OnExtendMenuClicked();
            }
        }

        private void BackIcon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            App.RootFrame.GoBack();
        }
    }
}
