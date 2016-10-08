#include "stdafx.h"
#include "ConstraintUtil.h"
#include "..\Data\Constraint\Constraint.h"
#include "..\Data\Constraint\NumberSetConstraint.h"
#include "..\Server\Global.h"
#include "..\Data\Analysis\HistoryData.h"

Constraint* CConstraintUtil::CreateConstraintInstance(const CString& strDesp)
{
	if (strDesp == VALID_RED_1)				return new RedInPositionConstraint(kR1, CNumSet(CRagion(1,27)));
	if (strDesp == VALID_RED_2)				return new RedInPositionConstraint(kR2, CNumSet(CRagion(2,27)));
	if (strDesp == VALID_RED_3)				return new RedInPositionConstraint(kR3, CNumSet(CRagion(3,27)));
	if (strDesp == VALID_RED_4)				return new RedInPositionConstraint(kR4, CNumSet(CRagion(4,27)));
	if (strDesp == VALID_RED_5)				return new RedInPositionConstraint(kR5, CNumSet(CRagion(5,27)));
	if (strDesp == VALID_RED_6)				return new RedInPositionConstraint(kR6, CNumSet(CRagion(6,27)));
	if (strDesp == VALID_RED)				return new RedNumbersConstraint();

	//if (strDesp == VALID_BLUE)				return new RedInPositionConstraint(kB,  CNumSet(CRagion(1,15)));

	if (strDesp == VALID_RED_STEP_12)		return new RedStepConstraint(kR1, kR2, CNumSet(CRagion(1,20)));
	if (strDesp == VALID_RED_STEP_23)		return new RedStepConstraint(kR2, kR3, CNumSet(CRagion(1,20)));
	if (strDesp == VALID_RED_STEP_34)		return new RedStepConstraint(kR3, kR4, CNumSet(CRagion(1,20)));
	if (strDesp == VALID_RED_STEP_45)		return new RedStepConstraint(kR4, kR5, CNumSet(CRagion(1,20)));
	if (strDesp == VALID_RED_STEP_56)		return new RedStepConstraint(kR5, kR6, CNumSet(CRagion(1,20)));

	if (strDesp == VALID_SUM)				return new NumberSumConstraint(CNumSet(CRagion(79,40)));
	if (strDesp == VALID_EVEN_COUNT)		return new EvenCountConstraint(CNumSet(CRagion(0,5)));
	if (strDesp == VALID_REPEAT_COUNT)		return new RepeatCountConstraint(CNumSet(CRagion(0,2)));
	if (strDesp == VALID_CONTINUOUS_COUNT)	return new ContinuityConstraint(CNumSet(CRagion(0,3)));
	if (strDesp == VALID_PRIME_NUM_COUNT)	return new PrimeNumConstraint(CNumSet(CRagion(0,5)));
	if (strDesp == VALID_SMALL_NUM_COUNT)	return new SmallNumConstraint(CNumSet(CRagion(1,3)));
	if (strDesp == VALID_TOTAL_MISSING)		return new TotalMissingConstraint(CNumSet(CRagion(10,20)));

	return NULL;
}

void CConstraintUtil::SuggestConstraints(Constraints* constraints)
{
	if (constraints == NULL)
		return;

	// Get test issue data.
	const CLuckyNum* pPrevious = Lucky::GetHistoryData()->GetTestNum();
	const CNumberCondition* pCondition = Lucky::GetHistoryData()->GetTestCondition();

	// Even...
	int iPreEven = pPrevious->GetEvenNumCount();

	EvenCountConstraint* pEvenCon = dynamic_cast<EvenCountConstraint*>(CreateConstraintInstance(VALID_EVEN_COUNT));
	pEvenCon->SetNumSet(CNumSet(CRagion(iPreEven >= 3 ? 2 : 3, 1)));
	constraints->AddConstraint(VALID_EVEN_COUNT, pEvenCon);
}