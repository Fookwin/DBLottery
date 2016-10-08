#include "stdafx.h"
#include "Global.h"
#include "Application.h"
#include "..\LuckyBallsDoc.h"
#include "..\UI\Controls\ProgressDlg.h"
#include "..\UI\NotesDlg.h"

static int s_TestIssue = -1;

int Lucky::GetTestIssue()
{
	return s_TestIssue;
}

void Lucky::SetTestIssue(int issue /*= -1*/)
{
	s_TestIssue = issue;
}

CApplication* Lucky::GetApplication()
{
	return CApplication::GetApplication();
}

CLuckyBallsDoc* Lucky::GetActiveDocument()
{
	return GetApplication()->GetActiveDocument();
}

CHistoryData* Lucky::GetHistoryData()
{
	return GetApplication()->GetHistoryData();
}

bool Lucky::InitProgress(const CString& strPrg, const CString& strTask, int iPos)
{
	CProgressDialog* pPrg = GetApplication()->GetProgressDlg();
	if (pPrg == NULL) return false;

	pPrg->SetProgress(strPrg);
	pPrg->SetTask(strTask);
	pPrg->SetPos(50);
	return true;
}

bool Lucky::ShowProgress(bool bShow)
{
	CProgressDialog* pPrg = GetApplication()->GetProgressDlg();
	if (pPrg == NULL) return false;

	pPrg->ShowWindow(bShow ? SW_SHOW : SW_HIDE);
	return true;
}

bool Lucky::SetProgress(const CString& strPrg)
{
	CProgressDialog* pPrg = GetApplication()->GetProgressDlg();
	if (pPrg == NULL) return false;

	pPrg->SetProgress(strPrg);
	return true;
}

bool Lucky::SetTask(const CString& strTask)
{
	CProgressDialog* pPrg = GetApplication()->GetProgressDlg();
	if (pPrg == NULL) return false;

	pPrg->SetTask(strTask);
	return true;
}

bool Lucky::SetPos(int iPos)
{
	CProgressDialog* pPrg = GetApplication()->GetProgressDlg();
	if (pPrg == NULL) return false;

	pPrg->SetPos(iPos);
	return true;
}

bool Lucky::AddNote(CString& strMsg)
{
	CAddNoteDlg* pOutputDlg = CApplication::GetApplication()->GetOutputDlg();
	if (pOutputDlg != NULL)
		pOutputDlg->AddNote(strMsg);

	return true;
}