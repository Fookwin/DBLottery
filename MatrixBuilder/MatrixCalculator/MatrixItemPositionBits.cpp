#include "stdafx.h"
#include "MatrixItemPositionBits.h"
#include "BuildMatrixUtil.h"

MatrixItemPositionBits::MatrixItemPositionBits()
{
}

MatrixItemPositionBits::~MatrixItemPositionBits()
{
	delete[] Flags;
}

MatrixItemPositionBits::MatrixItemPositionBits(int size, bool asEmpty)
{
	UINT64 init_flag = asEmpty ? 0 : ~(UINT64)0;
	const UINT64 int_1 = (UINT64)1;

	int segmemtCount = size / 64;
	int last = size % 64;

	_flagCount = segmemtCount;
	if (last != 0)
		++_flagCount;

	Flags = new UINT64[_flagCount];
	for (int i = 0; i < segmemtCount; ++i)
	{
		Flags[i] = init_flag;
	}

	// add the last one.
	if (last != 0)
	{
		if (asEmpty)
		{
			Flags[_flagCount - 1] = 0;
		}
		else
		{
			UINT64 temp = 1;
			for (int i = 0; i < last; ++i)
			{
				temp |= int_1 << i;
			}

			Flags[_flagCount - 1] = temp;
		}
	}

	_unhitCount = asEmpty ? 0 : size;
}

void MatrixItemPositionBits::AddSingle(UINT64 test, UINT16 segment)
{
	UINT64 before = Flags[segment];
	Flags[segment] |= test;

	if (before != Flags[segment])
		++_unhitCount;
}

bool MatrixItemPositionBits::Contains(int pos)
{
	int segment = pos / 64;
	UINT64 flag = (UINT64)1 << (pos % 64);

	return (Flags[segment] & flag) != 0;
}

int MatrixItemPositionBits::NextPosition(int previous, bool testHas)
{
	int startSeg = previous / 64;
	for (int i = startSeg; i < _flagCount; ++i)
	{
		UINT64 flag = 1;
		int startBit = i == startSeg ? (previous % 64) + 1 : 0;
		for (int j = startBit; j < 64; ++j)
		{
			if (testHas != ((flag & Flags[i]) == 0))
				return i * 64 + j;

			flag = flag << 1;
		}
	}

	return -1;
}

void MatrixItemPositionBits::AddSingle(int pos)
{
	AddSingle(((UINT64)1 << (pos % 64)), (UINT16)(pos / 64));
}

void MatrixItemPositionBits::AddMultiple(const MatrixItemPositionBits& test)
{
	for (int i = 0; i < _flagCount; ++i)
	{
		if (test.Flags[i] != 0)
			Flags[i] |= test.Flags[i];
	}

	_unhitCount = -1;
}

void MatrixItemPositionBits::RemoveSingle(int pos)
{
	int segment = pos / 64;
	UINT64 flag = (UINT64)1 << (pos % 64);

	UINT64 before = Flags[segment];
	Flags[segment] &= ~flag;

	if (before != Flags[segment])
		--_unhitCount;
}

void MatrixItemPositionBits::RemoveMultiple(const MatrixItemPositionBits& test)
{
	for (int i = 0; i < _flagCount; ++i)
	{
		if (test.Flags[i] != 0)
			Flags[i] &= ~test.Flags[i];
	}

	_unhitCount = -1;
}

MatrixItemPositionBits* MatrixItemPositionBits::Clone()
{
	MatrixItemPositionBits* copy = new MatrixItemPositionBits();
	copy->Flags = new UINT64[this->_flagCount];
	copy->_flagCount = this->_flagCount;
	copy->_unhitCount = this->_unhitCount;

	CloneFlags(Flags, copy->Flags, _flagCount);
	return copy;
}

bool MatrixItemPositionBits::IsClean()
{
	if (_unhitCount == 0)
		return true;

	for (int i = 0; i < _flagCount; ++i)
	{
		if (Flags[i] != 0)
			return false;
	}

	_unhitCount = 0;
	return true;
}

void MatrixItemPositionBits::CloneFlags(UINT64* source, UINT64* target, int count)
{
	// Copy the specified number of bytes from source to target.
	for (int i = 0; i < count; i++)
	{
		*target = *source;
		target++;
		source++;
	}
}

int MatrixItemPositionBits::GetUnhitCount()
{
	if (_unhitCount >= 0)
	return _unhitCount;

	_unhitCount = 0;

	for (int i = 0; i < _flagCount; ++i)
	{
		UINT64 segFlag = Flags[i];
		if (segFlag != 0)
		{
			_unhitCount += BuildMatrixUtil::BitCount(segFlag);
		}
	}

	return _unhitCount;
}
