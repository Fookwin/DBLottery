using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Selection;
using LuckyBallsData.Statistics;

namespace LuckyBallsData.Util
{
    public static class AttributeUtil
    {
        private static readonly int[] _redMatrixRow = new int[33] { 4, 4, 3, 3, 3, 4, 5, 5, 5, 5, 4, 3, 2, 2, 2, 2, 2, 3, 4, 5, 6, 6, 6, 6, 6, 6, 5, 4, 3, 2, 1, 1, 1 };
        private static readonly int[] _redMatrixColumn = new int[33] { 4, 3, 3, 4, 5, 5, 5, 4, 3, 2, 2, 2, 2, 3, 4, 5, 6, 6, 6, 6, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1, 1, 2, 3 };
        private static readonly int[] _blueMatrixRow = new int[16] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4 };
        private static readonly int[] _blueMatrixColumn = new int[16] { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 };
        private static List<string> _matrix_012 = new List<string>() {"0-0-6","0-1-5", "0-2-4", "0-3-3", "0-4-2", "0-5-1", "0-6-0", "1-0-5", "1-1-4",
            "1-2-3", "1-3-2", "1-4-1", "1-5-0", "2-0-4", "2-1-3", "2-2-2", "2-3-1", "2-4-0", "3-0-3", "3-1-2", "3-2-1", "3-3-0", 
            "4-0-2", "4-1-1", "4-2-0", "5-0-1", "5-1-0", "6-0-0"
        };

        private static List<string> _attributeKeys = null;
        private static SchemeAttributes _attributeTemplate = null;

        public static List<string> AttributeKeys()
        {
            if (_attributeKeys == null)
            {
                _attributeKeys = new List<string>();
                foreach (KeyValuePair<string, SchemeAttributeCategory> cat in GetAttributesTemplate().Categories)
                {
                    foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Value.Attributes)
                    {
                        _attributeKeys.Add(attri.Key);
                    }
                }
            }

            return _attributeKeys;
        }

        public static SchemeAttributes GetAttributesTemplate()
        {
            return _attributeTemplate;
        }

        public static void SetAttributesTemplate(SchemeAttributes template)
        {
            _attributeTemplate = template;
        }

        public static SchemeAttribute GetSchemeAttribute(string attriKey)
        {
            if (_attributeTemplate == null)
                throw new Exception("Attribute Template was not initialized.");

            return _attributeTemplate.Attribute(attriKey);
        }

        public static int RedRow(int red)
        {
            return _redMatrixRow[red - 1];
        }

        public static int RedColumn(int red)
        {
            return _redMatrixColumn[red - 1];
        }

        public static int BlueRow(int red)
        {
            return _blueMatrixRow[red - 1];
        }

        public static int BlueColumn(int red)
        {
            return _blueMatrixColumn[red - 1];
        }

        public static int IndexOfSum(int sum)
        {
            return sum > 140 ? 8 : (sum < 71 ? 0 : (sum - 61) / 10);
        }

        public static int IndexOf3Div(int countIn0, int countIn1, int countIn2)
        {
            string exp = countIn0.ToString() + "-" + countIn1.ToString() + "-" + countIn2.ToString();
            return _matrix_012.IndexOf(exp) + 1;
        }

        public static int IndexOf5Xing(int red)
        {
            string sRed = red.ToString().PadLeft(2, '0');
            if ("09 10 21 22 33".Contains(sRed))
                return 1;
            else if ("03 04 15 16 27 28".Contains(sRed))
                return 2;
            else if ("01 12 13 24 25".Contains(sRed))
                return 3;
            else if ("06 07 18 19 30 31".Contains(sRed))
                return 4;
            else //"02 05 08 11 14 17 20 23 26 29 32"
                return 5;
        }

        public static string IndexOf3DivToString(int index)
        {
            return _matrix_012[index - 1];
        }

        public static bool IsPrime(int num)
        {
            if (num == 1 ||
                num == 2 ||
                num == 3 ||
                num == 5 ||
                num == 7 ||
                num == 11 ||
                num == 13 ||
                num == 17 ||
                num == 19 ||
                num == 23 ||
                num == 29 ||
                num == 31)
                return true;
            else
                return false;
        }
    }
}
