
// TwoColorBallsDoc.cpp : implementation of the CTwoColorBallsDoc class
//

#include "stdafx.h"
#include "Frame\TwoColorBalls.h"
#include "HistoryData.h"
#include "TwoColorBallsDoc.h"
#include "Server\Global.h"
#include "Server\Application.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CTwoColorBallsDoc

IMPLEMENT_DYNCREATE(CTwoColorBallsDoc, CDocument)

BEGIN_MESSAGE_MAP(CTwoColorBallsDoc, CDocument)
END_MESSAGE_MAP()


// CTwoColorBallsDoc construction/destruction

CTwoColorBallsDoc::CTwoColorBallsDoc()
: m_bInitialized(false)
{
	// TODO: add one-time construction code here

}

CTwoColorBallsDoc::~CTwoColorBallsDoc()
{
	delete m_pData;
	m_pData = NULL;
}

BOOL CTwoColorBallsDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	m_pData = new CHistoryData(_T("C:\\history.txt"));
	m_bInitialized = m_pData->Init();

	Lucky::GetApplication()->SetActiveDocument(this);

	return TRUE;
}




// CTwoColorBallsDoc serialization

void CTwoColorBallsDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		// TODO: add storing code here
	}
	else
	{
		// TODO: add loading code here
	}
}


// CTwoColorBallsDoc diagnostics

#ifdef _DEBUG
void CTwoColorBallsDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CTwoColorBallsDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CTwoColorBallsDoc commands
