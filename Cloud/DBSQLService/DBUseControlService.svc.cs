using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DBSQLService.Data;

namespace DBSQLService
{
    public class DBUseControlService : IUseControlService
    {
        private static Dictionary<int, DBVersion.VersionInfo> s_VersionCache = new Dictionary<int, DBVersion.VersionInfo>();

        // User management
        //

        public void UserLogin(string devId, int platform, int clientVersion, string info)
        {
            _UserLoginAsync(devId, platform, clientVersion, info);
        }        

        // Software version Control
        //
        public void GetLatestSoftwareVersion(int platform, ref int version, ref bool forceUpgradingRequired)
        {
            _GetLatestSoftwareVersion(platform, ref version, ref forceUpgradingRequired);
        }

        public string GetReleaseNotes(int platform, int clientVersion)
        {
            return DBVersion.GetReleaseNotes(platform, clientVersion);
        }

        public void PostFeedback(string feedback)
        {
            _PostFeedback(feedback);
        }

        public void PostRecord(string record)
        {
            _PostRecord(record);
        }

        // private functions...
        //

        private async void _UserLoginAsync(string devId, int platform, int clientVersion, string info)
        {            
            await Task.Run(delegate
            {
                // login...
                DBDevice.Login(devId, platform, clientVersion, info, "");

                // add login log
                DBDevice.AddLoginLog(devId, DateTime.UtcNow, platform, clientVersion);
            });            
        }

        private void _GetLatestSoftwareVersion(int platform, ref int version, ref bool forceUpgradingRequired)
        {
            if (s_VersionCache.ContainsKey(platform))
            {
                // update the cache each hour.
                TimeSpan offsetSinceLastSync = DateTime.UtcNow - s_VersionCache[platform]._lastVersionSyncTime;
                if (offsetSinceLastSync.TotalMinutes < 60.0)
                {
                    version = s_VersionCache[platform]._version;
                    forceUpgradingRequired = s_VersionCache[platform]._forceUpgradingRequired;
                    return;
                }
            }

            DBVersion.GetLatestVersion(platform, ref version, ref forceUpgradingRequired);

            // update the cache.
            s_VersionCache[platform] = new DBVersion.VersionInfo()
            { 
                _lastVersionSyncTime = DateTime.UtcNow, 
                _forceUpgradingRequired = forceUpgradingRequired, 
                _platform = platform, 
                _version = version 
            };
        }

        public async void _PostFeedback(string feedback)
        {
            await Task.Run(delegate
            {
                DBUserData.AddFeedback(feedback);
            });
        }
        
        public async void _PostRecord(string record)
        {
            await Task.Run(delegate
            {
                DBUserData.AddRecord(record);
            });
        }
    }
}
