#pragma once
#include "../LuckyNumber.h"
#include <vector>
#include <set>

struct NumState
{
	NumState() : m_iHit(0), m_iMissing(0), m_iMaxMissing(0), m_iAverageMissing(0)
	{}

	int m_iHit;
	int m_iMissing;
	int m_iMaxMissing;
	int m_iAverageMissing;
};

class CNumberCondition
{
public:
	CNumberCondition() {};
	~CNumberCondition() {};

	NumState	m_RedNumStates[33];
	NumState	m_BlueNumStates[16];
};

class CHistoryData
{
public:
	CHistoryData();
	~CHistoryData();

	bool Init();

	const LuckyNums& GetHistory() const {return m_history;}
	int GetHistoryCount() const { return (int)m_history.size(); }
	const CLuckyNum* GetLastNum() const { return m_last; }
	const CLuckyNum* GetTestNum() const;
	const CNumberCondition* GetLastCondition() const { return m_lastCondition; }
	const CNumberCondition* GetTestCondition() const;
	const CNumberCondition* GetCondition(int issue) const;

	bool IsInitialized() const { return m_bInitialized; }

	CNumberCondition* GetHitCount(int issue);
	int GetRedMissing(int issue);
	int GetBlueMissing(int issue);

private:
	bool Analyze();

	CString								m_strFileName;
	bool								m_bInitialized;

	// history data...
	LuckyNums							m_history;
	std::map<int, CNumberCondition*>	m_NumConditions;
	CLuckyNum*							m_last;
	CNumberCondition*					m_lastCondition;
};


