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

// LuckyBallsDoc.cpp : implementation of the CLuckyBallsDoc class
//

#include "stdafx.h"
#include "LuckyBalls.h"
#include "LuckyBallsDoc.h"
#include "Server\Global.h"
#include "Server\Application.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CLuckyBallsDoc

IMPLEMENT_DYNCREATE(CLuckyBallsDoc, CDocument)

BEGIN_MESSAGE_MAP(CLuckyBallsDoc, CDocument)
END_MESSAGE_MAP()


// CLuckyBallsDoc construction/destruction

CLuckyBallsDoc::CLuckyBallsDoc()
{
}

CLuckyBallsDoc::~CLuckyBallsDoc()
{
}

BOOL CLuckyBallsDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	Lucky::GetApplication()->SetActiveDocument(this);

	return TRUE;
}




// CLuckyBallsDoc serialization

void CLuckyBallsDoc::Serialize(CArchive& ar)
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


// CLuckyBallsDoc diagnostics

#ifdef _DEBUG
void CLuckyBallsDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CLuckyBallsDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CLuckyBallsDoc commands
