#pragma once
#include "system.h"
#include <map>
#include <string>
#include <vector>
#include <basetsd.h>

using namespace std;

class MatrixItemByte;

class MTRxEXPORTS MTRxMatrixCalculator
{
public:
	MTRxMatrixCalculator();
	~MTRxMatrixCalculator();

	class State
	{
	public:
		string ThreadID;
		string Message;
		double Progress;
	};

	bool Build();
	map<string, State>* GetProgress() const;

	static bool ValidateSolution(int candidateCount, int selectCount, const vector<MatrixItemByte*>& test);

private:
	map<string, State>* m_progress;
};