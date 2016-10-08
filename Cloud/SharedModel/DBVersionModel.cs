using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataModel
{
    [DataContract]
    class DBVersionModel
    {
        [DataMember]
        public int CurrentIssue
        {
            get;
            set;
        }

        [DataMember]
        public int NextIssue
        {
            get;
            set;
        }

        [DataMember]
        public DateTime NextReleaseTime
        {
            get;
            set;
        }

        [DataMember]
        public DateTime SellOffTime
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<string, int> DataVersion
        {
            get;
            set;
        }
    }
}
