using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;
using LuckyBallsData.Selection;

namespace LuckyBallsServer.Pages
{
    /// <summary>
    /// Interaction logic for SchemeAnalysisPage.xaml
    /// </summary>
    public partial class SchemeAnalysisPage : Page
    {
        private static int[] SCORES = new int[] {1, 16, 165, 1240, 7192, 33563};

        enum AnalysisType
        {
            eSchemeReds = 0,
            eSchemeBlue = 1,
            eRedNumGroup = 2
        }

        public SchemeAnalysisPage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                Init();
            };
        }

        private void _validate()
        {
            int index = 0;

            int[] reds = new int[6];
            int count = reds.Count();
            for (int inx1 = 0; inx1 <= count - 6; inx1++)
            {
                reds[0] = inx1 + 1;
                for (int inx2 = inx1 + 1; inx2 <= count - 5; inx2++)
                {
                    reds[1] = inx2 + 1;
                    for (int inx3 = inx2 + 1; inx3 <= count - 4; inx3++)
                    {
                        reds[2] = inx3 + 1;
                        for (int inx4 = inx3 + 1; inx4 <= count - 3; inx4++)
                        {
                            reds[3] = inx4 + 1;
                            for (int inx5 = inx4 + 1; inx5 <= count - 2; inx5++)
                            {
                                reds[4] = inx5 + 1;
                                for (int inx6 = inx5 + 1; inx6 <= count - 1; inx6++)
                                {
                                    reds[5] = inx6 + 1;
                                    index++;

                                    // validate.
                                    int convertedIndex = Scheme.getIndexFromNumbers(reds);
                                    if (index != convertedIndex)
                                    {
                                        MessageBox.Show("original index:" + index.ToString() + " != " + "converted indes:" + convertedIndex.ToString());
                                        return;
                                    }

                                    int[] converted = Scheme.getNumbersFromIndex(index);
                                    for (int i = 0; i < 6; ++i)
                                    {
                                        if (reds[i] != converted[i])
                                        {
                                            MessageBox.Show("at " + i.ToString() + " original number:" + reds[i].ToString() + " converted number:" + converted[i].ToString());
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            MessageBox.Show("Passed!");

        }

        private void ToSchemeButton_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32(IndexFild.Text);
            SchemeField.Text = IndexToSchemeReds(index);
        }

        private string IndexToSchemeReds(int index)
        {
            int[] numbers = Scheme.getNumbersFromIndex(index);

            string output = "";
            for (int i = 0; i < 6; ++i)
            {
                output += numbers[i].ToString().PadLeft(2, '0') + " ";
            }

            return output.Trim();
        }

        private void ToIndexButton_Click(object sender, RoutedEventArgs e)
        {
            string[] subs = SchemeField.Text.Split(' ');
            if (subs.Count() != 6)
                return;

            int[] numbers = new int[6];
            for (int i = 0; i < 6; ++ i)
                numbers[i] = Convert.ToInt32(subs[i]);

            int index = Scheme.getIndexFromNumbers(numbers);

            IndexFild.Text = index.ToString();
        }

        private void Init()
        {
            // evaluating.
            AttributeManager mgr = LBDataManager.GetDataMgr().GetAttriMgr();
            int issue = LBDataManager.GetDataMgr().IssueInLocal;

            string bluefile = TargetFileName(issue, AnalysisType.eSchemeBlue, -1);
            if (File.Exists(bluefile))
            {
                string[] lines = File.ReadAllLines(bluefile);
                UpdateBlueResultList(lines);
            }

            string redfile = TargetFileName(issue, AnalysisType.eSchemeReds, -1);
            if (File.Exists(redfile))
            {
                string[] lines = File.ReadAllLines(redfile);
                UpdateRedsResultList(lines);
            }

            NumCountCombo.SelectedIndex = 0;

            string redGroupfile = TargetFileName(issue, AnalysisType.eRedNumGroup, 1);
            if (File.Exists(redGroupfile))
            {
                string[] lines = File.ReadAllLines(redGroupfile);
                UpdateRedsGroupResultList(lines);
            }
        }

        private void UpdateBlueResultList(string[] result)
        {
            Dictionary<string, int> evaluation_blue = new Dictionary<string, int>();
            foreach (string item in result)
            {
                string[] parts = item.Split();
                if (parts.Count() > 1)
                {
                    int score = Convert.ToInt32(parts[1]);
                    evaluation_blue.Add(parts[0], score);
                }
            }

            EvaluateDetailsBlue.DataContext = GetTableData(evaluation_blue);
        }

        private void UpdateRedsResultList(string[] result)
        {
            Dictionary<string, int> evaluation_reds = new Dictionary<string, int>();

            // show the top ten scheme
            string[] topTenIndices = new string[10];
            int[] topTenScores = new int[10];
            foreach (string item in result)
            {
                string[] parts = item.Split();
                if (parts.Count() > 1)
                {
                    int score = Convert.ToInt32(parts[1]);

                    bool bHandled = false;
                    for (int i = 9; i >= 0; --i)
                    {
                        if (score < topTenScores[i])
                        {
                            if (i < 9)
                            {
                                int index = 10;
                                while (-- index > i + 1)
                                {
                                    topTenIndices[index] = topTenIndices[index - 1];
                                    topTenScores[index] = topTenScores[index - 1]; 
                                }

                                topTenIndices[i + 1] = parts[0];
                                topTenScores[i + 1] = score;
                            }

                            bHandled = true;

                            break;
                        }
                    }

                    if (!bHandled)
                    {
                        // the max
                        int index = 10;
                        while (--index > 0)
                        {
                            topTenIndices[index] = topTenIndices[index - 1];
                            topTenScores[index] = topTenScores[index - 1];
                        }

                        topTenIndices[0] = parts[0];
                        topTenScores[0] = score;
                    }
                }
            }

            for (int i = 0; i < 10; ++i)
            {
                evaluation_reds.Add(IndexToSchemeReds(Convert.ToInt32(topTenIndices[i])), topTenScores[i]);
            }

            EvaluateDetailsReds.DataContext = GetTableData(evaluation_reds);
        }

        private void UpdateRedsGroupResultList(string[] names, int[] scores)
        {
            Dictionary<string, int> evaluation_reds = new Dictionary<string, int>();

            for (int i = 0; i < 10; ++i)
            {
                evaluation_reds.Add(names[i], scores[i]);
            }

            EvaluateNumberDetailsReds.DataContext = GetTableData(evaluation_reds);
        }

        private void UpdateRedsGroupResultList(string[] lines)
        {
            Dictionary<string, int> evaluation_reds = new Dictionary<string, int>();

            for (int i = 0; i < lines.Length; ++i)
            {
                string record = lines[i];
                if (record != "")
                {
                    string[] subs = record.Split('\t');
                    evaluation_reds.Add(subs[0], Convert.ToInt32(subs[1]));
                }           
            }

            EvaluateNumberDetailsReds.DataContext = GetTableData(evaluation_reds);
        }

        private string TargetFileName(int issue, AnalysisType type, int index)
        {
            if (type == AnalysisType.eSchemeReds)
                return "Z:/Analysis/Scheme/" + issue.ToString() + "_reds.txt";
            else if (type == AnalysisType.eSchemeBlue)
                return "Z:/Analysis/Scheme/" + issue.ToString() + "_blue.txt";
            else //if (type == AnalysisType.eRedNumGroup)
                return "Z:/Analysis/Scheme/" + issue.ToString() + "_redgroup_" + index.ToString() + ".txt";
        }

        private void EvaluateButton_Click(object sender, RoutedEventArgs e)
        {
            // evaluating.
            AttributeManager mgr = LBDataManager.GetDataMgr().GetAttriMgr();
            int issue = LBDataManager.GetDataMgr().IssueInLocal;

            SchemeAttributes testAttributes = mgr.Attributes(issue);

            DateTime startTime = DateTime.Now;

            // Get blue score
            string[] blueScores = new string[16]; 
            for (int blue = 1; blue <= 16; ++blue)
            {
                Scheme test = new Scheme(1, blue); // made a sample scheme for evaluate.
                int score = mgr.EvaluateScheme(testAttributes, test, AttributeManager.EvaluateSchemeOptionEnum.eBlue);
                blueScores[blue - 1] = blue.ToString() + "\t" + score.ToString() + "\r\n";
            }

            File.WriteAllLines(TargetFileName(issue, AnalysisType.eSchemeBlue, -1), blueScores);
            
            
            string output = "";
            int loopStep = 10000, testCount = 1107568;
            int loopCount = testCount / loopStep + 1;
            string[] totalResult = new string[testCount];

            ParallelOptions option = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            ParallelLoopResult loopResult = Parallel.For<string>(0, loopCount, option, () => "",
                (threadIndex, loopState, subResult) =>
            {
                int startIndex = loopStep * threadIndex + 1;
                if (startIndex <= testCount)
                {
                    int endIndex = Math.Min(testCount, startIndex + loopStep - 1);
                    for (int i = startIndex; i <= endIndex; ++i)
                    {
                        Scheme test = new Scheme(i, 1);
                        int redScore = mgr.EvaluateScheme(testAttributes, test, AttributeManager.EvaluateSchemeOptionEnum.eReds);
                        subResult += i.ToString() + "\t" + redScore.ToString() + "\r\n";
                    }
                }

                return subResult;
            },
            (subResult) =>
            {
                output += subResult;
            });

            // output.
            File.WriteAllText(TargetFileName(issue, AnalysisType.eSchemeReds, -1), output);

            TimeSpan duration = DateTime.Now - startTime;
            MessageBox.Show("Evaluation Done!" + " Duration: " + duration.ToString());
           
            // Update ui.
            UpdateBlueResultList(blueScores);
            UpdateRedsResultList(output.Split('\n'));
        }

        public DataTable GetTableData(Dictionary<string, int> result)
        {
            int start = 0;
            int count = result.Count;

            DataTable table = BuildTable();
            for (int index = start; index < count; ++index)
            {
                var element = result.ElementAt(index);
                AddRow(ref table, index + 1, element.Key, element.Value); 
            }
            return table;
        }

        private DataTable BuildTable()
        {
            DataTable table = new DataTable();

            Type intType = System.Type.GetType("System.Int32");
            Type stringType = System.Type.GetType("System.String");
            table.Columns.Add(CreateColumn("index", intType, false, false));
            table.Columns.Add(CreateColumn("item", stringType, false, false));
            table.Columns.Add(CreateColumn("score", intType, false, false));

            return table;
        }

        private DataColumn CreateColumn(string text, Type datatype, bool unique, bool writable)
        {
            DataColumn column = new DataColumn();
            column.DataType = datatype;
            column.ColumnName = text;
            column.ReadOnly = !writable;
            column.Unique = unique;

            return column;
        }

        private void AddRow(ref DataTable table, int index, string scheme, int score)
        {
            DataRow row = table.NewRow();
            row["index"] = index;
            row["item"] = scheme;
            row["score"] = score;
            table.Rows.Add(row);
        }

        private int EvaluateNum(int testCount, int[] test, int issue)
        {
            History _history = LBDataManager.GetDataMgr().History;

            int score = 0;
            for (int i = 0; i < _history.Count; ++ i)
            {
                Lottery lot = _history.Lotteries[i];
                if (lot.Issue > issue)
                    break;
                	
                int match = 0;
		        for (int j = 0; j < testCount; ++ j)
                {
                    if (lot.Scheme.Contains(test[j]))
                        ++ match;
                }

                if (match > 0)
			    {
                    score += SCORES[match - 1];
			    }
            }

            return score;
        }

        private void _putIntoTop10(string[] topNums, int[] topScores, int score, int[] numbers)
        {
            bool bHandled = false;
            for (int i = 9; i >= 0; -- i)
            {
                if (score > topScores[i])
                {
                    if (i < 9)
                    {
                        // insert.
                        int index = 10;
                        while (--index > i + 1)
                        {
                            topNums[index] = topNums[index - 1];
                            topScores[index] = topScores[index - 1];
                        }

                        topScores[i + 1] = score;
                        topNums[i + 1] = _getNumString(numbers);
                    }

                    bHandled = true;
                    break;
                }
            }

            if (!bHandled)
            {
                int index = 10;
                while (--index > 0)
                {
                    topNums[index] = topNums[index - 1];
                    topScores[index] = topScores[index - 1];
                }

                topScores[0] = score;
                topNums[0] = _getNumString(numbers);
            }
        }

        private string _getNumString(int[] numbers)
        {
            string name = "";
            foreach(int num in numbers)
            {
                name += num.ToString().PadLeft(2, '0') + " ";
            }

            return name.Trim();
        }

        private void EvaluateNumberButton_Click(object sender, RoutedEventArgs e)
        {
            int iTestCount = NumCountCombo.SelectedIndex + 1;
            if (iTestCount <= 0 || iTestCount > 6)
                return;

            // get existing.
            int issue = LBDataManager.GetDataMgr().IssueInLocal;
            string existingFile = TargetFileName(issue, AnalysisType.eRedNumGroup, iTestCount);
            if (File.Exists(existingFile))
            {
                string[] lines = File.ReadAllLines(existingFile);
                UpdateRedsGroupResultList(lines);
                return;
            }

            DateTime startTime = DateTime.Now;

            // calculate
            string[] names = new string[10];
            int[] scores = new int[10];
            for(int i = 0; i < 10; ++ i)
            {
                names[i] = "";
                scores[i] = 2147483647 - i;
            }

            int[] numbers = new int[iTestCount];
            for (int inx1 = 1; inx1 <= 34 - iTestCount; inx1++)
            {
                if (iTestCount < 2)
                {
                    numbers[0] = inx1;

                    int score = EvaluateNum(iTestCount, numbers, issue);
                    _putIntoTop10(names, scores, score, numbers);

                    continue;
                }

                for (int inx2 = inx1 + 1; inx2 <= 35 - iTestCount; inx2++)
                {
                    if (iTestCount < 3)
                    {
                        numbers[0] = inx1;
                        numbers[1] = inx2;

                        int score = EvaluateNum(iTestCount, numbers, issue);
                        _putIntoTop10(names, scores, score, numbers);

                        continue;
                    }

                    for (int inx3 = inx2 + 1; inx3 <= 36 - iTestCount; inx3++)
                    {
                        if (iTestCount < 4)
                        {
                            numbers[0] = inx1;
                            numbers[1] = inx2;
                            numbers[2] = inx3;

                            int score = EvaluateNum(iTestCount, numbers, issue);
                            _putIntoTop10(names, scores, score, numbers);

                            continue;
                        }

                        for (int inx4 = inx3 + 1; inx4 <= 37 - iTestCount; inx4++)
                        {
                            if (iTestCount < 5)
                            {
                                numbers[0] = inx1;
                                numbers[1] = inx2;
                                numbers[2] = inx3;
                                numbers[3] = inx4;

                                int score = EvaluateNum(iTestCount, numbers, issue);
                                _putIntoTop10(names, scores, score, numbers);

                                continue;
                            }

                            for (int inx5 = inx4 + 1; inx5 <= 38 - iTestCount; inx5++)
                            {
                                if (iTestCount < 6)
                                {
                                    numbers[0] = inx1;
                                    numbers[1] = inx2;
                                    numbers[2] = inx3;
                                    numbers[3] = inx4;
                                    numbers[4] = inx5;

                                    int score = EvaluateNum(iTestCount, numbers, issue);
                                    _putIntoTop10(names, scores, score, numbers);

                                    continue;
                                }

                                for (int inx6 = inx5 + 1; inx6 <= 39 - iTestCount; inx6++)
                                {
                                    numbers[0] = inx1;
                                    numbers[1] = inx2;
                                    numbers[2] = inx3;
                                    numbers[3] = inx4;
                                    numbers[4] = inx5;
                                    numbers[5] = inx6;

                                    int score = EvaluateNum(iTestCount, numbers, issue);
                                    _putIntoTop10(names, scores, score, numbers);
                                }
                            }
                        }
                    }
                }
            }

            UpdateRedsGroupResultList(names, scores);

            TimeSpan duration = DateTime.Now - startTime;
            MessageBox.Show("Evaluation Done!" + " Duration: " + duration.ToString());

            string[] output = new string[10];
            for (int i = 0; i < 10; ++ i)
            {
                output[i] = names[i] + "\t" + scores[i] + "\r\n";
            }

            File.WriteAllLines(TargetFileName(issue, AnalysisType.eRedNumGroup, iTestCount), output);
        }

        private void NumCountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int issue = LBDataManager.GetDataMgr().IssueInLocal;
            string redGroupfile = TargetFileName(issue, AnalysisType.eRedNumGroup, NumCountCombo.SelectedIndex + 1);
            if (File.Exists(redGroupfile))
            {
                string[] lines = File.ReadAllLines(redGroupfile);
                UpdateRedsGroupResultList(lines);
            }
            else
                EvaluateNumberDetailsReds.DataContext = null;
        }
    }
}
