using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixBuilder
{
    class BuildToken
    {
        private MatrixItemPositionBits RestItemsBits = null;
        private int[] NumHitCounts = null;
        private MatrixItemPositionBits NumBitsToSkip = null;
        private int UnhitNumCount = -1;

        private BuildToken()
        {
        }

        public BuildToken(MatrixBuildSettings settings, int candidateCount)
        {
            RestItemsBits = new MatrixItemPositionBits(settings.TestItemCollection.Count, false);
            NumBitsToSkip = new MatrixItemPositionBits(settings.TestItemCollection.Count, true);
            NumHitCounts = new int[candidateCount];
            UnhitNumCount = candidateCount;
        }

        public BuildToken Clone()
        {
            return new BuildToken()
            {
                RestItemsBits = this.RestItemsBits.Clone(),
                NumBitsToSkip = this.NumBitsToSkip.Clone(),
                NumHitCounts = CloneInts(this.NumHitCounts),
                UnhitNumCount = this.UnhitNumCount
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

        public void RefreshForCommit(int candidateCount, int maxHitCountForEach, List<NumberDistribution> numDistributions)
        {
            for (int i = 0; i < candidateCount; ++i)
            {
                if (NumHitCounts[i] >= maxHitCountForEach)
                    NumBitsToSkip.AddMultiple(numDistributions[i].Distribution);
            }
        }

        public unsafe void RefreshForAdd(int index, MatrixItemByte item, int selectCount, int candidateCount, int maxHitCountForEach, MatrixBuildSettings settings)
        {
            UInt64 mash = 1, itemBits = item.Bits;
            fixed (int* pArray = NumHitCounts)
            {
                int* ps = pArray;
                for (int i = 0, visited = 0; i < candidateCount; i++)
                {
                    if ((mash & itemBits) != 0)
                    {
                        ++ (*ps);
                        ++ visited;

                        if (*ps == 1)
                        {
                            --UnhitNumCount; // this number was just hitted.
                        }

                        if (*ps == maxHitCountForEach)
                        {
                            NumBitsToSkip.AddMultiple(settings.NumDistributions[i].Distribution);
                        }
                    }

                    mash = mash << 1;

                    ps++;
                }
            }

            RestItemsBits.RemoveMultiple(settings.TestItemMashCollection[index]);
        }

        public int NextItem(int pre)
        {
            return NumBitsToSkip.NextPosition(pre, false);
        }

        public int UncoveredNumCount()
        {
            return UnhitNumCount;
        }

        public bool IsAllItemsCovered()
        {
            return RestItemsBits.IsClean();
        }

        public int UncoveredItemCount()
        {
            return RestItemsBits.UnhitCount;
        }
    }

    class BuildContext
    {
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
            _seleteCount = settings.SelectNumCount;
            _settings = settings;
            _returnForAny = returnForAny;
            JobName = jobName;

            _maxSelectionCount = maxSelectionCount;
            _maxHitCountForEach = _maxSelectionCount * _seleteCount / _candidateCount + 1;

            _buildToken = new BuildToken(_settings, _candidateCount);
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
            int newMaxHitCountForEach = _maxSelectionCount * _seleteCount / _candidateCount + 1;

            // check if any numbers has been hit the max hit count that could be skipped.
            if (newMaxHitCountForEach < _maxHitCountForEach)
            {
                // let update the token stack to refresh the skip number bits for each
                foreach (var token in _tokenStack)
                {
                    token.RefreshForCommit(_candidateCount, newMaxHitCountForEach, _settings.NumDistributions);
                }

                // update current token
                _buildToken.RefreshForCommit(_candidateCount, newMaxHitCountForEach, _settings.NumDistributions);
            }

            _maxHitCountForEach = newMaxHitCountForEach;
        }

        public int NextItem(int pre)
        {
            return _buildToken.NextItem(pre);
        }

        // Check the current incomplete solution and determine if need to continue or not.
        private bool PreCheckSolution()
        {
            // No need to continue if the current solution already has no chance to get less steps than the existing one.
            int restStep = MaxSelectionCount - _currentSelection.Count;
            if (restStep <= 0)
                return false;

            // check the number coverage.
            if (_buildToken.UncoveredNumCount() > restStep * _settings.SelectNumCount)
                return false;

            // The max item count can be covered by rest steps. 
            if (_buildToken.UncoveredItemCount() > _settings.MaxItemCountCoveredByOneItem * restStep)
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
            _buildToken.RefreshForAdd(index, item, _seleteCount,  _candidateCount, _maxHitCountForEach, _settings);

            // check if we just get a solution.
            if (_currentSelection.Count > _settings.IdealMinItemCount && _buildToken.IsAllItemsCovered())
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

                    // no need to continue, since we could not get better solution at this level loop.
                    return context.ReturnForAny ? MatrixResult.Job_Succeeded : MatrixResult.Job_Succeeded_Continue; 
                }
                else if (status == BuildContext.Status.Continue)
                {
                    int next = context.NextItem(index);
                    if (next > 0)
                    {
                        Debug.Assert(next > index);

                        // if we got a valid solution, check if need to continue or not.
                        MatrixResult res = TraversalForAny(next, context);
                        if (res == MatrixResult.User_Aborted || res == MatrixResult.Job_Aborted || res == MatrixResult.Job_Succeeded)
                        {
                            return res;
                        }

                        if (res == MatrixResult.Job_Succeeded_Continue)
                        {
                            if (Settings.CurrentSolution.Count <= selectedCount + 2)
                            {
                                // impossible to get better solution at this leve of loop, break and continue.
                                context.Pop();
                                return MatrixResult.Job_Succeeded_Continue;
                            }
                        }
                    }
                    // could not find any valid item for next, skipping.
                }

                // recover the tests and continue.
                context.Pop();
            }

            return MatrixResult.Job_Failed;
        }
    }
}
