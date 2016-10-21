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
        [DataMember(Name = "currentIssue")]
        public int CurrentIssue
        {
            get;
            set;
        }

        [DataMember(Name = "nextIssue")]
        public int NextIssue
        {
            get;
            set;
        }

        [DataMember(Name = "nextReleaseTime")]
        public DateTime NextReleaseTime
        {
            get;
            set;
        }

        [DataMember(Name = "sellOffTime")]
        public DateTime SellOffTime
        {
            get;
            set;
        }

        [DataMember(Name = "lottery")]
        public DBLotteryModel Lottery
        {
            get;
            set;
        }

        [DataMember(Name = "recommendation")]
        public DBRecommendationModel Recommendation
        {
            get;
            set;
        }

        [DataMember(Name = "dateVersion")]
        public DBVersionModel DataVersion
        {
            get;
            set;
        }
    }
}