package com.fookwin.lotteryspirit;

import com.fookwin.lotterydata.data.SchemeAttribute;
import com.fookwin.lotterydata.data.SchemeAttributeValueStatus;
import com.fookwin.lotterydata.data.Set;
import com.fookwin.lotteryspirit.data.FilterOption;
import com.fookwin.lotteryspirit.data.HelpCenter;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.view.BannerAdView;

import android.os.Bundle;
import android.app.ActionBar;
import android.app.Activity;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

public class AttributeDetailActivity extends Activity 
{
	private class AttributeStateItem
	{
		public TextView name_text;
		public TextView hitcount_text;
		public TextView hitprob_text;
		public TextView avgobmm_text;
		public TextView maxobmm_text;
		public TextView curobmm_text;
		public TextView ptnprob_text;
		public View		selected_marker;
	}
	
	private class AttributeStateListAdapter extends BaseAdapter 
	{
		private int count = 10;
		private LayoutInflater inflater;
		private SchemeAttribute attribute = null;
		private FilterOption filterOption;
		private Set selectedIndices = null;
		
		public AttributeStateListAdapter(LayoutInflater inf, SchemeAttribute attri, Set selIndices)
		{
			inflater = inf;
			attribute = attri;
			count = attribute.getValueStates().size();
			selectedIndices = selIndices;			
			
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
			AttributeStateItem tempItem = null;
			if (convertView == null)
			{
				convertView = inflater.inflate(R.layout.list_item_attribute, parent, false);
				tempItem = new AttributeStateItem();
				
				tempItem.name_text = (TextView)convertView.findViewById(R.id.state_name);
				tempItem.hitcount_text = (TextView)convertView.findViewById(R.id.hit_count_text);
				tempItem.hitprob_text = (TextView)convertView.findViewById(R.id.hit_prob_text);
				tempItem.avgobmm_text = (TextView)convertView.findViewById(R.id.average_obmission);
				tempItem.maxobmm_text = (TextView)convertView.findViewById(R.id.max_obmission);
				tempItem.curobmm_text = (TextView)convertView.findViewById(R.id.current_obmission);
				tempItem.ptnprob_text = (TextView)convertView.findViewById(R.id.protential_prob);
				tempItem.selected_marker = (View)convertView.findViewById(R.id.selected_marker);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (AttributeStateItem)convertView.getTag();
			}

			
			SchemeAttributeValueStatus state = attribute.getValueStates().get(position);			
			if (state != null)
			{
				tempItem.name_text.setText(state.getValueExpression());
				tempItem.hitcount_text.setText(Integer.toString(state.getHitCount()));
				tempItem.hitprob_text.setText(Double.toString(state.getHitProbability()));
				tempItem.avgobmm_text.setText(Double.toString(state.getAverageOmission()));
				tempItem.maxobmm_text.setText(Integer.toString(state.getMaxOmission()));
				tempItem.curobmm_text.setText(Integer.toString(state.getImmediateOmission()));
				tempItem.ptnprob_text.setText(Double.toString(state.getProtentialEnergy()));
				
				if (filterOption.passed(state))
				{
					tempItem.ptnprob_text.setTextColor(convertView.getResources().getColor(R.color.red));
				}
				else
				{
					tempItem.ptnprob_text.setTextColor(convertView.getResources().getColor(R.color.darkgreen));
				}
				
				boolean isSelected = (selectedIndices != null && selectedIndices.Contains(position + 1));
				int colorId = isSelected ? colorId = R.color.lightgrey : R.color.translucent;
				convertView.setBackgroundColor(convertView.getResources().getColor(colorId));
				
				tempItem.selected_marker.setVisibility(isSelected ? View.VISIBLE : View.INVISIBLE);
			}
			
			return convertView;
		}
	}
	
	private AttributeStateListAdapter list_adapter;
	private ListView status_list;
	private SchemeAttribute attribute;	
	private ImageView helpIcon;
	private Set selectedIndices = null;
	private String preSelection = null;

	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_attribute_detail);	
		
		status_list = (ListView) findViewById(R.id.status_list);
		
		// get the data transfered from parent.   
		String key = getIntent().getStringExtra("key");
		preSelection = getIntent().getStringExtra("selected");
		if (preSelection != null)
		{
			selectedIndices = new Set(preSelection);
			
			getActionBar().setDisplayHomeAsUpEnabled(true);
			getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
			getActionBar().setTitle("选择属性值");
		}
		else
		{
			selectedIndices = null;
			
			getActionBar().setDisplayHomeAsUpEnabled(true);
			getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
			getActionBar().setTitle("属性详情");

			// Ad
			ViewGroup bannerContainer = (ViewGroup) findViewById(R.id.bannerContainer);
			BannerAdView bannerAdView = new BannerAdView(this, "6030514978471349", bannerContainer);
			bannerAdView.loadAD();
		}

		attribute = LBDataManager.GetInstance().getLastAttributes().Attribute(key);
		if (attribute != null)
		{			
			list_adapter = new AttributeStateListAdapter(getLayoutInflater(), attribute, selectedIndices);
			
			// Set header name.
			View headerView = getLayoutInflater().inflate(R.layout.list_header_attribute, status_list, false);
			TextView name_text = (TextView)headerView.findViewById(R.id.attribute_name);
			name_text.setText(attribute.getDisplayName());
			
			status_list.addHeaderView(headerView);
			status_list.setAdapter(list_adapter);
			
			status_list.setOnItemClickListener(new OnItemClickListener()
			{
				@Override
				public void onItemClick(AdapterView<?> arg0, View arg1,
						int position, long arg3) 
				{
					if (selectedIndices != null && attribute != null && position > 0) // skip header row.
					{
						if (selectedIndices.Contains(position))
							selectedIndices.Remove(position);
						else 
							selectedIndices.Add(position);
						
						list_adapter.notifyDataSetChanged();
					}					
				}			
			});
			
			helpIcon = (ImageView)findViewById(R.id.helpIcon);
			helpIcon.setOnClickListener(new OnClickListener()
			{
				@Override
				public void onClick(View arg0)
				{
					HelpCenter.Instance().Show(attribute.getHelpID());
				}			
			});
		}
	}
	
	@Override
	public void onBackPressed() 
	{
		if (selectedIndices != null)
		{
			// get the data transfered from parent.
			Intent intent = new Intent();
			intent.putExtra("key", attribute.getKey());
			intent.putExtra("selected", selectedIndices.toString());
			
			setResult(RESULT_OK, intent);
		}
		else
		{		
			setResult(RESULT_CANCELED, null);
		}
		
		finish();
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	        	onBackPressed();
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
}
