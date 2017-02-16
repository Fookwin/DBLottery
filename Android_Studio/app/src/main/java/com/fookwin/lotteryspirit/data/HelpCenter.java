package com.fookwin.lotteryspirit.data;

import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.database.sqlite.SQLiteDatabase;
import android.os.Bundle;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.data.HelpManager;
import com.fookwin.lotterydata.data.HelpNote;
import com.fookwin.lotterydata.data.HelpTopic;
import com.fookwin.lotterydata.util.DBXmlDocument;
import com.fookwin.lotterydata.util.DBXmlNode;
import com.fookwin.lotterydata.util.DataUtil;
import com.fookwin.lotterydata.util.DataUtil.SyncContext;
import com.fookwin.lotterydata.util.DataUtil.UpdateOptionEnum;
import com.fookwin.lotteryspirit.HelpActivity;

public class HelpCenter extends HelpManager
{
	public HelpCenter(Context context) {
		super(context);
	}

	private boolean _loaded = false;
    private static HelpCenter _instance = null;

    public boolean dataLoaded()
    {
    	return _loaded;
    }

    public void Show(int topicID)
    {
        // Load content if not loaded.
        if (dataLoaded())
        {			
			Intent intent = new Intent(LotterySpirit.getInstance(), HelpActivity.class);
			Bundle bundle = new Bundle();

			bundle.putInt("topic", topicID);
			intent.putExtras(bundle);
			
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            
            LotterySpirit.getInstance().startActivity(intent);
        }
    }

    public static HelpCenter Instance()
    {
        if (_instance == null)
        {
            _instance = new HelpCenter(LotterySpirit.getInstance());
        }

        return _instance;
    }

    public boolean Load()
    {
        if (_loaded)
            return false;
        
        SyncContext syncCt = LBDataManager.GetInstance().GetSyncContext();
        if (syncCt.NeedToResync(UpdateOptionEnum.NeedUpdateHelpContent) || _isDBJustCreated)
        {
        	// read from cloud.
            DBXmlDocument xml = DataUtil.syncHelpToCloud();
            if (xml != null)
            {
                try
                {
                    DBXmlNode root = xml.Root();
                    readFromXml(root);

                    // save to db.
                    save();
                }
                catch (Exception e)
                {
                	return false;
                }                
            }	
            else
            	return false;
        }
        
        _loaded = true;
        
        return true;
    }
    
    private void readFromXml(DBXmlNode node)
    {
        DBXmlNode topicsNode = node.FirstChildNode("Topics");
        for (DBXmlNode tpNode : topicsNode.ChildNodes())
        {
        	HelpTopic topic = new HelpTopic();
            topic.ReadFromXml(tpNode);
            _topics.put(topic.getID(), topic);
        }

        DBXmlNode notesNode = node.FirstChildNode("Notes");
        for (DBXmlNode ndNode : notesNode.ChildNodes())
        {
        	HelpNote note = new HelpNote();
            note.ReadFromXml(ndNode);
            _notes.put(note.getID(), note);
        }
    }
    
    private void save()
    {
		SQLiteDatabase db = this.getWritableDatabase();
		if (db.isOpen())
		{		
	    	// clear the old record.
			onUpgrade(db, DATABASE_VERSION, DATABASE_VERSION);
			
	        for (int i = 0; i < _topics.size(); ++ i)        	
	        {
	        	HelpTopic tp = _topics.valueAt(i);
	        	
				ContentValues values = new ContentValues();
				values.put("ID", tp.getID());
				values.put("Title", tp.getTitle());
				values.put("Description", tp.getDescription());
				values.put("Notes", tp.getNotes());
				
				db.insert(TOPIC_TABLE, null, values);
	        }

	        for (int i = 0; i < _notes.size(); ++ i)
	        {
	        	HelpNote nd = _notes.valueAt(i);
	        	
				ContentValues values = new ContentValues();
				values.put("ID", nd.getID());
				values.put("Content", nd.getContent());
				
				db.insert(NOTE_TABLE, null, values);
	        }	
		}
		
		db.close();
    }
}
