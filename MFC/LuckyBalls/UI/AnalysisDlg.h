
#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\Data\Analysis\HistoryData.h"


// CAnalysisDlg dialog
class CAnalysisDlg : public CDialog
{
	DECLARE_DYNAMIC(CAnalysisDlg)

public:
	CAnalysisDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CAnalysisDlg();

// Dialog Data
	enum { IDD = IDD_DIALOG_ANALYSIS };

	void SetHistory(CHistoryData* pData) {m_pHistory = pData;};

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
	afx_msg void OnTcnSelchangeTabAnalysis(NMHDR *pNMHDR, LRESULT *pResult);

	virtual BOOL OnInitDialog();
private:

	void ActivateTab(int iTab);

	CHistoryData* m_pHistory;
	CTabCtrl m_TabCtrl;

	std::vector<CDialog*> m_Tabs;
};
