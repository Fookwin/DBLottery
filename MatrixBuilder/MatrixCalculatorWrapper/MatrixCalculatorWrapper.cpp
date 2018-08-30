// This is the main DLL file.

#include "stdafx.h"

#include "MatrixCalculatorWrapper.h"


MatrixCalculatorWrapper::MatrixCalculatorWrapper()
{
	m_nativeClient = new MTRxMatrixCalculator();
	m_progress = gcnew List<ThreadProgress^>();
}

MatrixCalculatorWrapper::~MatrixCalculatorWrapper()
{

}

bool MatrixCalculatorWrapper::Build()
{
	return m_nativeClient->Build();
}

List<ThreadProgress^>^ MatrixCalculatorWrapper::GetProgress()
{
	m_progress->Clear();

	map<string, MTRxMatrixCalculator::State>* native_progress = m_nativeClient->GetProgress();
	
	for (map<string, MTRxMatrixCalculator::State>::const_iterator it = native_progress->begin(); it != native_progress->end(); ++it)
	{
		ThreadProgress^ state = gcnew ThreadProgress();
		state->ThreadID = gcnew String(it->second.ThreadID.c_str());
		state->Message = gcnew String(it->second.Message.c_str());
		state->Progress = it->second.Progress;

		m_progress->Add(state);
	}

	return m_progress;
}