using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using MessageProcessor.DataModel;

namespace MessageProcessor
{
    class LoginProcesser
    {
        private CloudStorageAccount _storageAccount = null;
        private CloudQueue _userLogQueue = null;
        private CloudTable _userLogTable = null;
        private CloudTable _deviceTable = null;
        private CloudTable _loginTable = null;
        private CloudBlobContainer _loginsContainer = null;

        private CloudStorageAccount Storage()
        {
            if (_storageAccount == null)
                _storageAccount = CloudStorageAccount.Parse(Properties.Settings.Default.AzureStorageConnection);

            return _storageAccount;
        }

        private CloudQueue UserLoginQueue()
        {
            if (_userLogQueue == null)
            {
                CloudStorageAccount storageAccount = Storage();

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                _userLogQueue = queueClient.GetQueueReference("dbusagelog");
            }

            return _userLogQueue;
        }

        private CloudTable UserLoginTable()
        {
            if (_userLogTable == null)
            {
                CloudStorageAccount storageAccount = Storage();

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                _userLogTable = tableClient.GetTableReference("Logins");
            }

            return _userLogTable;
        }

        private CloudTable DeviceTable()
        {
            if (_deviceTable == null)
            {
                CloudStorageAccount storageAccount = Storage();

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                _deviceTable = tableClient.GetTableReference("Devices");
            }

            return _deviceTable;
        }

        private CloudTable LoginTable()
        {
            if (_loginTable == null)
            {
                CloudStorageAccount storageAccount = Storage();

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                _loginTable = tableClient.GetTableReference("Logins");
            }

            return _loginTable;
        }

        private CloudBlobContainer LoginsContainer()
        {
            if (_loginsContainer == null)
            {
                CloudStorageAccount storageAccount = Storage();

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                _loginsContainer = blobClient.GetContainerReference("data-use-log");
            }

            return _loginsContainer;
        }

        private int _Process_HandleLegacy()
        {
            try
            {
                CloudBlobContainer container = LoginsContainer();

                int handledCount = 0;
                var logins = container.ListBlobs();
                foreach (CloudBlob loginBlob in logins)
                {
                    string content;
                    using (var memoryStream = new MemoryStream())
                    {
                        loginBlob.DownloadToStream(memoryStream);
                        content = System.Text.Encoding.ASCII.GetString(memoryStream.ToArray());
                    }

                    string[] lines = content.Split('\n');

                    foreach (string record in lines)
                    {
                        string[] subStrings = record.TrimEnd('\r').Split('\t');
                        if (subStrings.Count() == 2)
                        {
                            if (AddToLoginsTable(subStrings[0], DateTime.Parse(subStrings[1])))
                            {
                                ++handledCount;
                            }
                        }
                    }
                }

                return 20;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write("Failed to process login for " + e.Message);
                return 0;
            }
        }

        public int Process()
        {
            // handle legacy logs first
            int handledCount = _Process_HandleLegacy();

            try
            {
                CloudQueue queue = UserLoginQueue();

                // Fetch the queue attributes.
                queue.FetchAttributes();

                // Retrieve the cached approximate message count.
                int? cachedMessageCount = queue.ApproximateMessageCount;
                int count = cachedMessageCount != null ? cachedMessageCount.Value : 0;

                while (count > 0)
                {
                    var messages = queue.GetMessages(20, TimeSpan.FromMinutes(5));
                    if (messages.Count() <= 0)
                        break;

                    foreach (CloudQueueMessage message in messages)
                    {
                        if (AddToLoginsTable(message.AsString, message.InsertionTime.Value.LocalDateTime))
                        {
                            // Process all messages in less than 5 minutes, deleting each message after processing.
                            queue.DeleteMessage(message);
                            ++handledCount;
                        }

                        count--;
                    }
                }

                return count;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write("Failed to process login for " + e.Message);
                return 0;
            }
        }

        private bool AddToLoginsTable(string msg, DateTime? localTime)
        {
            DBLoginModel login = null;
            if (msg.StartsWith("{"))
            {
                login = JsonParser.Parse<DBLoginModel>(msg);
            }
            else
            {
                string devID = msg;
                if (msg.Contains("login"))
                {
                    // is the message legacy that just contains devid?
                    devID = msg.Substring(0, msg.Count() - 10);
                }

                // find the platform id
                DBDeviceModel device = GetDevice(devID);
                if (device != null)
                {
                    login = new DBLoginModel()
                    {
                        ID = devID,
                        Plateform = device.PlatForm,
                        LoginAt = localTime.Value,
                        Version = device.ClientVersion.ToString()
                    };

                    System.Diagnostics.Trace.Write("Handing " + JsonParser.Stringify(login));
                }
            }

            if (login == null)
                return false;

            // save login model to table as a new item.
            CloudTable table = LoginTable();
            if (!table.Exists())
            {
                return false;
            }

            string queryString = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, login.LoginAt.ToString("yyyyMMddHHmmss"));
            TableQuery<DBLoginTableItem> query = new TableQuery<DBLoginTableItem>().Where(queryString);

            IEnumerable<DBLoginTableItem> result = table.ExecuteQuery(query);
            if (result.Count() > 0)
            {
                // don't add duplicated item.
                return true;
            }

            DBLoginTableItem tableItem = new DBLoginTableItem(login);

            // Add a new item for this user.
            TableOperation operation = TableOperation.Insert(tableItem);
            table.Execute(operation);

            return true;
        }

        private DBDeviceModel GetDevice(string devId)
        {
            // Create the CloudTable object that represents the table. 
            CloudTable table = DeviceTable();
            if (!table.Exists())
            {
                return null;
            }

            // Construct the query operation for allstring devId, int platform, int clientVersion, string info customer entities where PartitionKey=userid. 
            string queryString = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, devId);
            TableQuery<DBDeviceModel> query = new TableQuery<DBDeviceModel>().Where(queryString);

            IEnumerable<DBDeviceModel> result = table.ExecuteQuery(query);
            if (result.Count() == 1)
            {
                // get device infomration.
                DBDeviceModel current = result.Last<DBDeviceModel>();
                return current;
            }

            return null;
        }
    }
}
