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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class NumButton : UserControl
    {
        public enum SelectModeEnum
        {
            NotSelectable = 0,
            DuplexSelectable = 1,
            TriplexSelectableWithHighlight = 2,
            TriplexSelectableWithGrayed = 3
        }

        public enum SelectStatusEnum
        {
            NotSelected = 0,
            Selected = 1,
            Highlighted = 2,
            Grayed = 3
        }

        public class NumStateChangeArg
        {
            public int number = -1;
            public SelectStatusEnum fromState = SelectStatusEnum.NotSelected;
            public SelectStatusEnum toState = SelectStatusEnum.NotSelected;
        }        

        public delegate void SelectionChangedEventHandler(object sender, NumButton.NumStateChangeArg e);
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Identifies the SelectMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectModeProperty =
            DependencyProperty.Register(
                "SelectMode",
                typeof(SelectModeEnum),
                typeof(NumButton),
                new PropertyMetadata(SelectModeEnum.DuplexSelectable, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Identifies the Selected dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectStatusProperty =
            DependencyProperty.Register(
                "SelectStatus",
                typeof(SelectStatusEnum),
                typeof(NumButton),
                new PropertyMetadata(SelectStatusEnum.NotSelected, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Identifies the Num dependency property.
        /// </summary>
        public static readonly DependencyProperty NumProperty =
            DependencyProperty.Register(
                "Num",
                typeof(string),
                typeof(NumButton),
                new PropertyMetadata("00", new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Identifies the PrimaryColor dependency property.
        /// </summary>
        public static readonly DependencyProperty PrimaryColorProperty =
            DependencyProperty.Register(
                "PrimaryColor",
                typeof(Color),
                typeof(NumButton),
                new PropertyMetadata(Colors.Black, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Identifies the SecondaryColor dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryColorProperty =
            DependencyProperty.Register(
                "SecondaryColor",
                typeof(Color),
                typeof(NumButton),
                new PropertyMetadata(Colors.Gray, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Identifies the TipColor dependency property.
        /// </summary>
        public static readonly DependencyProperty TipColorProperty =
            DependencyProperty.Register(
                "TipColor",
                typeof(Color),
                typeof(NumButton),
                new PropertyMetadata(Colors.Gray, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Identifies the Description dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                "Description",
                typeof(string),
                typeof(NumButton),
                new PropertyMetadata("0", new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Identifies the ShowDescription dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowDescriptionProperty =
            DependencyProperty.Register(
                "ShowDescription",
                typeof(Visibility),
                typeof(NumButton),
                new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnPropertyChanged)));

        public SelectStatusEnum SelectStatus
        {
            get { return (SelectStatusEnum)GetValue(SelectStatusProperty); }
            set 
            {
                SetValue(SelectStatusProperty, value);
                Update();
            }
        }

        public string Num 
        {
            get { return (string)GetValue(NumProperty); }
            set 
            { 
                SetValue(NumProperty, value);
                Update();
            }
        }

        public Color PrimaryColor
        {
            get { return (Color)GetValue(PrimaryColorProperty); }
            set { SetValue(PrimaryColorProperty, value); }
        }

        public Color TipColor
        {
            get { return (Color)GetValue(TipColorProperty); }
            set { SetValue(TipColorProperty, value); }
        }

        public Color SecondaryColor 
        {
            get { return (Color)GetValue(SecondaryColorProperty); }
            set { SetValue(SecondaryColorProperty, value); }
        }

        public Visibility ShowDescription
        {
            get { return (Visibility)GetValue(ShowDescriptionProperty); }
            set { SetValue(ShowDescriptionProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public SelectModeEnum SelectMode
        {
            get { return (SelectModeEnum)GetValue(SelectModeProperty); }
            set { SetValue(SelectModeProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumButton btn = d as NumButton;
            if (btn != null)
            {
                btn.Update();
            }
        }

        private bool PreHighlighted = false;
        
        public NumButton()
        {
            this.InitializeComponent();
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            SelectStatusEnum previous = SelectStatus;
            SelectStatusEnum next = SelectStatus;

            if (SelectMode == NumButton.SelectModeEnum.NotSelectable)
            {
                return; // do nothing.
            }
            else if (SelectMode == NumButton.SelectModeEnum.DuplexSelectable)
            {
                // notselected => selected => notselected
                next = previous == NumButton.SelectStatusEnum.NotSelected ? NumButton.SelectStatusEnum.Selected : NumButton.SelectStatusEnum.NotSelected; 
            }
            else if (SelectMode == NumButton.SelectModeEnum.TriplexSelectableWithHighlight)
            {
                // notselected => selected => highlighted => notselected
                switch (previous)
                {
                    case NumButton.SelectStatusEnum.NotSelected:
                        next = NumButton.SelectStatusEnum.Selected;
                        break;
                    case NumButton.SelectStatusEnum.Selected:
                        next = NumButton.SelectStatusEnum.Highlighted;
                        break;
                    case NumButton.SelectStatusEnum.Highlighted:
                        next = NumButton.SelectStatusEnum.NotSelected;
                        break;
                    default:
                        throw new Exception("Invalid select status in current select mode.");
                }
            }
            else if (SelectMode == NumButton.SelectModeEnum.TriplexSelectableWithGrayed)
            {
                // notselected => selected => grayed => notselected
                switch (previous)
                {
                    case NumButton.SelectStatusEnum.NotSelected:
                        next = NumButton.SelectStatusEnum.Selected;
                        break;
                    case NumButton.SelectStatusEnum.Selected:
                        next = NumButton.SelectStatusEnum.Grayed;
                        break;
                    case NumButton.SelectStatusEnum.Grayed:
                        next = NumButton.SelectStatusEnum.NotSelected;
                        break;
                    default:
                        throw new Exception("Invalid select status in current select mode.");
                }
            }

            // update the select status.
            SelectStatus = next;

            Update();

            if (SelectionChanged != null)
            {
                SelectionChanged(this, new NumButton.NumStateChangeArg()
                {
                    number = Convert.ToInt32(Num),
                    fromState = previous,
                    toState = next
                });
            }
        }

        private void Canvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (SelectMode != SelectModeEnum.NotSelectable)
            {
                PreHighlighted = true;
                Update();
            }
        }

        private void Canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (SelectMode != SelectModeEnum.NotSelectable)
            {
                PreHighlighted = false;
                Update();
            }
        }

        private void Update()
        {
            bool bSelected = SelectStatus != SelectStatusEnum.NotSelected;
            EL_Circle.Stroke = new SolidColorBrush(PreHighlighted == bSelected ? SecondaryColor : PrimaryColor);

            BkClr_Start.Color = Colors.White;
            switch (SelectStatus)
            {
                case SelectStatusEnum.NotSelected: BkClr_Stop.Color = SecondaryColor; break;
                case SelectStatusEnum.Selected: BkClr_Stop.Color = PrimaryColor; break;
                case SelectStatusEnum.Highlighted:
                    {
                        BkClr_Stop.Color = Colors.Gold;
                        BkClr_Start.Color = Colors.LightYellow;
                        break;
                    }
                case SelectStatusEnum.Grayed: BkClr_Stop.Color = Colors.DarkGray; break;
            }
            TB_Num.Text = Num.ToString();
            TB_Num.Foreground = new SolidColorBrush(bSelected ? Colors.White : Colors.Black);

            Obmission.Text = Description;
            Obmission.Visibility = ShowDescription;
            Obmission.Foreground = new SolidColorBrush(TipColor);
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            Update();
        }
    }
}
