package com.fookwin.lotteryspirit;

import java.util.ArrayList;

import com.fookwin.lotterydata.data.HelpNote;
import com.fookwin.lotterydata.data.HelpTopic;
import com.fookwin.lotteryspirit.data.HelpCenter;

import android.app.ActionBar;
import android.app.Activity;
import android.os.Bundle;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;

public class HelpActivity extends Activity {

	private TextView topic_text;
	private TextView topic_description;
	private ListView topic_notes;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_help);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		topic_text = (TextView) findViewById(R.id.titleText);
		topic_description = (TextView) findViewById(R.id.descriptionText);
		topic_notes = (ListView) findViewById(R.id.noteLists);
		
		// read data from bundle.
		Bundle bundle = this.getIntent().getExtras();    
		int topicID = bundle.getInt("topic"); 
		HelpTopic tp = HelpCenter.Instance().getTopic(topicID);
		if (tp != null)
		{		
			topic_text.setText(tp.getTitle());
			topic_description.setText(tp.getDescription());
			
			// get notes
			ArrayList<String> notes = new ArrayList<String>();
			ArrayList<HelpNote> noteList = HelpCenter.Instance().getTopicNotes(tp);
			for (HelpNote note : noteList)
			{
				notes.add("* " + note.getContent());
			}
	        
	        ArrayAdapter<String> adapter = new ArrayAdapter<String>(this,R.layout.list_item_helpnote,notes);  
	        topic_notes.setAdapter(adapter);
		}
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	            finish();
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
}
