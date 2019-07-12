using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DataModel
{
    [DataContract]
    public class DBAttributesModel
    {
        public static string[] s_VauleMatrix = {
            "HitCount",
            "HitProbability",
            "IdealProbility",
            "AverageOmission",
            "MaxOmission",
            "ImmediateOmission",
            "ProtentialEnergy"
        };

        [DataMember]
        public List<DBAttributeCategoryModel> Categories
        {
            get;
            set;
        } = new List<DBAttributeCategoryModel>();

        [DataMember]
        public string[] ValueMatrix
        {
            get;
        } = s_VauleMatrix;
    }

    [DataContract]
    public class DBAttributeCategoryModel
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public string Display
        {
            get;
            set;
        }

        [DataMember]
        public List<DBAttributeModel> Attributes
        {
            get;
            set;
        } = new List<DBAttributeModel>();
    }

    [DataContract]
    public class DBAttributeModel
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public string Display
        {
            get;
            set;
        }

        [DataMember]
        public string Region
        {
            get;
            set;
        }

        [DataMember]
        public string Description
        {
            get;
            set;
        }

        [DataMember]
        public int HID
        {
            get;
            set;
        }

        [DataMember]
        public List<DBAttributeStateModel> States
        {
            get;
            set;
        } = new List<DBAttributeStateModel>();
    }

    [DataContract]
    public class DBAttributeStateModel
    {
        [DataMember]
        public string Expression
        {
            get;
            set;
        }

        [DataMember]
        public string Value
        {
            get;
            set;
        }

        [DataMember]
        public string Region
        {
            get;
            set;
        }
    }
}