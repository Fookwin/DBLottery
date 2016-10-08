#include "stdafx.h"
#include "AnalysisUtil.h"
#include "..\Server\Global.h"

int CAnalysisUtil::GetSimilarity(const CLuckyNum& test1, const CLuckyNum& test2)
{
	int iHit = 0;
	for (int i = 0; i < 6; i ++)
	{
		for (int j = 0; j < 6; j ++)
		{
			if (test1.m_red[i] == test2.m_red[j])
			{
				iHit ++;
				break;
			}
		}
	}

	return iHit;
}

int CAnalysisUtil::GetAdjacentSimilarity(const CLuckyNum& test1, const CLuckyNum& test2)
{
	int iHit = 0;
	for (int i = 0; i < 6; i ++)
	{
		for (int j = 0; j < 6; j ++)
		{
			if (test1.m_red[i] == test2.m_red[j] - 1 && test1.m_red[i] != (j > 0 ? test2.m_red[j - 1] : -1))
			{
				iHit ++;
				break;
			}

			if (test1.m_red[i] == test2.m_red[j] + 1 && test1.m_red[i] != (j < 5 ? test2.m_red[j + 1] : -1))
			{
				iHit ++;
				break;
			}
		}
	}

	return iHit;
}

bool CAnalysisUtil::Suggest(const Constraints& condition, LuckyNumSet& result)
{
	CLuckyNum* pNum = NULL;

	// Get all numbers.
	int iIndex = 0;
	for (int inx1 = 1; inx1 <= 28; inx1 ++)
		for (int inx2 = inx1 + 1; inx2 <= 29; inx2 ++)
			for (int inx3 = inx2 + 1; inx3 <= 30; inx3 ++)
				for (int inx4 = inx3 + 1; inx4 <= 31; inx4 ++)
					for (int inx5 = inx4 + 1; inx5 <= 32; inx5 ++)
						for (int inx6 = inx5 + 1; inx6 <= 33; inx6 ++)
						{
							// checking...
							CLuckyNum* pNUM = new CLuckyNum(inx1, inx2, inx3, inx4, inx5, inx6, 0);
							result.push_back(pNUM);
						}

	return SuggestInResult(condition, result);
}

bool CAnalysisUtil::SuggestInResult(const Constraints& condition, LuckyNumSet& result)
{
	// Start progress...
	Lucky::InitProgress();
	Lucky::ShowProgress(true);
	Lucky::SetProgress(_T("Suggesting lucky numbers ..."));

	CLuckyNum* pNum = NULL;

	// Make a copy.
	LuckyNumSet copy;
	CopyTo(result, copy);
	result.clear();

	int iIndex = 0, iNewIndex = 0, iTotal = (int)copy.size();
	for (LuckyNumSetIt it = copy.begin(); it != copy.end(); it ++, iIndex ++)
	{
		// update task...
		if (iIndex % 100 == 0)
		{
			CString strTask;
			strTask.Format(_T("%d numbers detected in total. Analyzing %d, %d, %d, %d, %d, %d ..."),
				iNewIndex, (*it)->m_red[0], (*it)->m_red[1], (*it)->m_red[2],
				(*it)->m_red[3], (*it)->m_red[4], (*it)->m_red[5]);
			Lucky::SetTask(strTask);
		}

		// checking...
		if (!condition.Meet(*(*it)))
			delete (*it);
		else
		{
			iNewIndex ++;
			result.push_back(*it);
		}

		// Update progress...
		Lucky::SetPos(iIndex * 1000 / iTotal);
	}

	Lucky::ShowProgress(false);
	return true;
}

bool CAnalysisUtil::GetNumbersFromString(const CString& str, std::set<int>& nums)
{
	int iStart = 0, iEnd = -1, iLength = str.GetLength();
	while (iStart < iLength)
	{
		iEnd = str.Find(',', iStart);
		if (iEnd < 0)
		{
			iEnd = iLength;
		}

		CString strNum = str.Mid(iStart, iEnd - iStart);
		int iNum = _ttoi(strNum);
		if (iNum > 33 || iNum < 1)
			return false;

		nums.insert(iNum);
		iStart = iEnd + 1;
	}

	return true;
}

void CAnalysisUtil::DeleteAll(LuckyNumSet& set)
{
	for (LuckyNumSetIt it = set.begin(); it != set.end(); ++ it)
	{
		delete *it;
	}

	set.clear();
}

void CAnalysisUtil::CopyTo(const LuckyNumSet& from, LuckyNumSet& to, bool bHardCopy /*= false*/)
{
	to.clear();

	for (LuckyNumSetConstIt it = from.begin(); it != from.end(); ++ it)
	{
		if (bHardCopy)
			to.push_back(new CLuckyNum(**it));
		else
			to.push_back(*it);
	}
}

bool CAnalysisUtil::MatrixingInResult(int hitAtLeast, LuckyNumSet& result)
{
	if (hitAtLeast < 0 || hitAtLeast > 6)
		return false;

	LuckyNumSet all, filtered;
	CopyTo(result, all);
	CopyTo(all, filtered);
	Matrixing(hitAtLeast, filtered);
	result.clear();

	for (size_t i = 0; i < filtered.size(); ++ i)
	{
		result.push_back(new CLuckyNum(*(filtered[i])));
	}

	DeleteAll(all);

	return true;
}

void CAnalysisUtil::Matrixing(int hitAtLeast, LuckyNumSet& result)
{
	if (hitAtLeast < 0 || hitAtLeast > 6)
		return;

	if (result.size() <= 1)
		return;

	CLuckyNum* selectedSeed = NULL;
	LuckyNumSet tempSelected, selected;
	for (size_t seedIndex = 0; seedIndex < result.size(); ++ seedIndex)
	{
		tempSelected.clear();
		CopyTo(result, tempSelected);

		CLuckyNum* seed = result[seedIndex];
		tempSelected.erase(tempSelected.begin() + seedIndex);

		// Filtering by seed...
		Filter(seed, hitAtLeast, tempSelected);

		// Get better one...
		if (selected.empty() || selected.size() > tempSelected.size())
		{
			selected.clear();
			CopyTo(tempSelected, selected);
			selectedSeed = seed;
		}
	}

	// operate next...
	Matrixing(hitAtLeast, selected);

	// Add seed...
	selected.push_back(selectedSeed);

	result.clear();
	CopyTo(selected, result);
}

void CAnalysisUtil::Filter(CLuckyNum* seed, int hitAtLeast, LuckyNumSet& result)
{
	for (size_t i = 0; i < result.size();)
	{
		int iSimilarity = GetSimilarity(*seed, *(result[i]));
		if (iSimilarity >= hitAtLeast)
		{
			result.erase(result.begin() + i);
		}
		else
		{
			++ i;
		}
	}
}

bool CAnalysisUtil::Matrixing(const CNumSet& numbers, int hitAtLeast, LuckyNumSet& result)
{
	ASSERT(result.size() == 0);
	result.clear();

	int iIndex = 0;
	for (int inx1 = 1; inx1 <= 28; inx1 ++)
	{
		if (!numbers.Contains(inx1))
			continue;

		for (int inx2 = inx1 + 1; inx2 <= 29; inx2 ++)
		{
			if (!numbers.Contains(inx2))
				continue;

			for (int inx3 = inx2 + 1; inx3 <= 30; inx3 ++)
			{
				if (!numbers.Contains(inx3))
					continue;

				for (int inx4 = inx3 + 1; inx4 <= 31; inx4 ++)
				{
					if (!numbers.Contains(inx4))
						continue;

					for (int inx5 = inx4 + 1; inx5 <= 32; inx5 ++)
					{
						if (!numbers.Contains(inx5))
							continue;

						for (int inx6 = inx5 + 1; inx6 <= 33; inx6 ++)
						{
							if (!numbers.Contains(inx6))
								continue;

							// checking...
							CLuckyNum* pNUM = new CLuckyNum(inx1, inx2, inx3, inx4, inx5, inx6, 0);
							result.push_back(pNUM);
						}
					}
				}
			}
		}
	}	

	return MatrixingInResult(hitAtLeast, result);
}

void CAnalysisUtil::GetAllNums(LuckyNums& nums)
{
	int iIndex = 0;
	for (int inx1 = 1; inx1 <= 28; inx1 ++)
		for (int inx2 = inx1 + 1; inx2 <= 29; inx2 ++)
			for (int inx3 = inx2 + 1; inx3 <= 30; inx3 ++)
				for (int inx4 = inx3 + 1; inx4 <= 31; inx4 ++)
					for (int inx5 = inx4 + 1; inx5 <= 32; inx5 ++)
						for (int inx6 = inx5 + 1; inx6 <= 33; inx6 ++)
						{
							// checking...
							CLuckyNum* pNUM = new CLuckyNum(inx1, inx2, inx3, inx4, inx5, inx6, 0);
							nums.insert(std::pair<int, CLuckyNum*>(++ iIndex, pNUM));
						}
}

void CAnalysisUtil::GetNumProb(const LuckyNums& nums, double* pdProb)
{
	if (pdProb == NULL) return;

	int iTotal = (int)nums.size();
	int HitCounts[33] = {0};
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++)
	{
		for (int i = 0; i < 6; i ++)
		{
			HitCounts[it->second->m_red[i] - 1] ++;
		}
	}

	for (int i = 0; i < 33; i ++)
	{
		*(pdProb + i) = (double)HitCounts[i] / iTotal;
	}
}

bool CAnalysisUtil::MatchTest(const LuckyNumSet& numbers, const CLuckyNum& luckyNum, int* results)
{
	if (results == NULL)
		return false;

	for (int i = 0; i < 6; ++ i)
		results[i] = 0;

	for (LuckyNumSetConstIt it = numbers.begin(); it != numbers.end(); ++ it)
	{
		CLuckyNum* test = *it;
		int comp = GetSimilarity(*test, luckyNum);
		if (comp > 0)
			results[comp - 1] ++;
	}

	return true;
}

void CAnalysisUtil::GetNumProbInPos(const LuckyNums& nums, double* pdProb)
{
	if (pdProb == NULL) return;

	int iTotal = (int)nums.size();
	int HitCounts[33][6] = {0};
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++)
	{
		for (int i = 0; i < 6; i ++)
		{
			HitCounts[it->second->m_red[i] - 1][i] ++;
		}
	}

	for (int i = 0; i < 33; i ++)
	{
		for (int j = 0; j < 6; j ++)
			*(pdProb ++) = (double)(HitCounts[i][j] * 100) / iTotal;
	}
}

void CAnalysisUtil::CalulateKDJ(double iCur, double iMax, double iMin, double& k, double& d, double& j)
{
	ASSERT(iMax != iMin);
	double RSV = (iCur - iMin) / (iMax - iMin) * 60;
	k =  k * 2/3 + RSV * 1/3;
	d =  d * 2/3+ k * 1/3;
	j = d * 3 - k * 2;
}

void CAnalysisUtil::GetRagion(int* pNum, double& iMax, double& iMin, int iCount /*= 9*/)
{
	iMax = -1000;
	iMin = 1000;
	for (int i = 0; i < iCount; i ++)
	{
		if (*(pNum + i)> iMax) iMax = *(pNum + i);
		else if (*(pNum + i) < iMin) iMin = *(pNum + i);
	}
}

void CAnalysisUtil::GetRagion(double* pNum, double& iMax, double& iMin, int iCount /*= 9*/)
{
	iMax = -1000;
	iMin = 1000;
	for (int i = 0; i < iCount; i ++)
	{
		if (*(pNum + i)> iMax) iMax = *(pNum + i);
		else if (*(pNum + i) < iMin) iMin = *(pNum + i);
	}
}

bool CAnalysisUtil::RandomRedNums(CNumSet& result, 
	int limit /*= 1*/, const CNumSet& exclusion /*= CNumSet()*/, const CNumSet& from /*= CNumSet(CRagion(1, 32))*/)
{
	if (from.Count() < limit)
		return false;

	static bool bFirstTime = true;
	if (bFirstTime)
	{
		srand((unsigned int)time(NULL));
		bFirstTime = false;
	}

	int iCount = 0;
	while (iCount < limit)
	{
		bool bValid = false;
		while (!bValid)
		{
			int num = 1 + rand() % 33;
			if (from.Contains(num) && !result.Contains(num) && !exclusion.Contains(num))
			{
				result.AddNum(num);
				bValid = true;
			}
		}

		iCount ++;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
CKDJAnalyzer::CKDJAnalyzer(int iRefOffset /*= 10*/)
: m_iOffset(iRefOffset)
{
}

CKDJAnalyzer::~CKDJAnalyzer()
{
}

int CKDJAnalyzer::AddValue(int iVal, KDJ& kdj /*= KDJ()*/)
{
	m_Values.push_back(iVal);
	int iPos = (int)m_Values.size() - 1;

	if (iPos > m_iOffset)
	{
		// Initialize as the last.
		kdj = m_KDJValues.size() == 0 ? KDJ() : m_KDJValues[m_KDJValues.size() - 1];

		// Get max and min value within the last ragion.
		int iMax = 0, iMin = 0;
		GetRagion(iPos, iMax, iMin);

		CAnalysisUtil::CalulateKDJ(iVal, iMax, iMin, kdj.m_dK, kdj.m_dD, kdj.m_dJ);
		m_KDJValues.push_back(kdj);
	}

	return iPos;
}

bool CKDJAnalyzer::GetKDJ(int iPos, KDJ& kdj)
{
	if (iPos < 0 || iPos >= (int)m_KDJValues.size() + m_iOffset)
		return false;

	kdj = m_KDJValues[iPos - m_iOffset];
	return true;
}

void CKDJAnalyzer::GetRagion(int iIndex, int& iMax, int& iMin)
{
	if (iIndex < m_iOffset || iIndex > (int)m_Values.size()) return;

	iMax = -100000;
	iMin = 100000;
	for (int i = 1; i <= m_iOffset; i ++)
	{
		if (m_Values[iIndex - i] > iMax) iMax = m_Values[iIndex - i];
		else if (m_Values[iIndex - i] < iMin) iMin = m_Values[iIndex - i];
	}
}