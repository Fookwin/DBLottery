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

bool MatrixCalculatorWrapper::Build()
{
	return m_nativeClient->Build();
}

List<ThreadStatus^>^ MatrixCalculatorWrapper::GetProgress()
{
	m_progress->Clear();

	map<string, MTRxMatrixCalculator::State>* native_progress = m_nativeClient->GetProgress();
	
	for (map<string, MTRxMatrixCalculator::State>::const_iterator it = native_progress->begin(); it != native_progress->end(); ++it)
	{
		ThreadStatus^ state = gcnew ThreadStatus();
		state->ThreadID = gcnew String(it->second.ThreadID.c_str());
		state->Message = gcnew String(it->second.Message.c_str());
		state->Progress = it->second.Progress;

		m_progress->Add(state);
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