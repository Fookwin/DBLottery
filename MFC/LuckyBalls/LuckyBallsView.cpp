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

// LuckyBallsView.cpp : implementation of the CLuckyBallsView class
//

#include "stdafx.h"
#include "LuckyBalls.h"
#include "LuckyBallsDoc.h"
#include "LuckyBallsView.h"
#include "UI\HistoryDialog.h"
#include "UI\CalculateDlg.h"
#include "UI\AnalysisDlg.h"
#include "UI\NotesDlg.h"
#include "Server\Global.h"
#include "UI\Controls\LuckyNumGraphics.h"
#include "UI/DockablePanelBase.h"
#include "Server/Application.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CLuckyBallsView

IMPLEMENT_DYNCREATE(CLuckyBallsView, CView)

BEGIN_MESSAGE_MAP(CLuckyBallsView, CView)
	ON_COMMAND(ID_HISTORY_SHOWHISTORY, &CLuckyBallsView::OnHistoryShowhistory)
	ON_COMMAND(ID_ANALYSIS_ANALYZE, &CLuckyBallsView::OnAnalysisAnalyze)
	ON_COMMAND(ID_ANALYSIS_CALCULATE, &CLuckyBallsView::OnAnalysisCalculate)
	ON_COMMAND(ID_VIEW_ANALYZEHISTORY, &CLuckyBallsView::OnViewAnalyzeHistory)
	ON_COMMAND(ID_VIEW_ANALYZEHITCOUNT, &CLuckyBallsView::OnViewAnalyzeHitCount)
	ON_COMMAND(ID_VIEW_ANALYZESMALLNUM, &CLuckyBallsView::OnViewAnalyzeSmallNum)
	ON_COMMAND(ID_VIEW_ANALYZEEVENNUM, &CLuckyBallsView::OnViewAnalyzeEvenNum)
	ON_COMMAND(ID_NOTES_ADD, &CLuckyBallsView::OnAddNote)
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONDOWN()
	ON_WM_ERASEBKGND()
	ON_WM_MOUSEWHEEL()
END_MESSAGE_MAP()

// CLuckyBallsView construction/destruction

CLuckyBallsView::CLuckyBallsView()
	: m_pViewDialog(NULL), m_pCalDialog(NULL), m_pAnalysisDlg(NULL), m_pAddNoteDlg(NULL), m_strActiveViewName(VIEW_HISTORY_NAME)
{
	// Establish the view controllers.
	REGISTER_VIEW_CONTROLS(this, m_ViewCtrls);
}

CLuckyBallsView::~CLuckyBallsView()
{
	delete m_pViewDialog;
	m_pViewDialog = NULL;
	delete m_pCalDialog;
	m_pCalDialog = NULL;
	delete m_pAnalysisDlg;
	m_pAnalysisDlg = NULL;
	delete m_pAddNoteDlg;
	m_pAddNoteDlg = NULL;

	for (std::map<CString, CBaseViewCtrl*>::iterator it = m_ViewCtrls.begin(); it != m_ViewCtrls.end(); it ++)
		delete it->second;
	m_ViewCtrls.clear();
}

BOOL CLuckyBallsView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	return CView::PreCreateWindow(cs);
}

// CLuckyBallsView drawing

void CLuckyBallsView::OnDraw(CDC* pDC)
{
	CLuckyBallsDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// Get active control.
	CBaseViewCtrl* pViewCtrl = GetActiveGraphCtrl();
	if (pViewCtrl != NULL)
	{
		pViewCtrl->Draw(pDC);
	}
}

CBaseViewCtrl* CLuckyBallsView::GetActiveGraphCtrl() const
{
	std::map<CString, CBaseViewCtrl*>::const_iterator it = m_ViewCtrls.find(m_strActiveViewName);
	if (it == m_ViewCtrls.end()) return NULL;

	return it->second;
}

void CLuckyBallsView::OnRButtonUp(UINT nFlags, CPoint point)
{
	ClientToScreen(&point);
	OnContextMenu(this, point);
}

void CLuckyBallsView::OnLButtonDown(UINT nFlags, CPoint point)
{
	CBaseViewCtrl* pViewCtrl = GetActiveGraphCtrl();
	if (pViewCtrl != NULL)
	{
		pViewCtrl->OnLButtonDown(nFlags, point);
	}
}

void CLuckyBallsView::OnMouseMove(UINT nFlags, CPoint point)
{
	CBaseViewCtrl* pViewCtrl = GetActiveGraphCtrl();
	if (pViewCtrl != NULL)
	{
		pViewCtrl->OnMouseMove(nFlags, point);
	}
}

void CLuckyBallsView::OnContextMenu(CWnd* pWnd, CPoint point)
{
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
}

BOOL CLuckyBallsView::OnEraseBkgnd(CDC* pDC)
{
	return TRUE;
}

// CLuckyBallsView diagnostics

#ifdef _DEBUG
void CLuckyBallsView::AssertValid() const
{
	CView::AssertValid();
}

void CLuckyBallsView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CLuckyBallsDoc* CLuckyBallsView::GetDocument() const // non-debug version is inline
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CLuckyBallsDoc)));
	return (CLuckyBallsDoc*)m_pDocument;
}
#endif //_DEBUG


// CLuckyBallsView message handlers

void CLuckyBallsView::OnHistoryShowhistory()
{
	CLuckyBallsDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	if (pDoc != NULL)
	{
		if (m_pViewDialog == NULL)
		{
			m_pViewDialog = new CHistoryDialog(this);
			m_pViewDialog->SetHistory(Lucky::GetHistoryData());
			m_pViewDialog->Create(CHistoryDialog::IDD, this);
		}
		m_pViewDialog->ShowWindow(SW_SHOW);
	}
}

void CLuckyBallsView::OnAnalysisAnalyze()
{
	CLuckyBallsDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	if (pDoc != NULL)
	{
		if (m_pAnalysisDlg == NULL)
		{
			m_pAnalysisDlg = new CAnalysisDlg(this);
			m_pAnalysisDlg->SetHistory(Lucky::GetHistoryData());
			m_pAnalysisDlg->Create(CAnalysisDlg::IDD, this);
		}
		m_pAnalysisDlg->ShowWindow(SW_SHOW);
	}
}

void CLuckyBallsView::OnAnalysisCalculate()
{
	CLuckyBallsDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	if (pDoc != NULL)
	{
		if (m_pCalDialog == NULL)
		{
			m_pCalDialog = new CCalculateDlg(this);
			m_pCalDialog->SetHistory(Lucky::GetHistoryData());
			m_pCalDialog->Create(CCalculateDlg::IDD, this);
		}
		m_pCalDialog->ShowWindow(SW_SHOW);
	}
}

void CLuckyBallsView::OnViewAnalyzeHistory()
{
	if (m_strActiveViewName == VIEW_HISTORY_NAME)
		return;

	m_strActiveViewName = VIEW_HISTORY_NAME;
	InvalidateRect(NULL, 1);
}

void CLuckyBallsView::OnViewAnalyzeHitCount()
{
	if (m_strActiveViewName == VIEW_HITCOUNT_NAME)
		return;

	m_strActiveViewName = VIEW_HITCOUNT_NAME;
	InvalidateRect(NULL, 1);
}

void CLuckyBallsView::OnViewAnalyzeSmallNum()
{
	if (m_strActiveViewName == VIEW_SMALLODDS_NAME)
		return;

	m_strActiveViewName = VIEW_SMALLODDS_NAME;
	InvalidateRect(NULL, 1);
}

void CLuckyBallsView::OnViewAnalyzeEvenNum()
{
	if (m_strActiveViewName == VIEW_EVENCOUNT_NAME)
		return;

	m_strActiveViewName = VIEW_EVENCOUNT_NAME;
	InvalidateRect(NULL, 1);
}

void CLuckyBallsView::OnAddNote()
{
	CLuckyBallsDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	if (pDoc != NULL)
	{
		CDockablePanelBase* pToolPanel = CApplication::GetApplication()->GetToolPanel();
		ASSERT(pToolPanel);

		if (pToolPanel)
		{
			if (m_pAddNoteDlg == NULL)
			{
				m_pAddNoteDlg = new CAddNoteDlg(pToolPanel);
				m_pAddNoteDlg->Create(CAddNoteDlg::IDD, pToolPanel);
			}
			pToolPanel->ShowInPanel(m_pAddNoteDlg);
		}
	}
}

BOOL CLuckyBallsView::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	CBaseViewCtrl* pViewCtrl = GetActiveGraphCtrl();
	if (pViewCtrl != NULL)
	{
		return pViewCtrl->OnMouseWheel(nFlags, zDelta, pt);
	}

	return TRUE;
}