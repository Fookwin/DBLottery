package com.fookwin.lotteryspirit.view;

import java.util.ArrayList;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.LinearLayout.LayoutParams;

import com.fookwin.lotteryspirit.PurchaseDetailActivity;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.data.PurchaseInfo;
import com.fookwin.lotteryspirit.data.PurchaseManager;

public class PurchasesView {
	private class PurchaseItem
	{
		public TextView issue_text;
		public TextView releaseAt_text;
		public TextView cost_text;
		public TextView earning_text;
		public ImageView status_icon;
	}
	
	private class PurchaseListAdapter extends BaseAdapter 
	{
		private int count = 0;
		private LayoutInflater inflater;
		private PurchaseManager _manager = null;
		private int max_count;
		
		private int _winColor;
		private Context _context;

		public PurchaseListAdapter(LayoutInflater inf, Context context, 
				PurchaseManager manager)
		{
			inflater = inf;
			
			_manager = manager;
			
			max_count = _source != null ? _source.size() :_manager.getPurchaseCount();
			
		    _winColor = context.getResources().getColor(R.color.darkred);
		    _context = context;
		}
		
		public void refresh()
		{
			// reset count.
			max_count = _source != null ? _source.size() :_manager.getPurchaseCount();
			if (count > max_count)
				count = max_count;
		}
		
		public boolean showMore()
		{
			if (count < max_count)
			{
				count += _expandAll ? max_count : 10;
				if (count > max_count)
					count = max_count;
				
				return count < max_count;
			}
			
			return false;
		}
		
		public int getCount() {
			return count;
		}

		public Object getItem(int position) {
			return _source != null ?  _source.get(position) : _manager.getPurchase(position);	
		}

		public long getItemId(int position) {
			return 0;
		}

		public View getView(int position, View convertView, ViewGroup parent) 
		{
			PurchaseItem tempItem = null;
			if (convertView == null)
			{
				convertView = inflater.inflate(R.layout.list_item_purchase, parent, false);
				tempItem = new PurchaseItem();
				
				tempItem.issue_text = (TextView)convertView.findViewById(R.id.issue_number_text);
				tempItem.releaseAt_text = (TextView)convertView.findViewById(R.id.release_time_text);
				tempItem.cost_text = (TextView)convertView.findViewById(R.id.cost_text);
				tempItem.earning_text = (TextView)convertView.findViewById(R.id.earning_text);
				tempItem.status_icon = (ImageView)convertView.findViewById(R.id.purchase_status_icon);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (PurchaseItem)convertView.getTag();
			}
			
			PurchaseInfo itemData = (PurchaseInfo) getItem(position);			
			if (itemData != null)
			{
				tempItem.issue_text.setText(String.valueOf(itemData.Issue));
				tempItem.releaseAt_text.setText(itemData.ReleaseAt);
				tempItem.cost_text.setText(String.valueOf(itemData.Buy * 2) + "元");
				
				int imgID = R.drawable.icon_purchase_normal;
				if (itemData.Win < 0)
				{
					imgID = R.drawable.icon_purchase_pending;
					tempItem.earning_text.setText(itemData.Win == -1 ? "计算奖金" : "等待开奖");
					tempItem.earning_text.setTextAppearance(_context, android.R.style.TextAppearance_Small);
					if (itemData.Win == -1)
						tempItem.earning_text.setTextColor(_winColor);
				}
				else if (itemData.Win > 0)
				{
					imgID = R.drawable.icon_purchase_win;
					tempItem.earning_text.setText(String.valueOf(itemData.Win) + "元");
					tempItem.earning_text.setTextAppearance(_context, android.R.style.TextAppearance_Medium);
					tempItem.earning_text.setTextColor(_winColor);
				}
				else
				{
					tempItem.earning_text.setText("没有中奖");
					tempItem.earning_text.setTextAppearance(_context, android.R.style.TextAppearance_Small);
				}
					
				tempItem.status_icon.setImageResource(imgID);
			}
			
			return convertView;
		}
	}

	private PurchaseListAdapter purchase_list_adapter;
	private ListView purchase_list;
	private TextView total_cost;
	private TextView earn_money;
	
	private ArrayList<PurchaseInfo> _source = null;
	private boolean _expandAll = false;
	
	public void setSource(ArrayList<PurchaseInfo> src)
	{
		_source = src;
	}
	
	public void setExpandAll(boolean expand)
	{
		_expandAll = expand;
	}
	
	public void addToContainer(final Activity parent, LinearLayout container, LayoutInflater inflater)
	{
		PurchaseManager manager = LBDataManager.GetInstance().getPurchaseManager();
		
		View detailView = LinearLayout.inflate(parent.getApplicationContext(), 
				R.layout.view_purchases_panel,null);
		
		purchase_list = (ListView)detailView.findViewById(R.id.purchase_list);
		purchase_list_adapter = new PurchaseListAdapter(inflater, parent.getApplicationContext(), manager);
		
		if (purchase_list_adapter.showMore())
		{
			final TextView btnLoadMore = (TextView) inflater.inflate(R.layout.list_item_loadmore, purchase_list, false);
			purchase_list.addFooterView(btnLoadMore);
			
			btnLoadMore.setOnClickListener(new OnClickListener() {
				public void onClick(View v) {
					new Handler().post(new Runnable() {
						public void run() {
							if (purchase_list_adapter.showMore())
							{
								purchase_list_adapter.notifyDataSetChanged();
							}
							else
							{
								btnLoadMore.setEnabled(false);
								btnLoadMore.setText("没有了");
							}
						}
					});
				}
			});
		}
		
		purchase_list.setOnItemClickListener(new OnItemClickListener() 
		{
			@Override
			public void onItemClick(AdapterView<?> adapterView, View view, int position, long id)
			{				
				if (position >= 0)
				{
					PurchaseInfo info = (PurchaseInfo) purchase_list_adapter.getItem(position);
					if (info != null)
					{		
						Intent intent = new Intent(parent, PurchaseDetailActivity.class);
						
						Bundle bundle = new Bundle();
						bundle.putInt("purchase", info.ID);
						intent.putExtras(bundle);	
                        
                        parent.startActivityForResult(intent, 1);
					}					
				}
			}
		});
		
		purchase_list.setAdapter(purchase_list_adapter);
		
		// initialize the total summary
		total_cost = (TextView)detailView.findViewById(R.id.input_money);
		earn_money = (TextView)detailView.findViewById(R.id.earn_money);
		resetCostAndWin();
		
		if (_expandAll)
		{
			resetListHeight();
		}
		
		LayoutParams params = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);	    
		container.addView(detailView, params);
	}
	
	private void resetCostAndWin()
	{
		PurchaseManager manager = LBDataManager.GetInstance().getPurchaseManager();
		int cost = 0, win = 0;
		if (_source != null)
		{
			for (PurchaseInfo info : _source)
			{
				cost += info.Buy  * 2;
				
				if (info.Win > 0)
					win += info.Win;
			}
		}
		else			
		{
			cost = manager.getTotalCost();
			win = manager.getTotalWin();
		}
		
		total_cost.setText(String.valueOf(cost));
		earn_money.setText(String.valueOf(win));
	}
	
	private void resetListHeight()
	{
	    if (purchase_list_adapter == null) 
	        return;
 
	    int totalHeight = 0;
	    for (int i = 0; i < purchase_list_adapter.getCount(); i++)
	    {
	        View listItem = purchase_list_adapter.getView(i, null, purchase_list);
	        listItem.measure(0, 0); 
	        totalHeight += listItem.getMeasuredHeight();
	    }

	    ViewGroup.LayoutParams params = purchase_list.getLayoutParams();
	    params.height = totalHeight + (purchase_list.getDividerHeight() * (purchase_list_adapter.getCount() - 1));
	    purchase_list.setLayoutParams(params);
	}
	
	public void refresh()
	{
		resetCostAndWin();
		purchase_list_adapter.refresh();
		purchase_list_adapter.notifyDataSetChanged();
		
		if (_expandAll)
		{
			resetListHeight();
		}
	}
}
