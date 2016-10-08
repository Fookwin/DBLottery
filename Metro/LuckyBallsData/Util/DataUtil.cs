using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Statistics;
using LuckyBallsData.DBSQLService;
using LuckyBallsData.DBUserControlService;
using System.ServiceModel;
using LuckyBallsData.Selection;
using LuckyBallsData.UserData;
using Windows.Storage;
using LuckyBallsData.Util;
using LuckyBallsData.Filters;
using Windows.Data.Json;

namespace LuckyBallsData.Util
{
    public class DataUtil
    {
        #region Pre-declaration        

        public class SoftwareVersion
        {
            public int Version = -1;
            public bool SchemeChanged = false;
        }

        public class SyncContext
        {
            public DataVersion LocalVersion = null;
            public DataVersion CloudVersion = null;

            public bool NeedToResync(UpdateOptionEnum option)
            {
                if (LocalVersion == null || CloudVersion == null)
                    return false;

                switch (option)
                {
                    case UpdateOptionEnum.NeedUpdateHistory:
                        {
                            return LocalVersion.HistoryDataVersion != CloudVersion.HistoryDataVersion;
                        }
                    case UpdateOptionEnum.NeedUpdateReleaseInfo:
                        {
                            return LocalVersion.ReleaseDataVersion != CloudVersion.ReleaseDataVersion; 
                        }
                    case UpdateOptionEnum.NeedUpdateAttributes:
                        {
                            return LocalVersion.AttributeDataVersion != CloudVersion.AttributeDataVersion;
                        }
                    case UpdateOptionEnum.NeedUpdateAttributesTemplate:
                        {
                            return LocalVersion.AttributeTemplateVersion != CloudVersion.AttributeTemplateVersion;
                        }
                    case UpdateOptionEnum.NeedUpdateMatrixTable:
                        {
                            return LocalVersion.MatrixDataVersion != CloudVersion.MatrixDataVersion;
                        }
                    case UpdateOptionEnum.NeedUpdateLatestLottery:
                        {
                            return LocalVersion.LatestIssue < CloudVersion.LatestIssue || 
                                LocalVersion.LatestLotteryVersion != CloudVersion.LatestLotteryVersion;
                        }
                    case UpdateOptionEnum.NeedUpdateHelpContent:
                        {
                            return LocalVersion.HelpContentVersion != CloudVersion.HelpContentVersion;
                        }
                }

                return false;
            }
        }

        public enum UpdateOptionEnum
        {
            NeedUpdateHistory = 0, // update the history.xml
            NeedUpdateReleaseInfo = 1, // update release infomration.
            NeedUpdateAttributes = 2, // update attributes.
            NeedUpdateAttributesTemplate = 3, // update attribute template
            NeedUpdateMatrixTable = 4, // update matrix table.
            NeedUpdateLatestLottery = 5, // update the details of the latest lottery.
            NeedUpdateHelpContent = 6 // update the help xml.
        }

        private static int s_currentSoftwareVersion = 20141218;
        private const string s_historyFileName = "History.xml";
        private const string s_releaseInfoFileName = "ReleaseInformation.xml";
        private const string s_attributeFilterSettingFileName = "AttributeFilterSetting.xml";
        private const string s_attributesFileName = "LatestAttributes.xml";
        private const string s_versionFileName = "Version.xml";
        private const string s_attributesTemplateFileName = "AttributeTemplate.xml";
        private const string s_purchaseWebSites = "WebSites.txt";
        private const string s_purchaseFolderName = "SchemeSelectionLibrary";
        private const string s_helpFileName = "Help.xml";
        private const int s_platform = 1; // win store.

        private static SqlServiceClient _dataClient = null;
        private static UseControlServiceClient _useCtrlClient = null;

        public static int CurrentSoftwareVersion()
        {
            return s_currentSoftwareVersion;
        }

        public static int Platform()
        {
            return s_platform;
        }

        #endregion

        #region Context

        public static async Task<SyncContext> ConstructContext()
        {
            DataVersion versionOnCloud = null;
            try
            {
                // Read the cloud context.
                SqlServiceClient client = OpenDataService();

                // Check the version in cloud to determine if an update need to be performed.
                string dataVersion = await client.GetDataVersionAsync();                

                versionOnCloud = new DataVersion();

                DBXmlDocument xml = new DBXmlDocument();
                xml.Load(dataVersion);

                versionOnCloud.ReadFromXml(xml.Root());
            }
            catch (Exception e)
            {
                return null; // the service is not available.
            }

            SyncContext context = new SyncContext();
 
            // Read the local context.
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(s_versionFileName);
            }
            catch
            {
                file = null;
            }

            context.LocalVersion = new DataVersion();

            if (file != null)
            {
                DBXmlDocument xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file));
                DBXmlNode root = xml.Root();

                context.LocalVersion.ReadFromXml(root);
            }

            context.CloudVersion = versionOnCloud;

            return context;
        }      

        public static void SaveVersion(DataVersion ver)
        {
            Task tk = new Task(async delegate
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Version");

                ver.SaveToXml(ref root);

                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(s_versionFileName, CreationCollisionOption.ReplaceExisting);

                await xml.Save(file);           
            });

            tk.Start();
        }

        public static SqlServiceClient OpenDataService()
        {
            if (_dataClient == null)
            {
                _dataClient = new SqlServiceClient();
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

        #endregion

        #region Read/Write Lottery History
        public static async Task<Lottery> ReadLatestLottery(SyncContext context)
        {
            if (!context.NeedToResync(UpdateOptionEnum.NeedUpdateLatestLottery))
            {
                // read from local.
                StorageFolder folder = ApplicationData.Current.LocalFolder;

                StorageFile file = null;
                try
                {
                    file = await folder.GetFileAsync(s_historyFileName);
                }
                catch (System.IO.FileNotFoundException)
                {
                    file = null;
                }

                if (file != null)
                {
                    using (Stream inStream = await file.OpenStreamForReadAsync())
                    {
                        try
                        {
                            DBXmlDocument xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file));
                            DBXmlNode root = xml.Root();

                            return History.ReadSingleLottery(context.CloudVersion.LatestIssue, root);
                        }
                        catch
                        {
                            // still need read from server...
                        }
                    }
                }
            }

            // read the latest lottery from server.
            try
            {
                SqlServiceClient client = OpenDataService();
                string data = await client.GetLotteryDataAsync(context.CloudVersion.LatestIssue);

                if (data != null)
                {
                    DBXmlDocument xml = new DBXmlDocument();
                    xml.Load(data);
                    DBXmlNode root = xml.Root();

                    Lottery lot = new Lottery(null);
                    lot.ReadFromXml(root);

                    return lot;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }

            return null;
        }

        public static async Task<History> ReadLotteryHistory(SyncContext context)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(s_historyFileName);
            }
            catch (System.IO.FileNotFoundException)
            {
                file = null;
            }

            History history = null;

            if (file != null)
            {
                try
                {
                    DBXmlDocument xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file));
                    DBXmlNode root = xml.Root();

                    history = new History();
                    if (!history.ReadFromXml(root, History.InsertOption.eReplaceExists))
                        history = null; // clean the data.
                }
                catch (System.IO.FileNotFoundException)
                {
                    history = null; // clean the data.
                }
            }

            // sync to cloud. 
            return await SyncLotteryHistoryToCloud(history, context);
        }

        private static void SaveLotteryHistory(History history)
        {
            Task tk = new Task(async delegate
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("History");

                if (history.SaveToXml(ref root))
                {

                    // save the version information.
                    root.SetAttribute("Version", history.Version.ToString());
                    root.SetAttribute("LastIssue", history.LastIssue.ToString());

                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await folder.CreateFileAsync(s_historyFileName, CreationCollisionOption.ReplaceExisting);

                    await xml.Save(file);
                }
            });

            tk.Start();
        }

        private static async Task<History> SyncLotteryHistoryToCloud(History history, SyncContext context)
        {
            try
            {
                // need udpate?
                bool bNeedToResync = history == null || context.NeedToResync(UpdateOptionEnum.NeedUpdateHistory);
                bool bNewRowdded = context.LocalVersion.LatestIssue != context.CloudVersion.LatestIssue;
                bool bNeedResyncLatest = context.NeedToResync(UpdateOptionEnum.NeedUpdateLatestLottery);
                if (!bNeedToResync && !bNewRowdded && !bNeedResyncLatest)
                    return history;

                if (history == null)
                    history = new History();

                // Updating...
                SqlServiceClient client = OpenDataService();
                History.InsertOption option = History.InsertOption.eAppendToTail;
                string data = "";
                if (bNeedToResync)
                {
                    // need rebuild the cache data. 
                    data = await client.GetAllLotteriesAsync();
                    option = History.InsertOption.eReplaceExists;
                }
                else if (bNewRowdded)
                {
                    // get the data since last sync.
                    data = await client.GetLotteriesByIssueAsync(history.LastIssue, context.CloudVersion.LatestIssue);
                }
                else if (bNeedResyncLatest)
                {
                    data = await client.GetLotteryDataAsync(context.CloudVersion.LatestIssue);

                    DBXmlDocument xml = new DBXmlDocument();
                    xml.Load(data);

                    // Delete the current latest lottery.
                    if (history.LastLottery.ReadFromXml(xml.Root()))
                    {
                        SaveLotteryHistory(history);

                        return history;
                    }

                    return null;
                }

                if (data != null)
                {
                    DBXmlDocument xml = new DBXmlDocument();
                    xml.Load(data);

                    if (history.ReadFromXml(xml.Root(), option))
                    {
                        SaveLotteryHistory(history);

                        return history;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                string error = e.Message;
                return null;
            }            
        }
        #endregion

        #region Read/Write Scheme Atrribtes
        public static async Task<SchemeAttributes> ReadSchemeAttribute(SyncContext context)
        {
            SchemeAttributes template = AttributeUtil.GetAttributesTemplate();
            if (template == null)
                return null;

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile latestAttributesfile = null;

            DBXmlDocument xml = null;
            if (context.LocalVersion.LatestIssue == context.CloudVersion.LatestIssue &&
                !context.NeedToResync(UpdateOptionEnum.NeedUpdateAttributes))
            {
                try
                {
                    latestAttributesfile = await folder.GetFileAsync(s_attributesFileName);

                    if (latestAttributesfile != null)
                        xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(latestAttributesfile));
                }
                catch
                {
                    xml = null;
                }
            }

            if (xml == null)
            {
                // Get it from cloud.
                SqlServiceClient client = OpenDataService();
                string templateString = await client.GetLatestAttributesAsync();
                if (templateString.Length > 0)
                {
                    xml = new DBXmlDocument();
                    xml.Load(templateString);

                    Task tk = new Task(async delegate
                    {
                        latestAttributesfile = await folder.CreateFileAsync(s_attributesFileName, CreationCollisionOption.ReplaceExisting);
                        await xml.Save(latestAttributesfile);
                    });

                    tk.Start();
                }
            }

            if (xml == null)
                return null;

            // Create the attributes from template.
            SchemeAttributes attributes = template.Clone();

            try
            {
                // Parsing the latest data.
                DBXmlNode root = xml.Root();
                attributes.ReadValueFromXml(root);
            }
            catch (Exception e)
            {
                string msg = e.Message;
                attributes = null;
            }

            return attributes;
        }

        public static async Task<SchemeAttributes> GetAttributesTemplate(SyncContext context)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            // Step 1: need resync?
            StorageFile file = null;
            if (!context.NeedToResync(UpdateOptionEnum.NeedUpdateAttributesTemplate))
            {
                try
                {
                    file = await folder.GetFileAsync(s_attributesTemplateFileName);
                }
                catch
                {
                    file = null;
                }
            }

            // Step 2: if no resync needed, parse data from local file.
            DBXmlDocument xml = null;
            if (file != null)
            {
                try
                {
                    xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file));
                }
                catch
                {
                    xml = null;
                }
            }

            // Step 3: no data parsed correctly, re-sync from server.
            if (xml == null)
            {
                // Get it from cloud.
                SqlServiceClient client = OpenDataService();
                string templateString = await client.GetAttributesTemplateAsync();
                if (templateString.Length > 0)
                {
                    xml = new DBXmlDocument();
                    xml.Load(templateString);

                    Task tk = new Task(async delegate
                    {
                        file = await folder.CreateFileAsync(s_attributesTemplateFileName, CreationCollisionOption.ReplaceExisting);

                        await xml.Save(file);
                    });

                    tk.Start();
                }
            }

            // Create the attributes from template.
            if (xml != null)
            {
                SchemeAttributes attributes = new SchemeAttributes();
                DBXmlNode root = xml.Root();
                attributes.ReadFromTemplate(root);

                return attributes;
            }

            return null;
        }

        private static async Task<StorageFile> GetLatestAttributesFile(SyncContext context)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(s_attributesFileName);
            }
            catch
            {
                file = null;
            }

            if (file == null || 
                context.LocalVersion.LatestIssue != context.CloudVersion.LatestIssue ||
                context.NeedToResync(UpdateOptionEnum.NeedUpdateAttributes))
            {
                // Get it from cloud.
                SqlServiceClient client = OpenDataService();

                string templateString = await client.GetLatestAttributesAsync();
                if (templateString.Length > 0)
                {
                    DBXmlDocument xml = new DBXmlDocument();
                    xml.Load(templateString);

                    file = await folder.CreateFileAsync(s_attributesFileName, CreationCollisionOption.ReplaceExisting);

                    await xml.Save(file);
                }                
            }

            return file;
        }

        #endregion

        #region Read/Write Scheme Purchases

        // Return the location.
        public static async Task<string> SavePurchase(int issue, int index, Purchase order)
        {
            // Prepare for the storage location.
            string folderPath = s_purchaseFolderName + "\\" + issue.ToString() + "\\Selection_" + index.ToString();
            StorageFolder schemeFolder = await GetFolder(ApplicationData.Current.LocalFolder, folderPath, true);

            // Save selectors.
            if (order.Selectors.Count > 0)
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Selectors");

                foreach (SchemeSelector sel in order.Selectors)
                {
                    DBXmlNode node = root.AddChild("Item");
                    node.SetAttribute("Type", ((int)sel.GetSelectorType()).ToString());
                    node.SetAttribute("Expression", sel.Expression);
                }

                StorageFile file = await schemeFolder.CreateFileAsync("Selectors.xml", CreationCollisionOption.ReplaceExisting);

                await xml.Save(file);
            }

            // Save filters.
            if (order.Filters.Count > 0)
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Filters");

                foreach (Constraint con in order.Filters)
                {
                    DBXmlNode node = root.AddChild("Item");
                    node.SetAttribute("Type", ((int)con.GetConstraintType()).ToString());
                    con.WriteToXml(ref node);
                }

                StorageFile file = await schemeFolder.CreateFileAsync("Filters.xml", CreationCollisionOption.ReplaceExisting);

                await xml.Save(file);
            }

            // Save result.
            if (order.Selection.Count > 0)
            {                
                int count = order.Selection.Count;
                string[] lines = new string[count];

                for (int i = 0; i < count; ++ i)
                {
                    Scheme sm = order.Selection[i];
                    lines[i] = sm.RedBits.ToString() + " " + sm.Blue.ToString();
                }

                StorageFile file = await schemeFolder.CreateFileAsync("Selection.txt", CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteLinesAsync(file, lines);
            }

            return schemeFolder.Path;
        }

        public static async Task<List<int>> PurchaseBucketIndices()
        {
            // get the selection scheme folder.
            StorageFolder schemeLibFolder = await GetFolder(ApplicationData.Current.LocalFolder, s_purchaseFolderName, false);
            if (schemeLibFolder == null)
            {
                return null;
            }

            IReadOnlyList<StorageFolder> exists = await schemeLibFolder.GetFoldersAsync();

            List<int> indices = new List<int>();
            foreach (StorageFolder issueFolder in exists)
            {
                int issue = Convert.ToInt32(issueFolder.DisplayName);
                indices.Add(issue);
            }

            return indices;
        }

        public static async Task<List<int>> PurchaseIndices(int issue)
        {
            // get the selection scheme folder.
            StorageFolder issueFolder = await GetFolder(ApplicationData.Current.LocalFolder, s_purchaseFolderName + "\\" + issue.ToString(), false);
            if (issueFolder == null)
            {
                return null;
            }

            List<int> indices = new List<int>();

            IReadOnlyList<StorageFolder> exists = await issueFolder.GetFoldersAsync();
            foreach (StorageFolder orderFolder in exists)
            {
                int index = Convert.ToInt32(orderFolder.DisplayName.Substring(10));
                indices.Add(index);
            }

            return indices;
        }

        public static async Task<PurchaseBucket> ReadPurchaseBucket(int issue)
        {
            StorageFolder issueFolder = await GetFolder(ApplicationData.Current.LocalFolder, s_purchaseFolderName + "\\" + issue.ToString(), false);
            if (issueFolder == null)
            {
                return null;
            }

            return await ReadPurchaseBucket(issueFolder);
        }

        private static async Task<PurchaseBucket> ReadPurchaseBucket(StorageFolder purchasFolder)
        {
            int issue = Convert.ToInt32(purchasFolder.DisplayName);
            PurchaseBucket bucket = new PurchaseBucket() { Issue = issue };

            IReadOnlyList<StorageFolder> folders = await purchasFolder.GetFoldersAsync();
            foreach (StorageFolder folder in folders)
            {
                Purchase order = await ReadPurchase(folder);
                bucket.Orders.Add(order);
            }

            return bucket;
        }

        public static async Task<Purchase> ReadPurchase(int issue, int index)
        {
            string folderPath = s_purchaseFolderName + "\\" + issue.ToString() + "\\Selection_" + index.ToString();
            StorageFolder selectionFolder = await GetFolder(ApplicationData.Current.LocalFolder, folderPath, true);           
            if (selectionFolder == null)
            {
                return null;
            }

            Purchase order = await ReadPurchase(selectionFolder);

            return order;
        }

        private static async Task<Purchase> ReadPurchase(StorageFolder purchasFolder)
        {
            Purchase data = new Purchase();

            // Read selectors.
            StorageFile selectorsfile = await GetFile(purchasFolder, "Selectors.xml", false); 
            if (selectorsfile != null)
            {
                data.Selectors = new List<SchemeSelector>();

                DBXmlDocument xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(selectorsfile));
                DBXmlNode root = xml.Root();

                List<DBXmlNode> subNodes = root.ChildNodes();
                foreach (DBXmlNode node in subNodes)
                {
                    SchemeSelectorTypeEnum type = (SchemeSelectorTypeEnum)Convert.ToInt32(node.GetAttribute("Type"));
                    SchemeSelector selector = SchemeSelector.CreateSelector(type);
                    selector.ParseExpression(node.GetAttribute("Expression"));
                    data.Selectors.Add(selector);
                } 
            }

            // Read filters.
            StorageFile filtersfile = await GetFile(purchasFolder, "Filters.xml", false);
            if (filtersfile != null)
            {
                data.Filters = new List<Constraint>();

                DBXmlDocument xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(filtersfile));
                DBXmlNode root = xml.Root();

                List<DBXmlNode> subNodes = root.ChildNodes();
                foreach (DBXmlNode node in subNodes)
                {
                    ConstraintTypeEnum type = (ConstraintTypeEnum)Convert.ToInt32(node.GetAttribute("Type"));
                    Constraint con = Constraint.CreateConstraint(type);
                    con.ReadFromXml(node);

                    data.Filters.Add(con);
                }
            }

            // Read schemes.
            StorageFile schemesfile = await GetFile(purchasFolder, "Selection.txt", false);
            if (schemesfile != null)
            {
                data.Selection = new List<Scheme>();

                IList<string> lines = await FileIO.ReadLinesAsync(schemesfile);

                foreach (string line in lines)
                {
                    string[] subs = line.Split(' ');
                    if (subs.Count() == 2)
                    {
                        data.Selection.Add(new Scheme(Convert.ToUInt32(subs[0]), Convert.ToByte(subs[1])));
                    }
                    else
                    {
                        //backforward support.
                        data.Selection.Add(new Scheme(line));
                    }
                }
            }

            return data;
        }

        public static async Task CleanAllPurchase()
        {
            // Delete the top level folder directly.
            string rootFolderPath = s_purchaseFolderName;
            StorageFolder schemeFolder = await GetFolder(ApplicationData.Current.LocalFolder, rootFolderPath, false);
            if (schemeFolder != null)
            {
                await schemeFolder.DeleteAsync();
            }
        }

        public static async Task DeletePurchaseBucket(int issue)
        {
            StorageFolder issueFolder = await GetFolder(ApplicationData.Current.LocalFolder, s_purchaseFolderName + "\\" + issue.ToString(), false);
            if (issueFolder != null)
            {
                await issueFolder.DeleteAsync();
            }
        }

        public static async Task DeletePurchase(int issue, int index)
        {
            string folderPath = s_purchaseFolderName + "\\" + issue.ToString() + "\\Selection_" + index.ToString();
            StorageFolder selectionFolder = await GetFolder(ApplicationData.Current.LocalFolder, folderPath, false);
            if (selectionFolder != null)
            {
                await selectionFolder.DeleteAsync();
            }
        }

        #endregion

        #region Release Info

        public static async Task<DBXmlDocument> GetLatestReleaseInfo(SyncContext context)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;

            // Step 1: load local data if no resync needed.
            if (context.LocalVersion.LatestIssue == context.CloudVersion.LatestIssue &&
                !context.NeedToResync(UpdateOptionEnum.NeedUpdateReleaseInfo))
            {
                try
                {
                    file = await folder.GetFileAsync(s_releaseInfoFileName);
                }
                catch
                {
                    file = null;
                }
            }

            // Step 2: try to parse data from file.
            DBXmlDocument xml = null;
            if (file != null)
            {
                try
                {
                    xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file));
                }
                catch
                {
                    xml = null;
                }
            }

            // Step 3: re-sync from server if no luck to get data from local.
            if (xml == null)
            {
                SqlServiceClient client = OpenDataService();
                string releaseString = await client.GetLatestReleaseInfoAsync();
                if (releaseString.Length > 0)
                {
                    xml = new DBXmlDocument();
                    xml.Load(releaseString);

                    Task tk = new Task(async delegate
                    {
                        file = await folder.CreateFileAsync(s_releaseInfoFileName, CreationCollisionOption.OpenIfExists);

                        await xml.Save(file);
                    });

                    tk.Start();
                }
            }

            return xml;
        }

        #endregion

        #region Purchase Web Sites

        public static async Task<string> GetPurchaseWebSites()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(s_purchaseWebSites);
            }
            catch
            {
                file = null;
            }

            string output = "";
            if (file == null)
            {
                // Set as default.
                JsonArray jsonArray = new JsonArray();

                {
                    JsonObject jsonObject = new JsonObject();
                    jsonObject["Name"] = JsonValue.CreateStringValue("淘宝彩票");
                    jsonObject["Uri"] = JsonValue.CreateStringValue("http://caipiao.taobao.com/lottery/order/lottery_ssq.htm?spm=0.0.0.0.FKFyYe");
                    jsonObject["Format"] = JsonValue.CreateStringValue(" +");
                    jsonArray.Add(jsonObject);
                }

                {
                    JsonObject jsonObject = new JsonObject();
                    jsonObject["Name"] = JsonValue.CreateStringValue("新浪彩票");
                    jsonObject["Uri"] = JsonValue.CreateStringValue("http://sports.sina.com.cn/l/2caipiao/ssq");
                    jsonObject["Format"] = JsonValue.CreateStringValue("  ");
                    jsonArray.Add(jsonObject);
                }

                {
                    JsonObject jsonObject = new JsonObject();
                    jsonObject["Name"] = JsonValue.CreateStringValue("网易彩票");
                    jsonObject["Uri"] = JsonValue.CreateStringValue("http://caipiao.163.com/order/preBet_ssq.html");
                    jsonObject["Format"] = JsonValue.CreateStringValue(" :");
                    jsonArray.Add(jsonObject);
                }

                {
                    JsonObject jsonObject = new JsonObject();
                    jsonObject["Name"] = JsonValue.CreateStringValue("自定义格式");
                    jsonObject["Uri"] = JsonValue.CreateStringValue("");
                    jsonObject["Format"] = JsonValue.CreateStringValue(" :");
                    jsonArray.Add(jsonObject);
                }

                output = jsonArray.Stringify();

                file = await folder.CreateFileAsync(s_purchaseWebSites, CreationCollisionOption.OpenIfExists);

                await FileIO.WriteTextAsync(file, output);
            }
            else
            {
                output = await FileIO.ReadTextAsync(file);
            }

            return output;
        }

        public static async Task SavePurchaseWebSites(string jsonString)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(s_purchaseWebSites);
            }
            catch
            {
                file = null;
            }

            if (file == null)
            {
                file = await folder.CreateFileAsync(s_purchaseWebSites, CreationCollisionOption.OpenIfExists);
            }

            await FileIO.WriteTextAsync(file, jsonString);
        }

        #endregion

        #region Matrix Filters
        public static async Task<Dictionary<string, MatrixCell>> ReadMatrixFilters(SyncContext context)
        {
            // Prepare for the storage location.
            string folderPath = "Matrix";
            StorageFolder matrixFolder = await GetFolder(ApplicationData.Current.LocalFolder, folderPath, true);
            if (matrixFolder != null)
            {
                Dictionary<string, MatrixCell> result = new Dictionary<string, MatrixCell>();

                for (int i = 7; i <= 33; ++i)
                {
                    for (int j = 3; j <= 6; ++j)
                    {
                        // Read from the file.
                        string fileName = i.ToString() + "-" + j.ToString() + ".txt";
                        StorageFile file = await GetFile(matrixFolder, fileName, false);
                        if (file == null || context.NeedToResync(UpdateOptionEnum.NeedUpdateMatrixTable))
                        {
                            // currently we just need support 6-5.
                            if (j != 6 || i > 20)
                                continue;

                            // Get the file from cloud.
                            // Get it from cloud.
                            SqlServiceClient client = OpenDataService();

                            string templateString = await client.GetMatrixTableItemAsync(i, j);
                            if (templateString.Length > 0)
                            {
                                file = await matrixFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                                await FileIO.WriteTextAsync(file, templateString);
                            }
                            else
                                throw new Exception("Failed to get matrix data from server.");
                        }

                        if (file != null)
                        {
                            MatrixCell cell = new MatrixCell();

                            IList<string> lines = await FileIO.ReadLinesAsync(file);

                            foreach (string line in lines)
                            {
                                cell.Template.Add(new MatrixItemByte(line));
                            }

                            result.Add(i.ToString() + "-" + j.ToString(), cell);
                        }
                    }
                }

                return result;
            }
            else
                throw new Exception("Could not find matrix folder!");
        }

        
        #endregion

        #region PushNotification

        private static string s_deviceID = "";

        public static string GetDeviceID()
        {
            if (s_deviceID == "")
            {
                var packageSpecificToken = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
                Windows.Storage.Streams.IBuffer hardwareId = packageSpecificToken.Id;

                using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId))
                {
                    byte[] bytes = new byte[hardwareId.Length];
                    dataReader.ReadBytes(bytes);

                    // read the string
                    s_deviceID = BitConverter.ToString(bytes);
                }
            }

            return s_deviceID;
        }

        public static async Task Login(string channelUri)
        {
            try
            {
                string deviceID = GetDeviceID();
                if (deviceID == "")
                    return;

                UseControlServiceClient client = OpenUserCtrlService();
                await client.UserLoginAsync(deviceID, s_platform, s_currentSoftwareVersion, channelUri);
            }
            catch(Exception e)
            {
                return;
            }
        }

        #endregion

        #region Help

        public static async Task<DBXmlDocument> GetHelpXml(SyncContext context)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;

            // Step 1: load local data if no resync needed.
            if (!context.NeedToResync(UpdateOptionEnum.NeedUpdateHelpContent))
            {
                try
                {
                    file = await folder.GetFileAsync(s_helpFileName);
                }
                catch
                {
                    file = null;
                }
            }

            // Step 2: try to parse data from file.
            DBXmlDocument xml = null;
            if (file != null)
            {
                try
                {
                    xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file));
                }
                catch
                {
                    xml = null;
                }
            }

            // Step 3: re-sync from server if no luck to get data from local.
            if (xml == null)
            {
                SqlServiceClient client = OpenDataService();
                string helpString = await client.GetHelpAsync();
                if (helpString.Length > 0)
                {
                    xml = new DBXmlDocument();
                    xml.Load(helpString);

                    Task tk = new Task(async delegate
                    {
                        file = await folder.CreateFileAsync(s_helpFileName, CreationCollisionOption.OpenIfExists);

                        await xml.Save(file);
                    });

                    tk.Start();
                }
            }

            return xml;
        }

        #endregion

        #region User Data

        public static async Task PostFeedback(Feedback fb)
        {
            try
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode top = xml.AddRoot("Topic");
                fb.Write(top);

                UseControlServiceClient client = OpenUserCtrlService();
                await client.PostFeedbackAsync(xml.OuterXml());
            }
            catch (Exception e)
            {
                return;
            }
        }

        public static async Task PostRecord(Record rd)
        {
            try
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode top = xml.AddRoot("Record");
                rd.Write(top);

                UseControlServiceClient client = OpenUserCtrlService();
                await client.PostRecordAsync(xml.OuterXml());
            }
            catch (Exception e)
            {
                return;
            }
        }

        public static async Task<string> GetAttributeFilterSetting()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(s_attributeFilterSettingFileName);
            }
            catch
            {
                file = null;
            }

            if (file == null)
                return "";

            DBXmlDocument xml = null;
            try
            {
                xml = new DBXmlDocument(await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file));
            }
            catch
            {
                xml = null;
            }

            if (xml == null)
            {
                return "";
            }

            return xml.OuterXml();
        }

        public static async Task SaveAttributeFilterSetting(string settingString)
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(s_attributeFilterSettingFileName, CreationCollisionOption.ReplaceExisting);

                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, settingString);
                }
            }
            catch
            {
            }
        }

        public static async Task ClearLocalData()
        {
            // clear all files under local folder.
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                await file.DeleteAsync();
            }

            var subFolders = await ApplicationData.Current.LocalFolder.GetFoldersAsync();
            foreach (StorageFolder sub in subFolders)
            {
                // delete all except the customer's purchase data.
                if (sub.Name != s_purchaseFolderName)
                    await ClearFolder(sub, true);
            }
        }
        
        #endregion

        public static async Task<StorageFolder> GetFolder(StorageFolder parent, string path, bool bCreateIfNotExist)
        {
            StorageFolder folder = null;
            try
            {
                folder = await parent.GetFolderAsync(path);
            }
            catch
            {
                // not exists.
            }

            if (folder == null && bCreateIfNotExist)
            {
                // Get the folder tree from the path.
                string[] tree = path.Split(new char[] { '/', '\\' });
                if (tree.Count() == 1)
                {
                    folder = await parent.CreateFolderAsync(path);
                }
                else
                {
                    foreach (string sub in tree)
                    {
                        folder = await GetFolder(parent, sub, true);
                        parent = folder;
                    }
                }
            }

            return folder;
        }

        public static async Task<StorageFile> GetFile(StorageFolder folder, string fileName, bool bCreateIfNotExist)
        {
            StorageFile file = null;

            try
            {
                if (bCreateIfNotExist)
                    file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                else
                    file = await folder.GetFileAsync(fileName);
            }
            catch
            {
                file = null;
            }

            return file;
        }

        public static async Task ClearFolder(StorageFolder folder, bool deleteEmptyFolder)
        {
            var files = await folder.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                await file.DeleteAsync();
            }

            var subFolders = await folder.GetFoldersAsync();
            foreach (StorageFolder sub in subFolders)
            {
                await ClearFolder(sub, true);
            }

            // delete the empty folder.
            if (deleteEmptyFolder)
            {
                await folder.DeleteAsync();
            }
        }

        public static async Task<SoftwareVersion> GetLatestSoftwareVersion()
        {
            UseControlServiceClient client = OpenUserCtrlService();

            SoftwareVersion version = new SoftwareVersion();
            try
            {
                DBUserControlService.GetLatestSoftwareVersionRequest req = 
                    new DBUserControlService.GetLatestSoftwareVersionRequest(s_platform, 0, false);

                DBUserControlService.GetLatestSoftwareVersionResponse resp = await client.GetLatestSoftwareVersionAsync(req);

                version.Version = resp.version;
                version.SchemeChanged = resp.forceUpgradingRequired;
            }
            catch
            {
                return null;
            }

            return version;
        }

        public static async Task<string> GetReleaseNotes(int fromVer)
        {
            UseControlServiceClient client = OpenUserCtrlService();

            string output = "";
            try
            {
                output = await client.GetReleaseNotesAsync(s_platform, fromVer);
            }
            catch
            {
                return "";
            }

            return output;
        }
    }
}
