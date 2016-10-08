#include "stdafx.h"
#include "HistoryData.h"
#include <fstream>
#include <iostream>
#include <afxinet.h>
#include "..\..\Server\Global.h"

///////////////////////////////////////////////////////////////////////////////////////////////////
CHistoryData::CHistoryData() : m_bInitialized(false), m_last(NULL), m_lastCondition(NULL)
{
}

CHistoryData::~CHistoryData()
{
	for (LuckyNums::iterator it = m_history.begin(); it != m_history.end(); it ++)
		delete it->second;
	m_history.clear();

	for (std::map<int, CNumberCondition*>::iterator it = m_NumConditions.begin(); it != m_NumConditions.end(); it ++)
		delete it->second;
	m_NumConditions.clear();
};

const CLuckyNum* CHistoryData::GetTestNum() const
{
	if (Lucky::GetTestIssue() < 0)
		return GetLastNum();
	else
	{
		LuckyNums::const_iterator it = m_history.find(Lucky::GetTestIssue());
		if (it != m_history.end())
			return it->second;
		else
		{
			ASSERT(0);
			return NULL;
		}
	}
}

const CNumberCondition* CHistoryData::GetTestCondition() const
{
	if (Lucky::GetTestIssue() < 0)
		return GetLastCondition();
	else
	{
		std::map<int, CNumberCondition*>::const_iterator it = m_NumConditions.find(Lucky::GetTestIssue());
		if (it != m_NumConditions.end())
			return it->second;
		else
		{
			ASSERT(0);
			return NULL;
		}
	}
}

bool CHistoryData::Init()
{
	//CInternetSession session(_T("Downloading data from Server..."));
	//CFtpConnection* pConn = session.GetFtpConnection(_T("ftp.pi3011314.web-79.com"), 
	//	_T("pi3011314"), _T("pi3011314"), 0, TRUE);
	//if (!pConn)
	//{
	//	MessageBox(NULL, _T("Cannot connect to server!"), _T("Downloading data from Server..."), MB_OK);

	//	session.Close();
	//	return false;
	//}

	//BOOL bSuccess = pConn->SetCurrentDirectory(_T("data"));
	//if(bSuccess)
	//{
		//CString strSourceFile = _T("history_sever.txt");
		m_strFileName = _T("Z:/history.txt");
	//	bSuccess = pConn->GetFile(strSourceFile, m_strFileName, FALSE);

	//	pConn->Close();
	//	delete pConn;
	//	session.Close();
	//}

	//if (!bSuccess)
	//{
	//	MessageBox(NULL, _T("Fail to update history data!"), _T("Downloading data from Server..."), MB_OK);
	//}

	CFileStatus rStatus;
	if (!CFile::GetStatus(m_strFileName, rStatus))
	{
		Lucky::AddNote(CString(_T("Failed to load history data!")));
		return false;
	}
	
	Lucky::AddNote(CString(_T("Loading history data ...")));

	CLuckyNum* pData = NULL;
	int iIndex = 0;
	int num=0;
	int iIssue = -1;
	std::ifstream in(m_strFileName,std::ios::in);
	try
	{
		while(in.good())
		{
			in>>num;
			ASSERT(num);
			++iIndex;

			if (iIndex == 1)
			{
				iIssue = num;
				pData = new CLuckyNum();
			}
			else if (iIndex > 1 && iIndex < 8)
			{
				pData->m_red[iIndex - 2] = num;
			}
			else if (iIndex == 8)
			{
				pData->m_blue = num;
				m_history.insert(std::pair<int, CLuckyNum*>(iIssue, pData));
				iIssue = -1;
				pData = NULL;
				iIndex = 0;
			}
			else
			{
				break;
			}
		}
	}
	catch(...)
	{
	}

	in.close();

	if (pData)
	{
		// delete invalid data.
		delete pData;
		pData = NULL;
	}

	LuckyNums::reverse_iterator itLast = m_history.rbegin();
	CString strMsg;
	strMsg.Format(_T("History data is updated to NO. %d."), itLast->first);
	Lucky::AddNote(strMsg);

	// Analyze...
	m_bInitialized = Analyze();

	return m_bInitialized;
}

CNumberCondition* CHistoryData::GetHitCount(int issue)
{
	std::map<int, CNumberCondition*>::const_iterator it = m_NumConditions.find(issue);
	if (it == m_NumConditions.end())
		return NULL;

	return it->second;
}

const CNumberCondition* CHistoryData::GetCondition(int issue) const
{
	std::map<int, CNumberCondition*>::const_iterator itCondi = m_NumConditions.find(issue);
	if (itCondi == m_NumConditions.end())
		return NULL;

	return itCondi->second;
}

int CHistoryData::GetRedMissing(int issue)
{
	std::map<int, CNumberCondition*>::const_iterator itCondi = m_NumConditions.find(issue);
	if (itCondi == m_NumConditions.end())
		return -1;

	if (itCondi == m_NumConditions.begin())
		return 0;

	itCondi --;

	CNumberCondition* pCondition = itCondi->second;

	LuckyNums::iterator it = m_history.find(issue);
	if (it == m_history.end())
		return -1;

	int iMissing = 0;
	for (int i = 0; i < 6; i ++)
	{
		iMissing += pCondition->m_RedNumStates[it->second->m_red[i] - 1].m_iMissing;
	}

	return iMissing;
}

int CHistoryData::GetBlueMissing(int issue)
{
	std::map<int, CNumberCondition*>::const_iterator itCondi = m_NumConditions.find(issue);
	if (itCondi == m_NumConditions.end())
		return -1;

	if (itCondi == m_NumConditions.begin())
		return 0;

	itCondi --;

	CNumberCondition* pCondition = itCondi->second;

	LuckyNums::iterator it = m_history.find(issue);
	if (it == m_history.end())
		return -1;

	return pCondition->m_BlueNumStates[it->second->m_blue - 1].m_iMissing;
}

bool CHistoryData::Analyze()
{
	Lucky::AddNote(CString(_T("Analyzing history data ...")));

	const LuckyNums& nums = GetHistory();

	int iTotal = 1;
	int missingTotalRed[33] = {0};
	int missingTotalBlue[16] = {0};
	NumState statesRed[33];
	NumState statesBlue[16];
	CNumberCondition* pCondi = NULL;
	CLuckyNum* pLastNum = NULL;
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); pLastNum = it->second, it ++, iTotal ++)
	{
		// Red numbers.
		for (int i = 1; i <= 33; i ++)
		{
			if (it->second->HasNum(i))
			{
				statesRed[i - 1].m_iHit ++;
				statesRed[i - 1].m_iMissing = 0;
			}
			else
			{
				missingTotalRed[i - 1] ++;
				statesRed[i - 1].m_iMissing ++;
				if (statesRed[i - 1].m_iMaxMissing < statesRed[i - 1].m_iMissing)
					statesRed[i - 1].m_iMaxMissing = statesRed[i - 1].m_iMissing;
			}

			statesRed[i - 1].m_iAverageMissing = statesRed[i - 1].m_iHit ? missingTotalRed[i - 1] / statesRed[i - 1].m_iHit : 0;
		}

		// Blue numbers
		for (int i = 1; i <= 16; i ++)
		{
			if (it->second->m_blue == i)
			{
				statesBlue[i - 1].m_iHit ++;
				statesBlue[i - 1].m_iMissing = 0;
			}
			else
			{
				missingTotalBlue[i - 1] ++;
				statesBlue[i - 1].m_iMissing ++;
				if (statesBlue[i - 1].m_iMaxMissing < statesBlue[i - 1].m_iMissing)
					statesBlue[i - 1].m_iMaxMissing = statesBlue[i - 1].m_iMissing;
			}

			statesBlue[i - 1].m_iAverageMissing = statesBlue[i - 1].m_iHit ? missingTotalBlue[i - 1] / statesBlue[i - 1].m_iHit : 0;
		}

		pCondi = NULL;
		pCondi = new CNumberCondition();
		for (int i = 0; i < 33; i ++)
		{
			pCondi->m_RedNumStates[i] = statesRed[i];
		}

		for (int i = 0; i < 16; i ++)
		{
			pCondi->m_BlueNumStates[i] = statesBlue[i];
		}

		m_NumConditions.insert(std::pair<int, CNumberCondition*>(it->first, pCondi));
	}

	if (pCondi != NULL)
	{
		m_lastCondition = pCondi;
	}

	if (pLastNum != NULL)
	{
		m_last = pLastNum;
	}

	Lucky::AddNote(CString(_T("Done!")));

	return true;
}
