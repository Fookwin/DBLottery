using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;

namespace MatrixBuilder
{
    enum MatrixResult
    {
        Job_Succeeded,
        Job_Succeeded_Continue,
        Job_Failed,
        Job_Aborted,
        User_Aborted
    }

    class NumberDistribution
    {
        public UInt64 Bits = 0;
        public MatrixItemPositionBits Distribution = null;
        public int MinIndex = -1;
        public int MaxIndex = -1;
    }

    class IndexScope
    {
        public readonly int Start = -1;
        public readonly int End = -1;

        public IndexScope(int start, int end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return "(" + Start.ToString() + ", " + End.ToString() + ")";
        }
    }

    class MatrixBuildSettings
    {
        private object lockObject = new object();

        public readonly List<MatrixItemByte> TestItemCollection = null;
        public readonly List<MatrixItemPositionBits> TestItemMashCollection = null;
        public readonly List<NumberDistribution> NumDistributions = null;

        public readonly int CandidateNumCount = 0;  // the count of the numbers could be selected from.
        public readonly int SelectNumCount = 0;     // the count of the numbers to be selected.

        public readonly int MaxItemCountCoveredByOneItem = 0;
        public readonly int IdealMinItemCount = 0;

        private List<MatrixItemByte> _solution = null;
        private int _solutionItemCount = -1;

        public MatrixBuildSettings(int _candidateNumCount, int _selectNumCount)
        {
            SelectNumCount = _selectNumCount;
            CandidateNumCount = _candidateNumCount;

            // Get the test items.
            TestItemCollection = BuildMatrixUtil.GetTestItemCollection(_candidateNumCount, _selectNumCount);

            TestItemMashCollection = BuildMash();

            NumDistributions = BuildNumDistributions();

            MaxItemCountCoveredByOneItem = (_candidateNumCount - _selectNumCount) * _selectNumCount + 1;

            IdealMinItemCount = (TestItemMashCollection.Count - 1) / MaxItemCountCoveredByOneItem + 1;
        }

        public List<MatrixItemByte> CurrentSolution
        {
            get
            {
                return _solution;
            }
        }

        public int CurrentSolutionCount()
        {
            return _solutionItemCount;
        }

        public bool CommitSolution(List<MatrixItemByte> solution)
        {
            // don't commit for worse solution.
            if (_solutionItemCount > 0 && solution.Count >= _solutionItemCount)
                return false;

            lock (lockObject)
            {
                _solution = solution;
                _solutionItemCount = solution.Count;
            }

            return true;
        }

        private List<MatrixItemPositionBits> BuildMash()
        {
            int matchNumCount = SelectNumCount - 1;
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
                    if (itemA.Intersection(itemB) >= matchNumCount)
                    {
                        bitsA.AddSingle(j);
                        result[j].AddSingle(i);
                    }
                }
            }

            return result;
        }

        private List<NumberDistribution> BuildNumDistributions()
        {
            int testItemCount = TestItemCollection.Count;
            List<NumberDistribution> result = new List<NumberDistribution>();

            UInt64 testBit = 1;

            for (int i = 0; i < CandidateNumCount; ++i)
            {
                result.Add(new NumberDistribution()
                {
                    Bits = testBit,
                    Distribution = new MatrixItemPositionBits(testItemCount, true),
                    MaxIndex = 0,
                    MinIndex = 0
                });

                testBit = testBit << 1;
            }

            for (int i = 0; i < testItemCount; ++i)
            {
                MatrixItemByte item = TestItemCollection[i];
                for (int j = 0; j < CandidateNumCount; ++j)
                {
                    if ((item.Bits & result[j].Bits) != 0)
                    {
                        result[j].Distribution.AddSingle(i);

                        result[j].MinIndex = result[j].MaxIndex < 0 ? i : result[j].MinIndex; // set the min as the first time the number hit.
                        result[j].MaxIndex = i; // always set the max to get it to be the last index of the number hit.
                    }
                }
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
                    MatrixCell cell = new MatrixCell();

                    // Read from the file.
                    string file = ".\\Solution\\Best\\" + i.ToString() + "-" + j.ToString() + ".txt";
                    if (File.Exists(file))
                    {
                        IEnumerable<string> lines = File.ReadLines(file);
                        foreach (string line in lines)
                        {
                            cell.Template.Add(new MatrixItemByte(line));
                        }

                        cell.Status = MatrixCell.MatrixStatus.Best;
                    }
                    else
                    {
                        file = ".\\Solution\\Good\\" + i.ToString() + "-" + j.ToString() + ".txt";
                        if (File.Exists(file))
                        {
                            IEnumerable<string> lines = File.ReadLines(file);
                            foreach (string line in lines)
                            {
                                cell.Template.Add(new MatrixItemByte(line));
                            }

                            cell.Status = MatrixCell.MatrixStatus.Candidate;
                        }
                        else
                        {
                            file = ".\\Solution\\Default\\" + i.ToString() + "-" + j.ToString() + ".txt";
                            if (File.Exists(file))
                            {
                                IEnumerable<string> lines = File.ReadLines(file);
                                foreach (string line in lines)
                                {
                                    cell.Template.Add(new MatrixItemByte(line));
                                }
                            }
                            else
                            {
                                List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(i, j, _matrixTable);
                                if (defaultSoution != null)
                                {
                                    // save to file.
                                    List<string> output = new List<string>();
                                    foreach (MatrixItemByte item in defaultSoution)
                                    {
                                        output.Add(item.ToString());
                                    }

                                    File.WriteAllLines(file, output);

                                    cell.Template = defaultSoution;
                                }
                            }

                            cell.Status = MatrixCell.MatrixStatus.Default;
                        }
                    }

                    _matrixTable.SetCell(i, j, cell);
                }
            }
        }

        public int BuildMarixCell(int row, int col, int testLimit, int algorithm)
        {
            DateTime startTime = DateTime.Now;

            _matrixTable.Init();

            // (re)Set the global settings data.
            MatrixBuildSettings settings = new MatrixBuildSettings(row, col);

            if (algorithm == 0) // exhaustion algorithm
            {
                // if not specify the the max selection count, set it as the count of default solution.
                bool bReturnForAny = true;
                if (testLimit <= 0)
                {
                    // Get the default matrix as the candidate solution.
                    List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(settings.CandidateNumCount, settings.SelectNumCount, _matrixTable);
                    if (defaultSoution != null)
                    {
                        testLimit = defaultSoution.Count - 1; // try to find the better solution than default.
                    }

                    bReturnForAny = false;
                }

                string strConditions = "Conditions: ";
                strConditions += "ProcesserCount: " + Environment.ProcessorCount + "\n";
                strConditions += "CandidateNumCount: " + settings.CandidateNumCount + "\n";
                strConditions += "SelectNumCount: " + settings.SelectNumCount + "\n";
                strConditions += "IdealMinItemCount: " + settings.IdealMinItemCount + "\n";
                strConditions += "MaxItemCountCoveredByOneItem: " + settings.MaxItemCountCoveredByOneItem + "\n";
                strConditions += "TestItemCollectionCount: " + settings.TestItemCollection.Count() + "\n";
                strConditions += "TopLevelLoopMaxIndex: " + settings.NumDistributions[0].MaxIndex + "\n";

                if (MessageBox.Show(strConditions, "Ready?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return -1;
                }

                ExhaustionAlgorithmImpl impl = new ExhaustionAlgorithmImpl(settings, _matrixTable, MatrixProgressHandler);
                impl.Calculate(testLimit, bReturnForAny, true);
            }

            TimeSpan duration = DateTime.Now - startTime;

            if (settings.CurrentSolutionCount() > 0)
            {
                if (MessageBox.Show("Result:" + settings.CurrentSolutionCount().ToString() + ". Save it?", "Successful", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _matrixTable.SetCell(row, col, new MatrixCell() { Template = settings.CurrentSolution });

                    // save to file.
                    string file = row.ToString() + "-" + col.ToString() + ".txt";

                    List<string> output = new List<string>();

                    foreach (MatrixItemByte item in settings.CurrentSolution)
                    {
                        output.Add(item.ToString());
                    }

                    File.WriteAllLines(".\\Solution\\Good\\" + file, output);
                }

                MessageBox.Show("Found Solution with Count " + settings.CurrentSolutionCount().ToString() + " Duration: " + duration.ToString());

                return settings.CurrentSolutionCount();
            }

            MessageBox.Show("No Solution Found!" + " Duration: " + duration.ToString());
 
            return -1;
        }
    }

    
}