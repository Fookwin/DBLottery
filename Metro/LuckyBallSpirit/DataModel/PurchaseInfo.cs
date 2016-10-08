using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Selection;
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;
using Windows.Storage;
using Windows.UI.Xaml.Data;
using Windows.UI;

namespace LuckyBallSpirit.DataModel
{
    public class PurchaseBucketInfo
    {
        private bool _justVerified = false; // temp flag

        private int _issue = 0;
        private int _schemeCount = 0;
        private int _cost = 0;
        private int _earning = -1;
        private int _nextIndex = 1;

        private List<PurchaseInfo> _orders = new List<PurchaseInfo>();

        public PurchaseBucketInfo(int issue)
        {
            _issue = issue;
        }

        public bool JustVerified
        {
            get
            {
                return _justVerified;
            }
        }

        public bool Released
        {
            get;
            set;
        }

        public string ReleaseDate
        {
            get
            {
                if (Released)
                {
                    return Lottery.DateExp;
                }
                else
                {
                    return LBDataManager.GetInstance().ReleaseInfo.NextReleaseTime.ToString("yyyy-MM-dd");
                }
            }
        }

        public Lottery Lottery
        {
            get;
            set;
        }

        public List<PurchaseInfo> Orders
        {
            get
            {
                return _orders;
            }
        }

        public int Issue
        {
            get
            {
                return _issue;
            }
        }

        public int SchemeCount
        {
            get
            {
                return _schemeCount;
            }
        }

        public int Cost
        {
            get
            {
                return _cost;
            }
        }

        public int Earning
        {
            get
            {
                return _earning;
            }
        }

        public void Add(PurchaseInfo order)
        {
            _orders.Add(order);

            if (_nextIndex < order.Index)
            {
                _nextIndex = order.Index;
            }

            _nextIndex++;
        }

        public int NextIndex
        {
            get
            {
                return _nextIndex;
            }
        }

        public async Task Initialize()
        {
            if (_orders.Count == 0)
            {
                // Parse the summary from disk.
                List<int> order_indices = await DataUtil.PurchaseIndices(_issue);
                if (order_indices != null)
                {
                    foreach (int index in order_indices)
                    {
                        PurchaseInfo order = new PurchaseInfo(_issue, index);
                        Add(order);
                    }
                }
            }

            // Refresh summary.
            await RefreshSummary();
        }

        public async Task RefreshSummary()
        {
            _schemeCount = 0;
            _cost = 0;
            _earning = 0;
            _justVerified = false;

            foreach (PurchaseInfo order in _orders)
            {
                // Parse order.
                await order.Initialize();

                _schemeCount += order.SchemeCount;
                _cost += order.Cost;
                _earning += order.Earning;

                if (order.JustVerified)
                    _justVerified = true;
            }
        }
    }

    public class PurchaseInfo
    {
        // temp members
        private bool _dirty = true;
        private bool _justVerified = false;

        // persistent memebers.
        private int _issue = 0;
        private int _schemeCount = 0;
        private int _filterCount = 0;
        private int _cost = 0;
        private int _earning = 0;
        private int _index = 0;
        private bool _verified = false;

        private Purchase _source = null;

        public PurchaseInfo(int issue, int index)
        {
            _issue = issue;
            _index = index;
        }

        public PurchaseInfo(int issue, int index, Purchase source)
        {
            _issue = issue;
            _index = index;
            _source = source;

            // update the infomation from the source.
            _schemeCount = source.Selection.Count;
            _filterCount = source.Filters.Count;
            _cost = _schemeCount * 2;
        }

        public async Task<Purchase> GetSource()
        {
            if (_source == null)
            {
                _source = await DataUtil.ReadPurchase(_issue, _index);
            }

            return _source;
        }

        public bool Dirty
        {
            get
            {
                return _dirty;
            }
            set
            {
                _dirty = value;
            }
        }

        public bool JustVerified
        {
            get
            {
                return _justVerified;
            }
        }

        public async Task Initialize()
        {
            // Read from disk.
            await ReadInfo();
        }

        public bool Verified
        {
            get
            {
                return _verified;
            }
            set
            {
                _verified = value;
            }
        }

        public bool Editable
        {
            get
            {
                return _issue == LBDataManager.GetInstance().ReleaseInfo.NextIssue;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public int Issue
        {
            get
            {
                return _issue;
            }
        }

        public int SchemeCount
        {
            get
            {
                if (_source != null)
                {
                    _schemeCount = _source.Selection.Count;
                }
                return _schemeCount;
            }
        }

        public int FilterCount
        {
            get
            {
                if (_source != null)
                {
                    _filterCount = _source.Filters.Count;
                }
                return _filterCount;
            }
        }

        public int Cost
        {
            get
            {
                return _cost;
            }
        }

        public int Earning
        {
            get
            {
                return _earning;
            }
        }

        public async Task Save()
        {
            // Make sure the data of the information is up-to-date.
            Purchase order = await GetSource();
            _schemeCount = order.Selection.Count;
            _filterCount = order.Filters.Count;

            await SaveInfo();

            // save the purchase to disk asyncly
            DataUtil.SavePurchase(_issue, _index, _source);

            _dirty = false;
        }

        private async Task SaveInfo()
        {
            Task tk = new Task(async delegate
            {
                try
                {
                    // Get the file used to store the information.
                    string folderPath = "SchemeSelectionLibrary\\" + _issue.ToString() + "\\Selection_" + _index.ToString();
                    StorageFolder folder = await DataUtil.GetFolder(ApplicationData.Current.LocalFolder, folderPath, true);

                    // Saving...
                    DBXmlDocument xml = new DBXmlDocument();
                    DBXmlNode root = xml.AddRoot("Summary");

                    root.SetAttribute("SchemeCount", _schemeCount.ToString());
                    root.SetAttribute("FilterCount", _filterCount.ToString());
                    root.SetAttribute("Cost", _cost.ToString());
                    root.SetAttribute("Earning", _earning.ToString());
                    root.SetAttribute("Verified", _verified.ToString());

                    StorageFile file = await folder.CreateFileAsync("Summary.xml", CreationCollisionOption.ReplaceExisting);
                    await xml.Save(file);
                }
                catch
                {
                }
            });

            tk.Start();
        }

        private async Task ReadInfo()
        {
            bool bParsed = false;

            // Get the file used to store the information.
            string _location = "SchemeSelectionLibrary\\" + _issue.ToString() + "\\Selection_" + _index.ToString();
            StorageFolder folder = await DataUtil.GetFolder(ApplicationData.Current.LocalFolder, _location, false);
            if (folder != null)
            {
                // Get the file.
                StorageFile file = await DataUtil.GetFile(folder, "Summary.xml", false);
                if (file != null)
                {
                    // Parsing.
                    DBXmlDocument xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file));
                    DBXmlNode root = xml.Root();

                    _schemeCount = Convert.ToInt32(root.GetAttribute("SchemeCount"));
                    _filterCount = Convert.ToInt32(root.GetAttribute("FilterCount"));
                    _cost = Convert.ToInt32(root.GetAttribute("Cost"));
                    _earning = Convert.ToInt32(root.GetAttribute("Earning"));
                    _verified = Convert.ToBoolean(root.GetAttribute("Verified"));

                    bParsed = true;
                }
            }

            if (!bParsed)
            {
                // Check the scheme...
                Purchase target = await GetSource();

                _schemeCount = target.Selection.Count;
                _filterCount = target.Filters.Count;
                _cost = _schemeCount * 2;
            }

            if (!_verified)
            {
                if (await Verify())
                {
                    // Save to disk.
                    SaveInfo();

                    _justVerified = true;
                }
            }

            _dirty = false;
        }

        private async Task<bool> Verify()
        {
            if (_verified)
                return true;

            Purchase target = await GetSource();

            _schemeCount = target.Selection.Count;
            _filterCount = target.Filters.Count;
            _cost = _schemeCount * 2;

            int[] prizeList = null;
            _earning = CalculateEarning(target, out prizeList);

            if (_earning < 0)
            {
                _earning = -1;
                _verified = false;
            }
            else
            {
                _verified = true;

                // send the information to server.
                LBDataManager.GetInstance().PostRecord(Issue, Cost, _earning, prizeList);
            }

            return _verified;
        }

        private int CalculateEarning(Purchase target, out int[] prizeList)
        {
            prizeList = new int[6];

            int earning = 0;
            Lottery lot = LuckyBallsData.DataManageBase.Instance().History.LotteryInIssue(_issue);
            if (lot == null || lot.BonusAmount(1) <= 0) // calculate it only when the bonus information is ready.
            {
                return -1;
            }

            foreach (Scheme item in target.Selection)
            {
                int prize = lot.CheckPrize(item);
                if (prize > 0)
                {
                    prizeList[prize - 1]++;
                    earning += lot.BonusMoney(prize);
                }
            }

            return earning;
        }        
    }

    class PurchaseManager
    {        
        private Dictionary<int, PurchaseBucketInfo> _history = new Dictionary<int, PurchaseBucketInfo>();
        private bool _syncingRequired = true;

        public async Task<bool> Initialize()
        {
            if (!_syncingRequired)
                return true;

            int nextIssue = LBDataManager.GetInstance().ReleaseInfo.NextIssue;
            History history = LBDataManager.GetInstance().History;

            List<int> indices = await DataUtil.PurchaseBucketIndices();
            if (indices != null)
            {
                foreach (int issue in indices)
                {
                    PurchaseBucketInfo bucket = new PurchaseBucketInfo(issue) 
                    {
                        Released = (issue != nextIssue),
                        Lottery = history.LotteryInIssue(issue) 
                    };
                    //await bucket.Initialize(); // delay initalizing the data.

                    _history.Add(issue, bucket);
                }
            }

            _syncingRequired = false;

            return true;
        }

        public PurchaseBucketInfo PurchaseBucket(int issue)
        {
            if (_history.ContainsKey(issue))
                return _history[issue];
            else 
                return null;            
        }

        public Dictionary<int, PurchaseBucketInfo> AllPurchases
        {
            get
            {
                return _history;
            }
        }

        public async Task<PurchaseInfo> AddPurchase(int issue, Purchase order)
        {
            PurchaseBucketInfo bucket = PurchaseBucket(issue);
            if (bucket == null)
            {
                bucket = new PurchaseBucketInfo(issue);
                _history.Add(issue, bucket);
            }

            await bucket.Initialize(); // make sure the next index is correct.

            PurchaseInfo purchase = new PurchaseInfo(issue, bucket.NextIndex, order);
            bucket.Add(purchase);

            return purchase;
        }

        public async Task DeletePurchase(PurchaseInfo pPurcase)
        {
            PurchaseBucketInfo bucket = _history[pPurcase.Issue];
            if (bucket != null)
            {
                bucket.Orders.Remove(pPurcase);

                DataUtil.DeletePurchase(pPurcase.Issue, pPurcase.Index);
                
                await bucket.RefreshSummary();
            }
        }

        public void DeleteBucket(PurchaseBucketInfo pBucket)
        {
            _history.Remove(pBucket.Issue);

            DataUtil.DeletePurchaseBucket(pBucket.Issue);
        }

        public void DeleteAll()
        {
            _history.Clear();

            DataUtil.CleanAllPurchase();
        }
    }

    public class NumHitToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool temp = (bool)value;

            return temp ? 1 : 0.3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class FilterWinToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? temp = (bool?)value;

            return (temp != null && temp.Value) ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class FilterLossToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? temp = (bool?)value;

            return (temp != null && !temp.Value) ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class ReleaseStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? temp = (bool?)value;

            return (temp != null && temp.Value) ? Colors.DarkOrange : Colors.DarkGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    class VerifiedFilter
    {
        private Constraint _target = null;
        private Scheme _compare = null;
        private bool? _win = null;

        public VerifiedFilter(Constraint target, Scheme compare, int issueIndex)
        {
            _target = target;
            _compare = compare;

            if (compare != null)
            {
                _win = _target.Meet(_compare, issueIndex);
            }
        }

        public Constraint Target
        {
            get
            {
                return _target;
            }
        }

        public bool? Win
        {
            get
            {
                return _win;
            }
        }
    }

    class VerifiedScheme
    {
        private Scheme _compare = null;
        private Scheme _target = null;
        private Byte _bits = 0;

        public VerifiedScheme(Scheme target, Scheme compare)
        {
            _target = target;
            _compare = compare;

            if (_compare != null)
            {
                int[] reds = _target.GetRedNums();
                for (int i = 0; i < 6; ++i)
                {
                    if (compare.Contains(reds[i]))
                        _bits |= (Byte)(1 << i);
                }

                if (target.Blue == compare.Blue)
                    _bits |= (Byte)64;
            }
        }

        public Scheme Target
        {
            get 
            {
                return _target;
            }
        }

        public int Red1
        {
            get
            {
                return _target.Red(0);
            }
        }

        public bool IsHitRed1
        {
            get
            {
                return (_bits & (Byte)1) != 0;
            }
        }

        public int Red2
        {
            get
            {
                return _target.Red(1);
            }
        }

        public bool IsHitRed2
        {
            get
            {
                return (_bits & (Byte)2) != 0;
            }
        }

        public int Red3
        {
            get
            {
                return _target.Red(2);
            }
        }

        public bool IsHitRed3
        {
            get
            {
                return (_bits & (Byte)4) != 0;
            }
        }

        public int Red4
        {
            get
            {
                return _target.Red(3);
            }
        }

        public bool IsHitRed4
        {
            get
            {
                return (_bits & (Byte)8) != 0;
            }
        }

        public int Red5
        {
            get
            {
                return _target.Red(4);
            }
        }

        public bool IsHitRed5
        {
            get
            {
                return (_bits & (Byte)16) != 0;
            }
        }

        public int Red6
        {
            get
            {
                return _target.Red(5);
            }
        }

        public bool IsHitRed6
        {
            get
            {
                return (_bits & (Byte)32) != 0;
            }
        }

        public int Blue
        {
            get
            {
                return _target.Blue;
            }
        }

        public bool IsHitBlue
        {
            get
            {
                return (_bits & (Byte)64) != 0;
            }
        }
    }
}

