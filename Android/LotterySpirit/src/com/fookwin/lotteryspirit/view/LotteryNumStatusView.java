package com.fookwin.lotteryspirit.view;

import android.content.res.Resources;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.util.StringFormater;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.HelpCenter;
import com.fookwin.lotteryspirit.data.LotteryStateInfo;
import com.fookwin.lotteryspirit.data.NumberStateInfo;

public class LotteryNumStatusView
{
	private View view;
	private LinearLayout hot_red_view;
	private LinearLayout cool_red_view;
	private LinearLayout dan_red_view;
	private LinearLayout kill_red_view;
	private LinearLayout hot_blue_view;
	private LinearLayout cool_blue_view;
	private LinearLayout dan_blue_view;
	private LinearLayout kill_blue_view;
	
	private int num_ball_width = -1;
	
	private LotteryStateInfo data = null;
	private ImageView helpIcon;
	
	public void addToContainer(LinearLayout container)
	{
		if (num_ball_width < 0)
		{
			Resources res = container.getResources();
			num_ball_width = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 25, res.getDisplayMetrics());
		}
		
		if (view == null)
		{
			// create the view tab and add to container.
			view = LinearLayout.inflate(LotterySpirit.getInstance(), R.layout.view_number_status,null);
			
			// get the controls.
			hot_red_view = (LinearLayout)view.findViewById(R.id.numset_hot_red);
			cool_red_view = (LinearLayout)view.findViewById(R.id.numset_cool_red);
			dan_red_view = (LinearLayout)view.findViewById(R.id.numset_dan_red);
			kill_red_view = (LinearLayout)view.findViewById(R.id.numset_kill_red);
			hot_blue_view = (LinearLayout)view.findViewById(R.id.numset_hot_blue);
			cool_blue_view = (LinearLayout)view.findViewById(R.id.numset_cool_blue);
			dan_blue_view = (LinearLayout)view.findViewById(R.id.numset_dan_blue);
			kill_blue_view = (LinearLayout)view.findViewById(R.id.numset_kill_blue);
			
			helpIcon = (ImageView)view.findViewById(R.id.helpIcon);
			helpIcon.setOnClickListener(new OnClickListener()
			{
				@Override
				public void onClick(View arg0)
				{
					HelpCenter.Instance().Show(28);
				}			
			});
		}
		
		if (data != null)
		{
			refreshView();
		}
		
		LayoutParams params = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT);	    
		container.addView(view, params);
	}
	
	public void refreshView()
	{
		if (view != null && data != null)
		{	
			LayoutParams params = new LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
			params.gravity = Gravity.CENTER;
			params.setMargins(2, 2, 2, 2);
			
			for (NumberStateInfo num_info : data.RedsStateInfo)
			{			
				// hot red?
				if (num_info.getState().getTemperature() >= 4)
				{						
					hot_red_view.addView(buildNumButton(num_info.getNumber(), false), params);
				}
				else if (num_info.getState().getTemperature() == 0) // cool red?
				{					
					cool_red_view.addView(buildNumButton(num_info.getNumber(), false), params);
				}
				
				// dan red?
				if (num_info.getIncluded())
				{					
					dan_red_view.addView(buildNumButton(num_info.getNumber(), false), params);
				}
				else if (num_info.getExcluded()) // to kill
				{					
					kill_red_view.addView(buildNumButton(num_info.getNumber(), false), params);
				}
			}
			
			for (NumberStateInfo num_info : data.BluesStateInfo)
			{	
				// hot blue?
				if (num_info.getState().getTemperature() >= 2)
				{						
					hot_blue_view.addView(buildNumButton(num_info.getNumber(), true), params);
				}
				else if (num_info.getState().getTemperature() == 0) // cool blue?
				{					
					cool_blue_view.addView(buildNumButton(num_info.getNumber(), true), params);
				}
				
				// dan blue?
				if (num_info.getIncluded())
				{					
					dan_blue_view.addView(buildNumButton(num_info.getNumber(), true), params);
				}
				else if (num_info.getExcluded()) // to kill
				{					
					kill_blue_view.addView(buildNumButton(num_info.getNumber(), true), params);
				}
			}
		}
	}
	
	private View buildNumButton(int num, boolean isBlue)
	{		
		String num_text = StringFormater.padLeft(Integer.toString(num), 2, '0');
		
		View btnView = LinearLayout.inflate(LotterySpirit.getInstance(), R.layout.view_number_button,null);
		Button btn = (Button)btnView.findViewById(R.id.number_button);
		btn.setBackgroundResource(isBlue ? R.drawable.background_num_button_blue : R.drawable.background_num_button_red);
		btn.setText(num_text);
		
		btn.setFocusable(false);
		
		return btnView;
	}
	
	public void setData(LotteryStateInfo info)
	{		
		data = info;
	}
}
