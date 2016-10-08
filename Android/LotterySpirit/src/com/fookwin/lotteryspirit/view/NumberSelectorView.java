package com.fookwin.lotteryspirit.view;

import java.util.ArrayList;

import android.content.res.Resources;
import android.os.Handler;
import android.support.v7.widget.GridLayout;
import android.util.TypedValue;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import android.widget.TextView;

import com.fookwin.LotterySpirit;
import com.fookwin.lotterydata.data.Set;
import com.fookwin.lotterydata.util.StringFormater;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.data.LotteryStateInfo;
import com.fookwin.lotteryspirit.data.NumberStateInfo;

public class NumberSelectorView implements OnClickListener
{
	private enum NumStateEnum
	{
		UNSELECTED,
		SELECTED_RED,
		SELECTED_BLUE,
		SELECTED_DAN,
		EXCLUDED,
	}
	
	public enum SelectModeEnum
	{
		STANDARD_RED,
		STANDARD_BLUE,
		DANTUO_RED,
		RANDOM_RED,
		RANDOM_BLUE
	}
	
	public enum NumInfoEnum
	{
		NONE,
		OMISSION,
		TEMPERATURE,
		DANSHA,
		MARKUP
	}
	
	private class NumberSelectListItem
	{
		int number;
		NumberStateInfo info;
		View cellView;
		Button numButton;
		TextView numInfoText;
		NumStateEnum state = NumStateEnum.UNSELECTED;
		
		private NumStateEnum nextState(SelectModeEnum select_mode)
		{
			switch (state)
			{
			case EXCLUDED:
			{
				state = NumStateEnum.UNSELECTED;
				break;
			}
			case SELECTED_BLUE:
			{
				if (select_mode == SelectModeEnum.RANDOM_BLUE)
					state = NumStateEnum.EXCLUDED;
				else
					state = NumStateEnum.UNSELECTED;
				
				break;
			}
			case SELECTED_DAN:
				state = NumStateEnum.UNSELECTED;
				break;
			case SELECTED_RED:
			{
				if (select_mode == SelectModeEnum.RANDOM_RED)
					state = NumStateEnum.EXCLUDED;
				else if (select_mode == SelectModeEnum.DANTUO_RED)
					state = NumStateEnum.SELECTED_DAN;
				else
					state = NumStateEnum.UNSELECTED;
				
				break;
			}
			case UNSELECTED:
			{
				if (select_mode == SelectModeEnum.STANDARD_BLUE ||
					select_mode == SelectModeEnum.RANDOM_BLUE)
					state = NumStateEnum.SELECTED_BLUE;
				else 
					state = NumStateEnum.SELECTED_RED;
				
				break;
			}
			}
			return state;
		}
	}
	
	private Set numberSet1;
	private Set numberSet2;
	
	private LinearLayout containerLayout;
	private SelectModeEnum select_mode;
	private NumInfoEnum num_info_mode = NumInfoEnum.NONE;
	private ArrayList<NumberSelectListItem> numbers;
	private LotteryStateInfo latestStateInfo;
	
	private Handler onDataChangedHandler;
	
	private int cell_width;
	private int ball_width;
	private int row_count = -1;
	private int num_count = 16;
	private int num_count_in_row = 0;	
	
	// cache resources to be used here.	
	private int dr_num_red;
	private int dr_num_blue;
	private int dr_num_dim;
	private int dr_num_normal;
	private int dr_num_highlight;
	
	private int clr_grey;
	private int clr_white;
	private int clr_red;
	
	public void setNumSet(Set set1, Set set2)
	{
		numberSet1 = set1;
		numberSet2 = set2;
	}
	
	public void setNumInfoMode(NumInfoEnum mode)
	{
		num_info_mode = mode;
	}

	public void addToContainer(View container, SelectModeEnum mode, int screenWidth)
	{
		latestStateInfo = LBDataManager.GetInstance().GetLatestLotteryStateInfo();
		
		containerLayout = (LinearLayout) container;
		
		// calculate the width of unit.
		Resources res = container.getResources();
		ball_width = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 35, res.getDisplayMetrics());		

		select_mode = mode;
		num_count_in_row = screenWidth / (ball_width + 10);
		num_count = forBlue() ? 16 : 33;
		row_count = num_count / num_count_in_row;
		if (num_count % num_count_in_row != 0)
			row_count++;
		
		cell_width = screenWidth / num_count_in_row;		
		
		numbers = new ArrayList<NumberSelectListItem>();
		
		// initialize the global colors used for grid. 		
		dr_num_dim = R.drawable.background_num_button_grey;			
		dr_num_red = R.drawable.background_num_button_red;
		dr_num_blue = R.drawable.background_num_button_blue;
		dr_num_normal = R.drawable.background_num_button_transparent;	
		dr_num_highlight = R.drawable.background_num_button_golden;	

		clr_grey = res.getColor(R.color.grey);
		clr_white = res.getColor(R.color.white);
		clr_red =  res.getColor(R.color.red);
		
		// get list control.
		buildSelectPan();	
	}
	
	public void setDataChangedHandler(Handler handler)
	{
		onDataChangedHandler = handler;
	}
	
	public void refresh()
	{
		for (NumberSelectListItem item : numbers)
		{
			syncItemState(item);
			refreshCell(item);
		}
		
		// refresh from outside, no need to inform parent
		//if (onDataChangedHandler != null)
		//{
			//onDataChangedHandler.sendEmptyMessage(0);
		//}
	}
	
	private void buildSelectPan()	
	{
		for (int row = 0; row < row_count; ++ row)
		{
			// build this row.
			LinearLayout rowView = (LinearLayout) GridLayout.inflate(LotterySpirit.getInstance(), R.layout.view_number_button_row_template, null);

			int count_in_row = Math.min(num_count - row * num_count_in_row, num_count_in_row);
			for (int i = 0, num = row * num_count_in_row + 1; i < count_in_row; ++ i, ++ num)
			{
				NumberSelectListItem item = num > numbers.size() ? null : numbers.get(num - 1);
				if (item == null)
				{
					item = new NumberSelectListItem();
					
					// create a cell view and add it to row.
					item.number = num;
					item.info = forBlue() ? latestStateInfo.BluesStateInfo[num - 1] : latestStateInfo.RedsStateInfo[num - 1];
					item.state = NumStateEnum.UNSELECTED;
					item.cellView = GridLayout.inflate(LotterySpirit.getInstance(), R.layout.view_number_button,null);
					item.numButton = (Button)item.cellView.findViewById(R.id.number_button);
					item.numInfoText = (TextView) item.cellView.findViewById(R.id.number_info);
					
					LayoutParams templp = (LayoutParams) item.numButton.getLayoutParams();
					templp.width = ball_width;
					templp.height = ball_width;
					item.numButton.setLayoutParams(templp);
					
			        // connect to item
			        item.numButton.setTag(item);			        
			        item.numButton.setOnClickListener(this);
			        
					// add to row.
			        LayoutParams params = new LinearLayout.LayoutParams(cell_width, LayoutParams.WRAP_CONTENT);
			        rowView.addView(item.cellView, params);
			        
					numbers.add(item);
				}
				
				syncItemState(item);				
				refreshCell(item);
			}
			
			// add to container.
	        LayoutParams params = new LinearLayout.LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT);
	        containerLayout.addView(rowView, params);
		}
	}

	private void refreshCell(NumberSelectListItem item)
	{		
		String text = StringFormater.padLeft(Integer.toString(item.number), 2, '0');
		
		// set text.
		item.numButton.setText(text); 
		switch (item.state)
		{
		case EXCLUDED:
			item.numButton.setBackgroundResource(dr_num_dim);
			item.numButton.setTextColor(clr_white);
			break;
		case SELECTED_BLUE:
			item.numButton.setBackgroundResource(dr_num_blue);
			item.numButton.setTextColor(clr_white);
			break;
		case SELECTED_DAN:
			item.numButton.setBackgroundResource(dr_num_highlight);
			item.numButton.setTextColor(clr_white);
			break;
		case SELECTED_RED:
			item.numButton.setBackgroundResource(dr_num_red);
			item.numButton.setTextColor(clr_white);
			break;
		case UNSELECTED:
			item.numButton.setBackgroundResource(dr_num_normal);
			item.numButton.setTextColor(clr_grey);
			break;
		default:
			break;
		}
		
		switch (num_info_mode)
		{
		case DANSHA:
		{
			int color = clr_grey;
			String info = "";
			if (item.info.getIncluded())
			{
				info = "µ¨";
				color = clr_red;
			}
			else if (item.info.getExcluded())
			{
				info = "É±";
				color = clr_grey;
			}
				
			item.numInfoText.setText(info);
			item.numInfoText.setTextColor(color);
			item.numInfoText.setVisibility(View.VISIBLE);
			break;
		}
		case NONE:
			item.numInfoText.setVisibility(View.GONE);
			break;
		case TEMPERATURE:
		{
			boolean bBlue = forBlue();
			int temp = item.info.getState().getTemperature();
			
			String info = "";			
			int color = clr_grey;
			
			if ((bBlue && temp >= 2) || (!bBlue && temp >= 4)) 
			{
				info = "ÈÈ";
				color = clr_red;
			}
			else if (temp == 0)
			{
				info = "Àä";
			}
			
			item.numInfoText.setText(info);
			item.numInfoText.setTextColor(color);
			item.numInfoText.setVisibility(View.VISIBLE);
			break;
		}
		case OMISSION:
		{
			boolean bBlue = forBlue();
			int omission = item.info.getState().getOmission();
			int color = ((bBlue && omission > 32) || (!bBlue && omission > 11) )? clr_red : clr_grey;
			
			item.numInfoText.setText(Integer.toString(omission));
			item.numInfoText.setTextColor(color);
			item.numInfoText.setVisibility(View.VISIBLE);
			break;
		}
		case MARKUP:
		{
			Set markedRedsInclude = LBDataManager.GetInstance().getMarkedReds(true);
			Set markedBluesInclude = LBDataManager.GetInstance().getMarkedBlues(true);
			Set markedRedsExclude = LBDataManager.GetInstance().getMarkedReds(false);
			Set markedBluesExclude = LBDataManager.GetInstance().getMarkedBlues(false);
			
			boolean bBlue = forBlue();
			boolean bIncluded = bBlue ? markedBluesInclude.Contains(item.number) : markedRedsInclude.Contains(item.number);
			boolean bExcluded = bBlue ? markedBluesExclude.Contains(item.number) : markedRedsExclude.Contains(item.number);
			String markText = bIncluded || bExcluded ? "±ê" : "";
			int color = bIncluded ? clr_red : clr_grey;
			
			item.numInfoText.setText(markText);
			item.numInfoText.setTextColor(color);
			item.numInfoText.setVisibility(View.VISIBLE);
			break;
		}
		default:
			break;
		
		}
	}
	
	private void syncItemState(NumberSelectListItem item)
	{
		switch (select_mode)
		{
		case DANTUO_RED:
		{
			if (numberSet1.Contains(item.number))
				item.state = NumStateEnum.SELECTED_DAN;
			else if (numberSet2.Contains(item.number))
				item.state = NumStateEnum.SELECTED_RED;
			else
				item.state = NumStateEnum.UNSELECTED;
			break;
		}
		case RANDOM_RED:
		{
			if (numberSet1.Contains(item.number))
				item.state = NumStateEnum.SELECTED_RED;
			else if (numberSet2.Contains(item.number))
				item.state = NumStateEnum.EXCLUDED;
			else
				item.state = NumStateEnum.UNSELECTED;
			break;
		}
		case STANDARD_BLUE:
			item.state = numberSet1.Contains(item.number) ? NumStateEnum.SELECTED_BLUE : NumStateEnum.UNSELECTED;
			break;
		case STANDARD_RED:
			item.state = numberSet1.Contains(item.number) ? NumStateEnum.SELECTED_RED : NumStateEnum.UNSELECTED;
			break;
		case RANDOM_BLUE:
		{
			if (numberSet1.Contains(item.number))
				item.state = NumStateEnum.SELECTED_BLUE;
			else if (numberSet2.Contains(item.number))
				item.state = NumStateEnum.EXCLUDED;
			else
				item.state = NumStateEnum.UNSELECTED;
			break;
		}
		default:
			break;		
		}
	}

	@Override
	public void onClick(View v) 
	{		
		NumberSelectListItem targetItem = (NumberSelectListItem) v.getTag();
		if (targetItem != null)
		{
			// switch button status.
			NumStateEnum newState = targetItem.nextState(select_mode);
			
			// refresh the cell.
			refreshCell(targetItem);
			
			// update the target data.
			switch (select_mode)
			{
			case DANTUO_RED:
			{
				if (newState == NumStateEnum.UNSELECTED) {
					// remove from dan.
					numberSet1.Remove(targetItem.number);
				} else if (newState == NumStateEnum.SELECTED_DAN) {
					// move from tuo to dan
					numberSet2.Remove(targetItem.number);
					numberSet1.Add(targetItem.number);
				} else {
					// add to tuo
					numberSet2.Add(targetItem.number); 
				}

				break;
			}
			case RANDOM_RED:
			{
				if (newState == NumStateEnum.UNSELECTED) {
					// remove from include.
					numberSet2.Remove(targetItem.number);
				} else if (newState == NumStateEnum.SELECTED_RED) {
					// add to include					
					numberSet1.Add(targetItem.number);
				} else {
					// move from include to exclude
					numberSet1.Remove(targetItem.number);
					numberSet2.Add(targetItem.number); 
				}

				break;
			}
			case RANDOM_BLUE:
			{
				if (newState == NumStateEnum.UNSELECTED) {
					// remove from include.
					numberSet2.Remove(targetItem.number);
				} else if (newState == NumStateEnum.SELECTED_BLUE) {
					// add to include					
					numberSet1.Add(targetItem.number);
				} else {
					// move from include to exclude
					numberSet1.Remove(targetItem.number);
					numberSet2.Add(targetItem.number); 
				}

				break;
			}
			case STANDARD_BLUE:
			case STANDARD_RED:
			default:
			{
				if (newState == NumStateEnum.UNSELECTED)
					numberSet1.Remove(targetItem.number);
				else
					numberSet1.Add(targetItem.number);
			}
			}
			
			if (onDataChangedHandler != null)
			{
				onDataChangedHandler.sendEmptyMessage(0);
			}
		}
	}
	
	private boolean forBlue()	
	{
		return (select_mode == SelectModeEnum.STANDARD_BLUE || select_mode == SelectModeEnum.RANDOM_BLUE);
	}
}
