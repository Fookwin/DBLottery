package com.fookwin.lotteryspirit.view;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.content.Context;
import android.text.Html;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.BaseAdapter;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;
import com.fookwin.lotterydata.data.Constraint;
import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotterydata.data.SchemeSelector;
import com.fookwin.lotteryspirit.R;

public class VerifiedItemListView
{
	private class VerifiedItem
	{		
		public TextView text;
	}
	
	public enum ItemTypeEnum
	{
		SELECTOR,
		CONSTRAINT,
		SCHEME
	}
	
	private class VerifiedItemListAdapter extends BaseAdapter implements OnClickListener 
	{
		private Context context;	
		private List<Object> _data = null;
		private Lottery _baseline = null;
		private int _baselineIndex = -1;
		private ItemTypeEnum selection_type;

		public VerifiedItemListAdapter(Context _context, ItemTypeEnum type,
				ArrayList<Object> arrayList, Lottery baseline, int baselineIndex)
		{
			context = _context;
			selection_type = type;
			_baseline = baseline;
			_baselineIndex = baselineIndex;
			_data = arrayList;
		}

		public int getCount() {
			return _data.size();
		}

		public Object getItem(int position) {
			return _data.get(position);
		}

		public long getItemId(int position) {
			return 0;
		}

		public View getView(int position, View convertView, ViewGroup parent) 
		{				
			VerifiedItem tempItem = null;
			if (convertView == null)
			{
				convertView = LinearLayout.inflate(context,R.layout.list_item_verified_item, null);
				tempItem = new VerifiedItem();				
				tempItem.text = (TextView)convertView.findViewById(R.id.item_text);				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (VerifiedItem)convertView.getTag();
			}
			
			tempItem.text.setText(Integer.toString(position + 1));
			
			switch (selection_type)
			{
			case CONSTRAINT:				
			{
				Constraint constraint = (Constraint) _data.get(position);			
				if (constraint != null)
				{
					String exp = constraint.getDisplayExpression();
					if (_baseline != null && constraint.Meet(_baseline.getScheme(), _baselineIndex))
					{
						exp = "<font color='red'>" + exp + "</font>";
					}						
					
					tempItem.text.setText(Html.fromHtml(exp));
				}
				break;
			}
			case SCHEME:
			{
				Scheme item = (Scheme) _data.get(position);			
				if (item != null)
				{
					if (_baseline == null)
					{
						tempItem.text.setText(item.getDisplayExpression());
					}
					else
					{
						tempItem.text.setText(Html.fromHtml(item.getVerifiedExpression(_baseline.getScheme())));
					}
				}

				break;
			}
			case SELECTOR:
			{
				SchemeSelector selector = (SchemeSelector) _data.get(position);			
				if (selector != null)
				{
					tempItem.text.setText(selector.getDisplayExpression());
				}

				break;			
			}
			}
			
			return convertView;
		}

		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			
		}
	}
	
	private ItemTypeEnum selection_type;
	private VerifiedItemListAdapter adapter;
	
	public VerifiedItemListView(ItemTypeEnum type)
	{
		selection_type = type;
	}
	
	@SuppressWarnings("unchecked")
	public void buildList(Activity activity, ListView list, 
			Object arrayList, Lottery baseline, int baselineIndex)
	{
		adapter = new VerifiedItemListAdapter(activity, selection_type, 
				(ArrayList<Object>) arrayList, baseline, baselineIndex);
		list.setAdapter(adapter);
	}}
