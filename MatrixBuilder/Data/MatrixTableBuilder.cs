using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;

namespace MatrixBuilder
{
    class MatrixTableBuilder
    {
        private MatrixTable _matrixTable = new MatrixTable();
        private MatrixCalculatorWrapper _max_builder = new MatrixCalculatorWrapper();

        public MatrixTableBuilder()
        {
            Init(false);
        }

        public MatrixTable GetTable()
        {
            return _matrixTable;
        }

        public List<ThreadStatus> GetProgress()
        {
            return _max_builder.GetProgress();
        }

        public void Init(bool bUseDefault)
        {
            _matrixTable.Init();

            for (int i = 7; i <= 33; ++i)
            {
                for (int j = 3; j <= 6; ++j)
                {
                    MatrixCell cell = new MatrixCell();

                    // Read from the file.
                    string file = ".\\Solution\\Best\\" + i.ToString() + "-" + j.ToString() + ".txt";
                    if (File.Exists(file))
                    {
                        IEnumerable<string> lines = File.ReadLines(file);
                        foreach (string line in lines)
                        {
                            cell.Template.Add(new MatrixItem(line));
                        }

                        cell.Status = MatrixCell.MatrixStatus.Best;
                    }
                    else
                    {
                        file = ".\\Solution\\Good\\" + i.ToString() + "-" + j.ToString() + ".txt";
                        if (File.Exists(file))
                        {
                            IEnumerable<string> lines = File.ReadLines(file);
                            foreach (string line in lines)
                            {
                                cell.Template.Add(new MatrixItem(line));
                            }

                            cell.Status = MatrixCell.MatrixStatus.Candidate;
                        }
                        else
                        {
                            file = ".\\Solution\\Default\\" + i.ToString() + "-" + j.ToString() + ".txt";
                            if (File.Exists(file))
                            {
                                IEnumerable<string> lines = File.ReadLines(file);
                                foreach (string line in lines)
                                {
                                    cell.Template.Add(new MatrixItem(line));
                                }
                            }
                            else
                            {
                                List<MatrixItem> defaultSoution = GetDefaultSolution(i, j, _matrixTable);
                                if (defaultSoution != null)
                                {
                                    // save to file.
                                    List<string> output = new List<string>();
                                    foreach (MatrixItem item in defaultSoution)
                                    {
                                        output.Add(item.ToString());
                                    }

                                    File.WriteAllLines(file, output);

                                    cell.Template = defaultSoution;
                                }
                            }

                            cell.Status = MatrixCell.MatrixStatus.Default;
                        }
                    }

                    _matrixTable.SetCell(i, j, cell);
                }
            }
        }

        private List<MatrixItem> GetDefaultSolution(int candidateCount, int selectCount, MatrixTable refTable)
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
                
                result.Sort((x, y) => x.ToString().CompareTo(y.ToString()));

                return result;
            }

            return null;
        }

        public int BuildMarixCell(int row, int col, int algorithm, int? betterThan = null, bool bParallel = false, bool bReturnForAny = false)
        {
            DateTime startTime = DateTime.Now;

            _matrixTable.Init();

             // if not specify the the max selection count, set it as the count of default solution.
            if (betterThan == null)
            {
                // Get the default matrix as the candidate solution.
                List<MatrixItem> defaultSoution = GetDefaultSolution(row, col, _matrixTable);
                if (defaultSoution != null)
                {
                    betterThan = defaultSoution.Count; // try to find the better solution than default.
                }
            }

            string strConditions = "Conditions: ";
            strConditions += "ProcesserCount: " + Environment.ProcessorCount + "\n";
            strConditions += "CandidateNumCount: " + row + "\n";
            strConditions += "SelectNumCount: " + col + "\n";
            strConditions += "FindingBetterThan: " + betterThan.Value + "\n";
            if (MessageBox.Show(strConditions, "Ready?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return -1;
            }

            List<MatrixItem> foundSolution = new List<MatrixItem>();
            _max_builder.Calcuate(row, col, algorithm, betterThan.Value, bParallel, bReturnForAny, foundSolution);

            TimeSpan duration = DateTime.Now - startTime;

            if (foundSolution != null && foundSolution.Count() > 0)
            {
                if (MessageBox.Show("Result:" + foundSolution.Count().ToString() + ". Save it?", "Successful", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _matrixTable.SetCell(row, col, new MatrixCell() { Template = foundSolution });

                    // save to file.
                    string file = row.ToString() + "-" + col.ToString() + ".txt";

                    List<string> output = new List<string>();

                    foreach (MatrixItem item in foundSolution)
                    {
                        output.Add(item.ToString());
                    }

                    File.WriteAllLines(".\\Solution\\Good\\" + file, output);
                }

                MessageBox.Show("Found Solution with Count " + foundSolution.Count().ToString() + " Duration: " + duration.ToString());

                return foundSolution.Count();
            }

            MessageBox.Show("No Solution Found!" + " Duration: " + duration.ToString());
 
            return -1;
        }
    }
}