package com.fookwin.lotterydata.util;

import java.util.ArrayList;

import com.fookwin.lotterydata.data.Constraint;
import com.fookwin.lotterydata.data.DataManageBase;
import com.fookwin.lotterydata.data.MatrixCell;
import com.fookwin.lotterydata.data.MatrixItemByte;
import com.fookwin.lotterydata.data.MatrixTable;
import com.fookwin.lotterydata.data.RandomSchemeSelector;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotterydata.data.Set;

public class SelectUtil
{
	public static Set RandomReds(Set candidates, int count)
	{
		Set result = new Set();

		int validCount = 0;
		while (validCount < count)
		{
			java.util.Random rnd = new java.util.Random();
			int red = rnd.nextInt(33) + 1;
			if (!result.Contains(red) && candidates.Contains(red))
			{
				result.Add(red);
				validCount++;
			}
		}

		return result;
	}

	public static Set RandomBlues(Set candidates, int count)
	{
		Set result = new Set();

		int validCount = 0;
		while (validCount < count)
		{
			java.util.Random rnd = new java.util.Random();
			int blue = rnd.nextInt(16) + 1;
			if (!result.Contains(blue) && candidates.Contains(blue))
			{
				result.Add(blue);
				validCount++;
			}
		}

		return result;
	}

	public static void RandomSchemes(int count, RandomSchemeSelector selector, ArrayList<Scheme> result)
	{
		Set redCandidates = new Set();
		for (int i = 1; i <= 33; ++ i)
		{
			if (!selector.getIncludedReds().Contains(i) && !selector.getExcludedReds().Contains(i))
			{
				redCandidates.Add(i);
			}
		}

		Set blueCandidates = new Set();
		for (int i = 1; i <= 16; ++ i)
		{
			if (!selector.getIncludedBlues().Contains(i) && !selector.getExcludedBlues().Contains(i))
			{
				blueCandidates.Add(i);
			}
		}

		for (int i = 0; i < count; ++i)
		{
			// Get reds.

			Set selected = RandomReds(redCandidates, 6 - selector.getIncludedReds().getCount());

			// Add included.
			selected.Add(selector.getIncludedReds());

			// Get blue.
			int blue = 1;
			if (selector.getIncludedBlues().getCount() > 0)
			{
				blue = selector.getIncludedBlues().getNumbers()[0];
			}
			else
			{
				Set selectedBlue = RandomBlues(blueCandidates, 1);
				blue = selectedBlue.getNumbers()[0];
			}

			// create a scheme for this.
			int[] reds = selected.getNumbers();
			Scheme item = new Scheme(reds[0], reds[1], reds[2], reds[3], reds[4], reds[5], blue);
			result.add(item);
		}
	}

	public static java.util.ArrayList<Scheme> CalculateSchemeSelection(Set validReds, Set validBlues, boolean applyBlueForSingle, boolean bPickBlueRandomly, boolean matrixFilter)
	{
		if (validReds.getCount() < 6 || validBlues.getCount() <= 0)
		{
			return null;
		}

		java.util.Random rnd = new java.util.Random(System.currentTimeMillis());
		java.util.ArrayList<Scheme> result = new java.util.ArrayList<Scheme>();

		if (matrixFilter)
		{
			// Get matrix table.
			MatrixTable table = DataManageBase.Instance().getMatrixTable();

			// Get the template.
			MatrixCell cell = table.GetCell(validReds.getCount(), 6);

			int[] reds = validReds.getNumbers();
			int[] blues = validBlues.getNumbers();
			int blueIndex = -1;

			for (MatrixItemByte temp : cell.Template)
			{
				Integer[] matrix = temp.Instance(reds);

				if (applyBlueForSingle)
				{
					if (bPickBlueRandomly)
					{
						// pick the blue at random position.                            
						blueIndex = rnd.nextInt(blues.length);
					}
					else
					{
						// get the next index.
						blueIndex ++;
						
						if (blueIndex == blues.length)
							blueIndex = 0;
					}

					result.add(new Scheme(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5], blues[blueIndex]));
				}
				else
				{
					for (int blue : blues)
					{
						result.add(new Scheme(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5], blue));
					}
				}
			}
		}
		else
		{
			int[] reds = validReds.getNumbers();
			int[] blues = validBlues.getNumbers();
			int blueIndex = -1;
			
			int count = reds.length;
			for (int inx1 = 0; inx1 <= count - 6; inx1++)
			{
				for (int inx2 = inx1 + 1; inx2 <= count - 5; inx2++)
				{
					for (int inx3 = inx2 + 1; inx3 <= count - 4; inx3++)
					{
						for (int inx4 = inx3 + 1; inx4 <= count - 3; inx4++)
						{
							for (int inx5 = inx4 + 1; inx5 <= count - 2; inx5++)
							{
								for (int inx6 = inx5 + 1; inx6 <= count - 1; inx6++)
								{
									if (applyBlueForSingle)
									{
										if (bPickBlueRandomly)
										{
											// pick the blue at random position.
											blueIndex = rnd.nextInt(blues.length);
										}
										else
										{
											// get the next index.
											blueIndex ++;
											
											if (blueIndex == blues.length)
												blueIndex = 0;
										}
										
										result.add(new Scheme(reds[inx1], reds[inx2], reds[inx3], reds[inx4], reds[inx5], reds[inx6], blues[blueIndex]));
									}
									else
									{
										for (int blue : blues)
										{
											result.add(new Scheme(reds[inx1], reds[inx2], reds[inx3], reds[inx4], reds[inx5], reds[inx6], blue));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		return result;
	}

	public static java.util.ArrayList<Scheme> CalculateSchemeSelection(Set validDans, Set validTuos, Set validBlues)
	{
		if (validDans.getCount() > 6 || validDans.getCount() + validTuos.getCount() < 6 || validBlues.getCount() <= 0)
		{
			return null;
		}

		java.util.ArrayList<Scheme> result = new java.util.ArrayList<Scheme>();

		int[] tuos = validTuos.getNumbers();
		int[] blues = validBlues.getNumbers();

		Set temp = new Set(validDans);
		GetSchemeReds(tuos, 6 - validDans.getCount(), 0, temp, blues, result);

		return result;
	}

	private static void GetSchemeReds(int[] candidates, int count, int startInx, Set selected, int[] blues, ArrayList<Scheme> output)
	{
		if (count == 0)
		{
			int[] reds = selected.getNumbers();
			for (int blue : blues)
			{
				output.add(new Scheme(reds[0], reds[1], reds[2], reds[3], reds[4], reds[5], blue));
			}

			return;
		}

		while (startInx < candidates.length)
		{
			Set clone = new Set(selected);
			clone.Add(candidates[startInx++]);
			GetSchemeReds(candidates, count - 1, startInx, clone, blues, output);
		}
	}

	public static void Filter(ArrayList<Scheme> _selection, java.util.ArrayList<Constraint> constraints)
	{
		int testIndex = DataManageBase.Instance().getHistory().getCount() - 1; // get latest index.

		int size = _selection.size();
		for (int i = size - 1; i >= 0 ; -- i)
		{
			Scheme candi = _selection.get(i);

			boolean bPassed = true;
			for (Constraint con : constraints)
			{
				if (!con.Meet(candi, testIndex))
				{
					bPassed = false;
					break;
				}
			}

			if (!bPassed)
			{
				_selection.remove(i);
			}
		}
	}

	public static void RadomRemain(ArrayList<Scheme> candidates, int remainCount)
	{
		boolean bRemove = candidates.size() < remainCount * 2;
		int loopCount = bRemove ? candidates.size() - remainCount : remainCount;

		if (bRemove)
		{
			for (int i = 0; i < loopCount; ++i)
			{
				java.util.Random rnd = new java.util.Random(System.currentTimeMillis() + i);
				int pick = rnd.nextInt(candidates.size() - 1);

				candidates.remove(pick);
			}
		}
		else
		{
			java.util.ArrayList<Scheme> temp = new java.util.ArrayList<Scheme>();
			for (int i = 0; i < loopCount; ++i)
			{
				java.util.Random rnd = new java.util.Random(System.currentTimeMillis() + i);
				int pick = rnd.nextInt(candidates.size() - 1);
				temp.add(candidates.get(pick));

				candidates.remove(pick);
			}

			candidates.clear();
			candidates.addAll(temp);
		}
	}
}