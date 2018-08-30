#include "stdafx.h"
#include "ExhaustionAlgorithmImpl.h"

#pragma region BuildContext

BuildContext::BuildContext(MatrixBuildSettings* settings, bool returnForAny)
{
	_settings = settings;
	_returnForAny = returnForAny;
}

BuildContext::BuildContext(MatrixBuildSettings* settings, bool returnForAny, int maxSelectionCount)
{
	_settings = settings;
	_returnForAny = returnForAny;

	_maxSelectionCount = maxSelectionCount;
	_maxHitCountForEach = _maxSelectionCount * _settings->SelectNumCount / _settings->CandidateNumCount + 1;
	_minHitCountForEach = (_maxSelectionCount + 1) * _settings->SelectNumCount / _settings->CandidateNumCount - 1;

	_buildToken = new BuildToken(_settings);
}

BuildContext::~BuildContext()
{
	delete _buildToken;
	for each(auto token in _tokenStack._Get_container())
	{
		delete token;
	}
}

const stack<const MatrixItemByte*>& BuildContext::GetSolution() const
{
	return _currentSelection;
}

MatrixBuildSettings* BuildContext::Settings() const
{
	return _settings;
}

int BuildContext::SolutionCountFound() const
{
	return _solutionCountFound;
}

int BuildContext::MaxSelectionCount() const
{
	return _maxSelectionCount;
}

bool BuildContext::ReturnForAny() const
{
	return _returnForAny;
}

void BuildContext::RefreshTokens(int currentSoltionCount)
{
	_maxSelectionCount = currentSoltionCount - 1;

	// calculate the max num hit count.
	int newMinHitCountForEach = (_maxSelectionCount + 1) * _settings->SelectNumCount / _settings->CandidateNumCount - 1;
	int newMaxHitCountForEach = _maxSelectionCount * _settings->SelectNumCount / _settings->CandidateNumCount + 1;

	// check if any numbers has been hit the max hit count that could be skipped.
	if (newMaxHitCountForEach < _maxHitCountForEach && newMinHitCountForEach < _minHitCountForEach)
	{
		// let update the token stack to refresh the skip number bits for each
		for each(auto token in _tokenStack._Get_container())
		{
			token->RefreshForCommit(newMinHitCountForEach, newMaxHitCountForEach);
		}

		// update current token
		_buildToken->RefreshForCommit(newMinHitCountForEach, newMaxHitCountForEach);
	}

	_minHitCountForEach = newMinHitCountForEach;
	_maxHitCountForEach = newMaxHitCountForEach;
}

IndexScope* BuildContext::NextItemScope(int current)
{
	return _buildToken->NextItemScope(current);
}

void BuildContext::Pop()
{
	_currentSelection.pop();

	// recovery the build token.
	delete _buildToken;
	_buildToken = _tokenStack.top();
	_tokenStack.pop();
}

BuildContext::Status BuildContext::Push(int index, const MatrixItemByte* item)
{
	// update progress
	if (++CheckCountForUpdateProgress > CheckCountStep)
	{
		++CheckCount;
		CheckCountForUpdateProgress = 0;
	}

	// backup the token.
	_tokenStack.push(_buildToken->Clone());

	// add to selection.
	_currentSelection.push(item);

	// update the coverage of the items.
	_buildToken->UpdateItemCoverage(index);

	// check if we just get a solution.
	if (static_cast<int>(_currentSelection.size()) > _settings->IdealMinItemCount && _buildToken->IsAllItemsCovered())
	{
		return BuildContext::Status::Complete;
	}

	// if the current solution already has no chance to get less steps than the existing one, stop for failure.
	int restStep = MaxSelectionCount() - static_cast<int>(_currentSelection.size());
	if (restStep <= 0)
		return Status::Failed;

	// If it is impossible to cover all items within rest steps, stop for failure. 
	if (_buildToken->UncoveredItemCount() > _settings->MaxItemCountCoveredByOneItem * restStep)
		return Status::Failed;

	// update the coverage of the numbers.
	_buildToken->UpdateNumCoverage(*item, _minHitCountForEach, _maxHitCountForEach);

	// If it is impossible to cover all number with reset steps, stop for failure.
	if (_buildToken->UncoveredNumCount() > restStep * _settings->SelectNumCount)
		return Status::Failed;

	return Status::Continue;
}

int BuildContext::SelectionCount()
{
	return static_cast<int>(_currentSelection.size());
}

#pragma endregion


#pragma region ExhaustionAlgorithmImpl

ExhaustionAlgorithmImpl::~ExhaustionAlgorithmImpl()
{
	for each (auto var in collection_to_loop)
	{

	}
}

ExhaustionAlgorithmImpl::ExhaustionAlgorithmImpl(MatrixBuildSettings* settings)
{
	_settings = settings;
}

bool ExhaustionAlgorithmImpl::_CommitSolution(const vector<MatrixItemByte*>& solution)
{
	// don't commit for worse solution.
	if (_solutionItemCount > 0 && solution.size() >= _solutionItemCount)
		return false;

	//lock(lockObject)
	{
		_solution = solution;
		_solutionItemCount = solution.size();
	}

	return true;
}

const vector<MatrixItemByte*>& ExhaustionAlgorithmImpl::GetSolution() const
{
	return _solution;
}

int ExhaustionAlgorithmImpl::_CurrentSolutionCount()
{
	return _solutionItemCount;
}

void ExhaustionAlgorithmImpl::Calculate(int maxSelectionCount, ThreadProgressSet& progresses, bool returnForAny, bool bInParallel)
{
	// define the scope for the top level loop, since the first item would be always added, 
	// start from the second one and end with the last index contains '1'.
	IndexScope overallScope(1, _settings->NumDistribution(0).MaxIndex);

	//// make the task dispatcher.
	//MatrixTaskDispatcher dispatcher = new MatrixTaskDispatcher(overallScope);

	//CancellationTokenSource cts = new CancellationTokenSource();
	//if (bInParallel)
	//{
	//	int threadCount = 10;//Environment.ProcessorCount;
	//	bool bAborted = false;
	//	ParallelOptions option = new ParallelOptions(){ MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = cts.Token };
	//	ParallelLoopResult loopResult = Parallel.For(0, threadCount, option, (Index) = >
	//	{
	//		while (!bAborted && dispatcher.HasTask())
	//		{
	//			var scope = dispatcher.Take();
	//			if (scope != null)
	//			{
	//				//MatrixBuildSettings settings = new MatrixBuildSettings(_settings->CandidateNumCount, _settings->SelectNumCount);
	//				string threadName = "thread [" + Index.ToString() + "]    " + scope.ToString();
	//				if (_Calculate(_settings, threadName, maxSelectionCount, scope, returnForAny, progresses, option.CancellationToken) == MatrixResult.User_Aborted)
	//					bAborted = true;
	//			}
	//		}
	//	});
	//}
	//else
	//{
		ThreadProgress progress;
		progresses.insert(std::make_pair("main", progress));
		_Calculate(maxSelectionCount, overallScope, returnForAny, progress);
	//}
}

MatrixResult ExhaustionAlgorithmImpl::_Calculate(int maxSelectionCount, const IndexScope& scope, bool returnForAny, ThreadProgress& progress)
{
	// Build context.
	BuildContext context(_settings, returnForAny, maxSelectionCount);

	// Include the first always.
	const MatrixItemByte& firstItem = _settings->TestItem(0);
	context.Push(0, &firstItem);

	IndexScope nextScope(scope.Min(), scope.Max(), &(context.NextItemScope(0)->ValueCollection()));
	if (nextScope.Count() <= 0)
		return MatrixResult::Job_Failed;

	string message = "Started...";
	progress.Message = message;
	progress.Progress = 0;

	MatrixResult res = MatrixResult::Job_Failed;
	res = _TraversalForAny(nextScope, context, progress);
	if (context.SolutionCountFound > 0)
		res = MatrixResult::Job_Succeeded;

	message = "[" + std::to_string(res) + "]";
	if (res == MatrixResult::Job_Succeeded)
		message += "  SOLUTION: " + context.SolutionCountFound;
	message += "  VISITED: " + std::to_string(context.CheckCount);

	progress.Message = message;
	progress.Progress = 100;

	return res;
}

MatrixResult ExhaustionAlgorithmImpl::_TraversalForAny(const IndexScope& scope, BuildContext& context, ThreadProgress& progressMornitor)
{
	int selectedCount = context.SelectionCount();

	int visitedCount = 0;
	int count = std: Math.Min(scope.Max(), context.Settings.TestItemCount() - context.MaxSelectionCount + selectedCount);

	double progStart = progressMornitor.Progress;
	double progStep = context.progressRange / (count - scope.Min() + 1);
	context.progressRange = progStep > 0.001 ? progStep : 0;
	UINT64 preCheckCount = context.CheckCount;

	int index = scope.Next();
	while (index > 0)
	{
		if (preCheckCount != context.CheckCount)
		{
			preCheckCount = context.CheckCount;

			string message = context.progressMsg + progressMornitor.Progress.ToString("f3") + "%";
			message += "  SOLUTION: " + _CurrentSolutionCount();
			message += "  CHECKING: " + context.MaxSelectionCount;
			message += "  VISITED: " + context.CheckCount.ToString();

			progressMornitor.Message = message;
			progressMornitor.Progress = progStart + progStep * visitedCount;
		}

		// check if a better result has been found by other process.
		if (_CurrentSolutionCount() > 0 && context.MaxSelectionCount >= _CurrentSolutionCount())
		{
			if (context.ReturnForAny)
				return MatrixResult::Job_Aborted;

			context.RefreshTokens(_CurrentSolutionCount()); // refresh the tokens for the better solution.
		}

		++visitedCount;

		const MatrixItemByte& testItem = context.Settings.TestItem(index);

		// commit this item
		auto status = context.Push(index, &testItem);

		// do we get a solution? check only if the current solution has more steps than the ideal.
		if (status == BuildContext::Status::Complete)
		{
			// reset the solution.
			bool bCommitFail = _CommitSolution(context.GetSolution().ToList());

			// has better solution found by other process, don't continue if return any.
			if (context.ReturnForAny)
				return MatrixResult::Job_Aborted;

			context.Pop();

			// no need to continue, since we could not get better solution at this level loop.
			return bCommitFail ? MatrixResult::Job_Failed : (context.ReturnForAny ? MatrixResult::Job_Succeeded : MatrixResult::Job_Succeeded_Continue);
		}
		else if (status == BuildContext::Status::Continue)
		{
			IndexScope* nextIndex = context.NextItemScope(index);
			if (nextIndex)
			{
				// if we got a valid solution, check if need to continue or not.
				MatrixResult res = _TraversalForAny(nextIndex, context, progressMornitor);
				if (res == MatrixResult::User_Aborted || res == MatrixResult::Job_Aborted || res == MatrixResult::Job_Succeeded)
				{
					return res;
				}

				if (res == MatrixResult::Job_Succeeded_Continue)
				{
					if (_CurrentSolutionCount() <= selectedCount + 2)
					{
						// impossible to get better solution at this leve of loop, break and continue.
						context.Pop();
						return MatrixResult::Job_Succeeded_Continue;
					}
				}
			}
			// could not find any valid item for next, skipping.
		}

		// recover the tests and continue.
		context.Pop();

		index = scope.Next();
	}

	return MatrixResult::Job_Failed;
}

#pragma endregion