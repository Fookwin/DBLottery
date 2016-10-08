#pragma once
#include "..\Data\Analysis\HistoryData.h"
#include "..\Data\Constraint\Constraint.h"
#include <map>

class CAnalysisUtil
{
public:
	typedef std::vector<CLuckyNum*> LuckyNumSet;
	typedef std::vector<CLuckyNum*>::iterator LuckyNumSetIt;
	typedef std::vector<CLuckyNum*>::const_iterator LuckyNumSetConstIt;
	static void DeleteAll(LuckyNumSet& set);
	static void CopyTo(const LuckyNumSet& from, LuckyNumSet& to, bool bHardCopy = false);

	static bool Suggest(const Constraints& condition, LuckyNumSet& result);
	static bool SuggestInResult(const Constraints& condition, LuckyNumSet& result);

	static bool GetNumbersFromString(const CString& str, std::set<int>& nums);

	static int  GetSimilarity(const CLuckyNum& test1, const CLuckyNum& test2);
	static int  GetAdjacentSimilarity(const CLuckyNum& test1, const CLuckyNum& test2);

	static void GetAllNums(LuckyNums& nums);
	static void GetNumProb(const LuckyNums& nums, double* pdProb);
	static void GetNumProbInPos(const LuckyNums& nums, double* pdProb);
	static void CalulateKDJ(double iCur, double iMax, double iMin, double& k, double& d, double& j);
	static void GetRagion(int* pNum, double& iMax, double& iMin, int iCount = 9);
	static void GetRagion(double* pNum, double& iMax, double& iMin, int iCount = 9);

	static bool Matrixing(const CNumSet& numbers, int hitAtLeast, LuckyNumSet& result);
	static bool MatrixingInResult(int hitAtLeast, LuckyNumSet& result);

	static bool MatchTest(const LuckyNumSet& numbers, const CLuckyNum& luckyNum, int* results);

	static bool RandomRedNums(CNumSet& result, int limit = 1, const CNumSet& exclusion = CNumSet(), const CNumSet& from = CNumSet(CRagion(1, 32)));

private:
	static void Filter(CLuckyNum* seed, int hitAtLeast, LuckyNumSet& result);
	static void Matrixing(int hitAtLeast, LuckyNumSet& result);
};

struct KDJ
{
	KDJ() : m_dK(50), m_dD(50), m_dJ(50) {};
	double m_dK, m_dD, m_dJ;
};

class CKDJAnalyzer
{
public:
	CKDJAnalyzer(int iRefOffset = 10);
	~CKDJAnalyzer();

	int AddValue(int iVal, KDJ& kdj = KDJ());
	bool GetKDJ(int iPos, KDJ& kdj);

private:
	void GetRagion(int iIndex, int& iMax, int& iMin);

	std::vector<int> m_Values;
	std::vector<KDJ> m_KDJValues;
	int				 m_iOffset;
};
