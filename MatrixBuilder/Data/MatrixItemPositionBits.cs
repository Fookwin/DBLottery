using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixBuilder
{
    class MatrixItemPositionBits
    {
        private List<UInt64> Flags = new List<UInt64>();
        private int _unhitCount = -1;

        public MatrixItemPositionBits(int size, bool asEmpty)
        {
            UInt64 init_flag = asEmpty ? 0 : ~(UInt64)0;
            const UInt64 int_1 = (UInt64)1;

            int segmemtCount = size / 64;
            for (int i = 1; i <= segmemtCount; ++i)
            {
                Flags.Add(init_flag);
            }

            // add the last one.
            int last = size % 64;
            if (last != 0)
            {
                if (asEmpty)
                {
                    Flags.Add(0);
                }
                else
                {
                    UInt64 temp = 1;
                    for (int i = 0; i < last; ++i)
                    {
                        temp |= int_1 << i;
                    }

                    Flags.Add(temp);
                }
            }

            _unhitCount = asEmpty ? 0 : size;
        }

        public void AddSingle(UInt64 test, UInt16 segment)
        {
            UInt64 before = Flags[segment];
            Flags[segment] |= test;

            if (before != Flags[segment])
                ++_unhitCount;
        }

        public bool Contains(int pos)
        {
            int segment = pos / 64;
            UInt64 flag = (UInt64)1 << (pos % 64);

            return (Flags[segment] & flag) != 0;
        }

        public int NextPosition(int previous, bool testHas)
        {
            int count = Flags.Count;
            int startSeg = previous / 64;
            for (int i = startSeg; i < count; ++i)
            {
                UInt64 flag = 1;
                int startBit = i == startSeg ? (previous % 64) + 1 : 1;
                for (int j = startBit; j < 64; ++j)
                {
                    if (testHas != ((flag & Flags[i]) == 0))
                        return i * 64 + j;

                    flag = flag << 1;
                }
            }

            return count;
        }

        public void AddSingle(int pos)
        {
            AddSingle(((UInt64)1 << (pos % 64)), (UInt16)(pos / 64));
        }

        public void AddMultiple(MatrixItemPositionBits test)
        {
            int count = Flags.Count;
            for (int i = 0; i < count; ++i)
            {
                Flags[i] |= test.Flags[i];
            }

            _unhitCount = -1;
        }

        public void RemoveSingle(int pos)
        {
            int segment = pos / 64;
            UInt64 flag = (UInt64)1 << (pos % 64);

            UInt64 before = Flags[segment];
            Flags[segment] &= ~flag;

            if (before != Flags[segment])
                --_unhitCount;
        }

        public void RemoveMultiple(MatrixItemPositionBits test)
        {
            int count = Flags.Count;
            for (int i = 0; i < count; ++i)
            {
                Flags[i] &= ~test.Flags[i];
            }

            _unhitCount = -1;
        }

        public MatrixItemPositionBits Clone()
        {
            return new MatrixItemPositionBits() { Flags = this.Flags.ToList(), _unhitCount = this._unhitCount };
        }

        public bool IsClean()
        {
            if (_unhitCount == 0)
                return true;

            foreach (UInt64 segFlag in Flags)
            {
                if (segFlag != 0)
                    return false;
            }

            _unhitCount = 0;
            return true;
        }

        public void CopyTo(MatrixItemPositionBits to)
        {
            to.Flags = Flags.ToList();
            to._unhitCount = _unhitCount;
        }

        public int UnhitCount
        {
            get
            {
                if (_unhitCount >= 0)
                    return _unhitCount;

                _unhitCount = 0;

                int count = Flags.Count;
                for (int i = 0; i < count; ++i)
                {
                    UInt64 segFlag = Flags[i];
                    if (segFlag != 0)
                    {
                        _unhitCount += BuildMatrixUtil.BitCount(segFlag);
                    }
                }

                return _unhitCount;
            }
        }

        private MatrixItemPositionBits()
        {
        }
    }

}
