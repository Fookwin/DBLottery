package com.fookwin.lotteryspirit;

import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotterydata.data.SchemeSelector;
import com.fookwin.lotterydata.data.SchemeSelectorTypeEnum;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.view.SelectionListView;
import com.fookwin.lotteryspirit.view.SelectionListView.SelectionTypeEnum;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.app.AlertDialog.Builder;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.KeyEvent;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

public class SelectorsActivity extends Activity {

	private Purchase _editingPurchase;
	private SelectionListView selector_list_helper;
	
	private Button add_standard;
	private Button add_dantou;
	private Button add_random;
	private ListView selector_list;
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
			_editingPurchase.getSelectors().remove(position);
			_editingPurchase.markSelectorsRecomputeRequired();
			
			updateLists();
			updateStatusPanel();
		}
	};

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_selectors);

		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		_editingPurchase = LBDataManager.GetInstance().getPendingPurchase();
		
        // initialize the tab content.
		add_standard = (Button) findViewById(R.id.add_standard);
		add_dantou = (Button) findViewById(R.id.add_dantou);
		add_random = (Button) findViewById(R.id.add_random);
		
		add_standard.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				startSelector(SchemeSelectorTypeEnum.StandardSelectorType, -1);
			}			
		});
		
		add_dantou.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				startSelector(SchemeSelectorTypeEnum.DantuoSelectorType, -1);
			}			
		});
		
		add_random.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				startSelector(SchemeSelectorTypeEnum.RandomSelectorType, -1);
			}			
		});
		
		Button go_result_btn = (Button) findViewById(R.id.go_result_btn);
		go_result_btn.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				Intent intent = new Intent(SelectorsActivity.this, SchemesActivity.class);
				startActivity(intent);
			}	
		});
		
		selector_list = (ListView) findViewById(R.id.selector_list);
		selector_list.setOnItemClickListener(new OnItemClickListener()
		{
			@Override
			public void onItemClick(AdapterView<?> arg0, View arg1, int position,
					long arg3) 
			{
				SchemeSelector selector = _editingPurchase.getSelectors().get(position);
				if (selector != null)
					startSelector(selector.GetSelectorType(), position);				
			}			
		});
		
		status_text = (TextView) findViewById(R.id.status_text);
		substatus_text = (TextView) findViewById(R.id.substatus_text);
		
		updateLists();
		updateStatusPanel();
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	        	exit();
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if (keyCode == KeyEvent.KEYCODE_BACK) {
			exit();
		}
		return super.onKeyDown(keyCode, event);
	}
	
	private void startSelector(SchemeSelectorTypeEnum mode, int selectorIndex)
	{
		Intent intent = new Intent(this, SelectorEditorActivity.class);
		Bundle bundle = new Bundle();

		bundle.putInt("mode", mode.getValue());
		bundle.putInt("selector", selectorIndex);
		intent.putExtras(bundle);
        
		startActivityForResult(intent, 1);	
	}
	
	
	@Override
	protected void onResume() {
		super.onResume();
		
		updateStatusPanel();
	}

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) 
	{
		super.onActivityResult(requestCode, resultCode, data);
		
		if (requestCode == 1) // selectors changed.
		{
			updateLists();
		}
	}

	private void updateLists()
	{
		if (selector_list_helper != null)
		{			
			selector_list_helper.refresh();
		}
		else
		{
			selector_list_helper = new SelectionListView(SelectionTypeEnum.SELECTOR);
			selector_list_helper.buildList(this, selector_list, _editingPurchase.getSelectors(), true);
			selector_list_helper.setOnItemDeletedHandler(onListItemDeletedHandler);			
		}
	}

	private void exit() {
		Builder builder = new AlertDialog.Builder(SelectorsActivity.this);
		builder.setTitle("退出选号编辑");
		builder.setMessage("返回后将清空当前选号，确认要退出选号吗？");
		builder.setPositiveButton("确定", new DialogInterface.OnClickListener() {
			public void onClick(DialogInterface dialog, int which) 
			{				
				// make a new pending purchase and go to home page.
				LBDataManager.GetInstance().resetPendingPurchase();
				Intent intent = new Intent(SelectorsActivity.this, MainActivity.class);
				intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
				startActivity(intent);
			}
		});
		builder.setNegativeButton("不", null);
		builder.create().show();
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
				updateStatus();
				
				dialog.dismiss();
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
