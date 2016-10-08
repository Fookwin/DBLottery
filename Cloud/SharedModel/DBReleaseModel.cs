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
        [DataMember]
        public int Issue
        {
            get;
            set;
        }

        [DataMember]
        public List<int> Reds
        {
            get;
            set;
        }

        [DataMember]
        public int Blue
        {
            get;
            set;
        }

        [DataMember]
        public DateTime ReleaseAt
        {
            get;
            set;
        }

        [DataMember]
        public List<int> Bonus
        {
            get;
            set;
        }

        [DataMember]
        public int Bet
        {
            get;
            set;
        }

        [DataMember]
        public int Pool
        {
            get;
            set;
        }

        [DataMember]
        public string Order
        {
            get;
            set;
        }

        [DataMember]
        public string Info
        {
            get;
            set;
        }
    }
}