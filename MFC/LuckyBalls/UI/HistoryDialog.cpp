// HistoryDialog.cpp : implementation file
//

#include "stdafx.h"
#include "..\LuckyBalls.h"
#include "HistoryDialog.h"
#include "..\Data\Analysis\HistoryData.h"
#include "..\Utilities\AnalysisUtil.h"


// CHistoryDialog dialog

IMPLEMENT_DYNAMIC(CHistoryDialog, CDialog)

CHistoryDialog::CHistoryDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CHistoryDialog::IDD, pParent), m_pHistory(NULL)
{

}

CHistoryDialog::~CHistoryDialog()
{
}

void CHistoryDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_HISTORY, m_HistoryList);
	DDX_Control(pDX, IDC_STATIC_REPORT, m_staticReport);
}


BEGIN_MESSAGE_MAP(CHistoryDialog, CDialog)
END_MESSAGE_MAP()

static bool s_bReset = false;
void SuggestionTest(const CNumberCondition* pCondi, CNumSet& redSuggestion)
{
	const int iTotalCap = 9;
	std::map<int, const NumState*> candidates;
	for (int i = 1; i <= 33; ++ i)
	{
		candidates.insert(std::make_pair(i, &pCondi->m_RedNumStates[i-1]));
	}

	for (int cap = 1; cap <= iTotalCap; ++cap)
	{
		int iSelect1 = -1, iSelect2 = -1, iMax = -1, iMin = -1;
		for (std::map<int, const NumState*>::iterator it = candidates.begin(); it != candidates.end(); ++ it)
		{
			if (iMax < 0 || iMax > it->second->m_iHit)
			{
				iSelect1 = it->first;
				iMax = it->second->m_iHit;
			}

			if (iMin < it->second->m_iHit)
			{
				iSelect2 = it->first;
				iMin = it->second->m_iHit;
			}
		}

		redSuggestion.AddNum(iSelect1);
		candidates.erase(iSelect1);
		//redSuggestion.AddNum(iSelect2);
		//candidates.erase(iSelect2);
	}	

	static CNumSet randomRest;
	if (randomRest.Count() == 0 || s_bReset)
	{
		randomRest.Clear();
		CAnalysisUtil::RandomRedNums(randomRest, 0, redSuggestion);
		s_bReset = false;
	}

	redSuggestion.AddNums(randomRest);			

	candidates.clear();
	for (int i = 1; i <= 33; ++ i)
	{
		candidates.insert(std::make_pair(i, &pCondi->m_RedNumStates[i-1]));
	}

	for (int cap = 1; cap <= 0; ++cap)
	{
		int iSelect = -1, iMax = -1;
		for (std::map<int, const NumState*>::iterator it = candidates.begin(); it != candidates.end(); ++ it)
		{
			if (iMax < it->second->m_iMissing)
			{
				iSelect = it->first;
				iMax = it->second->m_iMissing;
			}
		}

		redSuggestion.AddNum(iSelect);
		candidates.erase(iSelect);
	}
}

// CHistoryDialog message handlers
BOOL CHistoryDialog::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	DWORD dwStyle = m_HistoryList.GetExtendedStyle();
	dwStyle |= LVS_EX_FULLROWSELECT;
	dwStyle |= LVS_EX_GRIDLINES;
	m_HistoryList.SetExtendedStyle(dwStyle);

	m_HistoryList.InsertColumn(0, _T("NO."), LVCFMT_CENTER, 60);
	m_HistoryList.InsertColumn(1, _T("R1"), LVCFMT_CENTER, 40);
	m_HistoryList.InsertColumn(2, _T("R2"), LVCFMT_CENTER, 40);
	m_HistoryList.InsertColumn(3, _T("R3"), LVCFMT_CENTER, 40);
	m_HistoryList.InsertColumn(4, _T("R4"), LVCFMT_CENTER, 40);
	m_HistoryList.InsertColumn(5, _T("R5"), LVCFMT_CENTER, 40);
	m_HistoryList.InsertColumn(6, _T("R6"), LVCFMT_CENTER, 40);
	m_HistoryList.InsertColumn(7, _T("B"), LVCFMT_CENTER, 40);
	m_HistoryList.InsertColumn(8, _T("CON"), LVCFMT_CENTER, 45);
	m_HistoryList.InsertColumn(9, _T("EVEN"), LVCFMT_CENTER, 45);
	m_HistoryList.InsertColumn(10, _T("SUM"), LVCFMT_RIGHT, 45);
	m_HistoryList.InsertColumn(11, _T("REP"), LVCFMT_CENTER, 45);
	m_HistoryList.InsertColumn(12, _T("ADJ"), LVCFMT_CENTER, 45);
	m_HistoryList.InsertColumn(13, _T("MISS"), LVCFMT_RIGHT, 45);
	m_HistoryList.InsertColumn(14, _T("10+|5~10|5~3|3~0| 0"), LVCFMT_RIGHT, 150);
	m_HistoryList.InsertColumn(15, _T("RED MATRIX"), LVCFMT_CENTER, 100);
	m_HistoryList.InsertColumn(16, _T("BLUE MATRIX"), LVCFMT_CENTER, 70);
	m_HistoryList.InsertColumn(17, _T("Red in recent 5"), LVCFMT_LEFT, 300);
	m_HistoryList.InsertColumn(18, _T("Red Suggestion"), LVCFMT_LEFT, 150);

	CString strFinalReport;
	int iLowScore = -1;
	int iTryCount = 150000;
	//while (iTryCount > 0)
	{
		s_bReset = true;

		int iScore = 0;
		CString strReport;
		Init(iScore, strReport);

		if (iLowScore < 0 || iLowScore > iScore)
		{
			iLowScore = iScore;
			strFinalReport = strReport;
		}

		-- iTryCount;
	}

	m_staticReport.SetWindowText(strFinalReport);

	return TRUE;
}

void CHistoryDialog::Init(int& iScore,  CString& strReport)
{
	m_HistoryList.DeleteAllItems();
	m_staticReport.SetWindowText(_T(""));
	
	int Missing[33] = {0};

	CString temp;
	const LuckyNums& history = m_pHistory->GetHistory();
	int iIssueCount = (int)history.size();
	int iIndex = 0;
	LuckyNums::const_iterator itPrv = history.end();
	const int iSkipIssueCount = (int)history.size() - 1000;
	int iSuccessRates[7] = {0};

	enum
	{
		kSum = 0,
		kEven,
		kContinue,
		kRepeat,
		kAdjacent,
		kMissing,
		kMissing1,
		kMissing2,
		kMissing3,
		kMissing4,
		kMissing5
	};

	// calculate the data for sum[0], even[1], continue[2], repeat[3], adjacent[4],
	// missing[5], missing>=10[6], missing>=5[7], missing>3[8], missing>0[9], missing=0[10],
	int iTotal[11] = {0};
	int iLast100[11] = {0};
	int iLast50[11] = {0};
	int iLast30[11] = {0};
	int iLast10[11] = {0};
	int iLast5[11] = {0};
	int iRedMissing[33][5] = {0};
	for (LuckyNums::const_iterator it = history.begin(); it != history.end(); itPrv = it, it ++, iIndex ++)
	{
		temp.Format(_T("%d"), it->first);
		m_HistoryList.InsertItem(iIndex, temp);

		temp.Format(_T("%.2d"), it->second->m_red[0]);
		m_HistoryList.SetItemText(iIndex, 1, temp);
		temp.Format(_T("%.2d"), it->second->m_red[1]);
		m_HistoryList.SetItemText(iIndex, 2, temp);
		temp.Format(_T("%.2d"), it->second->m_red[2]);
		m_HistoryList.SetItemText(iIndex, 3, temp);
		temp.Format(_T("%.2d"), it->second->m_red[3]);
		m_HistoryList.SetItemText(iIndex, 4, temp);
		temp.Format(_T("%.2d"), it->second->m_red[4]);
		m_HistoryList.SetItemText(iIndex, 5, temp);
		temp.Format(_T("%.2d"), it->second->m_red[5]);
		m_HistoryList.SetItemText(iIndex, 6, temp);

		temp.Format(_T("%.2d"), it->second->m_blue);
		m_HistoryList.SetItemText(iIndex, 7, temp);

		// sum...
		int iSum = it->second->GetSum();
		temp.Format(_T("%d"), iSum);
		m_HistoryList.SetItemText(iIndex, 10, temp);

		iTotal[kSum] += iSum;
		if (iIssueCount - iIndex <= 100) iLast100[kSum] += iSum;
		if (iIssueCount - iIndex <= 50) iLast50[kSum] += iSum;
		if (iIssueCount - iIndex <= 30) iLast30[kSum] += iSum;
		if (iIssueCount - iIndex <= 10) iLast10[kSum] += iSum;
		if (iIssueCount - iIndex <= 5) iLast5[kSum] += iSum;

		// even...
		int iEven = it->second->GetEvenNumCount();
		temp.Format(_T("%d"), iEven);
		m_HistoryList.SetItemText(iIndex, 9, temp);

		iTotal[kEven] += iEven;
		if (iIssueCount - iIndex <= 100) iLast100[kEven] += iEven;
		if (iIssueCount - iIndex <= 50) iLast50[kEven] += iEven;
		if (iIssueCount - iIndex <= 30) iLast30[kEven] += iEven;
		if (iIssueCount - iIndex <= 10) iLast10[kEven] += iEven;
		if (iIssueCount - iIndex <= 5) iLast5[kEven] += iEven;

		// continue...
		int iContinuity = it->second->GetContinuity();
		temp.Format(_T("%d"), iContinuity);
		m_HistoryList.SetItemText(iIndex, 8, temp);

		iTotal[kContinue] += iContinuity;
		if (iIssueCount - iIndex <= 100) iLast100[kContinue] += iContinuity;
		if (iIssueCount - iIndex <= 50) iLast50[kContinue] += iContinuity;
		if (iIssueCount - iIndex <= 30) iLast30[kContinue] += iContinuity;
		if (iIssueCount - iIndex <= 10) iLast10[kContinue] += iContinuity;
		if (iIssueCount - iIndex <= 5) iLast5[kContinue] += iContinuity;

		if (itPrv != history.end())
		{
			// repeat...
			int iRepeat = CAnalysisUtil::GetSimilarity(*it->second, *itPrv->second);
			temp.Format(_T("%d"), iRepeat);
			m_HistoryList.SetItemText(iIndex, 11, temp);

			iTotal[kRepeat] += iRepeat;
			if (iIssueCount - iIndex <= 100) iLast100[kRepeat] += iRepeat;
			if (iIssueCount - iIndex <= 50) iLast50[kRepeat] += iRepeat;
			if (iIssueCount - iIndex <= 30) iLast30[kRepeat] += iRepeat;
			if (iIssueCount - iIndex <= 10) iLast10[kRepeat] += iRepeat;
			if (iIssueCount - iIndex <= 5) iLast5[kRepeat] += iRepeat;

			// adjacent...
			int iAdjacent = CAnalysisUtil::GetAdjacentSimilarity(*it->second, *itPrv->second);
			temp.Format(_T("%d"), iAdjacent);
			m_HistoryList.SetItemText(iIndex, 12, temp);

			iTotal[kAdjacent] += iAdjacent;
			if (iIssueCount - iIndex <= 100) iLast100[kAdjacent] += iAdjacent;
			if (iIssueCount - iIndex <= 50) iLast50[kAdjacent] += iAdjacent;
			if (iIssueCount - iIndex <= 30) iLast30[kAdjacent] += iAdjacent;
			if (iIssueCount - iIndex <= 10) iLast10[kAdjacent] += iAdjacent;
			if (iIssueCount - iIndex <= 5) iLast5[kAdjacent] += iAdjacent;
		}

		if (iIndex > 10)
		{
			int redMtx[6] = {0};
			int iLarger10 = 0, i5To10 = 0, i3T05 = 0, iLess3 = 0, iEqual0 = 0, iMissTotal = 0;
			for (int j = 1, iPos = 0; j <= 33; j ++)
			{
				if (it->second->m_red[iPos] != j)
				{
					++ Missing[j - 1];
				}
				else
				{
					unsigned int iBit = j - 1;
					for (int i = 0; i < 6; i ++)
					{
						if (iBit <= 0) break;
						if (iBit%2 == 1) redMtx[i]++;
						iBit = iBit>>1;
					}

					switch(Missing[j - 1])
					{
					case 0:
						{
							iEqual0 ++;
							iTotal[10] ++;
							if (iIssueCount - iIndex <= 100) iLast100[10] ++;
							if (iIssueCount - iIndex <= 50) iLast50[10] ++;
							if (iIssueCount - iIndex <= 30) iLast30[10] ++;
							if (iIssueCount - iIndex <= 10) iLast10[10] ++;
							if (iIssueCount - iIndex <= 5) iLast5[10] ++;
							break;
						}
					case 1:
					case 2:
						{
							iLess3 ++;
							iTotal[9] ++;
							if (iIssueCount - iIndex <= 100) iLast100[9] ++;
							if (iIssueCount - iIndex <= 50) iLast50[9] ++;
							if (iIssueCount - iIndex <= 30) iLast30[9] ++;
							if (iIssueCount - iIndex <= 10) iLast10[9] ++;
							if (iIssueCount - iIndex <= 5) iLast5[9] ++;
							break;
						}
					case 3:
					case 4:
						{
							i3T05 ++;
							iTotal[8] ++;
							if (iIssueCount - iIndex <= 100) iLast100[8] ++;
							if (iIssueCount - iIndex <= 50) iLast50[8] ++;
							if (iIssueCount - iIndex <= 30) iLast30[8] ++;
							if (iIssueCount - iIndex <= 10) iLast10[8] ++;
							if (iIssueCount - iIndex <= 5) iLast5[8] ++;
							break;
						}
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
						{
							i5To10 ++;
							iTotal[7] ++;
							if (iIssueCount - iIndex <= 100) iLast100[7] ++;
							if (iIssueCount - iIndex <= 50) iLast50[7] ++;
							if (iIssueCount - iIndex <= 30) iLast30[7] ++;
							if (iIssueCount - iIndex <= 10) iLast10[7] ++;
							if (iIssueCount - iIndex <= 5) iLast5[7] ++;
							break;
						}
					default:
						{
							iLarger10 ++;
							iTotal[6] ++;
							if (iIssueCount - iIndex <= 100) iLast100[6] ++;
							if (iIssueCount - iIndex <= 50) iLast50[6] ++;
							if (iIssueCount - iIndex <= 30) iLast30[6] ++;
							if (iIssueCount - iIndex <= 10) iLast10[6] ++;
							if (iIssueCount - iIndex <= 5) iLast5[6] ++;
							break;
						}
					}

					iMissTotal += min(Missing[j - 1], 10);

					Missing[j - 1] = 0;
					iPos ++;
				}
			}

			// missing...
			iTotal[kMissing] += iMissTotal;
			if (iIssueCount - iIndex <= 100) iLast100[kMissing] += iMissTotal;
			if (iIssueCount - iIndex <= 50) iLast50[kMissing] += iMissTotal;
			if (iIssueCount - iIndex <= 30) iLast30[kMissing] += iMissTotal;
			if (iIssueCount - iIndex <= 10) iLast10[kMissing] += iMissTotal;
			if (iIssueCount - iIndex <= 5) iLast5[kMissing] += iMissTotal;

			temp.Format(_T("%d"), iMissTotal);
			m_HistoryList.SetItemText(iIndex, 13, temp);

			CString strMissing;
			temp.Format(_T("%d"), iLarger10); strMissing += temp + _T("  |   ");
			temp.Format(_T("%d"), i5To10); strMissing += temp + _T("   |  ");
			temp.Format(_T("%d"), i3T05); strMissing += temp + _T("  |  ");
			temp.Format(_T("%d"), iLess3); strMissing += temp + _T("  | ");
			temp.Format(_T("%d"), iEqual0); strMissing += temp;
			m_HistoryList.SetItemText(iIndex, 14, strMissing);

			// red matrix.
			CString strRedMtx;
			for (int i = 5; i >= 0; i --)
			{
				temp.Format(_T("%d"), redMtx[i]);
				strRedMtx += temp;
				if (i > 0) strRedMtx += _T("|");
			}
			m_HistoryList.SetItemText(iIndex, 15, strRedMtx);
		}

		// blue matrix.
		CString strBlueMatrix;
		switch (it->second->m_blue)
		{
		case 1: strBlueMatrix = _T("0 0 0 0"); break;
		case 2: strBlueMatrix = _T("0 0 0 1"); break;
		case 3: strBlueMatrix = _T("0 0 1 0"); break;
		case 4: strBlueMatrix = _T("0 0 1 1"); break;
		case 5: strBlueMatrix = _T("0 1 0 0"); break;
		case 6: strBlueMatrix = _T("0 1 0 1"); break;
		case 7: strBlueMatrix = _T("0 1 1 0"); break;
		case 8: strBlueMatrix = _T("0 1 1 1"); break;
		case 9: strBlueMatrix = _T("1 0 0 0"); break;
		case 10: strBlueMatrix = _T("1 0 0 1"); break;
		case 11: strBlueMatrix = _T("1 0 1 0"); break;
		case 12: strBlueMatrix = _T("1 0 1 1"); break;
		case 13: strBlueMatrix = _T("1 1 0 0"); break;
		case 14: strBlueMatrix = _T("1 1 0 1"); break;
		case 15: strBlueMatrix = _T("1 1 1 0"); break;
		case 16: strBlueMatrix = _T("1 1 1 1"); break;
		}
		m_HistoryList.SetItemText(iIndex, 16, strBlueMatrix);	

		if (iIndex >= iSkipIssueCount)
		{
			const CNumberCondition* pCondi = m_pHistory->GetCondition(itPrv->first);

			// suggest reds...
			CNumSet redSuggestion;
			SuggestionTest(pCondi, redSuggestion);

			// Validating the test result...
			int iHitCount = 0;
			for (int m = 0; m < 6; m ++)
			{
				if (redSuggestion.Contains(it->second->m_red[m]))
					iHitCount ++;
			}

			iSuccessRates[iHitCount] ++;

			temp.Format(_T("%d : "), iHitCount);
			m_HistoryList.SetItemText(iIndex, 18, temp + redSuggestion.GetText());
		}
	}

	m_HistoryList.EnsureVisible(iIndex - 1, TRUE);

	// Set the text for the report.
	strReport += _T("Average Status\r\n");

	strReport += _T("| Range | Continue | Even | Sum | Repeat | Adjacent | Missing |\r\n");

	temp.Format(_T("| %d  |"), iIssueCount);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iTotal[kContinue] / iIssueCount);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iTotal[kEven] / iIssueCount);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iTotal[kSum] / iIssueCount);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iTotal[kRepeat] / (iIssueCount - 1));
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iTotal[kAdjacent] / (iIssueCount - 1));
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iTotal[kMissing] / (iIssueCount - 10));
	strReport += temp;

	strReport += _T("\r\n");

	temp.Format(_T("|  %d  |"), 100);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast100[kContinue] / 100);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast100[kEven] / 100);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast100[kSum] / 100);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast100[kRepeat] / 100);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast100[kAdjacent] / 100);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast100[kMissing] / 100);
	strReport += temp;

	strReport += _T("\r\n");

	temp.Format(_T("|  %d   |"), 50);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast50[kContinue] / 50);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast50[kEven] / 50);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast50[kSum] / 50);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast50[kRepeat] / 50);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast50[kAdjacent] / 50);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast50[kMissing] / 50);
	strReport += temp;

	strReport += _T("\r\n");

	temp.Format(_T("|  %d  |"), 30);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast30[kContinue] / 30);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast30[kEven] / 30);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast30[kSum] / 30);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast30[kRepeat] / 30);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast30[kAdjacent] / 30);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast30[kMissing] / 30);
	strReport += temp;

	strReport += _T("\r\n");

	temp.Format(_T("|  %d   |"), 10);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast10[kContinue] / 10);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast10[kEven] / 10);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast10[kSum] / 10);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast10[kRepeat] / 10);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast10[kAdjacent] / 10);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast10[kMissing] / 10);
	strReport += temp;

	strReport += _T("\r\n");

	temp.Format(_T("|   %d   |"), 5);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast5[kContinue] / 5);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast5[kEven] / 5);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast5[kSum] / 5);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast5[kRepeat] / 5);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast5[kAdjacent] / 5);
	strReport += temp;

	temp.Format(_T("  %.1f  |"), (float)iLast5[kMissing] / 5);
	strReport += temp;

	strReport += _T("\r\n\n");

	strReport += _T("Average Missing\r\n");
	strReport += _T("| Range | >=10 | >=5  | >=3  |  >0  |  =0  |\r\n");
	temp.Format(_T("| %d |"), iIssueCount); strReport += temp;
	temp.Format(_T("   %.1f   |"), (float)iTotal[kMissing1] / (iIssueCount - 10)); strReport += temp;
	temp.Format(_T("  %.1f   |"), (float)iTotal[kMissing2] / (iIssueCount - 10)); strReport += temp;
	temp.Format(_T("   %.1f  |"), (float)iTotal[kMissing3] / (iIssueCount - 10)); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iTotal[kMissing4] / (iIssueCount - 10)); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iTotal[kMissing5] / (iIssueCount - 10)); strReport += temp;
	strReport += _T("\r\n");

	temp.Format(_T("|  %d |"), 100); strReport += temp;
	temp.Format(_T("   %.1f   |"), (float)iLast100[kMissing1] / 100); strReport += temp;
	temp.Format(_T("  %.1f   |"), (float)iLast100[kMissing2] / 100); strReport += temp;
	temp.Format(_T("   %.1f  |"), (float)iLast100[kMissing3] / 100); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast100[kMissing4] / 100); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast100[kMissing5] / 100); strReport += temp;
	strReport += _T("\r\n");

	temp.Format(_T("|  %d  |"), 50); strReport += temp;
	temp.Format(_T("   %.1f   |"), (float)iLast50[kMissing1] / 50); strReport += temp;
	temp.Format(_T("  %.1f   |"), (float)iLast50[kMissing2] / 50); strReport += temp;
	temp.Format(_T("   %.1f  |"), (float)iLast50[kMissing3] / 50); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast50[kMissing4] / 50); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast50[kMissing5] / 50); strReport += temp;
	strReport += _T("\r\n");

	temp.Format(_T("|  %d  |"), 30); strReport += temp;
	temp.Format(_T("   %.1f   |"), (float)iLast30[kMissing1] / 30); strReport += temp;
	temp.Format(_T("  %.1f   |"), (float)iLast30[kMissing2] / 30); strReport += temp;
	temp.Format(_T("   %.1f  |"), (float)iLast30[kMissing3] / 30); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast30[kMissing4] / 30); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast30[kMissing5] / 30); strReport += temp;
	strReport += _T("\r\n");

	temp.Format(_T("|  %d  |"), 10); strReport += temp;
	temp.Format(_T("   %.1f   |"), (float)iLast10[kMissing1] / 10); strReport += temp;
	temp.Format(_T("  %.1f   |"), (float)iLast10[kMissing2] / 10); strReport += temp;
	temp.Format(_T("   %.1f  |"), (float)iLast10[kMissing3] / 10); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast10[kMissing4] / 10); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast10[kMissing5] / 10); strReport += temp;
	strReport += _T("\r\n");

	temp.Format(_T("|  %d  |"), 5); strReport += temp;
	temp.Format(_T("   %.1f   |"), (float)iLast5[kMissing1] / 5); strReport += temp;
	temp.Format(_T("  %.1f   |"), (float)iLast5[kMissing2] / 5); strReport += temp;
	temp.Format(_T("   %.1f  |"), (float)iLast5[kMissing3] / 5); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast5[kMissing4] / 5); strReport += temp;
	temp.Format(_T("  %.1f  |"), (float)iLast5[kMissing5] / 5); strReport += temp;
	strReport += _T("\r\n\n");


	// Suggestion result...
	CString strTestResult = _T("Test Result: ");
	iScore = 0;
	int iHitTotal = 0;
	for (int i = 0; i <=6; ++i)
	{
		temp.Format(_T("%d|%d"), iSuccessRates[i], i);
		if (i < 6)
			temp += _T(", ");

		strTestResult += temp;
		iHitTotal += i * iSuccessRates[i];

		switch(i)
		{
		case 0:
		case 1:
		case 2:
			iScore += 5 * iSuccessRates[i]; break;
		case 3:
			iScore += 10 * iSuccessRates[i]; break;
		case 4:
			iScore += 350 * iSuccessRates[i]; break;
		case 5:
			iScore += 6000 * iSuccessRates[i]; break;
		case 6:
			iScore += 100000 * iSuccessRates[i]; break;
		}
	}

	strTestResult += _T("\r\n");
		
	//CString strPay; strPay.Format(_T("Pay:$&.2f"), );
	CString strScore; strScore.Format(_T("Gain:$%.2f or %.2f"), (double)iScore/16, (double)iHitTotal/100);
	strTestResult += strScore;
	strTestResult += _T("\r\n");

	// suggest reds...
	CNumSet redSuggestion;
	SuggestionTest(m_pHistory->GetLastCondition(), redSuggestion);
	CString strSuggestion = _T("Suggest: ");
	strTestResult += strSuggestion + redSuggestion.GetText();

	strReport += strTestResult;	
}
