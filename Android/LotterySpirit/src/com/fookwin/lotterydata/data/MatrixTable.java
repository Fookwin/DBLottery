package com.fookwin.lotterydata.data;

public class MatrixTable
{
	private int[][] _matrixCount = null;
	private MatrixCell[][] _matrixData = null;

	public final MatrixCell GetCell(int candidateCount, int selectionCount)
	{
		return _matrixData[candidateCount - 6][selectionCount - 2];
	}

	public final int GetCellItemCount(int candidateCount, int selectionCount)
	{
		return _matrixCount[candidateCount - 6][selectionCount - 2];
	}

	public final MatrixCell SetCell(int candidateCount, int selectionCount, MatrixCell cell)
	{
		_matrixData[candidateCount - 6][selectionCount - 2] = cell;
		_matrixCount[candidateCount - 6][selectionCount - 2] = cell.Template.size();

		return cell;
	}

	public final void Init()
	{
		if (_matrixCount != null)
		{
			return;
		}

		_matrixCount = new int[32][5];
		_matrixData = new MatrixCell[32][5];

		// Init first column;
		for (int i = 6; i <= 33; ++i)
		{
			int count = i / 2;

			MatrixCell cell = new MatrixCell();
			for (int m = 0; m < count; ++m)
			{
				int start = m * 2 + 1;

				MatrixItemByte item = new MatrixItemByte(2);
				item.Add(start);
				item.Add(start + 1);

				cell.Template.add(item);
			}

			if (i % 2 == 1)
			{
				MatrixItemByte item = new MatrixItemByte(2);
				item.Add(1);
				item.Add(i);

				cell.Template.add(item);
			}

			_matrixData[i - 6][0] = cell;
			_matrixCount[i - 6][0] = cell.Template.size();
		}

		// Init first row;
		_matrixCount[0][1] = 2;
		MatrixCell tempVar = new MatrixCell();
		tempVar.Template = new java.util.ArrayList<MatrixItemByte>(java.util.Arrays.asList(new MatrixItemByte[] { new MatrixItemByte("01 02 03"), new MatrixItemByte("04 05 06") }));
		_matrixData[0][1] = tempVar;

		_matrixCount[0][2] = 3;
		MatrixCell tempVar2 = new MatrixCell();
		tempVar2.Template = new java.util.ArrayList<MatrixItemByte>(java.util.Arrays.asList(new MatrixItemByte[] { new MatrixItemByte("01 02 03 04"), new MatrixItemByte("01 02 03 06"), new MatrixItemByte("03 04 05 06") }));
		_matrixData[0][2] = tempVar2;

		_matrixCount[0][3] = 1;
		MatrixCell tempVar3 = new MatrixCell();
		tempVar3.Template = new java.util.ArrayList<MatrixItemByte>(java.util.Arrays.asList(new MatrixItemByte[] { new MatrixItemByte("01 02 03 04 05") }));
		_matrixData[0][3] = tempVar3;

		_matrixCount[0][4] = 1;
		MatrixCell tempVar4 = new MatrixCell();
		tempVar4.Template = new java.util.ArrayList<MatrixItemByte>(java.util.Arrays.asList(new MatrixItemByte[] { new MatrixItemByte("01 02 03 04 05 06") }));
		_matrixData[0][4] = tempVar4;
	}
}