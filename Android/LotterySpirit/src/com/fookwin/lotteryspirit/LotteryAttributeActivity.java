package com.fookwin.lotteryspirit;

import com.fookwin.lotteryspirit.view.ConstraintListView;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.app.ActionBar.OnNavigationListener;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.ContextThemeWrapper;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ExpandableListView;
import android.widget.ListView;
import android.widget.SpinnerAdapter;

public class LotteryAttributeActivity extends Activity 
{
	private ContextThemeWrapper theme_context;
	private LayoutInflater layout_inflater;
	private SpinnerAdapter categories_adapter;
	
	private ListView recommended_filter_list;
	private ExpandableListView full_filter_list;
	private ConstraintListView constraintListView;
	private MenuItem filter_option_menu;
	
	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_lottery_attribute);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		layout_inflater = this.getLayoutInflater();	
		
		theme_context = new ContextThemeWrapper(this, android.R.style.Theme_Holo);
		
		categories_adapter = ArrayAdapter.createFromResource( theme_context,  
				R.array.filter_tabs, android.R.layout.simple_spinner_dropdown_item);
		
		updateActionBar();
		
        // initialize the tab content.
        recommended_filter_list = (ListView)findViewById(R.id.recommend_filter_list); 
        full_filter_list = (ExpandableListView)findViewById(R.id.filter_category_list);
        
        constraintListView = new ConstraintListView();
        constraintListView.setOnItemClickedHandler(new Handler()
        {
    		@Override
    		public void handleMessage(Message msg) {
    			super.handleMessage(msg);
    			
    			Bundle data = msg.getData();
    			String attributeKey = data.getString("key");
    			if (attributeKey != null)
    			{
					Intent intent = new Intent(LotteryAttributeActivity.this, AttributeDetailActivity.class);
					Bundle bundle = new Bundle();

					bundle.putString("key", attributeKey);
					intent.putExtras(bundle);
					
                    intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    
                    startActivity(intent);
    			}
    		}
        });
	}	
	
	@Override
	public void onResume() 
	{
		super.onResume();
		
		constraintListView.refreshRecommendedList();
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) 
	{
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.attribute_filter_option, menu);
		
		filter_option_menu = menu.findItem(R.id.edit_filter_option);

		return true;
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item)
	{
		switch (item.getItemId())
		{
		case R.id.edit_filter_option:
		{
			// goto the filter option setting.
			Intent intent = new Intent(this, AttributeFilterOptionActivity.class);			
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);           
            startActivity(intent);
            
			break;
		}
        case android.R.id.home:
            finish();
            return true;
		default:
			return false;
		}
		return false;
	}
	
	public void updateActionBar()
	{
		ActionBar actionBar = this.getActionBar();  
		actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_LIST);
		actionBar.setTitle("");
		
		actionBar.setListNavigationCallbacks( categories_adapter, new OnNavigationListener()
		{
			@Override
			public boolean onNavigationItemSelected(int position, long itemId) 
			{
				if (position == 0)
				{
					recommended_filter_list.setVisibility(View.VISIBLE);
					full_filter_list.setVisibility(View.INVISIBLE);
					filter_option_menu.setVisible(true);
					
					constraintListView.initRecommendedFilterList(layout_inflater, recommended_filter_list);
				}
				else 
				{
					recommended_filter_list.setVisibility(View.INVISIBLE);
					full_filter_list.setVisibility(View.VISIBLE);
					filter_option_menu.setVisible(false);
					
					constraintListView.initFullFilterList(layout_inflater, full_filter_list);
				}
				
				return true;
			}		
		});
	}
}
