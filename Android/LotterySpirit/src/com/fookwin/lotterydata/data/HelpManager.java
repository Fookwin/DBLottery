package com.fookwin.lotterydata.data;


import java.util.ArrayList;

import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.SparseArray;

public class HelpManager extends SQLiteOpenHelper
{
	protected final static String DATABASE_NAME = "help";
	protected final static int DATABASE_VERSION = 1; 
	protected final static String TOPIC_TABLE = "topics";
	protected final static String NOTE_TABLE = "notes";
	
	protected boolean _isDBJustCreated = false;
	protected SparseArray<HelpTopic> _topics = new SparseArray<HelpTopic>();
    protected SparseArray<HelpNote> _notes = new SparseArray<HelpNote>();
    
	public HelpManager(Context context) {
		super(context, DATABASE_NAME, null, DATABASE_VERSION);
	}

	@Override
	public void onCreate(SQLiteDatabase db) {
		String sql = "CREATE TABLE " + TOPIC_TABLE
				+ "(ID INTEGER PRIMARY KEY,"
				+ " Title VARCHAR(100) NOT NULL,"
				+ " Description TEXT NOT NULL,"
				+ " Notes TEXT NOT NULL)";
		db.execSQL(sql);
		
		sql = "CREATE TABLE " + NOTE_TABLE
				+ "(ID INTEGER PRIMARY KEY,"
				+ " Content TEXT NOT NULL)";
		db.execSQL(sql);
		
		_isDBJustCreated = true;
	}

	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        String sql = "DROP TABLE IF EXISTS " + TOPIC_TABLE;  
        db.execSQL(sql);  
        sql = "DROP TABLE IF EXISTS " + NOTE_TABLE;  
        db.execSQL(sql);  
        onCreate(db);		
	}

    public HelpTopic getTopic(int id)
    {
    	HelpTopic topic = _topics.get(id);
    	if (topic == null)
    	{
    		// read from database.	
    		SQLiteDatabase db = this.getWritableDatabase();
    		if (db.isOpen())
    		{
    			Cursor cr = db.rawQuery("SELECT * FROM " + TOPIC_TABLE 
    					+ " WHERE ID=" + Integer.toString(id), null);
    			
    			if (cr.moveToFirst())
    			{
    				topic = _readTopic(cr);				
    				_topics.put(id, topic);
    			}
    			
    			cr.close();
    		}
    		
    		db.close();
    	}
    	
    	return topic;
    }
    
    private HelpTopic _readTopic(Cursor cr)
    {
    	HelpTopic item = new HelpTopic();				
		item.setID(Integer.parseInt(cr.getString(cr.getColumnIndex("ID"))));
		item.setTitle(cr.getString(cr.getColumnIndex("Title")));
		item.setDescription(cr.getString(cr.getColumnIndex("Description")));
		item.setNotes(cr.getString(cr.getColumnIndex("Notes")));
		
    	return item;
    }

    protected HelpNote getNote(int id, SQLiteDatabase opennedDB)
    {
    	HelpNote note = _notes.get(id);
    	if (note == null)
    	{
    		// read from database.	
			Cursor cr = opennedDB.rawQuery("SELECT * FROM " + NOTE_TABLE 
					+ " WHERE ID=" + Integer.toString(id), null);
			
			if (cr.moveToFirst())
			{
				note = _readNote(cr);				
				_notes.put(id, note);
			}
			
			cr.close();
    	}
    	
    	return note;
    }
    
    private HelpNote _readNote(Cursor cr)
    {
    	HelpNote item = new HelpNote();				
		item.setID(Integer.parseInt(cr.getString(cr.getColumnIndex("ID"))));
		item.setContent(cr.getString(cr.getColumnIndex("Content")));
		
    	return item;
    }

    public ArrayList<HelpNote> getTopicNotes(HelpTopic tp)
    {
    	ArrayList<HelpNote> notes = new ArrayList<HelpNote>();

		SQLiteDatabase db = this.getWritableDatabase();
		if (db.isOpen())
		{
	        for (int nodeId : tp.getNoteIDs())
	        {
	        	HelpNote note = getNote(nodeId, db);
	            if (note != null)
	                notes.add(note);
	        }
		}
		
		db.close();

        return notes;
    }
}
