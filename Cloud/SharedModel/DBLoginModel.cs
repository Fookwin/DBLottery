using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.WindowsAzure.Storage.Table;

namespace DataModel
{
    [DataContract]
    class DBLoginModel
    {
        [DataMember]
        public DateTime LoginAt
        {
            get;
            set;
        }

        [DataMember]
        public string ID
        {
            get;
            set;
        }

        [DataMember]
        public string Plateform
        {
            get;
            set;
        }

        [DataMember]
        public string Version
        {
            get;
            set;
        }
    }
}
