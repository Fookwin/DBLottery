
#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\Data\Analysis\HistoryData.h"


// CHistoryDialog dialog
class CHistoryDialog : public CDialog
{
	DECLARE_DYNAMIC(CHistoryDialog)

public:
	CHistoryDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CHistoryDialog();

// Dialog Data
	enum { IDD = IDD_DIALOG_HISTORY };

	void SetHistory(CHistoryData* pData) {m_pHistory = pData;};

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	virtual BOOL OnInitDialog();

	void Init(int& iScore, CString& strReport);
private:
	CHistoryData* m_pHistory;
	CListCtrl m_HistoryList;
	CStatic m_staticReport;
};
