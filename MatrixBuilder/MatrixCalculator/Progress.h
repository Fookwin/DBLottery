#pragma once
#include "system.h"
#include <stack>
#include <vector>

using namespace std;
class MTRxEXPORTS ThreadProgress
{
public:
	int Total;
	vector<int> Progress;
	bool Aborted{ false };
};

typedef MTRxEXPORTS vector<ThreadProgress> ThreadProgressSet;