#include "stdafx.h"
#include "GreedyAlgorithmImpl.h"
#include <iostream>
#include <fstream>

#pragma region BuildContext

GreedyAlgorithmImpl::BuildContext::BuildContext(MatrixBuildSettings* settings, bool returnForAny)
{
	_settings = settings;
	_returnForAny = returnForAny;
}

GreedyAlgorithmImpl::BuildContext::BuildContext(MatrixBuildSettings* settings, bool returnForAny, int maxSelectionCount)
{
	_settings = settings;
	_returnForAny = returnForAny;

	_maxSelectionCount = maxSelectionCount;
	_maxHitCountForEach = _maxSelectionCount * _settings->SelectNumCount / _settings->CandidateNumCount + 1;
	_minHitCountForEach = (_maxSelectionCount + 1) * _settings->SelectNumCount / _settings->CandidateNumCount - 1;

	_buildToken = new BuildToken(_settings);
}

GreedyAlgorithmImpl::BuildContext::~BuildContext()
{
	delete _buildToken;
	for each(auto token in _tokenStack)
	{
		delete token;
	}
}

void GreedyAlgorithmImpl::BuildContext::GetSolution(vector<const MatrixItemByte*>& solution) const
{
	for each(auto item in _currentSelection)
	{
		solution.push_back(item);
	}
}

MatrixBuildSettings* GreedyAlgorithmImpl::BuildContext::Settings() const
{
	return _settings;
}

int GreedyAlgorithmImpl::BuildContext::MaxSelectionCount() const
{
	return _maxSelectionCount;
}

bool GreedyAlgorithmImpl::BuildContext::ReturnForAny() const
{
	return _returnForAny;
}

void GreedyAlgorithmImpl::BuildContext::RefreshTokens(int currentSoltionCount)
{
	_maxSelectionCount = currentSoltionCount - 1;

	// calculate the max num hit count.
	int newMinHitCountForEach = (_maxSelectionCount + 1) * _settings->SelectNumCount / _settings->CandidateNumCount - 1;
	int newMaxHitCountForEach = _maxSelectionCount * _settings->SelectNumCount / _settings->CandidateNumCount + 1;

	// check if any numbers has been hit the max hit count that could be skipped.
	if (newMaxHitCountForEach < _maxHitCountForEach && newMinHitCountForEach < _minHitCountForEach)
	{
		// let update the token stack to refresh the skip number bits for each
		for each(auto token in _tokenStack)
		{
			token->RefreshForCommit(newMinHitCountForEach, newMaxHitCountForEach);
		}

		// update current token
		_buildToken->RefreshForCommit(newMinHitCountForEach, newMaxHitCountForEach);
	}

	_minHitCountForEach = newMinHitCountForEach;
	_maxHitCountForEach = newMaxHitCountForEach;
}

bool GreedyAlgorithmImpl::BuildContext::NextItemScope(int current, IndexScope& nextScope)
{
	return _buildToken->NextItemScope(current, nextScope);
}

void GreedyAlgorithmImpl::BuildContext::Pop()
{
	_currentSelection.pop_back();

	// recovery the build token.
	delete _buildToken;
	_buildToken = _tokenStack.back();
	_tokenStack.pop_back();
}

GreedyAlgorithmImpl::BuildContext::Status GreedyAlgorithmImpl::BuildContext::Push(int index, const MatrixItemByte* item)
{
	// backup the token.
	_tokenStack.push_back(_buildToken->Clone());

	// add to selection.
	_currentSelection.emplace_back(item);

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

int GreedyAlgorithmImpl::BuildContext::SelectionCount()
{
	return static_cast<int>(_currentSelection.size());
}

#pragma endregion


#pragma region GreedyAlgorithmImpl

GreedyAlgorithmImpl::~GreedyAlgorithmImpl()
{
}

GreedyAlgorithmImpl::GreedyAlgorithmImpl(MatrixBuildSettings* settings)
{
	_settings = settings;
}

bool GreedyAlgorithmImpl::_CommitSolution(const vector<const MatrixItemByte*>& solution)
{
	// don't commit for worse solution.
	if (_solutionItemCount > 0 && static_cast<int>(solution.size()) >= _solutionItemCount)
		return false;

	//lock(lockObject)
	{
		_solution = solution;
		_solutionItemCount = static_cast<int>(solution.size());

		// save the solution to temp file
		string fileName = std::to_string(_settings->CandidateNumCount) + "-" + std::to_string(_settings->SelectNumCount) + ".txt";

		std::ofstream ofile("c:\\temp\\matrix\\" + fileName);

		for each(auto item in _solution)
		{
			ofile << item->ToString() << endl;
		}

		ofile.close();
	}

	return true;
}

const vector<const MatrixItemByte*>& GreedyAlgorithmImpl::GetSolution() const
{
	return _solution;
}

int GreedyAlgorithmImpl::_CurrentSolutionCount()
{
	return _solutionItemCount;
}

void GreedyAlgorithmImpl::Calculate(int maxSelectionCount, ThreadProgressSet& progresses, bool returnForAny, bool bInParallel)
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
		progress.Total = _settings->TestItemCount();
		progresses.push_back(progress);
		_Calculate(maxSelectionCount, overallScope, returnForAny, progresses[0]);
	//}
}

MatrixResult GreedyAlgorithmImpl::_Calculate(int maxSelectionCount, const IndexScope& scope, bool returnForAny, ThreadProgress& progress)
{
	// Build context.
	BuildContext context(_settings, returnForAny, maxSelectionCount);

	// Include the first always.
	const MatrixItemByte& firstItem = _settings->TestItem(0);
	context.Push(0, &firstItem);

	IndexScope nextScope;
	if (!context.NextItemScope(0, nextScope))
		return MatrixResult::Job_Failed;

	progress.Progress.push_back(0);
	return _TraversalForAny(nextScope, context, progress);
}

MatrixResult GreedyAlgorithmImpl::_TraversalForAny(const IndexScope& scope, BuildContext& context, ThreadProgress& progressMornitor)
{
	int selectedCount = context.SelectionCount();

	int index = scope.Next();
	while (index > 0)
	{
		// check if a better result has been found by other process.
		if (_CurrentSolutionCount() > 0 && context.MaxSelectionCount() >= _CurrentSolutionCount())
		{
			if (context.ReturnForAny())
				return MatrixResult::Job_Aborted;

			context.RefreshTokens(_CurrentSolutionCount()); // refresh the tokens for the better solution.
		}

		const MatrixItemByte& testItem = context.Settings()->TestItem(index);

		// commit this item
		auto status = context.Push(index, &testItem);
		progressMornitor.Progress.push_back(index);
		if (progressMornitor.Aborted)
		{
			return MatrixResult::User_Aborted;
		}

		// do we get a solution? check only if the current solution has more steps than the ideal.
		if (status == BuildContext::Status::Complete)
		{
			// reset the solution.
			vector<const MatrixItemByte*> currentSolution;
			context.GetSolution(currentSolution);

			bool bCommitFail = _CommitSolution(currentSolution);

			// has better solution found by other process, don't continue if return any.
			if (context.ReturnForAny())
				return MatrixResult::Job_Aborted;

			context.Pop();
			progressMornitor.Progress.pop_back();

			// no need to continue, since we could not get better solution at this level loop.
			return bCommitFail ? MatrixResult::Job_Failed : (context.ReturnForAny() ? MatrixResult::Job_Succeeded : MatrixResult::Job_Succeeded_Continue);
		}
		else if (status == BuildContext::Status::Continue)
		{
			IndexScope nextScope;
			if (context.NextItemScope(index, nextScope))
			{
				// if we got a valid solution, check if need to continue or not.
				MatrixResult res = _TraversalForAny(nextScope, context, progressMornitor);
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
						progressMornitor.Progress.pop_back();
						return MatrixResult::Job_Succeeded_Continue;
					}
				}
			}
			// could not find any valid item for next, skipping.
		}

		// recover the tests and continue.
		context.Pop();
		progressMornitor.Progress.pop_back();

		index = scope.Next();
	}

	return MatrixResult::Job_Failed;
}

#pragma endregion