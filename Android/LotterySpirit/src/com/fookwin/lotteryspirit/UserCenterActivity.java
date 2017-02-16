package com.fookwin.lotteryspirit;

import com.fookwin.lotteryspirit.view.PurchasesView;

import android.app.ActionBar;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.widget.LinearLayout;

public class UserCenterActivity extends Activity 
{
	private PurchasesView viewHelper;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_user_center);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		viewHelper = new PurchasesView();
		viewHelper.addToContainer(this,(LinearLayout) findViewById(R.id.container_layout), 
				getLayoutInflater());
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
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) 
	{
		super.onActivityResult(requestCode, resultCode, data);
		
		if (requestCode == 1)
		{
			viewHelper.refresh();
		}
	}
}
