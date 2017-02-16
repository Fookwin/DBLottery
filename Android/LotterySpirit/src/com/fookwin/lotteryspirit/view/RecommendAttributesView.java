package com.fookwin.lotteryspirit.view;

import java.util.List;

import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.data.SchemeAttributeValueStatus;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.FilterOption;
import com.fookwin.lotteryspirit.data.HelpCenter;
import com.fookwin.lotteryspirit.data.LBDataManager;

public class RecommendAttributesView
{
	private class AttributeListItem
	{
		public ImageView recommend_icon;
		public TextView energy;
		public TextView expression;
		public TextView arg_obm;
		public TextView cur_obm;
	}
	
	private class AttributeListAdapter extends BaseAdapter 
	{
		private int count = 5;
		private List<SchemeAttributeValueStatus> data = null;
		private FilterOption filterOption;		

		public AttributeListAdapter(List<SchemeAttributeValueStatus> _data)
		{
			data = _data;
			
		    // display 5 items in max. 
			count = Math.min(5, data.size());
			filterOption = LBDataManager.GetInstance().getFilterOption();
		}	

		public int getCount() {
			return count;
		}

		public Object getItem(int position) {
			return null;
		}

		public long getItemId(int position) {
			return 0;
		}

		public View getView(int position, View convertView, ViewGroup parent) 
		{				
			AttributeListItem tempItem = null;
			if (convertView == null)
			{
				convertView = LinearLayout.inflate(LotterySpirit.getInstance(), R.layout.list_item_filter_view, null);
				tempItem = new AttributeListItem();				
			
				tempItem.energy = (TextView)convertView.findViewById(R.id.filter_engine);
				tempItem.expression = (TextView)convertView.findViewById(R.id.filter_expression);
				tempItem.arg_obm = (TextView)convertView.findViewById(R.id.filter_average_obmission);
				tempItem.cur_obm = (TextView)convertView.findViewById(R.id.filter_cur_obmission);
				tempItem.recommend_icon = (ImageView)convertView.findViewById(R.id.recommend_icon);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (AttributeListItem)convertView.getTag();
			}
			
			SchemeAttributeValueStatus item = data.get(position);			
			if (item != null)
			{			    
				tempItem.energy.setText(Double.toString(item.getProtentialEnergy()));
				tempItem.expression.setText(item.getDisplayName());
				tempItem.arg_obm.setText(Double.toString(item.getAverageOmission()));
				tempItem.cur_obm.setText(Integer.toString(item.getMaxOmission()));
				tempItem.recommend_icon.setVisibility(filterOption.recommend(item) ? View.VISIBLE : View.INVISIBLE);
			}
			
			return convertView;
		}
	}
	
	private View detailView;
	private ListView filterListView;
	
	private List<SchemeAttributeValueStatus> data = null;
	private AttributeListAdapter listAdapter;
	private ImageView helpIcon;
	
	public void addToContainer(LinearLayout container)
	{
		if (detailView == null)
		{
			// create the view tab and add to container.
			detailView = LinearLayout.inflate(LotterySpirit.getInstance(), R.layout.view_filter_recommand,null);
			
			// get the controls.
			filterListView = (ListView)detailView.findViewById(R.id.filter_list);
			
			helpIcon = (ImageView)detailView.findViewById(R.id.helpIcon);
			helpIcon.setOnClickListener(new OnClickListener()
			{
				@Override
				public void onClick(View arg0)
				{
					HelpCenter.Instance().Show(29);
				}			
			});
		}
		
		if (data != null)
		{
			refreshView();
		}
		
		LayoutParams params = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT);	    
		container.addView(detailView, params);
	}
	
	private void refreshView()
	{
		if (detailView != null && data != null)
		{				
			// build filter list. Note: we just show the top five here.
			listAdapter = new AttributeListAdapter(data);
			filterListView.setAdapter(listAdapter);
			
			resetListHeight();
		}
	}
	
	private void resetListHeight()
	{
	    if (listAdapter == null) 
	        return;
 
	    int totalHeight = 0;
	    for (int i = 0; i < listAdapter.getCount(); i++)
	    {
	        View listItem = listAdapter.getView(i, null, filterListView);
	        listItem.measure(0, 0); 
	        totalHeight += listItem.getMeasuredHeight();
	    }

	    ViewGroup.LayoutParams params = filterListView.getLayoutParams();
	    params.height = totalHeight + (filterListView.getDividerHeight() * (listAdapter.getCount() - 1));
	    filterListView.setLayoutParams(params);
	}
	
	public void setData(List<SchemeAttributeValueStatus> recommendFilters)
	{		
		data = recommendFilters;
	}
}
