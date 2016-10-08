// ContinuityTab.cpp : implementation file
//

#include "stdafx.h"
#include "..\LuckyBalls.h"
#include "MissingTab.h"
#include "..\Data\Analysis\HistoryData.h"
#include "..\Utilities\AnalysisUtil.h"


// CMissingTab dialog

IMPLEMENT_DYNAMIC(CMissingTab, CDialog)

CMissingTab::CMissingTab(CWnd* pParent /*=NULL*/)
	: CDialog(CMissingTab::IDD, pParent), m_pHistory(NULL)
{
}

CMissingTab::~CMissingTab()
{
}

void CMissingTab::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT_ISSUE_STEP, m_editIssueStep);
	DDX_Control(pDX, IDC_LIST_RESULT, m_ResultList);
}


BEGIN_MESSAGE_MAP(CMissingTab, CDialog)
	ON_BN_CLICKED(IDC_BUTTON_CALCULATE, &CMissingTab::OnBnClickedButtonCalculate)
END_MESSAGE_MAP()


// CMissingTab message handlers
BOOL CMissingTab::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	DWORD dwStyle2 = m_ResultList.GetExtendedStyle();
	dwStyle2 |= LVS_EX_FULLROWSELECT;
	dwStyle2 |= LVS_EX_GRIDLINES;
	m_ResultList.SetExtendedStyle(dwStyle2);

	m_ResultList.InsertColumn(0, _T("ÒÅÂ©"), LVCFMT_CENTER, 50);
	m_ResultList.InsertColumn(1, _T("´ÎÊý"), LVCFMT_CENTER, 50);
	m_ResultList.InsertColumn(2, _T("Êý×Ö"), LVCFMT_CENTER, 300);

	CString strTemp;
	strTemp.Format(_T("%d"), 5);
	m_editIssueStep.SetWindowText(strTemp);

	return TRUE;
}

void CMissingTab::Update()
{
	m_ResultList.DeleteAllItems();

	CString strTemp;
	m_editIssueStep.GetWindowText(strTemp);
	int iStep = _ttoi(strTemp);

	const LuckyNums& nums = m_pHistory->GetHistory();
	int iIndex = 0, iTotal = (int)nums.size();
	int Missing[33] = {0};
	std::vector<int> iHits;
	for (int i = 0; i <= iStep; i ++)
		iHits.push_back(0);

	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++, iIndex ++)
	{
		if (iIndex < iTotal - 3 * iStep)
			continue;

		for (int j = 1, iPos = 0; j <= 33; j ++)
		{
			if (it->second->m_red[iPos] != j)
			{
				++ Missing[j - 1];
			}
			else
			{
				int iMis = min(Missing[j - 1], iStep);
				iHits[iMis] ++;
				Missing[j - 1] = 0;
				iPos ++;
			}
		}
	}

	for (int i = 0; i < (int)iHits.size(); i ++)
	{
		strTemp.Format(_T("%d"), i);
		m_ResultList.InsertItem(i, strTemp);

		strTemp.Format(_T("%d"), iHits[i]);
		m_ResultList.SetItemText(i, 1, strTemp);

		CString strNums;
		for (int j = 1; j <= 33; j ++)
		{
			if (min(Missing[j - 1], iStep) == i)
			{
				strTemp.Format(_T("%d,"), j);
				strNums += strTemp;
			}
		}

		m_ResultList.SetItemText(i, 2, strNums);
	}
}


void CMissingTab::OnBnClickedButtonCalculate()
{
	Update();
}
