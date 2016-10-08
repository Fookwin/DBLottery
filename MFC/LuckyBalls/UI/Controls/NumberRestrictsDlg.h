#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\..\..\LuckyBallsRes\resource.h"
#include "Data\Constraint\NumberSetConstraint.h"

// CProgressDialog dialog
class CNumRestrictsDlg : public CDialog
{
	DECLARE_DYNAMIC(CNumRestrictsDlg)

public:
	CNumRestrictsDlg(RedNumbersConstraint* pConst, CWnd* pParent = NULL);   // standard constructor
	virtual ~CNumRestrictsDlg();

// Dialog Data
	enum { IDD = IDD_DIALOG_RED };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	afx_msg void OnLvnEndlabeleditList1(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnNMDblclkList1(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnNMRClickList1(NMHDR *pNMHDR, LRESULT *pResult);

	virtual BOOL OnInitDialog();

private:
	CListCtrl				m_listRestricts;
	RedNumbersConstraint*	m_pConstraint;
public:
	afx_msg void OnLvnBeginlabeleditList1(NMHDR *pNMHDR, LRESULT *pResult);
};
