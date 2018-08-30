// MatrixCalculator.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "MatrixCalculator.h"

MTRxMatrixCalculator::MTRxMatrixCalculator()
{

}

MTRxMatrixCalculator::~MTRxMatrixCalculator()
{
	delete m_progress;
}

bool MTRxMatrixCalculator::Build()
{
	m_progress = new map<string, MTRxMatrixCalculator::State>();

	for (int i = 0; i < 10; ++i)
	{
		string thread = "thread" + std::to_string(i);
		MTRxMatrixCalculator::State st;
		st.ThreadID = thread;
		st.Progress = 0;
		st.Message = thread;

		m_progress->insert(std::make_pair(thread, st));

		for (int y = 0; y < 10; ++y)
		{
			Sleep(500);
			(*m_progress)[thread].Progress += 10;
		}
	}

	return false;
}

map<string, MTRxMatrixCalculator::State>* MTRxMatrixCalculator::GetProgress() const
{
	return m_progress;
}
