#pragma once

/// testes
#define VALID_RED_1				_T("valid_red_1")
#define VALID_RED_2				_T("valid_red_2")
#define VALID_RED_3				_T("valid_red_3")
#define VALID_RED_4				_T("valid_red_4")
#define VALID_RED_5				_T("valid_red_5")
#define VALID_RED_6				_T("valid_red_6")
#define VALID_RED				_T("valid_red")

#define VALID_BLUE				_T("valid_blue")

#define VALID_RED_STEP_12		_T("valid_step_12")
#define VALID_RED_STEP_23		_T("valid_step_23")
#define VALID_RED_STEP_34		_T("valid_step_34")
#define VALID_RED_STEP_45		_T("valid_step_45")
#define VALID_RED_STEP_56		_T("valid_step_56")

#define VALID_SUM				_T("valid_sum")
#define VALID_EVEN_COUNT		_T("valid_even_count")
#define VALID_REPEAT_COUNT		_T("valid_adjacent_count")
#define VALID_CONTINUOUS_COUNT	_T("valid_continuous_count")
#define VALID_PRIME_NUM_COUNT	_T("valid_prime_count")
#define VALID_SMALL_NUM_COUNT	_T("valid_small_count")
#define VALID_TOTAL_MISSING		_T("valid_missing_total")

class Constraint;
class Constraints;
class CConstraintUtil
{
public:
	static Constraint* CreateConstraintInstance(const CString& strDesp);
	static void SuggestConstraints(Constraints* constraints);
};
