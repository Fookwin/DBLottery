#pragma once
#include <map>
#include "Constraint.h"

#define XML_NODE_NUMBER_SET			_T("NumberSet")
#define XML_NODE_RESTRICT_SET		_T("RestrictSet")
#define XML_TAG_NUMBER_SET_VALUE	_T("Value")
#define XML_TAG_RESTRICT_SET_VALUE	_T("Value")

#define DECLARE_SIMPLE_NUM_SET_CONSTRAINT(class_name) \
class class_name : public Constraint \
{ \
public: \
	class_name(const CNumSet& set); \
	virtual ~class_name() {}; \
	virtual bool		Meet(const CLuckyNum& lucyNum) const; \
	virtual bool		ReadFromXml(const XMLNode& node); \
	virtual bool		WriteToXml(XMLNode& node) const; \
	void				SetNumSet(const CNumSet& set) {m_ValidNumbers = set;} \
	const CNumSet&		GetNumSet() const { return m_ValidNumbers; } \
protected: \
	CNumSet				m_ValidNumbers; \
};

#define IMPLEMENT_SIMPLE_NUM_SET_CONSTRAINT(class_name) \
class_name::class_name(const CNumSet& set)\
: Constraint(), m_ValidNumbers(set){\
}\
bool class_name::ReadFromXml(const XMLNode& node){\
	bool bRes = __super::ReadFromXml(node);\
	ASSERT(bRes); if (!bRes) return bRes;\
	XMLNode numberNode;\
	bRes = node.GetChildNode(XML_NODE_NUMBER_SET, numberNode);\
	ASSERT(bRes); if (!bRes) return bRes;\
	CString strNumSet;\
	bRes = numberNode.GetProperty(XML_TAG_NUMBER_SET_VALUE, strNumSet);\
	ASSERT(bRes); if (!bRes) return bRes;\
	bRes = m_ValidNumbers.SetText(strNumSet);\
	ASSERT(bRes); if (!bRes) return bRes;\
	return true;\
}\
bool class_name::WriteToXml(XMLNode& node) const{\
	bool bRes = __super::WriteToXml(node);\
	ASSERT(bRes); if (!bRes) return bRes;\
	XMLNode numberNode = node.AddNode(XML_NODE_NUMBER_SET);\
	bRes = numberNode.SetProperty(XML_TAG_NUMBER_SET_VALUE, m_ValidNumbers.GetText());\
	ASSERT(bRes); if (!bRes) return bRes;\
	return true;\
}

///////////////////////////////////////////////////////////////////////////////////////////////////
enum NumIndexEnum
{
	kR1 = 0,
	kR2,
	kR3,
	kR4,
	kR5,
	kR6
};

class RedInPositionConstraint : public Constraint
{
public:
	RedInPositionConstraint(NumIndexEnum iPos, const CNumSet& set);
	virtual ~RedInPositionConstraint() {};

	virtual bool		Meet(const CLuckyNum& lucyNum) const;
	virtual bool		ReadFromXml(const XMLNode& node);
	virtual bool		WriteToXml(XMLNode& node) const;

	void				SetNumSet(const CNumSet& set) {m_ValidNumbers = set;}
	const CNumSet&		GetNumSet() const { return m_ValidNumbers; }

protected:
	CNumSet				m_ValidNumbers;
	NumIndexEnum		m_eNumIndex;
};

///////////////////////////////////////////////////////////////////////////////////////////////////
class RedNumbersConstraint : public Constraint
{
	struct Restrict
	{
		CNumSet set;
		CRagion sel;
	};

	typedef std::vector<Restrict> Restricts;
public:
	RedNumbersConstraint();
	virtual ~RedNumbersConstraint() {};

	virtual bool		Meet(const CLuckyNum& lucyNum) const;
	virtual bool		ReadFromXml(const XMLNode& node);
	virtual bool		WriteToXml(XMLNode& node) const;

	CString				GetText() const;
	bool				SetText(const CString& str);

	bool				AddRestrict(const CNumSet& set, const CRagion& selCount);
	bool				GetRestrict(int iIndex, CNumSet& set, CRagion& selCount) const;
	int					GetRestrictCount() const { return (int)m_Restricts.size(); }

protected:
	Restricts			m_Restricts;
};

///////////////////////////////////////////////////////////////////////////////////////////////////
class RedStepConstraint : public Constraint
{
public:
	RedStepConstraint(NumIndexEnum iMin, NumIndexEnum iMax, const CNumSet& set);
	virtual ~RedStepConstraint() {};

	virtual bool		Meet(const CLuckyNum& lucyNum) const;
	virtual bool		ReadFromXml(const XMLNode& node);
	virtual bool		WriteToXml(XMLNode& node) const;

	void				SetNumSet(const CNumSet& set) {m_ValidNumbers = set;}
	const CNumSet&		GetNumSet() const { return m_ValidNumbers; }

protected:
	CNumSet				m_ValidNumbers;
	NumIndexEnum		m_eMinNumIndex;
	NumIndexEnum		m_eMaxNumIndex;
};

///////////////////////////////////////////////////////////////////////////////////////////////////
DECLARE_SIMPLE_NUM_SET_CONSTRAINT(NumberSumConstraint)
DECLARE_SIMPLE_NUM_SET_CONSTRAINT(EvenCountConstraint)
DECLARE_SIMPLE_NUM_SET_CONSTRAINT(ContinuityConstraint)
DECLARE_SIMPLE_NUM_SET_CONSTRAINT(SmallNumConstraint)
DECLARE_SIMPLE_NUM_SET_CONSTRAINT(PrimeNumConstraint)
DECLARE_SIMPLE_NUM_SET_CONSTRAINT(RepeatCountConstraint)
DECLARE_SIMPLE_NUM_SET_CONSTRAINT(TotalMissingConstraint)
