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
        private int NextPosMax = -1;

        private BuildToken()
        {
        }

        public BuildToken(MatrixBuildSettings settings)
        {
            RestItemsBits = new MatrixItemPositionBits(settings.TestItemCollection.Count, false);
            NumBitsToSkip = new MatrixItemPositionBits(settings.TestItemCollection.Count, true);
            NumHitCounts = new int[settings.CandidateNumCount];
            UnhitNumCount = settings.CandidateNumCount;
            NextPosMax = settings.TestItemCollection.Count - 1; // to the last
        }

        public BuildToken Clone()
        {
            return new BuildToken()
            {
                RestItemsBits = this.RestItemsBits.Clone(),
                NumBitsToSkip = this.NumBitsToSkip.Clone(),
                NumHitCounts = CloneInts(this.NumHitCounts),
                UnhitNumCount = this.UnhitNumCount,
                NextPosMax = this.NextPosMax
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

        public void RefreshForCommit(int minHitCountForEach, int maxHitCountForEach, MatrixBuildSettings settings)
        {
            int nextPosMax = -1;
            for (int i = 0; i < settings.CandidateNumCount; ++i)
            {
                if (NumHitCounts[i] >= maxHitCountForEach)
                    NumBitsToSkip.AddMultiple(settings.NumDistributions[i].Distribution);

                if (nextPosMax < 0 && NumHitCounts[i] < minHitCountForEach)
                {
                    nextPosMax = settings.NumDistributions[i].MaxIndex;
                }
            }

            if (nextPosMax > 0)
                NextPosMax = nextPosMax;
        }

        public unsafe void UpdateNumCoverage(MatrixItemByte item, int minHitCountForEach, int maxHitCountForEach, MatrixBuildSettings settings)
        {
            int nextPosMax = -1;

            UInt64 itemBits = item.Bits;
            fixed (int* pArray = NumHitCounts)
            {
                int* ps = pArray;
                for (int i = 0; i < settings.CandidateNumCount; i++)
                {
                    if ((settings.NumDistributions[i].Bits & itemBits) != 0)
                    {
                        ++ (*ps);

                        if (*ps == 1)
                        {
                            --UnhitNumCount; // this number was just hitted.
                        }

                        if (*ps == maxHitCountForEach)
                        {
                            NumBitsToSkip.AddMultiple(settings.NumDistributions[i].Distribution);
                        }
                    }

                    if (nextPosMax < 0 && * ps < minHitCountForEach)
                    {
                        nextPosMax = settings.NumDistributions[i].MaxIndex;
                    }

                    ps++;
                }
            }

            if (nextPosMax > 0)
                NextPosMax = nextPosMax;
        }

        public void UpdateItemCoverage(int addItemIndex, MatrixBuildSettings settings)
        {
            RestItemsBits.RemoveMultiple(settings.TestItemMashCollection[addItemIndex]);
        }

        public void NextItemScope(int pre, ref int min, ref int max)
        {
            min = NumBitsToSkip.NextPosition(pre, false);
            max = NextPosMax;
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
            Failed,
            Complete
        }

        private readonly MatrixBuildSettings _settings = null;
        private readonly bool _returnForAny = false;

        // Algorithm settings.
        private int _maxSelectionCount = -1;
        private int _maxHitCountForEach = -1;
        private int _minHitCountForEach = -1;
        private int _solutionCountFound = -1;

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

        private BuildContext(MatrixBuildSettings settings, bool returnForAny)
        {
            _settings = settings;
            _returnForAny = returnForAny;
        }

        public BuildContext(MatrixBuildSettings settings, bool returnForAny, int maxSelectionCount, string jobName)
        {
            _settings = settings;
            _returnForAny = returnForAny;
            JobName = jobName;

            _maxSelectionCount = maxSelectionCount;
            _maxHitCountForEach = _maxSelectionCount * _settings.SelectNumCount / _settings.CandidateNumCount + 1;
            _minHitCountForEach = (_maxSelectionCount + 1) * _settings.SelectNumCount / _settings.CandidateNumCount - 1;

            _buildToken = new BuildToken(_settings);
        }

        public int SolutionCountFound
        {
            get
            {
                return _solutionCountFound;
            }
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

        public bool Commit()
        {
            bool res = _settings.CommitSolution(_currentSelection.Reverse().ToList());
            if (res)
                _solutionCountFound = _currentSelection.Count;

            // refresh the token anyway.
            RefreshTokens();

            return res;
        }

        public void RefreshTokens()
        {
            _maxSelectionCount = _settings.CurrentSolutionCount() - 1;

            // calculate the max num hit count.
            int newMinHitCountForEach = (_maxSelectionCount + 1) * _settings.SelectNumCount / _settings.CandidateNumCount - 1;
            int newMaxHitCountForEach = _maxSelectionCount * _settings.SelectNumCount / _settings.CandidateNumCount + 1;

            // check if any numbers has been hit the max hit count that could be skipped.
            if (newMaxHitCountForEach < _maxHitCountForEach && newMinHitCountForEach < _minHitCountForEach)
            {
                // let update the token stack to refresh the skip number bits for each
                foreach (var token in _tokenStack)
                {
                    token.RefreshForCommit(newMinHitCountForEach, newMaxHitCountForEach, _settings);
                }

                // update current token
                _buildToken.RefreshForCommit(newMinHitCountForEach, newMaxHitCountForEach, _settings);
            }

            _minHitCountForEach = newMinHitCountForEach;
            _maxHitCountForEach = newMaxHitCountForEach;
        }

        public IndexScope NextItemScope(int current)
        {
            int min = -1, max = -1;
            _buildToken.NextItemScope(current, ref min, ref max);

            return (min >= 0 && min <= max) ? new IndexScope(min, max) : null;
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

            // update the coverage of the items.
            _buildToken.UpdateItemCoverage(index, _settings);

            // check if we just get a solution.
            if (_currentSelection.Count > _settings.IdealMinItemCount && _buildToken.IsAllItemsCovered())
            {
                return Status.Complete;
            }

            // if the current solution already has no chance to get less steps than the existing one, stop for failure.
            int restStep = MaxSelectionCount - _currentSelection.Count;
            if (restStep <= 0)
                return Status.Failed;

            // If it is impossible to cover all items within rest steps, stop for failure. 
            if (_buildToken.UncoveredItemCount() > _settings.MaxItemCountCoveredByOneItem * restStep)
                return Status.Failed;

            // update the coverage of the numbers.
            _buildToken.UpdateNumCoverage(item, _minHitCountForEach, _maxHitCountForEach, _settings);

            // If it is impossible to cover all number with reset steps, stop for failure.
            if (_buildToken.UncoveredNumCount() > restStep * _settings.SelectNumCount)
                return Status.Failed;

            return Status.Continue;
        }

        public int SelectionCount()
        {
            return _currentSelection.Count;
        }
    }

    class MatrixTaskDispatcher
    {
        private const int Step = 1;
        private Stack<IndexScope> Tasks = new Stack<IndexScope>();

        public MatrixTaskDispatcher(IndexScope overallScope)
        {
            int count = (overallScope.End - overallScope.Start - 1) / Step + 1;
            for (int i = 0; i < count; ++ i)
            {
                int startFrom = i * Step + overallScope.Start, endAt = Math.Min(overallScope.End, startFrom + Step - 1);
                Tasks.Push(new IndexScope(startFrom, endAt));
            }

            Tasks = new Stack<IndexScope>(Tasks.ToList());
        }

        public IndexScope Take()
        {
            return Tasks.Count > 0 ? Tasks.Pop() : null;
        }

        public bool HasTask()
        {
            return Tasks.Count > 0;
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

        public void Calculate(int maxSelectionCount, bool returnForAny, bool bInParallel)
        {
            // define the scope for the top level loop, since the first item would be always added, 
            // start from the second one and end with the last index contains '1'.
            IndexScope overallScope = new IndexScope(1, Settings.NumDistributions[0].MaxIndex);

            // make the task dispatcher.
            MatrixTaskDispatcher dispatcher = new MatrixTaskDispatcher(overallScope);

            if (bInParallel)
            {
                int threadCount = Environment.ProcessorCount;
                bool bAborted = false;
                ParallelOptions option = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
                ParallelLoopResult loopResult = Parallel.For(0, threadCount, option, (Index) =>
                {
                    while(!bAborted && dispatcher.HasTask())
                    {
                        var scope = dispatcher.Take();
                        if (scope != null)
                        {
                            string threadName = "thread [" + Index.ToString() + "]    " + scope.ToString();
                            if (_Calculate(threadName, maxSelectionCount, scope, returnForAny) == MatrixResult.User_Aborted)
                                bAborted = true;
                        }
                    }

                });
            }
            else
            {
                _Calculate("main", maxSelectionCount, overallScope, returnForAny);
            }
        }

        private MatrixResult _Calculate(string jobName, int maxSelectionCount, IndexScope scope, bool returnForAny)
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

            var res = MatrixResult.Job_Failed;
            res = TraversalForAny(scope, context);
            if (context.SolutionCountFound > 0)
                res = MatrixResult.Job_Succeeded;

            if (MatrixProgressHandler != null)
            {
                string message = "[" + res.ToString() + "]";
                if (res == MatrixResult.Job_Succeeded)
                    message += "  SOLUTION: " + context.SolutionCountFound;
                message += "  VISITED: " + context.CheckCount.ToString();
                MatrixProgressHandler(jobName, message, 100);
            }

            return res;
        }

        private MatrixResult TraversalForAny(IndexScope scope, BuildContext context)
        {
            int selectedCount = context.SelectionCount();

            int visitedCount = 0;
            int count = Math.Min(scope.End, Settings.TestItemCollection.Count - context.MaxSelectionCount + selectedCount);

            double progStart = context.progress;
            double progStep = context.progressRange / (count - scope.Start + 1);
            context.progressRange = progStep > 0.001 ? progStep : 0;
            UInt64 preCheckCount = context.CheckCount;

            bool bUpdateProgressMsg = context.SelectionCount() == 2;

            for (int index = scope.Start; index <= count; ++index)
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
                        message += "  SOLUTION: " + Settings.CurrentSolutionCount();
                        message += "  CHECKING: " + context.MaxSelectionCount;
                        message += "  VISITED: " + context.CheckCount.ToString();

                        int result = MatrixProgressHandler(context.JobName, message, context.progress);
                        if (result < 0)
                        {
                            return MatrixResult.User_Aborted;
                        }
                    }
                }

                // check if a better result has been found by other process.
                if (Settings.CurrentSolutionCount() > 0 && context.MaxSelectionCount >= Settings.CurrentSolutionCount())
                {
                    if (context.ReturnForAny)
                        return MatrixResult.Job_Aborted;

                    context.RefreshTokens(); // refresh the tokens for the better solution.
                }

                ++visitedCount;

                MatrixItemByte testItem = Settings.TestItemCollection[index];

                // commit this item
                var status = context.Push(index, testItem);

                // do we get a solution? check only if the current solution has more steps than the ideal.
                if (status == BuildContext.Status.Complete)
                {
                    // reset the solution.
                    bool bCommitFail = context.Commit();

                    // has better solution found by other process, don't continue if return any.
                    if (context.ReturnForAny)
                        return MatrixResult.Job_Aborted;

                    context.Pop();

                    // no need to continue, since we could not get better solution at this level loop.
                    return bCommitFail ? MatrixResult.Job_Failed : (context.ReturnForAny ? MatrixResult.Job_Succeeded : MatrixResult.Job_Succeeded_Continue); 
                }
                else if (status == BuildContext.Status.Continue)
                {
                    IndexScope nextIndex = context.NextItemScope(index);
                    if (nextIndex != null)
                    {
                        // if we got a valid solution, check if need to continue or not.
                        MatrixResult res = TraversalForAny(nextIndex, context);
                        if (res == MatrixResult.User_Aborted || res == MatrixResult.Job_Aborted || res == MatrixResult.Job_Succeeded)
                        {
                            return res;
                        }

                        if (res == MatrixResult.Job_Succeeded_Continue)
                        {
                            if (Settings.CurrentSolutionCount() <= selectedCount + 2)
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
