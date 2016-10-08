// AnalysisDialog.cpp : implementation file
//

#include "stdafx.h"
#include "..\LuckyBalls.h"
#include "NotesDlg.h"
#include "..\Data\Analysis\HistoryData.h"


// CAddNoteDlg dialog

IMPLEMENT_DYNAMIC(CAddNoteDlg, CDialog)

CAddNoteDlg::CAddNoteDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CAddNoteDlg::IDD, pParent)
{
}

CAddNoteDlg::~CAddNoteDlg()
{
}

void CAddNoteDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT_NOTE, m_editOutput);
}


BEGIN_MESSAGE_MAP(CAddNoteDlg, CDialog)
	ON_WM_SIZE()
END_MESSAGE_MAP()


// CAddNoteDlg message handlers
BOOL CAddNoteDlg::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	return TRUE;
}

void CAddNoteDlg::OnSize(UINT nType, int cx, int cy)
{
	CDialog::OnSize(nType, cx, cy);

	if (m_editOutput.GetSafeHwnd())
	{ 
		CRect rct;
		GetClientRect(rct);

		const int iMargin = -5;
		rct.InflateRect(iMargin, iMargin, iMargin, iMargin);
		m_editOutput.MoveWindow(rct);
	}
}

void CAddNoteDlg::AddNote(CString& str)
{
	// Add to tail.
	CString strText;
	m_editOutput.GetWindowText(strText);

	int nEnd = strText.GetLength();
	m_editOutput.SetSel(nEnd,nEnd);     //seek to end

	CString strTail = str + _T("\r\n");
	m_editOutput.ReplaceSel(strTail);

	// To end.
	int nline= m_editOutput.GetLineCount(); 
	m_editOutput.LineScroll(nline);
}
