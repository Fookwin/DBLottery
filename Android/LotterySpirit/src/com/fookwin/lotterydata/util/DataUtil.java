package com.fookwin.lotterydata.util;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.UUID;

import android.annotation.SuppressLint;
import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.telephony.TelephonyManager;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.data.Constraint;
import com.fookwin.lotterydata.data.ConstraintTypeEnum;
import com.fookwin.lotterydata.data.DataVersion;
import com.fookwin.lotterydata.data.Feedback;
import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotterydata.data.MatrixCell;
import com.fookwin.lotterydata.data.MatrixItemByte;
import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotterydata.data.Record;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotterydata.data.SchemeAttributes;
import com.fookwin.lotterydata.data.SchemeSelector;
import com.fookwin.lotterydata.data.SchemeSelectorTypeEnum;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetAllLotteriesBase;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetDataVersion;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetHelp;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetLatestAttributes;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetLotteriesBaseByIssue;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetLotteryData;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetAttributesTemplate;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetLatestReleaseInfo;
import com.fookwin.lotterydata.webservice.DBSqlServiceStub.GetMatrixTableItem;
import com.fookwin.lotterydata.webservice.DBUseControlServiceStub.GetLatestSoftwareVersion;
import com.fookwin.lotterydata.webservice.DBUseControlServiceStub.GetLatestSoftwareVersionResponse;
import com.fookwin.lotterydata.webservice.DBUseControlServiceStub.GetReleaseNotes;
import com.fookwin.lotterydata.webservice.DBUseControlServiceStub;
import com.fookwin.lotterydata.webservice.DBUseControlServiceStub.UserLogin;

public class DataUtil
{
	public static class SoftwareVersion
	{
		public int Version = -1;
		public boolean SchemeChanged = false;
	}

	public static class SyncContext
	{
		public DataVersion LocalVersion = null;
		public DataVersion CloudVersion = null;

		public final boolean NeedToResync(UpdateOptionEnum option)
		{
			if (LocalVersion == null || CloudVersion == null)
			{
				return false;
			}

			switch (option)
			{
				case NeedUpdateHistory:
					{
						return LocalVersion.getHistoryDataVersion() != CloudVersion.getHistoryDataVersion();
					}
				case NeedUpdateReleaseInfo:
					{
						return LocalVersion.getReleaseDataVersion() != CloudVersion.getReleaseDataVersion();
					}
				case NeedUpdateAttributes:
					{
						return LocalVersion.getAttributeDataVersion() != CloudVersion.getAttributeDataVersion();
					}
				case NeedUpdateAttributesTemplate:
					{
						return LocalVersion.getAttributeTemplateVersion() != CloudVersion.getAttributeTemplateVersion();
					}
				case NeedUpdateMatrixTable:
					{
						return LocalVersion.getMatrixDataVersion() != CloudVersion.getMatrixDataVersion();
					}
				case NeedUpdateLatestLottery:
					{
						return LocalVersion.getLatestIssue() < CloudVersion.getLatestIssue() || LocalVersion.getLatestLotteryVersion() != CloudVersion.getLatestLotteryVersion();
					}
				case NeedUpdateHelpContent:
                {
                    return LocalVersion.getHelpContentVersion() != CloudVersion.getHelpContentVersion();
                }
			}

			return false;
		}
	}

	public enum UpdateOptionEnum
	{
		NeedUpdateHistory(0), // update the history.xml
		NeedUpdateReleaseInfo(1), // update release information.
		NeedUpdateAttributes(2), // update attributes.
		NeedUpdateAttributesTemplate(3), // update attribute template
		NeedUpdateMatrixTable(4), // update matrix table.
		NeedUpdateLatestLottery(5), // update the details of the latest lottery.
		NeedUpdateHelpContent(6); // update the help xml.

		private int intValue;
		private static java.util.HashMap<Integer, UpdateOptionEnum> mappings;
		@SuppressLint("UseSparseArrays")
		private synchronized static java.util.HashMap<Integer, UpdateOptionEnum> getMappings()
		{
			if (mappings == null)
			{
				mappings = new java.util.HashMap<Integer, UpdateOptionEnum>();
			}
			return mappings;
		}

		private UpdateOptionEnum(int value)
		{
			intValue = value;
			UpdateOptionEnum.getMappings().put(value, this);
		}

		public int getValue()
		{
			return intValue;
		}

		public static UpdateOptionEnum forValue(int value)
		{
			return getMappings().get(value);
		}
	}

	private static int s_currentSoftwareVersion = 20150803;
	private static final String s_releaseInfoFileName = "ReleaseInformation.xml";
	private static final String s_attributesFileName = "LatestAttributes.xml";
	private static final String s_latestLotteryFileName = "LatestLottery.xml";
	private static final String s_versionFileName = "Version.xml";
	private static final String s_attributesTemplateFileName = "AttributeTemplate.xml";
    private static final String s_attributeFilterSettingFileName = "AttributeFilterSetting.xml";
	private static final int s_platform = 3;
    private static final String s_purchaseFolderName = "SavedSelection";
	private static final String s_latestInstallation = "http://dbdatastorage.blob.core.windows.net/dbinstallers/LotterySpirit_";
	private static String s_deviceID = "";
	
	private static DBSqlServiceStub _dataClient = null;
	private static DBUseControlServiceStub _useCtrlClient = null;

    public static int getPlatform()
    {
        return s_platform;
    }
    
	public static String getLatestApk(int version)
	{
		return s_latestInstallation + Integer.toString(version) + ".apk";
	}
	
	public static int CurrentSoftwareVersion()
	{
		return s_currentSoftwareVersion;
	}

	public static SyncContext ConstructContext() throws Exception
	{
		DataVersion versionOnCloud = null;
		try
		{
			// Read the cloud context.
			DBSqlServiceStub client = OpenDataService();

			// Check the version in cloud to determine if an update need to be performed.
			GetDataVersion req = new GetDataVersion();
			String dataVersion = client.getDataVersion(req).getGetDataVersionResult();

			versionOnCloud = new DataVersion();

			DBXmlDocument xml = new DBXmlDocument();
			xml.Load(dataVersion);

			versionOnCloud.ReadFromXml(xml.Root());
		}
		catch (java.lang.Exception e)
		{
			return null;
		}

		SyncContext context = new SyncContext();

		// Read the local context.
		StorageFile file = StorageFolder.getPrivateFolder().getFile(s_versionFileName);

		context.LocalVersion = new DataVersion();

		if (file != null)
		{
			DBXmlDocument xml = new DBXmlDocument();
			xml.Load(file.readString());
			DBXmlNode root = xml.Root();

			context.LocalVersion.ReadFromXml(root);
		}

		context.CloudVersion = versionOnCloud;

		return context;
	}

	public static void SaveVersion(DataVersion ver)
	{
		DBXmlDocument xml = new DBXmlDocument();
		DBXmlNode root = xml.AddRoot("Version");

		ver.SaveToXml(root);

		StorageFolder folder = StorageFolder.getPrivateFolder();
		StorageFile file = folder.createFile(s_versionFileName);
		xml.Save(file.getPath());
	}

	public static DBSqlServiceStub OpenDataService()
	{
		if (_dataClient == null)
		{
			_dataClient = new DBSqlServiceStub();
		}

		return _dataClient;
	}

	public static DBUseControlServiceStub OpenUserCtrlService()
	{
		if (_useCtrlClient == null)
		{
			_useCtrlClient = new DBUseControlServiceStub();
		}

		return _useCtrlClient;
	}

	public static Lottery ReadLatestLottery(SyncContext context) throws Exception
	{
		StorageFolder folder = StorageFolder.getPrivateFolder();

		DBXmlDocument xml = null;
		try
		{
			StorageFile file = folder.getFile(s_latestLotteryFileName);
			
			if (file == null 
				|| context.LocalVersion.getLatestIssue() != context.CloudVersion.getLatestIssue() 
				|| context.NeedToResync(UpdateOptionEnum.NeedUpdateLatestLottery))
			{
				// Always get the final release lottery from service.
				DBSqlServiceStub client = OpenDataService();

				GetLotteryData req = new GetLotteryData();
				req.setIssue(context.CloudVersion.getLatestIssue());
				
				String data = client.getLotteryData(req).getGetLotteryDataResult();

				if (data != null)
				{
					xml = new DBXmlDocument();
					xml.Load(data);
					
					file = folder.createFile(s_latestLotteryFileName);
					xml.Save(file.getPath()); // save to disk.
				}
			}
			else
			{
				xml = new DBXmlDocument();
				xml.Load(file.readString());
			}
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to get latest lottery file.");
		}
		
		if (xml != null)
		{
			DBXmlNode root = xml.Root();
			Lottery lot = new Lottery();
			lot.ReadFromXml(root, false);
			
			return lot;
		}

		return null;
	}
	
	public static DBXmlDocument getLotteryData(int issue) throws Exception
	{
		try
		{
			// Always get the final release lottery from service.
			DBSqlServiceStub client = OpenDataService();

			GetLotteryData req = new GetLotteryData();
			req.setIssue(issue);
			
			String data = client.getLotteryData(req).getGetLotteryDataResult();

			if (data != null)
			{
				DBXmlDocument xml = new DBXmlDocument();
				xml.Load(data);

				return xml;
			}
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to read latest lottery.");
		}

		return null;
	}
	
	public static DBXmlDocument syncLotteryHistoryToCloud(boolean resyncAll, SyncContext context) throws Exception
	{
		try
		{
			// Updating...
			DBSqlServiceStub client = OpenDataService();
			String data = "";
			if (resyncAll)
			{
				// need rebuild the cache data. 
				data = client.getAllLotteriesBase(new GetAllLotteriesBase()).getGetAllLotteriesBaseResult();
			}
			else
			{
				// get the data since last sync.
				GetLotteriesBaseByIssue req = new GetLotteriesBaseByIssue();
				req.setIssue_from(context.LocalVersion.getLatestIssue());
				req.setIssue_to(context.CloudVersion.getLatestIssue());
				
				data = client.getLotteriesBaseByIssue(req).getGetLotteriesBaseByIssueResult();
			}

			if (data != null)
			{
				DBXmlDocument xml = new DBXmlDocument();
				xml.Load(data);
				
				return xml;
			}

			return null;
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to sync history to cloud.");
		}
	}

	public static SchemeAttributes ReadSchemeAttribute(SyncContext context) throws Exception
	{
		SchemeAttributes template = AttributeUtil.GetAttributesTemplate();
		if (template == null)
		{
			return null;
		}

		StorageFile latestAttributesfile =  GetLatestAttributesFile(context);
		if (latestAttributesfile == null)
		{
			return null;
		}

		// Create the attributes from template.
		SchemeAttributes attributes = template.clone();

		try
		{
			// Parsing the latest data.
			DBXmlDocument xml = new DBXmlDocument();
			xml.Load(latestAttributesfile.readString());
			DBXmlNode root = xml.Root();
			attributes.ReadValueFromXml(root);
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to read scheme attributes.");
		}

		return attributes;
	}

	public static SchemeAttributes GetAttributesTemplate(SyncContext context) throws Exception
	{
		StorageFolder folder = StorageFolder.getPrivateFolder();

		StorageFile file = null;
		try
		{
			file = folder.getFile(s_attributesTemplateFileName);
			
			if (file == null || context.NeedToResync(UpdateOptionEnum.NeedUpdateAttributesTemplate))
			{
				// Get it from cloud.
				DBSqlServiceStub client = OpenDataService();

				String templateString = client.getAttributesTemplate(new GetAttributesTemplate()).getGetAttributesTemplateResult();
				if (templateString.length() > 0)
				{
					DBXmlDocument xml = new DBXmlDocument();
					xml.Load(templateString);

					file = folder.createFile(s_attributesTemplateFileName);

					xml.Save(file.getPath());
				}
			}
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to get attributes template.");
		}

		// Create the attributes from template.
		SchemeAttributes attributes = new SchemeAttributes();
		{
			DBXmlDocument xml = new DBXmlDocument();
			xml.Load(file.readString());
			DBXmlNode root = xml.Root();
			attributes.ReadFromTemplate(root);
		}

		return attributes;
	}

	private static StorageFile GetLatestAttributesFile(SyncContext context) throws Exception
	{
		StorageFolder folder = StorageFolder.getPrivateFolder();

		StorageFile file = null;
		try
		{
			file = folder.getFile(s_attributesFileName);
			
			if (file == null || context.LocalVersion.getLatestIssue() != context.CloudVersion.getLatestIssue() || context.NeedToResync(UpdateOptionEnum.NeedUpdateAttributes))
			{
				// Get it from cloud.
				DBSqlServiceStub client = OpenDataService();

				String templateString = client.getLatestAttributes(new GetLatestAttributes()).getGetLatestAttributesResult();
				if (templateString.length() > 0)
				{
					DBXmlDocument xml = new DBXmlDocument();
					xml.Load(templateString);

					file = folder.createFile(s_attributesFileName);

					xml.Save(file.getPath());
				}
			}
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to get latest attributes file.");
		}

		return file;
	}

	// Return the location.
	public static boolean SavePurchase(Purchase order)
	{
		// Prepare for the storage location.
		String folderPath = s_purchaseFolderName + "/" + Integer.toString(order.getId());
		StorageFolder schemeFolder = GetFolder(StorageFolder.getSDCardFolder(), folderPath, true);

		// Save selectors.
		if (order.getSelectors().size() > 0)
		{
			DBXmlDocument xml = new DBXmlDocument();
			DBXmlNode root = xml.AddRoot("Selectors");

			for (SchemeSelector sel : order.getSelectors())
			{
				DBXmlNode node = root.AddChild("Item");
				node.SetAttribute("Type", Integer.toString(sel.GetSelectorType().getValue()));
				node.SetAttribute("Expression", sel.getExpression());
			}

			StorageFile file = schemeFolder.createFile("Selectors.xml");

			xml.Save(file.getPath());
		}

		// Save filters.
		if (order.getConstraints().size() > 0)
		{
			DBXmlDocument xml = new DBXmlDocument();
			DBXmlNode root = xml.AddRoot("Filters");

			for (Constraint con : order.getConstraints())
			{
				DBXmlNode node = root.AddChild("Item");
				node.SetAttribute("Type", Integer.toString(con.GetConstraintType().getValue()));
				con.WriteToXml(node);
			}

			StorageFile file = schemeFolder.createFile("Filters.xml");

			xml.Save(file.getPath());
		}

		// Save result.
		if (order.getSelection().size() > 0)
		{
			int count = order.getSelection().size();
			String output = "";

			for (int i = 0; i < count; ++ i)
			{
				if (i > 0)
					output += " ";
				
				Scheme sm = order.getSelection().get(i);
				output += Long.toString(sm.getIndex()) + "+" + Integer.toString(sm.getBlue());
			}

			StorageFile file = schemeFolder.createFile("Selection.txt");

			file.writeText(output);
		}

		return true;
	}

	public static Purchase ReadPurchase(int id)
	{
		String folderPath = s_purchaseFolderName + "/" + Integer.toString(id);
		StorageFolder purchasFolder = GetFolder(StorageFolder.getSDCardFolder(), folderPath, false);
		if (purchasFolder == null)
		{
			return null;
		}

		Purchase data = new Purchase();
		data.setId(id);

		// Read selectors.
		StorageFile selectorsfile = GetFile(purchasFolder, "Selectors.xml", false);
		if (selectorsfile != null)
		{
			DBXmlDocument xml = new DBXmlDocument();
			xml.Load(selectorsfile.readString());
			DBXmlNode root = xml.Root();

			java.util.ArrayList<DBXmlNode> subNodes = root.ChildNodes();
			for (DBXmlNode node : subNodes)
			{
				SchemeSelectorTypeEnum type = SchemeSelectorTypeEnum.forValue(Integer.parseInt(node.GetAttribute("Type")));
				SchemeSelector selector = SchemeSelector.CreateSelector(type);
				selector.ParseExpression(node.GetAttribute("Expression"));
				data.getSelectors().add(selector);
			}
		}

		// Read filters.
		StorageFile filtersfile = GetFile(purchasFolder, "Filters.xml", false);
		if (filtersfile != null)
		{
			DBXmlDocument xml = new DBXmlDocument();
			xml.Load(filtersfile.readString());
			DBXmlNode root = xml.Root();

			java.util.ArrayList<DBXmlNode> subNodes = root.ChildNodes();
			for (DBXmlNode node : subNodes)
			{
				ConstraintTypeEnum type = ConstraintTypeEnum.forValue(Integer.parseInt(node.GetAttribute("Type")));
				Constraint con = Constraint.CreateConstraint(type);
				con.ReadFromXml(node);

				data.getConstraints().add(con);
			}
		}

		// Read schemes.
		StorageFile schemesfile = GetFile(purchasFolder, "Selection.txt", false);
		if (schemesfile != null)
		{
			String textString = schemesfile.readString();
			String[] subStrings = textString.split(" ");

			for (String line : subStrings)
			{
				Scheme item = Scheme.parseFromIdentifier(line);
				if (item != null)
				{
					data.getSelection().add(item);
				}
			}
		}

		return data;
	}

	public static void DeletePurchase(int id)
	{
		String folderPath = s_purchaseFolderName + "/" + Integer.toString(id);
		StorageFolder selectionFolder = GetFolder(StorageFolder.getSDCardFolder(), folderPath, false);
		if (selectionFolder != null)
		{
			selectionFolder.delete();
		}
	}

	public static DBXmlDocument GetLatestReleaseInfo(SyncContext context) throws Exception
	{
		StorageFolder folder = StorageFolder.getPrivateFolder();

		StorageFile file = null;
		try
		{
			file = folder.getFile(s_releaseInfoFileName);
			
			if (file == null || context.LocalVersion.getLatestIssue() != context.CloudVersion.getLatestIssue() || context.NeedToResync(UpdateOptionEnum.NeedUpdateReleaseInfo))
			{
				// Get it from cloud.
				DBSqlServiceStub client = OpenDataService();

				String releaseString = client.getLatestReleaseInfo(new GetLatestReleaseInfo()).getGetLatestReleaseInfoResult();
				if (releaseString.length() > 0)
				{
					DBXmlDocument xml = new DBXmlDocument();
					xml.Load(releaseString);

					file = folder.createFile(s_releaseInfoFileName);

					xml.Save(file.getPath());
					
					return xml;
				}
				
				return null;
			}
			else
			{
				DBXmlDocument xml = new DBXmlDocument();
				xml.Load(file.readString());
				return xml;
			}
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to get latest release info.");
		}
	}

	public static HashMap<String, MatrixCell> ReadMatrixFilters(SyncContext context) throws Exception
	{
		// Prepare for the storage location.
		String folderPath = "Matrix";
		StorageFolder matrixFolder = GetFolder(StorageFolder.getPrivateFolder(), folderPath, true);
		if (matrixFolder != null)
		{
			HashMap<String, MatrixCell> result = new HashMap<String, MatrixCell>();

			try
			{
				for (int i = 7; i <= 33; ++i)
				{
					for (int j = 3; j <= 6; ++j)
					{
						// currently we just need support 6-5.
						if (j != 6 || i > 20)
						{
							continue;
						}
						
						// Read from the file.
						String fileName = Integer.toString(i) + "-" + Integer.toString(j) + ".txt";
						StorageFile file = GetFile(matrixFolder, fileName, false);
						if (file == null || context.NeedToResync(UpdateOptionEnum.NeedUpdateMatrixTable))
						{
							// Get the file from cloud.
							// Get it from cloud.
							DBSqlServiceStub client = OpenDataService();
	
							GetMatrixTableItem req = new GetMatrixTableItem();
							req.setCandidateCount(i);
							req.setSelectCount(j);
							
							String templateString = client.getMatrixTableItem(req).getGetMatrixTableItemResult();
							if (templateString.length() > 0)
							{
								file = matrixFolder.createFile(fileName);
	
								file.writeText(templateString.trim());
							}
							else
							{
								throw new RuntimeException("Failed to get matrix data from server.");
							}
						}
	
						if (file != null)
						{
							MatrixCell cell = new MatrixCell();
	
							java.util.List<String> lines = file.readLines();
	
							for (String line : lines)
							{
								cell.Template.add(new MatrixItemByte(line));
							}
	
							result.put(Integer.toString(i) + "-" + Integer.toString(j), cell);
						}
					}
				}
			}
			catch (java.lang.Exception e)
			{
				throw new Exception("Failed to read matrix template. Detail: " + e.getMessage());
			}

			return result;
		}
		else
		{
			throw new RuntimeException("Could not find matrix folder!");
		}
	}
	
	public static String getDeviceId()
	{
		if (s_deviceID == "")
		{
			Context baseContext = LotterySpirit.getInstance().getBaseContext();
		    final TelephonyManager tm = (TelephonyManager) baseContext.getSystemService(Context.TELEPHONY_SERVICE);
	
		    final String tmDevice, tmSerial, androidId;
		    tmDevice = "" + tm.getDeviceId();
		    tmSerial = "" + tm.getSimSerialNumber();
		    androidId = "" + android.provider.Settings.Secure.getString(LotterySpirit.getInstance().getContentResolver(), android.provider.Settings.Secure.ANDROID_ID);
	
		    UUID deviceUuid = new UUID(androidId.hashCode(), ((long)tmDevice.hashCode() << 32) | tmSerial.hashCode());
		    s_deviceID = deviceUuid.toString();
		}
	    
	    return s_deviceID;
	}
	
	public static void Login(String channelUri) throws Exception
	{
		String deviceID = getDeviceId();

		try
		{		
			DBUseControlServiceStub client = OpenUserCtrlService();
			
			UserLogin req = new UserLogin();
			req.setPlatform(s_platform);
			req.setClientVersion(s_currentSoftwareVersion);
			req.setDevId(deviceID);
			req.setInfo(channelUri);
			
			client.userLogin(req);
		}
		catch (java.lang.Exception e)
		{
			e.printStackTrace();
			throw new Exception("Failed to login.");
		}
	}
	
	public static DBXmlDocument syncHelpToCloud()
    {
        DBXmlDocument xml = null;
        
    	DBSqlServiceStub client = OpenDataService();
        String helpString = "";
		try 
		{
			helpString = client.getHelp(new GetHelp()).getGetHelpResult();
		} 
		catch (Exception e) 
		{
			e.printStackTrace();
		}            
		
        if (helpString.length() > 0)
        {
            xml = new DBXmlDocument();
            xml.Load(helpString);
        }

        return xml;
    }

    public static void PostFeedback(Feedback fb)
    {
        try
        {
            DBXmlDocument xml = new DBXmlDocument();
            DBXmlNode top = xml.AddRoot("Topic");
            fb.Write(top);

            DBUseControlServiceStub client = OpenUserCtrlService();
            
            DBUseControlServiceStub.PostFeedback input = new DBUseControlServiceStub.PostFeedback();
            input.setFeedback(xml.GetText());
            client.postFeedback(input);
        }
        catch (Exception e)
        {
            return;
        }
    }

    public static void PostRecord(Record rd)
    {
        try
        {
            DBXmlDocument xml = new DBXmlDocument();
            DBXmlNode top = xml.AddRoot("Record");
            rd.Write(top);

            DBUseControlServiceStub client = OpenUserCtrlService();
            
            DBUseControlServiceStub.PostRecord input = new DBUseControlServiceStub.PostRecord();
            input.setRecord(xml.GetText());
            client.postRecord(input);
        }
        catch (Exception e)
        {
            return;
        }
    }

    public static DBXmlDocument GetAttributeFilterSetting()
    {
        StorageFolder folder = StorageFolder.getPrivateFolder();
        StorageFile file = null;
        try
        {
            file = GetFile(folder, s_attributeFilterSettingFileName, false);
        }
        catch (Exception e)
        {
            file = null;
        }

        if (file == null)
            return null;

        DBXmlDocument xml = null;
        try
        {
            xml = new DBXmlDocument();
            xml.Load(file.readString());
        }
        catch (Exception e)
        {
            xml = null;
        }

        return xml;
    }

    public static void SaveAttributeFilterSetting(DBXmlDocument setting)
    {
        try
        {
            StorageFile file = GetFile(StorageFolder.getPrivateFolder(), s_attributeFilterSettingFileName, true);

            if (file != null)
            {
            	setting.Save(file.getPath());
            }
        }
        catch (Exception e)
        {
        }
    }

	public static StorageFolder GetFolder(StorageFolder parent, String path, boolean bCreateIfNotExist)
	{
		StorageFolder folder = parent.createFolder(path);

		if (folder == null && bCreateIfNotExist)
		{
			parent.createFolder(path);
		}

		return folder;
	}

	public static StorageFile GetFile(StorageFolder folder, String fileName, boolean bCreateIfNotExist)
	{
		StorageFile file = null;

		if (bCreateIfNotExist)
		{
			file = folder.createFile(fileName);
		}
		else
		{
			file = folder.getFile(fileName);
		}

		return file;
	}

	public static SoftwareVersion GetLatestSoftwareVersion() throws Exception
	{
		SoftwareVersion version = new SoftwareVersion();
		try
		{
			DBUseControlServiceStub client = OpenUserCtrlService();
			
			GetLatestSoftwareVersion req = new GetLatestSoftwareVersion();
			req.setPlatform(s_platform);

			GetLatestSoftwareVersionResponse resp = client.getLatestSoftwareVersion(req);

			version.Version = resp.getVersion();
			version.SchemeChanged = resp.getForceUpgradingRequired();
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to get latest software version.");
		}

		return version;
	}

	public static String GetReleaseNotes(int fromVer) throws Exception
	{
		String output = "";
		try
		{
			DBUseControlServiceStub client = OpenUserCtrlService();
			
			GetReleaseNotes req = new GetReleaseNotes();
			req.setPlatform(s_platform);
			req.setClientVersion(fromVer);
			
			output = client.getReleaseNotes(req).getGetReleaseNotesResult();
		}
		catch (java.lang.Exception e)
		{
			throw new Exception("Failed to get release notes.");
		}

		return output;
	}
	
	public static boolean IsNetworkAvailable()
	{
		ConnectivityManager connManger = (ConnectivityManager) LotterySpirit.getInstance().getSystemService(Context.CONNECTIVITY_SERVICE);

		NetworkInfo active_info = connManger.getActiveNetworkInfo();
		return active_info != null && active_info.isAvailable() && active_info.isConnected();
	}
	
	public static void cleanLocalFiles()
	{
		StorageFolder folder = StorageFolder.getPrivateFolder();
		
		// delete all folders except purchase folder.
		ArrayList<StorageFolder> folders = folder.getFolders();
		for (StorageFolder fd : folders)
		{
			String name = fd.getDisplayName();
			if (!name.equalsIgnoreCase(s_purchaseFolderName))
				fd.delete();
		}
		
		// delete all files.
		ArrayList<StorageFile> files = folder.getFiles();
		for (StorageFile fl : files)
		{
			fl.delete();
		}
	}
}