#pragma once
#include "system.h"
#include "Progress.h"
#include <string>
#include <map>
#include <vector>
#include <basetsd.h>

using namespace std;

class MatrixItemByte;

enum AlgorithmTypeEnum
{
	Exhaustion = 0,
	Greedy = 1
};

class MTRxEXPORTS MTRxMatrixCalculator
{
public:
	MTRxMatrixCalculator();
	~MTRxMatrixCalculator();

	bool Calcuate(int row, int col, AlgorithmTypeEnum algorithm, int betterThan, bool bParallel, bool bReturnForAny, vector<string>& solution);

	const ThreadProgressSet& GetProgress() const;
	void Abort();

	static bool ValidateSolution(int candidateCount, int selectCount, const vector<MatrixItemByte*>& test);

private:
	ThreadProgressSet m_progress;
};