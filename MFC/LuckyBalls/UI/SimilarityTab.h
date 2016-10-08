
#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\Data\Analysis\HistoryData.h"

///////////////////////////////////////////////////////////////////////////////////////////////////
class CSimilarity
{
public:
	CSimilarity() : m_iMin(6), m_iMax(0)
	{
		for (int i = 0; i < 7; i ++)
			m_HitCounts[i] = 0;
	}

	CSimilarity& operator = (const CSimilarity& other)
	{
		m_iMin = other.m_iMin;
		m_iMax = other.m_iMax;

		for (int i = 0; i < 7; i ++)
			m_HitCounts[i] = other.m_HitCounts[i];

		return *this;
	}

	int		m_iMin;
	int		m_iMax;
	int		m_HitCounts[7];
};

// CSimilarityTab dialog
class CSimilarityTab : public CDialog
{
	DECLARE_DYNAMIC(CSimilarityTab)

public:
	CSimilarityTab(CWnd* pParent = NULL);   // standard constructor
	virtual ~CSimilarityTab();

// Dialog Data
	enum { IDD = IDD_TAB_SIMILARITY };

	void SetHistory(CHistoryData* pData) {m_pHistory = pData;};

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	virtual BOOL OnInitDialog();
	afx_msg void OnBnClickedButtonUpdate();
	afx_msg void OnEnChangeEditNum();
	afx_msg void OnCbnSelchangeComboIssue();

	void CalSimilarity(const CLuckyNum& test, const LuckyNums& history, CSimilarity& st,
		int iStartIssue = -1, int iEndIssue = -1, int iExceptIssue = -1);
private:
	class CNumStatus 
	{
	public:
		CNumStatus();
		~CNumStatus() {};

		CLuckyNum m_Num;
		int m_Status[6];
	};
	typedef std::vector<CNumStatus*> NumStatusVec;

	void InitNumStatus(NumStatusVec& result, int iTestCount = 6);
	void Update();
	void Update2();
	bool ReadRecord(CString strFile, int iTestCount, NumStatusVec& result, int& lastIssue);
	bool SaveRecord(CString strFile, int iTestCount, NumStatusVec& result, int lastIssue);

	CHistoryData* m_pHistory;
	CProgressCtrl m_Column0;
	CProgressCtrl m_Column1;
	CProgressCtrl m_Column2;
	CProgressCtrl m_Column3;
	CProgressCtrl m_Column4;
	CProgressCtrl m_Column5;
	CProgressCtrl m_Column6;
	CStatic m_State0;
	CStatic m_State1;
	CStatic m_State2;
	CStatic m_State3;
	CStatic m_State4;
	CStatic m_State5;
	CStatic m_State6;
	CEdit m_editStartIssue;
	CEdit m_editEndIssue;
	CComboBox m_comboxTestIssue;
public:
	afx_msg void OnBnClickedButtonUpdate2();
	CButton m_checkSpecifyFile;
};
