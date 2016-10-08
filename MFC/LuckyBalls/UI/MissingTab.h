
#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\Data\Analysis\HistoryData.h"


// CMissingTab dialog
class CMissingTab : public CDialog
{
	DECLARE_DYNAMIC(CMissingTab)

public:
	CMissingTab(CWnd* pParent = NULL);   // standard constructor
	virtual ~CMissingTab();

// Dialog Data
	enum { IDD = IDD_TAB_MISSING };

	void SetHistory(CHistoryData* pData) {m_pHistory = pData;};

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	afx_msg void OnBnClickedButtonCalculate();

	DECLARE_MESSAGE_MAP()

	virtual BOOL OnInitDialog();
private:
	void Update();

	CHistoryData* m_pHistory;
	CEdit m_editIssueStep;
	CListCtrl m_ResultList;
};
