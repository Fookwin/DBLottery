package com.fookwin.lotteryspirit.view;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Map;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.BaseExpandableListAdapter;
import android.widget.ExpandableListView;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ExpandableListView.OnChildClickListener;

import com.fookwin.lotterydata.data.SchemeAttribute;
import com.fookwin.lotterydata.data.SchemeAttributeCategory;
import com.fookwin.lotterydata.data.SchemeAttributes;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.FilterOption;
import com.fookwin.lotteryspirit.data.LBDataManager;

public class ConstraintListView
{
	private class FilterListItem
	{
		public TextView filter_text;
		public TextView engry_score;
		public ImageView recommend_icon;
	}
	
	private class FilterCategoryListItem
	{
		public TextView category_text;
	}
	
	private class FilterCategory
	{
		public SchemeAttributeCategory category;
		private ArrayList<SchemeAttribute> attributes = null;
		
		public FilterCategory(SchemeAttributeCategory data)
		{
			category = data;
		}
		
		public ArrayList<SchemeAttribute> getAttributes()
		{
			if (attributes == null)
			{
				attributes = new ArrayList<SchemeAttribute>();
				
				for (Map.Entry<String, SchemeAttribute> attri : category.getAttributes().entrySet())
				{
					attributes.add(attri.getValue());
				}
				
				Collections.sort(attributes, new Comparator<SchemeAttribute>() 
				{
					@Override
					public int compare(SchemeAttribute lhs, SchemeAttribute rhs) {
						return lhs.getKey().compareTo(rhs.getKey());
					}
				});
			}
			
			return attributes;
		}
	}
	
	private class FilterListAdapter extends BaseAdapter 
	{
		private int count = 10;
		private LayoutInflater inflater;
		private ArrayList<SchemeAttribute> filters = null;
		private FilterOption filterOption;		

		public FilterListAdapter(LayoutInflater inf, ArrayList<SchemeAttribute> data)
		{
			inflater = inf;
			filters = data;
			count = filters.size();
			filterOption = LBDataManager.GetInstance().getFilterOption();
		}	

		public int getCount() {
			return count;
		}

		public Object getItem(int position) {
			return filters.get(position);
		}

		public long getItemId(int position) {
			return 0;
		}

		public View getView(int position, View convertView, ViewGroup parent) 
		{				
			FilterListItem tempItem = null;
			if (convertView == null)
			{
				convertView = inflater.inflate(R.layout.list_item_filter, parent, false);
				tempItem = new FilterListItem();
				
				tempItem.filter_text = (TextView)convertView.findViewById(R.id.filter_name);
				tempItem.engry_score = (TextView)convertView.findViewById(R.id.engry_score);
				tempItem.recommend_icon = (ImageView)convertView.findViewById(R.id.recommend_icon);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (FilterListItem)convertView.getTag();
			}

			
			SchemeAttribute attri = filters.get(position);			
			if (attri != null)
			{
				double maxEnergy = attri.getMaxProtentialEnergy();
				boolean isRecommended = filterOption.recommend(maxEnergy);
				tempItem.filter_text.setText(attri.getDisplayName());
				tempItem.engry_score.setText("最大偏离值 " + Double.toString(maxEnergy));				
				tempItem.recommend_icon.setVisibility(isRecommended ? View.VISIBLE : View.INVISIBLE);
			}
			
			return convertView;
		}
	}

	private class FilterCategoryListAdapter extends BaseExpandableListAdapter
	{
		private LayoutInflater inflater;
		private ArrayList<FilterCategory> categories;
		private FilterOption filterOption;
		
		public FilterCategoryListAdapter(LayoutInflater inf, SchemeAttributes data)
		{
			inflater = inf;
			categories = new ArrayList<FilterCategory>();
			filterOption = LBDataManager.GetInstance().getFilterOption();
			
			for (Map.Entry<String, SchemeAttributeCategory> cat : data.getCategories().entrySet())
			{
				categories.add(new FilterCategory(cat.getValue()));
			}
			
			Collections.sort(categories, new Comparator<FilterCategory>() 
			{
				@Override
				public int compare(FilterCategory lhs, FilterCategory rhs) {
					return lhs.category.getName().compareTo(rhs.category.getName());
				}
			});
		}
		
		@Override
		public Object getChild(int groupPosition, int childPosition)
		{
			return categories.get(groupPosition).getAttributes().get(childPosition);
		}

		@Override
		public long getChildId(int groupPosition, int childPosition) {
	
			return childPosition;
		}

		@Override
		public View getChildView(int groupPosition, int childPosition,
				boolean isLastChild, View convertView, ViewGroup parent)
		{
			FilterListItem tempItem = null;
			if (convertView == null)
			{
				convertView = inflater.inflate(R.layout.list_item_filter, parent, false);
				tempItem = new FilterListItem();
				
				tempItem.filter_text = (TextView)convertView.findViewById(R.id.filter_name);
				tempItem.engry_score = (TextView)convertView.findViewById(R.id.engry_score);
				tempItem.recommend_icon = (ImageView)convertView.findViewById(R.id.recommend_icon);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (FilterListItem)convertView.getTag();
			}

			
			SchemeAttribute attri = (SchemeAttribute) getChild(groupPosition, childPosition);			
			if (attri != null)
			{
				double maxEnergy = attri.getMaxProtentialEnergy();
				boolean isRecommended = filterOption.recommend(maxEnergy) && filterOption.passed(attri);
				tempItem.filter_text.setText(attri.getDisplayName());
				tempItem.engry_score.setText("最大偏离值 " + Double.toString(maxEnergy));				
				tempItem.recommend_icon.setVisibility(isRecommended ? View.VISIBLE : View.INVISIBLE);
			}
			
			return convertView;
		}

		@Override
		public int getChildrenCount(int groupPosition) {
			return categories.get(groupPosition).getAttributes().size();
		}

		@Override
		public Object getGroup(int groupPosition) {
			return categories.get(groupPosition).category;
		}

		@Override
		public int getGroupCount() {
			return categories.size();
		}

		@Override
		public long getGroupId(int groupPosition) {
			return groupPosition;
		}

		@Override
		public View getGroupView(int groupPosition, boolean isExpanded,
				View convertView, ViewGroup parent) {
			
			FilterCategoryListItem tempItem = null;
			if (convertView == null)
			{
				convertView = inflater.inflate(R.layout.list_item_filter_category, parent, false);
				tempItem = new FilterCategoryListItem();
				
				tempItem.category_text = (TextView)convertView.findViewById(R.id.category_name);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (FilterCategoryListItem)convertView.getTag();
			}

			
			SchemeAttributeCategory cat = (SchemeAttributeCategory) this.getGroup(groupPosition);			
			if (cat != null)
			{
				tempItem.category_text.setText(cat.getDisplayName());
			}
			
			return convertView;
		}

		@Override
		public boolean hasStableIds() {
			return false;
		}

		@Override
		public boolean isChildSelectable(int groupPosition, int childPosition) {
			return true;
		}
	}	

	private ArrayList<SchemeAttribute> recommended_filters = null;
	private FilterListAdapter recommend_list_adapter;
	private FilterCategoryListAdapter full_list_adapter;
	
	private ListView recommended_list;
	
	private Handler onItemClickedHandler;
	
	public void setOnItemClickedHandler(Handler _handler)
	{
		onItemClickedHandler = _handler;
	}
	
	public ConstraintListView()
	{
	}	
	
	public void refreshRecommendedList()
	{
		if (recommended_list != null)
		{
			FilterOption filterOption = LBDataManager.GetInstance().getFilterOption();
			
			recommended_filters.clear();
			
			SchemeAttributes attributes = LBDataManager.GetInstance().getLastAttributes();
			for (Map.Entry<String, SchemeAttributeCategory> cat : attributes.getCategories().entrySet())
			{
				for (Map.Entry<String, SchemeAttribute> attri : cat.getValue().getAttributes().entrySet())
				{						
					if (filterOption.passed(attri.getValue()))
					{
						recommended_filters.add(attri.getValue());
					}
				}
			}
			
			Collections.sort(recommended_filters, new Comparator<SchemeAttribute>() 
			{
				@Override
				public int compare(SchemeAttribute lhs, SchemeAttribute rhs) {
					return Double.compare(rhs.getMaxProtentialEnergy(), lhs.getMaxProtentialEnergy());
				}
			});
			
			recommend_list_adapter.count = recommended_filters.size();
			recommend_list_adapter.notifyDataSetChanged();
		}
	}
	
	public void initRecommendedFilterList(LayoutInflater inflater, ListView list)
	{	
		recommended_list = list;
		
		FilterOption filterOption = LBDataManager.GetInstance().getFilterOption();
		
		if (recommend_list_adapter == null)
		{
			recommended_filters = new ArrayList<SchemeAttribute>();
			
			SchemeAttributes attributes = LBDataManager.GetInstance().getLastAttributes();
			for (Map.Entry<String, SchemeAttributeCategory> cat : attributes.getCategories().entrySet())
			{
				for (Map.Entry<String, SchemeAttribute> attri : cat.getValue().getAttributes().entrySet())
				{						
					if (filterOption.passed(attri.getValue()))
					{
						recommended_filters.add(attri.getValue());
					}
				}
			}
			
			Collections.sort(recommended_filters, new Comparator<SchemeAttribute>() 
			{
				@Override
				public int compare(SchemeAttribute lhs, SchemeAttribute rhs) {
					return Double.compare(rhs.getMaxProtentialEnergy(), lhs.getMaxProtentialEnergy());
				}
			});
			
			recommend_list_adapter = new FilterListAdapter(inflater, recommended_filters);
	
			list.setOnItemClickListener(new OnItemClickListener()
			{
				@Override
				public void onItemClick(AdapterView<?> adapterView, View view, int position, long id)
				{				
					if (position >= 0)
					{						
						SchemeAttribute attri = (SchemeAttribute) recommend_list_adapter.getItem(position);
						if (attri != null)
						{
							if (onItemClickedHandler != null)
							{
								Message msg = new Message();
								Bundle bundle = new Bundle();
								bundle.putString("key", attri.getKey());
								msg.setData(bundle);
								onItemClickedHandler.sendMessage(msg);
							}								
						}				
					}
				}
			});
			
			list.setAdapter(recommend_list_adapter);
		}
	}
	
	public void initFullFilterList(LayoutInflater inflater, ExpandableListView list)
	{	
		if (full_list_adapter == null)
		{
			SchemeAttributes attributes = LBDataManager.GetInstance().getLastAttributes();
			
			full_list_adapter = new FilterCategoryListAdapter(inflater, attributes);
			list.setAdapter(full_list_adapter);
	
			list.setOnChildClickListener(new OnChildClickListener() {
	
				@Override
				public boolean onChildClick(ExpandableListView parent, View v,
						int groupPosition, int childPosition, long id)
				{
					SchemeAttribute attri = (SchemeAttribute) full_list_adapter.getChild(groupPosition, childPosition);
					if (attri != null)
					{						
						Message msg = new Message();
						Bundle bundle = new Bundle();
						bundle.putString("key", attri.getKey());
						msg.setData(bundle);
						onItemClickedHandler.sendMessage(msg);
					}
					
					return false;
				}
			});
		}
	}
}
