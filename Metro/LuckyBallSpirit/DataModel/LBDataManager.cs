using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data;
using LuckyBallsData.Statistics;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;
using LuckyBallsData;
using LuckyBallsData.UserData;
using LuckyBallsData.Filters;
using LuckyBallsData.Configration;
using Windows.Data.Json;
using System.Net.NetworkInformation;

namespace LuckyBallSpirit.DataModel
{
    class LBDataManager : LuckyBallsData.DataManageBase
    {   
        private DataUtil.SyncContext _context = null;
        private bool _initialized = false;

        public delegate void DataInitializedEventHandler(string message, int progress);
        public event DataInitializedEventHandler OnDataInitalized = null;

        private SchemeAttributes _latestAttributes = null;
        private PurchaseManager _purchaseMgr = null;
        private LotteryInfo _latestLotteryInfo = null;
        private LotteryStateInfo _latestLotteryStateInfo = null;
        private PurchaseInfo _pendingPurchase = null;
        private ReleaseInfo _releaseInfo = null;
        private Lottery _latestLottery = null;
        private AttributeFilterOption _filterOption = null;

        private object thisLockObject = new object();

        private List<LotteryWebSite> _webSites = null;

        private LBDataManager()
        {
        }

        static public LBDataManager GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new LBDataManager();
            }

            return _Instance as LBDataManager;
        }

        public DataUtil.SyncContext GetSyncContext()
        {
            return _context;
        }

        public bool Initialized
        {
            get
            {
                return _initialized;
            }
        }

        public async Task Initialize()
        {
            if (_initialized)
                return;

            try
            {
                // Step 1: Check net work connection.
                if (OnDataInitalized != null)
                    OnDataInitalized("加载数据，请稍后...", 5);

                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    // net work is not available.
                    throw new Exception("没有连接网络, 请稍后重试");
                }

                // Init the context.
                _context = await DataUtil.ConstructContext();
                if (_context == null)
                {
                    // the service is not available.
                    throw new Exception("无法连接服务器, 请稍后重试");
                }

                // Step 2: Check software version.
                if (OnDataInitalized != null)
                    OnDataInitalized("检查版本更新...", 10);

                DataUtil.SoftwareVersion latestSoftVer = await DataUtil.GetLatestSoftwareVersion();
                int currentSoftwareVersion = DataUtil.CurrentSoftwareVersion();
                if (latestSoftVer != null && latestSoftVer.Version > currentSoftwareVersion)
                {
                    // Get the release notes.
                    string notes = await DataUtil.GetReleaseNotes(currentSoftwareVersion);

                    Controls.UpdateNotification notification = new Controls.UpdateNotification();
                    notification.Show(currentSoftwareVersion.ToString(),
                        latestSoftVer.Version.ToString(),
                        notes,//res.Result,
                        latestSoftVer.SchemeChanged);

                    if (latestSoftVer.SchemeChanged)
                        throw new Exception("请升级到新版本");
                }

                // Step 3: initialize global members.
                if (OnDataInitalized != null)
                    OnDataInitalized("加载数据模板...", 15);

                if (AttributeUtil.GetAttributesTemplate() == null)
                {
                    SchemeAttributes template = await DataUtil.GetAttributesTemplate(_context);
                    AttributeUtil.SetAttributesTemplate(template);
                }

                // Step 4: release information.
                if (OnDataInitalized != null)
                    OnDataInitalized("加载最新开奖结果...", 30);

                if (_releaseInfo == null)
                {
                    DBXmlDocument releaseInfoXml = await DataUtil.GetLatestReleaseInfo(_context);
                    if (releaseInfoXml != null)
                    {
                        _releaseInfo = new ReleaseInfo();
                        _releaseInfo.Read(releaseInfoXml.Root());
                    }
                    else
                    {
                        throw new Exception("无法获取开奖结果，请稍后重试");
                    }
                }

                // Update the version file in disk.
                DataUtil.SaveVersion(_context.CloudVersion);

                // Read the latest lottery.
                if (_latestLottery == null)
                {
                    _latestLottery = await DataUtil.ReadLatestLottery(_context);
                }

                // Step 4: initialiazing the scheme attributes
                if (OnDataInitalized != null)
                    OnDataInitalized("加载最新分析数据...", 50);

                if (_filterOption == null)
                {
                    _filterOption = new AttributeFilterOption();

                    // try to read the customized setting.
                    string settingStrin = await DataUtil.GetAttributeFilterSetting();
                    if (settingStrin != null)
                    {
                        try
                        {
                            DBXmlDocument xml = new DBXmlDocument();
                            xml.Load(settingStrin);

                            DBXmlNode root = xml.Root();
                            _filterOption.Read(root);
                        }
                        catch
                        {
                            // if fail, just use the default setting.
                        }
                    }
                }

                if (_latestAttributes == null)
                {
                    _latestAttributes = await DataUtil.ReadSchemeAttribute(_context);
                    if (_latestAttributes == null)
                    {
                        throw new Exception("无法加载分析数据，请稍后重试");
                    }
                }

                // Step 5:  initialiazing the history.
                if (OnDataInitalized != null)
                    OnDataInitalized("加载历史数据 (第一次会比较慢）...", 60);

                if (_History == null)
                {
                    _History = await DataUtil.ReadLotteryHistory(_context);
                    if (_History == null)
                    {
                        throw new Exception("无法加载历史数据，请稍后重试");
                    }
                }

                // Step 6: initializing purchase manager. 
                // do this when the history is ready, because verifying require the data.
                if (OnDataInitalized != null)
                    OnDataInitalized("创建购彩篮...", 75);

                if (_purchaseMgr == null)
                {
                    PurchaseManager purchasMgr = new PurchaseManager();
                    if (await purchasMgr.Initialize())
                    {
                        _purchaseMgr = purchasMgr;
                    }
                    else
                    {
                        throw new Exception("无法创建购彩篮");
                    }
                }

                if (_pendingPurchase == null)
                {
                    // init pending purchase.
                    if (await AddEmptyPurchase() == null)
                    {
                        throw new Exception("无法创建购彩篮");
                    }
                }

                // Step 7: initializing purchase manager. 
                // do this when the history is ready, because verifying require the data.
                if (OnDataInitalized != null)
                    OnDataInitalized("加载旋转矩阵模板...", 80);

                if (_matrixTable == null)
                {
                    if (await InitMatrixTable() == null)
                    {
                        throw new Exception("无法加载旋转矩阵模板");
                    }
                }

                // Step 8: initialize help center.
                if (OnDataInitalized != null)
                    OnDataInitalized("加载帮助文档...", 90);

                if (!HelpCenter.Instance().DataLoaded)
                {
                    if (!await HelpCenter.Instance().Load())
                    {
                        throw new Exception("无法加载帮助文档");
                    }
                }

                if (OnDataInitalized != null)
                    OnDataInitalized("数据加载完毕", 100);

                _initialized = true;
            }
            catch (Exception e)
            {
                if (OnDataInitalized != null)
                    OnDataInitalized(e.Message, -1);
            }
        }

        public async Task<MatrixTable> InitMatrixTable()
        {
            if (_matrixTable != null)
                return _matrixTable;

            Dictionary<string, MatrixCell> data = await DataUtil.ReadMatrixFilters(_context);
            if (data != null)
            {
                _matrixTable = new MatrixTable(); // Don't create it until the data is ready.
                _matrixTable.Init();

                foreach (KeyValuePair<string, MatrixCell> pair in data)
                {
                    string[] keys = pair.Key.Split('-');
                    int row = Convert.ToInt32(keys[0]);
                    int col = Convert.ToInt32(keys[1]);
                    _matrixTable.SetCell(row, col, pair.Value);
                }

                return _matrixTable;
            }
            else
                throw new Exception("Fail to get the matrix data.");
        }

        public async Task PostFeedback(string name, string email, string phone, string content)
        {
            Feedback fb = new Feedback();
            fb.DeviceID = DataUtil.GetDeviceID();
            fb.Name = name;
            fb.Email = email;
            fb.Phone = phone;
            fb.Content = content;
            fb.LocalVersion = DataUtil.CurrentSoftwareVersion();
            fb.Plateform = DataUtil.Platform();

            await DataUtil.PostFeedback(fb);
        }

        public async Task PostRecord(int issue, int cost, int bouns, int[] prizeList)
        {
            Record rd = new Record();
            rd.DeviceID = DataUtil.GetDeviceID();
            rd.Bonus = prizeList;
            rd.Cost = cost;
            rd.Prize = bouns;
            rd.Issue = issue;

            await DataUtil.PostRecord(rd);
        }

        public async Task SaveAttributeFilterOption()
        {
            if (_filterOption != null)
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Filters");
                _filterOption.Save(root);

                await DataUtil.SaveAttributeFilterSetting(xml.OuterXml());
            }
        }

        public async Task CleanLocalCache()
        {
            // delete the folder 
            await DataUtil.ClearLocalData();
        }

        #region Properties
        public PurchaseManager PurchaseManager
        {
            get
            {
                return _purchaseMgr;
            }
        }

        public ReleaseInfo ReleaseInfo
        {
            get
            {
                return _releaseInfo;
            }
        }

        public SchemeAttributes LastAttributes
        {
            get
            {
                if (_latestAttributes == null)
                {
                    return null;
                }

                return _latestAttributes;
            }
        }

        public async Task<List<LotteryWebSite>> PurchaseWebSites()
        {
            if (_webSites != null)
                return _webSites;
               
            _webSites = new List<LotteryWebSite>();

            string webSiteList = await DataUtil.GetPurchaseWebSites();
            if (webSiteList.Length > 0)
            {
                // Parse the json string.
                JsonValue json = JsonValue.Parse(webSiteList);
                int arraySize = json.GetArray().Count;
                for (int i = 0; i < arraySize; i++)
                {
                    IJsonValue element = json.GetArray()[i];
                    if (element != null)
                    {
                        _webSites.Add(LotteryWebSite.Parse(element.GetObject()));
                    }
                }
            }

            return _webSites;
        }

        public PurchaseInfo PendingPurchase
        {
            get
            {
                return _pendingPurchase;
            }
        }

        public async Task<PurchaseInfo> ReuseExistPurchase(Purchase exist)
        {
            _pendingPurchase = await _purchaseMgr.AddPurchase(_releaseInfo.NextIssue, exist.Clone());

            return _pendingPurchase;
        }

        public PurchaseInfo EditExistPurchase(PurchaseInfo exist)
        {
            return _pendingPurchase = exist;
        }

        public async Task<PurchaseInfo> AddEmptyPurchase()
        {
            _pendingPurchase = await _purchaseMgr.AddPurchase(_releaseInfo.NextIssue, new Purchase()
            {
                Filters = new List<Constraint>(),
                Selection = new List<Scheme>(),
                Selectors = new List<SchemeSelector>()
            });

            return _pendingPurchase;
        }

        public LotteryInfo GetLatestLotteryInfo()
        {
            lock (thisLockObject)
            {
                if (_latestLotteryInfo == null)
                {
                    if (_latestLottery == null)
                        return null; // data is not available.

                    _latestLotteryInfo = LotteryInfo.Create(_latestLottery);
                }
                
                return _latestLotteryInfo;
            }
        }

        public LotteryStateInfo GetLatestLotteryStateInfo()
        {
            lock (thisLockObject)
            {
                if (_latestLotteryStateInfo == null)
                {
                    if (_latestLottery == null || _releaseInfo == null)
                        return null; // data is not available.

                    _latestLotteryStateInfo = LotteryStateInfo.Create(_latestLottery, _releaseInfo);
                }

                return _latestLotteryStateInfo;
            }
        }

        public AttributeFilterOption AttributeFilterOption
        {
            get
            {
                return _filterOption;
            }
        }

        public List<SchemeAttributeValueStatus> GetRecommendedConditions()
        {
            List<SchemeAttributeValueStatus> result = new List<SchemeAttributeValueStatus>();

            SchemeAttributes _attributes = LastAttributes;

            foreach (KeyValuePair<string, SchemeAttributeCategory> pair in _attributes.Categories)
            {
                foreach (KeyValuePair<string, SchemeAttribute> attriPair in pair.Value.Attributes)
                {
                    foreach (SchemeAttributeValueStatus valueState in attriPair.Value.ValueStates)
                    {
                        if (_filterOption.Passed(valueState))
                        {
                            result.Add(valueState);
                        }
                    }
                }
            }

            result.Sort();
            result.Reverse();

            return result;
        }

        #endregion        
    }    
}
