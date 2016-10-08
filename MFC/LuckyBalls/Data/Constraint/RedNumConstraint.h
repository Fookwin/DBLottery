#pragma once
#include <map>
#include "Constraint.h"

class RedNumConstraint : public Constraint
{
public:
	/**
	 * the position of the red num to be checked, set -1 to check all
	 */
	RedNumConstraint(const CNumSet& set, int iRedPos = -1);
	virtual ~RedNumConstraint() {};

	virtual bool		Meet(const CLuckyNum& lucyNum) const;

	bool				SetNumSet(const CNumSet& set);
	const CNumSet&		GetNumSet() const;

protected:
	CNumSet				m_ValidNumbers;
	int					m_iNumPos;
};

