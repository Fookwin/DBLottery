package com.fookwin.lotteryspirit.fragment;

import com.fookwin.lotteryspirit.LotteryAttributeActivity;
import com.fookwin.lotteryspirit.LotteryHistoryActivity;
import com.fookwin.lotteryspirit.LotteryTrendChartActivity;
import com.fookwin.lotteryspirit.R;

import android.os.Bundle;
import android.app.ActionBar;
import android.app.Fragment;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.LinearLayout;

public class StatisticsFragment extends Fragment
{
	@Override  
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) 
	{
		// update action bar
		updateActionBar();
		
		View contentLayout = inflater.inflate(R.layout.fragment_statistics, container, false);

		LinearLayout gotoLotteryHistory = (LinearLayout)contentLayout.findViewById(R.id.gotoLotteryHistory);
		gotoLotteryHistory.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				Intent intent = new Intent(getActivity(), LotteryHistoryActivity.class);
				startActivity(intent);
			}
		});

		LinearLayout gotoTrendChart = (LinearLayout)contentLayout.findViewById(R.id.gotoTrendChart);
		gotoTrendChart.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				Intent intent = new Intent(getActivity(), LotteryTrendChartActivity.class);
				startActivity(intent);
			}
		});
		
		LinearLayout gotoributeAnalysis = (LinearLayout)contentLayout.findViewById(R.id.gotoributeAnalysis);
		gotoributeAnalysis.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				Intent intent = new Intent(getActivity(), LotteryAttributeActivity.class);
				startActivity(intent);
			}
		});
		
		return contentLayout;
	}
	
	public void updateActionBar()
	{
		ActionBar actionBar = getActivity().getActionBar();  
		actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);
		actionBar.setTitle("Êý¾Ý·ÖÎö");
		actionBar.setIcon(R.drawable.icon_analysis_grey);
	}
}
