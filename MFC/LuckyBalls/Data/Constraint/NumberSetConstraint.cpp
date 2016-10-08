#include "stdafx.h"
#include "Server\Global.h"
#include "Data\Analysis\HistoryData.h"
#include "NumberSetConstraint.h"

#define XML_TAG_NUM_INDEX			_T("NumberIndex")
#define XML_TAG_MIN_NUM_INDEX		_T("Min_NumberIndex")
#define XML_TAG_MAX_NUM_INDEX		_T("Max_NumberIndex")

///////////////////////////////////////////////////////////////////////////////////////////////////
RedInPositionConstraint::RedInPositionConstraint(NumIndexEnum iPos, const CNumSet& set)
: Constraint(), m_eNumIndex(iPos), m_ValidNumbers(set)
{
}

bool RedInPositionConstraint::Meet(const CLuckyNum& num) const
{
	return m_ValidNumbers.Contains(num.m_red[m_eNumIndex]);
}

bool RedInPositionConstraint::ReadFromXml(const XMLNode& node)
{
	bool bRes = __super::ReadFromXml(node);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = node.GetProperty(XML_TAG_NUM_INDEX, (int&)m_eNumIndex);
	ASSERT(bRes); if (!bRes) return bRes;

	XMLNode numberNode;
	bRes = node.GetChildNode(XML_NODE_NUMBER_SET, numberNode);
	ASSERT(bRes); if (!bRes) return bRes;

	CString strNumSet;
	bRes = numberNode.GetProperty(XML_TAG_NUMBER_SET_VALUE, strNumSet);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = m_ValidNumbers.SetText(strNumSet);
	ASSERT(bRes); if (!bRes) return bRes;

	return true;
}

bool RedInPositionConstraint::WriteToXml(XMLNode& node) const
{
	bool bRes = __super::WriteToXml(node);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = node.SetProperty(XML_TAG_NUM_INDEX, (int)m_eNumIndex);
	ASSERT(bRes); if (!bRes) return bRes;

	XMLNode numberNode = node.AddNode(XML_NODE_NUMBER_SET);
	bRes = numberNode.SetProperty(XML_TAG_NUMBER_SET_VALUE, m_ValidNumbers.GetText());
	ASSERT(bRes); if (!bRes) return bRes;

	return true;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
RedStepConstraint::RedStepConstraint(NumIndexEnum iMin, NumIndexEnum iMax, const CNumSet& set)
: Constraint(), m_eMinNumIndex(iMin), m_eMaxNumIndex(iMax), m_ValidNumbers(set)
{
	ASSERT(m_eMinNumIndex >= kR1 && m_eMinNumIndex <= kR6 &&
		m_eMaxNumIndex >= kR1 && m_eMaxNumIndex <= kR6 && m_eMaxNumIndex > m_eMinNumIndex);
}

bool RedStepConstraint::Meet(const CLuckyNum& num) const
{
	ASSERT(m_eMinNumIndex >= kR1 && m_eMinNumIndex <= kR6 &&
		m_eMaxNumIndex >= kR1 && m_eMaxNumIndex <= kR6 && m_eMaxNumIndex > m_eMinNumIndex);

	if (m_eMinNumIndex >= kR1 && m_eMinNumIndex <= kR6 &&
		m_eMaxNumIndex >= kR1 && m_eMaxNumIndex <= kR6 &&
		m_eMaxNumIndex > m_eMinNumIndex)
	{
		return m_ValidNumbers.Contains(num.m_red[m_eMaxNumIndex] - num.m_red[m_eMinNumIndex]);
	}
	else
		return false;
}

bool RedStepConstraint::ReadFromXml(const XMLNode& node)
{
	bool bRes = __super::ReadFromXml(node);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = node.GetProperty(XML_TAG_MIN_NUM_INDEX, (int&)m_eMinNumIndex);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = node.GetProperty(XML_TAG_MAX_NUM_INDEX, (int&)m_eMaxNumIndex);
	ASSERT(bRes); if (!bRes) return bRes;

	XMLNode numberNode;
	bRes = node.GetChildNode(XML_NODE_NUMBER_SET, numberNode);
	ASSERT(bRes); if (!bRes) return bRes;

	CString strNumSet;
	bRes = numberNode.GetProperty(XML_TAG_NUMBER_SET_VALUE, strNumSet);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = m_ValidNumbers.SetText(strNumSet);
	ASSERT(bRes); if (!bRes) return bRes;

	return true;
}

bool RedStepConstraint::WriteToXml(XMLNode& node) const
{
	bool bRes = __super::WriteToXml(node);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = node.SetProperty(XML_TAG_MIN_NUM_INDEX, (int)m_eMinNumIndex);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = node.SetProperty(XML_TAG_MAX_NUM_INDEX, (int)m_eMaxNumIndex);
	ASSERT(bRes); if (!bRes) return bRes;

	XMLNode numberNode = node.AddNode(XML_NODE_NUMBER_SET);
	bRes = numberNode.SetProperty(XML_TAG_NUMBER_SET_VALUE, m_ValidNumbers.GetText());
	ASSERT(bRes); if (!bRes) return bRes;

	return true;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
RedNumbersConstraint::RedNumbersConstraint()
: Constraint()
{
}

bool RedNumbersConstraint::Meet(const CLuckyNum& num) const
{
	for (Restricts::const_iterator it = m_Restricts.begin(); it != m_Restricts.end(); it ++)
	{
		int iHit = 0;
		for (int i = 0; i < 6; i ++)
		{
			if (it->set.Contains(num.m_red[i]))
				iHit ++;
		}

		if (it->sel.CompareTo(iHit) != 0)
			return false;
	}

	return true;
}

bool RedNumbersConstraint::ReadFromXml(const XMLNode& node)
{
	bool bRes = __super::ReadFromXml(node);
	ASSERT(bRes); if (!bRes) return bRes;

	XMLNode numberNode;
	bRes = node.GetChildNode(XML_NODE_RESTRICT_SET, numberNode);
	ASSERT(bRes); if (!bRes) return bRes;

	CString strNumSet;
	bRes = numberNode.GetProperty(XML_TAG_RESTRICT_SET_VALUE, strNumSet);
	ASSERT(bRes); if (!bRes) return bRes;

	bRes = SetText(strNumSet);
	ASSERT(bRes); if (!bRes) return bRes;

	return true;
}

bool RedNumbersConstraint::WriteToXml(XMLNode& node) const
{
	bool bRes = __super::WriteToXml(node);
	ASSERT(bRes); if (!bRes) return bRes;

	XMLNode numberNode = node.AddNode(XML_NODE_RESTRICT_SET);
	bRes = numberNode.SetProperty(XML_TAG_RESTRICT_SET_VALUE, GetText());
	ASSERT(bRes); if (!bRes) return bRes;

	return true;
}

bool RedNumbersConstraint::AddRestrict(const CNumSet& set, const CRagion& selCount)
{
	Restrict restrict;
	restrict.set = set;
	restrict.sel = selCount;

	m_Restricts.push_back(restrict);
	return true;
}

bool RedNumbersConstraint::GetRestrict(int iIndex, CNumSet& set, CRagion& selCount) const
{
	if (iIndex < 0 || iIndex > (int)m_Restricts.size())
		return false;

	set = m_Restricts[iIndex].set;
	selCount = m_Restricts[iIndex].sel;

	return true;
}

CString RedNumbersConstraint::GetText() const
{
	CString str;
	for (Restricts::const_iterator it = m_Restricts.begin(); it != m_Restricts.end(); it ++)
	{
		if (!str.IsEmpty())
			str += _T(";");

		str += it->sel.GetText();
		str += _T("|");
		str += it->set.GetText();
	}
	return str;
}

bool RedNumbersConstraint::SetText(const CString& str)
{
	m_Restricts.clear();

	int iStart = 0, iEnd = -1, iLength = str.GetLength();
	while (iStart < iLength)
	{
		iEnd = str.Find(';', iStart);
		if (iEnd < 0)
		{
			iEnd = iLength;
		}

		int iBreak = str.Find('|', iStart);
		if (iBreak < 0)
			return false;

		Restrict restrict;
		CString strNum = str.Mid(iStart, iBreak - iStart);
		if (!restrict.sel.SetText(strNum))
			return false;

		if (!restrict.set.SetText(str.Mid(iBreak + 1, iEnd - iBreak)))
			return false;

		m_Restricts.push_back(restrict);

		iStart = iEnd + 1;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
IMPLEMENT_SIMPLE_NUM_SET_CONSTRAINT(NumberSumConstraint);

bool NumberSumConstraint::Meet(const CLuckyNum& num) const
{
	return m_ValidNumbers.Contains(num.GetSum());
}

///////////////////////////////////////////////////////////////////////////////////////////////////
IMPLEMENT_SIMPLE_NUM_SET_CONSTRAINT(EvenCountConstraint);

bool EvenCountConstraint::Meet(const CLuckyNum& num) const
{
	return m_ValidNumbers.Contains(num.GetEvenNumCount());
}

///////////////////////////////////////////////////////////////////////////////////////////////////
IMPLEMENT_SIMPLE_NUM_SET_CONSTRAINT(ContinuityConstraint);

bool ContinuityConstraint::Meet(const CLuckyNum& num) const
{
	return m_ValidNumbers.Contains(num.GetContinuity());
}

///////////////////////////////////////////////////////////////////////////////////////////////////
IMPLEMENT_SIMPLE_NUM_SET_CONSTRAINT(SmallNumConstraint);

bool SmallNumConstraint::Meet(const CLuckyNum& num) const
{
	return m_ValidNumbers.Contains(num.GetSmallNumCount());
}

///////////////////////////////////////////////////////////////////////////////////////////////////
IMPLEMENT_SIMPLE_NUM_SET_CONSTRAINT(PrimeNumConstraint);

bool PrimeNumConstraint::Meet(const CLuckyNum& num) const
{
	return m_ValidNumbers.Contains(num.GetPirmeNumCount());
}

///////////////////////////////////////////////////////////////////////////////////////////////////
IMPLEMENT_SIMPLE_NUM_SET_CONSTRAINT(RepeatCountConstraint);

bool RepeatCountConstraint::Meet(const CLuckyNum& num) const
{
	return m_ValidNumbers.Contains(num.GetRepeat());
}

///////////////////////////////////////////////////////////////////////////////////////////////////
IMPLEMENT_SIMPLE_NUM_SET_CONSTRAINT(TotalMissingConstraint);

bool TotalMissingConstraint::Meet(const CLuckyNum& num) const
{
	return m_ValidNumbers.Contains(num.GetRedMissing());
}
