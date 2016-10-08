#include "stdafx.h"
#include "LuckyNumGraphics.h"
#include "Server\Global.h"
#include "..\..\LuckyBallsDoc.h"
#include "..\..\LuckyBallsView.h"
#include "..\..\Data\Analysis\HistoryData.h"
#include "..\..\Utilities\AnalysisUtil.h"

#define TOP_MARGIN 20
#define STATUS_MARGIN 120
#define LEFT_MARGIN 100
#define BUTTOM_MARGIN 60
#define RIGHT_MARGIN 20
#define H_OFFSET	30
#define KDJ_CALRAGION	33

///////////////////////////////////////////////////////////////////////////////////////////////////
void CBaseViewCtrl::DrawNum(CDC* pDC, int iNum, CPoint& leftTop)
{
	CString strTemp;
	strTemp.Format(_T("%d"), iNum);
	DrawText(pDC, strTemp, leftTop);
}

void CBaseViewCtrl::DrawText(CDC* pDC, const CString& str, CPoint& leftTop)
{
	CSize txtSize = pDC->GetOutputTextExtent(str);

	CRect rectT;
	rectT.left = leftTop.x;
	rectT.right = leftTop.x + txtSize.cx;
	rectT.top = leftTop.y;
	rectT.bottom = leftTop.y + txtSize.cy;
	pDC->DrawText(str, rectT, DT_SINGLELINE);
}

void CBaseViewCtrl::DrawLine(CDC* pDC, CPoint& start, CPoint& end)
{
	pDC->MoveTo(start);
	pDC->LineTo(end);
}

void CBaseViewCtrl::DrawPoint(CDC* pDC, CPoint& pos, int iRadius)
{
	ASSERT(iRadius > 0);

	CRect rectPt;
	rectPt.left = pos.x - iRadius;
	rectPt.right = pos.x + iRadius;
	rectPt.top = pos.y - iRadius;
	rectPt.bottom = pos.y + iRadius;

	pDC->Ellipse(rectPt);
}

///////////////////////////////////////////////////////////////////////////////////////////////////
CHistoryViewCtrl::CHistoryViewCtrl(CLuckyBallsView* pParent)
	: CBaseViewCtrl(pParent, VIEW_HISTORY_NAME), m_iCurStartIssue(-1), m_bShowMissingTimes(true)
{
}

CHistoryViewCtrl::~CHistoryViewCtrl()
{
}

// draw the view on specified DC.
void CHistoryViewCtrl::Draw(CDC* pDC)
{
	// update view.
	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (m_pParent == NULL || pHistory == NULL)
		return;

	// Create an off-screen DC for double-buffering
	CDC MemDC;
	if (!MemDC.CreateCompatibleDC(pDC)) return;

	// get view rect.
	CRect viewRect;
	m_pParent->GetClientRect(viewRect);

	// create drawing bitmap and select it.
	CBitmap hMemBitmap;
	hMemBitmap.CreateCompatibleBitmap(pDC,viewRect.Width(),viewRect.Height());
	MemDC.SelectObject(&hMemBitmap);

	// fill the background.
	MemDC.FillSolidRect(0,0,viewRect.Width(),viewRect.Height(),RGB(0,0,0));

	// Initialize the areas to show data...
	CRect rectData;
	rectData.left = viewRect.left + LEFT_MARGIN;
	rectData.right = viewRect.right - RIGHT_MARGIN;
	rectData.top = viewRect.top + TOP_MARGIN + STATUS_MARGIN;
	rectData.bottom = viewRect.bottom - BUTTOM_MARGIN;

	// drawing lines...
	DrawHistoryData(&MemDC, rectData);

	// copy to the view.
	pDC->BitBlt(0,0,viewRect.Width(),viewRect.Height(), &MemDC,0,0,SRCCOPY);

	hMemBitmap.DeleteObject();
	MemDC.DeleteDC();
}

void CHistoryViewCtrl::DrawHistoryData(CDC* pDC, CRect& rect)
{
	if (pDC == NULL) return;
	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (pHistory == NULL) return;

	if (m_iCurStartIssue < 0)
	{
		m_iCurStartIssue = pHistory->GetHistoryCount() - 30;
	}

	// drawing lines...
	//
	HPEN hOldPen = static_cast<HPEN>(::GetStockObject(BLACK_PEN));
	HPEN hPrimaryPenLine = ::CreatePen(PS_SOLID, 1, RGB(255, 255, 255));
	HPEN hSecondaryPenLine = ::CreatePen(PS_SOLID, 1, RGB(128,0,0));
	pDC->SetTextColor(RGB(255, 255, 255));

	// Get offset between two neighbour vertical lines.
	pDC->SelectObject(hSecondaryPenLine);
	int iVLCount = 33;
	int iVLOffset = rect.Height() / iVLCount;
	int iVLy = rect.bottom;
	const int iTopStep = 20;
	CString strTemp;
	for (int i = 1; i <= iVLCount; i ++, iVLy -= iVLOffset)
	{
		DrawLine(pDC, CPoint(rect.left, iVLy), CPoint(rect.right, iVLy));

		// Draw text;
		DrawNum(pDC, i, CPoint(rect.left - 30, iVLy - 21));
	}

	pDC->SetTextColor(RGB(0, 255, 0));
	for (int i = 0; i < 6; i ++)
	{
		DrawLine(pDC, CPoint(rect.left, rect.top - iTopStep * i), CPoint(rect.right, rect.top - iTopStep * i));

		// Draw text;
		switch (i)
		{
		case 0:
			strTemp = _T("Blue"); break;
		case 1:
			strTemp = _T("Cnty"); break;
		case 2:
			strTemp = _T("Even"); break;
		case 3:
			strTemp = _T("Sum"); break;
		case 4:
			strTemp = _T("Rep"); break;
		case 5:
			strTemp = _T("Mis"); break;
		}

		DrawText(pDC, strTemp, CPoint(rect.left - 90, rect.top - iTopStep * (i + 1)));
	}

	// Draw horizontal lines and numbers.
	const LuckyNums& nums = pHistory->GetHistory();
	int iHLOffset = H_OFFSET;
	int iHLx = rect.left;
	CString strYear;

	int Missing[33] = {0};
	int iIndex = 0;
	LuckyNums::const_iterator it = nums.begin();
	for (int i = 0; i < m_iCurStartIssue; i ++, it ++, iIndex ++)
	{
		for (int j = 1, iPos = 0; j <= 33; j ++)
		{
			if (it->second->m_red[iPos] != j)
			{
				++ Missing[j - 1];
			}
			else
			{
				int iMis = Missing[j - 1];
				Missing[j - 1] = 0;
				iPos ++;
			}
		}
	}

	LuckyNums::const_iterator itPrv = nums.end();
	for (; it != nums.end() && iHLx <= rect.right; itPrv = it, it ++, iHLx += iHLOffset, iIndex ++)
	{
		pDC->SetTextColor(RGB(255, 255, 255));

		DrawLine(pDC, CPoint(iHLx, rect.bottom), CPoint(iHLx, rect.top - STATUS_MARGIN));

		// Draw text;
		strTemp.Format(_T("%d"), it->first);
		if (strYear != strTemp.Left(4))
		{
			pDC->SetTextColor(RGB(255, 255, 0));
			strYear = strTemp.Left(4);

			DrawText(pDC, strYear, CPoint(iHLx, rect.bottom + 30));
			pDC->SetTextColor(RGB(255, 255, 255));
		}

		strTemp = strTemp.Right(3);
		strTemp.TrimLeft('0');
		DrawText(pDC, strTemp, CPoint(iHLx, rect.bottom + 10));

		// Blue.
		DrawNum(pDC, it->second->m_blue, CPoint(iHLx + 2, rect.top - iTopStep));

		// Continuity...
		DrawNum(pDC, it->second->GetContinuity(), CPoint(iHLx + 2, rect.top - iTopStep * 2));

		// Even count.
		DrawNum(pDC, it->second->GetEvenNumCount(), CPoint(iHLx + 2, rect.top - iTopStep * 3));

		// Sum.
		DrawNum(pDC, it->second->GetSum(), CPoint(iHLx + 2, rect.top - iTopStep * 4));

		// Similarity.
		if (itPrv != nums.end())
		{
			DrawNum(pDC, CAnalysisUtil::GetSimilarity(*it->second, *itPrv->second), CPoint(iHLx - 1, rect.top - iTopStep * 5));
		}

		// Missing.
		pDC->SetTextColor(RGB(255, 255, 255));
		DrawNum(pDC, Lucky::GetHistoryData()->GetRedMissing(it->first), CPoint(iHLx + 2, rect.top - iTopStep * 6));

		// Red numbers...
		int iCurMis = 0;
		for (int i = 1, iPos = 0; i <= 33; i ++)
		{
			int iNum = i;
			if (it->second->m_red[iPos] != i)
			{
				iNum = ++ Missing[i - 1];
				pDC->SetTextColor(iNum > 32 ? RGB(100, 0, 0) : RGB(60, 60, 60));
			}
			else
			{
				int iMis = Missing[i - 1];

				iCurMis += iMis;

				pDC->SetTextColor(RGB(200, 200, 0));
				Missing[i - 1] = 0;
				iPos ++;
			}

			DrawNum(pDC, iNum, CPoint(iHLx + 4, rect.bottom - i * iVLOffset + 1));
		}
	}

	pDC->SetTextColor(RGB(255, 255, 255));

	// Draw vertical base line.
	pDC->SelectObject(hPrimaryPenLine);
	DrawLine(pDC, CPoint(rect.left, rect.bottom), CPoint(rect.left, rect.top - STATUS_MARGIN));
	// Draw horizontal base line.
	DrawLine(pDC, CPoint(rect.left, rect.bottom), CPoint(rect.right, rect.bottom));

	pDC->SelectObject(hOldPen);
	::DeleteObject(hPrimaryPenLine);
	::DeleteObject(hSecondaryPenLine);
}

void CHistoryViewCtrl::OnLButtonDown(UINT nFlags, CPoint point)
{
	m_lastDBPoint = point;
}

void CHistoryViewCtrl::OnMouseMove(UINT nFlags, CPoint point)
{
	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (pHistory == NULL)
		return;

	// check which mouse button down.
	if((nFlags & MK_LBUTTON) == MK_LBUTTON)
	{
		//if ((nFlags & MK_RBUTTON) == MK_RBUTTON)
		{
			int iMaxCount = (int)pHistory->GetHistory().size();
			int iLastStartIssue = m_iCurStartIssue;

			int iStep = - (point.x - m_lastDBPoint.x) / H_OFFSET;
			if (iLastStartIssue + iStep < 0)
				m_iCurStartIssue = 0;
			else if (iLastStartIssue + iStep > iMaxCount)
				m_iCurStartIssue = iMaxCount;
			else
				m_iCurStartIssue += iStep;

			if (iLastStartIssue != m_iCurStartIssue)
			{
				// updating...
				m_pParent->InvalidateRect(NULL, 1);
				m_lastDBPoint = point;
			}
		}
	}
	else if ((nFlags & MK_MBUTTON) == MK_MBUTTON)
	{
	}
	else if ((nFlags & MK_RBUTTON) == MK_RBUTTON)
	{
	}
}

BOOL CHistoryViewCtrl::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (pHistory == NULL) return FALSE;

	int iMaxCount = (int)pHistory->GetHistory().size();
	int iLastStartIssue = m_iCurStartIssue;

	int iTime = - zDelta / 120;
	int iStep = iTime * 10;

	if (iLastStartIssue + iStep < 0)
		m_iCurStartIssue = 0;
	else if (iLastStartIssue + iStep > iMaxCount)
		m_iCurStartIssue = iMaxCount;
	else
		m_iCurStartIssue += iStep;

	// updating...
	m_pParent->InvalidateRect(NULL, 1);

	return TRUE;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
static bool s_bForcastForHit = true;
CHitCountViewCtrl::CHitCountViewCtrl(CLuckyBallsView* pParent)
: CBaseViewCtrl(pParent, VIEW_HITCOUNT_NAME), m_iCurStartIssue(-1), m_iCurNum(1),
m_bBlue(false)
{
	//LuckyNums nums;
	//CAnalysisUtil::GetAllNums(nums);
	//double hit[33][6];
	//CAnalysisUtil::GetNumProbInPos(nums, &hit[0][0]);
}

CHitCountViewCtrl::~CHitCountViewCtrl()
{
}

// draw the view on specified DC.
void CHitCountViewCtrl::Draw(CDC* pDC)
{
	// update view.
	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (m_pParent == NULL || pHistory == NULL)
		return;

	// Create an off-screen DC for double-buffering
	CDC MemDC;
	if (!MemDC.CreateCompatibleDC(pDC)) return;

	// get view rect.
	CRect viewRect;
	m_pParent->GetClientRect(viewRect);

	// create drawing bitmap and select it.
	CBitmap hMemBitmap;
	hMemBitmap.CreateCompatibleBitmap(pDC,viewRect.Width(),viewRect.Height());
	MemDC.SelectObject(&hMemBitmap);

	// fill the background.
	MemDC.FillSolidRect(0,0,viewRect.Width(),viewRect.Height(),RGB(0,0,0));

	// Initialize the areas to show data...
	CRect rectData;
	rectData.left = viewRect.left + LEFT_MARGIN;
	rectData.right = viewRect.right - RIGHT_MARGIN;
	rectData.top = viewRect.top + TOP_MARGIN;
	rectData.bottom = viewRect.bottom - BUTTOM_MARGIN;

	// drawing lines...
	DrawHitCounts(&MemDC, rectData);

	// copy to the view.
	pDC->BitBlt(0,0,viewRect.Width(),viewRect.Height(), &MemDC,0,0,SRCCOPY);

	hMemBitmap.DeleteObject();
	MemDC.DeleteDC();
}

void CHitCountViewCtrl::DrawHitCounts(CDC* pDC, CRect& rect)
{
	if (pDC == NULL) return;
	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (pHistory == NULL) return;

	if (m_iCurStartIssue < 0)
	{
		m_iCurStartIssue = pHistory->GetHistoryCount() - 30;
	}

	const LuckyNums& nums = pHistory->GetHistory();

	// drawing lines...
	//
	CString strTemp;
	HPEN hOldPen = static_cast<HPEN>(::GetStockObject(BLACK_PEN));
	HPEN hWhitePenLine =		::CreatePen(PS_SOLID, 1, RGB(255, 255, 255));
	HPEN hLightRedPenLine =		::CreatePen(PS_SOLID, 1, RGB(128,0,0));
	HPEN hYellowPenLine =		::CreatePen(PS_SOLID, 1, RGB(255, 255, 0));
	HPEN hBluePenLine =			::CreatePen(PS_SOLID, 1, RGB(0, 0, 255));
	HPEN hGreenPenLine =		::CreatePen(PS_SOLID, 1, RGB(0, 255, 0));
	HPEN hRedPenLine =			::CreatePen(PS_SOLID, 1, RGB(255, 0, 0));
	HPEN hRedVolume =			::CreatePen(PS_SOLID, 2, RGB(255, 0, 0));
	HPEN hGreenVolume =			::CreatePen(PS_SOLID, 2, RGB(0, 255, 0));

	// Draw vertical base line and horizontal base line.
	pDC->SelectObject(hWhitePenLine);
	DrawLine(pDC, CPoint(rect.left, rect.bottom), CPoint(rect.left, rect.top));
	DrawLine(pDC, CPoint(rect.left, rect.bottom), CPoint(rect.right, rect.bottom));

	// Calculate offset between two neighbor vertical lines.
	int iVLCount = 100;
	int iVLOffset = (rect.bottom - rect.top) / iVLCount;

	// Draw vertical lines and numbers.
	CString strHit;
	if (m_bBlue)
	{
		int iExpectedHit = (int)nums.size() * 1 / 16;
		int iRealHit = Lucky::GetHistoryData()->GetLastCondition()->m_BlueNumStates[m_iCurNum - 1].m_iHit; 

		strHit.Format(_T("%d (%d / %d)"), m_iCurNum, iRealHit, iExpectedHit);
		pDC->SetTextColor(iRealHit > iExpectedHit ? RGB(255, 255, 255) : RGB(255, 0, 0));
	}
	else
	{
		int iExpectedHit = (int)nums.size() * 2 / 11;
		int iRealHit = Lucky::GetHistoryData()->GetLastCondition()->m_RedNumStates[m_iCurNum - 1].m_iHit; 
	
		strHit.Format(_T("%d (%d / %d)"), m_iCurNum, iRealHit, iExpectedHit);
		pDC->SetTextColor(iRealHit > iExpectedHit ? RGB(255, 255, 255) : RGB(255, 0, 0));
	}
	DrawText(pDC, strHit, CPoint(rect.left - 80, rect.bottom + 40));

	pDC->SetTextColor(RGB(255, 255, 255));

	pDC->SelectObject(hLightRedPenLine);
	int iVLy = rect.bottom - iVLOffset;
	for (int i = 1; i <= iVLCount; i ++, iVLy -= iVLOffset)
	{
		DrawLine(pDC, CPoint(rect.left, iVLy), CPoint(rect.right, iVLy));

		if (i % 5 == 0)
		{
			// Draw text;
			DrawNum(pDC, i, CPoint(rect.left - 30, iVLy - 10));
		}
	}

	// Draw average line.
	pDC->SelectObject(hRedPenLine);
	DrawLine(pDC, CPoint(rect.left, rect.bottom - int(50 * iVLOffset)),
		CPoint(rect.right, rect.bottom - int(50 * iVLOffset)));
	DrawLine(pDC, CPoint(rect.left, rect.bottom - int(20 * iVLOffset)),
		CPoint(rect.right, rect.bottom - int(20 * iVLOffset)));
	DrawLine(pDC, CPoint(rect.left, rect.bottom - int(80 * iVLOffset)),
		CPoint(rect.right, rect.bottom - int(80 * iVLOffset)));

	// Draw horizontal lines and numbers.
	int iHLx = rect.left + H_OFFSET;

	// prepare data for kdj calculation
	int iRecentNums[KDJ_CALRAGION] = {0};
	int iOffset = 0;
	int iValue= 0;
	double iRecentMax = -1000, iRecentMin = 1000;
	double k = 50, d = 50, j = 50;
	CPoint prek(0,0), pred(0,0), prej(0,0), preStep(0, 0);
	CPoint preHot1(0,0), preHot2(0,0), preHot3(0,0);

	int iHitInRagion = 0, iRagion = 10;
	int iIndex = 0;
	int iCalStep = 17;
	CString strYear;
	LuckyNums::const_iterator itStepStart = nums.begin();
	LuckyNums::const_iterator itFrom1 = nums.begin();
	LuckyNums::const_iterator itFrom2 = nums.begin();
	LuckyNums::const_iterator itFrom3 = nums.begin();
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end() && iHLx <= rect.right; it ++, iIndex ++)
	{
		// Draw issue;
		strTemp.Format(_T("%d"), it->first);
		if (strYear != strTemp.Left(4))
		{
			pDC->SetTextColor(RGB(255, 255, 0));
			strYear = strTemp.Left(4);

			DrawText(pDC, strYear, CPoint(iHLx - 10, rect.bottom + 30));
			pDC->SetTextColor(RGB(255, 255, 255));
		}

		strTemp = strTemp.Right(3);
		strTemp.TrimLeft('0');
		DrawText(pDC, strTemp, CPoint(iHLx - 10, rect.bottom + 10));

		// Get the general sum value...
		bool bHas = m_bBlue ? it->second->m_blue == m_iCurNum : it->second->HasNum(m_iCurNum);
		if (bHas)
		{
			iValue += m_bBlue ? 15 : 9;
		}
		else
		{
			iValue -= m_bBlue ? 1 : 2;
		}

		// Update the set for recent numbers.
		iRecentNums[iOffset ++] = iValue;
		if (iOffset == KDJ_CALRAGION) iOffset = 0;

		if (iIndex > KDJ_CALRAGION)
		{
			// Update KDJ values.
			CAnalysisUtil::GetRagion(&iRecentNums[0], iRecentMax, iRecentMin, KDJ_CALRAGION);
			CAnalysisUtil::CalulateKDJ(iValue, iRecentMax, iRecentMin, k, d, j);
		}

		if (iIndex >= 11)
			itFrom1 ++;
		
		if (iIndex >= 22)
			itFrom2 ++;
		
		if (iIndex >=  33)
			itFrom3 ++;

		if (iIndex < m_iCurStartIssue)
			continue;

		// Draw horitical line with specific color according to ...
		pDC->SelectObject(hLightRedPenLine);
		DrawLine(pDC, CPoint(iHLx, rect.bottom), CPoint(iHLx, rect.top));
		
		if (iIndex > KDJ_CALRAGION)
		{
			CPoint posk(iHLx, rect.bottom - (int)(k+20) * iVLOffset);
			if (prek.x > 0)
			{
				pDC->SelectObject(hYellowPenLine);
				DrawLine(pDC, prek, posk);
			}
			prek = posk;

			CPoint posd(iHLx, rect.bottom - (int)(d+20) * iVLOffset);
			if (pred.x > 0)
			{
				pDC->SelectObject(hBluePenLine);
				DrawLine(pDC, pred, posd);
			}
			pred = posd;

			CPoint posj(iHLx, rect.bottom - (int)(j+20) * iVLOffset);
			if (prej.x > 0)
			{
				pDC->SelectObject(hGreenPenLine);
				DrawLine(pDC, prej, posj);
			}
			prej = posj;

			if (bHas)
			{
				pDC->SelectObject(hWhitePenLine);
				DrawPoint(pDC, CPoint(iHLx, rect.bottom - (int)(k+20) * iVLOffset), 3);
				DrawPoint(pDC, CPoint(iHLx, rect.bottom - (int)(d+20) * iVLOffset), 3);
				DrawPoint(pDC, CPoint(iHLx, rect.bottom - (int)(j+20) * iVLOffset), 3);
			}
		}
		else if (bHas)// Draw white point to indicate the number is appearing.
		{
			pDC->SelectObject(hWhitePenLine);
			DrawPoint(pDC, CPoint(iHLx, rect.bottom - 50 * iVLOffset), 3);
		}

		iHLx += H_OFFSET;
	}

	if (iHLx < rect.right && iIndex == (int)nums.size())
	{
		HPEN hYellowDashPenLine = ::CreatePen(PS_DOT, 1, RGB(255, 255, 0));
		HPEN hBlueDashPenLine = ::CreatePen(PS_DOT, 1, RGB(0, 0, 255));
		HPEN hGreenDashPenLine = ::CreatePen(PS_DOT, 1, RGB(0, 255, 0));
		HPEN hWriteDashPenLine = ::CreatePen(PS_DOT, 1, RGB(255, 255, 255));

		// forcasting...
		if (s_bForcastForHit)
		{
			iValue += m_bBlue ? 15 : 9;
		}
		else
		{
			iValue -= m_bBlue ? 1 : 2;
		}

		// Update the set for recent numbers.
		iRecentNums[iOffset ++] = iValue;
		if (iOffset == KDJ_CALRAGION) iOffset = 0;

		// Update KDJ values.
		CAnalysisUtil::GetRagion(&iRecentNums[0], iRecentMax, iRecentMin, KDJ_CALRAGION);
		CAnalysisUtil::CalulateKDJ(iValue, iRecentMax, iRecentMin, k, d, j);

		// Draw horitical line with specific color according to ...
		pDC->SelectObject(hLightRedPenLine);
		DrawLine(pDC, CPoint(iHLx, rect.bottom), CPoint(iHLx, rect.top));

		CPoint posk(iHLx, rect.bottom - (int)(k+20) * iVLOffset);
		pDC->SelectObject(hYellowDashPenLine);
		DrawLine(pDC, prek, posk);

		CPoint posd(iHLx, rect.bottom - (int)(d+20) * iVLOffset);
		pDC->SelectObject(hBlueDashPenLine);
		DrawLine(pDC, pred, posd);

		CPoint posj(iHLx, rect.bottom - (int)(j+20) * iVLOffset);
		pDC->SelectObject(hGreenDashPenLine);
		DrawLine(pDC, prej, posj);

		if (s_bForcastForHit)
		{
			pDC->SelectObject(hRedPenLine);
			DrawPoint(pDC, CPoint(iHLx, rect.bottom - (int)(k+20) * iVLOffset), 3);
			DrawPoint(pDC, CPoint(iHLx, rect.bottom - (int)(d+20) * iVLOffset), 3);
			DrawPoint(pDC, CPoint(iHLx, rect.bottom - (int)(j+20) * iVLOffset), 3);
		}

		::DeleteObject(hYellowDashPenLine);
		::DeleteObject(hBlueDashPenLine);
		::DeleteObject(hGreenDashPenLine);
		::DeleteObject(hWriteDashPenLine);
	}

	pDC->SelectObject(hOldPen);
	::DeleteObject(hWhitePenLine);
	::DeleteObject(hLightRedPenLine);
	::DeleteObject(hYellowPenLine);
	::DeleteObject(hBluePenLine);
	::DeleteObject(hGreenPenLine);
	::DeleteObject(hRedPenLine);
}

void CHitCountViewCtrl::OnLButtonDown(UINT nFlags, CPoint point)
{
	m_lastDBPoint = point;
}

void CHitCountViewCtrl::OnMouseMove(UINT nFlags, CPoint point)
{
	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (pHistory == NULL)
		return;

	// check which mouse button down.
	if((nFlags & MK_LBUTTON) == MK_LBUTTON)
	{
		int iMaxCount = (int)pHistory->GetHistory().size();
		int iLastStartIssue = m_iCurStartIssue;

		int iStep = - (point.x - m_lastDBPoint.x) / H_OFFSET;
		if (iLastStartIssue + iStep < 0)
			m_iCurStartIssue = 0;
		else if (iLastStartIssue + iStep > iMaxCount)
			m_iCurStartIssue = iMaxCount;
		else
			m_iCurStartIssue += iStep;

		if (iLastStartIssue != m_iCurStartIssue)
		{
			// updating...
			m_pParent->InvalidateRect(NULL, 1);
			m_lastDBPoint = point;
		}
	}
}

BOOL CHitCountViewCtrl::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	if (::GetKeyState(VK_TAB) < 0)
	{
		s_bForcastForHit = !s_bForcastForHit;

		// updating...
		m_pParent->InvalidateRect(NULL, 1);

		return TRUE;
	}

	if (::GetKeyState(VK_CONTROL) >= 0)
	{
		if (::GetKeyState(VK_SHIFT) < 0)
		{
			if (m_bBlue)
			{
				m_bBlue = false;
				m_iCurNum = 1;
			}
			else
			{
				m_bBlue = true;
				m_iCurNum = 1;
			}
		}
		else
		{
			CHistoryData* pHistory = Lucky::GetHistoryData();
			if (pHistory == NULL) return FALSE;

			int iMaxCount = (int)pHistory->GetHistory().size();
			int iLastStartIssue = m_iCurStartIssue;

			int iTime = - zDelta / 120;
			int iStep = iTime * 10;

			if (iLastStartIssue + iStep < 0)
				m_iCurStartIssue = 0;
			else if (iLastStartIssue + iStep > iMaxCount)
				m_iCurStartIssue = iMaxCount;
			else
				m_iCurStartIssue += iStep;
		}
	}
	else
	{
		// zoom in or out ?
		if (zDelta > 0)
		{
			if (m_iCurNum > 1)
				m_iCurNum --;
		}
		else
		{
			if (m_iCurNum < (m_bBlue ? 16 : 33))
				m_iCurNum ++;
		}
	}

	// updating...
	m_pParent->InvalidateRect(NULL, 1);

	return TRUE;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
CAverageLinesViewCtrl::CAverageLinesViewCtrl(CLuckyBallsView* pParent)
: CBaseViewCtrl(pParent, VIEW_EVENCOUNT_NAME), m_iCurStartIssue(-1),
m_iMinValue(0), m_iMaxValue(10), m_iCalStep(5)
{
}

CAverageLinesViewCtrl::~CAverageLinesViewCtrl()
{
}

void CAverageLinesViewCtrl::Init()
{
	m_iCurStartIssue = (int)Lucky::GetHistoryData()->GetHistory().size() - 60;
	ResetData();
}

void CAverageLinesViewCtrl::ResetData(AverageValueTypeEnum valType /*= kEvenValue*/)
{
	m_Values.clear();
	m_eCurrentValueType = valType;

	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (pHistory == NULL) return;

	const LuckyNums& nums = pHistory->GetHistory();
	int iIndex = 0;
	int iSumTotal = 0, iSumStep1 = 0, iSumStep2 = 0, iSumStep3 = 0;
	LuckyNums::const_iterator itStep1 = nums.begin(), itStep2 = nums.begin(), itStep3 = nums.begin();
	LuckyNums::const_iterator itPrevious = nums.end();
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); itPrevious = it, it ++, iIndex ++)
	{
		// Calculate the counts.
		int iValue = GetValue(it->second, itPrevious == nums.end() ? NULL : itPrevious->second, it->first, m_eCurrentValueType);

		iSumTotal += iValue;
		iSumStep1 += iValue;
		iSumStep2 += iValue;
		iSumStep3 += iValue;
		if (iIndex >= m_iCalStep)
		{
			LuckyNums::const_iterator itPrev = itStep1; itPrev --;
			iSumStep1 -= GetValue(itStep1->second, itPrev == nums.end() ? NULL : itPrev->second, itStep1->first, m_eCurrentValueType);
			if (iIndex >= m_iCalStep * 2)
			{
				itPrev = itStep2; itPrev --;
				iSumStep2 -= GetValue(itStep2->second, itPrev == nums.end() ? NULL : itPrev->second, itStep2->first, m_eCurrentValueType);
				if (iIndex >= m_iCalStep * 3)
				{
					itPrev = itStep3; itPrev --;
					iSumStep3 -= GetValue(itStep3->second, itPrev == nums.end() ? NULL : itPrev->second, itStep3->first, m_eCurrentValueType);
					++ itStep3;
				}
				++ itStep2;
			}
			++ itStep1;
		}

		AverageData data;
		data.m_Value = iValue;
		data.m_Average= ((double)iSumTotal) / (iIndex + 1);
		data.m_Average1 = ((double)iSumStep1) / m_iCalStep;
		data.m_Average2 = ((double)iSumStep2) / m_iCalStep / 2;
		data.m_Average3 = ((double)iSumStep3) / m_iCalStep / 3;

		m_Values.insert(std::make_pair(it->first, data));
	}

	switch (m_eCurrentValueType)
	{
	case kEvenValue: 		
		{
			m_iMinValue = 0;
			m_iMaxValue = 6;
			m_strValueTypeDesp = _T("Even");
			break;
		}
	case kContinueValue: 
		{
			m_iMinValue = 0;
			m_iMaxValue = 5;
			m_strValueTypeDesp = _T("Continue");
			break;
		}
	case kRepeatValue:
		{
			m_iMinValue = 0;
			m_iMaxValue = 6;
			m_strValueTypeDesp = _T("Repeat");
			break;
		}
	case kRedMissingValue:
		{
			m_iMinValue = 6;
			m_iMaxValue = 50;
			m_strValueTypeDesp = _T("Red_Miss");
			break;
		}
	case kBlueMissingValue:
		{
			m_iMinValue = 0;
			m_iMaxValue = 60;
			m_strValueTypeDesp = _T("Blue_Miss");
			break;
		}
	case kSumValue:
		{
			m_iMinValue = 24;
			m_iMaxValue = 183;
			m_strValueTypeDesp = _T("Sum");
			break;
		}
	case kSmallCountValue:
		{
			m_iMinValue = 0;
			m_iMaxValue = 6;
			m_strValueTypeDesp = _T("Small");
			break;
		}
	case kBlueValue:
		{
			m_iMinValue = 1;
			m_iMaxValue = 16;
			m_strValueTypeDesp = _T("Blue");
			break;
		}
	case kPirmeNumValue:
		{
			m_iMinValue = 0;
			m_iMaxValue = 6;
			m_strValueTypeDesp = _T("Pirme");
			break;
		}
	case kAdjacentValue:
		{
			m_iMinValue = 0;
			m_iMaxValue = 6;
			m_strValueTypeDesp = _T("Adjacent");
			break;
		}
	default:
		return;
	}
}

int CAverageLinesViewCtrl::GetValue(CLuckyNum* num, CLuckyNum* previous, int issue, AverageValueTypeEnum valType)
{
	if (num == NULL) 
		return 0;

	switch (m_eCurrentValueType)
	{
	case kEvenValue: 
		return num->GetEvenNumCount();
	case kContinueValue: 
		return num->GetContinuity();
	case kRepeatValue:
		if (NULL == previous)
			return 0;
		else
			return CAnalysisUtil::GetSimilarity(*num, *previous);
	case kRedMissingValue:
		return Lucky::GetHistoryData()->GetRedMissing(issue);
	case kBlueMissingValue:
		return Lucky::GetHistoryData()->GetBlueMissing(issue);
	case kSumValue:
		return num->GetSum();
	case kSmallCountValue:
		return num->GetSmallNumCount();
	case kBlueValue:
		return num->m_blue;
	case kPirmeNumValue:
		return num->GetPirmeNumCount();
	case kAdjacentValue:
		if (NULL == previous)
			return 0;
		else
			return CAnalysisUtil::GetAdjacentSimilarity(*num, *previous); 
	default:
		return 0;
	}
}

// draw the view on specified DC.
void CAverageLinesViewCtrl::Draw(CDC* pDC)
{
	// Create an off-screen DC for double-buffering
	CDC MemDC;
	if (!MemDC.CreateCompatibleDC(pDC)) return;

	// get view rect.
	CRect viewRect;
	m_pParent->GetClientRect(viewRect);

	// create drawing bitmap and select it.
	CBitmap hMemBitmap;
	hMemBitmap.CreateCompatibleBitmap(pDC,viewRect.Width(),viewRect.Height());
	MemDC.SelectObject(&hMemBitmap);

	// fill the background.
	MemDC.FillSolidRect(0,0,viewRect.Width(),viewRect.Height(),RGB(0,0,0));

	// Initialize the areas to show data...
	CRect rectData;
	rectData.left = viewRect.left + LEFT_MARGIN;
	rectData.right = viewRect.right - RIGHT_MARGIN;
	rectData.top = viewRect.top + TOP_MARGIN;
	rectData.bottom = viewRect.bottom - BUTTOM_MARGIN;

	// drawing data...
	DrawData(&MemDC, rectData);

	// copy to the view.
	pDC->BitBlt(0,0,viewRect.Width(),viewRect.Height(), &MemDC,0,0,SRCCOPY);

	hMemBitmap.DeleteObject();
	MemDC.DeleteDC();
}

void CAverageLinesViewCtrl::DrawData(CDC* pDC, CRect& rect)
{
	if (pDC == NULL) return;

	if (m_iCurStartIssue < 0)
	{
		Init();
	}

	// drawing lines...
	//
	CString strTemp;
	HPEN hOldPen = static_cast<HPEN>(::GetStockObject(BLACK_PEN));
	HPEN hPrimaryPenLine = ::CreatePen(PS_SOLID, 1, RGB(255, 255, 255));
	HPEN hSecondaryPenLine = ::CreatePen(PS_SOLID, 1, RGB(128,0,0));
	HPEN hConnectPenLine = ::CreatePen(PS_SOLID, 1, RGB(255, 255, 255));
	HPEN hConnect33PenLine = ::CreatePen(PS_SOLID, 1, RGB(255, 255, 0));
	HPEN hConnect66PenLine = ::CreatePen(PS_SOLID, 1, RGB(0, 0, 255));
	HPEN hConnect132PenLine = ::CreatePen(PS_SOLID, 1, RGB(0, 255, 0));
	pDC->SelectObject(hPrimaryPenLine);
	pDC->SetTextColor(RGB(255, 255, 255));

	// Get offset between two neighbor vertical lines.
	int iVLCount = m_iMaxValue - m_iMinValue;
	int iStep = 1;
	if (iVLCount > 25)
	{
		iStep = iVLCount / 25 + 1;
		iVLCount = 25;
	}

	int iVLOffset = (rect.bottom - rect.top) / iVLCount;

	pDC->SelectObject(hSecondaryPenLine);

	int iVLy = rect.bottom - iVLOffset;
	for (int i = 1; i <= iVLCount; i ++, iVLy -= iVLOffset)
	{
		DrawLine(pDC, CPoint(rect.left, iVLy), CPoint(rect.right, iVLy));

		// Draw text;
		DrawNum(pDC, m_iMinValue + iStep * i, CPoint(rect.left - 30, iVLy - 10));
	}

	// Draw vertical lines and numbers.
	pDC->SetTextColor(RGB(255, 255, 255));
	DrawText(pDC, m_strValueTypeDesp, CPoint(rect.left - 45, rect.bottom - 20));

	// Draw horizontal lines and numbers.
	int iHLOffset = H_OFFSET;
	int iHLx = rect.left + iHLOffset;
	int iIndex = 0;
	CString strYear;
	int iLastIndex = (int)m_Values.size() - 1;

	CPoint pre(0,0), pre1(0,0), pre2(0,0), pre3(0,0);
	for (TestValues::iterator it = m_Values.begin(); it != m_Values.end() && iHLx <= rect.right; it ++, iIndex ++)
	{
		if (iIndex < m_iCurStartIssue)
			continue;

		DrawLine(pDC, CPoint(iHLx, rect.bottom), CPoint(iHLx, rect.top));

		// Draw issue;
		strTemp.Format(_T("%d"), it->first);
		if (strYear != strTemp.Left(4))
		{
			pDC->SetTextColor(RGB(255, 255, 0));
			strYear = strTemp.Left(4);

			DrawText(pDC, strYear, CPoint(iHLx - 10, rect.bottom + 30));
		}

		strTemp = strTemp.Right(3);
		strTemp.TrimLeft('0');
		if (iIndex == iLastIndex - m_iCalStep + 1)
			pDC->SetTextColor(RGB(255, 255, 0));
		else if (iIndex == iLastIndex - m_iCalStep * 2 + 1)
			pDC->SetTextColor(RGB(0, 255, 0));
		else if (iIndex == iLastIndex - m_iCalStep * 3 + 1)
			pDC->SetTextColor(RGB(0, 0, 255));
		else
			pDC->SetTextColor(RGB(255, 255, 255));
		DrawText(pDC, strTemp, CPoint(iHLx - 10, rect.bottom + 10));

		// Draw small odds in whole issues.
		int iPos = rect.bottom - (int)((double)(it->second.m_Value - m_iMinValue) / iStep * iVLOffset);
		DrawPoint(pDC, CPoint(iHLx, iPos), 6);

		int iNumy = 0;
		if (iIndex >= 0)
		{
			iNumy = rect.bottom - (int)((it->second.m_Average - m_iMinValue) / iStep * iVLOffset);
			CPoint pos(iHLx, iNumy);
			if (pre.x > 0 && pre.y > 0)
			{
				pDC->SelectObject(hConnectPenLine);
				DrawLine(pDC, pre, pos);
				pDC->SelectObject(hSecondaryPenLine);
			}
			pre = pos;
		}

		// Draw within one step.
		if (iIndex >= m_iCalStep - 1)
		{
			iNumy = rect.bottom - (int)((it->second.m_Average1 - m_iMinValue) / iStep * iVLOffset);
			CPoint pos(iHLx, iNumy);

			if (pre1.x > 0 && pre1.y > 0)
			{
				pDC->SelectObject(hConnect33PenLine);
				DrawLine(pDC, pre1, pos);
				pDC->SelectObject(hSecondaryPenLine);
			}
			pre1 = pos;
		}

		// Draw within 2 steps.
		if (iIndex >= (m_iCalStep * 2 - 1))
		{
			iNumy = rect.bottom - (int)((it->second.m_Average2 - m_iMinValue) / iStep * iVLOffset);
			CPoint pos(iHLx, iNumy);

			if (pre2.x > 0 && pre2.y > 0)
			{
				pDC->SelectObject(hConnect66PenLine);
				DrawLine(pDC, pre2, pos);
				pDC->SelectObject(hSecondaryPenLine);
			}
			pre2 = pos;
		}

		// Draw within last 3 steps.
		if (iIndex >= (m_iCalStep * 3 - 1))
		{
			iNumy = rect.bottom - (int)((it->second.m_Average3 - m_iMinValue) / iStep * iVLOffset);
			CPoint pos(iHLx, iNumy);

			if (pre3.x > 0 && pre3.y > 0)
			{
				pDC->SelectObject(hConnect132PenLine);
				DrawLine(pDC, pre3, pos);
				pDC->SelectObject(hSecondaryPenLine);
			}
			pre3 = pos;
		}

		iHLx += iHLOffset;
	}

	// Draw vertical base line.
	pDC->SelectObject(hPrimaryPenLine);
	DrawLine(pDC, CPoint(rect.left, rect.bottom), CPoint(rect.left, rect.top - STATUS_MARGIN));
	// Draw horizontal base line.
	DrawLine(pDC, CPoint(rect.left, rect.bottom), CPoint(rect.right, rect.bottom));

	pDC->SelectObject(hOldPen);
	::DeleteObject(hPrimaryPenLine);
	::DeleteObject(hSecondaryPenLine);
	::DeleteObject(hConnectPenLine);
	::DeleteObject(hConnect33PenLine);
	::DeleteObject(hConnect66PenLine);
	::DeleteObject(hConnect132PenLine);

}

void CAverageLinesViewCtrl::OnLButtonDown(UINT nFlags, CPoint point)
{
	m_lastDBPoint = point;
}

void CAverageLinesViewCtrl::OnMouseMove(UINT nFlags, CPoint point)
{
	CHistoryData* pHistory = Lucky::GetHistoryData();
	if (pHistory == NULL)
		return;

	// check which mouse button down.
	if((nFlags & MK_LBUTTON) == MK_LBUTTON)
	{
		int iMaxCount = (int)pHistory->GetHistory().size();
		int iLastStartIssue = m_iCurStartIssue;

		int iStep = - (point.x - m_lastDBPoint.x) / H_OFFSET;
		if (iLastStartIssue + iStep < 0)
			m_iCurStartIssue = 0;
		else if (iLastStartIssue + iStep > iMaxCount)
			m_iCurStartIssue = iMaxCount;
		else
			m_iCurStartIssue += iStep;

		if (iLastStartIssue != m_iCurStartIssue)
		{
			// updating...
			m_pParent->InvalidateRect(NULL, 1);
			m_lastDBPoint = point;
		}
	}
}

BOOL CAverageLinesViewCtrl::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	if (::GetKeyState(VK_CONTROL) >= 0)
	{
		CHistoryData* pHistory = Lucky::GetHistoryData();
		if (pHistory == NULL) return FALSE;

		int iMaxCount = (int)pHistory->GetHistory().size();
		int iLastStartIssue = m_iCurStartIssue;

		int iTime = - zDelta / 120;
		int iStep = iTime * 10;

		if (iLastStartIssue + iStep < 0)
			m_iCurStartIssue = 0;
		else if (iLastStartIssue + iStep > iMaxCount)
			m_iCurStartIssue = iMaxCount;
		else
			m_iCurStartIssue += iStep;
	}
	else
	{
		// zoom in or out ?
		if (zDelta > 0)
		{
			if (m_eCurrentValueType > 0)
			{
				m_eCurrentValueType = (AverageValueTypeEnum)(m_eCurrentValueType - 1);
				ResetData(m_eCurrentValueType);
			}
		}
		else
		{
			if (m_eCurrentValueType < kBlueMissingValue)
			{
				m_eCurrentValueType = (AverageValueTypeEnum)(m_eCurrentValueType + 1);
				ResetData(m_eCurrentValueType);
			}
		}
	}

	// updating...
	m_pParent->InvalidateRect(NULL, 1);

	return TRUE;
}