using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallData.LBCloudService;
using LuckyBallData.LBUseManageService;

namespace LuckyBallsData.Util
{
    public class SoftwareVersion
    {
        public int Version = -1;
        public bool SchemeChanged = false;
    }

    class LBCloudServiceClient
    {
        private const int s_platform = 2; // win store.

        private static SqlServiceClient _dataClient = null;
        private static UseControlServiceClient _useCtrlClient = null;

        public static SqlServiceClient OpenDataService()
        {
            if (_dataClient == null)
            {
                _dataClient = new SqlServiceClient();
                _dataClient.OpenAsync();
            }

            return _dataClient;
        }

        public static void CloseDataService()
        {
            if (_dataClient != null)
            {
                _dataClient.CloseAsync();
                _dataClient = null;
            }
        }

        public static UseControlServiceClient OpenUserCtrlService()
        {
            if (_useCtrlClient == null)
            {
                _useCtrlClient = new UseControlServiceClient();
            }

            return _useCtrlClient;
        }

        public static void CloseUserCtrlService()
        {
            if (_useCtrlClient != null)
            {
                _useCtrlClient.CloseAsync();
                _useCtrlClient = null;
            }
        }

        public static Task<string> GetDataVersionAsync()
        {
            SqlServiceClient client = OpenDataService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<GetDataVersionCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetDataVersionCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.GetDataVersionCompleted += handler;
            client.GetDataVersionAsync();

            return tcs.Task;
        }

        public static Task<string> GetLotteryDataAsync(int issue)
        {
            SqlServiceClient client = OpenDataService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<GetLotteryDataCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetLotteryDataCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.GetLotteryDataCompleted += handler;
            client.GetLotteryDataAsync(issue);

            return tcs.Task;
        }

        public static Task<string> GetAllLotteriesAsync()
        {
            SqlServiceClient client = OpenDataService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<GetAllLotteriesCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetAllLotteriesCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.InnerChannel.OperationTimeout = new TimeSpan(0, 1000, 0);
            client.GetAllLotteriesCompleted += handler;
            client.GetAllLotteriesAsync();

            return tcs.Task;
        }

        public static Task<string> GetLotteriesByIssueAsync(int from, int to)
        {
            SqlServiceClient client = OpenDataService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<GetLotteriesByIssueCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetLotteriesByIssueCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.GetLotteriesByIssueCompleted += handler;
            client.GetLotteriesByIssueAsync(from, to);

            return tcs.Task;
        }

        public static Task<string> GetAttributesTemplateAsync()
        {
            SqlServiceClient client = OpenDataService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<GetAttributesTemplateCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetAttributesTemplateCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.GetAttributesTemplateCompleted += handler;
            client.GetAttributesTemplateAsync();

            return tcs.Task;
        }

        public static Task<string> GetLatestAttributesAsync()
        {
            SqlServiceClient client = OpenDataService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<GetLatestAttributesCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetLatestAttributesCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.GetLatestAttributesCompleted += handler;
            client.GetLatestAttributesAsync();

            return tcs.Task;
        }

        public static Task<string> GetLatestReleaseInfoAsync()
        {
            SqlServiceClient client = OpenDataService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<GetLatestReleaseInfoCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetLatestReleaseInfoCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.GetLatestReleaseInfoCompleted += handler;
            client.GetLatestReleaseInfoAsync();

            return tcs.Task;
        }

        public static Task<string> GetMatrixTableItemAsync(int candidateCount, int selectCount)
        {
            SqlServiceClient client = OpenDataService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<GetMatrixTableItemCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetMatrixTableItemCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.GetMatrixTableItemCompleted += handler;
            client.GetMatrixTableItemAsync(candidateCount, selectCount);

            return tcs.Task;
        }

        public static Task<SoftwareVersion> GetLatestSoftwareVersion()
        {
            UseControlServiceClient client = OpenUserCtrlService();

            var tcs = new TaskCompletionSource<SoftwareVersion>();

            EventHandler<LuckyBallData.LBUseManageService.GetLatestSoftwareVersionCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetLatestSoftwareVersionCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    SoftwareVersion version = new SoftwareVersion();
                    version.Version = e.version;
                    version.SchemeChanged = e.forceUpgradingRequired;

                    tcs.SetResult(version);
                }
            };

            int tempV = 0;
            bool tempSC = false;
            client.GetLatestSoftwareVersionCompleted += handler;
            client.GetLatestSoftwareVersionAsync(s_platform, tempV, tempSC);            

            return tcs.Task;
        }

        public static Task<string> GetReleaseNotes(int fromVer)
        {
            UseControlServiceClient client = OpenUserCtrlService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<LuckyBallData.LBUseManageService.GetReleaseNotesCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.GetReleaseNotesCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };

            client.GetReleaseNotesCompleted += handler;
            client.GetReleaseNotesAsync(s_platform, fromVer);

            return tcs.Task;
        }

        public static Task<string> Login(string deviceID, int curVersion, string channelUri)
        {
            UseControlServiceClient client = OpenUserCtrlService();

            var tcs = new TaskCompletionSource<string>();

            EventHandler<System.ComponentModel.AsyncCompletedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                client.UserLoginCompleted -= handler;

                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
            };

            client.UserLoginCompleted += handler;
            client.UserLoginAsync(deviceID, s_platform, curVersion, channelUri);

            return tcs.Task;
        }
    }
}
