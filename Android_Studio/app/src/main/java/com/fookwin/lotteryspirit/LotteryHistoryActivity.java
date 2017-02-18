package com.fookwin.lotteryspirit;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.data.History;
import com.fookwin.lotterydata.data.Lottery;
import com.fookwin.lotteryspirit.data.LBDataManager;

import android.app.ActionBar;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.AdapterView.OnItemClickListener;

public class LotteryHistoryActivity extends Activity 
{
	private class HistoryListItem
	{
		public TextView tip_text;
		public TextView issue_text;
		public TextView date_text;
		public Button red1_btn;
		public Button red2_btn;
		public Button red3_btn;
		public Button red4_btn;
		public Button red5_btn;
		public Button red6_btn;
		public Button blue_btn;
	}
	
	private class HistoryListAdapter extends BaseAdapter 
	{
		private int count = 10;
		private LayoutInflater inflater;
		private History _history = null;
		private int max_count = 0;

		public HistoryListAdapter(LayoutInflater inf)
		{
			inflater = inf;
			
			_history = LBDataManager.GetInstance().getHistory();
			max_count = _history.getCount();
			count = Math.min(10, max_count);
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
			HistoryListItem tempItem = null;
			if (convertView == null)
			{
				convertView = inflater.inflate(R.layout.list_item_lottery, parent, false);
				tempItem = new HistoryListItem();
				
				tempItem.tip_text = (TextView)convertView.findViewById(R.id.tip_text);
				tempItem.issue_text = (TextView)convertView.findViewById(R.id.issue_number_text);
				tempItem.date_text = (TextView)convertView.findViewById(R.id.release_time_text);
				tempItem.red1_btn = (Button)convertView.findViewById(R.id.Red_Btn_1);
				tempItem.red2_btn = (Button)convertView.findViewById(R.id.Red_Btn_2);
				tempItem.red3_btn = (Button)convertView.findViewById(R.id.Red_Btn_3);
				tempItem.red4_btn = (Button)convertView.findViewById(R.id.Red_Btn_4);
				tempItem.red5_btn = (Button)convertView.findViewById(R.id.Red_Btn_5);
				tempItem.red6_btn = (Button)convertView.findViewById(R.id.Red_Btn_6);
				tempItem.blue_btn = (Button)convertView.findViewById(R.id.Blue_Btn);
				
				convertView.setTag(tempItem);
			}
			else 
			{
				tempItem = (HistoryListItem)convertView.getTag();
			}

			
			Lottery lot = _history.getByIndex(max_count - position - 1);			
			if (lot != null)
			{    
				if (position < 10)
				{
					tempItem.tip_text.setText("隔" + Integer.toString(position) + "期");
					tempItem.tip_text.setVisibility(View.VISIBLE);
				}
				else
				{
					tempItem.tip_text.setVisibility(View.INVISIBLE);
				}

				tempItem.issue_text.setText(Integer.toString(lot.getIssue()));
				tempItem.date_text.setText(lot.getDateExp());
				
				tempItem.red1_btn.setText(lot.getScheme().getRedExp(0));
				tempItem.red2_btn.setText(lot.getScheme().getRedExp(1));
				tempItem.red3_btn.setText(lot.getScheme().getRedExp(2));
				tempItem.red4_btn.setText(lot.getScheme().getRedExp(3));
				tempItem.red5_btn.setText(lot.getScheme().getRedExp(4));
				tempItem.red6_btn.setText(lot.getScheme().getRedExp(5));
				tempItem.blue_btn.setText(lot.getScheme().getBlueExp());
			}
			
			return convertView;
		}
	}

	private ListView lottery_list;
	private HistoryListAdapter list_adapter;
	private Handler handler = new Handler();
	
	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_lottery_history);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);

		// get list control.
		lottery_list = (ListView)findViewById(R.id.history_list);
		list_adapter = new HistoryListAdapter(this.getLayoutInflater());
		
		final TextView btnLoadMore = (TextView) this.getLayoutInflater().inflate(R.layout.list_item_loadmore, lottery_list, false);
		btnLoadMore.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				handler.post(new Runnable() {
					public void run() {
						if (list_adapter.showMore())
						{
							list_adapter.notifyDataSetChanged();
						}
						else
						{
							btnLoadMore.setEnabled(false);
							btnLoadMore.setText("--- 到底了---");
						}
					}
				});
			}
		});
		
		lottery_list.setOnItemClickListener(new OnItemClickListener() {

			@Override
			public void onItemClick(AdapterView<?> adapterView, View view, int position, long id)
			{				
				if (position >= 0)
				{
					History _history = LBDataManager.GetInstance().getHistory();
					if (_history != null)
					{
						int index = _history.getCount() - position - 1;			
						Intent intent = new Intent(LotterySpirit.getInstance(), LotteryDetailActivity.class);
						Bundle bundle = new Bundle();

						bundle.putInt("lottery", index);
						intent.putExtras(bundle);
						
                        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                        
						startActivity(intent);
					}					
				}
			}
		});
		
		lottery_list.addFooterView(btnLoadMore);
		lottery_list.setAdapter(list_adapter);
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	            finish();
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
}
