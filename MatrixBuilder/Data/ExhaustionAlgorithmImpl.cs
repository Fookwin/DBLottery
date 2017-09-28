using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixBuilder
{
    class BuildContext
    {
        private class BuildToken
        {
            public UInt64 NumBitsCovered = 0;
            public MatrixItemPositionBits RestItemsBits = null;
            public int[] NumHitCounts = null;
            public MatrixItemPositionBits NumBitsToSkip = null;

            public BuildToken Clone()
            {
                return new BuildToken()
                {
                    NumBitsCovered = this.NumBitsCovered,
                    RestItemsBits = this.RestItemsBits.Clone(),
                    NumBitsToSkip = this.NumBitsToSkip.Clone(),
                    NumHitCounts = CloneInts(this.NumHitCounts)
                };
            }

            private unsafe static int[] CloneInts(int[] source)
            {
                int count = source.Length;
                int[] target = new int[count];
                fixed (int* pSource = source, pTarget = target)
                {
                    // Set the starting points in source and target for the copying.
                    int* ps = pSource;
                    int* pt = pTarget;

                    // Copy the specified number of bytes from source to target.
                    for (int i = 0; i < count; i++)
                    {
                        *pt = *ps;
                        pt++;
                        ps++;
                    }
                }

                return target;
            }

            public void RefreshNumBitsToSkip(int candidateCount, int maxHitCountForEach, List<MatrixItemPositionBits> numDistributions)
            {
                for (int i = 0; i < candidateCount; ++i)
                {
                    if (NumHitCounts[i] >= maxHitCountForEach)
                        NumBitsToSkip.AddMultiple(numDistributions[i]);
                }
            }
        }

        public enum Status
        {
            Continue,
            PreCheckFailed,
            Complete
        }

        private readonly MatrixBuildSettings _settings = null;
        private readonly int _candidateCount = -1;
        private readonly int _seleteCount = -1;
        private readonly bool _returnForAny = false;

        // Algorithm settings.
        private int _maxSelectionCount = -1;
        private int _maxHitCountForEach = -1;
        
        private BuildToken _buildToken = null;
        private Stack<MatrixItemByte> _currentSelection = new Stack<MatrixItemByte>();
        private Stack<BuildToken> _tokenStack = new Stack<BuildToken>();

        // Progress 
        public readonly string JobName = null;
        public string progressMsg = "";
        public double progress = 0.0;
        public double progressRange = 100.0;
        public UInt64 CheckCount = 0;
        public UInt64 CheckCountForUpdateProgress = 0;
        public UInt64 CheckCountStep = 1000000;

        public BuildContext(MatrixBuildSettings settings, bool returnForAny, int maxSelectionCount, string jobName)
        {
            _candidateCount = settings.CandidateNumCount;
            _seleteCount = settings.MatchNumCount + 1;
            _settings = settings;
            _returnForAny = returnForAny;
            JobName = jobName;

            _maxSelectionCount = maxSelectionCount;
            _maxHitCountForEach = (_maxSelectionCount / _candidateCount + 1) * _seleteCount;

            _buildToken = new BuildToken()
            {
                RestItemsBits = new MatrixItemPositionBits(_settings.TestItemCollection.Count, false),
                NumBitsToSkip = new MatrixItemPositionBits(_settings.TestItemCollection.Count, true),
                NumHitCounts  = new int[_candidateCount]
            };
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

        public void Commit()
        {
            // Save solution to context.
            _settings.CurrentSolution = _currentSelection.Reverse().ToList();

            _maxSelectionCount = _settings.CurrentSolution.Count - 1;

            // calculate the max num hit count.
            int newMaxHitCountForEach = (_maxSelectionCount / _candidateCount + 1) * _seleteCount;

            // check if any numbers has been hit the max hit count that could be skipped.
            if (newMaxHitCountForEach < _maxHitCountForEach)
            {
                // let update the token stack to refresh the skip number bits for each
                foreach (var token in _tokenStack)
                {
                    token.RefreshNumBitsToSkip(_candidateCount, newMaxHitCountForEach, _settings.NumDistributions);
                }

                // update current token
                _buildToken.RefreshNumBitsToSkip(_candidateCount, newMaxHitCountForEach, _settings.NumDistributions);
            }

            _maxHitCountForEach = newMaxHitCountForEach;
        }

        public void AddNumHits(MatrixItemByte item)
        {
            UInt64 mash = 1;
            for (int i = 0; i < _candidateCount; ++i)
            {
                if ((mash & item.Bits) != 0)
                {
                    ++ _buildToken.NumHitCounts[i];

                    if (_buildToken.NumHitCounts[i] == _maxHitCountForEach)
                    {
                        _buildToken.NumBitsToSkip.AddMultiple(_settings.NumDistributions[i]);
                    }
                }

                mash = mash << 1;
            }
        }

        public int NextItem(int pre)
        {
            return _buildToken.NumBitsToSkip.NextPosition(pre, false);
        }

        // Check the current incomplete solution and determine if need to continue or not.
        private bool PreCheckSolution()
        {
            // No need to continue if the current solution already has no chance to get less steps than the existing one.
            int restStep = MaxSelectionCount - _currentSelection.Count;
            if (restStep <= 0)
                return false;

            // check the number coverage.
            int restUncoveredNumCount = BuildMatrixUtil.BitCount(_buildToken.NumBitsCovered);
            if (restUncoveredNumCount > restStep * (_settings.MatchNumCount + 1))
                return false;

            // The max item count can be covered by rest steps. 
            if (_buildToken.RestItemsBits.UnhitCount > _settings.MaxItemCountCoveredByOneItem * restStep)
                return false;

            return true; // let's continue.
        }

        public void Pop()
        {
            var lastItem = _currentSelection.Pop();

            // recovery the build token.
            _buildToken = _tokenStack.Pop();
        }

        public Status Push(int index, MatrixItemByte item)
        {
            // update progress
            if (++CheckCountForUpdateProgress > CheckCountStep)
            {
                ++CheckCount;
                CheckCountForUpdateProgress = 0;
            }

            // backup the token.
            _tokenStack.Push(_buildToken.Clone());

            // add to selection.
            _currentSelection.Push(item);

            // update states.
            AddNumHits(item);
            _buildToken.RestItemsBits.RemoveMultiple(_settings.TestItemMashCollection[index]);
            _buildToken.NumBitsCovered &= ~item.Bits;

            // check if we just get a solution.
            if (_currentSelection.Count > _settings.IdealMinStepCount && _buildToken.RestItemsBits.IsClean())
            {
                return Status.Complete;
            }

            if (!PreCheckSolution())
            {
                return Status.PreCheckFailed;
            }

            return Status.Continue;
        }

        public int SelectionCount()
        {
            return _currentSelection.Count;
        }
    }

    class ExhaustionAlgorithmImpl
    {
        private readonly MatrixBuildSettings Settings = null;
        private readonly MatrixTable Table = null;
        public event MatrixTableBuilder.MatrixCalculationProgressHandler MatrixProgressHandler = null;

        public ExhaustionAlgorithmImpl(MatrixBuildSettings settings, MatrixTable refTable,
            MatrixTableBuilder.MatrixCalculationProgressHandler processMonitor)
        {
            Settings = settings;
            Table = refTable;
            MatrixProgressHandler = processMonitor;
        }

        public MatrixResult Calculate(string jobName, int maxSelectionCount, bool returnForAny)
        {
            // Build context.
            BuildContext context = new BuildContext(Settings, returnForAny, maxSelectionCount, jobName);

            if (MatrixProgressHandler != null)
            {
                string message = "Started...";
                MatrixProgressHandler(jobName, message, 0);
            }

            // Include the first always.
            MatrixItemByte firstItem = Settings.TestItemCollection[0];
            context.Push(0, firstItem);

            // Search the rest for possible soution.
            var res = TraversalForAny(1, context);

            if (MatrixProgressHandler != null)
            {
                string message = res.ToString();
                message += " Check Count: " + context.CheckCount.ToString();
                MatrixProgressHandler(jobName, message, 100);
            }

            return res;
        }

        private MatrixResult TraversalForAny(int startIndex, BuildContext context)
        {
            int selectedCount = context.SelectionCount();

            int visitedCount = 0;
            int count = Settings.TestItemCollection.Count - context.MaxSelectionCount + selectedCount;

            double progStart = context.progress;
            double progStep = context.progressRange / (count - startIndex);
            context.progressRange = progStep > 0.001 ? progStep : 0;
            UInt64 preCheckCount = context.CheckCount;

            bool bUpdateProgressMsg = context.SelectionCount() == 1;

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

                        int result = MatrixProgressHandler(context.JobName, message, context.progress);
                        if (result < 0)
                        {
                            return MatrixResult.User_Aborted;
                        }
                    }
                }

                // check if a better result has been found by other processer.
                if (Settings.CurrentSolution != null && context.MaxSelectionCount >= Settings.CurrentSolution.Count)
                    return MatrixResult.Job_Aborted;

                ++visitedCount;

                MatrixItemByte testItem = Settings.TestItemCollection[index];

                // commit this item
                var status = context.Push(index, testItem);

                // do we get a solution? check only if the current solution has more steps than the ideal.
                if (status == BuildContext.Status.Complete)
                {
                    // reset the solution.
                    context.Commit();

                    context.Pop();

                    // Get it!
                    if (MatrixProgressHandler != null)
                    {
                        context.progress = progStart + progStep * visitedCount;

                        string message = context.progressMsg + context.progress.ToString("f3") + "%";
                        message = " CURRENT:" + Settings.CurrentSolution.Count.ToString();
                        message += " CHECKED: " + context.CheckCount.ToString();

                        MatrixProgressHandler(context.JobName, message, context.progress);
                    }

                    return MatrixResult.Job_Succeeded; // return this solution and no need to continue.
                }
                else if (status == BuildContext.Status.Continue)
                {
                    int next = context.NextItem(index);

                    // if we got a valid solution, check if need to continue or not.
                    MatrixResult res = TraversalForAny(next, context);
                    if (res == MatrixResult.User_Aborted || res == MatrixResult.Job_Aborted)
                    {
                        return res;
                    }

                    if (res == MatrixResult.Job_Succeeded && (context.ReturnForAny || Settings.CurrentSolution.Count <= selectedCount + 2))
                    {
                        context.Pop();
                        return MatrixResult.Job_Succeeded; // no need to continue the check.
                    }
                }

                // recover the tests and continue.
                context.Pop();
            }

            return MatrixResult.Job_Failed;
        }
    }
}
