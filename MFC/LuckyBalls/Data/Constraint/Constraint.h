#pragma once
#include <map>
#include "../LuckyNumber.h"
#include "Utilities/XMLParser.h"

class Constraint
{
public:
	Constraint() : m_bSuppressed(false) {};
	virtual ~Constraint() {};

	virtual bool Meet(const CLuckyNum& lucyNum) const;
	virtual bool ReadFromXml(const XMLNode& node);
	virtual bool WriteToXml(XMLNode& node) const;

	bool IsSuppressed() const { return m_bSuppressed; }
	void SetSuppressed(bool bSuppress) { m_bSuppressed = bSuppress; }
private:
	bool	m_bSuppressed;
};

class Constraints
{
public:
	Constraints();
	~Constraints();

	bool WriteTo(const CString& file) const;
	bool ReadFrom(const CString& file);
	bool AsDefault();
	bool StoreAsDefault() const;
	bool Meet(const CLuckyNum& lucyNum) const;
	bool AddConstraint(const CString& name, Constraint* pConstraint);
	Constraint* GetConstraint(const CString& name) const;
	void Clear();

protected:
	CString GetDefaultFileName() const;
	typedef std::map<CString, Constraint*> ConstraintMap;
	ConstraintMap m_Constraints;
};
