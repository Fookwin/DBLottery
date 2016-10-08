// ScrollEdit.cpp : implementation file    
//    

#include "stdafx.h"     
#include "ScrollEdit.h"    

#ifdef _DEBUG    
#define new DEBUG_NEW    
#undef THIS_FILE    
static char THIS_FILE[] = __FILE__;   
#endif    

#define UWM_CHECKTEXT_MSG _T("UWM_CHECKTEXT_MSG-{D53C7065_FD4C_138A_BC72_01A0253651C0}")    

//Resize Message    
const UINT CScrollEdit::UWM_CHECKTEXT = ::RegisterWindowMessage(UWM_CHECKTEXT_MSG);   

/////////////////////////////////////////////////////////////////////////////    
// CScrollEdit    

CScrollEdit::CScrollEdit() : m_bShowHoriz(TRUE), m_bShowVert(TRUE)   
{   
}   

CScrollEdit::~CScrollEdit()   
{   
}   

void CScrollEdit::CheckScrolling(LPCTSTR lpszString)   
{   
	CRect oRect;   
	GetClientRect(&oRect);   
	CClientDC oDC(this);   
	int iHeight=0;   
	BOOL bHoriz = FALSE;   
	CFont* pEdtFont = GetFont();   
	if(pEdtFont != NULL)   
	{   
		//Determine Text Width and Height    
		SIZE oSize;   
		CFont* pOldFont = oDC.SelectObject(pEdtFont);   
		//Determine the line Height    
		oSize = oDC.GetTextExtent(CString(_T(" ")));   
		iHeight = oSize.cy;   
		//Text Width    
		int iWidth=0, i =0;   
		CString oStr;   
		//Parse the string, the lines in a multiline Edit are separated by "\r\n"    
		while(TRUE == ::AfxExtractSubString(oStr, lpszString, i, _T('\n')))   
		{   
			if(FALSE == bHoriz)   
			{   
				int iLen = oStr.GetLength()-1;   
				if(iLen >=0)   
				{   
					//Eliminate last '\r'    
					if(_T('\r') == oStr.GetAt(iLen))   
						oStr = oStr.Left(iLen);   
					oSize = oDC.GetTextExtent(oStr);   
					if(iWidth  < oSize.cx)   
						iWidth = oSize.cx;   
					if(iWidth >= oRect.Width())   
						bHoriz = TRUE;   
				}   
			}   
			i++;   
		}   
		oDC.SelectObject(pOldFont);   
		//Text Height    
		iHeight *= i;   
	}   
	ShowHorizScrollBar(bHoriz);   
	ShowVertScrollBar(iHeight >= oRect.Height());   
}   

//Overloaded Function    
void CScrollEdit::SetWindowText(LPCTSTR lpszString)   
{   
	CheckScrolling(lpszString);   
	CEdit::SetWindowText(lpszString);   
}   

void CScrollEdit::ReplaceSel(LPCTSTR lpszNewText, BOOL bCanUndo /* = FALSE */)
{
	CEdit::ReplaceSel(lpszNewText, bCanUndo);
	
	CString strText;
	GetWindowText(strText);
	CheckScrolling(strText);
}

BEGIN_MESSAGE_MAP(CScrollEdit, CEdit)   
	//{{AFX_MSG_MAP(CScrollEdit)    
	ON_WM_CHAR()   
	//}}AFX_MSG_MAP    
	ON_REGISTERED_MESSAGE(UWM_CHECKTEXT, OnCheckText)   
END_MESSAGE_MAP()   

/////////////////////////////////////////////////////////////////////////////    
// CScrollEdit message handlers    

BOOL CScrollEdit::PreCreateWindow(CREATESTRUCT& cs)    
{   
	//Add WS_HSCROLL, WS_VSCROLL, ES_MULTILINE, ES_AUTOHSCROLL, ES_AUTOVSCROLL    
	cs.style |= (WS_HSCROLL|WS_VSCROLL|ES_MULTILINE|ES_AUTOHSCROLL|ES_AUTOVSCROLL);   
	return CEdit::PreCreateWindow(cs);   
}   

void CScrollEdit::OnChar(UINT nChar, UINT nRepCnt, UINT nFlags)    
{   
	//Possible Text Change    
	PostMessage(UWM_CHECKTEXT);   
	CEdit::OnChar(nChar, nRepCnt, nFlags);   
}   

LRESULT CScrollEdit::OnCheckText(WPARAM wParam, LPARAM lParam)   
{   
	CString oStr;   
	GetWindowText(oStr);   
	CheckScrolling(oStr);   
	return 0;   
}   
