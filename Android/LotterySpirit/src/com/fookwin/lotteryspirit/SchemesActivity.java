package com.fookwin.lotteryspirit;

import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.view.SelectionListView;
import com.fookwin.lotteryspirit.view.SelectionListView.SelectionTypeEnum;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

public class SchemesActivity extends Activity {
	
	private Purchase _editingPurchase;

	private ListView result_list;
	private SelectionListView result_list_helper;
	private Button resume_selection;	
	private TextView status_text;
	private TextView substatus_text;
	
	@SuppressLint("HandlerLeak")
	Handler onListItemDeletedHandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) 
		{
			super.handleMessage(msg);
			
			int position = msg.what;
			//_editingPurchase.getSelection().remove(position);
			_editingPurchase.removeScheme(position);
			
			updateList();			
			updateStatusPanel();
			
			// enable the resume command for any change.
			resume_selection.setVisibility(View.VISIBLE);
		}
	};

	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_schemes);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);

		_editingPurchase = LBDataManager.GetInstance().getPendingPurchase();
		
		// initialize the tab content.	
		result_list = (ListView) findViewById(R.id.selection_result_list);
		
		resume_selection = (Button) findViewById(R.id.resume_selection);
		resume_selection.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				_editingPurchase.resumeSelection();
				
				updateList();
				updateStatusPanel();
				
				resume_selection.setVisibility(View.GONE);
			}			
		});
		
		Button go_constraints_btn = (Button) findViewById(R.id.go_constraints_btn);
		go_constraints_btn.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				Intent intent = new Intent(SchemesActivity.this, ConstraintsActivity.class);		        
				startActivityForResult(intent, 1);
			}	
		});
		
		Button export_btn = (Button) findViewById(R.id.export_btn);
		export_btn.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				Intent intent = new Intent(SchemesActivity.this, ExportSelectionActivity.class);
				startActivity(intent);
			}	
		});
		
		status_text = (TextView) findViewById(R.id.status_text);
		substatus_text = (TextView) findViewById(R.id.substatus_text);
		
		refresh();
	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		super.onActivityResult(requestCode, resultCode, data);
		
		if (requestCode == 1)
		{
			refresh();
		}
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
	
	public void refresh()
	{
		updateList();
		updateStatusPanel();
		
		resume_selection.setVisibility(_editingPurchase.getRemovedCount() > 0 ? View.VISIBLE : View.GONE);
	}
	
	private void updateList()
	{
		// update the result.
		if (result_list_helper != null)
		{
			result_list_helper.refresh();
		}
		else
		{
			result_list_helper = new SelectionListView(SelectionTypeEnum.SCHEME);
			result_list_helper.buildList(this, result_list, _editingPurchase.getSelection(), false);
			result_list_helper.setOnItemDeletedHandler(onListItemDeletedHandler);
		}
	}
	
	@SuppressLint("HandlerLeak")
	public void updateStatusPanel()
	{	
		if (!_editingPurchase.requireCompute())
		{
			updateStatus();
			return;
		}
		
        final ProgressDialog dialog = new ProgressDialog(this);
        dialog.setMessage("加载走势图数据...");
        dialog.setIndeterminate(true);
        dialog.setCancelable(false);
        dialog.show();
		
		final Handler onComputeCompleteHandler = new Handler()
		{
			@Override
			public void handleMessage(Message msg) 
			{
				super.handleMessage(msg);
				
				dialog.dismiss();
				updateStatus();
			}
		};
		
		new Thread(new Runnable()
		{
			public void run() 
			{
				int count = _editingPurchase.getSelection().size();				
				onComputeCompleteHandler.sendEmptyMessage(count);
			}
		}).start();			
	}
	
	private void updateStatus()
	{
		int count = _editingPurchase.getSelection().size();
		int filteredCount = _editingPurchase.getFilteredCount();
		int removedCount = _editingPurchase.getRemovedCount();
		
		String status = Integer.toString(count) + " 注  ";
		String subStatus = "过滤 " + Integer.toString(filteredCount) + " 注 ";
		subStatus += "删除 " + Integer.toString(removedCount) + " 注";
		
		status_text.setText(status);			
		substatus_text.setText(subStatus);
	}
}
