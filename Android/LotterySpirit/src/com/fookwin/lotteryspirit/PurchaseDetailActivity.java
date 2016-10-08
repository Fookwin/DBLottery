package com.fookwin.lotteryspirit;

import com.fookwin.lotterydata.data.History;
import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.data.PurchaseInfo;
import com.fookwin.lotteryspirit.view.VerifiedItemListView;
import com.fookwin.lotteryspirit.view.VerifiedItemListView.ItemTypeEnum;

import android.app.ActionBar;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.AlertDialog.Builder;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

public class PurchaseDetailActivity extends Activity 
{	
	protected ListView scheme_list;
	private View selector_frame;
	private View fillter_frame;
	private View summary_frame;
	private View scheme_frame;
	private boolean bHasFilters;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_purchase_detail);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		summary_frame = findViewById(R.id.summary_frame);
		selector_frame = findViewById(R.id.selector_frame);
		fillter_frame = findViewById(R.id.fillter_frame);
		scheme_frame = findViewById(R.id.scheme_frame);
		
		// read data from bundle.
		Bundle bundle = this.getIntent().getExtras();    
		final int id = bundle.getInt("purchase"); 
		PurchaseInfo info = LBDataManager.GetInstance().getPurchaseManager().getPurchaseById(id);
		if (info != null)
		{
			final Purchase source = info.getSource();
			History history = LBDataManager.GetInstance().getHistory();
			final Lottery targetLot = history.getByIssue(info.Issue);
			final int targetLotIndex = history.issueToIndex(info.Issue);
			
			// initial summary
			TextView issue_text = (TextView)findViewById(R.id.issue_text);
			issue_text.setText(String.valueOf(info.Issue));
			
			TextView result_text = (TextView)findViewById(R.id.result_text);
			result_text.setText(targetLot != null ? targetLot.getScheme().toString() : "等待开奖");
			
			TextView cost_text = (TextView)findViewById(R.id.cost_text);
			cost_text.setText(String.valueOf(info.Buy * 2) + "元");
			
			TextView earning_text = (TextView)findViewById(R.id.earning_text);
			earning_text.setText(info.Win < 0 ? (info.Win == -1 ? "计算奖金" : "等待开奖") : (info.Win == 0 ? "未中奖" : String.valueOf(info.Win) + "元"));

			// selectors
			ListView selector_list = (ListView) findViewById(R.id.selector_list);
			VerifiedItemListView selectorListHelper = new VerifiedItemListView(ItemTypeEnum.SELECTOR);
			selectorListHelper.buildList(this, selector_list, source.getSelectors(), targetLot, targetLotIndex);
			
			TextView showhide_selector_btn = (TextView) findViewById(R.id.showhide_selector_btn);
			showhide_selector_btn.setOnClickListener(new OnClickListener()
			{
				private boolean _showAll = false;

				@Override
				public void onClick(View arg0) {

					TextView btn = (TextView) arg0;
					if (!_showAll )
					{						
						summary_frame.setVisibility(View.GONE);
						fillter_frame.setVisibility(View.GONE);
						scheme_frame.setVisibility(View.GONE);
						btn.setText("v 收起");
					}
					else
					{
						summary_frame.setVisibility(View.VISIBLE);
						scheme_frame.setVisibility(View.VISIBLE);
						
						if (bHasFilters)
							fillter_frame.setVisibility(View.VISIBLE);
						
						btn.setText("^ 展开");
					}
					
					_showAll = !_showAll;
				}
			});
			
			// filters
			bHasFilters = false;
			if(source.getConstraints().size() > 0)
			{
				bHasFilters = true;
				ListView filter_list = (ListView) findViewById(R.id.filter_list);
				VerifiedItemListView filterListHelper = new VerifiedItemListView(ItemTypeEnum.CONSTRAINT);
				filterListHelper.buildList(this, filter_list, source.getConstraints(), targetLot, targetLotIndex);
				
				TextView showhide_constraint_btn = (TextView) findViewById(R.id.showhide_constraint_btn);
				showhide_constraint_btn.setOnClickListener(new OnClickListener()
				{
					private boolean _showAll = false;

					@Override
					public void onClick(View arg0) {

						TextView btn = (TextView) arg0;
						if (!_showAll )
						{						
							summary_frame.setVisibility(View.GONE);
							selector_frame.setVisibility(View.GONE);
							scheme_frame.setVisibility(View.GONE);
							btn.setText("v 收起");
						}
						else
						{
							summary_frame.setVisibility(View.VISIBLE);
							selector_frame.setVisibility(View.VISIBLE);
							scheme_frame.setVisibility(View.VISIBLE);
							btn.setText("^ 展开");
						}
						
						_showAll = !_showAll;
					}
				});
			}
			else
			{
				fillter_frame.setVisibility(View.GONE);
				bHasFilters = false;
			}
			
			// schemes
			scheme_list = (ListView) findViewById(R.id.scheme_list);
			VerifiedItemListView schemeListHelper = new VerifiedItemListView(ItemTypeEnum.SCHEME);
			schemeListHelper.buildList(PurchaseDetailActivity.this, scheme_list, 
					source.getSelection(), targetLot, targetLotIndex);
			
			TextView showhide_detail_btn = (TextView) findViewById(R.id.showhide_detail_btn);
			showhide_detail_btn.setOnClickListener(new OnClickListener()
			{
				private boolean _showAll = false;

				@Override
				public void onClick(View arg0) {

					TextView btn = (TextView) arg0;
					if (!_showAll)
					{						
						summary_frame.setVisibility(View.GONE);
						selector_frame.setVisibility(View.GONE);
						fillter_frame.setVisibility(View.GONE);
						btn.setText("v 收起");
					}
					else
					{
						summary_frame.setVisibility(View.VISIBLE);
						selector_frame.setVisibility(View.VISIBLE);
						if (bHasFilters)
							fillter_frame.setVisibility(View.VISIBLE);
						
						btn.setText("^ 展开");
					}
					
					_showAll = !_showAll;
				}
			});
			
			// reuse solution
			View delete_btn = (View) findViewById(R.id.delete_btn);
			delete_btn.setOnClickListener(new OnClickListener()
			{
				@Override
				public void onClick(View v) {
					
					Builder builder = new AlertDialog.Builder(PurchaseDetailActivity.this);
					builder.setTitle("删除方案");
					builder.setMessage("确认要删除当前选号方案吗？");
					builder.setPositiveButton("确定", new DialogInterface.OnClickListener() {
						public void onClick(DialogInterface dialog, int which) {
							LBDataManager.GetInstance().getPurchaseManager().remove(id);
							finish();
						}
					});
					builder.setNegativeButton("不", null);
					builder.create().show();
				}
			});
			
			// reuse solution
			Button reuse_solution_btn = (Button) findViewById(R.id.reuse_solution_btn);
			reuse_solution_btn.setOnClickListener(new OnClickListener()
			{
				@Override
				public void onClick(View v) {
					
					// reset the pending purchase and navigate to selection page.
					LBDataManager.GetInstance().resetPendingPurchase(source, false);
					
					Intent intent = new Intent(PurchaseDetailActivity.this, SelectorsActivity.class);
					startActivity(intent);
				}
			});
			
			// reuse selection
			Button reuse_selection_btn = (Button) findViewById(R.id.reuse_selection_btn);
			reuse_selection_btn.setOnClickListener(new OnClickListener()
			{
				@Override
				public void onClick(View v) {
					// reset the pending purchase and navigate to selection page.
					LBDataManager.GetInstance().resetPendingPurchase(source, true);
					
					Intent intent = new Intent(PurchaseDetailActivity.this, SelectorsActivity.class);
					startActivity(intent);
				}
			});
			
			if (info.Issue == LBDataManager.GetInstance().getReleaseInfo().getNextIssue())
			{
				// edit selection
				View edit_btn = findViewById(R.id.edit_btn);
				edit_btn.setOnClickListener(new OnClickListener()
				{
					@Override
					public void onClick(View v) {
						// reset the pending purchase and navigate to selection page.
						LBDataManager.GetInstance().resetPendingPurchase(source);
						
						Intent intent = new Intent(PurchaseDetailActivity.this, SelectorsActivity.class);
						startActivity(intent);
					}
				});
				edit_btn.setVisibility(View.VISIBLE);
			}
		}
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) 
	{
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.purchase_detail_option, menu);

		return true;
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	        {
	            finish();
	            return true;
	        }
			case R.id.goto_donation_option:
			{
				// goto the filter option setting.
				Intent intent = new Intent(this, DonationActivity.class);
	            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
	            startActivity(intent);
	            
	            return true;
			}
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
}
