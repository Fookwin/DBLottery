using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataModel
{
    [DataContract]
    public class DBVersionModel
    {
        [DataMember(Name = "latestIssue")]
        public int LatestIssue
        {
            get;
            set;
        }

        [DataMember(Name = "historyDataVersion")]
        public int HistoryDataVersion
        {
            get;
            set;
        }

        [DataMember(Name = "attributeDataVersion")]
        public int AttributeDataVersion
        {
            get;
            set;
        }

        [DataMember(Name = "attributeTemplateVersion")]
        public int AttributeTemplateVersion
        {
            get;
            set;
        }

        [DataMember(Name = "releaseDataVersion")]
        public int ReleaseDataVersion
        {
            get;
            set;
        }

        [DataMember(Name = "latestLotteryVersion")]
        public int LatestLotteryVersion
        {
            get;
            set;
        }

        [DataMember(Name = "matrixDataVersion")]
        public int MatrixDataVersion
        {
            get;
            set;
        }

        [DataMember(Name = "helpContentVersion")]
        public int HelpContentVersion
        {
            get;
            set;
        }
    }
}
