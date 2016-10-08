using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using LuckyBallsData.Util;
using LuckyBallsData.Statistics;
using LuckyBallsData.Selection;
using LuckyBallsData;
using System.Security.AccessControl;

namespace LuckyBallsData.Util
{
    public class AttributeManager
    {
        private const int Version = 1;
        private static HashSet<string> s_blueAttributeCatKeys = new HashSet<string>()
        {
            "Blue_BasicAttribute",
            "Blue_Matrix",
            "Blue_Mantissa_Repeat_Previous"
        };

        private Dictionary<string, SchemeAttributes> _lotteryAttributes = new Dictionary<string, SchemeAttributes>();
        private string _location = "";

        public bool DataChanged
        {
            get;
            set;
        }
        
        public AttributeManager(string location)
        {
            DataChanged = false;

            // Formate the location.
            _location = location.TrimEnd(new char[] { '/', '\\' });
            _location += "/";
        }

        public SchemeAttributes Attributes(int issue)
        {
            if (!_lotteryAttributes.ContainsKey(issue.ToString()))
                return null;

            if (_lotteryAttributes[issue.ToString()] == null)
            {
                // Load it from local.
                string attriFilePath = _location + issue.ToString() + ".xml";
                if (File.Exists(attriFilePath))
                {
                    _lotteryAttributes[issue.ToString()] =  AttributeUtil.GetAttributesTemplate().Clone();

                    DBXmlDocument xml = new DBXmlDocument();

                    try
                    {
                        xml.Load(attriFilePath);
                    }
                    catch (Exception e)
                    {
                        string str = e.Message;
                        return null;
                    }

                    DBXmlNode root = xml.Root();
                    _lotteryAttributes[issue.ToString()].ReadValueFromXml(root);
                }
            }
            
            return _lotteryAttributes[issue.ToString()];
        }

        public SchemeAttributes LatestAttributes()
        {
            return Attributes(DataManageBase.Instance().History.LastIssue);
        }

        public void Init()
        {
            if (_location == "")
                return;

            // Get lotteries.
            List<Lottery> lots = DataManageBase.Instance().History.Lotteries;

            foreach (Lottery lot in lots)
            {
                string attriFilePath = _location + lot.Issue.ToString() + ".xml";
                if (!File.Exists(attriFilePath))
                {
                    SchemeAttributes attriSet = AddAttributesForLottery(lot.Issue, lot.Scheme);
                }
                else
                {
                    _lotteryAttributes.Add(lot.Issue.ToString(), null); // set it as null, so that it could be delay loaded when it is used in future.
                }
            }
        }

        public void SaveToLocal(string location)
        {
            if (!DataChanged)
                return;

            if (location == "")
                return;

            Directory.CreateDirectory(location);

            foreach (KeyValuePair<string, SchemeAttributes> pair in _lotteryAttributes)
            {
                string attriFilePath = _location + pair.Key + ".xml";
                if (!File.Exists(attriFilePath))
                {
                    DBXmlDocument xml = new DBXmlDocument();
                    DBXmlNode root = xml.AddRoot("Attributes");

                    pair.Value.SaveValueToXml(ref root);

                    // Save it to local for testing...
                    xml.Save(attriFilePath);
                }
            }
        }

        public void SaveLatestAttributes(string file)
        {
            // save the attributes for latest issue.
            SchemeAttributes last = LatestAttributes();

            DBXmlDocument xml = new DBXmlDocument();
            DBXmlNode root = xml.AddRoot("Attributes");

            last.SaveValueToXml(ref root);

            // Save it to local for testing...
            xml.Save(file);
        }  

        public SchemeAttributes AddAttributesForLottery(int issue, Scheme scheme)
        {
            if (_lotteryAttributes.ContainsKey(issue.ToString()))
                return _lotteryAttributes[issue.ToString()];

            // Get the previous.
            SchemeAttributes previousSet = _lotteryAttributes.Count > 0 ? Attributes(Convert.ToInt32(_lotteryAttributes.Last().Key)) : null;

            // Create the attributes.
            SchemeAttributes attributeSet = previousSet == null ? AttributeUtil.GetAttributesTemplate().Clone() : previousSet.Clone();

            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in attributeSet.Categories)
            {
                foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Value.Attributes)
                {
                    int value = scheme.Attribute(attri.Key, _lotteryAttributes.Count - 1);

                    // Update the states.
                    foreach (SchemeAttributeValueStatus status in attri.Value.ValueStates)
                    {
                        double totalOmission = status.AverageOmission * status.HitCount;                        

                        if (status.ValueRegion.Contains(value))
                        {
                            status.HitCount ++;
                            status.ImmediateOmission = 0;
                        }
                        else
                        {
                            status.ImmediateOmission++;
                            totalOmission++;
                        }

                        if (status.ImmediateOmission > status.MaxOmission)
                            status.MaxOmission = status.ImmediateOmission;

                        status.AverageOmission = status.HitCount == 0 ? -1.0 : totalOmission / status.HitCount;
                        status.HitProbability = ((double)status.HitCount * 100) / (_lotteryAttributes.Count + 1);

                        if (status.AverageOmission <= 0.0 || status.MaxOmission == status.AverageOmission)
                            status.ProtentialEnergy = 0;
                        else
                        {
                            //status.ProtentialEnergy = (status.ImmediateOmission - status.AverageOmission) / (status.MaxOmission - status.AverageOmission);
                            status.ProtentialEnergy = status.ImmediateOmission / status.AverageOmission;
                        }
                    }
                }
            }

            _lotteryAttributes.Add(issue.ToString(), attributeSet);

            DataChanged = true;

            return attributeSet;
        }

        public enum EvaluateSchemeOptionEnum
        {
            eAll = 0,
            eReds = 1,
            eBlue = 2
        }

        public int EvaluateScheme(SchemeAttributes testAttributes, Scheme scheme, EvaluateSchemeOptionEnum option)
        {
            double score = 0.0;
            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in testAttributes.Categories)
            {
                if (option == EvaluateSchemeOptionEnum.eAll || (option == EvaluateSchemeOptionEnum.eBlue == s_blueAttributeCatKeys.Contains(cat.Key)))
                {
                    foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Value.Attributes)
                    {
                        int value = scheme.Attribute(attri.Key, _lotteryAttributes.Count - 1);

                        // Update the states.
                        foreach (SchemeAttributeValueStatus status in attri.Value.ValueStates)
                        {
                            if (status.ValueRegion.Contains(value))
                            {
                                score += status.ProtentialEnergy;
                                break;
                            }
                        }
                    }
                }
            }

            return Convert.ToInt32(score);
        }
    }
}
