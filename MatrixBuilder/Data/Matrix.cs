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
