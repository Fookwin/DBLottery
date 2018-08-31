// MatrixCalculatorWrapper.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

class MTRxMatrixCalculator;

public ref class ThreadStatus
{
public:
	property String^ ThreadID;
	property String^ Message;
	property double Progress;
};

public ref class MatrixItem
{
public:
	MatrixItem();
	MatrixItem(MatrixItem^ other);
	MatrixItem(String^ str);
	~MatrixItem();

	int Count();
	void Add(int num);
	List<int>^ Numbers();
	String^ ToString() override;

private:
	List<int>^ _set = gcnew List<int>();
};

public ref class MatrixCalculatorWrapper
{
public:
	MatrixCalculatorWrapper();
	~MatrixCalculatorWrapper();

	bool Build();

	List<ThreadStatus^>^ GetProgress();

	static bool ValidateSolution(int candidateCount, int selectCount, List<MatrixItem^>^ test);

private:
	MTRxMatrixCalculator* m_nativeClient;
	List<ThreadStatus^>^ m_progress;
};
