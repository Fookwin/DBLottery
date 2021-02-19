#pragma once
#include "Progress.h"

class MatrixItemByte;

__interface IAlgorithmImpl
{
public:
    virtual void Calculate(int maxSelectionCount, ThreadProgressSet& progresses, bool returnForAny, bool bInParallel) = 0;
    virtual const vector<const MatrixItemByte*>& GetSolution() const = 0;
};