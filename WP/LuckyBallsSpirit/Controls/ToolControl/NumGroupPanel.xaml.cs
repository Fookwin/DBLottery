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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class NumGroupPanel : UserControl
    {
        public NumGroupPanel()
        {
            this.InitializeComponent();
        }

        // Numbers property
        private static void OnNumbersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumGroupPanel ctrl = d as NumGroupPanel;
            if (ctrl != null)
            {
                List<SimpleNumberBallViewModel> val = e.NewValue as List<SimpleNumberBallViewModel>;
                ctrl.UpdateNumbers(val);
            }
        }

        private void UpdateNumbers(List<SimpleNumberBallViewModel> value)
        {
            NumGrid.ItemsSource = value;
        }

        public static readonly DependencyProperty NumbersProperty = DependencyProperty.Register(
                        "Numbers", typeof(List<SimpleNumberBallViewModel>), typeof(NumGroupPanel),
                        new PropertyMetadata(null,
                        new PropertyChangedCallback(OnNumbersPropertyChanged)));

        public List<SimpleNumberBallViewModel> Numbers
        {
            get { return (List<SimpleNumberBallViewModel>)GetValue(NumbersProperty); }
            set { SetValue(NumbersProperty, value); }
        }

        // Title property
        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumGroupPanel ctrl = d as NumGroupPanel;
            if (ctrl != null)
            {
                string val = e.NewValue as string;
                ctrl.UpdateTitle(val);
            }
        }

        private void UpdateTitle(string value)
        {
            TitleTB.Text = value;
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
                        "Title", typeof(string), typeof(NumGroupPanel),
                        new PropertyMetadata("",
                        new PropertyChangedCallback(OnTitlePropertyChanged)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // TitleColor property
        private static void OnTitleColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumGroupPanel ctrl = d as NumGroupPanel;
            if (ctrl != null)
            {
                Color? val = e.NewValue as Color?;
                ctrl.UpdateTitleColor(val.Value);
            }
        }

        private void UpdateTitleColor(Color value)
        {
            Color secondaryColor = new Color() { A = 150, R = value.R, G = value.G, B = value.B };
            //Color titleColor = new Color() { A = 50, R = value.R, G = value.G, B = value.B };

            //SolidColorBrush stbrush = TitleCircle.Stroke as SolidColorBrush;
            //stbrush.Color = titleColor;

            //SolidColorBrush tlBrush = TitleTB.Foreground as SolidColorBrush;
            //tlBrush.Color = titleColor;

            BorderBackColorStop.Color = value;
            BorderBackColorStart.Color = secondaryColor;
        }

        public static readonly DependencyProperty TitleColorProperty = DependencyProperty.Register(
                        "TitleColor", typeof(Color), typeof(NumGroupPanel),
                        new PropertyMetadata(ColorPicker.DarkGray,
                        new PropertyChangedCallback(OnTitleColorPropertyChanged)));

        public Color TitleColor
        {
            get { return (Color)GetValue(TitleColorProperty); }
            set { SetValue(TitleColorProperty, value); }
        }

        // Discription property
        private static void OnDiscriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumGroupPanel ctrl = d as NumGroupPanel;
            if (ctrl != null)
            {
                string val = e.NewValue as string;
                ctrl.UpdateDiscription(val);
            }
        }

        private void UpdateDiscription(string value)
        {
            GroupDisp.Text = value;
        }

        public static readonly DependencyProperty DiscriptionProperty = DependencyProperty.Register(
                        "Discription", typeof(string), typeof(NumGroupPanel),
                        new PropertyMetadata("",
                        new PropertyChangedCallback(OnDiscriptionPropertyChanged)));

        public string Discription
        {
            get { return (string)GetValue(DiscriptionProperty); }
            set { SetValue(DiscriptionProperty, value); }
        }
    }
}
