#include "stdafx.h"
#include "Application.h"
#include "Data\Analysis\HistoryData.h"
#include "..\UI\Controls\ProgressDlg.h"

CApplication s_Application;

CApplication::CApplication()
: m_pProgressDlg(NULL), m_pSingleDocument(NULL), m_pMainFrame(NULL), m_pHistoryData(NULL), m_pToolPanel(NULL), m_pOutputDlg(NULL)
{
	m_pHistoryData = new CHistoryData();
}

CApplication::~CApplication()
{
	delete m_pProgressDlg;
	m_pProgressDlg = NULL;

	delete m_pHistoryData;
	m_pHistoryData = NULL;
}

CApplication* CApplication::GetApplication()
{
	return &s_Application;
}

CHistoryData* CApplication::GetHistoryData() const 
{ 
	if (!m_pHistoryData->IsInitialized())
	{
		// Initialize the history data...
		bool bRes = m_pHistoryData->Init();
		ASSERT(bRes);
	}
	return m_pHistoryData; 
}

CProgressDialog* CApplication::GetProgressDlg()
{
	if (m_pProgressDlg == NULL)
	{
		m_pProgressDlg = new CProgressDialog(m_pMainFrame);
		m_pProgressDlg->Create(CProgressDialog::IDD, m_pMainFrame);
		CRect rect;
		m_pMainFrame->GetWindowRect(rect);

		CRect rc;
		m_pProgressDlg->GetWindowRect(rc);

		int ix = (rect.right - rect.left - rc.Width()) / 2;
		int iy = (rect.bottom - rect.top - rc.Height()) / 2;
		m_pProgressDlg->MoveWindow(ix, iy, rc.Width(), rc.Height());
	}
	return m_pProgressDlg;
}