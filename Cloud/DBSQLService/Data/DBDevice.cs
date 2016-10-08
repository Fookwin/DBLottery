using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DBSQLService.Data
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

    public class DBDevice : TableEntity
    {
        public DBDevice(int platform, string devId, int clientVer, string channelId, string lastLoginDate, string userId)
        {
            this.PartitionKey = platform.ToString();
            this.RowKey = devId;
            ChannalId = channelId;
            ClientVersion = clientVer;
            LastLogin = DateTime.Parse(lastLoginDate);
            UserId = userId;
        }

        public DBDevice()
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

        public static async void Login(string devId, int platform, int clientVersion, string chanelUri, string userID)
        {
            String tableName = "Devices";

            // Create the CloudTable object that represents the table. 
            CloudTable table = DBCloudStorageClient.Instance().GetTable(tableName);
            if (!table.Exists())
            {
                throw new Exception("Table for user data does not exist.");
            }

            // Construct the query operation for allstring devId, int platform, int clientVersion, string info customer entities where PartitionKey=userid. 
            string queryString = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, devId);
            TableQuery<DBDevice> query = new TableQuery<DBDevice>().Where(queryString);

            IEnumerable<DBDevice> result = table.ExecuteQuery(query);
            if (result.Count() == 1)
            {
                // register or update the channel for this device.
                string channelID = result.First().ChannalId;
                channelID = await DBNotification.Instance().RegisterChannel((PlatformIndex)platform, chanelUri, channelID);

                // Refresh the user information.
                DBDevice current = result.Last<DBDevice>();
                current.ChannalId = channelID;
                current.ClientVersion = clientVersion;
                current.LastLogin = DateTime.UtcNow;
                current.UserId = userID;

                TableOperation operation = TableOperation.Replace(current);
                table.Execute(operation); // the login date would be updated as well.
            }
            else
            {
                // register or update the channel for this device.
                string channelID = await DBNotification.Instance().RegisterChannel((PlatformIndex)platform, chanelUri, "");

                // Add a new item for this user.
                DBDevice current = new DBDevice(platform, devId, clientVersion, channelID, DateTime.UtcNow.ToString(), userID);

                TableOperation operation = TableOperation.Insert(current);
                table.Execute(operation);
            }
        }

        public static void AddLoginLog(string devId, DateTime loginTime, int plateformIndex, int softVer)
        {
            // build login model.
            DataModel.DBLoginModel login = new DataModel.DBLoginModel()
            {
                ID = devId,
                LoginAt = loginTime,
                Plateform = plateformIndex.ToString(),
                Version = softVer.ToString()
            };

            // save login model to table as a new item.
            CloudTable table = DBCloudStorageClient.Instance().GetTable("Logins");
            if (!table.Exists())
            {
                return;
            }

            DataModel.DBLoginTableItem tableItem = new DataModel.DBLoginTableItem(login);

            try
            {
                // Add a new item for this user.
                TableOperation operation = TableOperation.Insert(tableItem);
                table.Execute(operation);
            }
            catch (Exception e)
            {
                // is it possible that the same item already in the table?
                string queryString = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, tableItem.RowKey);
                TableQuery<DataModel.DBLoginTableItem> query = new TableQuery<DataModel.DBLoginTableItem>().Where(queryString);

                IEnumerable<DataModel.DBLoginTableItem> result = table.ExecuteQuery(query);
                if (result.Count() > 0)
                {
                    throw new Exception(e.Message + " --- Duplicated Row Key: " + tableItem.RowKey);
                }

                // unknown error.
                throw new Exception(e.Message + " --- Unknown failure with :" + "{ DevId : " + devId + ", Key: " + tableItem.RowKey + ", Time: " + loginTime.ToString() + " }");
            }
        }
    }
}