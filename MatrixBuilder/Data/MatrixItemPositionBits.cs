using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixBuilder
{
    class MatrixItemPositionBits
    {
        private UInt64[] Flags;
        private int _unhitCount = -1;
        private int _flagCount = -1;

        public MatrixItemPositionBits(int size, bool asEmpty)
        {
            UInt64 init_flag = asEmpty ? 0 : ~(UInt64)0;
            const UInt64 int_1 = (UInt64)1;

            int segmemtCount = size / 64;
            int last = size % 64;

            _flagCount = segmemtCount;
            if (last != 0)
                ++_flagCount;

            Flags = new UInt64[_flagCount];
            for (int i = 0; i < segmemtCount; ++i)
            {
                Flags[i] = init_flag;
            }

            // add the last one.
            if (last != 0)
            {
                if (asEmpty)
                {
                    Flags[_flagCount - 1] = 0;
                }
                else
                {
                    UInt64 temp = 1;
                    for (int i = 0; i < last; ++i)
                    {
                        temp |= int_1 << i;
                    }

                    Flags[_flagCount - 1] = temp;
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
            int startSeg = previous / 64;
            for (int i = startSeg; i < _flagCount; ++i)
            {
                UInt64 flag = 1;
                int startBit = i == startSeg ? (previous % 64) + 1 : 0;
                for (int j = startBit; j < 64; ++j)
                {
                    if (testHas != ((flag & Flags[i]) == 0))
                        return i * 64 + j;

                    flag = flag << 1;
                }
            }

            return -1;
        }

        public void AddSingle(int pos)
        {
            AddSingle(((UInt64)1 << (pos % 64)), (UInt16)(pos / 64));
        }

        public void AddMultiple(MatrixItemPositionBits test)
        {
            for (int i = 0; i < _flagCount; ++i)
            {
                if (test.Flags[i] != 0)
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
            for (int i = 0; i < _flagCount; ++i)
            {
                if (test.Flags[i] != 0)
                    Flags[i] &= ~test.Flags[i];
            }

            _unhitCount = -1;
        }

        public MatrixItemPositionBits Clone()
        {
            var copy = new MatrixItemPositionBits() { Flags = new UInt64[this._flagCount], _flagCount = this._flagCount, _unhitCount = this._unhitCount };
            CloneFlags(Flags, copy.Flags, _flagCount);
            return copy;
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

        private unsafe static void CloneFlags(UInt64[] source, UInt64[] target, int count)
        {
            fixed (UInt64* pSource = source, pTarget = target)
            {
                // Set the starting points in source and target for the copying.
                UInt64* ps = pSource;
                UInt64* pt = pTarget;

                // Copy the specified number of bytes from source to target.
                for (int i = 0; i < count; i++)
                {
                    *pt = *ps;
                    pt++;
                    ps++;
                }
            }
        }

        public int UnhitCount
        {
            get
            {
                if (_unhitCount >= 0)
                    return _unhitCount;

                _unhitCount = 0;
                
                for (int i = 0; i < _flagCount; ++i)
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
