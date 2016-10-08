using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace LuckyBallsServer
{
    public class StorageUtil
    {
        class UserDataItem : TableEntity
        {
            public UserDataItem(string platform, string userId, string chanelUri, string name, string lastLoginDate)
            {
                this.PartitionKey = platform;
                this.RowKey = userId;
                ChannelUri = chanelUri;
                Name = name;
                LastLogin = DateTime.Parse(lastLoginDate);
            }

            public UserDataItem()
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

            public Guid UserId
            {
                get
                {
                    return Guid.Parse(this.RowKey);
                }
                set
                {
                    this.RowKey = value.ToString();
                }
            }

            public string ChannelUri { get; set; }
            public string Name { get; set; }
            public DateTime LastLogin { get; set; }
        }

        private static CloudStorageAccount _storageAccount = null;

        private static CloudStorageAccount Storage()
        {
            if (_storageAccount == null)
                _storageAccount = CloudStorageAccount.Parse(Properties.Settings.Default.AzureStorageConnection);

            return _storageAccount;
        }

        public static string[] GetUserChannels(string platform)
        {
            CloudStorageAccount storageAccount = Storage();

            List<string> channels = new List<string>();

            // Create the table client. 
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            var list = tableClient.ListTables();
            int count = list.Count();

            String tableName = "UserData";

            // Create the CloudTable object that represents the table. 
            CloudTable table = tableClient.GetTableReference(tableName);
            if (!table.Exists())
            {
                throw new Exception("Table for user data does not exist.");
            }

            string queryString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, platform);
            TableQuery<UserDataItem> query = new TableQuery<UserDataItem>().Where(queryString);

            IEnumerable<UserDataItem> result = table.ExecuteQuery(query);
            foreach (UserDataItem item in result)
            {
                if (item.ChannelUri != "")
                    channels.Add(item.ChannelUri);
            }

            return channels.ToArray();
        }
    }
}
