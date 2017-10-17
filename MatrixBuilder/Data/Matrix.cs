using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixBuilder
{
    public class MatrixCell
    {
        public enum MatrixStatus
        {
            Best,
            Candidate,
            Default
        };

        public List<MatrixItemByte> Template = new List<MatrixItemByte>();
        public MatrixStatus Status = MatrixStatus.Candidate;
    }

    public class MatrixItemByte : IEquatable<MatrixItemByte>, IComparable<MatrixItemByte>
    {
        public static UInt64[] _rBits = new UInt64[33] 
        {
            1, 2,4,8,16,32,64,128,256,512,
            1024,2048,4096,8192,16384,32768, 65536,131072,262144, 524288,
            1048576,2097152,4194304,8388608, 16777216,33554432,67108864,134217728,268435456,
            536870912,1073741824, 2147483648,4294967296
        };

        private UInt64 _set = 0;
        private int _bitSize = 0;

        public MatrixItemByte(int size)
        {
            _bitSize = size;
        }

        public MatrixItemByte(int size, UInt64 copyFrom)
        {
            _bitSize = size;
            _set = copyFrom;
        }

        public MatrixItemByte(string values)
        {
            string[] vals = values.Split(new char[] { ' ', ',' });            
            for (int i = 0; i < vals.Count(); ++i)
            {
                _set |= _rBits[Convert.ToInt32(vals[i]) - 1];
            }

            _bitSize = vals.Count();
        }

        public MatrixItemByte Clone()
        {
            MatrixItemByte copy = new MatrixItemByte();
            copy._set = _set;
            copy._bitSize = _bitSize;

            return copy;
        }

        public UInt64 Bits
        {
            get
            {
                return _set;
            }             
        }

        public int Size
        {
            get
            {
                return _bitSize;
            }
        }

        public void Add(int num)
        {
            _set |= _rBits[num - 1];
        }

        public override string ToString()
        {
            string str = "";

            int index = 1;
            foreach (UInt64 bit in _rBits)
            {
                if ((_set | bit) == _set)
                {
                    if (str != "")
                        str += " ";

                    str += index.ToString().PadLeft(2, '0');
                }

                ++index;
            }

            return str;
        }

        public int[] Indices
        {
            get
            {
                List<int> output = new List<int>();
                int index = 1;
                foreach (UInt64 bit in _rBits)
                {
                    if ((_set | bit) == _set)
                    {
                        output.Add(index);
                    }

                    ++index;
                }

                return output.ToArray();
            }
        }

        public int[] Instance(int[] candidates)
        {
            int[] indices = Indices;
            // replace with the corresponding candidate at the same postion.
            for (int i = 0; i < indices.Count(); ++i)
            {
                indices[i] = candidates[indices[i] - 1];
            }

            return indices;
        }

        public int Intersection(MatrixItemByte compareTo)
        {
            UInt64 comp = _set & compareTo._set;
            int hit = 0;
            while (comp != 0)
            {
                ++hit;

                comp = comp & (comp - 1);
            }

            return hit;
        }

        bool IEquatable<MatrixItemByte>.Equals(MatrixItemByte other)
        {
            return _set == other._set;
        }

        int IComparable<MatrixItemByte>.CompareTo(MatrixItemByte other)
        {
            return this.ToString().CompareTo(other.ToString());
        }

        private MatrixItemByte()
        { 

        }
    }


    public class MatrixTable
    {
        private int[,] _matrixCount = null;
        private MatrixCell[,] _matrixData = null;

        public MatrixCell GetCell(int candidateCount, int selectionCount)
        {
            return _matrixData[candidateCount - 6, selectionCount - 2];
        }

        public int GetCellItemCount(int candidateCount, int selectionCount)
        {
            return _matrixCount[candidateCount - 6, selectionCount - 2];
        }        

        public MatrixCell SetCell(int candidateCount, int selectionCount, MatrixCell cell)
        {
            _matrixData[candidateCount - 6, selectionCount - 2] = cell;
            _matrixCount[candidateCount - 6, selectionCount - 2] = cell.Template.Count;

            return cell;
        }

        public void Init()
        {
            if (_matrixCount != null)
                return;

            _matrixCount = new int[32, 5];
            _matrixData = new MatrixCell[32, 5];

            // Init first column;
            for (int i = 6; i <= 33; ++i)
            {
                int count = i / 2;

                MatrixCell cell = new MatrixCell();
                for (int m = 0; m < count; ++m)
                {
                    int start = m * 2 + 1;

                    MatrixItemByte item = new MatrixItemByte(2);
                    item.Add(start);
                    item.Add(start + 1);

                    cell.Template.Add(item);
                }

                if (i % 2 == 1)
                {
                    MatrixItemByte item = new MatrixItemByte(2);
                    item.Add(1);
                    item.Add(i);

                    cell.Template.Add(item);
                }

                _matrixData[i - 6, 0] = cell;
                _matrixCount[i - 6, 0] = cell.Template.Count;
            }

            // Init first row;
            _matrixCount[0, 1] = 2;
            _matrixData[0, 1] = new MatrixCell() { Template = new List<MatrixItemByte>() { new MatrixItemByte("01 02 03"), new MatrixItemByte("04 05 06") } };

            _matrixCount[0, 2] = 3;
            _matrixData[0, 2] = new MatrixCell() { Template = new List<MatrixItemByte>() { new MatrixItemByte("01 02 03 04"), new MatrixItemByte("01 02 03 06"), new MatrixItemByte("03 04 05 06") } };

            _matrixCount[0, 3] = 1;
            _matrixData[0, 3] = new MatrixCell() { Template = new List<MatrixItemByte>() { new MatrixItemByte("01 02 03 04 05") } };

            _matrixCount[0, 4] = 1;
            _matrixData[0, 4] = new MatrixCell() { Template = new List<MatrixItemByte>() { new MatrixItemByte("01 02 03 04 05 06") } };
        }
    }    
}
