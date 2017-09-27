using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixBuilder
{
    class BuildContext
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
        private int[] _numHitCounts = null;

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

            _numHitCounts = new int[_candidateCount];
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
            // calculate the max num hit count.
            _maxHitCountForEach = ((limit - 1) / _candidateCount + 1) * _seleteCount;

            if (limit < _testLimit)
            {
                for (int i = 0; i < _candidateCount; ++i)
                {
                    if (_numHitCounts[i] >= _maxHitCountForEach)
                        _numBitsToSkip.AddMultiple(_settings.NumDistributions[i]);
                }
            }

            _testLimit = limit;
        }

        public void AddNumHits(MatrixItemByte item)
        {
            UInt64 mash = 1;
            for (int i = 0; i < _candidateCount; ++i)
            {
                if ((mash & item.Bits) != 0)
                {
                    ++_numHitCounts[i];

                    if (_numHitCounts[i] == _maxHitCountForEach)
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

    class ExhaustionAlgorithmImpl
    {
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

            Stack<MatrixItemByte> currentSelected = new Stack<MatrixItemByte>();

            // Include the first always.
            MatrixItemByte firstItem = Settings.TestItemCollection[0];
            currentSelected.Push(firstItem);
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

        private MatrixResult TraversalForAny(Stack<MatrixItemByte> currentSelected, int startIndex, BuildContext context)
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

                currentSelected.Push(testItem);
                context.AddNumHits(testItem);

                // Check the filter.
                CheckItem(index, testItem, context);

                // do we get a solution? check only if the current solution has more steps than the ideal.
                if (currentSelected.Count > Settings.IdealMinStepCount && context.RestItemsBits.IsClean())
                {
                    // Save solution to context.
                    Settings.CurrentSolution = currentSelected.ToList();

                    context.ResetTestLimit(Settings.CurrentSolution.Count);
                    currentSelected.Pop();
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
                        currentSelected.Pop();
                        context.RemoveNumHits(testItem);
                        return MatrixResult.Aborted;
                    }

                    if (res == MatrixResult.Succeeded && Settings.CurrentSolution.Count <= selectedCount + 2)
                    {
                        currentSelected.Pop();
                        context.RemoveNumHits(testItem);
                        return MatrixResult.Succeeded;// no need to continue the check.
                    }
                }

                // recover the tests and continue.
                context.RemoveNumHits(testItem);
                currentSelected.Pop();

                _restItems.CopyTo(context.RestItemsBits);
                context.NumBitsCovered = _unhitNums;
            }

            return MatrixResult.Failed;
        }

        // Check the current incomplete solution and determine if need to continue or not.
        private bool PreCheckSolution(Stack<MatrixItemByte> testSolution, BuildContext context)
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
}
