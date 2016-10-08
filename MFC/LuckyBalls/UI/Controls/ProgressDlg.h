
#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\..\..\LuckyBallsRes\resource.h"

// CProgressDialog dialog
class CProgressDialog : public CDialog
{
	DECLARE_DYNAMIC(CProgressDialog)

public:
	CProgressDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CProgressDialog();

// Dialog Data
	enum { IDD = IDD_DIALOG_PROGRESS };

	void SetProgress(const CString& strPrg);
	void SetTask(const CString& strTask);
	void SetPos(int iPos);

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	virtual BOOL OnInitDialog();
private:
	CStatic m_staticProgress;
	CStatic m_staticTask;
	CProgressCtrl m_ProgressCtrl;
};
