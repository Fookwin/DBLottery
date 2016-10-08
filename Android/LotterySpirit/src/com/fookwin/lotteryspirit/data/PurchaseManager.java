package com.fookwin.lotteryspirit.data;

import java.text.ParseException;
import java.util.ArrayList;
import java.util.Date;

import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotterydata.util.DataUtil;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.SparseArray;

public class PurchaseManager extends SQLiteOpenHelper
{
	private final static String DATABASE_NAME = "user_purchase_history";
	private final static int DATABASE_VERSION = 1; 
	private final static String HISTORY_TABLE = "History";
	
	private int _nextIssue = 0;
	private int _latestID = 0;
	private int _countInDb = 0;
	private String _releaseDate = null;
	private int _totalCost = 0;
	private int _totalWin = 0;
	
	private ArrayList<PurchaseInfo> _purchasesInOrder = new ArrayList<PurchaseInfo>();
	private ArrayList<PurchaseInfo> _purchasesVerified = new ArrayList<PurchaseInfo>();
	private SparseArray<PurchaseInfo> _purchases = new SparseArray<PurchaseInfo>();
	
	public PurchaseManager(Context context) {
		super(context, DATABASE_NAME, null, DATABASE_VERSION);
	}

	@Override
	public void onCreate(SQLiteDatabase db) {
		String sql = "CREATE TABLE " + HISTORY_TABLE
				+ "(ID INTEGER PRIMARY KEY,"
				+ " Issue INTEGER NOT NULL,"
				+ " CreateAt VARCHAR(30) NOT NULL,"
				+ " ReleaseAt VARCHAR(30) NOT NULL,"
				+ " Buy INTEGER NOT NULL,"
				+ " Win INTEGER NOT NULL)";
		db.execSQL(sql);
	}

	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        String sql = "DROP TABLE IF EXISTS " + HISTORY_TABLE;  
        db.execSQL(sql);  
        onCreate(db);		
	}
	
	public void init(int nextIssue, String releaseDate)
	{
		_nextIssue = nextIssue;
		_releaseDate = releaseDate;
		
		SQLiteDatabase db = this.getWritableDatabase();
		if (db.isOpen())
		{
			Cursor count_cr = db.rawQuery("SELECT COUNT(*) FROM "+ HISTORY_TABLE,null); 
			if (count_cr.moveToFirst())
			{
				_countInDb = (int) count_cr.getLong(0);  
			}
			count_cr.close();
			
			if (_countInDb > 0)
			{
				// get the latest purchase with this issue.
				Cursor max_cr = db.rawQuery("SELECT MAX(ID) FROM " 
							+ HISTORY_TABLE 
							+ " WHERE Issue=" 
							+ Integer.toString(_nextIssue), null);
				
				if (max_cr.moveToFirst())
				{
					_latestID = (int) max_cr.getLong(0); 
				}
				
				max_cr.close();
				
				readCostAndWin(db);
			}

			db.close();
		}
		
		if (_latestID == 0)
		{
			// initialize it
			_latestID = _nextIssue * 1000; // reserve 1000 purchases for each issue.
		}
	}
	
	private void readCostAndWin(SQLiteDatabase db)
	{
		_totalCost = 0;
		_totalWin = 0;
		
		// get the total cost.
		Cursor cost_cr = db.rawQuery("SELECT SUM(Buy) FROM " + HISTORY_TABLE, null);
	
		if (cost_cr.moveToFirst())
		{
			_totalCost = (int) cost_cr.getLong(0) * 2; 
		}
		
		cost_cr.close();
		
		// get the total earning.
		Cursor earning_cr = db.rawQuery("SELECT SUM(Win) FROM " + HISTORY_TABLE
				+ " WHERE Win>=0", null);
	
		if (earning_cr.moveToFirst())
		{
			_totalWin = (int) earning_cr.getLong(0); 
		}
		
		earning_cr.close();
	}
	
	public int getTotalCost()
	{
		return _totalCost;
	}
	
	public int getTotalWin()
	{
		return _totalWin;
	}
	
	public void remove(int id)
	{
		// remove the record from db.
		SQLiteDatabase db = this.getWritableDatabase();
		if (db.isOpen())
		{	
			db.delete(HISTORY_TABLE, "ID=?", new String[] {String.valueOf(id)});
			
			// reset the cached data.
			_countInDb --;			
			readCostAndWin(db);
			
			db.close();
		}
		
		// delete purchase detail from disk.
		DataUtil.DeletePurchase(id);
		
		// remove it from cache list.
		int index = 0;
		for (PurchaseInfo item : _purchasesInOrder)			
		{
			if (item.ID == id)
			{
				_purchasesInOrder.remove(index);
				
				break;
			}
			
			++ index;
		}
		
		// remove it from the verified list
		index = 0;
		for (PurchaseInfo item : _purchasesVerified)			
		{
			if (item.ID == id)
			{
				_purchasesVerified.remove(index);
				
				break;
			}
			
			++ index;
		}
		
		// remove from cache
		_purchases.delete(id);
	}
	
	public void commit(Purchase order)
	{
		PurchaseInfo info = null;
		boolean isNew = order.getId() < 0;
		if (isNew)
		{
			// this is a unsaved purchase.
			int id = ++ _latestID;
			order.setId(id);
			
			Date now = new Date();
			
			// add to cache collection.
			info = new PurchaseInfo();
			info.setSource(order);
			info.ID = id;
			info.Issue = _nextIssue;
			info.CreateAt = now.toString();
			info.ReleaseAt = _releaseDate;
			info.Buy = order.getSelection().size();
			info.Win = -2;
			

			_countInDb ++;
			_totalCost += info.Buy * 2;
			
			// add to cache
			_purchases.append(id, info);
			
			// add it to just verified list.
			_purchasesVerified.add(info);
			
			// always add to top.
			_purchasesInOrder.add(0, info); 
		}
		else
		{
			// get existing purchase
			info = _purchases.get(order.getId());
			if (info == null)
				return;
			
			// update the info.
			Date now = new Date();
			info.CreateAt = now.toString();
			info.Buy = order.getSelection().size();
		}

		// save the purchase to disk
		if (DataUtil.SavePurchase(order))
		{	
			// Add a record in database.
			SQLiteDatabase db = this.getWritableDatabase();
			if (db.isOpen())
			{				
				ContentValues values = new ContentValues();
				values.put("ID", info.ID);
				values.put("Issue", info.Issue);
				values.put("CreateAt", info.CreateAt);
				values.put("ReleaseAt", info.ReleaseAt);
				values.put("Buy", info.Buy);
				values.put("Win", info.Win);
				
				if (isNew)
				{
					db.insert(HISTORY_TABLE, null, values);
				}
				else
				{
					db.update(HISTORY_TABLE, values, "ID=?", new String[] {String.valueOf(order.getId())});
				}
				
				order.setDirty(false); // clean the dirty flag.
				db.close();
			}
		}
	}
	
	public ArrayList<PurchaseInfo> verify()
	{
		// select the unverified purchases		
		SQLiteDatabase db = this.getWritableDatabase();
		if (db.isOpen())
		{
			Cursor cr = db.rawQuery("SELECT * FROM "
									+ HISTORY_TABLE
									+ " WHERE Win<0"
									+ " ORDER BY ID DESC", null);
							
			while (cr.moveToNext())
			{
				PurchaseInfo item = _read(cr);
				try {
					if (item.verify())
					{
						db.execSQL("UPDATE " + HISTORY_TABLE 
								+ " SET Win= " + item.Win
								+ " WHERE ID=" + item.ID);
					}
				} catch (SQLException e) {
					e.printStackTrace();
				} catch (ParseException e) {
					e.printStackTrace();
				}
				
				// set to cache.
				_purchases.append(item.ID, item);
				_purchasesVerified.add(item);
			}
			
			cr.close();
		}
		
		db.close();
		
		return _purchasesVerified;
	}
	
	private PurchaseInfo _read(Cursor cr)
	{
		PurchaseInfo item = new PurchaseInfo();				
		item.ID = Integer.parseInt(cr.getString(cr.getColumnIndex("ID")));
		item.Issue = Integer.parseInt(cr.getString(cr.getColumnIndex("Issue")));
		item.CreateAt = cr.getString(cr.getColumnIndex("CreateAt"));
		item.ReleaseAt = cr.getString(cr.getColumnIndex("ReleaseAt"));
		item.Buy = Integer.parseInt(cr.getString(cr.getColumnIndex("Buy")));
		item.Win = Integer.parseInt(cr.getString(cr.getColumnIndex("Win")));
		
		return item;
	}
	
	public int getPurchaseCount()
	{
		return _countInDb;
	}
	
	private boolean readMorePurchasesInOrder(int returnCount)
	{
		int readCount = Math.min(_countInDb - _purchasesInOrder.size(), returnCount);
		if (readCount <= 0)
			return false;
		
		// latest 
		SQLiteDatabase db = this.getWritableDatabase();
		if (db.isOpen())
		{
			String sql= "SELECT * FROM " + HISTORY_TABLE 
					+ " ORDER BY ID DESC"
					+ " LIMIT " + Integer.toString(readCount) 
					+ " OFFSET " + Integer.toString(_purchasesInOrder.size()); 
			Cursor cr = db.rawQuery(sql, null);
			
			while (cr.moveToNext())
			{
				int id = Integer.parseInt(cr.getString(cr.getColumnIndex("ID")));
				
				// exist in cache?
				PurchaseInfo item = _purchases.get(id);
				if (item == null)
				{
					item = _read(cr);	
					_purchases.append(id, item);
				}
				
				_purchasesInOrder.add(item);
			}
			
			cr.close();
		}

		return false;
	}
	
	public PurchaseInfo getPurchase(int index)
	{
		if (index >= _countInDb)
			return null;
		
		if (index >= _purchasesInOrder.size())
		{
			// not loaded, get it from data base.
			// to get better performance, read multiple items in once.
			readMorePurchasesInOrder(10);
		}
		
		return _purchasesInOrder.get(index);
	}
	
	public PurchaseInfo getPurchaseById(int id)
	{		
		// find it in cache
		PurchaseInfo foundItem = _purchases.get(id);
		if (foundItem != null)
		{
			return foundItem;
		}
		
		// read from db.
		SQLiteDatabase db = this.getWritableDatabase();
		if (db.isOpen())
		{
			String sql= "SELECT * FROM " + HISTORY_TABLE + " WHERE ID=" + Integer.toString(id); 
			Cursor cr = db.rawQuery(sql, null);
			
			if (cr.moveToNext())
			{
				foundItem = _read(cr);
				_purchases.append(foundItem.ID, foundItem);
			}
			
			cr.close();
		}
		
		return foundItem;
	}
}