// MatrixCalculator.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "MatrixCalculator.h"
#include "MatrixBuildSettings.h"
#include "ExhaustionAlgorithmImpl.h"

MTRxMatrixCalculator::MTRxMatrixCalculator()
{

}

MTRxMatrixCalculator::~MTRxMatrixCalculator()
{
}

bool MTRxMatrixCalculator::Calcuate(int row, int col, int algorithm, int betterThan, bool bParallel, bool bReturnForAny, vector<string>& solution)
{
	MatrixBuildSettings settings(row, col);

	ExhaustionAlgorithmImpl impl(&settings);
	impl.Calculate(betterThan - 1, m_progress, bReturnForAny, bParallel);

	auto sol = impl.GetSolution();
	for each (auto var in sol)
	{
		solution.push_back(var->ToString());
	}

	return true;
}

const ThreadProgressSet& MTRxMatrixCalculator::GetProgress() const
{
	return m_progress;
}

void MTRxMatrixCalculator::Abort()
{
	for (auto & progress : m_progress)
	{
		progress.Aborted = true;
	}
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
