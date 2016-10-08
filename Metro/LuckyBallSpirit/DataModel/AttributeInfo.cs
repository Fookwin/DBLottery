using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using LuckyBallsData.Selection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LuckyBallSpirit.DataModel
{
    public class RecommandedBoolToBorderThickness : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool temp = (bool)value;

            return temp ? new Thickness(3,0,0,0) : new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    class AttributeValue
    {
        public string DisplayName
        {
            get;
            set;
        }

        public SchemeAttributeValueStatus Source
        {
            get;
            set;
        }

        public string Score
        {
            get;
            set;
        }

        public bool Highlight
        {
            get;
            set;
        }
    }

    class AttributeItem : IComparable<AttributeItem>
    {
        public string DisplayName
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
       
        public bool Filtered
        {
            get;
            set;
        }

        public double MaxScore
        {
            get;
            set;
        }

        public List<AttributeValue> Values
        {
            get;
            set;
        }

        public int CompareTo(AttributeItem other)
        {
            // If other is not a valid object reference, this instance is greater. 
            if (other == null) return 1;

            // The temperature comparison depends on the comparison of  
            // the underlying Double values.  
            return MaxScore.CompareTo(other.MaxScore);
        }
    };

    class AttributeGroup
    {
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
    };
}
