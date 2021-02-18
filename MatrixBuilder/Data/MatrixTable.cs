using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixBuilder
{
    public class MatrixCell
    {
        public enum MatrixStatus
        {
            Best,
            Candidate,
            Default
        };

        public List<MatrixItem> Template = new List<MatrixItem>();
        public MatrixStatus Status = MatrixStatus.Candidate;

        public bool IsNormalized()
        {
            if (Template.Count == 0)
                return true;

            SortedDictionary<int, int> sortedNumbers = new SortedDictionary<int, int>();

            // the first item must be 01 02 ...
            for (int i = 0; i < Template.Count; ++ i)
            {
                if (i == 0)
                {
                    int index = 1;
                    foreach (var num in Template[i].Numbers())
                    {
                        if (num != index++)
                            return false;
                    }
                }

                foreach (var num in Template[i].Numbers())
                {
                    if (sortedNumbers.ContainsKey(num))
                        sortedNumbers[num]++;
                    else
                        sortedNumbers.Add(num, 0);
                }
            }

            // check if the number's hit count is sorted by order
            var hitCounts = sortedNumbers.Values;
            int lastHit = 0;
            foreach (var hit in hitCounts)
            {
                if (lastHit <= 0)
                {
                    lastHit = hit;
                    continue;
                }

                if (lastHit < hit)
                    return false;

                lastHit = hit;
            }

            return true;
        }

        public bool Normalize()
        {
            if (Template.Count == 0)
                return false;

            SortedDictionary<int, int> sortedNumbers = new SortedDictionary<int, int>();

            // the first item must be 01 02 ...
            for (int i = 0; i < Template.Count; ++i)
            {
                foreach (var num in Template[i].Numbers())
                {
                    if (sortedNumbers.ContainsKey(num))
                        sortedNumbers[num]++;
                    else
                        sortedNumbers.Add(num, 0);
                }
            }

            // sort the num by hit count.
            var sortedByHits = sortedNumbers.OrderBy(o => -o.Value).ToList();

            // build the number change map
            Dictionary<int, int> changeMap = new Dictionary<int, int>();
            int index = 1;
            foreach (var changed in sortedByHits)
            {
                changeMap.Add(changed.Key, index++);
            }

            // replace the numbers and get the first item string.
            string firstStrVal = null;
            for (int i = 0; i < Template.Count; ++i)
            {
                var sorted = new SortedSet<int>();

                foreach (var num in Template[i].Numbers())
                {
                    sorted.Add(changeMap[num]);
                }

                string strVal = "";
                foreach (var num in sorted)
                {
                    strVal += " " + num.ToString().PadLeft(2, '0');
                }

                strVal = strVal.Trim();

                if (firstStrVal == null || firstStrVal.CompareTo(strVal) > 0)
                    firstStrVal = strVal;
            }

            // check if we need adjust the first item
            Dictionary<int, int> changeMap2 = new Dictionary<int, int>();
            MatrixItem tempItem = new MatrixItem(firstStrVal);
            index = 1;
            foreach (var num in tempItem.Numbers())
            {
                if (num != index)
                {
                    // exchange the number with the index. actually num > index
                    if (changeMap2.ContainsKey(index))
                    {
                        changeMap2.Add(num, index);
                        changeMap2[changeMap2[index]] = num;
                    }
                    else
                    {
                        changeMap2.Add(num, index);
                        changeMap2.Add(index, num);
                    }
                }

                index++;
            }

            SortedSet<string> sortedSolution = new SortedSet<string>();
            for (int i = 0; i < Template.Count; ++i)
            {
                var sorted = new SortedSet<int>();

                foreach (var num in Template[i].Numbers())
                {
                    int newNum = changeMap[num];
                    if (changeMap2.ContainsKey(newNum))
                        newNum = changeMap2[newNum];

                    sorted.Add(newNum);
                }

                string strVal = "";
                foreach (var num in sorted)
                {
                    strVal += " " + num.ToString().PadLeft(2, '0');
                }

                sortedSolution.Add(strVal.Trim());
            }

            Template.Clear();
            foreach (var val in sortedSolution)
            {
                Template.Add(new MatrixItem(val));
            }

            return true;
        }
    }

    public class MatrixTable
    {
        private int[,] _matrixCount = null;
        private MatrixCell[,] _matrixData = null;

        public MatrixCell GetCell(int candidateCount, int selectionCount)
        {
            return _matrixData[candidateCount - 6, selectionCount - 2];
        }

        public int GetCellItemCount(int candidateCount, int selectionCount)
        {
            return _matrixCount[candidateCount - 6, selectionCount - 2];
        }        

        public MatrixCell SetCell(int candidateCount, int selectionCount, MatrixCell cell)
        {
            _matrixData[candidateCount - 6, selectionCount - 2] = cell;
            _matrixCount[candidateCount - 6, selectionCount - 2] = cell.Template.Count;

            return cell;
        }

        public void Init()
        {
            if (_matrixCount != null)
                return;

            _matrixCount = new int[32, 5];
            _matrixData = new MatrixCell[32, 5];

            // Init first column;
            for (int i = 6; i <= 33; ++i)
            {
                int count = i / 2;

                MatrixCell cell = new MatrixCell();
                for (int m = 0; m < count; ++m)
                {
                    int start = m * 2 + 1;

                    MatrixItem item = new MatrixItem();
                    item.Add(start);
                    item.Add(start + 1);

                    cell.Template.Add(item);
                }

                if (i % 2 == 1)
                {
                    MatrixItem item = new MatrixItem();
                    item.Add(1);
                    item.Add(i);

                    cell.Template.Add(item);
                }

                _matrixData[i - 6, 0] = cell;
                _matrixCount[i - 6, 0] = cell.Template.Count;
            }

            // Init first row;
            _matrixCount[0, 1] = 2;
            _matrixData[0, 1] = new MatrixCell() { Template = new List<MatrixItem>() { new MatrixItem("01 02 03"), new MatrixItem("04 05 06") } };

            _matrixCount[0, 2] = 3;
            _matrixData[0, 2] = new MatrixCell() { Template = new List<MatrixItem>() { new MatrixItem("01 02 03 04"), new MatrixItem("01 02 03 06"), new MatrixItem("03 04 05 06") } };

            _matrixCount[0, 3] = 1;
            _matrixData[0, 3] = new MatrixCell() { Template = new List<MatrixItem>() { new MatrixItem("01 02 03 04 05") } };

            _matrixCount[0, 4] = 1;
            _matrixData[0, 4] = new MatrixCell() { Template = new List<MatrixItem>() { new MatrixItem("01 02 03 04 05 06") } };
        }
    }    
}
