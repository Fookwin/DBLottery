#include "stdafx.h"
#include "RedNumConstraint.h"

RedNumConstraint::RedNumConstraint(NumPos iPos, const CNumSet& set)
: Constraint(), m_iNumPos(iPos), m_ValidNumbers(set)
{
	ASSERT(m_iNumPos >= kR && m_iNumPos <= kB);
}

bool RedNumConstraint::Meet(const CLuckyNum& num) const
{
	if (m_iNumPos < 0)
	{
		for (int i = kR1; i <= kR6; i ++)
		{
			if (!m_ValidNumbers.Contains(num.m_red[i]))
				return false;
		}

		return true;
	}
	else
	{
		ASSERT(m_iNumPos >= 0 && m_iNumPos <= 6);
		if (m_iNumPos < kR1 || m_iNumPos > kB)
			return false;

		if (m_iNumPos <)
	}

	return m_ValidNumbers.Contains(m_iNumPos <= kR6 ? num.m_red[m_iNumPos] : num.m_blue);
}

bool RedNumConstraint::SetNumSet(const CNumSet& set)
{
	m_ValidNumbers = set;
	return true;
}

const CNumSet& RedNumConstraint::GetNumSet() const
{
	return m_ValidNumbers;
}
