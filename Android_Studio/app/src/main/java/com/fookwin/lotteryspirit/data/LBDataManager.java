package com.fookwin.lotteryspirit.data;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import android.annotation.SuppressLint;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.data.Constraint;
import com.fookwin.lotterydata.data.DataManageBase;
import com.fookwin.lotterydata.data.Feedback;
import com.fookwin.lotterydata.data.History;
import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotterydata.data.MatrixCell;
import com.fookwin.lotterydata.data.MatrixTable;
import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotterydata.data.Record;
import com.fookwin.lotterydata.data.ReleaseInfo;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotterydata.data.SchemeAttribute;
import com.fookwin.lotterydata.data.SchemeAttributeCategory;
import com.fookwin.lotterydata.data.SchemeAttributeValueStatus;
import com.fookwin.lotterydata.data.SchemeAttributes;
import com.fookwin.lotterydata.data.SchemeSelector;
import com.fookwin.lotterydata.data.Set;
import com.fookwin.lotterydata.data.StandardSchemeSelector;
import com.fookwin.lotterydata.util.AttributeUtil;
import com.fookwin.lotterydata.util.DBXmlDocument;
import com.fookwin.lotterydata.util.DBXmlNode;
import com.fookwin.lotterydata.util.DataUtil;
import com.fookwin.lotterydata.util.DataUtil.SyncContext;

public class LBDataManager extends DataManageBase {
	
	public class SoftwareVersionUpdate
	{
		public int currentVersion;
		public int latestVersion;
		public String releaseNotes = "";
		public boolean schemeChanged = false;
	}
	
	private SoftwareVersionUpdate _softVerion = null;
	private SyncContext _context = null;
	private boolean _initialized = false;

	private SchemeAttributes _latestAttributes = null;
	private PurchaseManager _purchaseMgr = null;
	private LotteryStateInfo _latestLotteryStateInfo = null;
	private Purchase _pendingPurchase = null;
	private ReleaseInfo _releaseInfo = null;
	private Lottery _latestLottery = null;
	private FilterOption _filterOption = null;

	private Object thisLockObject = new Object();

	private Handler initializingHandler = null;
	private ArrayList<PurchaseInfo> _justVerifiedPurchases;
	
	private Set markedRedsInclude = new Set();
	private Set markedBluesInclude = new Set();
	private Set markedRedsExclude = new Set();
	private Set markedBluesExclude = new Set();

	private LBDataManager() {
	}

	public static LBDataManager GetInstance() {
		if (_Instance == null) {
			_Instance = new LBDataManager();
		}

		return (LBDataManager) ((_Instance instanceof LBDataManager) ? _Instance
				: null);
	}
	
	public SoftwareVersionUpdate getSoftwareVersion()
	{
		return _softVerion;
	}

	public void setInitializingHandler(Handler h) {
		initializingHandler = h;
	}

	public final SyncContext GetSyncContext() {
		return _context;
	}

	public final boolean getInitialized() {
		return _initialized;
	}
	
	public final ArrayList<PurchaseInfo> getJustVerifiedPurchases()
	{
		return _justVerifiedPurchases;
	}
	
	public Set getMarkedBlues(boolean included)
	{
		return included ? markedBluesInclude : markedBluesExclude;
	}
	
	public Set getMarkedReds(boolean included)
	{
		return included ? markedRedsInclude : markedRedsExclude;
	}

	@SuppressLint("SimpleDateFormat")
	public final void Initialize() {
		if (_initialized) {
			return;
		}

		try {
			// Step 1: Check net work connection.
			if (!DataUtil.IsNetworkAvailable()){
				// the service is not available.
				throw new Exception("无法连接网络, 请稍后重试");
			}
			
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("连接服务器...", 0);
			}

			// Init the context.
			_context = DataUtil.ConstructContext();
			if (_context == null) {
				// the service is not available.
				throw new Exception("无法连接服务器, 请稍后重试");
			}

			// Step 2: Check software version.
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("检查版本更新...", 10);
			}
			
			DataUtil.SoftwareVersion ver = DataUtil.GetLatestSoftwareVersion();
			if (ver != null)
			{
				_softVerion = new SoftwareVersionUpdate();
				_softVerion.latestVersion = ver.Version;
				_softVerion.currentVersion = DataUtil.CurrentSoftwareVersion();
				_softVerion.schemeChanged = ver.SchemeChanged;
				if (ver != null && ver.Version > _softVerion.currentVersion) 
				{
					// Get the release notes.
					_softVerion.releaseNotes = DataUtil.GetReleaseNotes(_softVerion.currentVersion);
				}
			}

			// Step 3: initialize global members.
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("加载数据模板...", 15);
			}

			if (AttributeUtil.GetAttributesTemplate() == null) {
				SchemeAttributes template = DataUtil
						.GetAttributesTemplate(_context);
				AttributeUtil.SetAttributesTemplate(template);
			}

			// Step 4: release information.
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("加载最新开奖结果...", 30);
			}

			if (_releaseInfo == null) 
			{
				_releaseInfo = new ReleaseInfo();

				DBXmlDocument xml = DataUtil.GetLatestReleaseInfo(_context);
				if (xml != null)
				{
					_releaseInfo.Read(xml.Root());
				} else {
					throw new Exception("无法获取开奖结果，请稍后重试");
				}
			}

			// Read the latest lottery.
			if (_latestLottery == null) {
				_latestLottery = DataUtil.ReadLatestLottery(_context);
			}

			// Step 4: initializing the scheme attributes
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("加载最新分析数据...", 50);
			}
			
            if (_filterOption == null)
            {
                _filterOption = new FilterOption();

                // try to read the customized setting.
                try
                {
                    DBXmlDocument xml = DataUtil.GetAttributeFilterSetting();
                    if (xml != null)
                    {
                        DBXmlNode root = xml.Root();
                        _filterOption.Read(root);
                    }
                }
                catch (Exception e)
                {
                    // if fail, just use the default setting.
                }
            }

			if (_latestAttributes == null) {
				_latestAttributes = DataUtil.ReadSchemeAttribute(_context);
				if (_latestAttributes == null) {
					throw new Exception("无法加载分析数据，请稍后重试");
				}
			}

			// Step 5: initializing the history.
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("加载历史数据 (第一次会比较慢）...", 60);
			}

			if (_History == null) {
				_History = new History(LotterySpirit.getInstance());
				if (!_History.init()){
					throw new Exception("无法加载历史数据，请稍后重试");
				}
			}

			// Step 6: initializing purchase manager.
			// do this when the history is ready, because verifying require the
			// data.
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("兑奖中...", 70);
			}

			if (_purchaseMgr == null) {
				_purchaseMgr = new PurchaseManager(LotterySpirit.getInstance());
				
				SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd"); 				
				_purchaseMgr.init(_releaseInfo.getNextIssue(), sdf.format(_releaseInfo.getNextReleaseTime()));
				
				// verify the unverified purchases.
				_justVerifiedPurchases = _purchaseMgr.verify();
			}

			if (_pendingPurchase == null) {
				// init pending purchase.
				if (resetPendingPurchase() == null) {
					throw new Exception("无法创建购彩篮");
				}
			}

			// Step 7: initializing purchase manager.
			// do this when the history is ready, because verifying require the
			// data.
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("加载旋转矩阵模板...", 80);
			}

			if (_matrixTable == null) {
				if (InitMatrixTable() == null) {
					throw new Exception("无法加载旋转矩阵模板");
				}
			}
			
			// Step 8: initialize help center.
            if (initializingHandler != null)
            	UpdateInitilaizationProgress("加载帮助文档...", 90);

            if (!HelpCenter.Instance().dataLoaded())
            {
                if (!HelpCenter.Instance().Load())
                {
                    throw new Exception("无法加载帮助文档");
                }
            }

			// Update the version file in disk.
			DataUtil.SaveVersion(_context.CloudVersion);
			
			if (initializingHandler != null) {
				UpdateInitilaizationProgress("数据加载完毕", 100);
			}

			_initialized = true;
		
		} catch (Exception e) {
			if (initializingHandler != null) {
				UpdateInitilaizationProgress(e.getMessage(), -1);
			}
		}
	}

	private final void UpdateInitilaizationProgress(String title, int progress) {
		Message msg = new Message();
		Bundle msgData = new Bundle();
		msg.setData(msgData);
		msgData.putString("TITLE", title);
		msgData.putInt("PROGRESS", progress);
		initializingHandler.sendMessage(msg);
	}

	public final MatrixTable InitMatrixTable() throws Exception {
		if (_matrixTable != null) {
			return _matrixTable;
		}

		java.util.HashMap<String, MatrixCell> data = DataUtil
				.ReadMatrixFilters(_context);
		if (data != null) {
			_matrixTable = new MatrixTable(); // Don't create it until the data
												// is ready.
			_matrixTable.Init();

			for (java.util.Map.Entry<String, MatrixCell> pair : data.entrySet()) {
				String[] keys = pair.getKey().split("[-]", -1);
				int row = Integer.parseInt(keys[0]);
				int col = Integer.parseInt(keys[1]);
				_matrixTable.SetCell(row, col, pair.getValue());
			}

			return _matrixTable;
		} else {
			throw new RuntimeException("Fail to get the matrix data.");
		}
	}

	public final PurchaseManager getPurchaseManager() {
		return _purchaseMgr;
	}

	public final ReleaseInfo getReleaseInfo() {
		return _releaseInfo;
	}

	public final SchemeAttributes getLastAttributes() {
		if (_latestAttributes == null) {
			return null;
		}

		return _latestAttributes;
	}

	public final Purchase getPendingPurchase() {
		return _pendingPurchase;
	}

	public final Purchase resetPendingPurchase() {
		_pendingPurchase = new Purchase();
		return _pendingPurchase;
	}
	
	public final Purchase resetPendingPurchase(Purchase newTarget) {
		_pendingPurchase = newTarget;
		return _pendingPurchase;
	}
	
	public final Purchase resetPendingPurchase(Purchase copyFrom, boolean reuseSelection) {
		_pendingPurchase = new Purchase();
		
		if (!reuseSelection)
		{
			for (SchemeSelector sel : copyFrom.getSelectors())
			{
				_pendingPurchase.getSelectors().add(sel.clone());
			}	
			
			for (Constraint ct : copyFrom.getConstraints())				
			{
				_pendingPurchase.getConstraints().add(ct.clone());
			}
		}
		else
		{
			for (Scheme sm : copyFrom.getSelection())
			{
				_pendingPurchase.getSelectors().add(StandardSchemeSelector.createFrom(sm));
			}
		}
		
		// mark flag to make it computed.
		_pendingPurchase.markSelectorsRecomputeRequired();

		return _pendingPurchase;
	}

	public final Lottery getLatestLottery() {
		return _latestLottery;
	}

	public final LotteryStateInfo GetLatestLotteryStateInfo() {
		synchronized (thisLockObject) {
			if (_latestLotteryStateInfo == null) {
				if (_latestLottery == null || _releaseInfo == null) {
					return null; // data is not available.
				}

				try {
					_latestLotteryStateInfo = LotteryStateInfo.Create(
							_latestLottery, _releaseInfo);
				} catch (ParseException e) {
					e.printStackTrace();
				}
			}

			return _latestLotteryStateInfo;
		}
	}
	
	public final void PostFeedback(String name, String email, String phone, String content)
    {
        Feedback fb = new Feedback();
        fb.setDeviceID(DataUtil.getDeviceId());
        fb.setName(name);
        fb.setEmail(email);
        fb.setPhone(phone);
        fb.setContent(content);
        fb.setLocalVersion(DataUtil.CurrentSoftwareVersion());
        fb.setPlateform(DataUtil.getPlatform());

        DataUtil.PostFeedback(fb);
    }

    public final void PostRecord(int issue, int cost, int bouns, int[] prizeList)
    {
        Record rd = new Record();
        rd.setDeviceID(DataUtil.getDeviceId());
        rd.setBonus(prizeList);
        rd.setCost(cost);
        rd.setPrize(bouns);
        rd.setIssue(issue);

        DataUtil.PostRecord(rd);
    }

    public void SaveAttributeFilterOption()
    {
        if (_filterOption != null)
        {
            DBXmlDocument xml = new DBXmlDocument();
            DBXmlNode root = xml.AddRoot("Filters");
            _filterOption.Save(root);

            DataUtil.SaveAttributeFilterSetting(xml);
        }
    }

	public final FilterOption getFilterOption() {
		return _filterOption;
	}

	public final List<SchemeAttributeValueStatus> GetRecommendedConditions() {
		SchemeAttributes _attributes = getLastAttributes();
		if (_attributes == null)
			return null;

		List<SchemeAttributeValueStatus> result = new ArrayList<SchemeAttributeValueStatus>();

		for (SchemeAttributeCategory cat : _attributes.getCategories().values()) {
			for (SchemeAttribute attri : cat.getAttributes().values()) {
				for (SchemeAttributeValueStatus valueState : attri
						.getValueStates()) {
					if (_filterOption.passed(valueState)) {
						result.add(valueState);
					}
				}
			}
		}

		Collections.sort(result);
		Collections.reverse(result);

		return result;
	}
}