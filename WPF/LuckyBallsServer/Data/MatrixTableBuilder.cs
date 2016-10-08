using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Diagnostics;
using LuckyBallsData.Filters;

namespace LuckyBallsServer
{
    enum MatrixResult
    {
        Failed = 0,
        Succeeded = 1,
        Aborted = 2
    }

    class BuildMatrixUtil
    {
        public static UInt64[] Bits = new UInt64[33] 
        {
            1, 2,4,8,16,32,64,128,256,512,
            1024,2048,4096,8192,16384,32768, 65536,131072,262144, 524288,
            1048576,2097152,4194304,8388608, 16777216,33554432,67108864,134217728,268435456,
            536870912,1073741824, 2147483648,4294967296
        };

        public static int BitCount(UInt64 test)
        {
            int count = 0;
            UInt64 temp = test;
            while (temp != 0)
            {
                temp = temp & (temp - 1);
                ++count;
            }

            return count;
        }

        public static List<MatrixItemByte> GetDefaultSolution(int candidateCount, int selectCount, MatrixTable refTable)
        {
            // Get the solution from previous matrix results.
            if (refTable.GetCell(candidateCount - 1, selectCount) != null &&
                refTable.GetCell(candidateCount - 1, selectCount - 1) != null)
            {
                List<MatrixItemByte> result = new List<MatrixItemByte>();
                result.AddRange(refTable.GetCell(candidateCount - 1, selectCount).Template);

                foreach (MatrixItemByte filter in refTable.GetCell(candidateCount - 1, selectCount - 1).Template)
                {
                    MatrixItemByte item = new MatrixItemByte(selectCount, filter.Bits);
                    item.Add(candidateCount);

                    result.Add(item);
                }

                return result;
            }

            return null;
        }

        public static bool ValidateSolution(int candidateCount, int selectCount, List<MatrixItemByte> test)
        {
            MatrixBuildSettings settings = new MatrixBuildSettings(candidateCount, selectCount);

            MatrixItemPositionBits restItemsBits = new MatrixItemPositionBits(settings.TestItemCollection.Count, false);

            // Include the preselected items.
            int startIndex = 0;
            foreach (MatrixItemByte item in test)
            {
                for (int i = startIndex; i < settings.TestItemCollection.Count; ++i)
                {
                    if (settings.TestItemCollection[i].Bits == item.Bits)
                    {
                        restItemsBits.RemoveMultiple(settings.TestItemMashCollection[i]);
                        startIndex = i + 1;
                        break;
                    }
                }
            }

            return restItemsBits.UnhitCount == 0;
        }

        public static List<MatrixItemByte> GetTestItemCollection(int candidateCount, int selectCount)
        {
            List<MatrixItemByte> output = new List<MatrixItemByte>();

            // Get candidates.
            Byte[] candidates = new Byte[candidateCount];
            for (Byte i = 0; i < candidateCount; ++i)
            {
                candidates[i] = (Byte)(i + 1);
            }

            for (int i = 1; i <= candidateCount - selectCount + 1; ++i)
            {
                MatrixItemByte temp = new MatrixItemByte(selectCount);
                temp.Add(i);

                GetTestItemCollection(temp, candidates, i, selectCount - 1, ref output);
            }

            return output;
        }

        private static void GetTestItemCollection(MatrixItemByte sel, Byte[] candidates, int index, int selectCount, ref List<MatrixItemByte> output)
        {
            int insertAt = sel.Size - selectCount;

            if (selectCount == 1)
            {
                for (int i = index; i < candidates.Count(); ++i)
                {
                    MatrixItemByte copy = sel.Clone();
                    copy.Add(candidates[i]);

                    output.Add(copy);
                }
            }
            else
            {
                int endIndex = candidates.Count() - selectCount + 1;
                for (int i = index; i <= endIndex; ++i)
                {
                    MatrixItemByte copy = sel.Clone();
                    copy.Add(candidates[i]);

                    GetTestItemCollection(copy, candidates, i + 1, selectCount - 1, ref output);
                }
            }
        }
    }

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
            int startSeg = previous/ 64;
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

        private List<MatrixItemPositionBits>  BuildMash()
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
        private MatrixTable Target = null;
        public delegate int MatrixCalculationProgressHandler(string threadID, string message, double progress);
        public event MatrixCalculationProgressHandler MatrixProgressHandler = null;

        public MatrixTableBuilder(MatrixTable table)
        {
            Target = table;
        } 

        public void Init(bool bUseDefault)
        {
            Target.Init();

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

                        Target.SetCell(i, j, cell);
                    }
                    else if (bUseDefault)
                    {
                        List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(i, j, Target);
                        if (defaultSoution != null)
                        {
                            Target.SetCell(i, j, new MatrixCell() { Template = defaultSoution });

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
            Target.Init();

            if (Target.GetCell(row, col) != null)
                return Target.GetCellItemCount(row, col);

            // (re)Set the global settings data.
            MatrixBuildSettings settings = new MatrixBuildSettings(row, col);            
            
            List<MatrixItemByte> result = null;

            if (algorithm == 0) // exhaustion algorithm
            {
                ExhaustionAlgorithmImpl impl = new ExhaustionAlgorithmImpl(settings, Target, testLimit, MatrixProgressHandler);
                result = impl.Calculate();
            }
            else if (algorithm == 1) // greedy algorithm
            {
                GreedyAlgorithmImpl impl = new GreedyAlgorithmImpl(settings, Target, testLimit, MatrixProgressHandler);
                result = impl.Calculate();
            }
            else if (algorithm == 2) // inheritance algorithm
            {
                // Get the default matrix as the candidate solution.
                InheritanceAlgorithmImpl impl = new InheritanceAlgorithmImpl(settings, Target, testLimit, MatrixProgressHandler);
                result = impl.Calculate();
            }
            else if (algorithm == 3)
            {
                CoolingAlgorithmImpl impl = new CoolingAlgorithmImpl(settings, Target, testLimit, MatrixProgressHandler);
                result = impl.Calculate();
            }
            else if (algorithm == 4)
            {
                ExhaustionInParallelAlgorithmImpl impl = new ExhaustionInParallelAlgorithmImpl(settings, Target, testLimit, MatrixProgressHandler);
                result = impl.Calculate();
            }

            if (result != null)
            {
                if (MessageBox.Show("Result:" + result.Count.ToString() + ". Save it?", "Successful", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Target.SetCell(row, col, new MatrixCell() { Template = result });

                    // save to file.
                    string file = row.ToString() + "-" + col.ToString() + ".txt";

                    List<string> output = new List<string>();

                    foreach (MatrixItemByte item in result)
                    {
                        output.Add(item.ToString());
                    }

                    File.WriteAllLines("Z:\\matrix\\" + file, output);
                }

                return result.Count;
            }

            return -1;
        }
    }

    class ExhaustionAlgorithmImpl
    {
        private class BuildContext
        {
            private readonly MatrixBuildSettings _settings = null;

            private int _candidateCount = -1;
            private int _seleteCount = -1;
            
            // Algorithm settings.
            private int _testLimit = -1;
            public bool FindAnyReturn = false;

            // Currnt State
            public UInt64 NumBitsCovered = 0;
            public MatrixItemPositionBits RestItemsBits = null;
            private List<int> _numHitCounts = null;

            private MatrixItemPositionBits _numBitsToSkip = null;
            private int _maxHitCountForEach = -1;

            // Progress 
            public string progressMsg = "";
            public double progress = 0.0;
            public double progressRange = 100.0;
            public UInt64 CheckCount = 0;
            public UInt64 CheckCountForUpdateProgress = 0;
            public UInt64 CheckCountStep = 1000000;

            public BuildContext(MatrixBuildSettings settings)
            {
                _candidateCount = settings.CandidateNumCount;
                _seleteCount = settings.MatchNumCount + 1;
                _settings = settings;

                _numHitCounts = new List<int>(new int[_candidateCount]);
                _numBitsToSkip = new MatrixItemPositionBits(_settings.TestItemCollection.Count, true);
            }

            public int TestLimit
            {
                get
                {
                    return _testLimit;
                }
            }

            public void ResetTestLimit(int limit)
            {
                _testLimit = limit;

                // calculate the max num hit count.
                _maxHitCountForEach = ((_testLimit - 1) / _candidateCount + 1) * _seleteCount;
            }

            public void AddNumHits(MatrixItemByte item)
            {
                UInt64 mash = 1;
                for (int i = 0; i < _candidateCount; ++i)
                {
                    if ((mash & item.Bits) != 0)
                    {
                        ++_numHitCounts[i];

                        if (_numHitCounts[i] >= _maxHitCountForEach)
                            _numBitsToSkip.AddMultiple(_settings.NumDistributions[i]);
                    }

                    mash = mash << 1;
                }
            }

            public void RemoveNumHits(MatrixItemByte item)
            {
                UInt64 mash = 1;
                for (int i = 0; i < _candidateCount; ++i)
                {
                    if ((mash & item.Bits) != 0)
                    {
                        if (_numHitCounts[i] == _maxHitCountForEach)
                            _numBitsToSkip.RemoveMultiple(_settings.NumDistributions[i]);                  
                            
                        --_numHitCounts[i];
                    }

                    mash = mash << 1;
                }
            }

            public int NextItem(int pre)
            {
                return _numBitsToSkip.NextPosition(pre, false);
            }
        }

        private readonly MatrixBuildSettings Settings = null;
        private readonly MatrixTable Table = null;
        private int TestLimit = -1;
        public event MatrixTableBuilder.MatrixCalculationProgressHandler MatrixProgressHandler = null;

        public ExhaustionAlgorithmImpl(MatrixBuildSettings settings, MatrixTable refTable, int testLimit, 
            MatrixTableBuilder.MatrixCalculationProgressHandler processMonitor)
        {
            Settings = settings;
            Table = refTable;
            TestLimit = testLimit;
            MatrixProgressHandler = processMonitor;
        }

        public List<MatrixItemByte> Calculate()
        {
            // Build context.
            BuildContext context = new BuildContext(Settings);

            if (TestLimit > 0)
            {
                context.ResetTestLimit(TestLimit);
                context.FindAnyReturn = true;                
            }
            else
            {
                // Get the default matrix as the candidate solution.
                List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(Settings.CandidateNumCount, Settings.MatchNumCount + 1, Table);
                if (defaultSoution != null)
                {
                    Settings.CurrentSolution = defaultSoution;

                    context.ResetTestLimit(Settings.CurrentSolution.Count);                    
                }

                context.FindAnyReturn = false;
            } 

            if (MatrixProgressHandler != null)
            {
                string message = "Started...";
                MatrixProgressHandler("Main_Thread", message, 0);
            }

            // Prepare...
            context.RestItemsBits = new MatrixItemPositionBits(Settings.TestItemCollection.Count, false);

            List<MatrixItemByte> currentSelected = new List<MatrixItemByte>();

            // Include the first always.
            MatrixItemByte firstItem = Settings.TestItemCollection[0];
            currentSelected.Add(firstItem);
            CheckItem(0, firstItem, context);
            context.RestItemsBits.RemoveSingle(0);
            context.AddNumHits(firstItem);

            // Search the rest for possible soution.
            TraversalForAny(currentSelected, 1, context);

            if (MatrixProgressHandler != null)
            {
                string message = "Finished! Result: [" + Settings.CurrentSolution.Count.ToString() + "]";
                message += " CheckCount: " + context.CheckCount.ToString();
                MatrixProgressHandler("Main_Thread", message, 100);
            }

            return Settings.CurrentSolution;
        }

        private void CheckItem(int testIndex, MatrixItemByte testItem, BuildContext context)
        {
            if (++context.CheckCountForUpdateProgress > context.CheckCountStep)
            {
                ++context.CheckCount;
                context.CheckCountForUpdateProgress = 0;
            }

            context.RestItemsBits.RemoveMultiple(Settings.TestItemMashCollection[testIndex]);

            context.NumBitsCovered &= ~testItem.Bits;
        }

        private MatrixResult TraversalForAny(List<MatrixItemByte> currentSelected, int startIndex, BuildContext context)
        {  
            // back-up tests.
            MatrixItemPositionBits _restItems = context.RestItemsBits.Clone();
            UInt64 _unhitNums = context.NumBitsCovered;

            int selectedCount = currentSelected.Count; 

            int visitedCount = 0;
            int count = Settings.TestItemCollection.Count - context.TestLimit + selectedCount + 1;

            double progStart = context.progress;
            double progStep = context.progressRange / (count - startIndex);
            context.progressRange = progStep > 0.001 ? progStep : 0;
            UInt64 preCheckCount = context.CheckCount;

            bool bUpdateProgressMsg = currentSelected.Count == 1;

            for (int index = startIndex; index < count; ++index)
            {
                if (bUpdateProgressMsg)
                {
                    context.progressMsg = "[" + index.ToString() + "] ";
                }

                if (MatrixProgressHandler != null)
                {
                    if (preCheckCount != context.CheckCount)
                    {
                        preCheckCount = context.CheckCount;

                        context.progress = progStart + progStep * visitedCount;

                        string message = context.progressMsg + context.progress.ToString("f3") + "%";
                        message += " CURRENT: " + (Settings.CurrentSolution != null ? Settings.CurrentSolution.Count.ToString() : "-1");
                        message += " CHECKED: " + context.CheckCount.ToString();

                        int result = MatrixProgressHandler("Main_Thread", message, context.progress);
                        if (result < 0)
                        {
                            return MatrixResult.Aborted;
                        }
                    }
                }

                ++visitedCount;

                MatrixItemByte testItem = Settings.TestItemCollection[index];

                currentSelected.Add(testItem);
                context.AddNumHits(testItem);
 
                // Check the filter.
                CheckItem(index, testItem, context);

                // do we get a solution? check only if the current solution has more steps than the ideal.
                if (currentSelected.Count > Settings.IdealMinStepCount && context.RestItemsBits.IsClean())
                {
                    // Save solution to context.
                    Settings.CurrentSolution = currentSelected.ToList();

                    context.ResetTestLimit(Settings.CurrentSolution.Count);
                    currentSelected.RemoveAt(currentSelected.Count - 1);
                    context.RemoveNumHits(testItem);

                    if (context.FindAnyReturn)
                    {
                        return MatrixResult.Aborted;
                    }

                    // Get it!
                    if (MatrixProgressHandler != null)
                    {
                        context.progress = progStart + progStep * visitedCount;

                        string message = context.progressMsg + context.progress.ToString("f3") + "%";
                        message = " CURRENT:" + Settings.CurrentSolution.Count.ToString();
                        message += " CHECKED: " + context.CheckCount.ToString();

                        MatrixProgressHandler("Main_Thread", message, context.progress);
                    }

                    return MatrixResult.Succeeded; // return this solution and no need to continue.
                }

                // Check the current solution.
                if (PreCheckSolution(currentSelected, context))
                {
                    int next = context.NextItem(index);
                    // if we got a valid solution, check if need to continue or not.
                    MatrixResult res = TraversalForAny(currentSelected, next, context);
                    if (res == MatrixResult.Aborted)
                    {
                        currentSelected.RemoveAt(currentSelected.Count - 1);
                        context.RemoveNumHits(testItem);
                        return MatrixResult.Aborted;
                    }

                    if (res == MatrixResult.Succeeded && Settings.CurrentSolution.Count <= selectedCount + 2)
                    {
                        currentSelected.RemoveAt(currentSelected.Count - 1);
                        context.RemoveNumHits(testItem);
                        return MatrixResult.Succeeded;// no need to continue the check.
                    }
                }

                // recover the tests and continue.
                context.RemoveNumHits(testItem);
                currentSelected.RemoveAt(currentSelected.Count - 1);
                _restItems.CopyTo(context.RestItemsBits);
                context.NumBitsCovered = _unhitNums;
            }

            return MatrixResult.Failed;
        }

        // Check the current incomplete solution and determine if need to continue or not.
        private bool PreCheckSolution(List<MatrixItemByte> testSolution, BuildContext context)
        {                
            // Get current test limit.
            int expectStepCount = context.TestLimit - 1;

            // No need to continue if the current solution already has no chance to get less steps than the existing one.
            int restStep = expectStepCount - testSolution.Count;
            if (restStep <= 0)
                return false;

            // check the number coverage.
            int restUncoveredNumCount = BuildMatrixUtil.BitCount(context.NumBitsCovered);
            if (restUncoveredNumCount > restStep * (Settings.MatchNumCount + 1))
                return false;

            // The max item count can be covered by rest steps. 
            if (context.RestItemsBits.UnhitCount > Settings.MaxItemCountCoveredByOneItem * restStep)
                return false;

            return true; // let's continue.
        }
    }

    class GreedyAlgorithmImpl
    {
        private class BuildContext
        {
            // Currnt State
            public MatrixItemPositionBits RestItemsBits = null;
            public int ThresholdInParallel = 0;
            public int LoopCountLimit = 1;

            // Progress 
            public double progress = 0.0;
            public double progressRange = 100.0;

            public BuildContext Clone()
            {
                BuildContext copy = new BuildContext();

                copy.progress = progress;
                copy.progressRange = progressRange;

                copy.RestItemsBits = RestItemsBits != null ? RestItemsBits.Clone() : null;
                copy.ThresholdInParallel = ThresholdInParallel;
                copy.LoopCountLimit = LoopCountLimit;

                return copy;
            }
        }

        private MatrixBuildSettings Settings = null;
        private MatrixTable Table = null;
        public event MatrixTableBuilder.MatrixCalculationProgressHandler MatrixProgressHandler = null;
        public MatrixItemPositionBits BestSolution = null;
        private int threadIndex = 0;
        private int TestLimit = -1;

        public GreedyAlgorithmImpl(MatrixBuildSettings settings, MatrixTable refTable, int limit, 
            MatrixTableBuilder.MatrixCalculationProgressHandler processMonitor)
        {
            Settings = settings;
            Table = refTable;
            MatrixProgressHandler = processMonitor;
            TestLimit = limit;
        }

        public List<MatrixItemByte> Calculate()
        {
            // Build context.
            BuildContext context = new BuildContext();

            if (MatrixProgressHandler != null)
            {
                string message = "Started...";
                MatrixProgressHandler("Main_Thread", message, 0);
            }

            // Prepare...
            context.RestItemsBits = new MatrixItemPositionBits(Settings.TestItemCollection.Count, false);
            context.ThresholdInParallel = Settings.CandidateNumCount - Settings.MatchNumCount;

            MatrixItemPositionBits currentSelected = new MatrixItemPositionBits(Settings.TestItemCollection.Count, true);

            // Include the first and the last one.
            currentSelected.AddSingle(0);
            context.RestItemsBits.RemoveMultiple(Settings.TestItemMashCollection[0]);

            // Search the rest for possible soution.            
            TraversalForBest(currentSelected, context.RestItemsBits, context, false);

            // Did we find one?
            if (BestSolution != null)
            {
                List<MatrixItemByte> result = new List<MatrixItemByte>();
                int count = Settings.TestItemCollection.Count;
                for (int index = 0; index < count; ++index)
                {
                    if (BestSolution.Contains(index))
                        result.Add(Settings.TestItemCollection[index]);
                }

                if (MatrixProgressHandler != null)
                {
                    string message = "Succeeded! Result: [" + result.Count.ToString() + "]";
                    MatrixProgressHandler("Main_Thread", message, 100);
                }

                return result;
            }
            else
            {
                if (MatrixProgressHandler != null)
                {
                    string message = "Failed!";
                    MatrixProgressHandler("Main_Thread", message, 100);
                }

                return null;
            }
        }

        private MatrixResult TraversalForBest(MatrixItemPositionBits currentSelected, MatrixItemPositionBits restItems, BuildContext context, bool bInParallel)
        {
            if (BestSolution != null && currentSelected.UnhitCount >= BestSolution.UnhitCount - 1)
                return MatrixResult.Failed;

            if (TestLimit > 0 &&  currentSelected.UnhitCount >= TestLimit - 1)
                return MatrixResult.Failed;

            List<int> nextCandidates = new List<int>();
            List<MatrixItemPositionBits> nextRestItems = new List<MatrixItemPositionBits>();
            int count = Settings.TestItemCollection.Count;

            if (Settings.CandidateNumCount > (currentSelected.UnhitCount + 1) * (Settings.MatchNumCount + 1))
            {
                // Find the next  by checking the item cover count.
                UInt64 numBits = 0;
                for (int index = 0; index < count; ++index)
                {
                    if (currentSelected.Contains(index))
                    {
                        numBits |= Settings.TestItemCollection[index].Bits;
                    }
                    else
                    {
                        if ((numBits & Settings.TestItemCollection[index].Bits) == 0)
                        {
                            nextCandidates.Add(index);

                            MatrixItemPositionBits temp = restItems.Clone();
                            temp.RemoveMultiple(Settings.TestItemMashCollection[index]);
                            nextRestItems.Add(temp);
                            break;
                        }
                    }
                }
            }
            else
            {     
                int coveredByNext = -1;
                for (int index = 0; index < count; ++index)
                {
                    if (currentSelected.Contains(index))
                        continue;

                    MatrixItemPositionBits temp = restItems.Clone();
                    temp.RemoveMultiple(Settings.TestItemMashCollection[index]);

                    UInt64 bits = Settings.TestItemCollection[index].Bits;

                    int newlyCoveredItemCount = restItems.UnhitCount - temp.UnhitCount;

                    if (newlyCoveredItemCount >= coveredByNext)
                    {
                        if (newlyCoveredItemCount > coveredByNext)
                        {
                            nextCandidates.Clear();
                            nextRestItems.Clear();
                            coveredByNext = newlyCoveredItemCount;
                        }

                        nextCandidates.Add(index);
                        nextRestItems.Add(temp);

                        if (coveredByNext == Settings.MaxItemCountCoveredByOneItem || temp.UnhitCount == 0)
                        {
                            // it has been the best, just return.
                            break;
                        }
                    }
                }

                if (coveredByNext <= 0 || nextRestItems.Count == 0)
                {
                    return MatrixResult.Failed;
                }
            }

            if (nextRestItems.Count > 0 && nextRestItems[0].UnhitCount == 0)
            {
                // End the searching and check the solution.
                if (BestSolution == null)
                {
                    BestSolution = currentSelected.Clone();
                    BestSolution.AddSingle(nextCandidates[0]);
                }
                else if (currentSelected.UnhitCount + 1 < BestSolution.UnhitCount)
                {
                    currentSelected.CopyTo(BestSolution);
                    BestSolution.AddSingle(nextCandidates[0]);
                }
                else
                {
                    return MatrixResult.Failed;
                }

                if (MatrixProgressHandler != null)
                {
                    string message = "PROGRESS: " + context.progress.ToString("f3") + "%";
                    message += " CURRENT: " + (BestSolution != null ? BestSolution.UnhitCount.ToString() : "-1");
                    message += " LEVEL: " + "unknown";
                    MatrixProgressHandler(Thread.CurrentThread.Name != null ? Thread.CurrentThread.Name : "Main_Thread", message, context.progress);
                }

                return MatrixResult.Succeeded;
            } 

            // if it has no chance to get the best, skip.
            if (BestSolution != null && currentSelected.UnhitCount + 2 >= BestSolution.UnhitCount)
                return MatrixResult.Failed;

            // continue...
            int searchLimit = context.LoopCountLimit > 0 ? Math.Min(nextCandidates.Count, context.LoopCountLimit) : nextCandidates.Count;

            bool bStartInParallel = !bInParallel && currentSelected.UnhitCount >= context.ThresholdInParallel && searchLimit > 1;
            if (bStartInParallel)
            {
                if (MatrixProgressHandler != null)
                {
                    string message = "PROGRESS: " + context.progress.ToString("f3") + "%";
                    message += " CURRENT: " + (BestSolution != null ? BestSolution.UnhitCount.ToString() : "-1");
                    message += " LEVEL: " + currentSelected.UnhitCount.ToString() + " Sub Threads: " + searchLimit.ToString();
                    MatrixProgressHandler("Main_Thread", message, context.progress);
                }

                bool bAborted = false;
                ParallelOptions option = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
                ParallelLoopResult loopResult = Parallel.For(0, searchLimit, option, (Index) =>
                {
                    if (!bAborted)
                    {
                        if (Thread.CurrentThread.Name == null)
                        {
                            Thread.CurrentThread.Name = "Thread_" + (++threadIndex).ToString();
                        }

                        // Clone a context object and reset the progress.
                        BuildContext copyContext = context.Clone();
                        copyContext.progress = 0;
                        copyContext.progressRange = 100;

                        MatrixItemPositionBits copiedSelected = currentSelected.Clone();
                        copiedSelected.AddSingle(nextCandidates[Index]);

                        if (TraversalForBest(copiedSelected, nextRestItems[Index], copyContext, true) == MatrixResult.Aborted)
                        {
                            bAborted = true;
                        }
                    }

                    MatrixProgressHandler(Thread.CurrentThread.Name, "", -1);
                });

                return bAborted ? MatrixResult.Aborted : MatrixResult.Succeeded;
            }
            else
            {
                double progStep = context.progressRange / searchLimit;
                if (progStep < 0.001)
                    progStep = 0;

                double progress = context.progress;
                context.progressRange = progStep;

                for (int i = 0; i < searchLimit; ++i)
                {
                    currentSelected.AddSingle(nextCandidates[i]);

                    if (TraversalForBest(currentSelected, nextRestItems[i], context, bInParallel) == MatrixResult.Aborted)
                        return MatrixResult.Aborted;

                    // restore the selection.
                    currentSelected.RemoveSingle(nextCandidates[i]);

                    //if (progStep > 0)
                    {
                        progress += progStep;

                        string message = "PROGRESS: " + progress.ToString("f3") + "%";
                        message += " CURRENT: " + (BestSolution != null ? BestSolution.UnhitCount.ToString() : "-1");
                        message += " LEVEL: " + currentSelected.UnhitCount.ToString() + " (" + i.ToString() + " of " + searchLimit.ToString() + ")";
                        int result = MatrixProgressHandler(bInParallel ? Thread.CurrentThread.Name : "Main_Thread", message, progress);
                        if (result < 0)
                        {
                            return MatrixResult.Aborted;
                        }
                    }
                }

                return MatrixResult.Succeeded;
            }
        }
    }

    class InheritanceAlgorithmImpl
    {
        private class BuildContext
        {
            // Currnt State
            public MatrixItemPositionBits RestItemsBits = null;

            // Progress 
            public double progress = 0.0;
            public double progressRange = 100.0;
            public Int64 CheckCount = 0;            
        }

        private MatrixBuildSettings Settings = null;
        private MatrixTable Table = null;
        public event MatrixTableBuilder.MatrixCalculationProgressHandler MatrixProgressHandler = null;
        public int TestLimit = -1;

        public InheritanceAlgorithmImpl(MatrixBuildSettings settings, MatrixTable refTable, int limit, MatrixTableBuilder.MatrixCalculationProgressHandler processMonitor)
        {
            Settings = settings;
            Table = refTable;
            MatrixProgressHandler = processMonitor;
            TestLimit = limit;
        }

        public List<MatrixItemByte> Calculate()
        {
            if (MatrixProgressHandler != null)
            {
                string message = "Started...";
                MatrixProgressHandler("Main_Thread", message, 0);
            }

            // Build context.
            BuildContext context = new BuildContext();
            context.RestItemsBits = new MatrixItemPositionBits(Settings.TestItemCollection.Count, false);

            if (TestLimit < 0)
            {
                List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(Settings.CandidateNumCount, Settings.MatchNumCount + 1, Table);
                if (defaultSoution != null)
                {
                    Settings.CurrentSolution = defaultSoution;
                    TestLimit = defaultSoution.Count - 1;
                }
            }

            // inherit the selection from previous cell.
            List<MatrixItemByte> preSelected = Table.GetCell(Settings.CandidateNumCount - 1, Settings.MatchNumCount + 1).Template;

            List<MatrixItemByte> currentSelected = new List<MatrixItemByte>();

            // Include the preselected items.
            int lastNum = Settings.CandidateNumCount;
            int startIndex = 0;
            foreach (MatrixItemByte item in preSelected)
            {
                // replace "01" with the last number.
                int[] orginal = item.Indices;
                MatrixItemByte replacedItem = new MatrixItemByte(Settings.MatchNumCount + 1);
                foreach (int num in orginal)
                {
                    if (num == 1)
                    {
                        replacedItem.Add(lastNum);
                    }
                    else
                    {
                        replacedItem.Add(num);
                    }
                }

                for (int i = startIndex; i < Settings.TestItemCollection.Count; ++i)
                {
                    if (Settings.TestItemCollection[i].Bits == replacedItem.Bits)
                    {
                        currentSelected.Add(Settings.TestItemCollection[i]);
                        context.RestItemsBits.RemoveMultiple(Settings.TestItemMashCollection[i]);

                        startIndex = i + 1;
                        break;
                    }
                }
            }

            // Search the rest for possible soution.
            TraversalForAnyWithFirst(currentSelected, 0, context);

            if (MatrixProgressHandler != null)
            {
                string message = "Finished!";
                message += " CheckCount: " + context.CheckCount.ToString();
                MatrixProgressHandler("Main_Thread", message, 100);
            }

            return Settings.CurrentSolution;
        }

        private MatrixResult TraversalForAnyWithFirst(List<MatrixItemByte> currentSelected, int startIndex, BuildContext context)
        {
            // back-up tests.
            MatrixItemPositionBits _restItems = context.RestItemsBits.Clone();

            int selectedCount = currentSelected.Count;

            int hitCount = 0;
            int count = Settings.ItemCountInLoopLevels[0];

            double progStart = context.progress;
            double progStep = context.progressRange / (count - startIndex);
            context.progressRange = progStep > 0.01 ? progStep : 0;

            for (int index = startIndex; index < count; ++index)
            {
                if (MatrixProgressHandler != null)
                {
                    if (progStep > 0 && progStep * hitCount > 0.01)
                    {
                        context.progress = progStart + progStep * hitCount;

                        string message = "PROGRESS: " + context.progress.ToString("f3") + "%";
                        message += " CURRENT: " + (Settings.CurrentSolution != null ? Settings.CurrentSolution.Count.ToString() : "-1");
                        message += " CHECKED: " + context.CheckCount.ToString();

                        int result = MatrixProgressHandler("Main_Thread", message, context.progress);
                        if (result < 0)
                        {
                            return MatrixResult.Aborted;
                        }
                    }
                }

                MatrixItemByte testItem = Settings.TestItemCollection[index];
                currentSelected.Add(testItem);

                // Check the filter.
                context.RestItemsBits.RemoveMultiple(Settings.TestItemMashCollection[index]);

                // do we get a solution? check only if the current solution has more steps than the ideal.
                if (currentSelected.Count >= Settings.IdealMinStepCount && context.RestItemsBits.IsClean())
                {
                    // Save solution to context.
                    Settings.CurrentSolution = currentSelected.ToList();
                    TestLimit = Settings.CurrentSolution.Count - 1;
                    currentSelected.RemoveAt(currentSelected.Count - 1);

                    // Get it!
                    if (MatrixProgressHandler != null)
                    {
                        context.progress = progStart + progStep * hitCount;

                        string message = "PROGRESS: " + context.progress.ToString("f3") + "%";
                        message = " CURRENT:" + Settings.CurrentSolution.Count.ToString();
                        message += " CHECKED: " + context.CheckCount.ToString();

                        MatrixProgressHandler("Main_Thread", message, context.progress);
                    }

                    return MatrixResult.Succeeded; // return this solution and no need to continue.
                }

                // Check the current solution.
                if (PreCheckSolution(currentSelected, context))
                {
                    // if we got a valid solution, check if need to continue or not.
                    MatrixResult res = TraversalForAnyWithFirst(currentSelected, index + 1, context);
                    if (res == MatrixResult.Aborted)
                    {
                        currentSelected.RemoveAt(currentSelected.Count - 1);
                        return MatrixResult.Aborted;
                    }

                    if (res == MatrixResult.Succeeded && Settings.CurrentSolution.Count <= selectedCount + 2)
                    {
                        currentSelected.RemoveAt(currentSelected.Count - 1);
                        return MatrixResult.Succeeded;// no need to continue the check.
                    }
                }

                // recover the tests and continue.
                currentSelected.RemoveAt(currentSelected.Count - 1);
                _restItems.CopyTo(context.RestItemsBits);

                ++hitCount;
            }

            return MatrixResult.Failed;
        }

        // Check the current incomplete solution and determine if need to continue or not.
        private bool PreCheckSolution(List<MatrixItemByte> testSolution, BuildContext context)
        {
            // No need to continue if the current solution already has no chance to get less steps than the existing one.
            int restStep = TestLimit - testSolution.Count;
            if (restStep <= 0)
                return false;

            // The max item count can be covered by rest steps. 
            if (context.RestItemsBits.UnhitCount > Settings.MaxItemCountCoveredByOneItem * restStep)
                return false;

            return true; // let's continue.
        }
    }

    class CoolingAlgorithmImpl
    {
        private class BuildContext
        {
            // Currnt State
            public MatrixItemPositionBits RestItemsBits = null;

            // Progress 
            public double progress = 0.0;
            public double progressRange = 100.0;

            public BuildContext Clone()
            {
                BuildContext copy = new BuildContext();

                copy.progress = progress;
                copy.progressRange = progressRange;

                copy.RestItemsBits = RestItemsBits != null ? RestItemsBits.Clone() : null;

                return copy;
            }
        }

        private MatrixBuildSettings Settings = null;
        private MatrixTable Table = null;
        public event MatrixTableBuilder.MatrixCalculationProgressHandler MatrixProgressHandler = null;
        public MatrixItemPositionBits BestSolution = null;
        private int TestLimit = -1;
        private int MaxIntersection = -1;

        public CoolingAlgorithmImpl(MatrixBuildSettings settings, MatrixTable refTable, int limit,
            MatrixTableBuilder.MatrixCalculationProgressHandler processMonitor)
        {
            Settings = settings;
            Table = refTable;
            MatrixProgressHandler = processMonitor;
            TestLimit = limit;
            MaxIntersection = Math.Min(settings.CandidateNumCount - settings.MatchNumCount - 1, settings.MatchNumCount + 1);
        }

        public List<MatrixItemByte> Calculate()
        {
            // Build context.
            BuildContext context = new BuildContext();

            if (MatrixProgressHandler != null)
            {
                string message = "Started...";
                MatrixProgressHandler("Main_Thread", message, 0);
            }

            // Prepare...
            context.RestItemsBits = new MatrixItemPositionBits(Settings.TestItemCollection.Count, false);

            MatrixItemPositionBits currentSelected = new MatrixItemPositionBits(Settings.TestItemCollection.Count, true);

            // Include the first and the last one.
            currentSelected.AddSingle(0);
            context.RestItemsBits.RemoveMultiple(Settings.TestItemMashCollection[0]);

            // Search the rest for possible soution.            
            TraversalForNext(currentSelected, 0, context.RestItemsBits, context, false);

            // Did we find one?
            if (BestSolution != null)
            {
                List<MatrixItemByte> result = new List<MatrixItemByte>();
                int count = Settings.TestItemCollection.Count;
                for (int index = 0; index < count; ++index)
                {
                    if (BestSolution.Contains(index))
                        result.Add(Settings.TestItemCollection[index]);
                }

                if (MatrixProgressHandler != null)
                {
                    string message = "Succeeded! Result: [" + result.Count.ToString() + "]";
                    MatrixProgressHandler("Main_Thread", message, 100);
                }

                return result;
            }
            else
            {
                if (MatrixProgressHandler != null)
                {
                    string message = "Failed!";
                    MatrixProgressHandler("Main_Thread", message, 100);
                }

                return null;
            }
        }

        private MatrixResult TraversalForNext(MatrixItemPositionBits currentSelected, int previous, MatrixItemPositionBits restItems, BuildContext context, bool bInParallel)
        {
            if (BestSolution != null && currentSelected.UnhitCount >= BestSolution.UnhitCount - 1)
                return MatrixResult.Failed;

            if (TestLimit > 0 && currentSelected.UnhitCount >= TestLimit - 1)
                return MatrixResult.Failed;

            int nextIndex = -1;
            MatrixItemPositionBits nextRestItems = null;
            int count = Settings.TestItemCollection.Count;

            if (Settings.CandidateNumCount > (currentSelected.UnhitCount + 1) * (Settings.MatchNumCount + 1))
            {
                // Find the next  by checking the item cover count.
                UInt64 numBits = 0;
                for (int index = 0; index < count; ++index)
                {
                    if (currentSelected.Contains(index))
                    {
                        numBits |= Settings.TestItemCollection[index].Bits;
                    }
                    else
                    {
                        if ((numBits & Settings.TestItemCollection[index].Bits) == 0)
                        {
                            nextIndex = index;

                            MatrixItemPositionBits temp = restItems.Clone();
                            temp.RemoveMultiple(Settings.TestItemMashCollection[index]);
                            nextRestItems = temp;
                            break;
                        }
                    }
                }
            }
            else
            {
                MatrixItemByte preItem = Settings.TestItemCollection[previous];

                //int coveredByNext = -1;
                for (int index = 0; index < count; ++index)
                {
                    if (currentSelected.Contains(index))
                        continue;

                    MatrixItemByte testItem = Settings.TestItemCollection[index];
                    if (preItem.Intersection(testItem) != MaxIntersection)
                        continue;

                    MatrixItemPositionBits temp = restItems.Clone();
                    temp.RemoveMultiple(Settings.TestItemMashCollection[index]);

                    UInt64 bits = Settings.TestItemCollection[index].Bits;

                    int newlyCoveredItemCount = restItems.UnhitCount - temp.UnhitCount;

                    // TODO: not finished,
                    //if (newlyCoveredItemCount >= coveredByNext)
                    //{
                    //    if (newlyCoveredItemCount > coveredByNext)
                    //    {
                    //        nextCandidates.Clear();
                    //        nextRestItems.Clear();
                    //        coveredByNext = newlyCoveredItemCount;
                    //    }

                    //    nextCandidates.Add(index);
                    //    nextRestItems.Add(temp);

                    //    if (coveredByNext == Settings.MaxItemCountCoveredByOneItem || temp.UnhitCount == 0)
                    //    {
                    //        // it has been the best, just return.
                    //        break;
                    //    }
                    //}
                }
            }

            if (nextRestItems != null && nextRestItems.UnhitCount == 0)
            {
                // End the searching and check the solution.
                if (BestSolution == null)
                {
                    BestSolution = currentSelected.Clone();
                    BestSolution.AddSingle(nextIndex);
                }
                else if (currentSelected.UnhitCount + 1 < BestSolution.UnhitCount)
                {
                    currentSelected.CopyTo(BestSolution);
                    BestSolution.AddSingle(nextIndex);
                }
                else
                {
                    return MatrixResult.Failed;
                }

                if (MatrixProgressHandler != null)
                {
                    string message = "PROGRESS: " + context.progress.ToString("f3") + "%";
                    message += " CURRENT: " + (BestSolution != null ? BestSolution.UnhitCount.ToString() : "-1");
                    message += " LEVEL: " + "unknown";
                    MatrixProgressHandler(Thread.CurrentThread.Name != null ? Thread.CurrentThread.Name : "Main_Thread", message, context.progress);
                }

                return MatrixResult.Succeeded;
            }

            // if it has no chance to get the best, skip.
            if (BestSolution != null && currentSelected.UnhitCount + 2 >= BestSolution.UnhitCount)
                return MatrixResult.Failed;

            // continue...
            currentSelected.AddSingle(nextIndex);

            return TraversalForNext(currentSelected, nextIndex, nextRestItems, context, bInParallel);
        }
    }

    class ExhaustionInParallelAlgorithmImpl
    {
        private class BuildContext
        {
            private readonly int CandidateNumCount = -1;
            public readonly int IdealMinStepCount = 0;
            public readonly List<MatrixItemByte> TestItemCollection = null;

            public bool FindAnyReturn = false;
            public int TestStepCount = 2147483647;

            // temp data for solution extraction
            private List<int> _currentSolution = null;
            private List<UInt64> _numCoverageInLevels = null;
            private UInt64 _targetNumCoverage = 0;

            public BuildContext(int _candidateNumCount, int _selectNumCount, List<MatrixItemByte> testItems)
            {
                CandidateNumCount = _candidateNumCount;
                TestItemCollection = testItems;

                int maxItemCovered = (_candidateNumCount - _selectNumCount) * _selectNumCount + 1;

                IdealMinStepCount = (TestItemCollection.Count - 1) / maxItemCovered + 1;

                _targetNumCoverage = (((UInt64)1) << _candidateNumCount) - 1;                    
            }     
       
            public List<int> _GetCurrentSolution()
            {
                return _currentSolution;
            }

            public List<List<int>> GetValidSolutions(int resultCount)
            {
                // prepare data...
                int stepLimit = TestStepCount;
                int itemCount = TestItemCollection.Count;

                // Init current solution.
                if (_currentSolution == null)
                {
                    _currentSolution = new List<int>();
                    _numCoverageInLevels = new List<UInt64>();

                    UInt64 lastLevelCoverage = 0;
                    for (int i = 0; i < stepLimit; ++i)
                    {
                        _currentSolution.Add(i);
                        lastLevelCoverage |= TestItemCollection[i].Bits;
                        _numCoverageInLevels.Add(lastLevelCoverage);
                    }
                }

                if (stepLimit > _currentSolution.Count)
                    throw new Exception("asking for worse solution?");
                else if (stepLimit < _currentSolution.Count)
                {
                    _currentSolution.RemoveRange(stepLimit, _currentSolution.Count - stepLimit);
                    _numCoverageInLevels.RemoveRange(stepLimit, _currentSolution.Count - stepLimit);
                }

                List<List<int>> output = new List<List<int>>();

                int index = 1;
                bool bContinue = true;
                while (bContinue)
                {
                    if (Next(stepLimit - 1, stepLimit, itemCount) < 0)
                    {
                        break; // has find the last one.
                    }

                    if (_numCoverageInLevels[stepLimit - 1] == _targetNumCoverage)
                    {
                        output.Add(_currentSolution.ToList());

                        ++index;

                        if (resultCount > 0 && index > resultCount)
                            break; // get enough.
                    }
                }

                return output;
            }

            private int Next(int index, int stepLimit, int itemCount)
            {
                int newVal = -1;
                if (_currentSolution[index] > itemCount - stepLimit + index - 1)
                {
                    // need go back to the start.
                    if (index == 0)
                        return -1;
                    else
                    {
                        int lowInPrevious = Next(index - 1, stepLimit, itemCount);
                        if (lowInPrevious < 0)
                            return -1;

                        newVal = lowInPrevious + 1;
                    }
                }
                else
                {
                    // Increase the value at this position.
                    newVal = _currentSolution[index] + 1;
                }

                _currentSolution[index] = newVal;
                _numCoverageInLevels[index] = _numCoverageInLevels[index - 1] | TestItemCollection[newVal].Bits;

                return newVal;
            }
        }

        private class LoopParam
        {
            public List<List<int>> TESTS = null;
            public List<int> RESULT;
            public bool FINISHED = false;
        };

        private MatrixBuildSettings Settings = null;
        private MatrixTable Table = null;
        private int TestLimit = -1;
        public event MatrixTableBuilder.MatrixCalculationProgressHandler MatrixProgressHandler = null;
        private BuildContext context = null;

        public ExhaustionInParallelAlgorithmImpl(MatrixBuildSettings settings, MatrixTable refTable, int testLimit,
            MatrixTableBuilder.MatrixCalculationProgressHandler processMonitor)
        {
            Settings = settings;
            Table = refTable;
            TestLimit = testLimit;
            MatrixProgressHandler = processMonitor;
        }

        public List<MatrixItemByte> Calculate()
        {
            // Build context.
            context = new BuildContext(Settings.CandidateNumCount, Settings.MatchNumCount + 1, Settings.TestItemCollection);
            
            if (TestLimit > 0)
            {
                context.TestStepCount = TestLimit;
                context.FindAnyReturn = true;
            }
            else
            {
                // Get the default matrix as the candidate solution.
                List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(Settings.CandidateNumCount, Settings.MatchNumCount + 1, Table);
                if (defaultSoution != null)
                {
                    context.TestStepCount = defaultSoution.Count - 1;
                }
            }

            List<int> bestSolution = null;
            int processedCount = 0;
            while(true)
            {  
                CancellationTokenSource cts = new CancellationTokenSource();

                ParallelOptions option = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
                option.CancellationToken = cts.Token;

                try
                {
                    ParallelLoopResult loopResult = Parallel.For<LoopParam>(0, Environment.ProcessorCount + 1, option, 
                        () => new LoopParam()
                        {
                            TESTS = context.GetValidSolutions(200000),
                            RESULT = null,
                            FINISHED = false
                        },
                        (threadIndex, loopState, param) =>
                        {
                            option.CancellationToken.ThrowIfCancellationRequested();

                            // Get all tests.
                            if (param.TESTS.Count == 0)
                            {
                                param.FINISHED = true;
                                return param;
                            }

                            param.RESULT = FindBestSolution(param.TESTS);
                            if (param.RESULT != null && param.RESULT.Count > 0 && param.RESULT.Count < context.TestStepCount)
                            {
                                return param;
                            }
                            else
                            {
                                param.RESULT = null;
                                return param;
                            }
                        },
                        (param) =>
                        {
                            if (param.FINISHED)
                            {
                                cts.Cancel();
                            }
                            else if (param.RESULT != null)
                            {
                                context.TestStepCount = param.RESULT.Count - 1;
                                bestSolution = param.RESULT;

                                if (context.FindAnyReturn)
                                    cts.Cancel();
                            }

                            if (MatrixProgressHandler != null)
                            {
                                string message = "Processing: [";
                                var current = context._GetCurrentSolution();
                                for (int i = 0; i < 3; ++ i)
                                {
                                    message += current[i] + " ";
                                }
                                message += "...]";
                                message += " Processed: " + (++processedCount).ToString();
                                message += " Best Now: " + (bestSolution != null ? bestSolution.Count.ToString() : context.TestStepCount.ToString());

                                int result = MatrixProgressHandler("Main thread", message, 50);
                                if (result < 0)
                                {
                                    cts.Cancel();
                                }
                            }
                        }
                    );
                }
                catch (OperationCanceledException e)
                {
                    cts.Dispose();
                    break;
                }                
            }            

            if (bestSolution != null)
            {
                List<MatrixItemByte> result = new List<MatrixItemByte>();

                int count = Settings.TestItemCollection.Count;
                for (int index = 0; index < count; ++index)
                {
                    if (bestSolution.Contains(index))
                        result.Add(Settings.TestItemCollection[index]);
                }

                return result;
            }

            return null;
        }

        private void CheckItem(int testIndex, MatrixItemPositionBits restItems)
        {
            restItems.RemoveMultiple(Settings.TestItemMashCollection[testIndex]);
        }

        private List<int> FindBestSolution(List<List<int>> tests)
        {
            // Search the rest for possible soution.
            int count = tests.Count;
            List<int> bestSolution = null;

            for (int i = 0; i < count; ++i)
            {
                MatrixItemPositionBits restItemBits = new MatrixItemPositionBits(Settings.TestItemCollection.Count, false);

                int testCount = (bestSolution != null && bestSolution.Count < context.TestStepCount) ? bestSolution.Count : context.TestStepCount;
                List<int> solution = tests[i];
                for (int index = 0; index < testCount; ++index)
                {
                    // Check the filter.
                    CheckItem(solution[index], restItemBits);

                    // do we get a solution? check only if the current solution has more steps than the ideal.
                    if (index >= context.IdealMinStepCount && restItemBits.IsClean())
                    {
                        // Save solution to context.
                        bestSolution = (index == solution.Count) ? solution : solution.GetRange(0, index + 1);
                        break;
                    }
                }
            }

            return bestSolution;
        }
    }

}
