using System;
using System.Collections.Generic;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace LuckyBallSpirit.DataModel
{
    class NotificationCenter
    {
        private static NotificationCenter _instance = null;

        private static NotificationCenter Instance()
        {
            if (_instance == null)
            {
                _instance = new NotificationCenter();
            }

            return _instance;
        }

        public static void UpdateTile()
        {
            try
            {
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

                List<Uri> urisToPoll = new List<Uri>(5);
                urisToPoll.Add(new Uri("https://dbdatastorage.blob.core.windows.net/dbnotification/Tile_num_analysis.xml"));
                urisToPoll.Add(new Uri("https://dbdatastorage.blob.core.windows.net/dbnotification/Tile_attribute_analysis.xml"));
                urisToPoll.Add(new Uri("https://dbdatastorage.blob.core.windows.net/dbnotification/Tile_issue_details.xml"));
                urisToPoll.Add(new Uri("https://dbdatastorage.blob.core.windows.net/dbnotification/Tile_Default.xml"));

                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdateBatch(urisToPoll, PeriodicUpdateRecurrence.HalfHour);
            }
            catch
            {
            }
        }

        private NotificationCenter()
        {
        }
    }
}
