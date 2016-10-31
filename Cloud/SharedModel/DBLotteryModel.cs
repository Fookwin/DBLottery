using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DataModel
{
    [DataContract]
    public class DBLotteryModel
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
        private string _dateString;
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            this._dateString = this.Date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            this._dateString = "1900-01-01 00:00:00";
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            this.Date = Convert.ToDateTime(this._dateString);
        }
    }
}