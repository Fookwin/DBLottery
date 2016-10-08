#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\Data\Analysis\HistoryData.h"
#include "..\Data\Constraint\Constraint.h"
#include "..\..\LuckyBallsRes\resource.h"
#include "..\Utilities\AnalysisUtil.h"

class CNumSetEdit : public CEdit
{
	DECLARE_DYNAMIC(CNumSetEdit)
public:
	CNumSetEdit(const CRagion& ragion = CRagion(1, 32), int iDispSpace = 1);
	virtual ~CNumSetEdit();

	CNumSet GetNumSet() const;
protected:
	DECLARE_MESSAGE_MAP()

	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);

private:
	CRagion m_numRagion;
	int		m_iSpace;
};

// CCalculateDlg dialog
class CCalculateDlg : public CDialog
{
	DECLARE_DYNAMIC(CCalculateDlg)

public:
	CCalculateDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CCalculateDlg();

	void SetHistory(CHistoryData* pData) {m_pHistory = pData;};

// Dialog Data
	enum { IDD = IDD_DIALOG_SUGGEST };

	afx_msg void OnBnClickedButtonSaveResult();
	afx_msg void OnBnClickedButtonLoadResult();
	afx_msg void OnBnClickedButtonSuggest();
	afx_msg void OnLvnGetdispinfoListResult(NMHDR *pNMHDR, LRESULT *pResult);

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	virtual BOOL OnInitDialog();

	bool GetConstraints(Constraints& condition);
	void SetConditionCtrls(const Constraints& condition);
private:
	CHistoryData*m_pHistory;
	CAnalysisUtil::LuckyNumSet m_CalResult;

	// Constraints...
	CNumSetEdit m_editRed1;
	CNumSetEdit m_editRed2;
	CNumSetEdit m_editRed3;
	CNumSetEdit m_editRed4;
	CNumSetEdit m_editRed5;
	CNumSetEdit m_editRed6;

	CNumSetEdit m_editStep12;
	CNumSetEdit m_editStep23;
	CNumSetEdit m_editStep34;
	CNumSetEdit m_editStep45;
	CNumSetEdit m_editStep56;

	CNumSetEdit m_editContinuity;
	CEdit		m_editRedRestricts;
	CNumSetEdit m_editEven;
	CNumSetEdit m_editRepeat;
	CNumSetEdit m_editSum;
	CNumSetEdit m_editPrime;
	CNumSetEdit m_editSmall;
	CNumSetEdit m_editMissing;

	CListCtrl m_ResultList;
	CButton m_btnFindInResult;

	CButton m_btnCheckRed;
	CButton m_btnCheckRedGeneral;
	CButton m_btnCheckRepeat;
	CButton m_btnCheckContinuity;
	CButton m_btnCheckEvenCount;
	CButton m_btnCheckSum;
	CButton m_btnCheckRedStep;
	CButton m_btnCheckPrime;
	CButton m_btnCheckSmall;
	CButton m_btnCheckMissing;
	
	CButton m_btnRadio645;
	CButton m_btnRadio644;
	CEdit   m_editRedSelection;
	CComboBox m_TestRange;
	CButton m_checkEvaluateResult;
public:
	afx_msg void OnBnClickedButtonAsDefault();
	afx_msg void OnBnClickedButtonImport();
	afx_msg void OnBnClickedButtonExport();
	afx_msg void OnBnClickedButtonEditRed();
	afx_msg void OnBnClickedCheckMatrix();
	afx_msg void OnBnClickedRadio645();
	afx_msg void OnBnClickedRadio545();
	afx_msg void OnBnClickedButtonMatrixFilter();
	afx_msg void OnBnClickedButtonStratgiesTest();
	afx_msg void OnCbnSelchangeComboTestIssues();
};
