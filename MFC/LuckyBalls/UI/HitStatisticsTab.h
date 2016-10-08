
#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\Data\Analysis\HistoryData.h"


// CHitStatisticsTab dialog
class CHitStatisticsTab : public CDialog
{
	DECLARE_DYNAMIC(CHitStatisticsTab)

public:
	CHitStatisticsTab(CWnd* pParent = NULL);   // standard constructor
	virtual ~CHitStatisticsTab();

// Dialog Data
	enum { IDD = IDD_TAB_HIT_STATISTICS };

	void SetHistory(CHistoryData* pData) {m_pHistory = pData;};
	afx_msg void OnBnClickedButtonUpdate();
	afx_msg void OnBnClickedRadio1();
	afx_msg void OnBnClickedRadio2();
	afx_msg void OnBnClickedRadio3();
	afx_msg void OnBnClickedRadio4();
	afx_msg void OnBnClickedRadio5();
	afx_msg void OnBnClickedRadio6();

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL PreTranslateMessage(MSG* pMsg);

	DECLARE_MESSAGE_MAP()

	virtual BOOL OnInitDialog();
private:
	void Update();
	void SetCheck(int iPos);
	void UpdateColumns();
	void Clear();

	CHistoryData* m_pHistory;
	CProgressCtrl m_Column1;
	CProgressCtrl m_Column2;
	CProgressCtrl m_Column3;
	CProgressCtrl m_Column4;
	CProgressCtrl m_Column5;
	CProgressCtrl m_Column6;
	CProgressCtrl m_Column7;
	CProgressCtrl m_Column8;
	CProgressCtrl m_Column9;
	CProgressCtrl m_Column10;
	CProgressCtrl m_Column11;
	CProgressCtrl m_Column12;
	CProgressCtrl m_Column13;
	CProgressCtrl m_Column14;
	CProgressCtrl m_Column15;
	CProgressCtrl m_Column16;
	CProgressCtrl m_Column17;
	CProgressCtrl m_Column18;
	CProgressCtrl m_Column19;
	CProgressCtrl m_Column20;
	CProgressCtrl m_Column21;
	CProgressCtrl m_Column22;
	CProgressCtrl m_Column23;
	CProgressCtrl m_Column24;
	CProgressCtrl m_Column25;
	CProgressCtrl m_Column26;
	CProgressCtrl m_Column27;
	CProgressCtrl m_Column28;
	CProgressCtrl m_Column29;
	CProgressCtrl m_Column30;
	CProgressCtrl m_Column31;
	CProgressCtrl m_Column32;
	CProgressCtrl m_Column33;
	CButton m_radioPos1;
	CButton m_radioPos2;
	CButton m_radioPos3;
	CButton m_radioPos4;
	CButton m_radioPos5;
	CButton m_radioPos6;

	CEdit m_editStartIssue;
	CEdit m_editEndIssue;

	CToolTipCtrl m_ToolTipCtrl;

	int m_iActivateBtn;

	int m_HitMatrix[6][33];
	int m_iMaxHitCount;
};
