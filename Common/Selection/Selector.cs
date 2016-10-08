using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;
using System.Diagnostics;

namespace LuckyBallsData.Selection
{
    public enum SchemeSelectorTypeEnum
    {
        StandardSelectorType = 0,
        DantuoSelectorType = 1,
        RandomSelectorType = 2,
        UploadSelectorType = 3
    }

    public abstract class SchemeSelector
    {
        public string Expression
        {
            get
            {
                return GetExpression(); 
            }
        }

        public List<SchemeSelector> BasicSelectors
        {
            get
            {
                return GetBasicSelectors();
            }
        }

        public string DisplayExpression
        {
            get
            {
                return GetDisplayExpression();
            }
        }

        public abstract SchemeSelectorTypeEnum GetSelectorType();
        protected abstract string GetExpression();
        public abstract void ParseExpression(string exp);
        protected abstract string GetDisplayExpression();
        public abstract List<Scheme> GetResult();
        public abstract int GetSchemeCount();
        public abstract SchemeSelector Clone();
        public abstract void Reset();
        protected abstract List<SchemeSelector> GetBasicSelectors();
        public abstract string HasError();

        static public SchemeSelector CreateSelector(SchemeSelectorTypeEnum type)
        {
            switch (type)
            {
                case SchemeSelectorTypeEnum.StandardSelectorType:
                    return new StandardSchemeSelector();
                case SchemeSelectorTypeEnum.DantuoSelectorType:
                    return new DantuoSchemeSelector();
                case SchemeSelectorTypeEnum.RandomSelectorType:
                    return new RandomSchemeSelector();
                case SchemeSelectorTypeEnum.UploadSelectorType:
                default:
                    throw new Exception("Unsupported selector type.");
            }
        }
    }

    public class StandardSchemeSelector : SchemeSelector
    {
        public enum RedBlueConnectionTypeEnum
        {
            Duplicate = 0,
            OneToOneInOrder = 1,
            OneToOneInRandom = 2
        }

        private Set selected_reds = new Set();
        private Set selected_blues = new Set();
        private bool applyMatrixFilter = false;
        private RedBlueConnectionTypeEnum connectType = RedBlueConnectionTypeEnum.Duplicate;

        public Set SelectedReds
        {
            get { return selected_reds; }
            set { selected_reds = value; }
        }

        public Set SelectedBlues
        {
            get { return selected_blues; }
            set { selected_blues = value; }
        }

        public bool ApplyMatrixFilter
        {
            get { return applyMatrixFilter; }
            set { applyMatrixFilter = value; }
        }

        public RedBlueConnectionTypeEnum BlueConnectionType
        {
            get { return connectType; }
            set { connectType = value; }
        }

        public StandardSchemeSelector()
        {
        }

        public override SchemeSelectorTypeEnum GetSelectorType()
        {
            return SchemeSelectorTypeEnum.StandardSelectorType;
        }

        protected override string GetExpression()
        {
            string exp = SelectedReds.ToString() + "+" + SelectedBlues.ToString();
            exp += "+" + applyMatrixFilter.ToString() + "+" + ((int)connectType).ToString();
            return exp;
        }

        public override void ParseExpression(string exp)
        {
            string[] sub = exp.Split('+');
            SelectedReds = new Set(sub[0]);
            SelectedBlues = new Set(sub[1]);
            if (sub.Count() == 4)
            {
                applyMatrixFilter = Convert.ToBoolean(sub[2]);
                connectType = (RedBlueConnectionTypeEnum)(Convert.ToInt32(sub[3]));
            }
        }

        protected override string GetDisplayExpression()
        {
            string display = selected_reds.Count != 6 ? "[复式] " : "[单式] ";
            display += "[" + this.GetSchemeCount().ToString() + "注] ";
            display += selected_reds.DisplayExpression;
            display += " : " + selected_blues.DisplayExpression;

            switch (connectType)
            {
                case RedBlueConnectionTypeEnum.Duplicate:
                    display += " <蓝球复式>";
                    break;
                case RedBlueConnectionTypeEnum.OneToOneInOrder:
                    display += " <顺序选择>";
                    break;
                case RedBlueConnectionTypeEnum.OneToOneInRandom:
                    display += " <随机选择>";
                    break;
            }

            if (applyMatrixFilter)
                display += " [选6中5缩水]";

            return display;
        }

        public override int GetSchemeCount()
        {
            int redCount = SelectedReds.Count;
            if (redCount < 6)
                return 0;

            int blueCount = SelectedBlues.Count;
            if (blueCount <= 0)
                return 0;

            int count = 0;
            if (applyMatrixFilter)
            {
                count = DataManageBase.Instance().MatrixTable.GetCellItemCount(redCount, 6);
            }
            else
            {
                const int devide = 720;

                count = 1;
                for (int i = 0; i < 6; ++i)
                    count *= redCount - i;

                count /= devide;
            }

            return connectType == RedBlueConnectionTypeEnum.Duplicate ? count * blueCount : count;
        }

        public override List<Scheme> GetResult()
        {
            return SelectUtil.CalculateSchemeSelection(selected_reds, SelectedBlues,
                connectType != RedBlueConnectionTypeEnum.Duplicate,
                connectType == RedBlueConnectionTypeEnum.OneToOneInRandom,
                applyMatrixFilter);
        }

        public override SchemeSelector Clone()
        {
            SchemeSelector copy = new StandardSchemeSelector()
            { 
                selected_reds = new Set(this.selected_reds),
                selected_blues = new Set(this.selected_blues),
                connectType = this.BlueConnectionType,
                applyMatrixFilter = this.applyMatrixFilter,
            };

            return copy;
        }

        public override void Reset()
        {
            selected_reds.Clear();
            selected_blues.Clear();
            applyMatrixFilter = false;
            connectType = RedBlueConnectionTypeEnum.Duplicate;
        }

        protected override List<SchemeSelector> GetBasicSelectors()
        {
            return new List<SchemeSelector> { this.Clone() };
        }

        public override string HasError()
        {
            if (selected_reds.Count < 6)
                return "至少需要选择6个红球";

            if (selected_blues.Count < 1)
                return "至少需要选择1个蓝球";

            return "";
        }
    }

    public class DantuoSchemeSelector : SchemeSelector
    {
        private Set selected_tuos = new Set();
        private Set selected_dans = new Set();
        private Set selected_blues = new Set();

        public Set SelectedTuos
        {
            get { return selected_tuos; }
            set { selected_tuos = value; }
        }

        public Set SelectedDans
        {
            get { return selected_dans; }
            set { selected_dans = value; }
        }

        public Set SelectedBlues
        {
            get { return selected_blues; }
            set { selected_blues = value; }
        }

        public DantuoSchemeSelector()
        {
        }

        public override SchemeSelectorTypeEnum GetSelectorType()
        {
            return SchemeSelectorTypeEnum.DantuoSelectorType;
        }

        protected override string GetExpression()
        {
            string expression = "";
            if (selected_dans.Count > 0)
                expression = "(" + selected_dans.ToString() + ")";

            return expression + SelectedTuos.ToString() + "+" + SelectedBlues.ToString();
        }

        public override void ParseExpression(string exp)
        {
            int separator1 = exp.IndexOf(')');
            if (separator1 > 0)
            {
                selected_dans = new Set(exp.Substring(1, separator1 - 1));
            }

            int separator2 = exp.IndexOf('+');
            SelectedTuos = new Set(exp.Substring(separator1 + 1, separator2 - separator1 - 1));
            SelectedBlues = new Set(exp.Substring(separator2 + 1, exp.Length - separator2 - 1));
        }

        public override int GetSchemeCount()
        {
            int tuoCount = SelectedTuos.Count;
            if (tuoCount + selected_dans.Count  < 6)
                return 0;

            int blueCount = SelectedBlues.Count;
            if (blueCount <= 0)
                return 0;

            int danCount = selected_dans.Count;
            if (danCount == 0)
            {
                const int devide = 720;

                int total = 1;
                for (int i = 0; i < 6; ++i)
                    total *= tuoCount - i;

                total /= devide;

                return total * blueCount;
            }
            else
            {
                int total = 1;
                for (int i = 0; i < (6 - danCount); ++i)
                    total *= tuoCount - i;

                int devide = 1;
                for (int i = 6 - danCount; i > 0; --i)
                    devide *= i;

                total /= devide;

                return total * blueCount;
            }
        }

        public override List<Scheme> GetResult()
        {
            return SelectUtil.CalculateSchemeSelection(selected_dans, selected_tuos, SelectedBlues);
        }

        public override SchemeSelector Clone()
        {
            SchemeSelector copy = new DantuoSchemeSelector()
            {
                selected_tuos = new Set(this.selected_tuos),
                selected_dans = new Set(this.selected_dans),
                selected_blues = new Set(this.selected_blues)
            };

            return copy;
        }

        public override void Reset()
        {
            selected_tuos.Clear();
            selected_dans.Clear();
            selected_blues.Clear();
        }

        protected override List<SchemeSelector> GetBasicSelectors()
        {
            return new List<SchemeSelector> { this.Clone() };
        }

        protected override string GetDisplayExpression()
        {
            string display = "[胆拖] ";
            display += "[" + this.GetSchemeCount().ToString() + "注] ";
            display += "(" + selected_dans.DisplayExpression + ") ";
            display += selected_tuos.DisplayExpression;
            display += " : " + selected_blues.DisplayExpression;
            return display;
        }

        public override string HasError()
        {
            if (selected_dans.Count + selected_tuos.Count < 6)
                return "至少需要选择6个红球";

            if (selected_dans.Count > 5)
                return "最多只能选5个胆号红球";

            if (selected_blues.Count < 1)
                return "至少需要选择1个蓝球";

            return "";
        }
    }

    public class RandomSchemeSelector : SchemeSelector
    {
        private Set included_reds = new Set();
        private Set excluded_reds = new Set();
        private Set included_blues = new Set();
        private Set excluded_blues = new Set();
        private int selectedCount = 1;

        public int SelectedCount
        {
            get { return selectedCount; }
            set { selectedCount = value; }
        }

        public Set IncludedReds
        {
            get { return included_reds; }
            set { included_reds = value; }
        }

        public Set ExcludedReds
        {
            get { return excluded_reds; }
            set { excluded_reds = value; }
        }

        public Set IncludedBlues
        {
            get { return included_blues; }
            set { included_blues = value; }
        }

        public Set ExcludedBlues
        {
            get { return excluded_blues; }
            set { excluded_blues = value; }
        }

        public RandomSchemeSelector()
        {
        }

        public override SchemeSelectorTypeEnum GetSelectorType()
        {
            return SchemeSelectorTypeEnum.RandomSelectorType;
        }

        protected override string GetExpression()
        {
            string expression = "";
            if (included_reds.Count > 0)
                expression = "(" + included_reds.ToString() + ")";

            if (excluded_reds.Count > 0)
                expression = "[" + excluded_reds.ToString() + "]";

            expression += "+";

            if (included_blues.Count > 0)
                expression = "(" + included_blues.ToString() + ")";

            if (excluded_blues.Count > 0)
                expression = "[" + excluded_blues.ToString() + "]";

            return expression;
        }

        public override void ParseExpression(string exp)
        {
            int separator0 = exp.IndexOf('+');

            string reds = exp.Substring(0, separator0);
            if (reds.Length > 0)
            {
                int separator1 = reds.IndexOf(')');
                if (separator1 > 0)
                {
                    // included...
                    included_reds = new Set(reds.Substring(1, separator1 - 1));
                }

                int separator2 = reds.IndexOf('[');
                if (separator2 > 0)
                {
                    // excluded...
                    excluded_reds = new Set(reds.Substring(separator2 + 1, reds.Length - separator2 - 2));
                }
            }

            string blues = exp.Substring(separator0 + 1, exp.Length - separator0 - 1);
            if (blues.Length > 0)
            {
                int separator1 = blues.IndexOf(')');
                if (separator1 > 0)
                {
                    // included...
                    included_blues = new Set(blues.Substring(1, separator1 - 1));
                }

                int separator2 = blues.IndexOf('[');
                if (separator2 > 0)
                {
                    // excluded...
                    excluded_blues = new Set(blues.Substring(separator2 + 1, blues.Length - separator2 - 2));
                }
            }
        }

        public override int GetSchemeCount()
        {
            if (excluded_reds.Count <= 27 && excluded_blues.Count <= 15)
                return selectedCount;

            return 0;
        }

        public override List<Scheme> GetResult()
        {
            return SelectUtil.RandomSchemes(selectedCount, this);
        }

        public override SchemeSelector Clone()
        {
            SchemeSelector copy = new RandomSchemeSelector()
            {
                included_reds = new Set(this.included_reds),
                excluded_reds = new Set(this.excluded_reds),
                included_blues = new Set(this.included_blues),
                excluded_blues = new Set(this.excluded_blues),
                selectedCount = this.selectedCount
            };

            return copy;
        }

        public override void Reset()
        {
            included_reds.Clear();
            excluded_reds.Clear();
            included_blues.Clear();
            excluded_blues.Clear();
            selectedCount = 1;
        }

        protected override List<SchemeSelector> GetBasicSelectors()
        {
            List<SchemeSelector> result = new List<SchemeSelector>();

            List<Scheme> selected = GetResult();
            foreach (Scheme item in selected)
            {
                StandardSchemeSelector sel = new StandardSchemeSelector();
                sel.SelectedReds = new Set(new int[6] { item.Red(0), item.Red(1), item.Red(2), item.Red(3), item.Red(4), item.Red(5) });
                sel.SelectedBlues.Add(item.Blue);

                result.Add(sel);
            }
            return result;
        }

        protected override string GetDisplayExpression()
        {
            return "";
        }

        public override string HasError()
        {
            if (included_reds.Count > 6)
                return "选为必出的红球不能超过6个";

            if (excluded_reds.Count > 27)
                return "选为不出的红球不能超过27个";

            if (included_blues.Count > 1)
                return "选为必出蓝球只能有1个";

            if (excluded_blues.Count > 15)
                return "选为不出的蓝球不能超过15个";

            if(selectedCount == 0)
                return "至少要选一注";

            return "";
        }
    }
}
