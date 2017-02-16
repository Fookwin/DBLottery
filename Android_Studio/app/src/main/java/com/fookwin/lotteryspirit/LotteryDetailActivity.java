package com.fookwin.lotteryspirit;

import java.text.ParseException;

import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.data.LotteryInfo;
import com.fookwin.lotteryspirit.view.LotteryDetailView;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.app.ProgressDialog;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.LinearLayout;

public class LotteryDetailActivity extends Activity
{
	private LinearLayout detail_container;
	
	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_lottery_detail);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		getActionBar().setTitle("开奖详情");
		
		detail_container = (LinearLayout) findViewById(R.id.detail_container);
		
		// read data from bundle.
		Bundle bundle = this.getIntent().getExtras();    
		int index = bundle.getInt("lottery"); 
		final Lottery lot = LBDataManager.GetInstance().getHistory().getByIndex(index);
		if (lot != null)
		{	
	        final ProgressDialog dialog = new ProgressDialog(this);
	        dialog.setMessage("加载历史详情...");
	        dialog.setIndeterminate(true);
	        dialog.setCancelable(false);
	        dialog.show();
	        
			final Handler handler = new Handler()
			{
				@Override
				public void handleMessage(Message msg) {
					super.handleMessage(msg);
					
					LotteryDetailView viewHelper = new LotteryDetailView();
					try {
						viewHelper.setData(LotteryInfo.Create(lot));
					} catch (ParseException e) {
						e.printStackTrace();
					}
					viewHelper.addToContainer(detail_container);
					
					dialog.dismiss();
				}
			};
			
			new Thread(new Runnable()
			{
				public void run() 
				{	
					try {
						lot.initDetails();
					} catch (ParseException e) {
						e.printStackTrace();
					}
					handler.sendEmptyMessage(-1);
				}
			}).start();
		}
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		//getMenuInflater().inflate(R.menu.lottery_detail, menu);
		return true;
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
