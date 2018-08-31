#include "stdafx.h"
#include "MatrixItemByte.h"
#include "BuildMatrixUtil.h"
#include <iterator> 
#include <sstream>

MatrixItemByte::MatrixItemByte()
{
}


MatrixItemByte::~MatrixItemByte()
{
}


MatrixItemByte::MatrixItemByte(int size)
{
	_bitSize = size;
}

MatrixItemByte::MatrixItemByte(int size, UINT64 copyFrom)
{
	_bitSize = size;
	_set = copyFrom;
}

UINT64 MatrixItemByte::GetBits() const
{
	return _set;
}

int MatrixItemByte::GetSize() const
{
	return _bitSize;
}

void MatrixItemByte::Add(int num)
{
	_set |= BuildMatrixUtil::RedBits[num - 1];
}

string MatrixItemByte::ToString() const
{
	string str = "";

	for (int i = 0; i < 33; ++ i)
	{
		if ((_set | BuildMatrixUtil::RedBits[i]) == _set)
		{
			if (str != "")
				str += " ";

			str += BuildMatrixUtil::RedStrs[i];
		}
	}

	return str;
}

int MatrixItemByte::Intersection(MatrixItemByte& compareTo)
{
	UINT64 comp = _set & compareTo._set;
	int hit = 0;
	while (comp != 0)
	{
		++hit;

		comp = comp & (comp - 1);
	}

	return hit;
}
