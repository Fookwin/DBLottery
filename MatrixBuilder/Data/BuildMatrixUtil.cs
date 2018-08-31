using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixBuilder
{
    class BuildMatrixUtil
    {
        public static List<MatrixItem> GetDefaultSolution(int candidateCount, int selectCount, MatrixTable refTable)
        {
            // Get the solution from previous matrix results.
            if (refTable.GetCell(candidateCount - 1, selectCount) != null &&
                refTable.GetCell(candidateCount - 1, selectCount - 1) != null)
            {
                List<MatrixItem> result = new List<MatrixItem>();
                result.AddRange(refTable.GetCell(candidateCount - 1, selectCount).Template);

                foreach (MatrixItem filter in refTable.GetCell(candidateCount - 1, selectCount - 1).Template)
                {
                    MatrixItem item = new MatrixItem(filter);
                    item.Add(candidateCount);

                    result.Add(item);
                }

                result.Sort();

                return result;
            }

            return null;
        }
    }

}
