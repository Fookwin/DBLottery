package com.fookwin.lotteryspirit.view;

import java.util.ArrayList;
import java.util.HashMap;

import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.TextView;

import com.fookwin.LotterySpirit;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.BonusInfo;
import com.fookwin.lotteryspirit.data.HelpCenter;
import com.fookwin.lotteryspirit.data.LotteryInfo;

public class LotteryDetailView
{
	private View detailView;
	private TextView issueText;
	private TextView releaseTimeText;
	private TextView sellAmountText;
	private TextView poolAmountText;
	private ListView bonusListView;
	private TextView moreInfoView;
	private Button red_Btn_1;
	private Button red_Btn_2;
	private Button red_Btn_3;
	private Button red_Btn_4;
	private Button red_Btn_5;
	private Button red_Btn_6;
	private Button blue_btn;
	
	private LotteryInfo data = null;
	private ImageView helpIcon;
	
	public void addToContainer(LinearLayout container)
	{
		if (detailView == null)
		{
			// create the view tab and add to container.
			detailView = LinearLayout.inflate(LotterySpirit.getInstance(), R.layout.view_lottery_release_detail,null);
			
			// get the controls.
			issueText = (TextView)detailView.findViewById(R.id.issue_number_text);
			releaseTimeText = (TextView)detailView.findViewById(R.id.release_time_text);
			sellAmountText = (TextView)detailView.findViewById(R.id.sell_amount_text);
			poolAmountText = (TextView)detailView.findViewById(R.id.pool_amount_text);
			bonusListView = (ListView)detailView.findViewById(R.id.bonus_detail_list);
			moreInfoView = (TextView)detailView.findViewById(R.id.more_information_view);
			
			red_Btn_1 = (Button)detailView.findViewById(R.id.Red_Btn_1);
			red_Btn_2 = (Button)detailView.findViewById(R.id.Red_Btn_2);
			red_Btn_3 = (Button)detailView.findViewById(R.id.Red_Btn_3);
			red_Btn_4 = (Button)detailView.findViewById(R.id.Red_Btn_4);
			red_Btn_5 = (Button)detailView.findViewById(R.id.Red_Btn_5);
			red_Btn_6 = (Button)detailView.findViewById(R.id.Red_Btn_6);
			blue_btn = (Button)detailView.findViewById(R.id.Blue_Btn);
	
			helpIcon = (ImageView)detailView.findViewById(R.id.helpIcon);
			helpIcon.setOnClickListener(new OnClickListener()
			{
				@Override
				public void onClick(View arg0)
				{
					HelpCenter.Instance().Show(27);
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
	
	public void refreshView()
	{
		if (detailView != null && data != null)
		{	
			issueText.setText(data.Issue);
			releaseTimeText.setText(data.Date);
			sellAmountText.setText(data.BetAmount);
			poolAmountText.setText(data.PoolAmount);
			moreInfoView.setText(data.More);
			
			red_Btn_1.setText(data.Red1);
			red_Btn_2.setText(data.Red2);
			red_Btn_3.setText(data.Red3);
			red_Btn_4.setText(data.Red4);
			red_Btn_5.setText(data.Red5);
			red_Btn_6.setText(data.Red6);
			blue_btn.setText(data.Blue);
			
			// build bonus list.
			ArrayList<HashMap<String, String>> bonuseRowData = new ArrayList<HashMap<String, String>>();  
			for (BonusInfo bonus : data.Winners)  
			{  
			    HashMap<String, String> map = new HashMap<String, String>();  
			    map.put("title", bonus.Name);  
			    map.put("prize", bonus.Bonus);
			    map.put("amount", bonus.Count);
			    bonuseRowData.add(map); 
			}  
	
			SimpleAdapter mBonuseListAdapter = new SimpleAdapter(LotterySpirit.getInstance(), 
					bonuseRowData,
	                R.layout.list_item_bonus_view, 
	                new String[] {"title", "prize", "amount"},   
	                new int[] {R.id.bonus_item_name,R.id.bonus_item_prize, R.id.bonus_item_count});  
			bonusListView.setAdapter(mBonuseListAdapter);
			
			resetListHeight();
		}
	}
	
	private void resetListHeight()
	{
	    ListAdapter listAdapter = bonusListView.getAdapter();
	    if (listAdapter == null) 
	        return;

	    int totalHeight = 0;
	    for (int i = 0, len = listAdapter.getCount(); i < len; i++)
	    {
	        View listItem = listAdapter.getView(i, null, bonusListView);
	        listItem.measure(0, 0); 
	        totalHeight += listItem.getMeasuredHeight();
	    }

	    ViewGroup.LayoutParams params = bonusListView.getLayoutParams();
	    params.height = totalHeight + (bonusListView.getDividerHeight() * (listAdapter.getCount() - 1));
	    bonusListView.setLayoutParams(params);
	}
	
	public void setData(LotteryInfo info)
	{		
		data = info;
	}
}
