using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LuckyBallsData.Selection
{
    public class Region : IEquatable<Region>
    {
        public Region()
        {
            m_iMin = 0;
            m_iStep = 0;
        }

        public Region(int single)
        {
            m_iMin = single;
            m_iStep = 0;
        }

        public Region(int iMin, int iMax)
        {
            Reset(iMin, iMax);
        }

        public Region(string str)
        {
            Parse(str);
        }

        public bool Equals(Region other)
        {
            if (this.m_iMin == other.m_iMin && this.m_iStep == other.m_iStep)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset(int iMin, int iMax)
        {
            if (iMin >= 0 && iMax >= iMin)
            {
                m_iMin = iMin;
                m_iStep = iMax - iMin;
            }
            else
            {
                throw new Exception("Both Min and Step must not be less than zero.");
            }
        }

        public void Reset(Region other)
        {
            Reset(other.Min, other.Max);
        }

        // Result:  1 - larger than the specific num or ragion.
        //          -1 - smaller than the specific num or ragion
        //          0 - contains the number or cross with the specific ragion 
	    public int CompareTo(int iNum)
	    {
		    return CompareTo(new Region(iNum));
	    }

	    public int CompareTo(Region other)
	    {
		    if (m_iMin > other.m_iMin + other.m_iStep)
			    return 1;
		    else if (m_iMin + m_iStep < other.m_iMin)
			    return -1;
		    else
			    return 0;
	    }

	    public override string ToString()
	    {
            string str;
		    if (m_iStep == 0)
			    str = m_iMin.ToString();
		    else
                str = m_iMin.ToString() + "~" + (m_iMin + m_iStep).ToString();
		    return str;
	    }

        public int[] Numbers
        {
            get
            {
                int[] nums = new int[m_iStep + 1];
                for (int i = 0; i <= m_iStep; ++i)
                {
                    nums[i] = i + m_iMin;
                }

                return nums;
            }
        }

        public void Parse(string str)
	    {
            int iBreak = str.IndexOf('~');
            if (iBreak < 0)
		    {
			    // a single number.
			    m_iMin = Convert.ToInt32(str);
			    m_iStep = 0;
		    }
		    else
		    {                
			    string strMin = str.Substring(0, iBreak);
                string strMax = str.Substring(iBreak + 1, str.Length - iBreak - 1);
			    m_iMin = Convert.ToInt32(strMin);
                m_iStep = Convert.ToInt32(strMax) - m_iMin;
                if (m_iStep < 0 || m_iMin < 0)
                {
                    throw new Exception("Input string could not be parsed as a valid ragion.");
                }
		    }            
        }

        public int Min
        {
            get { return m_iMin; }
        }

        public int Max
        {
            get { return m_iMin + m_iStep; }
        }

        public int Step
        {
            get { return m_iStep; }
        }

        public void Extend(int head, int tail)
        {
            m_iMin -= head;
            m_iStep += head;
            m_iStep += tail;
        }

        public bool Merge(Region other)
        {
            if (m_iMin > other.Max + 1 || Max + 1 < other.Min)
                return false;

            int tempMax = Max;

            m_iMin = Math.Min(Min, other.Min);
            m_iStep = Math.Max(tempMax, other.Max) - m_iMin;

            return true;
        }

        public bool Contains(int iNum)
        {
            return m_iMin <= iNum && Max >= iNum;
        }

	    private int m_iMin;
	    private int m_iStep; // iStep == 0, means it represents a single number.
    }

    public class Set
    {
        protected List<Region> m_ValidNumbers = new List<Region>();

        public Set()
        {
        }

	    public Set(Region ragion)
        {
            Reset(ragion);
        }

        public Set(int[] nums)
        {
            Reset(nums);
        }

	    public Set(string str)
        {
            Parse(str);
        }

	    public Set(Set from)
        {
            Reset(from);
        }

        public void Parse(string str)
        {
            Reset(str);
        }

	    public override string ToString()
        {
            string str = "";
            foreach (Region condi in m_ValidNumbers)
            {
                if (str.Length > 0)
                    str += ",";

                str += condi.ToString();
            }

            return str;
        }

        public void Reset(Set from)
        {
            m_ValidNumbers.Clear();

            foreach (Region item in from.m_ValidNumbers)
            {
                m_ValidNumbers.Add(item);
            }
        }

	    public void	Reset(Region ragion)
        {
            m_ValidNumbers.Clear();
            m_ValidNumbers.Add(ragion);
        }

        // the numbers should be ordered from smaller to larger and no duplication.
        public void Reset(int[] nums)
        {
            m_ValidNumbers.Clear();

            Region tempRagion = null;
            foreach (int num in nums)
            {
                if (tempRagion == null)
                {
                    // init temp.
                    tempRagion = new Region(num);
                }
                else
                {
                    int comp = tempRagion.CompareTo(num);
                    if (comp >= 0)
                    {
                        Clear();
                        throw new Exception("Input nums should be sorted by numeric order and no duplication.");
                    }

                    // be connected with the ragion?
                    if (num == tempRagion.Max + 1)
                    {
                        tempRagion.Extend(0, 1);
                    }
                    else
                    {
                        // submit existing and adding a new ragion.
                        m_ValidNumbers.Add(tempRagion);

                        // reset temp
                        tempRagion = new Region(num);
                    }
                }
            }

            // submit last one.
            m_ValidNumbers.Add(tempRagion);
        }

        public void Reset(string str)
        {
            m_ValidNumbers.Clear();

            int iStart = 0, iEnd = -1, iLength = str.Length;
            while (iStart < iLength)
            {
                iEnd = str.IndexOf(',', iStart);
                if (iEnd < 0)
                {
                    iEnd = iLength;
                }

                Add(new Region(str.Substring(iStart, iEnd - iStart)));

                iStart = iEnd + 1;
            }
        }

	    public void	Add(int iNum)
        {
            Add(new Region(iNum));
        }

        public void Add(Set nums)
        {
            foreach (Region rg in nums.Ragions)
            {
                Add(rg);
            }
        }

        public void Add(Region ragion)
        {
            int InsertAt = 0;
            Region temp = ragion;       
            int count = m_ValidNumbers.Count();
            for (int i = count - 1; i >= 0; --i)
            {
                int iComp = m_ValidNumbers[i].CompareTo(temp);

                // try to merge with this.
                if (temp.Merge(m_ValidNumbers[i]))
                {
                    // Remove current ragion.
                    m_ValidNumbers.RemoveAt(i);
                }
                else if (iComp < 0)
                {
                    // insert here.
                    InsertAt = i + 1;                    
                    break; // end.
                }
            }

            // Insert ...
            m_ValidNumbers.Insert(InsertAt, temp);
        }

        public void Remove(int iNum)
        {
            int count = m_ValidNumbers.Count();
            for (int i = 0; i < count; ++i)
            {
                int comp = m_ValidNumbers[i].CompareTo(iNum);
                if (comp > 0)
                    return; // not included.

                if (comp == 0)
                {
                    // Split this ragion by the number.
                    int tempMin = m_ValidNumbers[i].Min;
                    int tempMax = m_ValidNumbers[i].Max;

                    // Remove this ragion.
                    m_ValidNumbers.RemoveAt(i);

                    // Part 1.
                    if (iNum > tempMin)
                    {
                        m_ValidNumbers.Insert(i++, new Region(tempMin, iNum - 1));
                    }

                    // Part 2.
                    if (iNum < tempMax)
                    {
                        m_ValidNumbers.Insert(i++, new Region(iNum + 1, tempMax));
                    }
                    
                    break; // end.
                }
            }
        }

        public void Clear()
        {
            m_ValidNumbers.Clear();
        }

	    public bool	Contains(int iNum)
        {
            foreach (Region condi in m_ValidNumbers)
            {
                int iComp = condi.CompareTo(iNum);
                if (iComp == 0) // just it...
                    return true;

                if (iComp > 0) // not in...
                    return false;
            }

            return false;
        }

        public bool Contains(Region test)
        {
            foreach (Region condi in m_ValidNumbers)
            {
                int iComp = condi.CompareTo(test);
                if (iComp == 0 && condi.Min <= test.Min && condi.Max >= test.Max)
                {
                    return true;
                }

                if (iComp > 0) // not in...
                    return false;
            }

            return false;
        }

        public Set Intersection(Set compare)
        {
            Set result = new Set();

            foreach (int num in Numbers)
            {
                if (compare.Contains(num))
                    result.Add(num);
            }

            return result;
        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (Region condi in m_ValidNumbers)
                {
                    count += condi.Step + 1;
                }

                return count;
            }
        }

        public List<Region> Ragions 
        {
            get
            {
                return m_ValidNumbers;
            }
        }

        public int[] Numbers
        {
            get
            {
                int index = 0;
                int[] nums = new int[Count];
                foreach (Region rg in m_ValidNumbers)
                {
                    foreach (int num in rg.Numbers)
                    {
                        nums[index++] = num;
                    }
                }

                return nums;
            }
        }

        public string DisplayExpression
        {
            get
            {
                int[] nums = Numbers;
                string str = "";
                for (int i = 0; i < nums.Count(); ++ i)
                {
                    str += nums[i].ToString().PadLeft(2, '0');
                    if (i != nums.Count() - 1)
                        str += ",";
                }

                return str;
            }
        }
    };
}
