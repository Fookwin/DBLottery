using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;

namespace LuckyBallsData.Selection
{
    public enum ConstraintTypeEnum
    {
        SchemeAttributeConstraintType = 0,
        RedNumSetConstraintType = 1,
        HistoryDuplicateConstraintType = 2
    }

    public abstract class Constraint
    {
        public Constraint()
        {            
        }

        public virtual bool Meet(Scheme lucyNum, int refIssueIndex)
        {
            return false;
        }

        public string DisplayExpression
        {
            get
            {
                return GetDisplayExpression();
            }
        }

        public abstract Constraint Clone();
        public abstract void ReadFromXml(DBXmlNode node);
        public abstract void WriteToXml(ref DBXmlNode node);
        public abstract ConstraintTypeEnum GetConstraintType();
        protected abstract string GetDisplayExpression();
        public abstract string HasError();

        static public Constraint CreateConstraint(ConstraintTypeEnum type)
        {
            switch (type)
            {
                case ConstraintTypeEnum.RedNumSetConstraintType:
                    return new RedNumSetConstraint();
                case ConstraintTypeEnum.SchemeAttributeConstraintType:
                    return new SchemeAttributeConstraint();
                case ConstraintTypeEnum.HistoryDuplicateConstraintType:
                    return new HistoryDuplicateConstraint();
                default:
                    throw new Exception("Unsupported constraint type.");
            }
        }
    }    

    public class SchemeAttributeConstraint : Constraint
    {
        private string _attributeKey = "";
        private string _attributeName = "";
        protected Set _validNumbers;

        public SchemeAttributeConstraint()
        {
        }

        public SchemeAttributeConstraint(string attributeKey, string attributeName, Set set)
        {
             _validNumbers = set;
            _attributeKey = attributeKey;
            _attributeName = attributeName;
        }

        public override ConstraintTypeEnum GetConstraintType()
        {
            return ConstraintTypeEnum.SchemeAttributeConstraintType;
        }

        public string AttributeKey
        {
            get
            {
                return _attributeKey;
            }
        }

        public Set Values
        {
            get
            {
                return _validNumbers;
            }
            set
            {
                _validNumbers = value;
            }
        }

        public override Constraint Clone()
        {
           return new SchemeAttributeConstraint(_attributeKey, _attributeName, new Set(_validNumbers));
        }

        protected override string GetDisplayExpression()
        {
            // Get the corresponding attribute.
            SchemeAttribute template = AttributeUtil.GetSchemeAttribute(_attributeKey);

            string valueExp = "";
            foreach (SchemeAttributeValueStatus value in template.ValueStates)
            {
                if (_validNumbers.Contains(value.ValueRegion))
                {
                    if (valueExp != "")
                        valueExp += ", ";
                    valueExp += value.ValueExpression;
                }
            }

            return "[属性过滤] " + _attributeName + " = [" + valueExp + "]";
        }

        public override bool Meet(Scheme num, int refIssueIndex)
        {
            return _validNumbers.Contains(num.Attribute(_attributeKey, refIssueIndex));
        } 

        public override void ReadFromXml(DBXmlNode node)
        {
            if (_validNumbers == null)
                _validNumbers = new Set();

            _attributeKey = node.GetAttribute("Attribute");
            _attributeName = node.GetAttribute("AttributeName");

            DBXmlNode numberNode = node.FirstChildNode("Values");
            _validNumbers.Parse(numberNode.GetAttribute("Expression"));
        }

        public override void WriteToXml(ref DBXmlNode node)
        {
            node.SetAttribute("Attribute", _attributeKey);
            node.SetAttribute("AttributeName", _attributeName);

            DBXmlNode numberNode = node.AddChild("Values");
            numberNode.SetAttribute("Expression", _validNumbers.ToString());
        }

        public override string HasError()
        {
            if (_attributeKey.Length == 0)
                return "请选择一个属性";

            if (_validNumbers.Count == 0)
                return "请选择至少一个属性值";

            return "";
        }
    }

    public class RedNumSetConstraint : Constraint
    {
        private Set _selectSet = new Set();
        private Set _hitLimits = new Set();

        public override ConstraintTypeEnum GetConstraintType()
        {
            return ConstraintTypeEnum.RedNumSetConstraintType;
        }

        public override bool Meet(Scheme num, int refIssueIndex)
        {
            int[] numbers = _selectSet.Numbers;

            int iHit = 0;
            foreach (int test in numbers)
            {
                if (num.Contains(test))
                    iHit++;
            }

            return _hitLimits.Contains(iHit);
        }

        public override Constraint Clone()
        {
            return new RedNumSetConstraint()
            {
                _selectSet = new Set(this._selectSet),
                _hitLimits = new Set(this._hitLimits)
            };
        }

        public Set SelectSet
        {
            get
            {
                return _selectSet;
            }
            set
            {
                _selectSet = value;
            }
        }

        public Set HitLimits
        {
            get
            {
                return _hitLimits;
            }

            set
            {
                _hitLimits = value;
            }
        }

        protected override string GetDisplayExpression()
        {
            return "[号码组过滤] " + "红球" + "(" + _selectSet.DisplayExpression + ")" + "出现" + "(" + _hitLimits.DisplayExpression + ")" +"次"; 
        }

        public override void ReadFromXml(DBXmlNode node)
        {
            string selection = node.GetAttribute("SelectSet");
            _selectSet.Parse(selection);

            string limits = node.GetAttribute("HitLimits");
            _hitLimits.Parse(limits);
        }

        public override void WriteToXml(ref DBXmlNode node)
        {
            node.SetAttribute("SelectSet", _selectSet.ToString());
            node.SetAttribute("HitLimits", _hitLimits.ToString());
        }

        public override string HasError()
        {
            if (_selectSet.Count == 0)
                return "请选择至少一个号码";

            if (_hitLimits.Count == 0)
                return "请选择所选号码出现的个数";

            int[] values = _hitLimits.Numbers;
            foreach (int val in values)
            {
                if (val > _selectSet.Count)
                {
                    return "出现个数超过了号码个数！";
                }
            }

            return "";
        }
    }

    public class HistoryDuplicateConstraint : Constraint
    {
        private int _referencedIssueCount = -1; // -1 for all.
        private int _condition = 6;

        public override ConstraintTypeEnum GetConstraintType()
        {
            return ConstraintTypeEnum.HistoryDuplicateConstraintType;
        }

        public override bool Meet(Scheme num, int refIssueIndex)
        {
            List<Lottery> history = DataManageBase.Instance().History.Lotteries;

            int index = _referencedIssueCount < 0 ? 0 : refIssueIndex - _referencedIssueCount + 1;

            for (; index <= refIssueIndex; ++index)
            {
                if (num.Similarity(history[index].Scheme) >= _condition)
                    return false;
            }

            return true;
        }

        public override Constraint Clone()
        {
            return new HistoryDuplicateConstraint()
            {
                _referencedIssueCount = this._referencedIssueCount,
                _condition = this._condition
            };
        }

        public int ReferenceCount
        {
            get
            {
                return _referencedIssueCount;
            }
            set
            {
                _referencedIssueCount = value;
            }
        }

        public int ExcludeCondition
        {
            get
            {
                return _condition;
            }

            set
            {
                _condition = value;
            }
        }

        protected override string GetDisplayExpression()
        {
            string reference = _referencedIssueCount < 0 ? "[所有开奖中]" : "[最近" + _referencedIssueCount.ToString() + "期]";
            return "[历史过滤] " + "排除与" + reference + "有[" + _condition.ToString() + "]个以上红球相同的号码组";
        }

        public override void ReadFromXml(DBXmlNode node)
        {
            _referencedIssueCount = Convert.ToInt32(node.GetAttribute("Reference"));

            _condition = Convert.ToInt32(node.GetAttribute("Condition"));
        }

        public override void WriteToXml(ref DBXmlNode node)
        {
            node.SetAttribute("Reference", _referencedIssueCount.ToString());
            node.SetAttribute("Condition", _condition.ToString());
        }

        public override string HasError()
        {
            return ""; // should no error for any value.
        }
    }
}


