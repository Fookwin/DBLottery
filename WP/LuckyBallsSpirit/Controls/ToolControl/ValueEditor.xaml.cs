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

namespace LuckyBallsSpirit.Controls
{
    public class ValueChangeArgs
    {
        public double Before
        {
            get;
            set;
        }

        public double After
        {
            get;
            set;
        }
    }

    public sealed partial class ValueEditorCtrl : UserControl
    {
        private double _value = 0.0;

        public delegate void ValueChangedEventHandler(object sender, ValueChangeArgs e);
        public event ValueChangedEventHandler ValueChanged = null;

        public double LowLimit
        {
            get;
            set;
        }

        public double HighLimit
        {
            get;
            set;
        }

        // ValueName property
        private static void OnValueNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueEditorCtrl ctrl = d as ValueEditorCtrl;
            if (ctrl != null)
            {
                string val = e.NewValue as string;
                ctrl.UpdateValueName(val);
            }
        }

        private void UpdateValueName(string value)
        {
            ValueNameText.Text = value;
        }

        public static readonly DependencyProperty ValueNameProperty = DependencyProperty.Register(
                        "ValueName", typeof(string), typeof(ValueEditorCtrl),
                        new PropertyMetadata("",
                        new PropertyChangedCallback(OnValueNamePropertyChanged)));

        public string ValueName
        {
            get { return (string)GetValue(ValueNameProperty); }
            set { SetValue(ValueNameProperty, value); }
        }

        // Value property
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueEditorCtrl ctrl = d as ValueEditorCtrl;
            if (ctrl != null)
            {
                double? val = e.NewValue as double?;
                ctrl.UpdateValue(val.Value);
            }
        }

        private void UpdateValue(double value)
        {
            _value = value; // set the internal value first before update the ui to avoid firing the event.
            ValueBox.Text = value.ToString();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                        "Value", typeof(double), typeof(ValueEditorCtrl),
                        new PropertyMetadata(0.0,
                        new PropertyChangedCallback(OnValuePropertyChanged)));

        public double Value
        {
            get { return _value; }
            set { SetValue(ValueProperty, value); }
        }


        public ValueEditorCtrl()
        {
            this.InitializeComponent();
        }

        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_value > LowLimit)
            {
                ValueChangeArgs arg = new ValueChangeArgs();
                arg.Before = _value;

                _value -= 1;
                if (_value < LowLimit)
                    _value = LowLimit;

                arg.After = _value;

                ValueBox.Text = _value.ToString();

                if (ValueChanged != null)
                {
                    ValueChanged(this, arg);
                }
            }
        }

        private void Increase_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_value < HighLimit)
            {
                ValueChangeArgs arg = new ValueChangeArgs();
                arg.Before = _value;

                _value += 1;
                if (_value > HighLimit)
                    _value = HighLimit;

                arg.After = _value;

                ValueBox.Text = _value.ToString();

                if (ValueChanged != null)
                {
                    ValueChanged(this, arg);
                }
            }
        }

        private void Value_Changed(object sender, TextChangedEventArgs e)
        {
            double newVal = _value;
            if (Double.TryParse(ValueBox.Text, out newVal) && newVal != _value)
            {
                // is value valid?
                if (newVal < LowLimit)
                {
                    newVal = LowLimit;

                    ValueBox.Text = newVal.ToString();
                }
                else if (newVal > HighLimit)
                {
                    newVal = HighLimit;

                    ValueBox.Text = newVal.ToString();
                }
                
                // update value.
                ValueChangeArgs arg = new ValueChangeArgs();
                arg.Before = _value;
                arg.After = newVal;

                _value = newVal;

                if (ValueChanged != null)
                {
                    ValueChanged(this, arg);
                }
            }
        }
    }
}
