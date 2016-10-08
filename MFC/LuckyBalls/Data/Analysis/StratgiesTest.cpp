#include "stdafx.h"
#include "StratgiesTest.h"

CTestResult::CTestResult(int issue)
	: m_Issue(issue), m_iMinScore(0), m_iMaxScore(0)
{
}

CTestResult::~CTestResult()
{
	CAnalysisUtil::DeleteAll(m_CalResult);
}

void CTestResult::Calculate()
{
	CAnalysisUtil::MatchTest(m_CalResult, m_LuckyNum, m_MatchStatus);
	m_iMinScore = m_MatchStatus[3] * 10 + m_MatchStatus[4] * 200 + m_MatchStatus[5] * 10000 - (int)m_CalResult.size() * 2;
	m_iMaxScore = m_MatchStatus[2] * 10 + m_MatchStatus[3] * 200 + m_MatchStatus[4] * 3000 + m_MatchStatus[5] * 5000000- (int)m_CalResult.size() * 2;
}

