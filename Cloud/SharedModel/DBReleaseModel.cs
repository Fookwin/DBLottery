using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DataModel
{
    [DataContract]
    public class DBReleaseModel
    {
        [DataMember(Name = "issue")]
        public int Issue
        {
            get;
            set;
        }

        [DataMember(Name = "scheme")]
        public string Scheme
        {
            get;
            set;
        }

        [DataMember(Name = "date")]
        public DateTime Date
        {
            get;
            set;
        }

        [DataMember(Name = "bonus")]
        public List<int> Bonus
        {
            get;
            set;
        }

        [DataMember(Name = "bet")]
        public int Bet
        {
            get;
            set;
        }

        [DataMember(Name = "pool")]
        public int Pool
        {
            get;
            set;
        }

        [DataMember(Name = "details")]
        public string Details
        {
            get;
            set;
        }
    }
}