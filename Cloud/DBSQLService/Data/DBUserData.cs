using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using LuckyBallsData;
using LuckyBallsData.Util;
using LuckyBallsData.UserData;

namespace DBSQLService.Data
{
    public class DBRecord : TableEntity
    {
        public DBRecord(Record rd)
        {
            this.PartitionKey = rd.Issue.ToString();
            this.RowKey = rd.DeviceID;
            Bonus = rd.GetBonus();
            Cost = rd.Cost;
            Prize = rd.Prize;
        }

        public DBRecord()
        {
        }

        public string Bonus
        {
            get;
            set;
        }

        public int Cost
        {
            get;
            set;
        }

        public int Prize
        {
            get;
            set;
        }
    }
    public class DBUserData
    {
        public static void AddFeedback(string feedback)
        {
            // add log.
            String queueName = "dbfeedback";

            // Create the CloudTable object that represents the table. 
            CloudQueue logQueue = DBCloudStorageClient.Instance().GetQueue(queueName);
            if (!logQueue.Exists())
            {
                throw new Exception("Queue for user data does not exist.");
            }

            logQueue.AddMessage(new CloudQueueMessage(feedback));
        }

        public static void AddRecord(string record)
        {
            String tableName = "Records";

            // Create the CloudTable object that represents the table. 
            CloudTable table = DBCloudStorageClient.Instance().GetTable(tableName);
            if (!table.Exists())
            {
                throw new Exception("Table for user data does not exist.");
            }

            // parse the record.
            DBXmlDocument xml = new DBXmlDocument();
            xml.Load(record);

            Record rd = new Record();
            rd.Read(xml.Root());

            // Add a new item for this user.
            DBRecord current = new DBRecord(rd);

            TableOperation operation = TableOperation.Insert(current);
            table.Execute(operation);
        }
    }
}