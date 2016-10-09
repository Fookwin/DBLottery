using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using LuckyBallsData.Management;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace LuckyBallsData.Util
{
    public class ServerUtil
    {
        public delegate bool ProcessLoginInfoHandler(string info);

        public class Device : TableEntity
        {
            public Device(int platform, string devId, int clientVer, string channelId, string lastLoginDate, string userId)
            {
                this.PartitionKey = platform.ToString();
                this.RowKey = devId;
                ChannalId = channelId;
                ClientVersion = clientVer;
                LastLogin = DateTime.Parse(lastLoginDate);
                UserId = userId;
            }

            public Device()
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

        private static CloudStorageAccount _storageAccount = null;

        private static CloudStorageAccount Storage()
        {
            if (_storageAccount == null)
                _storageAccount = CloudStorageAccount.Parse(Properties.Settings.Default.AzureStorageConnection);

            return _storageAccount;
        }

        public static List<User> GetUsers(int platform)
        {
            CloudStorageAccount storageAccount = Storage();

            // Create the table client. 
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            List<User> output = new List<User>();

            // Create the CloudTable object that represents the table. 
            CloudTable table = tableClient.GetTableReference("Devices");
            if (!table.Exists())
            {
                throw new Exception("Table for user data does not exist.");
            }

            string queryString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, platform.ToString());
            TableQuery<Device> query = new TableQuery<Device>().Where(queryString);

            int pos = 0;
            IEnumerable<Device> result = table.ExecuteQuery(query);
            foreach (Device item in result)
            {  
                output.Add(new User()
                {
                    ID = item.DeviceId,
                    Platform = item.PlatForm,
                    ChannelID = item.ChannalId,
                    LastLoginDate = item.LastLogin.ToLocalTime(),
                    SoftwareVersion = item.ClientVersion,
                    UserID = item.UserId,
                    Index = ++ pos
                });
            }

            output.Sort(delegate(User user1, User user2)
            {
                return user2.LastLoginDate.CompareTo(user1.LastLoginDate);
            });

            return output;
        }    

        public static void PushNotification(int platform, string messsage)
        {
            // Notitify the users registered in notification hub.
            DBManageServiceReference.DBManagementClient mgrClient = new DBManageServiceReference.DBManagementClient();
            mgrClient.PushNotification(platform, messsage, "");
        }

        public static void ProcessLoginInfo(ProcessLoginInfoHandler handler)
        {
            CloudStorageAccount storageAccount = Storage();

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("dbusagelog");
            if (!queue.Exists())
            {
                throw new Exception("Queue does not exist.");
            }

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
                    if (handler != null)
                    {
                        string msg = message.AsString;
                        msg = msg.Substring(0, msg.Count() - 10);
                        msg += "\t" + message.InsertionTime.Value.LocalDateTime.ToString("g") + "\r\n";

                        if (!handler(msg))
                            throw new Exception("Fail to handle the message.");
                    }

                    // Process all messages in less than 5 minutes, deleting each message after processing.
                    queue.DeleteMessage(message);

                    count--;
                }
            }
        }

        public static string UploadToAzureStorage(string source, string targetContainer, string targetFile)
        {
            try
            {
                CloudStorageAccount storageAccount = Storage();

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(targetContainer);
                if (container == null || !container.Exists())
                {
                    return "can't find container: " + targetContainer;
                }

                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(targetFile);

                using (var fileStream = File.OpenRead(source))
                {
                    blockBlob.UploadFromStream(fileStream);
                } 
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return "Done";
        }

        public static string ExecuteSqlQueries(string sqlLines)
        {
            try
            {
                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
                builder["Server"] = "ppuvjzarol.database.windows.net";
                builder["User ID"] = "pi3011314@ppuvjzarol.database.windows.net";
                builder["Password"] = "zzx&jjj1314";

                builder["Database"] = "dbhistory";
                builder["Trusted_Connection"] = false;
                builder["Integrated Security"] = false;
                builder["Encrypt"] = true;

                //1. Define an Exponential Backoff retry strategy for Azure SQL Database throttling (ExponentialBackoff Class). An exponential back-off strategy will gracefully back off the load on the service.
                int retryCount = 4;
                int minBackoffDelayMilliseconds = 2000;
                int maxBackoffDelayMilliseconds = 8000;
                int deltaBackoffMilliseconds = 2000;

                ExponentialBackoff exponentialBackoffStrategy =
                  new ExponentialBackoff("exponentialBackoffStrategy",
                      retryCount,
                      TimeSpan.FromMilliseconds(minBackoffDelayMilliseconds),
                      TimeSpan.FromMilliseconds(maxBackoffDelayMilliseconds),
                      TimeSpan.FromMilliseconds(deltaBackoffMilliseconds));

                //2. Set a default strategy to Exponential Backoff.
                RetryManager manager = new RetryManager(new List<RetryStrategy>
                {  
                    exponentialBackoffStrategy 
                }, "exponentialBackoffStrategy");

                //3. Set a default Retry Manager. A RetryManager provides retry functionality, or if you are using declarative configuration, you can invoke the RetryPolicyFactory.CreateDefault
                RetryManager.SetDefault(manager);

                //4. Define a default SQL Connection retry policy and SQL Command retry policy. A policy provides a retry mechanism for unreliable actions and transient conditions.
                RetryPolicy retryConnectionPolicy = manager.GetDefaultSqlConnectionRetryPolicy();
                RetryPolicy retryCommandPolicy = manager.GetDefaultSqlCommandRetryPolicy();

                //5. Create a function that will retry the connection using a ReliableSqlConnection.
                retryConnectionPolicy.ExecuteAction(() =>
                {
                    using (ReliableSqlConnection connection = new ReliableSqlConnection(builder.ConnectionString))
                    {
                        connection.Open();

                        IDbCommand command = connection.CreateCommand();
                        command.CommandText = sqlLines;

                        //6. Create a function that will retry the command calling ExecuteCommand() from the ReliableSqlConnection
                        retryCommandPolicy.ExecuteAction(() =>
                        {
                            using (IDataReader reader = connection.ExecuteCommand<IDataReader>(command))
                            {
                                while (reader.Read())
                                {
                                    string name = reader.GetString(0);
                                    Console.WriteLine(name);
                                }
                            }
                        });
                    }
                });
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return "Done";
        }
       
    }
}
