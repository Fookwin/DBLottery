#pragma once
#include "system.h"
#include <string>
#include <vector>
#include <map>

using namespace std;

class MatrixTable;
class MatrixItemByte;
class MatrixBuildSettings;
class MatrixItemPositionBits;
typedef vector<int> NumberCollection;

class IndexScope
{
public:
	IndexScope();
	IndexScope(int start, int end, const NumberCollection* values = nullptr);
	~IndexScope();

	string ToString() const;
	int Count() const;
	int Min() const;
	int Max() const;
	const NumberCollection* ValueCollection() const;
	int Next() const;

private:
	int MaxValue = -1;
	int MinValue = -1;
	const NumberCollection* Values;
	mutable int* _index = nullptr;
	mutable int* _count = nullptr;
};

enum MatrixResult
{
	Job_Succeeded,
	Job_Succeeded_Continue,
	Job_Failed,
	Job_Aborted,
	User_Aborted
};

class BuildToken
{
public:
	BuildToken(MatrixBuildSettings* settings);
	~BuildToken();

	BuildToken* Clone();

	void RefreshForCommit(int minHitCountForEach, int maxHitCountForEach);

	void UpdateNumCoverage(const MatrixItemByte& item, int minHitCountForEach, int maxHitCountForEach);

	void UpdateItemCoverage(int addItemIndex);

	bool NextItemScope(int curIndex, IndexScope& nextScope);

	int UncoveredNumCount();

	bool IsAllItemsCovered();

	int UncoveredItemCount();

private:
	BuildToken();

	void CloneInts(int* source, int* target, int count);

	MatrixBuildSettings* _settings = nullptr;
	MatrixItemPositionBits* RestItemsBits = nullptr;
	MatrixItemPositionBits* NumBitsToSkip = nullptr;
	int* NumHitCounts = nullptr;
	int UnhitNumCount = -1;
	int NextPosMax = -1;
};

class BuildMatrixUtil
{
public:
	BuildMatrixUtil();
	~BuildMatrixUtil();

	static UINT64 RedBits[33];

	static string RedStrs[33];

	static int BitCount(UINT64 test);

	static void GetTestItemCollection(int candidateCount, int selectCount, vector<MatrixItemByte*>& collection);

private:

	static void GetTestItemCollection(const MatrixItemByte& sel, const vector<BYTE>& candidates, int index, int selectCount, vector<MatrixItemByte*>& output);
};

