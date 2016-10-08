using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;


namespace LuckyBallsSpirit.Controls
{
    public partial class ValueListPicker : UserControl
    {
        public delegate void ValueChangedEventHandler();
        public event ValueChangedEventHandler ValueChanged = null;

        private ObservableCollection<string> _values = new ObservableCollection<string>();
        private string _selectedValue;

        public ValueListPicker()
        {
            InitializeComponent();
        }

        private static void OnValueNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueListPicker ctrl = d as ValueListPicker;
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
                        "ValueName", typeof(string), typeof(ValueListPicker),
                        new PropertyMetadata("",
                        new PropertyChangedCallback(OnValueNamePropertyChanged)));

        public string ValueName
        {
            get { return (string)GetValue(ValueNameProperty); }
            set { SetValue(ValueNameProperty, value); }
        }

        // Values property
        private static void OnValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueListPicker ctrl = d as ValueListPicker;
            if (ctrl != null)
            {
                List<string> val = e.NewValue as List<string>;
                ctrl.UpdateValues(val);
            }
        }

        private void UpdateValues(List<string> values)
        {
            _values.Clear();

            if (values != null)
            {
                foreach (string str in values)
                {
                    _values.Add(str);
                }
            }
        }

        public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
                        "Values", typeof(List<string>), typeof(ValueListPicker),
                        new PropertyMetadata(null,
                        new PropertyChangedCallback(OnValuesPropertyChanged)));

        public List<string> Values
        {
            get { return _values.ToList(); }
            set { SetValue(ValuesProperty, value); }
        }

        // Selected Value property
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueListPicker ctrl = d as ValueListPicker;
            if (ctrl != null)
            {
                string val = e.NewValue as string;
                ctrl.UpdateValue(val);
            }
        }

        private void UpdateValue(string value)
        {
            if (value != null && _values.Contains(value))
            {
                _selectedValue = value.ToString();
                ValueBox.Content = _selectedValue;
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                        "Value", typeof(string), typeof(ValueListPicker),
                        new PropertyMetadata(null,
                        new PropertyChangedCallback(OnValuePropertyChanged)));

        public string Value
        {
            get { return _selectedValue; }
            set { SetValue(ValueProperty, value); }
        }

        // Selected Index property
        private static void OnValueIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueListPicker ctrl = d as ValueListPicker;
            if (ctrl != null)
            {
                int? val = e.NewValue as int?;
                if (val != null)
                {
                    ctrl.UpdateValueIndex(val.Value);
                }
            }
        }

        private void UpdateValueIndex(int value)
        {
            if (_values.Count() > value && value >= 0)
            {
                _selectedValue = _values[value];
                ValueBox.Content = _selectedValue;
            }
        }

        public static readonly DependencyProperty ValueIndexProperty = DependencyProperty.Register(
                        "ValueIndex", typeof(int), typeof(ValueListPicker),
                        new PropertyMetadata(-1,
                        new PropertyChangedCallback(OnValueIndexPropertyChanged)));

        public int ValueIndex
        {
            get { return _selectedValue != null ? _values.IndexOf(_selectedValue) : -1; }
            set { SetValue(ValueIndexProperty, value); }
        }

        /////
        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            // select the previous one.
            if (_selectedValue != null)
            {
                int curIndex = _values.IndexOf(_selectedValue);
                if (curIndex > 0)
                {
                    ValueIndex = curIndex - 1;

                    if (ValueChanged != null)
                    {
                        ValueChanged();
                    }
                }
            }
        }

        private void Increase_Button_Click(object sender, RoutedEventArgs e)
        {
            // select the previous one.
            if (_selectedValue != null)
            {
                int curIndex = _values.IndexOf(_selectedValue);
                if (curIndex < _values.Count - 1)
                {
                    ValueIndex = curIndex + 1;

                    if (ValueChanged != null)
                    {
                        ValueChanged();
                    }
                }
            }
        }

        private void ValueBox_Click(object sender, RoutedEventArgs e)
        {
            ValuePickerFlyoutCombo picker = ValuePickerFlyoutCombo.UniqueInstance;
            picker.Show(ValueName, _values.ToList(), false, true, new List<int>() { ValueIndex });
            picker.ValueConfirmed += OnValueConfirmed;
        }

        private void OnValueConfirmed(List<string> selected)
        {
            if (selected.Count > 0)
            {
                string selectedValue = selected[0];
                if (selectedValue != _selectedValue)
                {
                    Value = selectedValue;

                    if (ValueChanged != null)
                    {
                        ValueChanged();
                    }
                }
            }
        }
    }
}
