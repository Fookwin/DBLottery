#pragma once
#include <map>

#define VIEW_HISTORY_NAME _T("View History")
#define VIEW_HITCOUNT_NAME	_T("View Hit Count")
#define VIEW_SMALLODDS_NAME	_T("View Small Odds")
#define VIEW_EVENCOUNT_NAME	_T("View Even Count")

class CLuckyNum;

///////////////////////////////////////////////////////////////////////////////////////////////////
class CHistoryData;
class CLuckyBallsView;
class CHitCountData;
class CBaseViewCtrl
{
public:
	CBaseViewCtrl(CLuckyBallsView* pParent, const CString strName)
		: m_pParent(pParent), m_strInternalName(strName)
	{}
	virtual ~CBaseViewCtrl() {};

	// draw the view on specified DC.
	virtual void Draw(CDC* pDC) {}
	virtual void OnLButtonDown(UINT nFlags, CPoint point) {}
	virtual void OnMouseMove(UINT nFlags, CPoint point) {}
	virtual BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt) { return TRUE; }
	CString GetInternalName() const { return m_strInternalName; }

protected:

	void DrawText(CDC* pDC, const CString& str, CPoint& leftTop);
	void DrawNum(CDC* pDC, int iNum, CPoint& leftTop);
	void DrawLine(CDC* pDC, CPoint& start, CPoint& end);
	void DrawPoint(CDC* pDC, CPoint& pos, int iRadius);

	CLuckyBallsView* m_pParent;
	CString       m_strInternalName;
};

///////////////////////////////////////////////////////////////////////////////////////////////////
class CHistoryViewCtrl : public CBaseViewCtrl
{
public:
	CHistoryViewCtrl(CLuckyBallsView* pParent);
	~CHistoryViewCtrl();

	// draw the view on specified DC.
	virtual void Draw(CDC* pDC);
	virtual void OnLButtonDown(UINT nFlags, CPoint point);
	virtual void OnMouseMove(UINT nFlags, CPoint point);
	virtual BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);

private:
	void DrawHistoryData(CDC* pDC, CRect& rect);

	int m_iCurStartIssue;
	CPoint m_lastDBPoint;

	bool m_bShowMissingTimes;
};

///////////////////////////////////////////////////////////////////////////////////////////////////
class CHitCountViewCtrl : public CBaseViewCtrl
{
public:
	CHitCountViewCtrl(CLuckyBallsView* pParent);
	~CHitCountViewCtrl();

	// draw the view on specified DC.
	virtual void Draw(CDC* pDC);
	virtual void OnLButtonDown(UINT nFlags, CPoint point);
	virtual void OnMouseMove(UINT nFlags, CPoint point);
	virtual BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);

private:
	void DrawHitCounts(CDC* pDC, CRect& rect);

	int m_iCurStartIssue;
	int m_iCurNum;
	CPoint m_lastDBPoint;
	bool m_bBlue;
};

///////////////////////////////////////////////////////////////////////////////////////////////////
class CAverageLinesViewCtrl : public CBaseViewCtrl
{
public:
	CAverageLinesViewCtrl(CLuckyBallsView* pParent);
	~CAverageLinesViewCtrl();

	// draw the view on specified DC.
	virtual void Draw(CDC* pDC);
	virtual void OnLButtonDown(UINT nFlags, CPoint point);
	virtual void OnMouseMove(UINT nFlags, CPoint point);
	virtual BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);

private:
	enum AverageValueTypeEnum
	{
		kEvenValue = 0,
		kContinueValue,
		kRepeatValue,
		kRedMissingValue,
		kSumValue,
		kSmallCountValue,
		kPirmeNumValue,
		kAdjacentValue,
		kBlueValue,
		kBlueMissingValue,
	};

	struct AverageData
	{
		int m_Value;
		double m_Average;
		double m_Average1;
		double m_Average2;
		double m_Average3;
	};

	void DrawData(CDC* pDC, CRect& rect);
	void ResetData(AverageValueTypeEnum valType = kEvenValue);
	void Init();
	int GetValue(CLuckyNum* num, CLuckyNum* previous, int issue, AverageValueTypeEnum valType);

	typedef std::map<int, AverageData> TestValues;

	int m_iCurStartIssue;
	CPoint m_lastDBPoint;
	int m_iCalStep;

	// Data...
	int m_iMinValue;
	int m_iMaxValue;
	TestValues m_Values;
	AverageValueTypeEnum m_eCurrentValueType;

	CString m_strValueTypeDesp;
	CString m_strFirstLineDesp;
	CString m_strSecondLineDesp;
	CString m_strThirdLineDesp;
};

#define REGISTER_VIEW_CONTROLS(pView, controls) \
	CBaseViewCtrl* pCtrl = new CHistoryViewCtrl(pView);\
	controls.insert(std::pair<CString, CBaseViewCtrl*>(pCtrl->GetInternalName(), pCtrl));\
	pCtrl = new CHitCountViewCtrl(pView);\
	controls.insert(std::pair<CString, CBaseViewCtrl*>(pCtrl->GetInternalName(), pCtrl));\
	pCtrl = new CAverageLinesViewCtrl(pView);\
	controls.insert(std::pair<CString, CBaseViewCtrl*>(pCtrl->GetInternalName(), pCtrl));
