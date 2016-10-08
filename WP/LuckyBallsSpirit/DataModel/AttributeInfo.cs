using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using LuckyBallsData.Selection;

namespace LuckyBallsSpirit.DataModel
{
    class AttributeItem : IComparable
    {
        public string Key
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public SchemeAttribute Source
        {
            get;
            set;
        }

        public bool Recommended
        {
            get;
            set;
        }

        public double MaxEnegine
        {
            get;
            set;
        }

        public int CompareTo(object obj)
        {
            int res = 0;
            try
            {
                AttributeItem sObj = (AttributeItem)obj;
                if (this.MaxEnegine > sObj.MaxEnegine)
                {
                    res = -1;
                }
                else if (this.MaxEnegine < sObj.MaxEnegine)
                {
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("比较异常", ex.InnerException);
            }
            return res;
        }

    };

    class AttributeGroup : INotifyPropertyChanged
    {
        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool _expanded = false;
        private AttributeItem _selected = null;

        public string DisplayName
        {
            get;
            set;
        }

        public List<AttributeItem> Attributes
        {
            get;
            set;
        }

        public bool Expanded
        {
            get
            {
                return _expanded;
            }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged("Expanded");
                }
            }
        }

        public AttributeItem SelectedItem
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }
    };
}
