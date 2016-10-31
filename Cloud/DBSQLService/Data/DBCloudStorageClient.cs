using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DBSQLService.Data
{
    public class DBCloudStorageClient
    {
        private static DBCloudStorageClient s_singleton = null;

        private CloudStorageAccount m_storageAccount = CloudStorageAccount.Parse(Properties.Settings.Default.StorageConnectionString);
        private CloudTableClient m_tableClient = null;
        private CloudBlobClient m_blobClient = null;
        private CloudQueueClient m_queueClient = null;

        public static DBCloudStorageClient Instance()
        {
            if (s_singleton == null)
                s_singleton = new DBCloudStorageClient();

            return s_singleton;
        }

        private CloudTableClient GetTableClient()
        {
            if (m_tableClient == null)
                m_tableClient = m_storageAccount.CreateCloudTableClient();

            return m_tableClient;
        }

        private CloudBlobClient GetBlobClient()
        {
            if (m_blobClient == null)
                m_blobClient = m_storageAccount.CreateCloudBlobClient();

            return m_blobClient;
        }

        private CloudQueueClient GetQueueClient()
        {
            if (m_queueClient == null)
                m_queueClient = m_storageAccount.CreateCloudQueueClient();

            return m_queueClient;
        }

        public CloudTable GetTable(string tableName)
        {
            // Create the CloudTable object that represents the table. 
            CloudTable table = GetTableClient().GetTableReference(tableName);
            if (table == null || !table.Exists())
            {
                return null;
            }

            return table;
        }

        public CloudQueue GetQueue(string queueName)
        {
            CloudQueue queue = GetQueueClient().GetQueueReference(queueName);
            if (queue == null || !queue.Exists())
                return null;

            return queue;
        }

        public CloudBlockBlob GetBlockBlob(string containerName, string blobName)
        {
            CloudBlobContainer container = GetBlobContainer(containerName);
            if (container != null)
            {
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                if (blob == null)
                    return null;

                return blob;
            }

            return null;
        }

        public CloudBlobContainer GetBlobContainer(string containerName)
        {
            CloudBlobContainer container = GetBlobClient().GetContainerReference(containerName);
            if (container != null && container.Exists())
            {
                return container;
            }

            return null;
        }

        public string ReadBlobAsString(string blobName, string fileName)
        {
            try
            {
                CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob(blobName, fileName);

                string text = null;
                using (var memoryStream = new MemoryStream())
                {
                    blob.DownloadToStream(memoryStream);
                    text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                return text;
            }
            catch
            {
                return null;
            }
        }

        public bool WriteTextAsBlob(string blobName, string fileName, string text, bool async = true)
        {
            try
            {
                CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob(blobName, fileName);
                if (async)
                    blob.UploadTextAsync(text);
                else
                    blob.UploadText(text);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}