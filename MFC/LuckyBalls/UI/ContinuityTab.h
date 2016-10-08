
#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\Data\Analysis\HistoryData.h"


// CContinuityTab dialog
class CContinuityTab : public CDialog
{
	DECLARE_DYNAMIC(CContinuityTab)

public:
	CContinuityTab(CWnd* pParent = NULL);   // standard constructor
	virtual ~CContinuityTab();

// Dialog Data
	enum { IDD = IDD_TAB_CONTINUITY };

	void SetHistory(CHistoryData* pData) {m_pHistory = pData;};

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	virtual BOOL OnInitDialog();
private:
	void Update();

	CHistoryData* m_pHistory;
	CProgressCtrl m_Column0;
	CProgressCtrl m_Column1;
	CProgressCtrl m_Column2;
	CProgressCtrl m_Column3;
	CProgressCtrl m_Column4;
	CProgressCtrl m_Column5;
	CStatic m_State0;
	CStatic m_State1;
	CStatic m_State2;
	CStatic m_State3;
	CStatic m_State4;
	CStatic m_State5;
	CEdit m_editStartIssue;
	CEdit m_editEndIssue;
public:
	afx_msg void OnBnClickedButtonUpdate();
};
