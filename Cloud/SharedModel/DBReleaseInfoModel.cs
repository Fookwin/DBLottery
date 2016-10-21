using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DataModel
{
    [DataContract]
    public class DBReleaseInfoModel
    {
        [DataMember(Name = "issue")]
        public int Issue
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

        [DataMember(Name = "cutOffTime")]
        public DateTime CutOffTime
        {
            get;
            set;
        }

        [DataMember(Name = "lotteryTime")]
        public DateTime LotteryTime
        {
            get;
            set;
        }
    }
}