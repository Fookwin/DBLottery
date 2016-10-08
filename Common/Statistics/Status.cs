using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuckyBallsData.Util;

namespace LuckyBallsData.Statistics
{
    public class NumberState
    {
	    private int _hitCount = 0;
        private int _omission = 0;
        private int _temperature = 0; // <2 -> cool; >=2 & <= 8 -> normal; > 8 hot;

        public int HitCount
        {
            get
            {
                return _hitCount;
            }
            set
            {
                _hitCount = value;
            }
        }

        public int Omission
        {
            get
            {
                return _omission;
            }
            set
            {
                _omission = value;
            }
        }

        public int Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
            }
        }
    }

    public class AttributeState
    {
        private string _key = "";
        private int _value = 0;

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }

    public class Status
    {
        private NumberState[] _redNumStates = new NumberState[33];
        private NumberState[] _blueNumStates = new NumberState[16];
        private Dictionary<string, AttributeState> _attributeStates = null;

        public NumberState[] RedNumStates
        {
            get
            {
                return _redNumStates;
            }
        }

        public NumberState[] BlueNumStates
        {
            get
            {
                return _blueNumStates;
            }
        }

        public Dictionary<string, AttributeState> AttributeStates
        {
            get
            {
                if (_attributeStates == null)
                {
                    ConstructAttributes();
                }

                return _attributeStates;
            }
        }
    
        public void SaveToXml(DBXmlNode node)
        {
            string reds_hit = "", reds_omission = "", reds_temp = "";
            for ( int i = 1; i <= 33; ++ i)
            {
                reds_hit += _redNumStates[i - 1].HitCount.ToString() + " ";
                reds_omission += _redNumStates[i - 1].Omission.ToString() + " ";
                reds_temp += _redNumStates[i - 1].Temperature.ToString() + " ";
            }

            node.SetAttribute("Red_HitCount", reds_hit);
            node.SetAttribute("Red_Omission", reds_omission);
            node.SetAttribute("Red_Temperature", reds_temp);

            string blues_hit = "", blues_omission = "", blues_temp = "";
            for (int i = 1; i <= 16; ++i)
            {
                blues_hit += _blueNumStates[i - 1].HitCount.ToString() + " ";
                blues_omission += _blueNumStates[i - 1].Omission.ToString() + " ";
                blues_temp += _blueNumStates[i - 1].Temperature.ToString() + " ";
            }

            node.SetAttribute("Blue_HitCount", blues_hit);
            node.SetAttribute("Blue_Omission", blues_omission);
            node.SetAttribute("Blue_Temperature", blues_temp);

            string attributeValues = "";
            foreach (KeyValuePair<string, AttributeState> state in _attributeStates)
            {
                attributeValues += state.Value.Value.ToString() + " ";
            }

            node.SetAttribute("Attribute_Value", attributeValues);
        }

        public void ReadFromXml(DBXmlNode node)
        {
            string[] reds_hit = node.GetAttribute("Red_HitCount").Split(' ');
            string[] reds_omission = node.GetAttribute("Red_Omission").Split(' ');
            string[] reds_temp = node.GetAttribute("Red_Temperature").Split(' ');

            for (int i = 0; i < 33; ++i)
            {
                if (_redNumStates[i] == null)
                    _redNumStates[i] = new NumberState();

                _redNumStates[i].HitCount = Convert.ToInt32(reds_hit[i]);
                _redNumStates[i].Omission = Convert.ToInt32(reds_omission[i]);
                _redNumStates[i].Temperature = Convert.ToInt32(reds_temp[i]);
            }

            string[] blues_hit = node.GetAttribute("Blue_HitCount").Split(' ');
            string[] blues_omission = node.GetAttribute("Blue_Omission").Split(' ');
            string[] blues_temp = node.GetAttribute("Blue_Temperature").Split(' ');

            for (int i = 0; i < 16; ++i)
            {
                if (_blueNumStates[i] == null)
                    _blueNumStates[i] = new NumberState();

                _blueNumStates[i].HitCount = Convert.ToInt32(blues_hit[i]);
                _blueNumStates[i].Omission = Convert.ToInt32(blues_omission[i]);
                _blueNumStates[i].Temperature = Convert.ToInt32(blues_temp[i]);
            }


            string[] attribute_value = node.GetAttribute("Attribute_Value").TrimEnd().Split(' ');
            if (AttributeStates.Count() != attribute_value.Count())
                throw new Exception("The defined attributes do not match with the input data.");

            string names = node.GetAttribute("Attribute_Name");
            if (names != null && names != "")
            {
                string[] attribute_names = names.TrimEnd().Split(' ');

                int count = AttributeStates.Count;
                for (int i = 0; i < count; ++i)
                {
                    AttributeState state = AttributeStates[attribute_names[i]];
                    state.Value = Convert.ToInt32(attribute_value[i]);
                }
            }
            else
            {
                int count = AttributeStates.Count;
                for (int i = 0; i < count; ++i)
                {
                    AttributeState state = AttributeStates.ElementAt(i).Value;
                    state.Value = Convert.ToInt32(attribute_value[i]);
                } 
            }
        }

        private void ConstructAttributes()
        {
            _attributeStates = new Dictionary<string, AttributeState>();

            List<string> keys = AttributeUtil.AttributeKeys();
            foreach (string key in keys)
            {
                _attributeStates.Add(key, new AttributeState() { Key = key });
            }
        }
    }
}
