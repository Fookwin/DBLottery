m_OutputDialog
// DockablePanelBase.cpp : implementation file
//

#include "stdafx.h"
#include "LuckyBalls.h"
#include "DockablePanelBase.h"


// CDockablePanelBase

IMPLEMENT_DYNAMIC(CDockablePanelBase, CDockablePane)

CDockablePanelBase::CDockablePanelBase()
: m_pContentDlg(NULL)
{

}

CDockablePanelBase::~CDockablePanelBase()
{
}


BEGIN_MESSAGE_MAP(CDockablePanelBase, CDockablePane)
	ON_WM_CREATE()
	ON_WM_DESTROY()
	ON_WM_SIZE()
END_MESSAGE_MAP()



// CDockablePanelBase message handlers


int CDockablePanelBase::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CDockablePane::OnCreate(lpCreateStruct) == -1)
		return -1;

	if (!OnInitContent())
		return -1;

	return 0;
}

void CDockablePanelBase::OnDestroy()
{
	CDockablePane::OnDestroy();

	OnDestroyContent();
}

void CDockablePanelBase::OnSize(UINT nType, int cx, int cy)
{
	CDockablePane::OnSize(nType, cx, cy);

	if(m_pContentDlg && m_pContentDlg->GetSafeHwnd())
	{ 
		CRect rct;
		GetClientRect(rct);
		m_pContentDlg->MoveWindow(rct);
	}
}

void CDockablePanelBase::ShowInPanel(CDialog* pDlg)
{
	if (m_pContentDlg != NULL)
	{
		m_pContentDlg->ShowWindow(SW_HIDE);
	}

	m_pContentDlg = pDlg;
	m_pContentDlg->ShowWindow(SW_SHOW);

	CRect rct;
	GetClientRect(rct);
	m_pContentDlg->MoveWindow(rct);

	Invalidate(TRUE);
}

/////////////////////////////////////////////////////////////////////////////////////////////////
bool COutputPanel::OnInitContent()
{
	m_OutputDialog.Create(CAddNoteDlg::IDD, this);
	m_OutputDialog.ShowWindow(SW_SHOW);

	m_pContentDlg = &m_OutputDialog;

	return true;
}

void COutputPanel::OnDestroyContent()
{
	m_OutputDialog.DestroyWindow();
}


