#pragma once
#include "Progress.h"

class MatrixItemByte;

__interface IAlgorithmImpl
{
public:
    virtual void Calculate(int expectedItemCount, ThreadProgressSet& progresses) = 0;
    virtual const vector<const MatrixItemByte*>& GetSolution() const = 0;
};