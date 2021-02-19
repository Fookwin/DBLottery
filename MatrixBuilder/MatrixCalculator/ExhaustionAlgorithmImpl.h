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

	BuildContext(MatrixBuildSettings* settings, int expectedItemCount);
	~BuildContext();

	void GetSolution(vector<const MatrixItemByte*>& solution) const;

	MatrixBuildSettings* Settings() const;

	int ExpectedItemCount() const;

	void RefreshTokens(int currentSoltionCount);

	bool NextItemScope(int current, IndexScope& nextScope);

	void Pop();

	Status Push(int index, const MatrixItemByte* item);

	int SelectionCount();

private:
	BuildContext(MatrixBuildSettings* settings);

	MatrixBuildSettings* _settings = nullptr;

	// Algorithm settings.
	int _expectedItemCount = -1;
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

	void Calculate(int expectedItemCount, ThreadProgressSet& progresses) override;
	const vector<const MatrixItemByte*>& GetSolution() const override;

private:

	bool _CommitSolution(const vector<const MatrixItemByte*>& solution);

	int _CurrentSolutionCount();

	MatrixResult _Calculate(int expectedItemCount, const IndexScope& scope, ThreadProgress& progress);

	MatrixResult _TraversalForAny(const IndexScope& scope, BuildContext& context, ThreadProgress& progress);

	MatrixBuildSettings* _settings = nullptr;
	vector<const MatrixItemByte*> _solution;
	int _solutionItemCount = 0;
};

