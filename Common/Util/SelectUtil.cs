using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Selection;
using LuckyBallsData.Statistics;
using LuckyBallsData.Filters;

namespace LuckyBallsData.Util
{
    public class SelectUtil
    {
        public static Set RandomReds(Set candidates, int count)
        {
            Set result = new Set();            

            int validCount = 0;
            while (validCount < count)
            {
                Random rnd = new Random((int)DateTime.UtcNow.Ticks);
                int red = rnd.Next(1, 33);
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
                Random rnd = new Random((int)DateTime.UtcNow.Ticks);
                int blue = rnd.Next(1, 16);
                if (!result.Contains(blue) && candidates.Contains(blue))
                {
                    result.Add(blue);
                    validCount++;
                }
            }

            return result;
        }

        public static List<Scheme> RandomSchemes(int count, RandomSchemeSelector selector)
        {
            List<Scheme> result = new List<Scheme>();

            Set redCandidates = new Set();
            for (int i = 1; i <= 33; ++ i)
            {
                if (!selector.IncludedReds.Contains(i) && !selector.ExcludedReds.Contains(i))
                    redCandidates.Add(i);
            }

            Set blueCandidates = new Set();
            for (int i = 1; i <= 16; ++ i)
            {
                if (!selector.IncludedBlues.Contains(i) && !selector.ExcludedBlues.Contains(i))
                    blueCandidates.Add(i);
            }

            for (int i = 0; i < count; ++i)
            {
                // Get reds.
               
                Set selected = RandomReds(redCandidates, 6 - selector.IncludedReds.Count);

                // Add inclucded.
                selected.Add(selector.IncludedReds);

                // Get blue.
                int blue = 1;
                if (selector.IncludedBlues.Count > 0)
                    blue = selector.IncludedBlues.Numbers[0];
                else
                {
                    Set selectedBlue = RandomBlues(blueCandidates, 1);
                    blue = selectedBlue.Numbers[0];
                }

                // create a scheme for this.
                int[] reds = selected.Numbers;
                Scheme item = new Scheme(reds[0], reds[1], reds[2], reds[3], reds[4], reds[5], blue);
                result.Add(item);
            }

            return result;
        }

        public static List<Scheme> CalculateSchemeSelection(Set validReds, Set validBlues, bool applyBlueForSingle, 
            bool bPickBlueRandomly, bool matrixFilter)
        {
            if (validReds.Count < 6 || validBlues.Count <= 0)
                return null;

            Random rnd = new Random((int)DateTime.UtcNow.Ticks);
            List<Scheme> result = new List<Scheme>();

            if (matrixFilter)
            {
                // Get matrix table.
                MatrixTable table = DataManageBase.Instance().MatrixTable;
                
                // Get the template.
                MatrixCell cell = table.GetCell(validReds.Count, 6);

                int[] reds = validReds.Numbers;
                List<int> blues = validBlues.Numbers.ToList();

                foreach (MatrixItemByte temp in cell.Template)
                {
                    int[] matrix = temp.Instance(reds);

                    if (applyBlueForSingle)
                    {
                        if (blues.Count == 0)
                            blues = validBlues.Numbers.ToList(); // reuse the bules when if the solution are more then blues..

                        int blueIndex = 0;
                        if (bPickBlueRandomly)
                        {
                            // pick the blue at random position.                            
                            blueIndex = rnd.Next(0, blues.Count);
                        }

                        result.Add(new Scheme(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5], blues[blueIndex]));

                        // erase the selected blue.
                        blues.RemoveAt(blueIndex);
                    }
                    else
                    {
                        foreach (int blue in blues)
                            result.Add(new Scheme(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5], blue));
                    }
                }
            }
            else
            {
                int[] reds = validReds.Numbers;
                List<int> blues = validBlues.Numbers.ToList();
                int count = reds.Count();
                for (int inx1 = 0; inx1 <= count - 6; inx1++)
                    for (int inx2 = inx1 + 1; inx2 <= count - 5; inx2++)
                        for (int inx3 = inx2 + 1; inx3 <= count - 4; inx3++)
                            for (int inx4 = inx3 + 1; inx4 <= count - 3; inx4++)
                                for (int inx5 = inx4 + 1; inx5 <= count - 2; inx5++)
                                    for (int inx6 = inx5 + 1; inx6 <= count - 1; inx6++)
                                    {
                                        if (applyBlueForSingle)
                                        {
                                            if (blues.Count == 0)
                                                blues = validBlues.Numbers.ToList(); // reuse the bules when if the solution are more then blues..

                                            int blueIndex = 0;
                                            if (bPickBlueRandomly)
                                            {
                                                // pick the blue at random position.
                                                blueIndex = rnd.Next(0, blues.Count);
                                            }
                                            
                                            result.Add(new Scheme(reds[inx1], reds[inx2], reds[inx3], reds[inx4], reds[inx5], reds[inx6], blues[blueIndex]));

                                            // erase the selected blue.
                                            blues.RemoveAt(blueIndex);
                                        }
                                        else
                                        {
                                            foreach (int blue in blues)
                                                result.Add(new Scheme(reds[inx1], reds[inx2], reds[inx3], reds[inx4], reds[inx5], reds[inx6], blue));
                                        }
                                    }
            }

            return result;
        }

        public static List<Scheme> CalculateSchemeSelection(Set validDans, Set validTuos, Set validBlues)
        {
            if (validDans.Count > 6 || validDans.Count + validTuos.Count < 6 || validBlues.Count <= 0)
                return null;

            List<Scheme> result = new List<Scheme>();

            int[] tuos = validTuos.Numbers;
            int[] blues = validBlues.Numbers;

            Set temp = new Set(validDans);
            GetSchemeReds(tuos, 6 - validDans.Count, 0, ref temp, blues, ref result);
            
            return result;
        }

        private static void GetSchemeReds(int[] candidates, int count, int startInx, ref Set selected, int[] blues, ref List<Scheme> output)
        {
            if (count == 0)
            {
                int[] reds = selected.Numbers;
                foreach (int blue in blues)
                    output.Add(new Scheme(reds[0], reds[1], reds[2], reds[3], reds[4], reds[5], blue));

                return;
            }            

            while (startInx < candidates.Count())
            {
                Set clone = new Set(selected);
                clone.Add(candidates[startInx++]);
                GetSchemeReds(candidates, count - 1, startInx, ref clone, blues, ref output);
            }
        }

        public static void Filter(ref List<Scheme> candidates, List<Constraint> constraints)
        {
            int testIndex = DataManageBase.Instance().History.Count - 1; // get latest index.

            for (int i = candidates.Count - 1; i >= 0 ; -- i)
            {
                Scheme candi = candidates[i];

                bool bPassed = true;
                foreach (Constraint con in constraints)
                {
                    if (!con.Meet(candi, testIndex))
                    {
                        bPassed = false;
                        break;
                    }
                }

                if (!bPassed)
                    candidates.RemoveAt(i);
            }
        }

        public static void RadomRemain(ref List<Scheme> candidates, int remainCount)
        {
            bool bRemove = candidates.Count < remainCount * 2;
            int loopCount = bRemove ? candidates.Count - remainCount : remainCount;

            if (bRemove)
            {
                for (int i = 0; i < loopCount; ++i)
                {
                    Random rnd = new Random((int)DateTime.UtcNow.Ticks + i);
                    int pick = rnd.Next(0, candidates.Count - 1);

                    candidates.RemoveAt(pick);
                }
            }
            else
            {
                List<Scheme> temp = new List<Scheme>();
                for (int i = 0; i < loopCount; ++i)
                {
                    Random rnd = new Random((int)DateTime.UtcNow.Ticks + i);
                    int pick = rnd.Next(0, candidates.Count - 1);
                    temp.Add(candidates[pick]);

                    candidates.RemoveAt(pick);
                }

                candidates.Clear();
                candidates.AddRange(temp);
            }
        }
    }
}
