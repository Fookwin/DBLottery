#pragma once
#include <string>
#include <vector>
#include <map>

using namespace std;

class MatrixTable;
class MatrixItemByte;
class MatrixBuildSettings;
class MatrixItemPositionBits;

class ThreadProgress
{
public:
	string Message;
	double Progress;
};

typedef map<string, ThreadProgress> ThreadProgressSet;


class IndexScope
{
public:
	IndexScope(int start, int end, const vector<int>* values = nullptr);
	~IndexScope();

	string ToString() const;

	int Count();

	int Min() const;

	int Max() const;

	const vector<int>& ValueCollection() const;

	int Next() const;

private:
	int MaxValue = -1;
	int MinValue = -1;
	bool bHasValues = false;
	vector<int> Values;
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

	IndexScope* NextItemScope(int current);

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

