#pragma once

class CApplication;
class CLuckyBallsDoc;
class CHistoryData;

namespace Lucky
{
	CApplication* GetApplication();
	CLuckyBallsDoc* GetActiveDocument();
	CHistoryData*	GetHistoryData();
	int GetTestIssue();
	void SetTestIssue(int issue = -1);

	// Progress dialog...
	bool InitProgress(const CString& strPrg = _T(""), const CString& strTask = _T(""), int iPos = 0);
	bool ShowProgress(bool bShow);
	bool SetProgress(const CString& strPrg);
	bool SetTask(const CString& strTask);
	bool SetPos(int iPos);

	// output dialog...
	bool AddNote(CString& strMsg);
};