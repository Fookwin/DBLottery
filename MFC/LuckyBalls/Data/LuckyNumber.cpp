#include "stdafx.h"
#include "LuckyNumber.h"
#include "Analysis/HistoryData.h"
#include "Server/Global.h"

CLuckyNum::CLuckyNum()
: m_blue(1), m_iContinuity(-1), m_iEvenCount(-1), m_iSum(-1), m_iMinNumCount(-1), m_iRepeatPreviousIssue(-1),
  m_iPrimeNumCount(-1), m_iRedMissing(-1), m_iBlueMissing(-1)
{
	m_red[0] = 0;
	m_red[1] = 0;
	m_red[2] = 0;
	m_red[3] = 0;
	m_red[4] = 0;
	m_red[5] = 0;
}

CLuckyNum::CLuckyNum(int red1, int red2, int red3, int red4, int red5, int red6, int iblue)
: m_blue(iblue), m_iContinuity(-1), m_iEvenCount(-1), m_iSum(-1), m_iMinNumCount(-1), m_iRepeatPreviousIssue(-1),
  m_iPrimeNumCount(-1), m_iRedMissing(-1), m_iBlueMissing(-1)
{
	m_red[0] = red1;
	m_red[1] = red2;
	m_red[2] = red3;
	m_red[3] = red4;
	m_red[4] = red5;
	m_red[5] = red6;
}

int CLuckyNum::GetContinuity() const
{
	if (m_iContinuity < 0)
	{
		m_iContinuity = 0;
		for (int i = 0; i < 5; i ++)
		{
			if (m_red[i] + 1 == m_red[i + 1])
				m_iContinuity ++;
		}
	}

	return m_iContinuity;
}

int CLuckyNum::GetSum() const
{
	if (m_iSum < 0)
	{
		m_iSum = 0;
		for (int i = 0; i < 6; i ++)
		{
			m_iSum += m_red[i];
		}
	}

	return m_iSum;
}

int CLuckyNum::GetRepeat() const
{
	if (m_iRepeatPreviousIssue < 0)
	{
		const CLuckyNum* pLast = Lucky::GetHistoryData()->GetTestNum();
		ASSERT(pLast); if (!pLast) return -1;

		m_iRepeatPreviousIssue = 0;
		for (int i = 0; i < 6; i ++)
		{
			if (pLast->HasNum(m_red[i]))
				m_iRepeatPreviousIssue ++;
		}
	}
	return m_iRepeatPreviousIssue;
}

bool CLuckyNum::IsValid() const
{
	for (int i = 0; i < 5; i ++)
	{
		if (m_red[i] >= m_red[i + 1])
			return false;
	}
	return true;
}

int CLuckyNum::GetEvenNumCount() const
{
	if (m_iEvenCount < 0)
	{
		m_iEvenCount = 0;
		for (int i = 0; i < 6; i ++)
		{
			if (m_red[i] % 2 == 0)
				m_iEvenCount ++;
		}
	}

	return m_iEvenCount;
}

bool CLuckyNum::HasNum(int iTest) const
{
	for (int i = 0; i < 6; i ++)
	{
		if (m_red[i] == iTest)
			return true;
		else if (iTest < m_red[i])
			return false;
	}
	return false;
}

int CLuckyNum::GetSmallNumCount() const
{
	if (m_iMinNumCount < 0)
	{
		m_iMinNumCount = 0;
		for (int i = 0; i < 6; i ++)
		{
			if (m_red[i] < 17)
				m_iMinNumCount ++;
		}
	}

	return m_iMinNumCount;
}

int CLuckyNum::GetPirmeNumCount() const
{
	if (m_iPrimeNumCount < 0)
	{
		m_iPrimeNumCount = 0;
		for (int i = 0; i < 6; i ++)
		{
			if (m_red[i] == 1 ||
				m_red[i] == 2 ||
				m_red[i] == 3 ||
				m_red[i] == 5 ||
				m_red[i] == 7 ||
				m_red[i] == 11 ||
				m_red[i] == 13 ||
				m_red[i] == 17 ||
				m_red[i] == 19 ||
				m_red[i] == 23 ||
				m_red[i] == 29 ||
				m_red[i] == 31)
				m_iPrimeNumCount ++;
		}
	}

	return m_iPrimeNumCount;
}

int CLuckyNum::GetRedMissing() const
{
	if (m_iRedMissing < 0)
	{
		const CNumberCondition* pCondi = Lucky::GetHistoryData()->GetTestCondition();
		ASSERT(pCondi); if (!pCondi) return -1;

		m_iRedMissing = 0;
		for (int i = 0; i < 6; i ++)
		{
			m_iRedMissing += pCondi->m_RedNumStates[m_red[i] - 1].m_iMissing;
		}
	}
	return m_iRedMissing;
}

int CLuckyNum::GetBlueMissing() const
{
	if (m_iBlueMissing < 0)
	{
		const CNumberCondition* pCondi = Lucky::GetHistoryData()->GetTestCondition();
		ASSERT(pCondi); if (!pCondi) return -1;

		m_iBlueMissing = pCondi->m_BlueNumStates[m_blue - 1].m_iMissing;
	}
	return m_iBlueMissing;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
CNumSet::CNumSet(const CRagion& ragion)
{
	bool bRes = SetNumSet(ragion);
	ASSERT(bRes);
}

CNumSet::CNumSet(int* pNums, int iCount)
{
	bool bRes = SetNumSet(pNums, iCount);
	ASSERT(bRes);
}

CNumSet::CNumSet(const CString& strText)
{
	SetText(strText);
}

CNumSet::CNumSet(const CNumSet& other)
{
	for (Ragions::const_iterator it = other.m_ValidNumbers.begin();
		it != other.m_ValidNumbers.end(); it ++)
	{
		m_ValidNumbers.push_back(*it);
	}
}

bool CNumSet::SetNumSet(const CRagion& ragion)
{
	m_ValidNumbers.clear();

	m_ValidNumbers.push_back(ragion);
	return true;
}

bool CNumSet::SetNumSet(int* pNums, int iCount)
{
	ASSERT(pNums && iCount > 0);
	if (pNums && iCount > 0)
	{
		m_ValidNumbers.clear();

		CRagion temp;
		for (int i = 0; i < iCount; i ++)
		{
			// must not less than 0.
			ASSERT(*(pNums + i) >= 0);
			if (*(pNums + i) < 0)
			{
				m_ValidNumbers.clear();
				return false;
			}

			if (i == 0)
			{
				// init temp.
				temp.m_iMin = *(pNums + i); // init temp
				temp.m_iStep = 0;
			}
			else
			{
				// must larger than the previous.
				ASSERT(temp.CompareTo(*(pNums + i)) < 0);
				if (temp.CompareTo(*(pNums + i)) < 0)
				{
					m_ValidNumbers.clear();
					return false;
				}

				// does continue?
				if (*(pNums + i) == temp.m_iMin + temp.m_iStep + 1)
				{
					// continue
					temp.m_iStep ++;
				}
				else
				{
					// not continue, submit it.
					m_ValidNumbers.push_back(temp);

					// reset temp
					temp.m_iMin = *(pNums + i);
					temp.m_iStep = 0;
				}
			}
		}

		// submit the last one.
		m_ValidNumbers.push_back(temp);

		return true;
	}
	return false;
}

bool CNumSet::SetNumSet(const CString& str)
{
	m_ValidNumbers.clear();

	int iStart = 0, iEnd = -1, iLength = str.GetLength();
	while (iStart < iLength)
	{
		iEnd = str.Find(',', iStart);
		if (iEnd < 0)
		{
			iEnd = iLength;
		}

		CString strNum = str.Mid(iStart, iEnd - iStart);
		CRagion ragionThis;
		if (ragionThis.SetText(strNum))
		{
			AddNum(ragionThis);
		}

		iStart = iEnd + 1;
	}

	return true;
}

bool CNumSet::SetText(const CString& str)
{
	return SetNumSet(str);
}

CString CNumSet::GetText() const
{
	CString str, strTemp;
	for (Ragions::const_iterator it = m_ValidNumbers.begin(); it != m_ValidNumbers.end(); it ++)
	{
		if (!str.IsEmpty())
			str += _T(",");

		str += it->GetText();		
	}

	return str;
}

bool CNumSet::AddNum(int iNum)
{
	return AddNum(CRagion(iNum));
}

bool CNumSet::AddNum(const CRagion& ragion)
{
	Ragions::const_iterator itPos = m_ValidNumbers.end();
	for (Ragions::const_iterator it = m_ValidNumbers.begin();
		it != m_ValidNumbers.end(); it ++)
	{
		int iComp = it->CompareTo(ragion);

		if (iComp == 0)
			return false;
		else if (iComp > 0)
		{
			itPos = it;
			break;
		}
	}

	if (itPos == m_ValidNumbers.end())
		m_ValidNumbers.push_back(ragion);
	else
		m_ValidNumbers.insert(itPos, ragion);

	return true;
}

bool CNumSet::AddNums(const CNumSet& addition)
{
	bool bRes = false;
	const Ragions nums = addition.GetNums();
	for (Ragions::const_iterator it = nums.begin(); it != nums.end(); ++ it)
	{
		bRes = AddNum(*it);
		if (!bRes)
			return false;
	}

	return true;
}

bool CNumSet::Contains(int iNum) const
{
	for (Ragions::const_iterator it = m_ValidNumbers.begin();
		it != m_ValidNumbers.end(); it ++)
	{
		int iComp = it->CompareTo(iNum);
		if (iComp == 0)
			return true;

		if (iComp > 0)
			return false;
	}

	return false;
}

CNumSet& CNumSet::operator=(const CNumSet& other)
{
	m_ValidNumbers.clear();

	for (Ragions::const_iterator it = other.m_ValidNumbers.begin();
		it != other.m_ValidNumbers.end(); it ++)
	{
		m_ValidNumbers.push_back(*it);
	}

	return *this;
}

int CNumSet::Count() const
{
	int iCount = 0;
	for (Ragions::const_iterator it = m_ValidNumbers.begin(); it != m_ValidNumbers.end(); it ++)
	{
		iCount += it->Count();
	}

	return iCount;
}