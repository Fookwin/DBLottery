#include "stdafx.h"
#include "BuildMatrixUtil.h"
#include "MatrixItemPositionBits.h"
#include "MatrixBuildSettings.h"

UINT64 BuildMatrixUtil::RedBits[33] =
{
	1, 2,4,8,16,32,64,128,256,512,
	1024,2048,4096,8192,16384,32768, 65536,131072,262144, 524288,
	1048576,2097152,4194304,8388608, 16777216,33554432,67108864,134217728,268435456,
	536870912,1073741824, 2147483648,4294967296
};

string BuildMatrixUtil::RedStrs[33] =
{
	"01","02","03","04","05","06","07","08","09","10","11",
	"12","13","14","15","16","17","18","19","20","21","22",
	"23","24","25","26","27","28","29","30","31","32","33"
};

IndexScope::IndexScope()
{

}

IndexScope::IndexScope(int start, int end, const NumberCollection* values /*= nullptr*/)
{
	Reset(start, end, values);
}

IndexScope::~IndexScope()
{
	delete _count;
	_count = nullptr;
	delete _index;
	_index = nullptr;
}

void IndexScope::Reset(int start, int end, const NumberCollection* values /*= nullptr*/)
{
	MinValue = start;
	MaxValue = end;
	Values = values;

	delete _count;
	_count = nullptr;
	delete _index;
	_index = nullptr;
}

string IndexScope::ToString() const
{
	return "(" + std::to_string(MinValue) + ", " + std::to_string(MaxValue) + ")";
}

int IndexScope::Count() const
{
	if (!Values)
		return MaxValue - MinValue + 1;
	else if (_count)
		return *_count;
	else
	{
		int count = 0;
		for (size_t i = 0; i < Values->size(); ++i)
		{
			if ((*Values)[i] > MaxValue)
				break;

			if ((*Values)[i] >= MinValue)
			{
				++count;
			}
		}

		_count = new int(count); // cache it.

		return count;
	}
}

int IndexScope::Min() const
{
	return MinValue;
}

int IndexScope::Max() const
{
	return MaxValue;
}

const NumberCollection* IndexScope::ValueCollection() const
{
	return Values;
}

int IndexScope::Next() const
{
	if (!Values)
	{
		if (!_index)
		{
			// start from min value.
			_index = new int(MinValue);
		}
		else
		{
			++(*_index);

			if (*_index > MaxValue)
				*_index = -1; // not be larger than the max
		}

		return *_index;
	}
	else
	{
		if (!_index)
		{
			_index = new int(-1);

			// get the first index match the scope
			for (size_t i = 0; i < Values->size(); ++i)
			{
				if ((*Values)[i] >= MinValue && (*Values)[i] <= MaxValue)
				{
					*_index = static_cast<int>(i);
					break;
				}
			}
		}
		else if (*_index >= 0)
		{
			++(*_index);

			// not be larger than the max value and should be one in the values.
			if (*_index >= static_cast<int>(Values->size()) || (*Values)[*_index] > MaxValue)
				*_index = -1;
		}

		return *_index >= 0 ? (*Values)[*_index] : -1;
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////
//

BuildToken::BuildToken()
{
}

BuildToken::~BuildToken()
{
	delete _RestItemsBits;
	delete _NumBitsToSkip;
	delete[] _NumHitCounts;
}

BuildToken::BuildToken(MatrixBuildSettings* settings)
{
	_settings = settings;
	_RestItemsBits = new MatrixItemPositionBits(_settings->TestItemCount(), false);
	_NumBitsToSkip = new MatrixItemPositionBits(_settings->TestItemCount(), true);
	_NumHitCounts = new int[_settings->CandidateNumCount]{ 0 };
	_UnhitNumCount = _settings->CandidateNumCount;
	_NextPosMax = _settings->TestItemCount() - 1; // to the last
}

BuildToken* BuildToken::Clone()
{
	auto token = new BuildToken();

	token->_settings = _settings;
	token->_RestItemsBits = _RestItemsBits->Clone();
	token->_NumBitsToSkip = _NumBitsToSkip->Clone();
	token->_NumHitCounts = new int[_settings->CandidateNumCount]{ 0 };
	CloneInts(_NumHitCounts, token->_NumHitCounts, _settings->CandidateNumCount);
	token->_UnhitNumCount = _UnhitNumCount;
	token->_NextPosMax = _NextPosMax;

	return token;
}

void BuildToken::CloneInts(int* source, int* target, int count)
{
	// Copy the specified number of bytes from source to target.
	for (int i = 0; i < count; i++)
	{
		*target = *source;
		target++;
		source++;
	}
}

void BuildToken::RefreshForCommit(int minHitCountForEach, int maxHitCountForEach)
{
	int nextPosMax = -1;
	for (int i = 0; i < _settings->CandidateNumCount; ++i)
	{
		if (_NumHitCounts[i] >= maxHitCountForEach)
			_NumBitsToSkip->AddMultiple(*(_settings->NumDistribution(i).Distribution));

		if (nextPosMax < 0 && _NumHitCounts[i] < minHitCountForEach)
		{
			nextPosMax = _settings->NumDistribution(i).MaxIndex;
		}
	}

	if (nextPosMax > 0)
		_NextPosMax = nextPosMax;
}

void BuildToken::UpdateNumCoverage(const MatrixItemByte& item, int minHitCountForEach, int maxHitCountForEach)
{
	int nextPosMax = -1;

	UINT64 itemBits = item.GetBits();

	int* ps = _NumHitCounts;
	for (int i = 0; i < _settings->CandidateNumCount; i++)
	{
		if ((_settings->NumDistribution(i).Bits & itemBits) != 0)
		{
			++(*ps);

			if (*ps == 1)
			{
				// if this number is just hitted, reduce the unhit number count.
				--_UnhitNumCount; // this number was just hitted.
			}

			if (*ps == maxHitCountForEach)
			{
				// this number has been hitted enough times, skip all items that contains this number for next search.
				_NumBitsToSkip->AddMultiple(*(_settings->NumDistribution(i).Distribution));
			}
		}

		if (nextPosMax < 0 && * ps < minHitCountForEach)
		{
			// if this number is not yet hit the max hit count, expect it could be selected in future, 
			// so mark the max next postion as the last item contains this number.
			nextPosMax = _settings->NumDistribution(i).MaxIndex;
		}

		ps++;
	}

	if (nextPosMax > 0)
		_NextPosMax = nextPosMax;
}

void BuildToken::UpdateItemCoverage(int addItemIndex)
{
	_RestItemsBits->RemoveMultiple(_settings->TestItemMash(addItemIndex));
}

bool BuildToken::NextItemScope(int curIndex, IndexScope& nextScope)
{
	bool bJumpToNextUnhitted = false;
	if (bJumpToNextUnhitted)
	{
		// get the next uncoverred item.
		int nextRestPos = _RestItemsBits->NextPosition(0, true);
		int min = nextRestPos; // mark it as the first choise for next step
		int max = _NextPosMax;

		if (min >= 0 && min <= max)
		{
			// limited the selection for next item into the items that could cover the next item.
			nextScope.Reset(min, max, &(_settings->TestItemCoveredBy(nextRestPos)));
			return true;
		}
	}
	else
	{
		int min = _NumBitsToSkip->NextPosition(curIndex, false);
		int max = _NextPosMax;

		if (min >= 0 && min <= max)
		{
			nextScope.Reset(min, max);
			return true;
		}
	}

	return false;
}

int BuildToken::UncoveredNumCount()
{
	return _UnhitNumCount;
}

bool BuildToken::IsAllItemsCovered()
{
	return _RestItemsBits->IsClean();
}

int BuildToken::UncoveredItemCount()
{
	return _RestItemsBits->GetUnhitCount();
}

////////////////////////////////////////////////////////////////////////////////////////////////
//

BuildMatrixUtil::BuildMatrixUtil()
{
}


BuildMatrixUtil::~BuildMatrixUtil()
{
}

int BuildMatrixUtil::BitCount(UINT64 test)
{
	int count = 0;
	while (test != 0)
	{
		test = test & (test - 1);
		++count;
	}

	return count;
}

void BuildMatrixUtil::GetTestItemCollection(int candidateCount, int selectCount, vector<MatrixItemByte*>& output)
{
	// Get candidates.
	vector<BYTE> candidates(candidateCount);
	for (BYTE i = 0; i < candidateCount; ++i)
	{
		candidates[i] = (BYTE)(i + 1);
	}

	for (int i = 1; i <= candidateCount - selectCount + 1; ++i)
	{
		MatrixItemByte temp(selectCount);
		temp.Add(i);

		GetTestItemCollection(temp, candidates, i, selectCount - 1, output);
	}
}

void BuildMatrixUtil::GetTestItemCollection(const MatrixItemByte& sel, const vector<BYTE>& candidates, int index, int selectCount, vector<MatrixItemByte*>& output)
{
	int insertAt = sel.GetSize() - selectCount;

	if (selectCount == 1)
	{
		for (int i = index; i < candidates.size(); ++i)
		{
			MatrixItemByte* copy = new MatrixItemByte(sel);
			copy->Add(candidates[i]);

			output.push_back(copy);
		}
	}
	else
	{
		int endIndex = static_cast<int>(candidates.size()) - selectCount + 1;
		for (int i = index; i <= endIndex; ++i)
		{
			MatrixItemByte next(sel);
			next.Add(candidates[i]);

			GetTestItemCollection(next, candidates, i + 1, selectCount - 1, output);
		}
	}
}
