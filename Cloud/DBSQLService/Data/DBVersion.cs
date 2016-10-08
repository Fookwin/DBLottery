using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DBSQLService.Data
{
    public class DBVersion
    {
        public class VersionInfo
        {
            public DateTime _lastVersionSyncTime = DateTime.UtcNow;
            public int _platform = -1;
            public int _version = -1;
            public bool _forceUpgradingRequired = false;
        }

        private static string PlatformPrefix(PlatformIndex index)
        {
            switch (index)
            {
                case PlatformIndex.winphone:
                    return "wp_";
                case PlatformIndex.winstore:
                    return "ws_";
                case PlatformIndex.androidphone:
                    return "ap_";
                case PlatformIndex.androidtablet:
                    return "at_";
                case PlatformIndex.iphone:
                    return "ip_";
                case PlatformIndex.ipad:
                    return "id_";
                default:
                    return "";
            }
        }

        public static void GetLatestVersion(int platform, ref int version, ref bool forceUpgradingRequired)
        {
            string fileName = "";
            if (platform == (int)PlatformIndex.winstore)
            {
                fileName = "winstore_version.txt";
            }
            else if (platform == (int)PlatformIndex.winphone)
            {
                fileName = "winphone_version.txt";
            }
            else if (platform == (int)PlatformIndex.androidphone)
            {
                fileName = "androidphone_version.txt";
            }
            else if (platform == (int)PlatformIndex.androidtablet)
            {
                fileName = "androidtablet_version.txt";
            }
            else if (platform == (int)PlatformIndex.iphone)
            {
                fileName = "iphone_version.txt";
            }
            else if (platform == (int)PlatformIndex.ipad)
            {
                fileName = "ipad_version.txt";
            }
            else
                throw new Exception("Unsupported Platform.");

            CloudBlockBlob blob = DBCloudStorageClient.Instance().GetBlockBlob("dbreleases", fileName);

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            string[] segs = text.Split(' ');

            version = Convert.ToInt32(segs[0]);
            forceUpgradingRequired = Convert.ToBoolean(segs[1]);
        }

        public static string GetReleaseNotes(int platform, int clientVersion)
        {
            string output = "";

            CloudBlobContainer container = DBCloudStorageClient.Instance().GetBlobContainer("dbreleases");

            string prefix = DBVersion.PlatformPrefix((PlatformIndex)platform);

            var blobList = container.ListBlobs();
            foreach (IListBlobItem item in blobList)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    if (blob.Name.StartsWith(prefix))
                    {
                        int version = Convert.ToInt32(blob.Name.Substring(3, 8)); // xx_xxxxxxxx
                        if (version > clientVersion)
                        {
                            string text;
                            using (var memoryStream = new MemoryStream())
                            {
                                blob.DownloadToStream(memoryStream);
                                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                            }

                            output += text + "\n";
                        }
                    }
                }
            }

            return output;
        }
    }
}