#pragma once
#include "system.h"
#include <map>
#include <string>

using namespace std;

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

private:
	map<string, State>* m_progress;
};