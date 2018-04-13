using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixBuilder
{
    class BuildMatrixUtil
    {
        public static UInt64[] Bits = new UInt64[33]
        {
            1, 2,4,8,16,32,64,128,256,512,
            1024,2048,4096,8192,16384,32768, 65536,131072,262144, 524288,
            1048576,2097152,4194304,8388608, 16777216,33554432,67108864,134217728,268435456,
            536870912,1073741824, 2147483648,4294967296
        };

        public static int BitCount(UInt64 test)
        {
            int count = 0;
            while (test != 0)
            {
                test = test & (test - 1);
                ++count;
            }

            return count;
        }

        public static List<MatrixItemByte> GetDefaultSolution(int candidateCount, int selectCount, MatrixTable refTable)
        {
            // Get the solution from previous matrix results.
            if (refTable.GetCell(candidateCount - 1, selectCount) != null &&
                refTable.GetCell(candidateCount - 1, selectCount - 1) != null)
            {
                List<MatrixItemByte> result = new List<MatrixItemByte>();
                result.AddRange(refTable.GetCell(candidateCount - 1, selectCount).Template);

                foreach (MatrixItemByte filter in refTable.GetCell(candidateCount - 1, selectCount - 1).Template)
                {
                    MatrixItemByte item = new MatrixItemByte(selectCount, filter.Bits);
                    item.Add(candidateCount);

                    result.Add(item);
                }

                result.Sort();

                return result;
            }

            return null;
        }

        public static bool ValidateSolution(int candidateCount, int selectCount, List<MatrixItemByte> test)
        {
            MatrixBuildSettings settings = new MatrixBuildSettings(candidateCount, selectCount);

            MatrixItemPositionBits restItemsBits = new MatrixItemPositionBits(settings.TestItemCount(), false);

            // Include the preselected items.
            int startIndex = 0;
            foreach (MatrixItemByte item in test)
            {
                for (int i = startIndex; i < settings.TestItemCount(); ++i)
                {
                    if (settings.TestItem(i).Bits == item.Bits)
                    {
                        restItemsBits.RemoveMultiple(settings.TestItemMash(i));
                        startIndex = i + 1;
                        break;
                    }
                }
            }

            return restItemsBits.UnhitCount == 0;
        }

        public static List<MatrixItemByte> GetTestItemCollection(int candidateCount, int selectCount)
        {
            List<MatrixItemByte> output = new List<MatrixItemByte>();

            // Get candidates.
            Byte[] candidates = new Byte[candidateCount];
            for (Byte i = 0; i < candidateCount; ++i)
            {
                candidates[i] = (Byte)(i + 1);
            }

            for (int i = 1; i <= candidateCount - selectCount + 1; ++i)
            {
                MatrixItemByte temp = new MatrixItemByte(selectCount);
                temp.Add(i);

                GetTestItemCollection(temp, candidates, i, selectCount - 1, ref output);
            }

            return output;
        }

        private static void GetTestItemCollection(MatrixItemByte sel, Byte[] candidates, int index, int selectCount, ref List<MatrixItemByte> output)
        {
            int insertAt = sel.Size - selectCount;

            if (selectCount == 1)
            {
                for (int i = index; i < candidates.Count(); ++i)
                {
                    MatrixItemByte copy = sel.Clone();
                    copy.Add(candidates[i]);

                    output.Add(copy);
                }
            }
            else
            {
                int endIndex = candidates.Count() - selectCount + 1;
                for (int i = index; i <= endIndex; ++i)
                {
                    MatrixItemByte copy = sel.Clone();
                    copy.Add(candidates[i]);

                    GetTestItemCollection(copy, candidates, i + 1, selectCount - 1, ref output);
                }
            }
        }
    }

}
