#pragma once
#include "system.h"
#include <string>
#include <vector>

using namespace std;

class MTRxEXPORTS MatrixItemByte
{
public:
	MatrixItemByte(int size);
	MatrixItemByte(int size, UINT64 copyFrom);
	MatrixItemByte(string values);
	~MatrixItemByte();

	UINT64 GetBits() const;
	int GetSize() const;
	void Add(int num);
	string ToString() const;
	int Intersection(MatrixItemByte& compareTo);

private:
	MatrixItemByte();

	UINT64 _set = 0;
	int _bitSize = 0;
};

