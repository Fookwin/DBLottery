// AnalysisDialog.cpp : implementation file
//

#include "stdafx.h"
#include "..\LuckyBalls.h"
#include "AnalysisDlg.h"
#include "ContinuityTab.h"
#include "SimilarityTab.h"
#include "HitStatisticsTab.h"
#include "StepsTab.h"
#include "EvenCountTab.h"
#include "MissingTab.h"
#include "..\Data\Analysis\HistoryData.h"


// CAnalysisDlg dialog

IMPLEMENT_DYNAMIC(CAnalysisDlg, CDialog)

CAnalysisDlg::CAnalysisDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CAnalysisDlg::IDD, pParent), m_pHistory(NULL)
{
}

CAnalysisDlg::~CAnalysisDlg()
{
	for (std::vector<CDialog*>::iterator it = m_Tabs.begin(); it != m_Tabs.end(); it ++)
		delete *it;

	m_Tabs.clear();
}

void CAnalysisDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TAB_ANALYSIS, m_TabCtrl);
}


BEGIN_MESSAGE_MAP(CAnalysisDlg, CDialog)
	ON_NOTIFY(TCN_SELCHANGE, IDC_TAB_ANALYSIS, &CAnalysisDlg::OnTcnSelchangeTabAnalysis)
END_MESSAGE_MAP()


// CAnalysisDlg message handlers
BOOL CAnalysisDlg::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	// Initilaize sub tabs.
	CContinuityTab* pContinuityTab = new CContinuityTab(&m_TabCtrl);
	m_Tabs.push_back(pContinuityTab);
	pContinuityTab->SetHistory(m_pHistory);
	pContinuityTab->Create(CContinuityTab::IDD, &m_TabCtrl);

	CSimilarityTab* pSimilarityTab = new CSimilarityTab(&m_TabCtrl);
	m_Tabs.push_back(pSimilarityTab);
	pSimilarityTab->SetHistory(m_pHistory);
	pSimilarityTab->Create(CSimilarityTab::IDD, &m_TabCtrl);

	CHitStatisticsTab* pHitCountTab = new CHitStatisticsTab(&m_TabCtrl);
	m_Tabs.push_back(pHitCountTab);
	pHitCountTab->SetHistory(m_pHistory);
	pHitCountTab->Create(CHitStatisticsTab::IDD, &m_TabCtrl);

	CStepsTab* pStepsTab = new CStepsTab(&m_TabCtrl);
	m_Tabs.push_back(pStepsTab);
	pStepsTab->SetHistory(m_pHistory);
	pStepsTab->Create(CStepsTab::IDD, &m_TabCtrl);

	CEvenCountTab* pEvenCountTab = new CEvenCountTab(&m_TabCtrl);
	m_Tabs.push_back(pEvenCountTab);
	pEvenCountTab->SetHistory(m_pHistory);
	pEvenCountTab->Create(CEvenCountTab::IDD, &m_TabCtrl);

	CMissingTab* pMissingTab = new CMissingTab(&m_TabCtrl);
	m_Tabs.push_back(pMissingTab);
	pMissingTab->SetHistory(m_pHistory);
	pMissingTab->Create(CMissingTab::IDD, &m_TabCtrl);

	// Insert to tab controls.
	m_TabCtrl.InsertItem(0, _T("Continuity"));
	m_TabCtrl.InsertItem(1, _T("Similarity"));
	m_TabCtrl.InsertItem(2, _T("Hit Statistics"));
	m_TabCtrl.InsertItem(3, _T("Steps"));
	m_TabCtrl.InsertItem(4, _T("Even Count"));
	m_TabCtrl.InsertItem(5, _T("Missing"));

	// Activate the first one.
	ActivateTab(0);

	return TRUE;
}

void CAnalysisDlg::ActivateTab(int iTab)
{
	int iTabCount = (int)m_Tabs.size();
	if (iTab < 0 || iTab >= iTabCount) return;

	for (int i = 0; i < iTabCount; i ++)
	{
		if (i == iTab)
		{
			// Show current.
			CRect rc;
			m_TabCtrl.GetClientRect(rc);
			rc.top += 30;
			rc.left += 1;
			m_Tabs[i]->SetWindowPos(NULL, rc.left, rc.top, 0, 0, SWP_NOOWNERZORDER|SWP_NOZORDER|SWP_NOSIZE|SWP_NOREDRAW);
			m_Tabs[i]->ShowWindow(SW_SHOW);
		}
		else
		{
			// Hide other.
			m_Tabs[i]->ShowWindow(SW_HIDE);
		}
	}
}

void CAnalysisDlg::OnTcnSelchangeTabAnalysis(NMHDR *pNMHDR, LRESULT *pResult)
{
	ActivateTab(m_TabCtrl.GetCurSel());
	*pResult = 0;
}
