#pragma once
#include <vector>
#include "MatrixItemPositionBits.h"
#include "MatrixItemByte.h"
#include "BuildMatrixUtil.h"


using namespace std;

class NumberDistribution
{
public:
	UINT64 Bits = 0;
	MatrixItemPositionBits* Distribution = nullptr;
	int MinIndex = -1;
	int MaxIndex = -1;
};

class MatrixTestItem
{
public:
	MatrixItemByte* ItemByte = nullptr;
	MatrixItemPositionBits* CoverageMash = nullptr;
	NumberCollection CoveredBy;
};

class MatrixBuildSettings
{
public:
	MatrixBuildSettings(int _candidateNumCount, int _selectNumCount);
	~MatrixBuildSettings();	

	const NumberDistribution& NumDistribution(int index) const;
	const MatrixItemByte& TestItem(int index) const;
	int TestItemCount() const;
	const MatrixItemPositionBits& TestItemMash(int index) const;
	const NumberCollection& TestItemCoveredBy(int index) const;

	int CandidateNumCount = 0;  // the count of the numbers could be selected from.
	int SelectNumCount = 0;     // the count of the numbers to be selected.
	int MaxItemCountCoveredByOneItem = 0;
	int IdealMinItemCount = 0;

private:
	
	void BuildTestItems(int candidateNumCount, int selectNumCount);
	void BuildNumDistributions();

	vector<MatrixTestItem*> TestItemCollection;
	vector<NumberDistribution*> NumDistributions;
};

