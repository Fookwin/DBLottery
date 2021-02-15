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
};

typedef MTRxEXPORTS vector<ThreadProgress> ThreadProgressSet;