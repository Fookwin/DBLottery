#pragma once
#include <vector>
#include "..\..\utilities\AnalysisUtil.h"

class CTestResult
{
public:
	CTestResult(int issue);
	~CTestResult();

	void Calculate();

	int m_Issue;
	CLuckyNum m_LuckyNum;
	CAnalysisUtil::LuckyNumSet m_CalResult;
	int m_MatchStatus[6];

	int m_iMinScore;
	int m_iMaxScore;
};

typedef std::vector<CTestResult*> TestResults;

	 