﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DataModel
{
    [DataContract]
    public class DBReleaseModel
    {
        [DataMember(Name = "next")]
        public DBReleaseInfoModel NextRelease
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

        [DataMember(Name = "dataVersion")]
        public DBVersionModel DataVersion
        {
            get;
            set;
        }
    }
}