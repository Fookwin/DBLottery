
// TwoColorBallsView.h : interface of the CTwoColorBallsView class
//


#pragma once

class CPanelBar;
class CTwoColorBallsView : public CView
{
protected: // create from serialization only
	CTwoColorBallsView();
	DECLARE_DYNCREATE(CTwoColorBallsView)

// Attributes
public:
	CTwoColorBallsDoc* GetDocument() const;

// Operations
public:

// Overrides
public:
	virtual void OnDraw(CDC* pDC);  // overridden to draw this view
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// Implementation
public:
	virtual ~CTwoColorBallsView();
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
	DECLARE_MESSAGE_MAP()

private:
	CPanelBar* m_pPanelBar;
};

#ifndef _DEBUG  // debug version in TwoColorBallsView.cpp
inline CTwoColorBallsDoc* CTwoColorBallsView::GetDocument() const
   { return reinterpret_cast<CTwoColorBallsDoc*>(m_pDocument); }
#endif

