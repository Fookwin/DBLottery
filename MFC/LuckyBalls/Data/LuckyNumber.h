#pragma once
#include <map>
#include <vector>
#include "Support\ITextSupport.h"

class CLuckyNum
{
public:
	CLuckyNum();
	CLuckyNum(int red1, int red2, int red3, int red4, int red5, int red6, int blue);

	bool IsValid() const;
	int GetContinuity() const;
	int GetEvenNumCount() const;
	int GetPirmeNumCount() const;
	int GetSum() const;
	int GetSmallNumCount() const;
	bool HasNum(int iTest) const;
	int GetRepeat() const;
	int GetRedMissing() const;
	int GetBlueMissing() const;

	int m_red[6];
	int m_blue;

	// Analyze info.
	mutable int m_iContinuity;
	mutable int m_iEvenCount;
	mutable int m_iSum;
	mutable	int m_iMinNumCount; // the count of the numbers less than 17.
	mutable	int m_iPrimeNumCount;
	mutable int	m_iRepeatPreviousIssue;
	mutable int	m_iRedMissing;
	mutable int	m_iBlueMissing;
};
typedef std::map<int, CLuckyNum*> LuckyNums;

///////////////////////////////////////////////////////////////////////////////////////////////////
class CRagion
{
public:
	CRagion(int iMin = 0, int iStep = 0) : m_iMin(iMin), m_iStep(iStep)
	{
		ASSERT(m_iMin >= 0 && iStep >= 0);
	}

	~CRagion() {}

	int Count() const
	{
		return m_iStep + 1;
	}

	int CompareTo(int iNum) const
	{
		return CompareTo(CRagion(iNum));
	}

	int CompareTo(const CRagion& other) const
	{
		if (m_iMin > other.m_iMin + other.m_iStep)
			return 1;
		else if (m_iMin + m_iStep < other.m_iMin)
			return -1;
		else
			return 0;
	}

	CString GetText() const
	{
		CString str;
		if (m_iStep == 0)
			str.Format(_T("%d"), m_iMin);
		else
			str.Format(_T("(%d~%d)"), m_iMin, m_iMin + m_iStep);
		return str;
	}

	bool SetText(const CString& str)
	{
		int iLeftBracket = str.Find('(');
		if (iLeftBracket < 0)
		{
			// a single number.
			m_iMin = _ttoi(str);
			ASSERT(m_iMin >= 0);
			m_iStep = 0;
		}
		else
		{
			int iRightBracket = str.Find(')');
			ASSERT(iRightBracket > iLeftBracket + 1);
			if (iRightBracket <= iLeftBracket + 1)
				return false;

			int iBreak = str.Find('~');
			CString strMin = str.Mid(iLeftBracket + 1, iBreak - iLeftBracket - 1);
			CString strMax = str.Mid(iBreak + 1, iRightBracket - iBreak - 1);
			m_iMin = _ttoi(strMin);
			m_iStep = _ttoi(strMax) - m_iMin;
			ASSERT(m_iStep >= 0 && m_iMin >= 0);
			if (m_iStep < 0 || m_iMin < 0)
				return false;
		}

		return true;
	}

	int m_iMin;
	int m_iStep; // iStep == 0, means it represents a single number.
};
typedef std::vector<CRagion> Ragions;

class CNumSet : public ITextSupport
{
public:
	CNumSet() {};
	CNumSet(const CRagion& ragion);
	CNumSet(int* pNums, int iCount);
	CNumSet(const CString& strText);
	CNumSet(const CNumSet& other);
	~CNumSet() {};

	virtual bool		SetText(const CString& str);
	virtual CString		GetText() const;

	bool				SetNumSet(const CRagion& ragion);
	bool				SetNumSet(int* pNums, int iCount);
	bool				SetNumSet(const CString& strText);

	bool				AddNum(int iNum);
	bool				AddNum(const CRagion& ragion);
	bool				AddNums(const CNumSet& addition);
	bool				Contains(int iNum) const;
	const				Ragions& GetNums() const { return m_ValidNumbers; }
	void				Clear() { m_ValidNumbers.clear(); }

	CNumSet&			operator=(const CNumSet& other);

	int					Count() const;

protected:
	Ragions				m_ValidNumbers;
};

