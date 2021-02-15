// This is the main DLL file.

#include "stdafx.h"
#include "MatrixCalculatorWrapper.h"
#include "MatrixCalculator\MatrixCalculator.h"
#include "MatrixCalculator\MatrixItemByte.h"

MatrixItem::MatrixItem()
{
}

MatrixItem::MatrixItem(MatrixItem^ other)
	: MatrixItem(other->ToString())
{
}

MatrixItem::MatrixItem(String^ str)
{
	String^ delimStr = " ,\t";
	auto nums = str->Split(delimStr->ToCharArray());
	for each (String^ num in nums)
	{
		_set->Add(Convert::ToInt32(num)); 
	}
}

MatrixItem::~MatrixItem()
{

}

List<int>^ MatrixItem::Numbers()
{
	return _set;
}

int MatrixItem::Count() 
{ 
	return _set->Count;
}

void MatrixItem::Add(int num)
{
	_set->Add(num);
}

String^ MatrixItem::ToString()
{
	String^ str = "";
	for each (auto num in _set)
	{
		if (!String::IsNullOrEmpty(str))
		{
			str += " ";
		}

		str += num.ToString()->PadLeft(2, '0');
	}

	return str;
}

MatrixCalculatorWrapper::MatrixCalculatorWrapper()
{
	m_nativeClient = new MTRxMatrixCalculator();
	m_progress = gcnew List<ThreadStatus^>();
}

MatrixCalculatorWrapper::~MatrixCalculatorWrapper()
{

}

bool MatrixCalculatorWrapper::Calcuate(int row, int col, int algorithm, int betterThan, bool bParallel, bool bReturnForAny, List<MatrixItem^>^ solution)
{
	vector<string> native_solution;
	bool bRes = m_nativeClient->Calcuate(row, col, algorithm, betterThan, bParallel, bReturnForAny, native_solution);
	if (bRes)
	{
		for each (string str in native_solution)
		{
			solution->Add(gcnew MatrixItem(gcnew String(str.c_str())));
		}
	}

	return bRes;
}

List<ThreadStatus^>^ MatrixCalculatorWrapper::GetProgress()
{
	m_progress->Clear();

	auto native_progress = m_nativeClient->GetProgress();
	if (!native_progress.empty())
	{
		auto firstProg = native_progress[0];

		int index = 0;
		for each (auto progress in firstProg.Progress)
		{
			ThreadStatus^ state = gcnew ThreadStatus();
			state->Progress = progress * 100 / firstProg.Total;
			state->Message = Convert::ToString(++ index) + " => " + Convert::ToString(progress);
			m_progress->Add(state);
		}
	}

	return m_progress;
}

bool MatrixCalculatorWrapper::ValidateSolution(int candidateCount, int selectCount, List<MatrixItem^>^ test)
{
	vector<MatrixItemByte*> testBytes;
	for each (auto item in test)
	{
		MatrixItemByte* itemByte = new MatrixItemByte(selectCount);
		auto nums = item->Numbers();
		for each (auto num in nums)
		{
			itemByte->Add(num);
		}
		testBytes.push_back(itemByte);
	}
	
	bool res = MTRxMatrixCalculator::ValidateSolution(candidateCount, selectCount, testBytes);

	for each (auto itemByte in testBytes)
	{
		delete itemByte;
	}

	return res;
}