using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace MessageProcessor.DataModel
{
    class DBDeviceModel : TableEntity
    {
        public enum PlatformIndex
        {
            // DON'T change the value of the member!
            winstore = 1,
            winphone = 2,
            androidphone = 3,
            androidtablet = 4,
            iphone = 5,
            ipad = 6
        }

        public DBDeviceModel(int platform, string devId, int clientVer, string channelId, string lastLoginDate, string userId)
        {
            this.PartitionKey = platform.ToString();
            this.RowKey = devId;
            ChannalId = channelId;
            ClientVersion = clientVer;
            LastLogin = DateTime.Parse(lastLoginDate);
            UserId = userId;
        }

        public DBDeviceModel()
        {
        }

        public string PlatForm
        {
            get
            {
                return this.PartitionKey;
            }
            set
            {
                this.PartitionKey = value;
            }
        }

        public string DeviceId
        {
            get
            {
                return this.RowKey;
            }
            set
            {
                this.RowKey = value;
            }
        }

        public string ChannalId { get; set; }
        public DateTime LastLogin { get; set; }
        public int ClientVersion { get; set; }
        public string UserId { get; set; }
    }
}
