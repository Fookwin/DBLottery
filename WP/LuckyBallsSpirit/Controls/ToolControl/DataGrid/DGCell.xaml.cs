using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
// 
//
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class DGCell : UserControl
    {
        public DGCell()
        {
            this.InitializeComponent();
        }

        private static void OnCircleColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                Color? val = e.NewValue as Color?;
                if (val != null)
                {
                    ctrl.UpdateCircleColor(val.Value);
                }
            }
        }

        private static void OnCircleVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                bool? val = e.NewValue as bool?;
                if (val != null)
                {
                    ctrl.UpdateCircleVisiblity(val.Value);
                }
            }
        }

        private static void OnBackgroundColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                Color? val = e.NewValue as Color?;
                if (val != null)
                {
                    ctrl.UpdateBackgroundColor(val.Value);
                }
            }
        }

        private static void OnBorderColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                Color? val = e.NewValue as Color?;
                if (val != null)
                {
                    ctrl.UpdateBorderColor(val.Value);
                }
            }
        }

        private static void OnBorderWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                double? val = e.NewValue as double?;
                ctrl.UpdateBorderWidth(val.Value);
            }
        }

        private static void OnCellHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                double? val = e.NewValue as double?;
                ctrl.UpdateCellHeight(val.Value);
            }
        }

        private static void OnCellContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                string val = e.NewValue as string;
                ctrl.UpdateCellContent(val);
            }
        }

        private static void OnTextFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                int? val = e.NewValue as int?;
                if (val != null)
                {
                    ctrl.UpdateTextFontSize(val.Value);
                }
            }
        }

        private static void OnTextColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                Color? val = e.NewValue as Color?;
                if (val != null)
                {
                    ctrl.UpdateTextColor(val.Value);
                }
            }
        }

        private static void OnVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DGCell ctrl = d as DGCell;
            if (ctrl != null)
            {
                bool? val = e.NewValue as bool?;
                if (val != null)
                {
                    ctrl.UpdateVisible(val.Value);
                }
            }
        }

        private void UpdateCircleColor(Color value)
        {
            CellCircleBk.Fill = new SolidColorBrush(value);
        }

        private void UpdateCircleVisiblity(bool value)
        {
            CellCircleBk.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateBackgroundColor(Color value)
        {
            CellBorder.Background = new SolidColorBrush(value);
        }

        private void UpdateBorderColor(Color value)
        {
            CellBorder.BorderBrush = new SolidColorBrush(value);
        }

        private void UpdateBorderWidth(double value)
        {
            CellBorder.BorderThickness = new Thickness(value);
        }

        private void UpdateCellHeight(double value)
        {
            CellBorder.Height = value;
            CellCircleBk.Height = CellCircleBk.Width = value * 0.8;
        }

        private void UpdateCellContent(string value)
        {
            CellTextBlock.Text = value;
        }

        private void UpdateTextFontSize(int value)
        {
            CellTextBlock.FontSize = value;
        }

        private void UpdateTextColor(Color value)
        {
            CellTextBlock.Foreground = new SolidColorBrush(value);
        }

        private void UpdateVisible(bool value)
        {
            this.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor", typeof(Color), typeof(DGCell),
            new PropertyMetadata(Colors.White,
            new PropertyChangedCallback(OnBackgroundColorPropertyChanged)));

        public static readonly DependencyProperty CircleColorProperty = DependencyProperty.Register(
            "CircleColor", typeof(Color), typeof(DGCell),
            new PropertyMetadata(Colors.White,
            new PropertyChangedCallback(OnCircleColorPropertyChanged)));

        public static readonly DependencyProperty CircleVisibilityProperty = DependencyProperty.Register(
            "CircleVisibility", typeof(Boolean), typeof(DGCell),
            new PropertyMetadata(false,
            new PropertyChangedCallback(OnCircleVisibilityPropertyChanged)));

        public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register(
            "BorderColor", typeof(Color), typeof(DGCell),
            new PropertyMetadata(Colors.White,
            new PropertyChangedCallback(OnBorderColorPropertyChanged)));

        public static readonly DependencyProperty BorderWidthProperty = DependencyProperty.Register(
            "BorderWidth", typeof(double), typeof(DGCell),
            new PropertyMetadata(1.0,
            new PropertyChangedCallback(OnBorderWidthPropertyChanged)));

        public static readonly DependencyProperty CellContentProperty = DependencyProperty.Register(
            "CellContent", typeof(string), typeof(DGCell),
            new PropertyMetadata("",
            new PropertyChangedCallback(OnCellContentPropertyChanged)));

        public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register(
            "TextFontSize", typeof(int), typeof(DGCell),
            new PropertyMetadata(15,
            new PropertyChangedCallback(OnTextFontSizePropertyChanged)));

       public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
            "TextColor", typeof(Color), typeof(DGCell),
            new PropertyMetadata(Colors.Black,
            new PropertyChangedCallback(OnTextColorPropertyChanged)));

       public static readonly DependencyProperty CellHeightProperty = DependencyProperty.Register(
            "CellHeight", typeof(double), typeof(DGCell),
            new PropertyMetadata(1.0,
            new PropertyChangedCallback(OnCellHeightPropertyChanged)));

       public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register(
            "Visible", typeof(bool), typeof(DGCell),
            new PropertyMetadata(true,
            new PropertyChangedCallback(OnVisiblePropertyChanged)));
      
        public Color CircleColor
        {
            get { return (Color)GetValue(CircleColorProperty); }
            set { SetValue(CircleColorProperty, value); }
        }

        public bool CircleVisibility
        {
            get { return (bool)GetValue(CircleVisibilityProperty); }
            set { SetValue(CircleVisibilityProperty, value); }
        }

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public string CellContent
        {
            get { return (string)GetValue(CellContentProperty); }
            set { SetValue(CellContentProperty, value); }
        }

        public int TextFontSize
        {
            get { return (int)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public double CellHeight
        {
            get { return (double)GetValue(CellHeightProperty); }
            set { SetValue(CellHeightProperty, value); }
        }

        public bool Visible
        {
            get { return (bool)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }
    }
}
