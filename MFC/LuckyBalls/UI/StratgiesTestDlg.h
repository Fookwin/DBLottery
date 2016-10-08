#pragma once
#include "afxwin.h"
#include "afxcmn.h"
#include "..\Data\Analysis\StratgiesTest.h"

// StratgiesTestDlg dialog

class CStratgiesTestDlg : public CDialog
{
	DECLARE_DYNAMIC(CStratgiesTestDlg)

public:
	CStratgiesTestDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CStratgiesTestDlg();

	TestResults& GetResult() { return m_Details;}

// Dialog Data
	enum { IDD = IDD_STRATGIES_TEXT };

	afx_msg void OnBnClickedOk();
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();

	void UpdateControls();

	DECLARE_MESSAGE_MAP()

	CStatic m_Score;
	CListCtrl m_DetailResult;

	TestResults m_Details;
};
