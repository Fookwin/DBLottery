using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MatrixBuilder
{
    enum MatrixResult
    {
        Failed = 0,
        Succeeded = 1,
        Aborted = 2
    }

    class MatrixBuildSettings
    {
        public readonly List<MatrixItemByte> TestItemCollection = null;
        public readonly List<MatrixItemPositionBits> TestItemMashCollection = null;
        public readonly List<MatrixItemPositionBits> NumDistributions = null;

        public readonly int MatchNumCount = 0;
        public readonly int CandidateNumCount = 0;
        public readonly int MaxItemCountCoveredByOneItem = 0;
        public readonly int IdealMinStepCount = 0;
        public readonly int[] ItemCountInLoopLevels = null;

        private List<MatrixItemByte> _solution = null;
        private object lockObject = new object();

        public MatrixBuildSettings(int _candidateNumCount, int _selectNumCount)
        {
            MatchNumCount = _selectNumCount - 1;
            CandidateNumCount = _candidateNumCount;

            // Get the test items.
            TestItemCollection = BuildMatrixUtil.GetTestItemCollection(_candidateNumCount, _selectNumCount);

            TestItemMashCollection = BuildMash();

            NumDistributions = BuildNumDistributions();

            MaxItemCountCoveredByOneItem = (_candidateNumCount - _selectNumCount) * _selectNumCount + 1;

            IdealMinStepCount = (TestItemMashCollection.Count - 1) / MaxItemCountCoveredByOneItem + 1;

            ItemCountInLoopLevels = CalculateItemCountInLoopLevels();
        }

        public List<MatrixItemByte> CurrentSolution
        {
            get
            {
                return _solution;
            }

            set
            {
                lock (lockObject)
                {
                    _solution = value;
                }
            }
        }

        private List<MatrixItemPositionBits> BuildMash()
        {
            List<MatrixItemPositionBits> result = new List<MatrixItemPositionBits>();

            int count = TestItemCollection.Count;
            for (int i = 0; i < count; ++i)
            {
                result.Add(new MatrixItemPositionBits(count, true));
            }

            for (int i = 0; i < count; ++i)
            {
                MatrixItemByte itemA = TestItemCollection[i];

                // Init and Add itself.
                MatrixItemPositionBits bitsA = result[i];
                bitsA.AddSingle(i);

                for (int j = i + 1; j < count; ++j)
                {
                    MatrixItemByte itemB = TestItemCollection[j];
                    if (itemA.Intersection(itemB) >= MatchNumCount)
                    {
                        bitsA.AddSingle(j);
                        result[j].AddSingle(i);
                    }
                }
            }

            return result;
        }

        private List<MatrixItemPositionBits> BuildNumDistributions()
        {
            List<MatrixItemPositionBits> result = new List<MatrixItemPositionBits>();

            int count = TestItemCollection.Count;
            for (int i = 0; i < CandidateNumCount; ++i)
            {
                result.Add(new MatrixItemPositionBits(count, true));
            }

            for (int i = 0; i < count; ++i)
            {
                MatrixItemByte item = TestItemCollection[i];

                UInt64 testBit = 1;
                for (int j = 0; j < CandidateNumCount; ++j)
                {
                    if ((item.Bits & testBit) != 0)
                    {
                        result[j].AddSingle(i);
                    }

                    testBit = testBit << 1;
                }
            }

            return result;
        }

        private int[] CalculateItemCountInLoopLevels()
        {
            int[] result = new int[CandidateNumCount - MatchNumCount];
            for (int level = 0; level < CandidateNumCount - MatchNumCount; ++level)
            {
                int total = 1;
                int devide = 1;
                for (int i = 1; i <= MatchNumCount; ++i)
                {
                    total *= (CandidateNumCount - level - i);
                    devide *= i;
                }

                result[level] = total / devide;
            }

            return result;
        }
    }

    class MatrixTableBuilder
    {
        protected MatrixTable _matrixTable = new MatrixTable();
        public delegate int MatrixCalculationProgressHandler(string threadID, string message, double progress);
        public event MatrixCalculationProgressHandler MatrixProgressHandler = null;

        public MatrixTableBuilder()
        {
            Init(false);
        }

        public MatrixTable GetTable()
        {
            return _matrixTable;
        }

        public void Init(bool bUseDefault)
        {
            _matrixTable.Init();

            for (int i = 7; i <= 33; ++i)
            {
                for (int j = 3; j <= 6; ++j)
                {
                    // Read from the file.
                    string file = "Z:\\matrix\\" + i.ToString() + "-" + j.ToString() + ".txt";
                    if (File.Exists(file))
                    {
                        MatrixCell cell = new MatrixCell();

                        IEnumerable<string> lines = File.ReadLines(file);
                        foreach (string line in lines)
                        {
                            cell.Template.Add(new MatrixItemByte(line));
                        }

                        _matrixTable.SetCell(i, j, cell);
                    }
                    else if (bUseDefault)
                    {
                        List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(i, j, _matrixTable);
                        if (defaultSoution != null)
                        {
                            _matrixTable.SetCell(i, j, new MatrixCell() { Template = defaultSoution });

                            // save to file.
                            List<string> output = new List<string>();

                            foreach (MatrixItemByte item in defaultSoution)
                            {
                                output.Add(item.ToString());
                            }

                            File.WriteAllLines(file, output);
                        }
                    }
                }
            }
        }

        public int BuildMarixCell(int row, int col, int testLimit, int algorithm)
        {
            DateTime startTime = DateTime.Now;

            _matrixTable.Init();

            // (re)Set the global settings data.
            MatrixBuildSettings settings = new MatrixBuildSettings(row, col);

            List<MatrixItemByte> result = null;

            if (algorithm == 0) // exhaustion algorithm
            {
                ExhaustionAlgorithmImpl impl = new ExhaustionAlgorithmImpl(settings, _matrixTable, MatrixProgressHandler);
                result = impl.Calculate("main", testLimit, testLimit > 0);
            }

            TimeSpan duration = DateTime.Now - startTime;

            if (result != null)
            {
                if (MessageBox.Show("Result:" + result.Count.ToString() + ". Save it?", "Successful", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _matrixTable.SetCell(row, col, new MatrixCell() { Template = result });

                    // save to file.
                    string file = row.ToString() + "-" + col.ToString() + ".txt";

                    List<string> output = new List<string>();

                    foreach (MatrixItemByte item in result)
                    {
                        output.Add(item.ToString());
                    }

                    File.WriteAllLines("Z:\\matrix\\" + file, output);
                }

                MessageBox.Show("Found Solution with Count " + result.Count.ToString() + " Duration: " + duration.ToString());

                return result.Count;
            }

            MessageBox.Show("No Solution Found!" + " Duration: " + duration.ToString());
 
            return -1;
        }
    }

    
}