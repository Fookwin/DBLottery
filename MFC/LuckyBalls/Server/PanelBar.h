#pragma once

// CPanelBar dialog
class CHistoryDialog;
class CCalculateDlg;
class CTwoColorBallsView;
class CPanelBar : public CDialog
{
	DECLARE_DYNCREATE(CPanelBar)

public:
	CPanelBar(CTwoColorBallsView* pView = NULL);   // standard constructor
	virtual ~CPanelBar();
// Overrides

// Dialog Data
	enum { IDD = IDD_PANEL };

	afx_msg void OnBnClickedCmdView();

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();

	DECLARE_MESSAGE_MAP()
	DECLARE_DHTML_EVENT_MAP()

private:
	CHistoryDialog* m_pViewDialog;
	CCalculateDlg*  m_pCalDialog;
	CTwoColorBallsView* m_pView;
public:
	afx_msg void OnBnClickedCmdAnalysis();
	afx_msg void OnBnClickedCmdCalculate();
};
