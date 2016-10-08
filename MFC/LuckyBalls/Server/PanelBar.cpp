// PanelBar.cpp : implementation file
//

#include "stdafx.h"
#include "Frame\TwoColorBalls.h"
#include "PanelBar.h"
#include "HistoryDialog.h"
#include "CalculateDlg.h"
#include "Server\Global.h"
#include "TwoColorBallsView.h"
#include "TwoColorBallsDoc.h"

// CPanelBar dialog

IMPLEMENT_DYNCREATE(CPanelBar, CDialog)

CPanelBar::CPanelBar(CTwoColorBallsView* pView)
	: CDialog(CPanelBar::IDD, pView), m_pViewDialog(NULL), m_pView(pView), m_pCalDialog(NULL)
{
}

CPanelBar::~CPanelBar()
{
	delete m_pViewDialog;
	m_pViewDialog = NULL;
}

void CPanelBar::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BOOL CPanelBar::OnInitDialog()
{
	CDialog::OnInitDialog();
	return TRUE;  // return TRUE  unless you set the focus to a control
}

BEGIN_MESSAGE_MAP(CPanelBar, CDialog)
	ON_BN_CLICKED(IDC_CMD_VIEW, &CPanelBar::OnBnClickedCmdView)
	ON_BN_CLICKED(IDC_CMD_ANALYSIS, &CPanelBar::OnBnClickedCmdAnalysis)
	ON_BN_CLICKED(IDC_CMD_CALCULATE, &CPanelBar::OnBnClickedCmdCalculate)
END_MESSAGE_MAP()

BEGIN_DHTML_EVENT_MAP(CPanelBar)
END_DHTML_EVENT_MAP()

// CPanelBar message handlers
void CPanelBar::OnBnClickedCmdView()
{
	CTwoColorBallsDoc* pDoc = Lucky::GetActiveDocument();
	if (pDoc != NULL && pDoc->IsInitialized())
	{
		if (m_pViewDialog == NULL)
		{
			m_pViewDialog = new CHistoryDialog(m_pView);
			m_pViewDialog->SetHistory(pDoc->GetHistoryData());
			m_pViewDialog->Create(CHistoryDialog::IDD, m_pView);
		}
		m_pViewDialog->ShowWindow(SW_SHOW);
	}
}

void CPanelBar::OnBnClickedCmdAnalysis()
{
	//CTwoColorBallsDoc* pDoc = Lucky::GetActiveDocument();
	//if (pDoc != NULL && pDoc->IsInitialized())
	//{
	//	if (m_pDialog == NULL)
	//	{
	//		m_pDialog = new CHistoryDialog(m_pView);
	//		m_pDialog->SetHistory(pDoc->GetHistoryData());
	//		m_pDialog->Create(CHistoryDialog::IDD, m_pView);
	//	}
	//	m_pDialog->ShowWindow(SW_SHOW);
	//}
}

void CPanelBar::OnBnClickedCmdCalculate()
{
	CTwoColorBallsDoc* pDoc = Lucky::GetActiveDocument();
	if (pDoc != NULL && pDoc->IsInitialized())
	{
		if (m_pCalDialog == NULL)
		{
			m_pCalDialog = new CCalculateDlg(m_pView);
			m_pCalDialog->SetHistory(pDoc->GetHistoryData());
			m_pCalDialog->Create(CCalculateDlg::IDD, m_pView);
		}
		m_pCalDialog->ShowWindow(SW_SHOW);
	}
}
