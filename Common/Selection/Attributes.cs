using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;

namespace LuckyBallsData.Selection
{
    public class SchemeAttributes
    {
        private Dictionary<string, SchemeAttributeCategory> _categories = new Dictionary<string,SchemeAttributeCategory>();

        public SchemeAttributes()
        {
        }

        public SchemeAttributeCategory Category(string name)
        {
            if (_categories.ContainsKey(name))
                return _categories[name];

            return null;
        }

        public Dictionary<string, SchemeAttributeCategory> Categories
        {
            get
            {
                return _categories;
            }
        }

        public SchemeAttributeCategory AddCategory(SchemeAttributeCategory cat)
        {
            if (cat != null)
                _categories.Add(cat.Name, cat);

            return cat;
        }

        public SchemeAttributes Clone()
        {
            SchemeAttributes copy = new SchemeAttributes();

            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in _categories)
            {
                copy.AddCategory(cat.Value.Clone());
            }

            return copy;
        }

        public SchemeAttribute Attribute(string category, string key)
        {
            SchemeAttributeCategory cat = Category(category);
            if (cat == null)
                return null;

            return cat.Attribute(key);
        }

        // Use Attribute(string category, string key) if you know the [aremt category. 
        public SchemeAttribute Attribute(string key)
        {
            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in _categories)
            {
                SchemeAttribute temp = cat.Value.Attribute(key);
                if (temp != null)
                    return temp;
            }

            return null;
        }

        public void SaveAsTemplate(ref DBXmlNode node)
        {
            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in _categories)
            {
                DBXmlNode catNode = node.AddChild(cat.Key);
                cat.Value.SaveAsTemplate(ref catNode);
            }
        }

        public void ReadFromTemplate(DBXmlNode node)
        {
            _categories.Clear();

            List<DBXmlNode> catNodes = node.ChildNodes();
            foreach (DBXmlNode catNode in catNodes)
            {
                SchemeAttributeCategory cat = new SchemeAttributeCategory();
                cat.ReadFromTemplate(catNode);

                _categories.Add(catNode.Name(), cat);
            }
        }

        public void SaveValueToXml(ref DBXmlNode node)
        {
            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in _categories)
            {
                DBXmlNode catNode = node.AddChild(cat.Key);
                cat.Value.SaveValueToXml(ref catNode);
            }
        }

        public void ReadValueFromXml(DBXmlNode node)
        {
            List<DBXmlNode> catNodes = node.ChildNodes();
            foreach (DBXmlNode catNode in catNodes)
            {
                SchemeAttributeCategory cat = Category(catNode.Name());
                cat.ReadValueFromXml(catNode);
            }
        }
    }

    public class SchemeAttributeCategory
    {
        private Dictionary<string, SchemeAttribute> _attributes = new Dictionary<string, SchemeAttribute>();

        public string Name
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public Dictionary<string, SchemeAttribute> Attributes
        {
            get
            {
                return _attributes;
            }
        }

        public SchemeAttribute Attribute(string Key)
        {
            if (_attributes.ContainsKey(Key))
                return _attributes[Key];    
    
            return null;
        }

        public void AddAttribute(SchemeAttribute attri)
        {
            if (_attributes.ContainsKey(attri.Key))
                throw new Exception("The attribute with the same key already exists.");

            _attributes.Add(attri.Key, attri);
        }

        public void RemoveAttribute(string Key)
        {
            if (_attributes.ContainsKey(Key))
                _attributes.Remove(Key);
        }

        public SchemeAttributeCategory Clone()
        {
            SchemeAttributeCategory copy = new SchemeAttributeCategory() { Name = this.Name, DisplayName = this.DisplayName };

            foreach (KeyValuePair<string, SchemeAttribute> pair in _attributes)
            {
                copy.AddAttribute(pair.Value.Clone());
            }

            return copy;
        }

        public void SaveAsTemplate(ref DBXmlNode node)
        {
            node.SetAttribute("Display", DisplayName);            
            foreach (KeyValuePair<string, SchemeAttribute> pair in _attributes)
            {
                DBXmlNode attriNode = node.AddChild(pair.Key);
                pair.Value.SaveAsTemplate(ref attriNode);
            }
        }

        public void ReadFromTemplate(DBXmlNode node)
        {
            Name = node.Name();
            DisplayName = node.GetAttribute("Display");

            _attributes.Clear();
            List<DBXmlNode> attiNodes = node.ChildNodes();
            foreach (DBXmlNode attiNode in attiNodes)
            {
                SchemeAttribute attri = new SchemeAttribute();
                attri.ReadFromTemplate(attiNode);

                _attributes.Add(attiNode.Name(), attri);
            }
        }

        public void SaveValueToXml(ref DBXmlNode node)
        {
            foreach (KeyValuePair<string, SchemeAttribute> pair in _attributes)
            {
                DBXmlNode attriNode = node.AddChild(pair.Key);
                pair.Value.SaveValueToXml(ref attriNode);
            }
        }

        public void ReadValueFromXml(DBXmlNode node)
        {
            if (_attributes.Count == 0)
                throw new Exception("Attributes Not Initialized.");

            List<DBXmlNode> attiNodes = node.ChildNodes();
            foreach (DBXmlNode attiNode in attiNodes)
            {
                SchemeAttribute attri = Attribute(attiNode.Name());
                attri.ReadValueFromXml(attiNode);
            }
        }
    }

    public class SchemeAttribute
    {
        private List<SchemeAttributeValueStatus> _valueStates;
        private bool _needParseValues = false;
        private DBXmlNode _xmlValueNode = null;

        public SchemeAttribute()
        {
            HelpID = -1;
        }

        public string Key
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public Region ValidRegion
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int HelpID
        {
            get;
            set;
        }
        
        public List<SchemeAttributeValueStatus> ValueStates
        {
            get
            {
                if (_needParseValues)
                    ParseValues();

                return _valueStates; 
            }
            set { _valueStates = value; }
        }

        public void ReadFromTemplate(DBXmlNode node)
        {
            Key = node.Name();
            DisplayName = node.GetAttribute("Display");
            ValidRegion = new Region(node.GetAttribute("Region"));
            Description = node.GetAttribute("Description");
            string helpId = node.GetAttribute("HelpID");
            if (helpId != null && helpId != "")
                HelpID = Convert.ToInt32(helpId);

            if (_valueStates == null)
                _valueStates = new List<SchemeAttributeValueStatus>();

            _valueStates.Clear();
            
            foreach (DBXmlNode stateNode in node.ChildNodes())
            {
                SchemeAttributeValueStatus state = new SchemeAttributeValueStatus(this);
                state.ReadFromTemplate(stateNode);

                _valueStates.Add(state);
            }
        }

        public void SaveAsTemplate(ref DBXmlNode node)
        {
            node.SetAttribute("Display", DisplayName);
            node.SetAttribute("Region", ValidRegion.ToString());
            node.SetAttribute("Description", Description);
            node.SetAttribute("HelpID", HelpID.ToString());

            foreach (SchemeAttributeValueStatus state in _valueStates)
            {
                DBXmlNode stateNode = node.AddChild("State");
                state.SaveAsTemplate(ref stateNode);
            }
        }

        public void ReadValueFromXml(DBXmlNode node)
        {
            _xmlValueNode = node;
            _needParseValues = true;
        }

        private void ParseValues()
        {
            int index = 0;
            foreach (DBXmlNode stateNode in _xmlValueNode.ChildNodes())
            {
                SchemeAttributeValueStatus state = _valueStates[index++];
                state.ReadValueFromXml(stateNode);
            }

            _needParseValues = false;
        }

        public void SaveValueToXml(ref DBXmlNode node)
        {
            foreach (SchemeAttributeValueStatus state in ValueStates)
            {
                DBXmlNode stateNode = node.AddChild("State");
                state.SaveValueToXml(ref stateNode);
            }
        }

        public SchemeAttribute Clone()
        {
            SchemeAttribute copy = new SchemeAttribute()
            {
                Key = this.Key,
                DisplayName = this.DisplayName,
                Description = this.Description,
                ValidRegion = this.ValidRegion,
                HelpID = this.HelpID,
                ValueStates = new List<SchemeAttributeValueStatus>()
            };

            foreach (SchemeAttributeValueStatus state in ValueStates)
            {
                copy.ValueStates.Add(state.Clone());
            }

            return copy;
        }
    }

    public class SchemeAttributeValueStatus : IComparable<SchemeAttributeValueStatus>
    {
        private SchemeAttribute _parent;
        private int _hitCount = 0;
        private double _hitProbability = 0.0;
        private double _idealProbility = -1.0;
        private double _averageOmission = 0.0;
        private int _maxOmission = 0;
        private int _immediateOmission = 0;
        private double _protentialEnergy = 0.0;

        public int CompareTo(SchemeAttributeValueStatus other)
        {
            // If other is not a valid object reference, this instance is greater. 
            if (other == null) return 1;

            // The temperature comparison depends on the comparison of  
            // the underlying Double values.  
            return _protentialEnergy.CompareTo(other._protentialEnergy);
        }


        public string DisplayName
        {
            get 
            {
                return _parent.DisplayName + " = [" + ValueExpression + "]";
            }
        }

        public Region ValueRegion
        {
            get;
            set;
        }

        public string ValueExpression
        {
            get;
            set;
        }

        public SchemeAttribute Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public int HitCount
        {
            get { return _hitCount; }
            set { _hitCount = value; }
        }

        public double HitProbability
        {
            get { return _hitProbability; }
            set { _hitProbability = value; }
        }

        public double IdealProbility
        {
            get { return _idealProbility; }
            set { _idealProbility = value; }
        }

        public double AverageOmission
        {
            get { return _averageOmission; }
            set { _averageOmission = value; }
        }

        public int MaxOmission
        {
            get { return _maxOmission; }
            set { _maxOmission = value; }
        }

        public int ImmediateOmission
        {
            get { return _immediateOmission; }
            set { _immediateOmission = value; }
        }

        public double ProtentialEnergy
        {
            get { return _protentialEnergy; }
            set { _protentialEnergy = value; }
        }

        public SchemeAttributeValueStatus()
        {
            _parent = null;
        }

        public SchemeAttributeValueStatus(SchemeAttribute parent_Attri)
        {
            _parent = parent_Attri;
        }

        public void ReadValueFromXml(DBXmlNode node)
        {
            string value = node.GetAttribute("Value");
            string[] data = value.Split(',');
            if (data.Count() != 7)
                throw new Exception("Attribute Value Count is not correct.");

            int index = 0;
            HitCount = Convert.ToInt32(data[index++]);
            HitProbability = Convert.ToDouble(data[index++]);
            IdealProbility = Convert.ToDouble(data[index++]);
            AverageOmission = Convert.ToDouble(data[index++]);
            MaxOmission = Convert.ToInt32(data[index++]);
            ImmediateOmission = Convert.ToInt32(data[index++]);
            ProtentialEnergy = Convert.ToDouble(data[index++]);     
        }

        public void SaveValueToXml(ref DBXmlNode node)
        {
            node.SetAttribute("Value", ToString());
        }

        public override string ToString()
        {
            string value = "";

            value += HitCount.ToString() + ",";
            value += HitProbability.ToString("F1") + ",";
            value += IdealProbility.ToString("F1") + ",";
            value += AverageOmission.ToString("F1") + ",";
            value += MaxOmission.ToString() + ",";
            value += ImmediateOmission.ToString() + ",";
            value += ProtentialEnergy.ToString("F1");

            return value;
        }

        public void ReadFromTemplate(DBXmlNode node)
        {
            ValueExpression = node.GetAttribute("Expression");
            ValueRegion = new Region(node.GetAttribute("Region"));
        }

        public void SaveAsTemplate(ref DBXmlNode node)
        {
            node.SetAttribute("Expression", ValueExpression);
            node.SetAttribute("Region", ValueRegion.ToString());
        }

        public SchemeAttributeValueStatus Clone()
        {
            return new SchemeAttributeValueStatus(_parent)
            {
                ValueExpression = this.ValueExpression,
                ValueRegion = new Region(this.ValueRegion.Min, this.ValueRegion.Max),
                HitCount = this.HitCount,
                HitProbability = this.HitProbability,
                IdealProbility = this.IdealProbility,
                AverageOmission = this.AverageOmission,
                MaxOmission = this.MaxOmission,
                ImmediateOmission = this.ImmediateOmission,
                ProtentialEnergy = this.ProtentialEnergy
            };
        }
    }
}
