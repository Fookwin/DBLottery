#pragma once
#include "IAlgorithmImpl.h"
#include "MatrixBuildSettings.h"
#include "MatrixItemByte.h"
#include "BuildMatrixUtil.h"
#include "Progress.h"
#include <stack>

class BuildContext
{
public:
	enum Status
	{
		Continue,
		Failed,
		Complete
	};

	BuildContext(MatrixBuildSettings* settings, bool returnForAny, int maxSelectionCount);
	~BuildContext();

	void GetSolution(vector<const MatrixItemByte*>& solution) const;

	MatrixBuildSettings* Settings() const;

	int MaxSelectionCount() const;

	bool ReturnForAny() const;

	void RefreshTokens(int currentSoltionCount);

	bool NextItemScope(int current, IndexScope& nextScope);

	void Pop();

	Status Push(int index, const MatrixItemByte* item);

	int SelectionCount();

private:
	BuildContext(MatrixBuildSettings* settings, bool returnForAny);

	MatrixBuildSettings* _settings = nullptr;
	bool _returnForAny = false;

	// Algorithm settings.
	int _maxSelectionCount = -1;
	int _maxHitCountForEach = -1;
	int _minHitCountForEach = -1;

	BuildToken* _buildToken = nullptr;
	vector<const MatrixItemByte*> _currentSelection;
	vector<BuildToken*> _tokenStack;
};

class ExhaustionAlgorithmImpl : public IAlgorithmImpl
{
public:
	ExhaustionAlgorithmImpl(MatrixBuildSettings* settings);
	~ExhaustionAlgorithmImpl();

	void Calculate(int maxSelectionCount, ThreadProgressSet& progresses, bool returnForAny, bool bInParallel) override;
	const vector<const MatrixItemByte*>& GetSolution() const override;

private:

	bool _CommitSolution(const vector<const MatrixItemByte*>& solution);

	int _CurrentSolutionCount();

	MatrixResult _Calculate(int maxSelectionCount, const IndexScope& scope, bool returnForAny, ThreadProgress& progress);

	MatrixResult _TraversalForAny(const IndexScope& scope, BuildContext& context, ThreadProgress& progress);

	MatrixBuildSettings* _settings = nullptr;
	vector<const MatrixItemByte*> _solution;
	int _solutionItemCount = 0;
};

