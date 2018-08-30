#pragma once
#include "MatrixBuildSettings.h"
#include "MatrixItemByte.h"
#include "BuildMatrixUtil.h"
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

	const stack<const MatrixItemByte*>& GetSolution() const;

	MatrixBuildSettings* Settings() const;

	int SolutionCountFound() const;

	int MaxSelectionCount() const;

	bool ReturnForAny() const;

	void RefreshTokens(int currentSoltionCount);

	IndexScope* NextItemScope(int current);

	void Pop();

	Status Push(int index, const MatrixItemByte* item);

	int SelectionCount();

	// Progress 
	string progressMsg = "";
	double progressRange = 100.0;
	UINT64 CheckCount = 0;
	UINT64 CheckCountForUpdateProgress = 0;
	UINT64 CheckCountStep = 1000000;

private:
	BuildContext(MatrixBuildSettings* settings, bool returnForAny);

	MatrixBuildSettings* _settings = nullptr;
	bool _returnForAny = false;

	// Algorithm settings.
	int _maxSelectionCount = -1;
	int _maxHitCountForEach = -1;
	int _minHitCountForEach = -1;
	int _solutionCountFound = -1;

	BuildToken* _buildToken = nullptr;
	stack<const MatrixItemByte*> _currentSelection;
	stack<BuildToken*> _tokenStack;
};

class ExhaustionAlgorithmImpl
{
public:
	ExhaustionAlgorithmImpl(MatrixBuildSettings* settings);
	~ExhaustionAlgorithmImpl();

	const vector<MatrixItemByte*>& GetSolution() const;

	void Calculate(int maxSelectionCount, ThreadProgressSet& progresses, bool returnForAny, bool bInParallel);

private:

	bool _CommitSolution(const vector<MatrixItemByte*>& solution);

	int _CurrentSolutionCount();

	MatrixResult _Calculate(int maxSelectionCount, const IndexScope& scope, bool returnForAny, ThreadProgress& progress);

	MatrixResult _TraversalForAny(const IndexScope& scope, BuildContext& context, ThreadProgress& progress);

	MatrixBuildSettings* _settings = nullptr;
	vector<MatrixItemByte*> _solution;
	size_t _solutionItemCount = 0;
};

