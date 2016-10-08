
// TwoColorBallsView.cpp : implementation of the CTwoColorBallsView class
//

#include "stdafx.h"
#include "TwoColorBalls.h"
#include "TwoColorBallsDoc.h"
#include "TwoColorBallsView.h"
#include "PanelBar.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CTwoColorBallsView

IMPLEMENT_DYNCREATE(CTwoColorBallsView, CView)

BEGIN_MESSAGE_MAP(CTwoColorBallsView, CView)
	// Standard printing commands
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CTwoColorBallsView::OnFilePrintPreview)
END_MESSAGE_MAP()

// CTwoColorBallsView construction/destruction

CTwoColorBallsView::CTwoColorBallsView()
: m_pPanelBar(NULL)
{
	// TODO: add construction code here
}

CTwoColorBallsView::~CTwoColorBallsView()
{
	delete m_pPanelBar;
	m_pPanelBar = NULL;
}

BOOL CTwoColorBallsView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	return CView::PreCreateWindow(cs);
}

// CTwoColorBallsView drawing

void CTwoColorBallsView::OnDraw(CDC* /*pDC*/)
{
	CTwoColorBallsDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: add draw code for native data here
	if (pDoc->IsInitialized() && m_pPanelBar == NULL)
	{
		m_pPanelBar = new CPanelBar(this);
		m_pPanelBar->Create(CPanelBar::IDD, this);
		m_pPanelBar->ShowWindow(SW_SHOW);
	}
}


// CTwoColorBallsView printing


void CTwoColorBallsView::OnFilePrintPreview()
{
	AFXPrintPreview(this);
}

BOOL CTwoColorBallsView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// default preparation
	return DoPreparePrinting(pInfo);
}

void CTwoColorBallsView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add extra initialization before printing
}

void CTwoColorBallsView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add cleanup after printing
}

void CTwoColorBallsView::OnRButtonUp(UINT nFlags, CPoint point)
{
	ClientToScreen(&point);
	OnContextMenu(this, point);
}

void CTwoColorBallsView::OnContextMenu(CWnd* pWnd, CPoint point)
{
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
}


// CTwoColorBallsView diagnostics

#ifdef _DEBUG
void CTwoColorBallsView::AssertValid() const
{
	CView::AssertValid();
}

void CTwoColorBallsView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CTwoColorBallsDoc* CTwoColorBallsView::GetDocument() const // non-debug version is inline
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CTwoColorBallsDoc)));
	return (CTwoColorBallsDoc*)m_pDocument;
}
#endif //_DEBUG


// CTwoColorBallsView message handlers
