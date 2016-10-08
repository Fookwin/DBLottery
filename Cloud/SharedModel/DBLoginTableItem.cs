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
    class DBLoginTableItem : TableEntity
    {
        private DBLoginModel _dataModel = null;

        public DBLoginTableItem()
        {
            _dataModel = new DBLoginModel();
        }

        public DBLoginTableItem(DBLoginModel data)
        {
            _dataModel = data;
            this.RowKey = _dataModel.LoginAt.Ticks.ToString();
            this.PartitionKey = _dataModel.Plateform;
        }

        public DateTime LoginTime
        {
            get 
            {
                return _dataModel.LoginAt;
            }
            set 
            {
                _dataModel.LoginAt = value;

                // set the time as the row key.
                this.RowKey = value.Ticks.ToString();
            }
        }

        public string ID
        {
            get
            {
                return _dataModel.ID;
            }
            set
            {
                _dataModel.ID = value;
            }
        }

        public string Plateform
        {
            get
            {
                return _dataModel.Plateform;
            }
            set
            {
                _dataModel.Plateform = value;

                // set as partition key.
                this.PartitionKey = value;
            }
        }

        public string Version
        {
            get
            {
                return _dataModel.Version;
            }
            set
            {
                _dataModel.Version = value;
            }
        }

        public DBLoginModel GetModel()
        {
            return _dataModel;
        }
    }
}
