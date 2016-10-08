
#pragma once
#include "afxcmn.h"
#include "afxwin.h"
#include "..\Data\Analysis\HistoryData.h"
#include "Controls\ScrollEdit.h"


// CAddNoteDlg dialog
class CAddNoteDlg : public CDialog
{
	DECLARE_DYNAMIC(CAddNoteDlg)

public:
	CAddNoteDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CAddNoteDlg();

	void AddNote(CString& str);

// Dialog Data
	enum { IDD = IDD_DIALOG_ADD_NOTE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

	virtual BOOL OnInitDialog();
	void OnSize(UINT nType, int cx, int cy);

private:
	CScrollEdit m_editOutput;
};
