package com.fookwin.lotteryspirit;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.AlertDialog.Builder;
import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.res.Resources;
import android.net.Uri;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;

import com.fookwin.lotterydata.util.DataUtil;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.data.LBDataManager.SoftwareVersionUpdate;
import com.fookwin.lotteryspirit.fragment.HomeFragment;
import com.fookwin.lotteryspirit.fragment.SelectionFragment;
import com.fookwin.lotteryspirit.fragment.StatisticsFragment;
import com.fookwin.lotteryspirit.util.NotificationUtil;
import com.fookwin.lotteryspirit.view.NavigateButton;

public class MainActivity extends Activity 
{
	private FragmentManager fragmentManager;
	private HomeFragment home_fragment;
	private StatisticsFragment statistics_fragment;
		
	private NavigateButton gohome_btn;
	private NavigateButton gostatistics_btn;
	private NavigateButton goselection_btn;	
	


	// flag to indicate the view updating status: 
	// 0 - not initialized
	// -1 - being initialized
	// 1 - initialized
	private int softwareChecked = 0;
	private SelectionFragment selection_fragment;
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if (keyCode == KeyEvent.KEYCODE_BACK) {
			showMessgeDialog();
		}
		return super.onKeyDown(keyCode, event);
	}

	protected void onCreate(Bundle savedInstanceState) 
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);	
		
		fragmentManager = getFragmentManager();
		
		// Foot...
		initTabView();
		setSelectedTab(0);

		refreshViews();
	}
	
	private void refreshViews()
	{		
		if (softwareChecked == 0)
		{
			final SoftwareVersionUpdate softVer = LBDataManager.GetInstance().getSoftwareVersion();
			if (softVer != null)
			{
				softwareChecked = -1;
				
				if (softVer.currentVersion < softVer.latestVersion)
				{
					NotificationUtil.ShowDialog(MainActivity.this, "检查到新版本", softVer.releaseNotes, 
							"立即更新", "下次再说", !softVer.schemeChanged,
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog, int which) 
								{
									Intent browse = new Intent( Intent.ACTION_VIEW, Uri.parse( DataUtil.getLatestApk(softVer.latestVersion) ) );
								    startActivity( browse );
								}
							}, null);
				}
	
				softwareChecked = 1;
			}
		}
	}
	
	public void initTabView()
	{
		Resources res = getResources();
		
		// get the navigate buttons.
		gohome_btn = (NavigateButton) findViewById(R.id.gohome_btn);
		gostatistics_btn = (NavigateButton) findViewById(R.id.gostatistics_btn);
		goselection_btn = (NavigateButton) findViewById(R.id.gopurchase_btn);
		
		gohome_btn.setText("最新");
		gohome_btn.setImage(R.drawable.icon_news_red, R.drawable.icon_news_grey);
		gohome_btn.setTextColor(res.getColor(R.color.darkred), res.getColor(R.color.grey));
		
		gostatistics_btn.setText("分析");
		gostatistics_btn.setImage(R.drawable.icon_analysis_red, R.drawable.icon_analysis_grey);
		gostatistics_btn.setTextColor(res.getColor(R.color.darkred), res.getColor(R.color.grey));
		
		goselection_btn.setText("选号");
		goselection_btn.setImage(R.drawable.icon_selection_red, R.drawable.icon_selection_grey);
		goselection_btn.setTextColor(res.getColor(R.color.darkred), res.getColor(R.color.grey));
		
		// Add button events.
		gohome_btn.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				if (!gohome_btn.isSelected())
				{
					setSelectedTab(0);
				}
			}
		});
		
		gostatistics_btn.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				if (!gostatistics_btn.isSelected())
				{
					setSelectedTab(3);
				}
			}
		});
		
		goselection_btn.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				if (!goselection_btn.isSelected())
				{
					setSelectedTab(4);
				}
			}
		});
	}
	
	private void setSelectedTab(int index)
	{
		// hide all fragments first.
		FragmentTransaction transaction = fragmentManager.beginTransaction(); 		

		// reset all buttons.
		gohome_btn.setSelected(false);
		gostatistics_btn.setSelected(false);
		goselection_btn.setSelected(false);

		hideFragments(transaction);
		
		// select the corresponding button and show the content.
		switch (index)
		{
		case 0:	
		{
			gohome_btn.setSelected(true);
			
			if (home_fragment == null)				
			{
				home_fragment = new HomeFragment();
				transaction.add(R.id.content_view, home_fragment);  
			}
			else
			{
				transaction.show(home_fragment);
				
				// refresh the action bar to make it for the active fragment.
				home_fragment.updateActionBar();
			}
			
			break;
		}
		case 3:
		{
			gostatistics_btn.setSelected(true);
			
			if (statistics_fragment == null)				
			{
				statistics_fragment = new StatisticsFragment();
				transaction.add(R.id.content_view, statistics_fragment);  
			}
			else
			{
				transaction.show(statistics_fragment); 
				
				// refresh the action bar to make it for the active fragment.
				statistics_fragment.updateActionBar();
			}
			
			break;
		}
		case 4:		
		{
			goselection_btn.setSelected(true);
			
			if (selection_fragment == null)				
			{
				selection_fragment = new SelectionFragment();
				transaction.add(R.id.content_view, selection_fragment);  
			}
			else
			{
				transaction.show(selection_fragment); 
				
				// refresh the action bar to make it for the active fragment.
				selection_fragment.updateActionBar();
			}
		
			break;
		}
		}
		
		transaction.commit();
	}
	
	private void hideFragments(FragmentTransaction transaction) 
	{
		if (home_fragment != null) 
		{
			transaction.hide(home_fragment);
		}
		
		if (statistics_fragment != null)
		{
			transaction.hide(statistics_fragment);
		}
		
		if (selection_fragment != null)
		{
			transaction.hide(selection_fragment);
		}
	}
	
	private void showMessgeDialog() {
		Builder builder = new AlertDialog.Builder(MainActivity.this);
		builder.setTitle("退出");
		builder.setMessage("您确认要退出福盈双色球吗？");
		builder.setPositiveButton("确定", new DialogInterface.OnClickListener() {
			public void onClick(DialogInterface dialog, int which) {
				System.exit(0);
			}
		});
		builder.setNegativeButton("不", null);
		builder.create().show();
	}
}
