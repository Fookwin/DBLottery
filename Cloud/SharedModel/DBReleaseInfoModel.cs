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
        private string _dateString;
        public DateTime Date
        {
            get;
            set;
        }

        [DataMember(Name = "cutOffTime")]
        private string _cutOffDateString;
        public DateTime CutOffTime
        {
            get;
            set;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            this._dateString = this.Date.ToString("yyyy-MM-dd HH:mm:ss");
            this._cutOffDateString = this.CutOffTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            this._dateString = "1900-01-01 00:00:00";
            this._cutOffDateString = "1900-01-01 00:00:00";
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            this.Date = Convert.ToDateTime(this._dateString);
            this.CutOffTime = Convert.ToDateTime(this._cutOffDateString);
        }
    }
}