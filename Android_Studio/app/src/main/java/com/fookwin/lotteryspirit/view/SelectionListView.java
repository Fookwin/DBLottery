package com.fookwin.lotteryspirit.view;

import java.util.List;

import android.app.Activity;
import android.content.Context;
import android.os.Handler;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.BaseAdapter;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.TextView;
import com.fookwin.lotterydata.data.Constraint;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotterydata.data.SchemeSelector;
import com.fookwin.lotteryspirit.R;

public class SelectionListView
{
	private class SelectionItem
	{		
		public TextView item_index;
		public TextView summary;
		public ImageButton deleteBtn;
	}
	
	public enum SelectionTypeEnum
	{
		SELECTOR,
		CONSTRAINT,
		SCHEME
	}
	
	private class SelectionListAdapter extends BaseAdapter implements OnClickListener 
	{
		private int count = 10;
		private int max_count = 0;
		private Context context;	
		private Handler onItemDeletedHandler;
		private SelectionTypeEnum selection_type;

		public SelectionListAdapter(Context _context, SelectionTypeEnum type)
		{
			context = _context;
			selection_type = type;
			
			refreshCount();
		}	
		
		public void setOnItemDeletedHandler(Handler handler)
		{
			onItemDeletedHandler = handler;
		}
		
		public boolean refreshCount()
		{
			switch (selection_type)
			{
			case CONSTRAINT:
				count = sourceDataList.size();
				max_count = count;
				break;
			case SCHEME:
				max_count = sourceDataList.size();
				if (displayAllItems)
					count = max_count;
				else
				{
					count = Math.min(10, max_count);
				
					if (max_count > 10)
					{
						return true; // indicates to show loading more button.
					}
				}
				
				break;
			case SELECTOR:
				count = sourceDataList.size();
				max_count = count;
				break;			
			}	
			
			return false;
		}
		
		public boolean showMore()
		{
			if (count < max_count)
			{
				count += 10;
				if (count > max_count)
					count = max_count;
				
				return true;
			}
			
			return false;
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
			SelectionItem tempItem = null;
			if (convertView == null)
			{
				convertView = LinearLayout.inflate(context,R.layout.list_item_selection_view, null);
				tempItem = new SelectionItem();
				
				tempItem.item_index = (TextView)convertView.findViewById(R.id.item_index);
				tempItem.summary = (TextView)convertView.findViewById(R.id.item_summary);
				tempItem.deleteBtn = (ImageButton)convertView.findViewById(R.id.delete_btn);
				tempItem.deleteBtn.setOnClickListener(this);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (SelectionItem)convertView.getTag();
			}
			
			tempItem.item_index.setText(Integer.toString(position + 1));
			
			tempItem.deleteBtn.setVisibility(isDeletable ? View.VISIBLE : View.GONE);
			
			switch (selection_type)
			{
			case CONSTRAINT:				
			{
				Constraint constraint = (Constraint) sourceDataList.get(position);			
				if (constraint != null)
				{
					tempItem.summary.setText(constraint.getDisplayExpression());
					tempItem.deleteBtn.setTag(position);
				}
				break;
			}
			case SCHEME:
			{
				Scheme item = (Scheme) sourceDataList.get(position);			
				if (item != null)
				{
					tempItem.summary.setText(item.getDisplayExpression());
					tempItem.deleteBtn.setTag(position);
				}

				break;
			}
			case SELECTOR:
			{
				SchemeSelector selector = (SchemeSelector) sourceDataList.get(position);			
				if (selector != null)
				{
					tempItem.summary.setText(selector.getDisplayExpression());
					tempItem.deleteBtn.setTag(position);
				}
				break;			
			}
			}
			
			return convertView;
		}

		@Override
		public void onClick(View v) 
		{
			int position = (Integer) v.getTag();
			if (position >= 0)
			{				
				if (onItemDeletedHandler != null)
					onItemDeletedHandler.sendEmptyMessage(position);
			}
		}
	}
	
	private boolean displayAllItems = false;
	private SelectionTypeEnum selection_type;
	private SelectionListAdapter adapter;
	
	private Activity own_activity;
	private TextView btnLoadMoreScheme;
	private ListView listView;
	
	private List<?> sourceDataList;
	private boolean isDeletable = true;
	
	public SelectionListView(SelectionTypeEnum type)
	{
		selection_type = type;
	}
	
	public boolean isDeletable()
	{
		return isDeletable;
	}
	
	public void setDeletable(boolean deletable)
	{
		isDeletable = deletable;
	}
	
	public void buildList(Activity activity, ListView list, List<?> source, boolean expandAll)
	{
		own_activity = activity;
		listView = list;
		sourceDataList = source;
		adapter = new SelectionListAdapter(own_activity, selection_type);

		displayAllItems = expandAll;
		showMoreButton(adapter.refreshCount());		
		list.setAdapter(adapter);
	}
	
	public void setOnItemDeletedHandler(Handler handler)
	{
		adapter.setOnItemDeletedHandler(handler);
	}
	
	public void refresh()
	{
		showMoreButton(adapter.refreshCount());
		adapter.notifyDataSetChanged();
	}
	
	public void resetListHeight()
	{
	    if (adapter == null) 
	        return;

	    int totalHeight = 0;
	    for (int i = 0, len = adapter.getCount(); i < len; i++)
	    {
	        View listItem = adapter.getView(i, null, listView);
	        listItem.measure(0, 0); 
	        totalHeight += listItem.getMeasuredHeight();
	    }

	    ViewGroup.LayoutParams params = listView.getLayoutParams();
	    params.height = totalHeight + (listView.getDividerHeight() * (adapter.getCount() - 1));
	    listView.setLayoutParams(params);
	}
	
	private void showMoreButton(boolean show)
	{
		if (show)
		{
			if (btnLoadMoreScheme == null)
			{
				btnLoadMoreScheme = (TextView) own_activity.getLayoutInflater().inflate(R.layout.list_item_loadmore, null);
				
				btnLoadMoreScheme.setOnClickListener(new OnClickListener() {
					public void onClick(View v) {
						new Handler().post(new Runnable() {
							public void run() {
								if (adapter.showMore())
								{
									adapter.notifyDataSetChanged();
								}
								else
								{
									btnLoadMoreScheme.setEnabled(false);
									btnLoadMoreScheme.setText("－－－　到底了　－－－");
								}
							}
						});
					}
				});
			}			
			
			listView.addFooterView(btnLoadMoreScheme);
		}
		else
		{
			if (btnLoadMoreScheme != null)
				listView.removeFooterView(btnLoadMoreScheme);
		}
	}
}
