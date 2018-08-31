// MatrixCalculator.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "MatrixCalculator.h"
#include "MatrixBuildSettings.h"

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

bool MTRxMatrixCalculator::ValidateSolution(int candidateCount, int selectCount, const vector<MatrixItemByte*>& test)
{
	MatrixBuildSettings settings(candidateCount, selectCount);

	MatrixItemPositionBits restItemsBits(settings.TestItemCount(), false);

	// Include the preselected items.
	int startIndex = 0;
	for each(MatrixItemByte* item in test)
	{
		for (int i = startIndex; i < settings.TestItemCount(); ++i)
		{
			if (settings.TestItem(i).GetBits() == item->GetBits())
			{
				restItemsBits.RemoveMultiple(settings.TestItemMash(i));
				startIndex = i + 1;
				break;
			}
		}
	}

	return restItemsBits.GetUnhitCount() == 0;
}
