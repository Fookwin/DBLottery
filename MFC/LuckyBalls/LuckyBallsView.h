// This MFC Samples source code demonstrates using MFC Microsoft Office Fluent User Interface 
// (the "Fluent UI") and is provided only as referential material to supplement the 
// Microsoft Foundation Classes Reference and related electronic documentation 
// included with the MFC C++ library software.  
// License terms to copy, use or distribute the Fluent UI are available separately.  
// To learn more about our Fluent UI licensing program, please visit 
// http://msdn.microsoft.com/officeui.
//
// Copyright (C) Microsoft Corporation
// All rights reserved.

// LuckyBallsView.h : interface of the CLuckyBallsView class
//

#pragma once
#include <map>

class CHistoryDialog;
class CCalculateDlg;
class CAnalysisDlg;
class CLuckyBallsDoc;
class CBaseViewCtrl;
class CAddNoteDlg;
class CMainFrame;
class CLuckyBallsView : public CView
{
protected: // create from serialization only
	CLuckyBallsView();
	DECLARE_DYNCREATE(CLuckyBallsView)

// Attributes
public:
	CLuckyBallsDoc* GetDocument() const;

// Operations
public:

// Overrides
public:
	virtual void OnDraw(CDC* pDC);  // overridden to draw this view
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:

// Implementation
public:
	virtual ~CLuckyBallsView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	afx_msg void OnFilePrintPreview();
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
	DECLARE_MESSAGE_MAP()

	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnHistoryShowhistory();
	afx_msg void OnAnalysisAnalyze();
	afx_msg void OnAnalysisCalculate();
	afx_msg void OnViewAnalyzeHistory();
	afx_msg void OnViewAnalyzeHitCount();
	afx_msg void OnViewAnalyzeSmallNum();
	afx_msg void OnViewAnalyzeEvenNum();
	afx_msg void OnAddNote();
private:

	CBaseViewCtrl* GetActiveGraphCtrl() const;

	CHistoryDialog* m_pViewDialog;
	CCalculateDlg*  m_pCalDialog;
	CAnalysisDlg*   m_pAnalysisDlg;
	CAddNoteDlg*  m_pAddNoteDlg;

	std::map<CString, CBaseViewCtrl*> m_ViewCtrls;

	CString			m_strActiveViewName;
	CMainFrame*		m_pMainFrame;
};

#ifndef _DEBUG  // debug version in LuckyBallsView.cpp
inline CLuckyBallsDoc* CLuckyBallsView::GetDocument() const
   { return reinterpret_cast<CLuckyBallsDoc*>(m_pDocument); }
#endif

