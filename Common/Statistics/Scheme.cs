using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;

namespace LuckyBallsData.Statistics
{
    public class Scheme
    {
        public static int[,] indexMatrix = new int[33, 5] {
            {201376,0,0,0,0},  //1
            {169911,31465,0,0,0},
            {142506,27405,4060,0,0},
            {118755,23751,3654,406,0},
            {98280,20475,3276,378,28}, //5
            {80730,17550,2925,351,27},
            {65780,14950,2600,325,26},
            {53130,12650,2300,300,25},
            {42504,10626,2024,276,24},
            {33649,8855,1771,253,23}, //10
            {26334,7315,1540,231,22},
            {20349,5985,1330,210,21},
            {15504,4845,1140,190,20},
            {11628,3876,969,171,19},
            {8568,3060,816,153,18}, //15
            {6188,2380,680,136,17},
            {4368,1820,560,120,16},
            {3003,1365,455,105,15},
            {2002,1001,364,91,14},
            {1287,715,286,78,13}, //20
            {792,495,220,66,12},
            {462,330,165,55,11},
            {252,210,120,45,10},
            {126,126,84,36,9},
            {56,70,56,28,8}, // 25
            {21,35,35,21,7},
            {6,15,20,15,6},
            {1,5,10,10,5},
            {0,1,4,6,4},
            {0,0,1,3,3}, // 30
            {0,0,0,1,2},
            {0,0,0,0,1},
            {0,0,0,0,0},
        };

        public static int[] getNumbersFromIndex(int index)
        {
            int[] numbers = new int[6];
            int pre_num = 0;
            for (int pos = 0; pos < 5; ++pos)
            {
                for (int num = pre_num; num < 33; ++num)
                {
                    if (index > indexMatrix[num, pos])
                    {
                        index -= indexMatrix[num, pos];
                    }
                    else
                    {
                        numbers[pos] = num + 1;
                        pre_num = numbers[pos];
                        break;
                    }
                }
            }

            numbers[5] = index + numbers[4]; // the last one.

            return numbers;
        }

        public static int getIndexFromNumbers(int[] numbers)
        {
            int index = 0, prv_num = 0;
            for (int pos = 0; pos < 6; ++pos)
            {
                int test_num = numbers[pos];

                if (pos == 5)
                    index += test_num - prv_num;
                else
                {
                    for (int num = prv_num + 1; num <= 33; ++num)
                    {
                        if (num >= test_num)
                            break;

                        index += indexMatrix[num - 1, pos];
                    }
                }

                prv_num = test_num;
            }

            return index;
        }

        // Data...
        protected UInt32 _hightBits = 0; // for the reds except 33.
        protected Byte _lowBit = 0; // for the blue and red 33.
 
        public Scheme()
        {
        }

        public Scheme(int index, int blue)
        {
            int[] numbers = getNumbersFromIndex(index);
            _init(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5], blue);
        }

        public Scheme(UInt32 highBits, Byte lowBits)
        {
            _hightBits = highBits;
            _lowBit = lowBits;
        }

	    public Scheme(int red1, int red2, int red3, int red4, int red5, int red6, int blue)
        {
            _init(red1, red2, red3, red4, red5, red6, blue);
        }

        public Scheme(string text)
        {
            string[] nums = text.Split(new char[] { ' ', '+', ',' });
            if (nums.Count() == 7)
            {
                UInt32 init = (UInt32)1;
                for (int i = 0; i < 5; ++i)
                {
                    _hightBits |= init << (Convert.ToInt32(nums[i]) - 1);
                }

                int red6 = Convert.ToInt32(nums[5]);
                if (red6 != 33)
                    _hightBits |= init << (red6 - 1);

                _lowBit |= Convert.ToByte(nums[6]);
                if (red6 == 33)
                    _lowBit |= (Byte)128;
            }
        }

        public Scheme(Scheme from)
        {
            _hightBits = from._hightBits;
            _lowBit = from._lowBit;
        }

        public int GetIndex()
        {
            return getIndexFromNumbers(GetRedNums());
        }

        public int[] GetRedNums()
        {
            int[] _redNums = new int[6];

            int index = 0;
            UInt32 init = (UInt32)1;
            for (int i = 0; i < 32; ++i)
            {
                if ((_hightBits & (init << i)) != 0)
                {
                    _redNums[index++] = i + 1;

                    if (index == 6)
                        break;
                }
            }

            if (index == 5)
                _redNums[5] = 33;

            return _redNums;
        }

        public bool IsValid()
        {
            if (Blue > 16 || Blue < 1)
                return false;

            int[] reds = GetRedNums();

            int previous = 0;
            foreach (int num in reds)
            {
                if (num <= previous || num > 33)
                    return false;

                previous = num;
            }

            return true;
        }

        public int Blue
        {
            get
            {
                return (int)(_lowBit & (Byte)127);
            }
        }

        public string BlueExp
        {
            get
            {
                return Blue.ToString().PadLeft(2, '0');
            }
        }

        private void _init(int red1, int red2, int red3, int red4, int red5, int red6, int blue)
        {
            // High bits...
            UInt32 init = (UInt32)1;
            _hightBits |= init << (red1 - 1);
            _hightBits |= init << (red2 - 1);
            _hightBits |= init << (red3 - 1);
            _hightBits |= init << (red4 - 1);
            _hightBits |= init << (red5 - 1);
            if (red6 != 33)
            {
                _hightBits |= init << (red6 - 1);
            }


            // Low bits...
            _lowBit = (Byte)blue;

            if (red6 == 33)
            {
                _lowBit |= (Byte)128;
            }
        }

        // Properties...
        public int Continuity
        {
            get
            {
                UInt32 temp = _hightBits & (_hightBits >> 1);

                int conti = 0;
                while (temp != 0)
                {
                    ++conti;

                    temp &= temp - 1;
                }

                if (Contain33() && (_hightBits & ((UInt32)1 << 31)) != 0)
                    ++conti;

                return conti;
            }
        }

        public int Sum
        {
            get
            {
                int[] reds = GetRedNums();

                int _iSum = 0;
                foreach (int num in reds)
                {
                    _iSum += num;
                }

                return _iSum;
            }
        }

        public int EvenNumCount
        {
            get
            {
                UInt32 temp = _hightBits & 0xAAAAAAAA;

                int even = 0;
                while (temp != 0)
                {
                    ++even;

                    temp &= temp - 1;
                }

                return even;
            }
        }

        public int PrimeNumCount
        {
            get
            {
                UInt32 init = 1;

                UInt32 mask = 1;
                mask |= init << 1 ;
                mask |= init << 2 ;
                mask |= init << 4 ;
                mask |= init << 6 ;
                mask |= init << 10;
                mask |= init << 12;
                mask |= init << 16;
                mask |= init << 18;
                mask |= init << 22;
                mask |= init << 28;
                mask |= init << 30;

                UInt32 temp = _hightBits & mask;

                int prime = 0;
                while (temp != 0)
                {
                    ++prime;

                    temp &= temp - 1;
                }

                return prime;
            }
        }

        public int SmallNumCount
        {
            get
            {
                UInt32 temp = _hightBits & (((UInt32)1 << 16) - 1);

                int small = 0;
                while (temp != 0)
                {
                    ++small;

                    temp &= temp - 1;
                }

                return small;
            }            
        }

        public int OddSum
        {
            get
            {
                int[] reds = GetRedNums();

                int _iOddSum = 0;
                foreach (int num in reds)
                {
                    if (num%2 == 1)
                        _iOddSum += num;
                }

                return _iOddSum;
            }
        }

        public int EvenSum
        {
            get
            {
                int[] reds = GetRedNums();

                int _iEvenSum = 0;
                foreach (int num in reds)
                {
                    if (num % 2 == 0)
                        _iEvenSum += num;
                }

                return _iEvenSum;
            }
        }

        public int PrimeSum
        {
            get
            {
                int[] reds = GetRedNums();

                int _iPrimeSum = 0;
                foreach (int num in reds)
                {
                    if (AttributeUtil.IsPrime(num))
                        _iPrimeSum += num;
                }

                return _iPrimeSum;
            }
        }

        public int CompositSum
        {
            get
            {
                int[] reds = GetRedNums();

                int _iCompositSum = 0;
                foreach (int num in reds)
                {
                    if (!AttributeUtil.IsPrime(num))
                        _iCompositSum += num;
                }

                return _iCompositSum;
            }
        }

        public bool Contains(int num)
        {
            if (num == 33)
                return Contain33();

            return (_hightBits & ((UInt32)1 << (num - 1))) != 0 ? true : false;
        }

        public int Similarity(Scheme other)
        {
            UInt32 temp = _hightBits & other._hightBits;

            int siml = 0;
            while (temp != 0)
            {
                ++siml;

                temp &= temp - 1;
            }

            if (Contain33() && other.Contain33())
                ++siml;

            return siml;
        }

        private bool Contain33()
        {
            return (_lowBit & 128) != 0;
        }

        public int Red(int index/*0~5*/)
        {
            int[] reds = GetRedNums();
            return reds[index];
        }

        public string RedsExp
        {
            get
            {
                string str = "";

                int[] reds = GetRedNums();
                foreach (int red in reds)
                {
                    str += red.ToString().PadLeft(2, '0') + " ";
                }

                return str.TrimEnd(' ');
            }
        }

        public string Red1Exp
        {
            get
            {
                return Red(0).ToString().PadLeft(2, '0');
            }
        }

        public string Red2Exp
        {
            get
            {
                return Red(1).ToString().PadLeft(2, '0');
            }
        }

        public string Red3Exp
        {
            get
            {
                return Red(2).ToString().PadLeft(2, '0');
            }
        }

        public string Red4Exp
        {
            get
            {
                return Red(3).ToString().PadLeft(2, '0');
            }
        }

        public string Red5Exp
        {
            get
            {
                return Red(4).ToString().PadLeft(2, '0');
            }
        }

        public string Red6Exp
        {
            get
            {
                return Red(5).ToString().PadLeft(2, '0');
            }
        }

        public string DisplayExpression
        {
            get
            {
                string str = "";

                int[] reds = GetRedNums();
                foreach (int red in reds)
                {
                    str += red.ToString().PadLeft(2, '0') + " ";
                }

                return str + "+ " + Blue.ToString().PadLeft(2, '0');
            }
        }

        public UInt64 RedBits
        {
            get
            {
                return _hightBits;
            }
        }

        public int GetRemainBy3(ref int countIn0, ref int countIn1, ref int countIn2)
        {
            countIn0 = 0;
            countIn1 = 0;
            countIn2 = 0;
            int[] reds = GetRedNums();
            foreach (int red in reds)
            {
                switch (red % 3)
                {
                    case 0:
                        countIn0++; break;
                    case 1:
                        countIn1++; break;
                    case 2:
                        countIn2++; break;
                }
            }

            return AttributeUtil.IndexOf3Div(countIn0, countIn1, countIn2);
 
        }

        // small-middle-big index
        public int GetSmallMiddleBig(ref int countIn0, ref int countIn1, ref int countIn2)
        {
            countIn0 = 0;
            countIn1 = 0;
            countIn2 = 0;
            int[] reds = GetRedNums();
            foreach (int red in reds)
            {
                if (red <= 11)
                {
                    countIn0++;
                }
                else if (red <= 22)
                {
                    countIn1++;
                }
                else
                {
                    countIn2++;
                }
            }

            return AttributeUtil.IndexOf3Div(countIn0, countIn1, countIn2);
        }

        public override string ToString()
        {
            string str= "";

            int[] reds = GetRedNums();
            foreach (int red in reds)
            {
                str +=  red.ToString().PadLeft(2, '0') + " ";
            }

            return str.TrimEnd() + "+" + Blue.ToString().PadLeft(2, '0');
        }

        public string ToString(string format)
        {
            if (format.Length == 0)
                return ToString();

            string red_sep = " ", blue_sep = " ";

            char[] chars = format.ToCharArray();
            if (chars.Count() == 2)
            {
                red_sep = chars[0].ToString();
                blue_sep = chars[1].ToString();
            }
            else
            {
                // Not supported format, switch to default.
                return ToString();
            }
            
            string str = "";
            int[] reds = GetRedNums();
            foreach (int red in reds)
            {
                if (str != "")
                    str += red_sep;
                
                str += red.ToString().PadLeft(2, '0');
            }

            return str + blue_sep + Blue.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// Get the value on the specific attribute.
        /// </summary>
        /// <param name="key">attribute name</param>
        /// <param name="previousIssueIndex">the index of the previous issue to be referenced by some attributes calcuation</param>
        /// <returns></returns>
        public int Attribute(string key, int previousIssueIndex)
        {
            if (key.Contains("Red_Repeat_Previous_"))
            {
                int step = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                if (previousIssueIndex - step >= 0)
                {
                    Lottery refLot = DataManageBase.Instance().History.Lotteries[previousIssueIndex - step];
                    return refLot.Scheme.Similarity(this);
                }
                else
                    return 0;
            }
            else if (key == "Blue_Amplitude")
            {
                if (previousIssueIndex >= 0)
                {
                    Lottery refLot = DataManageBase.Instance().History.Lotteries[previousIssueIndex];
                    return Math.Abs(Blue - refLot.Scheme.Blue);
                }
                else
                    return Blue; // treat the bule of prevouse as zero.
            }
            else if (key.Contains("Blue_Mantissa_Repeat_Previous_"))
            {
                int step = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                if (previousIssueIndex - step >= 0)
                {
                    Lottery refLot = DataManageBase.Instance().History.Lotteries[previousIssueIndex - step];
                    return refLot.Scheme.Blue % 10 == Blue % 10 ? 1 : 0;
                }
                else
                    return 0;
            }
            else if (key == "Red_FuGeZhong")
            {
                if (previousIssueIndex > 1)
                {
                    Lottery lotPre1 = DataManageBase.Instance().History.Lotteries[previousIssueIndex];
                    Lottery lotPre2 = DataManageBase.Instance().History.Lotteries[previousIssueIndex - 1];

                    int countIn0 = 0, countIn1 = 0, countIn2 = 0;
                    int[] reds = GetRedNums();
                    foreach (int red in reds)
                    {
                        if (lotPre1.Scheme.Contains(red))
                        {
                            countIn0++;
                        }
                        else if (lotPre2.Scheme.Contains(red))
                        {
                            countIn1++;
                        }
                        else
                        {
                            countIn2++;
                        }
                    }

                    return AttributeUtil.IndexOf3Div(countIn0, countIn1, countIn2);
                }
                else
                    return AttributeUtil.IndexOf3Div(0, 0, 6);
            }
            else
                return Attribute2(key);
        }

        /// <summary>
        /// Get the value on the specific attribute.
        /// </summary>
        /// <param name="key">attribute name</param>
        /// <param name="previousIssueIndex">the index of the previous issue to be referenced by some attributes calcuation</param>
        /// <returns></returns>
        public int Attribute2(string key)
        {
            if (key.Contains("Red_InPos_"))
            {
                int pos = Convert.ToInt32(key.Substring(key.Length - 1, 1));
                return Red(pos - 1);
            }
            else if (key.Contains("Red_Mantissa_InPos_"))
            {
                int pos = Convert.ToInt32(key.Substring(key.Length - 1, 1));
                return Red(pos - 1) % 10;
            }
            else if (key == "Red_Mantissa_Max")
            {
                int[] reds = GetRedNums();
                return Math.Max(reds[0] % 10,
                        Math.Max(reds[1] % 10,
                        Math.Max(reds[2] % 10,
                        Math.Max(reds[3] % 10,
                        Math.Max(reds[4] % 10,
                        reds[5] % 10)))));
            }
            else if (key == "Red_Mantissa_Min")
            {
                int[] reds = GetRedNums();
                return Math.Min(reds[0] % 10,
                        Math.Min(reds[1] % 10,
                        Math.Min(reds[2] % 10,
                        Math.Min(reds[3] % 10,
                        Math.Min(reds[4] % 10,
                        reds[5] % 10)))));
            }
            else if (key == "Red_Sum")
            {
                return Sum;
            }
            else if (key == "Red_Sum_Mantissa")
            {
                return Sum % 10;
            }
            else if (key == "Red_Odd_Sum")
            {
                return OddSum;
            }
            else if (key == "Red_Odd_Sum_Mantissa")
            {
                return OddSum % 10;
            }
            else if (key == "Red_Even_Sum")
            {
                return EvenSum;
            }
            else if (key == "Red_Even_Sum_Mantissa")
            {
                return EvenSum % 10;
            }
            else if (key == "Red_Composite_Sum")
            {
                return CompositSum;
            }
            else if (key == "Red_Composite_Sum_Mantissa")
            {
                return CompositSum % 10;
            }
            else if (key == "Red_Primary_Sum")
            {
                return PrimeSum;
            }
            else if (key == "Red_Primary_Sum_Mantissa")
            {
                return PrimeSum % 10;
            }
            else if (key == "Red_BigSmall")
            {
                return 6 - SmallNumCount;
            }
            else if (key == "Red_OddEven")
            {
                return 6 - EvenNumCount;
            }
            else if (key == "Red_PrimaryComposite")
            {
                return PrimeNumCount;
            }
            else if (key.Contains("Red_Remainder_Devide"))
            {
                int div = Convert.ToInt32(key.Substring(key.Length - 3, 1));
                int rmd = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int iHit = 0;
                int[] reds = GetRedNums();
                foreach (int red in reds)
                {
                    if (rmd == red % div)
                        iHit++;
                }
                return iHit;
            }
            else if (key.Contains("Red_Sum_Pos"))
            {
                int pos1 = Convert.ToInt32(key.Substring(key.Length - 2, 1));
                int pos2 = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int[] reds = GetRedNums();
                return reds[pos2 - 1] + reds[pos1 - 1];
            }
            else if (key.Contains("Red_Diff_Pos"))
            {
                int pos1 = Convert.ToInt32(key.Substring(key.Length - 2, 1));
                int pos2 = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int[] reds = GetRedNums();
                return reds[pos2 - 1] - reds[pos1 - 1];
            }
            else if (key.Contains("Red_Zone_In"))
            {
                int div = Convert.ToInt32(key.Substring(key.Length - 5, 2));
                int rmd = Convert.ToInt32(key.Substring(key.Length - 2, 2));

                int iHit = 0;
                int[] reds = GetRedNums();
                foreach (int red in reds)
                {
                    int divAt = 0;
                    if (div == 3)
                        divAt = (red - 1) / 11 + 1;
                    else if (div == 4)
                        divAt = red < 17 ? (red - 1) / 8 + 1 : (red - 2) / 8 + 1;
                    else if (div == 7)
                        divAt = (red - 1) / 5 + 1;
                    else if (div == 11)
                        divAt = (red - 1) / 3 + 1;

                    if (rmd == divAt)
                        iHit++;
                }
                return iHit;
            }
            else if (key.Contains("Red_Row_"))
            {
                int row = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int iHit = 0;
                int[] reds = GetRedNums();
                foreach (int red in reds)
                {
                    if (row == AttributeUtil.RedRow(red))
                        iHit++;
                }

                return iHit;
            }
            else if (key.Contains("Red_Column_"))
            {
                int col = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int iHit = 0;
                int[] reds = GetRedNums();
                foreach (int red in reds)
                {
                    if (col == AttributeUtil.RedColumn(red))
                        iHit++;
                }

                return iHit;
            }
            else if (key.Contains("Red_Omission_"))
            {
                int num = Convert.ToInt32(key.Substring(key.Length - 2, 2));
                return Contains(num) ? 1 : 0;
            }
            else if (key == "Blue_Omission")
            {
                return Blue;
            }
            else if (key == "Blue_Rows")
            {
                return AttributeUtil.BlueRow(Blue);
            }
            else if (key == "Blue_Columns")
            {
                return AttributeUtil.BlueColumn(Blue);
            }
            else if (key == "Blue_Mantissa_Omission")
            {
                return Blue % 10;
            }
            else if (key == "Red_012")
            {
                int temp1 = 0, temp2 = 0, temp3 = 0;
                return GetRemainBy3(ref temp1, ref temp2, ref  temp3);
            }
            else if (key == "Red_SmallMidBig")
            {
                int countIn0 = 0, countIn1 = 0, countIn2 = 0;
                int[] reds = GetRedNums();
                foreach (int red in reds)
                {
                    switch ((red - 1) / 11)
                    {
                        case 0:
                            countIn0++; break;
                        case 1:
                            countIn1++; break;
                        case 2:
                            countIn2++; break;
                    }
                }

                return AttributeUtil.IndexOf3Div(countIn0, countIn1, countIn2);
            }
            else if (key.Contains("Red_5Xing_"))
            {
                int pos = Convert.ToInt32(key.Substring(key.Length - 1, 1));
                return AttributeUtil.IndexOf5Xing(Red(pos - 1));
            }
            else if (key.Contains("Red_PC_Pos"))
            {
                int pos1 = Convert.ToInt32(key.Substring(key.Length - 2, 1));
                int pos2 = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int[] reds = GetRedNums();
                int A1 = AttributeUtil.IsPrime(reds[pos1 - 1]) ? 0 : 1;
                int A2 = AttributeUtil.IsPrime(reds[pos2 - 1]) ? 0 : 1;
                return A1 * 2 + A2 + 1;
            }
            else if (key.Contains("Red_WX_Pos"))
            {
                int pos1 = Convert.ToInt32(key.Substring(key.Length - 2, 1));
                int pos2 = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int[] reds = GetRedNums();
                int A1 = AttributeUtil.IndexOf5Xing(reds[pos1 - 1]);
                int A2 = AttributeUtil.IndexOf5Xing(reds[pos2 - 1]);
                return (A1 - 1) * 5 + A2;
            }
            else if (key.Contains("Red_012_Pos"))
            {
                int pos1 = Convert.ToInt32(key.Substring(key.Length - 2, 1));
                int pos2 = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int[] reds = GetRedNums();
                int A1 = reds[pos1 - 1] % 3;
                int A2 = reds[pos2 - 1] % 3;
                return A1 * 3 + A2 + 1;
            }
            else if (key.Contains("Red_OE_Pos"))
            {
                int pos1 = Convert.ToInt32(key.Substring(key.Length - 2, 1));
                int pos2 = Convert.ToInt32(key.Substring(key.Length - 1, 1));

                int[] reds = GetRedNums();
                int A1 = (reds[pos1 - 1] + 1) % 2;
                int A2 = (reds[pos2 - 1] + 1) % 2;
                return A1 * 2 + A2 + 1;
            }
            else
            {
                throw new Exception("The attribute is not supported");
            }
        }
    }
}
