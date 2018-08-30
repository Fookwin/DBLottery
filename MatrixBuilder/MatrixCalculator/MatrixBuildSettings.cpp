#include "stdafx.h"
#include "MatrixBuildSettings.h"
#include "MatrixItemByte.h"
#include "BuildMatrixUtil.h"

MatrixBuildSettings::~MatrixBuildSettings()
{
	for each (auto var in TestItemCollection)
	{
		delete var->CoverageMash;
		delete var->ItemByte;
		delete var;
	}

	for each (auto var in NumDistributions)
	{
		delete var->Distribution;
		delete var;
	}
}

MatrixBuildSettings::MatrixBuildSettings(int _candidateNumCount, int _selectNumCount)
{
	SelectNumCount = _selectNumCount;
	CandidateNumCount = _candidateNumCount;

	// Get the test items.
	BuildTestItems(_candidateNumCount, _selectNumCount);

	BuildNumDistributions();

	MaxItemCountCoveredByOneItem = (_candidateNumCount - _selectNumCount) * _selectNumCount + 1;

	IdealMinItemCount = (static_cast<int>(TestItemCollection.size()) - 1) / MaxItemCountCoveredByOneItem + 1;
}

const NumberDistribution& MatrixBuildSettings::NumDistribution(int index) const
{
	return *NumDistributions[index];
}

const MatrixItemByte& MatrixBuildSettings::TestItem(int index) const
{
	return *TestItemCollection[index]->ItemByte;
}

int MatrixBuildSettings::TestItemCount() const
{
	return static_cast<int>(TestItemCollection.size());
}

const MatrixItemPositionBits& MatrixBuildSettings::TestItemMash(int index) const
{
	return *TestItemCollection[index]->CoverageMash;
}

const vector<int>& MatrixBuildSettings::TestItemCoveredBy(int index) const
{
	return TestItemCollection[index]->CoveredBy;
}

void MatrixBuildSettings::BuildTestItems(int candidateNumCount, int selectNumCount)
{
	vector<MatrixItemByte*> itemByteCollection;
	BuildMatrixUtil::GetTestItemCollection(candidateNumCount, selectNumCount, itemByteCollection);

	int matchNumCount = SelectNumCount - 1;

	int count = static_cast<int>(itemByteCollection.size());
	for (int i = 0; i < count; ++i)
	{
		auto item = new MatrixTestItem();
		item->ItemByte = itemByteCollection[i];
		item->CoverageMash = new MatrixItemPositionBits(count, true);

		TestItemCollection.push_back(item);
	}

	for (int i = 0; i < count; ++i)
	{
		MatrixItemByte* itemA = itemByteCollection[i];

		// Init and Add itself.
		TestItemCollection[i]->CoverageMash->AddSingle(i);
		TestItemCollection[i]->CoveredBy.push_back(i);

		for (int j = i + 1; j < count; ++j)
		{
			MatrixItemByte* itemB = itemByteCollection[j];
			if (itemA->Intersection(*itemB) >= matchNumCount)
			{
				TestItemCollection[i]->CoverageMash->AddSingle(j);
				TestItemCollection[i]->CoveredBy.push_back(j);
				TestItemCollection[j]->CoverageMash->AddSingle(i);
				TestItemCollection[j]->CoveredBy.push_back(i);
			}
		}
	}
}

void MatrixBuildSettings::BuildNumDistributions()
{
	int testItemCount = static_cast<int>(TestItemCollection.size());

	UINT64 testBit = 1;

	for (int i = 0; i < CandidateNumCount; ++i)
	{
		auto dist = new NumberDistribution();
		dist->Bits = testBit;
		dist->Distribution = new MatrixItemPositionBits(testItemCount, true);
		dist->MaxIndex = 0;
		dist->MinIndex = 0;
		NumDistributions.push_back(dist);

		testBit = testBit << 1;
	}

	for (int i = 0; i < testItemCount; ++i)
	{
		MatrixItemByte* item = TestItemCollection[i]->ItemByte;
		for (int j = 0; j < CandidateNumCount; ++j)
		{
			if ((item->GetBits() & NumDistributions[j]->Bits) != 0)
			{
				NumDistributions[j]->Distribution->AddSingle(i);
				NumDistributions[j]->MinIndex = NumDistributions[j]->MaxIndex < 0 ? i : NumDistributions[j]->MinIndex; // set the min as the first time the number hit.
				NumDistributions[j]->MaxIndex = i; // always set the max to get it to be the last index of the number hit.
			}
		}
	}
}
