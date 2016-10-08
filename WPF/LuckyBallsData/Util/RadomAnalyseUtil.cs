using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Statistics;

namespace LuckyBallsData.Util
{
    public class BlueRadomAnalyser
    {
        private int[,] _numMatrix = null;
        private List<Lottery> _tests = null;
        private Dictionary<int, List<Sequence>> _result = new Dictionary<int, List<Sequence>>();
        private int _maxOmission = -1;

        public class Sequence
        {
            public UInt64 _bits = 0;

            public Int32 NumAt(Int32 pos)
            {
                Int32 res = (Int32)(_bits >> (pos * 4));
                res &= 0xF;

                return ++ res;
            }

            public void SetNumAt(Int32 pos, Int32 num)
            {
                UInt64 mash = ((UInt64)num) << (pos * 4);
                _bits |= mash;
            }

            public int Score
            {
                get;
                set;
            }

            public override string ToString()
            {
                string output = "";
                for (int i = 0; i < 16; ++i)
                    output += NumAt(i).ToString().PadLeft(2, '0') + " ";

                return output;
            }
        }

        public void Init(List<Lottery> tests)
        {
            BuildMatrix(tests);

            _tests = tests;
        }

        public int MaxOmission()
        {
            return _maxOmission;
        }
        
        private void BuildMatrix(List<Lottery> tests)
        {
            if (_numMatrix == null)
            {
                _numMatrix = new int[16, 16];
            }

            for (int num = 0; num < 16; ++num)
            {
                for (int pos = 0; pos < 16; ++pos)
                {
                    _numMatrix[num, pos] = ExpectScore(num + 1, pos, tests);
                }
            }
        }

        public int[,] GetMatrix()
        {
            return _numMatrix;
        }

        public List<Sequence> GetResult(int threshold)
        {
            if (_result.ContainsKey(threshold))
                return _result[threshold];

            return null;
        }

        public List<Sequence> Calculate(int threshold)
        {
            if (_result.ContainsKey(threshold))
                return _result[threshold];

            List<Sequence> list = new List<Sequence>();

            TestSequences(0, 0xFFFF, _tests, threshold, new Sequence(), ref list);

            _result.Add(threshold, list);

            if (_maxOmission < 0)
            {
                foreach (Sequence item in list)
                {
                    if (item.Score > _maxOmission)
                        _maxOmission = item.Score;
                }
            }

            return list;
        }

        private void TestSequences(Int32 pos, UInt16 restNums, List<Lottery> tests, int threshold, Sequence currentQ, ref List<Sequence> output)
        {
            if (restNums == 0)
            {
                // Get one, verify it.
                if (IsValidSquence(currentQ, tests, threshold))
                    output.Add(currentQ);

                return;
            }

            for (Int32 num = 0; num < 16; ++num)
            {
                int mask = ((UInt16)1) << num;

                if ((restNums & mask) == 0)
                    continue;

                // check this number, if it is impossible to be a candidate in this position, skip it.
                if (_numMatrix[num, pos] < threshold)
                    continue;

                // continue.
                UInt16 copyRest = (UInt16)(restNums & (~mask));
                Sequence copySeq = new Sequence() { _bits = currentQ._bits };
                copySeq.SetNumAt(pos, num);

                TestSequences(pos + 1, copyRest, tests, threshold, copySeq, ref output);                
            }
        }

        private bool IsValidSquence(Sequence currentQ, List<Lottery> tests, int threshold)
        {
            int start = threshold - (threshold % 16);
            for (int i = start; i < tests.Count; ++i)
            {
                Int32 index = i % 16;
                Int32 num = currentQ.NumAt(index);

                if (tests[i].Scheme.Blue == num)
                {
                    currentQ.Score = i + 1;
                    return true;
                }
            }

            currentQ.Score = tests.Count;
            return true;
        }

        private int ExpectScore(int testNum, int pos, List<Lottery> tests)
        {
            int loopCount = tests.Count / 16;
            for (int loopIndex = 0; loopIndex < loopCount; ++loopIndex)
            {
                int index = loopIndex * 16 + pos;

                if (tests[index].Scheme.Blue == testNum)
                {
                    return index; 
                }
            }

            return loopCount * 16 + pos;
        }
    }
}
