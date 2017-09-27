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
        private readonly int _candidateCount = -1;
        private readonly int _seleteCount = -1;
        private readonly bool _returnForAny = false;


        // Algorithm settings.
        private int _maxSelectionCount = -1;

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

        public BuildContext(MatrixBuildSettings settings, bool returnForAny, int maxSelectionCount)
        {
            _candidateCount = settings.CandidateNumCount;
            _seleteCount = settings.MatchNumCount + 1;
            _settings = settings;
            _maxSelectionCount = maxSelectionCount;
            _returnForAny = returnForAny;
            _maxHitCountForEach = (_maxSelectionCount / _candidateCount + 1) * _seleteCount;

            _numHitCounts = new int[_candidateCount];
            _numBitsToSkip = new MatrixItemPositionBits(_settings.TestItemCollection.Count, true);
        }

        public int MaxSelectionCount
        {
            get
            {
                return _maxSelectionCount;
            }
        }

        public bool ReturnForAny
        {
            get
            {
                return _returnForAny;
            }
        }

        public void SetSolution(List<MatrixItemByte> solution)
        {
            // Save solution to context.
            _settings.CurrentSolution = solution;

            // reset the max selectio count for further calculation.
            int newMaxSelectionCount = solution.Count - 1;

            // calculate the max num hit count.
            _maxHitCountForEach = (newMaxSelectionCount / _candidateCount + 1) * _seleteCount;

            // check if any numbers has been hit the max hit count that could be skipped.
            if (newMaxSelectionCount < _maxSelectionCount)
            {
                for (int i = 0; i < _candidateCount; ++i)
                {
                    if (_numHitCounts[i] >= _maxHitCountForEach)
                        _numBitsToSkip.AddMultiple(_settings.NumDistributions[i]);
                }
            }

            _maxSelectionCount = newMaxSelectionCount;
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
            bool returnForAny = true;
            int maxSelectionCount = TestLimit;

            // if not specify the the max selection count, set it as the count of default solution.
            if (maxSelectionCount <= 0)
            {
                // Get the default matrix as the candidate solution.
                List<MatrixItemByte> defaultSoution = BuildMatrixUtil.GetDefaultSolution(Settings.CandidateNumCount, Settings.MatchNumCount + 1, Table);
                if (defaultSoution != null)
                {
                    Settings.CurrentSolution = defaultSoution;
                    maxSelectionCount = Settings.CurrentSolution.Count - 1; // try to find the better solution than default.
                }

                returnForAny = false; // expect to find the best
            }

            // Build context.
            BuildContext context = new BuildContext(Settings, returnForAny, maxSelectionCount);

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
            int count = Settings.TestItemCollection.Count - context.MaxSelectionCount + selectedCount;

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
                    // reset the solution.
                    context.SetSolution(currentSelected.ToList());

                    currentSelected.Pop();
                    context.RemoveNumHits(testItem);

                    if (context.ReturnForAny)
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
                currentSelected.Pop();
                context.RemoveNumHits(testItem);

                _restItems.CopyTo(context.RestItemsBits);
                context.NumBitsCovered = _unhitNums;
            }

            return MatrixResult.Failed;
        }

        // Check the current incomplete solution and determine if need to continue or not.
        private bool PreCheckSolution(Stack<MatrixItemByte> testSolution, BuildContext context)
        {
            // No need to continue if the current solution already has no chance to get less steps than the existing one.
            int restStep = context.MaxSelectionCount - testSolution.Count;
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
