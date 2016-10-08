#pragma once
#include "..\..\Data\LuckyNumber.h"
#include "afxwin.h"

// CSelectNumDlg dialog

class CSelectNumDlg : public CDialog
{
	DECLARE_DYNAMIC(CSelectNumDlg)

public:
	CSelectNumDlg(CNumSet& init, CRagion ragion = CRagion(0, 32),
		int iUnitRagion = 1, const CString& strCapture = _T("Select Numbers"), CWnd* pParent = NULL);   // standard constructor
	virtual ~CSelectNumDlg();

// Dialog Data
	enum { IDD = IDD_DIALOG_NUM_SELECT };

	void GetNumSet(CNumSet& num) { num = m_numSet; }
	bool SetNumSet(const CNumSet& numSet);

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	virtual void OnOK();

	virtual BOOL OnInitDialog();

	bool UpdateButtonState(CButton* pBtn,
						   int iIndex,
						   int& iMin,
						   int iColCount,
						   int iBtnWidth);

	bool UpdateNumSet();

private:
	CNumSet m_numSet;
	int		m_iUnitRagion;
	CRagion m_Ragion;
	CPoint	m_Position;

	std::vector<CButton*> m_Buttons;

	CString m_strTitle;
	CEdit m_EditText;
public:
	afx_msg void OnEnChangeEditText();
	afx_msg void OnBnClickedCheck();
};
