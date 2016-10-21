using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DataModel
{
    [DataContract]
    public class DBRecommendationModel
    {
        [DataMember(Name = "redIncludes")]
        public List<int> RedIncludes
        {
            get;
            set;
        }

        [DataMember(Name = "redExcludes")]
        public List<int> RedExcludes
        {
            get;
            set;
        }

        [DataMember(Name = "blueIncludes")]
        public List<int> BlueIncludes
        {
            get;
            set;
        }

        [DataMember(Name = "blueExcludes")]
        public List<int> BlueExcludes
        {
            get;
            set;
        }
    }
}