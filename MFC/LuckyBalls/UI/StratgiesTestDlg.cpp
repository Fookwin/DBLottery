// StratgiesTestDlg.cpp : implementation file
//

#include "stdafx.h"
#include "LuckyBalls.h"
#include "StratgiesTestDlg.h"
#include "afxdialogex.h"


// StratgiesTestDlg dialog

IMPLEMENT_DYNAMIC(CStratgiesTestDlg, CDialog)

CStratgiesTestDlg::CStratgiesTestDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CStratgiesTestDlg::IDD, pParent)
{

}

CStratgiesTestDlg::~CStratgiesTestDlg()
{
	for (TestResults::iterator it = m_Details.begin(); it != m_Details.end(); ++ it)
	{
		delete *it;
	}
	m_Details.clear();
}

void CStratgiesTestDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_STATIC_SCORE, m_Score);
	DDX_Control(pDX, IDC_LIST_TEST_DETAIL, m_DetailResult);
}


BEGIN_MESSAGE_MAP(CStratgiesTestDlg, CDialog)
	ON_BN_CLICKED(IDOK, &CStratgiesTestDlg::OnBnClickedOk)
END_MESSAGE_MAP()


// StratgiesTestDlg message handlers


void CStratgiesTestDlg::OnBnClickedOk()
{
	// TODO: Add your control notification handler code here
	CDialog::OnOK();
}

BOOL CStratgiesTestDlg::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	DWORD dwStyle2 = m_DetailResult.GetExtendedStyle();
	dwStyle2 |= LVS_EX_FULLROWSELECT;
	dwStyle2 |= LVS_EX_GRIDLINES;
	m_DetailResult.SetExtendedStyle(dwStyle2);

	m_DetailResult.InsertColumn(0, _T("Issue"), LVCFMT_CENTER, 80);
	m_DetailResult.InsertColumn(1, _T("Red Numbers"), LVCFMT_CENTER, 150);
	m_DetailResult.InsertColumn(2, _T("Min"), LVCFMT_CENTER, 50);
	m_DetailResult.InsertColumn(3, _T("Max"), LVCFMT_CENTER, 50);
	m_DetailResult.InsertColumn(4, _T("Count"), LVCFMT_CENTER, 80);
	m_DetailResult.InsertColumn(5, _T("Detail (6|5|4|3)"), LVCFMT_CENTER, 150);

	UpdateControls();

	return TRUE;
}

void CStratgiesTestDlg::UpdateControls()
{
	m_DetailResult.DeleteAllItems();

	CString strTemp;
	int index = 0;
	double dTotalMinScore = 0.0, dTotalMaxScore = 0.0;
	for (TestResults::iterator it = m_Details.begin(); it != m_Details.end(); ++ it, ++ index)
	{
		CTestResult* result = *it;
		strTemp.Format(_T("%d"), result->m_Issue);
		m_DetailResult.InsertItem(index, strTemp);

		strTemp.Format(_T("%d %d %d %d %d %d"), result->m_LuckyNum.m_red[0],
			result->m_LuckyNum.m_red[1],
			result->m_LuckyNum.m_red[2],
			result->m_LuckyNum.m_red[3],
			result->m_LuckyNum.m_red[4],
			result->m_LuckyNum.m_red[5]);
		m_DetailResult.SetItemText(index, 1, strTemp);

		strTemp.Format(_T("%d"), result->m_iMinScore);
		m_DetailResult.SetItemText(index, 2, strTemp);

		strTemp.Format(_T("%d"), result->m_iMaxScore);
		m_DetailResult.SetItemText(index, 3, strTemp);

		strTemp.Format(_T("%d"), (int)result->m_CalResult.size());
		m_DetailResult.SetItemText(index, 4, strTemp);

		strTemp.Format(_T("%d | %d | %d | %d"), result->m_MatchStatus[5],
			result->m_MatchStatus[4],
			result->m_MatchStatus[3],
			result->m_MatchStatus[2]);
		m_DetailResult.SetItemText(index, 5, strTemp);

		dTotalMinScore += result->m_iMinScore;
		dTotalMaxScore += result->m_iMaxScore;
	}
	
	strTemp.Format(_T("%.1f ~ %.1f"), dTotalMinScore / (int)m_Details.size(), dTotalMaxScore / (int)m_Details.size());
	m_Score.SetWindowText(strTemp);
}
