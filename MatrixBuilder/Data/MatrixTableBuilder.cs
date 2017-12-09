using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;

namespace MatrixBuilder
{
    class ProgressState
    {
        public string ThreadID
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public double Progress
        {
            get; set;
        }
    }

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
        private readonly int MaxValue = -1;
        private readonly int MinValue = -1;
        private readonly List<int> Values = null;
        private int? _index = null;
        private int? _count = null;

        public IndexScope(int start, int end, List<int> values)
        {
            MinValue = start;
            MaxValue = end;
            Values = values;
        }

        public override string ToString()
        {
            return "(" + MinValue.ToString() + ", " + MaxValue.ToString() + ")";
        }

        public int Count()
        {
            if (Values == null)
                return MaxValue - MinValue + 1;
            else if (_count != null)
                return _count.Value;
            else
            {
                int count = 0;
                for (int i = 0; i < Values.Count; ++i)
                {
                    if (Values[i] > MaxValue)
                        break;

                    if (Values[i] >= MinValue)
                    {
                        ++count;
                    }
                }

                _count = count; // cache it.

                return count;
            }
        }

        public int Min()
        {
            return MinValue;
        }

        public int Max()
        {
            return MaxValue;
        }

        public List<int> ValueCollection()
        {
            return Values;
        }

        public int Next()
        {
            if (Values == null)
            {
                if (_index == null)
                {
                    // start from min value.
                    _index = MinValue;
                }
                else
                {
                    ++_index;

                    if (_index > MaxValue)
                        _index = -1; // not be larger than the max
                }

                return _index.Value;
            }
            else
            {
                if (_index == null)
                {
                    _index = -1;

                    // get the first index match the scope
                    for (int i = 0; i < Values.Count; ++i)
                    {
                        if (Values[i] >= MinValue && Values[i] <= MaxValue)
                        {
                            _index = i;
                            break;
                        }
                    }
                }
                else if (_index.Value >= 0)
                {
                    ++_index;

                    // not be larger than the max value and should be one in the values.
                    if (_index >= Values.Count || Values[_index.Value] > MaxValue)
                        _index = -1;
                }

                return _index.Value >= 0 ? Values[_index.Value] : -1;
            }
        }
    }

    class MatrixTestItem
    {
        public MatrixItemByte ItemByte = null;
        public MatrixItemPositionBits CoverageMash = null;
        public List<int> CoveredBy = null;
    }

    class MatrixBuildSettings
    {
        private readonly List<MatrixTestItem> TestItemCollection = null;
        private readonly List<NumberDistribution> NumDistributions = null;

        public readonly int CandidateNumCount = 0;  // the count of the numbers could be selected from.
        public readonly int SelectNumCount = 0;     // the count of the numbers to be selected.

        public readonly int MaxItemCountCoveredByOneItem = 0;
        public readonly int IdealMinItemCount = 0;

        public MatrixBuildSettings(int _candidateNumCount, int _selectNumCount)
        {
            SelectNumCount = _selectNumCount;
            CandidateNumCount = _candidateNumCount;

            // Get the test items.
            TestItemCollection = BuildTestItems(_candidateNumCount, _selectNumCount);

            NumDistributions = BuildNumDistributions();

            MaxItemCountCoveredByOneItem = (_candidateNumCount - _selectNumCount) * _selectNumCount + 1;

            IdealMinItemCount = (TestItemCollection.Count - 1) / MaxItemCountCoveredByOneItem + 1;
        }

        public NumberDistribution NumDistribution(int index)
        {
            return NumDistributions[index];
        }

        public MatrixItemByte TestItem(int index)
        {
            return TestItemCollection[index].ItemByte;
        }

        public int TestItemCount()
        {
            return TestItemCollection.Count;
        }

        public MatrixItemPositionBits TestItemMash(int index)
        {
            return TestItemCollection[index].CoverageMash;
        }

        public List<int> TestItemCoveredBy(int index)
        {
            return TestItemCollection[index].CoveredBy;
        }

        private List<MatrixTestItem> BuildTestItems(int candidateNumCount, int selectNumCount)
        {
            var itemByteCollection = BuildMatrixUtil.GetTestItemCollection(candidateNumCount, selectNumCount);

            int matchNumCount = SelectNumCount - 1;
            List<MatrixTestItem> result = new List<MatrixTestItem>();

            int count = itemByteCollection.Count;
            for (int i = 0; i < count; ++i)
            {
                result.Add(new MatrixTestItem {
                    ItemByte = itemByteCollection[i],
                    CoverageMash = new MatrixItemPositionBits(count, true),
                    CoveredBy = new List<int>()
                });
            }

            for (int i = 0; i < count; ++i)
            {
                MatrixItemByte itemA = itemByteCollection[i];

                // Init and Add itself.
                result[i].CoverageMash.AddSingle(i);
                result[i].CoveredBy.Add(i);

                for (int j = i + 1; j < count; ++j)
                {
                    MatrixItemByte itemB = itemByteCollection[j];
                    if (itemA.Intersection(itemB) >= matchNumCount)
                    {
                        result[i].CoverageMash.AddSingle(j);
                        result[i].CoveredBy.Add(j);
                        result[j].CoverageMash.AddSingle(i);
                        result[j].CoveredBy.Add(i);
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
                MatrixItemByte item = TestItemCollection[i].ItemByte;
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
        private MatrixTable _matrixTable = new MatrixTable();
        private SortedDictionary<string, ProgressState> _progressStates = new SortedDictionary<string, ProgressState>();

        public MatrixTableBuilder()
        {
            Init(false);
        }

        public MatrixTable GetTable()
        {
            return _matrixTable;
        }

        public ProgressState RegisterProgress(string key)
        {
            var progress = new ProgressState() { ThreadID = key };
            _progressStates.Add(key, progress);
            return progress;
        }

        public List<KeyValuePair<string, ProgressState>> GetProgress()
        {
            return _progressStates.ToList();
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

        public int BuildMarixCell(int row, int col, int algorithm, int? betterThan = null, bool bParallel = false, bool bReturnForAny = false)
        {
            DateTime startTime = DateTime.Now;

            _matrixTable.Init();

            // (re)Set the global settings data.
            MatrixBuildSettings settings = new MatrixBuildSettings(row, col);

            List<MatrixItemByte> foundSolution = null;
            if (algorithm == 0) // exhaustion algorithm
            {
                // if not specify the the max selection count, set it as the count of default solution.
                if (betterThan == null)
                {
                    // Get the default matrix as the candidate solution.
                    List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(settings.CandidateNumCount, settings.SelectNumCount, _matrixTable);
                    if (defaultSoution != null)
                    {
                        betterThan = defaultSoution.Count; // try to find the better solution than default.
                    }
                }

                string strConditions = "Conditions: ";
                strConditions += "ProcesserCount: " + Environment.ProcessorCount + "\n";
                strConditions += "CandidateNumCount: " + settings.CandidateNumCount + "\n";
                strConditions += "SelectNumCount: " + settings.SelectNumCount + "\n";
                strConditions += "FindingBetterThan: " + betterThan.Value + "\n";   
                strConditions += "IdealMinItemCount: " + settings.IdealMinItemCount + "\n";
                strConditions += "MaxItemCountCoveredByOneItem: " + settings.MaxItemCountCoveredByOneItem + "\n";
                strConditions += "TestItemCollectionCount: " + settings.TestItemCount() + "\n";
                strConditions += "TopLevelLoopMaxIndex: " + settings.NumDistribution(0).MaxIndex + "\n";

                if (MessageBox.Show(strConditions, "Ready?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return -1;
                }

                ExhaustionAlgorithmImpl impl = new ExhaustionAlgorithmImpl(settings);
                impl.Calculate(betterThan.Value - 1, _progressStates, bReturnForAny, bParallel);
                foundSolution = impl.GetSolution();
            }

            TimeSpan duration = DateTime.Now - startTime;

            if (foundSolution != null && foundSolution.Count() > 0)
            {
                if (MessageBox.Show("Result:" + foundSolution.Count().ToString() + ". Save it?", "Successful", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _matrixTable.SetCell(row, col, new MatrixCell() { Template = foundSolution });

                    // save to file.
                    string file = row.ToString() + "-" + col.ToString() + ".txt";

                    List<string> output = new List<string>();

                    foreach (MatrixItemByte item in foundSolution)
                    {
                        output.Add(item.ToString());
                    }

                    File.WriteAllLines(".\\Solution\\Good\\" + file, output);
                }

                MessageBox.Show("Found Solution with Count " + foundSolution.Count().ToString() + " Duration: " + duration.ToString());

                return foundSolution.Count();
            }

            MessageBox.Show("No Solution Found!" + " Duration: " + duration.ToString());
 
            return -1;
        }
    }

    
}