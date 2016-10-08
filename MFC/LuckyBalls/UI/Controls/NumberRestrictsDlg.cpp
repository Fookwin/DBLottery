// NumRestrictsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "NumberRestrictsDlg.h"

// CNumRestrictsDlg dialog
IMPLEMENT_DYNAMIC(CNumRestrictsDlg, CDialog)

CNumRestrictsDlg::CNumRestrictsDlg(RedNumbersConstraint* pConst, CWnd* pParent /*=NULL*/)
	: CDialog(CNumRestrictsDlg::IDD, pParent), m_pConstraint(pConst)
{
}

CNumRestrictsDlg::~CNumRestrictsDlg()
{
}

void CNumRestrictsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST1, m_listRestricts);
}

BEGIN_MESSAGE_MAP(CNumRestrictsDlg, CDialog)
	ON_NOTIFY(LVN_ENDLABELEDIT, IDC_LIST1, &CNumRestrictsDlg::OnLvnEndlabeleditList1)
	ON_NOTIFY(NM_DBLCLK, IDC_LIST1, &CNumRestrictsDlg::OnNMDblclkList1)
	ON_NOTIFY(NM_RCLICK, IDC_LIST1, &CNumRestrictsDlg::OnNMRClickList1)
	ON_NOTIFY(LVN_BEGINLABELEDIT, IDC_LIST1, &CNumRestrictsDlg::OnLvnBeginlabeleditList1)
END_MESSAGE_MAP()

// CProgressDialog message handlers
BOOL CNumRestrictsDlg::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	DWORD dwStyle2 = m_listRestricts.GetExtendedStyle();
	dwStyle2 |= LVS_EX_FULLROWSELECT;
	dwStyle2 |= LVS_EX_GRIDLINES;
	m_listRestricts.SetExtendedStyle(dwStyle2);

	m_listRestricts.InsertColumn(0, _T(""), LVCFMT_CENTER, 20);
	m_listRestricts.InsertColumn(1, _T("Select Count"), LVCFMT_CENTER, 60);
	m_listRestricts.InsertColumn(2, _T("Number Set"), LVCFMT_CENTER, 100);

	// Initialize the grid.
	if (m_pConstraint != NULL)
	{
		int iCount = m_pConstraint->GetRestrictCount();
		for (int i = 0; i < iCount; i ++)
		{
			CNumSet set;
			CRagion selCount;
			m_pConstraint->GetRestrict(i, set, selCount);

			m_listRestricts.InsertItem(i, _T(""));
			m_listRestricts.SetItemText(i, 1, selCount.GetText());
			m_listRestricts.SetItemText(i, 2, set.GetText());
		}
	}

	return TRUE;
}

void CNumRestrictsDlg::OnLvnEndlabeleditList1(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO *pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	int iItem = pDispInfo->item.iItem;
	int iSub = pDispInfo->item.iSubItem;

	m_listRestricts.SetItemText(iItem, iSub, pDispInfo->item.pszText);

	*pResult = 0;
}

void CNumRestrictsDlg::OnNMDblclkList1(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);
	// TODO: Add your control notification handler code here
	*pResult = 0;
}

void CNumRestrictsDlg::OnNMRClickList1(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);
	// TODO: Add your control notification handler code here
	*pResult = 0;
}

void CNumRestrictsDlg::OnLvnBeginlabeleditList1(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLVDISPINFO *pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	CNumSet set;
	CRagion sel;
	m_pConstraint->GetRestrict(pDispInfo->item.iItem, set, sel);

	pDispInfo->item.iSubItem = 2;
	//pDispInfo->item.pszText = sel.GetText() + _T("|") + set.GetText();
	
	*pResult = 0;
}
