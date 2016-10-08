using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;

namespace DBSQLService.Data
{
    public class DBNotification
    {
        private static DBNotification s_singleton = null;

        public static DBNotification Instance()
        {
            if (s_singleton == null)
            {
                s_singleton = new DBNotification();
            }
            
            return s_singleton;
        }

        private NotificationHubClient m_notificationHub = null;

        private NotificationHubClient GetNotificationHubClient()
        {
            if (m_notificationHub == null)
            {
                //const string m_connectionListen = "Endpoint=sb://fookwin.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=PmqiBhHPdqvcs/a9nGbAl1LzLx5n7VuA/wBq4kaHTuI=";
                const string m_connectionFull = "Endpoint=sb://fookwin.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=h+2DctyVGFRcf18/IAr5MRz6gdb4fcw695QNZpmbzco=";
                const string m_hubName = "dbnotificationcenter";

                m_notificationHub = NotificationHubClient.CreateClientFromConnectionString(m_connectionFull, m_hubName);
            }

            return m_notificationHub;
        }

        public async Task<string> RegisterChannel(PlatformIndex platform, string channel, string channelID)
        {
            try
            {
                if (platform == PlatformIndex.winstore)
                {
                    var reg = new WindowsRegistrationDescription(channel);

                    if (channelID == "")
                    {
                        channelID = await GetNotificationHubClient().CreateRegistrationIdAsync();
                    }

                    reg.RegistrationId = channelID;
                    reg.Tags = new SortedSet<string>() { FormatTagForPlatform(platform) };

                    // register or update the channel.
                    WindowsRegistrationDescription regDesp = await GetNotificationHubClient().CreateOrUpdateRegistrationAsync(reg);
                    if (regDesp != null)
                        return regDesp.RegistrationId;
                }
                else if (platform == PlatformIndex.winphone)
                {
                    var reg = new MpnsRegistrationDescription(channel);

                    if (channelID == "")
                    {
                        channelID = await GetNotificationHubClient().CreateRegistrationIdAsync();
                    }

                    reg.RegistrationId = channelID;
                    reg.Tags = new SortedSet<string>() { FormatTagForPlatform(platform) };

                    // register or update the channel.
                    MpnsRegistrationDescription regDesp = await GetNotificationHubClient().CreateOrUpdateRegistrationAsync(reg);
                    if (regDesp != null)
                        return regDesp.RegistrationId;        
                }
                else if (platform == PlatformIndex.androidphone)
                {
                    string[] subString = channel.Split(' ');
                    if (subString.Length != 2)
                        return "";

                    var reg = new BaiduRegistrationDescription(subString[0], subString[1], new SortedSet<string>() { FormatTagForPlatform(platform) });

                    if (channelID == "")
                    {
                        channelID = await GetNotificationHubClient().CreateRegistrationIdAsync();
                    }

                    reg.RegistrationId = channelID;

                    // register or update the channel.
                    BaiduRegistrationDescription regDesp = await GetNotificationHubClient().CreateOrUpdateRegistrationAsync(reg);
                    if (regDesp != null)
                        return regDesp.RegistrationId; 
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return "";
        }

        public async void PushNotification(PlatformIndex platform, string message, List<string> toDevices)
        {
            if (toDevices.Count > 0)
            {
                // TODO: notify the specified devcies.
            }

            Notification noti = CreateNotification(platform, message);
            if (noti != null)
            {
                try
                {
                    // temp for testing
                    //string message1 = "{\"title\":\"((Notification title))\",\"description\":\"Hello from Azure\"}";
                    //var result = await GetNotificationHubClient().SendBaiduNativeNotificationAsync(message1);

                    NotificationOutcome res = await GetNotificationHubClient().SendNotificationAsync(noti, new List<string> { FormatTagForPlatform(platform) });
                }
                catch (Exception e)
                {
                    string mest = e.Message;
                }
            }
        }

        private string FormatTagForPlatform(PlatformIndex platform)
        {
            return "platform-" + platform.ToString();
        }

        private Notification CreateNotification(PlatformIndex platform, string message)
        {
            switch (platform)
            {
                case PlatformIndex.winstore:
                    {
                        WindowsNotification notification = new WindowsNotification(message);
                        return notification;
                    }
                case PlatformIndex.winphone:
                    {
                        MpnsNotification notification = new MpnsNotification(message);
                        return notification;
                    }
                case PlatformIndex.androidphone:
                    {
                        BaiduNotification notification = new BaiduNotification(message);
                        return notification;
                    }
                default:
                    return null;
            }
        }
    }
}