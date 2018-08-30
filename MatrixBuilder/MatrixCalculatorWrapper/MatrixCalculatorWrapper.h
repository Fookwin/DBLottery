// MatrixCalculatorWrapper.h

#pragma once

#include "MatrixCalculator\MatrixCalculator.h"

using namespace System;
using namespace System::Collections::Generic;

public ref class ThreadProgress
{
public:
	property String^ ThreadID;
	property String^ Message;
	property double Progress;
};

public ref class MatrixCalculatorWrapper
{
public:
	MatrixCalculatorWrapper();
	~MatrixCalculatorWrapper();

	bool Build();

	List<ThreadProgress^>^ GetProgress();

private:
	MTRxMatrixCalculator* m_nativeClient;
	List<ThreadProgress^>^ m_progress;
};
