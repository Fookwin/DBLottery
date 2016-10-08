#include "stdafx.h"
#include "Constraint.h"
#include "Utilities/ConstraintUtil.h"

#define XML_NODE_ROOT					_T("LuckyBalls")
#define XML_NODE_CONSTRAINTS			_T("Constraints")
#define XML_NODE_CONSTRAINT				_T("Constraint")
#define XML_TAG_CONSTRAINT_NAME			_T("InternalName")
#define XML_TAG_SUPPRESSED				_T("Suppressed")

///////////////////////////////////////////////////////////////////////////////////////////////////
bool Constraint::Meet(const CLuckyNum& lucyNum) const
{
	return m_bSuppressed;
}

bool Constraint::ReadFromXml(const XMLNode& node)
{
	node.GetProperty(XML_TAG_SUPPRESSED, m_bSuppressed);
	return true;
}

bool Constraint::WriteToXml(XMLNode& node) const
{
	node.SetProperty(XML_TAG_SUPPRESSED, m_bSuppressed);
	return true;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
Constraints::Constraints()
{
}

Constraints::~Constraints()
{
	Clear();
}

bool Constraints::Meet(const CLuckyNum& luckyNum) const
{
	for (ConstraintMap::const_iterator it = m_Constraints.begin(); it != m_Constraints.end(); ++ it)
	{
		if (!it->second->IsSuppressed() && !it->second->Meet(luckyNum))
			return false;
	}
	
	return true;
}

bool Constraints::AddConstraint(const CString& name, Constraint* pConstraint)
{
	if (m_Constraints.find(name) != m_Constraints.end())
		return false;

	m_Constraints.insert(std::make_pair(name, pConstraint));
	return true;
}

Constraint* Constraints::GetConstraint(const CString& name) const
{
	ConstraintMap::const_iterator it = m_Constraints.find(name);
	if (it != m_Constraints.end())
	{
		return it->second;
	}

	return NULL;
}

bool Constraints::WriteTo(const CString& file) const
{
	XMLParser oParser;
	XMLNode rootNode = oParser.AddNode(XML_NODE_ROOT);
	XMLNode constsNode = rootNode.AddNode(XML_NODE_CONSTRAINTS);

	bool bRes = false;
	for (ConstraintMap::const_iterator it = m_Constraints.begin(); it != m_Constraints.end(); ++ it)
	{
		XMLNode node = constsNode.AddNode(XML_NODE_CONSTRAINT);

		// Add name.
		bRes = node.SetProperty(XML_TAG_CONSTRAINT_NAME, it->first);
		ASSERT(bRes); if (!bRes) return bRes;

		// Write to xml.
		bRes = it->second->WriteToXml(node);
		ASSERT(bRes); if (!bRes) return bRes;
	}

	return oParser.SaveXmlDocument(file, true);
}

bool Constraints::ReadFrom(const CString& file)
{
	XMLParser oParser;
	bool bRes = oParser.LoadXmlDocument(file);
	ASSERT(bRes); if (!bRes) return bRes;

	XMLNode rootNode = oParser.GetRootNode();

	XMLNode constsNode;
	bRes = rootNode.GetChildNode(XML_NODE_CONSTRAINTS, constsNode);
	ASSERT(bRes); if (!bRes) return bRes;

	XMLNodeArray nodes;
	bRes = constsNode.GetChildNodes(XML_NODE_CONSTRAINT, nodes);
	ASSERT(bRes); if (!bRes) return bRes;

	Clear();

	for (INT_PTR i = 0; i < nodes.GetCount(); i ++)
	{
		XMLNode node = nodes.GetAt(i);

		// get name.
		CString strConstraintName;
		bRes = node.GetProperty(XML_TAG_CONSTRAINT_NAME, strConstraintName);
		ASSERT(bRes); if (!bRes) return bRes;

		// Create instance.
		Constraint* pConstraint = CConstraintUtil::CreateConstraintInstance(strConstraintName);
		ASSERT(pConstraint != NULL); if (pConstraint == NULL) return false;

		// Parsing the data.
		bRes = pConstraint->ReadFromXml(node);
		ASSERT(bRes);

		// Add to constraints.
		bRes = AddConstraint(strConstraintName, pConstraint);
		ASSERT(bRes); if (!bRes) return bRes;
	}

	return true;
}

CString Constraints::GetDefaultFileName() const
{
	// get the default setting file.
	CString strDir(::getenv("_LuckyBalls_Dir"));
	CString strDefaultFile = _T("\\Configuration\\DefaultConstraints.xml");
	return strDir + strDefaultFile;
}

bool Constraints::StoreAsDefault() const
{
	return WriteTo(GetDefaultFileName());
}

bool Constraints::AsDefault()
{
	CFileFind fileFinder; 
	BOOL bRetVal; 
	bRetVal = fileFinder.FindFile(GetDefaultFileName(),0);
	if (bRetVal)
	{
		return ReadFrom(GetDefaultFileName());
	}
	else
	{
		Clear();

		//VALID_RED_1
		if (!AddConstraint(VALID_RED_1, CConstraintUtil::CreateConstraintInstance(VALID_RED_1)))
			return false;

		//VALID_RED_2
		if (!AddConstraint(VALID_RED_2, CConstraintUtil::CreateConstraintInstance(VALID_RED_2)))
			return false;

		//VALID_RED_3
		if (!AddConstraint(VALID_RED_3, CConstraintUtil::CreateConstraintInstance(VALID_RED_3)))
			return false;

		//VALID_RED_4
		if (!AddConstraint(VALID_RED_4, CConstraintUtil::CreateConstraintInstance(VALID_RED_4)))
			return false;

		//VALID_RED_5
		if (!AddConstraint(VALID_RED_5, CConstraintUtil::CreateConstraintInstance(VALID_RED_5)))
			return false;

		//VALID_RED_6
		if (!AddConstraint(VALID_RED_6, CConstraintUtil::CreateConstraintInstance(VALID_RED_6)))
			return false;

		//VALID_RED
		if (!AddConstraint(VALID_RED, CConstraintUtil::CreateConstraintInstance(VALID_RED)))
			return false;

		//VALID_BLUE
		//if (!AddConstraint(VALID_BLUE, CConstraintUtil::CreateConstraintInstance(VALID_BLUE)))
		//	return false;

		//VALID_RED_STEP_12
		if (!AddConstraint(VALID_RED_STEP_12, CConstraintUtil::CreateConstraintInstance(VALID_RED_STEP_12)))
			return false;

		//VALID_RED_STEP_23
		if (!AddConstraint(VALID_RED_STEP_23, CConstraintUtil::CreateConstraintInstance(VALID_RED_STEP_23)))
			return false;

		//VALID_RED_STEP_34
		if (!AddConstraint(VALID_RED_STEP_34, CConstraintUtil::CreateConstraintInstance(VALID_RED_STEP_34)))
			return false;

		//VALID_RED_STEP_45
		if (!AddConstraint(VALID_RED_STEP_45, CConstraintUtil::CreateConstraintInstance(VALID_RED_STEP_45)))
			return false;

		//VALID_RED_STEP_56
		if (!AddConstraint(VALID_RED_STEP_56, CConstraintUtil::CreateConstraintInstance(VALID_RED_STEP_56)))
			return false;

		//VALID_SUM
		if (!AddConstraint(VALID_SUM, CConstraintUtil::CreateConstraintInstance(VALID_SUM)))
			return false;

		//VALID_EVEN_COUNT
		if (!AddConstraint(VALID_EVEN_COUNT, CConstraintUtil::CreateConstraintInstance(VALID_EVEN_COUNT)))
			return false;

		//VALID_REPEAT_COUNT
		if (!AddConstraint(VALID_REPEAT_COUNT, CConstraintUtil::CreateConstraintInstance(VALID_REPEAT_COUNT)))
			return false;

		//VALID_CONTINUOUS_COUNT
		if (!AddConstraint(VALID_CONTINUOUS_COUNT, CConstraintUtil::CreateConstraintInstance(VALID_CONTINUOUS_COUNT)))
			return false;

		//VALID_SMALL_NUM_COUNT
		if (!AddConstraint(VALID_SMALL_NUM_COUNT, CConstraintUtil::CreateConstraintInstance(VALID_SMALL_NUM_COUNT)))
			return false;

		//VALID_PRIME_NUM_COUNT
		if (!AddConstraint(VALID_PRIME_NUM_COUNT, CConstraintUtil::CreateConstraintInstance(VALID_PRIME_NUM_COUNT)))
			return false;

		//VALID_TOTAL_MISSING
		if (!AddConstraint(VALID_TOTAL_MISSING, CConstraintUtil::CreateConstraintInstance(VALID_TOTAL_MISSING)))
			return false;
	}

	return false;
}

void Constraints::Clear()
{
	for (ConstraintMap::iterator it = m_Constraints.begin(); it != m_Constraints.end(); ++ it)
		delete it->second;

	m_Constraints.clear();
}