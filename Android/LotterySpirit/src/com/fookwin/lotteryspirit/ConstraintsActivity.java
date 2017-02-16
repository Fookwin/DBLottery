package com.fookwin.lotteryspirit;

import com.fookwin.lotterydata.data.Constraint;
import com.fookwin.lotterydata.data.ConstraintTypeEnum;
import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.view.SelectionListView;
import com.fookwin.lotteryspirit.view.SelectionListView.SelectionTypeEnum;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.AdapterView.OnItemClickListener;

public class ConstraintsActivity extends Activity {

	private Purchase _editingPurchase;	
	private ListView constraint_list;
	private Button add_attribute_filter;
	private Button add_numset_filter;
	private Button add_history_filter;
	private SelectionListView constraint_list_helper;
	
	@SuppressLint("HandlerLeak")
	Handler onListItemDeletedHandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) 
		{
			super.handleMessage(msg);
			
			int position = msg.what;
			
			_editingPurchase.getConstraints().remove(position);
			_editingPurchase.markConstraintRecomputeRequired();
				
			// for any constraint change has to recompute the selection
			_editingPurchase.markSelectorsRecomputeRequired(); 
			
			updateLists();
		}
	};
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_constraints);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
	
		_editingPurchase = LBDataManager.GetInstance().getPendingPurchase();
		
        // initialize the tab content.
		add_attribute_filter = (Button) findViewById(R.id.add_attribute_filter);
		add_numset_filter = (Button) findViewById(R.id.add_numset_filter);
		add_history_filter = (Button) findViewById(R.id.add_history_filter);
		
		constraint_list = (ListView) findViewById(R.id.filter_list);
		constraint_list.setOnItemClickListener(new OnItemClickListener()
		{
			@Override
			public void onItemClick(AdapterView<?> arg0, View arg1, int position,
					long arg3) 
			{
				Constraint constraint = _editingPurchase.getConstraints().get(position);
				if (constraint != null)	
					startConstraint(constraint.GetConstraintType(), position);
			}			
		});
		
		setButtonClickHandlers();
		updateLists();
	}

	private void setButtonClickHandlers()
	{
		add_attribute_filter.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				startConstraint(ConstraintTypeEnum.SchemeAttributeConstraintType, -1);
			}			
		});
		
		add_numset_filter.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				startConstraint(ConstraintTypeEnum.RedNumSetConstraintType, -1);
			}			
		});
		
		add_history_filter.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				startConstraint(ConstraintTypeEnum.HistoryDuplicateConstraintType, -1);
			}			
		});
	}
	
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	            finish();
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
	
	private void startConstraint(ConstraintTypeEnum mode, int position)
	{
		Intent intent = new Intent(this, ConstraintEditorActivity.class);
		Bundle bundle = new Bundle();

		bundle.putInt("mode", mode.getValue());
		bundle.putInt("constraint", position);
		intent.putExtras(bundle);
        
		intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		startActivity(intent);
	}
	
	@Override
	public void onResume() 
	{
		super.onResume();
		updateLists();
	}

	private void updateLists()
	{			
		// update the list.
		if (constraint_list_helper != null)
		{
			constraint_list_helper.refresh();
		}
		else
		{
			constraint_list_helper = new SelectionListView(SelectionTypeEnum.CONSTRAINT);
			constraint_list_helper.buildList(this, constraint_list, _editingPurchase.getConstraints(), true);
			constraint_list_helper.setOnItemDeletedHandler(onListItemDeletedHandler);				
		}
	}
}
