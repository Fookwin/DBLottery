using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataModel
{
    [DataContract]
    public class DBDateTime
    {
        [DataMember(Name = "dateTime")]
        private string _dataString;

        public DBDateTime(DateTime date)
        {
            DateTime = date;
        }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            this._dataString = this.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            this._dataString = "1900-01-01 00:00:00";
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            this.DateTime = Convert.ToDateTime(this._dataString);
        }
        
        public DateTime DateTime
        {
            get;
            set;
        }
    }
}
