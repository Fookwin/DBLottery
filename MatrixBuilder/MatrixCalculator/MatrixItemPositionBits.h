#pragma once
class MatrixItemPositionBits
{
public:
	MatrixItemPositionBits(int size, bool asEmpty);
	~MatrixItemPositionBits();

	void AddSingle(UINT64 test, UINT16 segment);
	bool Contains(int pos);
	int NextPosition(int previous, bool testHas);
	void AddSingle(int pos);
	void AddMultiple(const MatrixItemPositionBits& test);
	void RemoveSingle(int pos);
	void RemoveMultiple(const MatrixItemPositionBits& test);
	MatrixItemPositionBits* Clone();
	bool IsClean();
	int GetUnhitCount();

private:
	MatrixItemPositionBits();
	void CloneFlags(UINT64* source, UINT64* target, int count);

	UINT64* Flags = nullptr;
	int _unhitCount = -1;
	int _flagCount = -1;
};

