package com.fookwin.lotterydata.data;

import java.text.ParseException;
import java.util.ArrayList;

import org.json.JSONException;

import com.fookwin.lotterydata.util.DBXmlDocument;
import com.fookwin.lotterydata.util.DBXmlNode;
import com.fookwin.lotterydata.util.DataUtil;
import com.fookwin.lotterydata.util.DataUtil.SyncContext;
import com.fookwin.lotterydata.util.DataUtil.UpdateOptionEnum;
import com.fookwin.lotteryspirit.data.LBDataManager;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.SparseArray;

public class History extends SQLiteOpenHelper
{
	private final static String DATABASE_NAME = "lottery_history";
	private final static int DATABASE_VERSION = 1; 
	private final static String HISTORY_TABLE = "History";	

	// analysis data...
	protected Status _status = null;
	
	private SparseArray<Lottery> m_LotteriesByIndex = new SparseArray<Lottery>();
	private SparseArray<Lottery> m_LotteriesByIssue = new SparseArray<Lottery>();
	private int _countInDb = 0;
	
	private boolean _justCreated = false;

	@Override
	public void onCreate(SQLiteDatabase db) {
		String sql = "CREATE TABLE " + HISTORY_TABLE
				+ "(Issue INTEGER PRIMARY KEY,"
				+ " Inx INTEGER NOT NULL,"
				+ " ReleaseAt TEXT NOT NULL,"
				+ " Scheme TEXT NOT NULL)";
		db.execSQL(sql);
		
		_justCreated = true;
	}

	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        String sql = "DROP TABLE IF EXISTS " + HISTORY_TABLE;  
        db.execSQL(sql);  
        onCreate(db);	
	}
	
	public History(Context context)
	{
		super(context, DATABASE_NAME, null, DATABASE_VERSION);
	}
	
	public boolean init() throws Exception
	{
		// read the count in data base. 
		SQLiteDatabase db = this.getReadableDatabase();
		if (db.isOpen())
		{
			Cursor count_cr = db.rawQuery("SELECT COUNT(*) FROM "+ HISTORY_TABLE,null); 
			if (count_cr.moveToFirst())
			{
				_countInDb = (int) count_cr.getLong(0);  
			}
			count_cr.close();
		}
		db.close();
		
		// check if the local data is up-to-date.
		SyncContext context = LBDataManager.GetInstance().GetSyncContext();
		
		// need update?
		boolean bNeedToResync = context.NeedToResync(UpdateOptionEnum.NeedUpdateHistory) || _justCreated;
		boolean bNewRowdded = context.LocalVersion.getLatestIssue() != context.CloudVersion.getLatestIssue();
		if (!bNeedToResync && !bNewRowdded)
		{
			return true;
		}
		
		DBXmlDocument data = DataUtil.syncLotteryHistoryToCloud(bNeedToResync, context);
		if (data == null)
			return false;
		
		final ArrayList<Lottery> _lotteries = new java.util.ArrayList<Lottery>();
	
		DBXmlNode topNode = data.Root();
		if (topNode.Name() == "Lottery")
		{
			// read single lottery.
			Lottery lot = new Lottery();
			lot.ReadFromXml(topNode, true);
			_lotteries.add(lot);
		}
		else
		{
			// read multiple lottery.
			for (DBXmlNode childNode : topNode.ChildNodes())
			{
				Lottery lot = new Lottery();
				lot.ReadFromXml(childNode, true);
				_lotteries.add(lot);
			}
		}			

		// update the database in sync.				
		_updateDb(_lotteries, bNeedToResync);	
		
		return true;
	}
	
	private boolean _updateDb(ArrayList<Lottery> lottories, boolean clearDB)
	{
		int pre_count = _countInDb;
		boolean bSuccess = false;
		SQLiteDatabase db = this.getWritableDatabase();
		if (db.isOpen())
		{
			if (!_justCreated && clearDB)
			{
				onUpgrade(db, DATABASE_VERSION, DATABASE_VERSION);
			}
			
			db.beginTransaction();
			try {				
				for (Lottery item : lottories) 
				{					
					ContentValues values = new ContentValues();
					values.put("Issue", item.getIssue());
					values.put("ReleaseAt", item.getDateExp());
					values.put("Scheme", item.getScheme().toString());					
					
					int index = _countInDb ++;
					values.put("Inx", index); // make a new index					
					db.insert(HISTORY_TABLE, null, values);
					
    				m_LotteriesByIndex.put(index, item);
    				m_LotteriesByIssue.put(item.getIssue(), item);
				}
				
				bSuccess = true;
				db.setTransactionSuccessful();
			} finally {
				db.endTransaction();
			}
		}
		db.close();
		
		if (!bSuccess)
			_countInDb = pre_count; // recover the count before change.
		
		return bSuccess;
	}

	public final int getCount()
	{
		return _countInDb;
	}
	
	public final Lottery getByIssue(int issue)
	{
		Lottery lot = m_LotteriesByIssue.get(issue);
    	if (lot == null)
    	{
			try {
	    		// read from database.	
	    		SQLiteDatabase db = this.getReadableDatabase();
	    		if (db.isOpen())
	    		{
	    			Cursor cr = db.rawQuery("SELECT * FROM " + HISTORY_TABLE 
	    					+ " WHERE Issue=" + Integer.toString(issue), null);
	    			
	    			if (cr.moveToFirst())
	    			{
	    				lot = Lottery.create(cr);
	    				m_LotteriesByIndex.put(cr.getInt(cr.getColumnIndex("Inx")), lot);
	    				m_LotteriesByIssue.put(issue, lot);
	    			}
	    			
	    			cr.close();
	    		}
	    		
	    		db.close();
			} catch (ParseException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (JSONException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
    	}
    	
    	return lot;
	}
	
	public final Lottery getByIndex(int index)
	{
		Lottery lot = m_LotteriesByIndex.get(index);
    	if (lot == null)
    	{
			try {
	    		// read from database.	
	    		SQLiteDatabase db = this.getReadableDatabase();
	    		if (db.isOpen())
	    		{
	    			Cursor cr = db.rawQuery("SELECT * FROM " + HISTORY_TABLE 
	    					+ " WHERE Inx=" + Integer.toString(index), null);
	    			
	    			if (cr.moveToFirst())
	    			{
	    				lot = Lottery.create(cr);										
	    				m_LotteriesByIndex.put(index, lot);
	    				m_LotteriesByIssue.put(lot.getIssue(), lot);
	    			}
	    			
	    			cr.close();
	    		}
	    		
	    		db.close();
			} catch (ParseException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (JSONException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
    	}
    	
    	return lot;
	}
	
	public final int issueToIndex(int issue)
	{
		int index = -1;
		
		// read from database.	
		SQLiteDatabase db = this.getReadableDatabase();
		if (db.isOpen())
		{
			Cursor cr = db.rawQuery("SELECT * FROM " + HISTORY_TABLE 
					+ " WHERE Issue=" + Integer.toString(issue), null);
			
			if (cr.moveToFirst())
			{
				index = cr.getInt(cr.getColumnIndex("Inx"));
			}
			
			cr.close();
		}
		
		db.close();
		
		return index;
	}
}