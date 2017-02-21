package com.fookwin.lotteryspirit.fragment;

import android.app.ActionBar;
import android.app.Activity;
import android.app.Fragment;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.fookwin.lotterydata.data.ReleaseInfo;
import com.fookwin.lotterydata.data.SchemeAttributeValueStatus;
import com.fookwin.lotteryspirit.AboutActivity;
import com.fookwin.lotteryspirit.AttributeFilterOptionActivity;
import com.fookwin.lotteryspirit.DonationActivity;
import com.fookwin.lotteryspirit.FeedbackActivity;
import com.fookwin.lotteryspirit.LotteryAttributeActivity;
import com.fookwin.lotteryspirit.LotteryHistoryActivity;
import com.fookwin.lotteryspirit.LotteryTrendChartActivity;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.UserCenterActivity;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.data.LotteryInfo;
import com.fookwin.lotteryspirit.data.LotteryStateInfo;
import com.fookwin.lotteryspirit.data.PurchaseInfo;
import com.fookwin.lotteryspirit.view.LotteryDetailView;
import com.fookwin.lotteryspirit.view.LotteryNumStatusView;
import com.fookwin.lotteryspirit.view.NativeADView;
import com.fookwin.lotteryspirit.view.PurchasesView;
import com.fookwin.lotteryspirit.view.RecommendAttributesView;
import com.fookwin.lotteryspirit.view.ReleaseTimeDownView;

import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

public class HomeFragment extends Fragment
{		
	private LinearLayout header_view_container;
	private LinearLayout content_item_latestlot;
	private LinearLayout content_item_lateststate;
	private LinearLayout content_item_latestfilter;
	
	private ReleaseTimeDownView headerView;
	private LotteryDetailView lottoDetailView;
	private LotteryNumStatusView lottoStatusView;
	private RecommendAttributesView lottoFilterView;
	private LinearLayout content_item_verifiedpurchases;
	private PurchasesView verfiedPurchaseView;
	private TextView no_purchase_text;
	
	@Override  
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) 
	{
		updateActionBar();
		setHasOptionsMenu(true);
		
		View contentLayout = inflater.inflate(R.layout.fragment_home_view, container, false);
		
		final Activity parentActivity = this.getActivity();
	
		// Header...
		header_view_container = (LinearLayout) contentLayout.findViewById(R.id.release_timedown);
		
		// Content...
		content_item_latestlot = (LinearLayout) contentLayout.findViewById(R.id.content_item_1);
		content_item_lateststate = (LinearLayout) contentLayout.findViewById(R.id.content_item_2);
		content_item_latestfilter = (LinearLayout) contentLayout.findViewById(R.id.content_item_3);
		content_item_verifiedpurchases = (LinearLayout) contentLayout.findViewById(R.id.content_item_4);
		
		no_purchase_text = (TextView) contentLayout.findViewById(R.id.no_purchase_text);
		
		// Ad
		View native_ad_container = (View) contentLayout.findViewById(R.id.nativeADContainer);
		NativeADView adView = new NativeADView(this.getActivity(), "7030002594648242", native_ad_container);
		adView.loadAD();
		
		// Buttons...
		TextView goto_history_detail_btn = (TextView) contentLayout.findViewById(R.id.goto_history_detail_btn);
		goto_history_detail_btn.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) {
				Intent intent = new Intent( parentActivity , LotteryHistoryActivity.class );
				intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			    startActivity( intent );
			}
		});
		
		TextView goto_number_diagram_btn = (TextView) contentLayout.findViewById(R.id.goto_number_diagram_btn);
		goto_number_diagram_btn.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) {
				Intent intent = new Intent( parentActivity , LotteryTrendChartActivity.class );
				intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			    startActivity( intent );
			}
		});
		
		TextView goto_attributes_btn = (TextView) contentLayout.findViewById(R.id.goto_attributes_btn);
		goto_attributes_btn.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) {
				Intent intent = new Intent( parentActivity , LotteryAttributeActivity.class );
				intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			    startActivity( intent );
			}
		});
		
		TextView goto_purchases_btn = (TextView) contentLayout.findViewById(R.id.goto_purchases_btn);
		goto_purchases_btn.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) {
				Intent intent = new Intent( parentActivity , UserCenterActivity.class );
				intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			    startActivity( intent );
			}
		});
		
		initViews();
		
		return contentLayout;
	}
	
	@Override
	public void onCreateOptionsMenu(Menu menu, MenuInflater inflater)
	{
		// Inflate the menu; this adds items to the action bar if it is present.
		inflater.inflate(R.menu.home_menu, menu);		

		super.onCreateOptionsMenu(menu, inflater);
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item)
	{
		switch (item.getItemId())
		{
		case R.id.goto_mypurchase:
		{
			Intent intent = new Intent( this.getActivity() , UserCenterActivity.class );
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		    startActivity( intent );		
			break;
		}
		case R.id.goto_feedback:
		{
			Intent intent = new Intent(this.getActivity(), FeedbackActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            startActivity(intent);
            break;
		}
		case R.id.goto_configration:
		{
			Intent intent = new Intent(this.getActivity(), AttributeFilterOptionActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            startActivity(intent);
			break;
		}
		case R.id.goto_donation:
		{
			Intent intent = new Intent(this.getActivity(), DonationActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            startActivity(intent);
            break;
		}
		case R.id.goto_about:
		{
			Intent intent = new Intent(this.getActivity(), AboutActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            startActivity(intent);
            break;
		}
		}
		
		return super.onOptionsItemSelected(item);
	}
	
	private void initViews()
	{
		ReleaseInfo releaseInfo = LBDataManager.GetInstance().getReleaseInfo();
		if (releaseInfo != null) 
		{
			if (headerView == null)
				headerView = new ReleaseTimeDownView();

			headerView.AddToContainer(header_view_container);
			headerView.StartTimer();
		}

		LotteryInfo lotInfo = null;
		try {
			lotInfo = LotteryInfo.Create(LBDataManager.GetInstance().getLatestLottery());
		} catch (ParseException e) {
			e.printStackTrace();
		}
		
		if (lotInfo != null) 
		{
			if (lottoDetailView == null)
				lottoDetailView = new LotteryDetailView();

			lottoDetailView.setData(lotInfo);
			lottoDetailView.addToContainer(content_item_latestlot);
		}

		LotteryStateInfo stateInfo = LBDataManager.GetInstance().GetLatestLotteryStateInfo();
		if (stateInfo != null) 
		{
			if (lottoStatusView == null)
				lottoStatusView = new LotteryNumStatusView();

			lottoStatusView.setData(stateInfo);
			lottoStatusView.addToContainer(content_item_lateststate);
		}

		List<SchemeAttributeValueStatus> recommendFilters = LBDataManager.GetInstance().GetRecommendedConditions();
		if (recommendFilters != null) 
		{
			if (lottoFilterView == null)
				lottoFilterView = new RecommendAttributesView();

			lottoFilterView.setData(recommendFilters);
			lottoFilterView.addToContainer(content_item_latestfilter);
		}
		
		ArrayList<PurchaseInfo> verifiedPurchases = LBDataManager.GetInstance().getJustVerifiedPurchases();
		if (verifiedPurchases != null && verifiedPurchases.size() > 0)
		{
			if (verfiedPurchaseView == null)
				verfiedPurchaseView = new PurchasesView();
			
			verfiedPurchaseView.setSource(verifiedPurchases);
			verfiedPurchaseView.setExpandAll(true);
			verfiedPurchaseView.addToContainer(this.getActivity(), content_item_verifiedpurchases, 
					this.getActivity().getLayoutInflater());
		}
		else
		{
			// if there is nothing to display make the no-purchase text visible.
			no_purchase_text.setVisibility(View.VISIBLE);
		}
	}
	
	public void updateActionBar()
	{
		ActionBar actionBar = getActivity().getActionBar();  
		actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);
		actionBar.setTitle("最新");
		actionBar.setIcon(R.drawable.icon_news_grey);
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) 
	{
		super.onActivityResult(requestCode, resultCode, data);
		
		if (verfiedPurchaseView != null)
		{
			verfiedPurchaseView.refresh();
		}
	}
}
