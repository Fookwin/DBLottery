#pragma once

class CHistoryData;
class CLuckyBallsDoc;
class CProgressDialog;
class CDockablePanelBase;
class CAddNoteDlg;
class CApplication
{
public:
	CApplication();
	~CApplication();

	void SetActiveDocument(CLuckyBallsDoc* pDoc) { m_pSingleDocument = pDoc;}
	CLuckyBallsDoc* GetActiveDocument() const { return m_pSingleDocument; }
	CHistoryData* GetHistoryData() const;
	CProgressDialog* GetProgressDlg();

	void SetMainFrame(CWnd* pFrame) { m_pMainFrame = pFrame;}
	CWnd* GetMainFramWnd() const { return m_pMainFrame;}

	void SetOutputDlg(CAddNoteDlg* pDlg) { m_pOutputDlg = pDlg;}
	CAddNoteDlg* GetOutputDlg() const { return m_pOutputDlg;}

	void SetToolPanel(CDockablePanelBase* pDlg) { m_pToolPanel = pDlg;}
	CDockablePanelBase* GetToolPanel() const { return m_pToolPanel;}

	static CApplication* GetApplication();

private:
	CLuckyBallsDoc*		m_pSingleDocument;
	CProgressDialog*	m_pProgressDlg;
	CWnd*				m_pMainFrame;
	CHistoryData*		m_pHistoryData;
	CDockablePanelBase* m_pToolPanel;
	CAddNoteDlg*		m_pOutputDlg;
};