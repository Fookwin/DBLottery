#pragma once
#include "NotesDlg.h"

// CDockablePanelBase

class CDockablePanelBase : public CDockablePane
{
	DECLARE_DYNAMIC(CDockablePanelBase)

public:
	CDockablePanelBase();
	virtual ~CDockablePanelBase();

	int OnCreate(LPCREATESTRUCT lpCreateStruct);
	void OnDestroy();
	void OnSize(UINT nType, int cx, int cy);

	void ShowInPanel(CDialog* pDlg);

	virtual bool OnInitContent() {return true;};
	virtual void OnDestroyContent() {};

protected:
	DECLARE_MESSAGE_MAP()

	CDialog* m_pContentDlg;
};

class COutputPanel : public CDockablePanelBase
{
public:
	COutputPanel() {};
	~COutputPanel() {};

	virtual bool OnInitContent();
	virtual void OnDestroyContent();

	CAddNoteDlg* GetOutputDialog() {return &m_OutputDialog; } 
private:

	CAddNoteDlg m_OutputDialog;
};


