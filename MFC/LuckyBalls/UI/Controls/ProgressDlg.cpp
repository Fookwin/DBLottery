// HistoryDialog.cpp : implementation file
//

#include "stdafx.h"
#include "ProgressDlg.h"

// CProgressDialog dialog
IMPLEMENT_DYNAMIC(CProgressDialog, CDialog)

CProgressDialog::CProgressDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CProgressDialog::IDD, pParent)
{
}

CProgressDialog::~CProgressDialog()
{
}

void CProgressDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_STATIC_PROCESS, m_staticProgress);
	DDX_Control(pDX, IDC_STATIC_TASK, m_staticTask);
	DDX_Control(pDX, IDC_PROGRESS, m_ProgressCtrl);
}

BEGIN_MESSAGE_MAP(CProgressDialog, CDialog)
END_MESSAGE_MAP()

// CProgressDialog message handlers
BOOL CProgressDialog::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	m_ProgressCtrl.SetRange(0, 1000);
	m_ProgressCtrl.SetStep(1);
	m_ProgressCtrl.SetPos(0);

	return TRUE;
}

void CProgressDialog::SetProgress(const CString& strPrg)
{
	if (::IsWindow(this->GetSafeHwnd()))
	{
		m_staticProgress.SetWindowText(strPrg);
		Invalidate();
	}
}

void CProgressDialog::SetTask(const CString& strTask)
{
	if (::IsWindow(this->GetSafeHwnd()))
	{
		m_staticTask.SetWindowText(strTask);
		Invalidate();
	}
}

void CProgressDialog::SetPos(int iPos)
{
	if (::IsWindow(this->GetSafeHwnd()))
	{
		m_ProgressCtrl.SetPos(iPos);
		Invalidate();
	}
}
