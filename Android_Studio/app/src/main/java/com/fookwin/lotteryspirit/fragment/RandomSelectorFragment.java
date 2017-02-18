package com.fookwin.lotteryspirit.fragment;

import com.fookwin.lotterydata.data.RandomSchemeSelector;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.view.NumberEditor;
import com.fookwin.lotteryspirit.view.NumberSelectorView;
import com.fookwin.lotteryspirit.view.SelectionListView;
import com.fookwin.lotteryspirit.view.NumberSelectorView.NumInfoEnum;
import com.fookwin.lotteryspirit.view.NumberSelectorView.SelectModeEnum;
import com.fookwin.lotteryspirit.view.SelectionListView.SelectionTypeEnum;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.annotation.SuppressLint;
import android.app.Fragment;
import android.util.DisplayMetrics;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ListView;
import android.widget.TextView;

public class RandomSelectorFragment extends Fragment
{
	private RandomSchemeSelector selector;
	private NumInfoEnum num_info_state = NumInfoEnum.NONE;
	
	private int screenWidth;
	
	private View red_select_pan;
	private View blue_select_pan;
	private TextView red_dan_count_text;
	private TextView red_exclude_count_text;
	private TextView blue_dan_count_text;
	private TextView blue_exclude_count_text;
	
	private NumberSelectorView red_select_view;
	private NumberSelectorView blue_select_view;
	private NumberEditor random_count_editor;
	
	
	@SuppressLint("HandlerLeak")
	Handler numSelectionChangedhandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) 
		{
			super.handleMessage(msg);		

			// the condition has been changed, recalculate the result.
			if (selector.HasError() == "")
			{
				selector.random(true);
				
				updateList();
			}
			
			// refresh the status.
			refreshStatusAndOptions();
			
			// inform the parent updating.
			informParentForChange();
		}
	};
	
	@SuppressLint("HandlerLeak")
	Handler randomCountChangedhandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) {
			super.handleMessage(msg);	
			
			selector.setSelectedCount(msg.what);
			
			if (selector.HasError() == "")
			{
				selector.random(false); // no need to recalculate
				
				updateList();
				informParentForChange();
			}
		}
	};
	
	@SuppressLint("HandlerLeak")
	Handler randomButtonClickedHandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) 
		{
			super.handleMessage(msg);
			
			if (selector.HasError() == "")
			{
				selector.random(true); // recalculate
			
				updateList();			
				informParentForChange();
			}
		}
	};
	
	private Handler onDataChangedHandler;
	private SelectionListView result_list_helper;
	private ListView result_list;
	
	public RandomSelectorFragment(RandomSchemeSelector _selector)
	{
		selector = _selector;
	}
	
	public void setDataChangedHandler(Handler handler)
	{
		onDataChangedHandler = handler;
	}
	
	@Override  
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) 
	{		
		View contentLayout = inflater.inflate(R.layout.fragment_random_selector, container, false);		
		red_select_pan = contentLayout.findViewById(R.id.red_select_pan);
		blue_select_pan = contentLayout.findViewById(R.id.blue_select_pan);
		
		red_dan_count_text = (TextView) contentLayout.findViewById(R.id.dan_red_count);
		red_exclude_count_text= (TextView) contentLayout.findViewById(R.id.exclude_red_count);
		blue_dan_count_text= (TextView) contentLayout.findViewById(R.id.dan_blue_count);
		blue_exclude_count_text= (TextView) contentLayout.findViewById(R.id.exclude_blue_count);
		
		random_count_editor = (NumberEditor) contentLayout.findViewById(R.id.random_count_editor);
		random_count_editor.setRegion(1, 100);
		random_count_editor.setTitle("点击换一组");
		random_count_editor.setUnit("注");
		random_count_editor.setValue(selector.getSelectedCount());
		random_count_editor.setOnValueChangedHandler(randomCountChangedhandler);
		random_count_editor.setOnValueNameClickedHandler(randomButtonClickedHandler);
		random_count_editor.setClickable(true);
		
		result_list = (ListView) contentLayout.findViewById(R.id.result_list);
		
        // Get the screen size.
        DisplayMetrics dm = new DisplayMetrics();  
		getActivity().getWindowManager().getDefaultDisplay().getMetrics(dm); 
		screenWidth = dm.widthPixels;
		
		red_select_view = new NumberSelectorView();
		red_select_view.setNumSet(selector.getIncludedReds(), selector.getExcludedReds());
		red_select_view.setNumInfoMode(num_info_state);
		red_select_view.setDataChangedHandler(numSelectionChangedhandler);
		red_select_view.addToContainer(red_select_pan, SelectModeEnum.RANDOM_RED, screenWidth - 30);
	
		blue_select_view = new NumberSelectorView();
		blue_select_view.setNumSet(selector.getIncludedBlues(), selector.getExcludedBlues());
		blue_select_view.setNumInfoMode(num_info_state);
		blue_select_view.setDataChangedHandler(numSelectionChangedhandler);
		blue_select_view.addToContainer(blue_select_pan, SelectModeEnum.RANDOM_BLUE, screenWidth - 30);		
		
		if (selector.HasError() == "")
		{
			// make sure the result is up-to-date
			selector.random(false);
			informParentForChange();
		}
		
		// update the list.
		updateList();
		
		// refresh the status.
		refreshStatusAndOptions();
		
		return contentLayout;
	}
	
	private void updateList()
	{
		// update the result.
		if (result_list_helper != null)
		{
			result_list_helper.refresh();
		}
		else
		{
			result_list_helper = new SelectionListView(SelectionTypeEnum.SCHEME);
			result_list_helper.setDeletable(false);
			result_list_helper.buildList(this.getActivity(), result_list, selector.getResultList(), true);
		}
		
		result_list_helper.resetListHeight();
	}
	
	public void updateNumbInfo(NumInfoEnum type)
	{
		num_info_state = type;
		
		if (red_select_view != null)
		{
			red_select_view.setNumInfoMode(type);
			red_select_view.refresh();
			blue_select_view.setNumInfoMode(type);	
			blue_select_view.refresh();
		}
	}
	
	private void refreshStatusAndOptions()
	{
		int redIncludeCount = selector.getIncludedReds().getCount();
		int redExcludeCount = selector.getExcludedReds().getCount();		

		red_dan_count_text.setText(Integer.toString(redIncludeCount));
		red_exclude_count_text.setText(Integer.toString(redExcludeCount));
		
		int blueIncludeCount = selector.getIncludedBlues().getCount();
		int blueExcludeCount = selector.getExcludedBlues().getCount();

		blue_dan_count_text.setText(Integer.toString(blueIncludeCount));
		blue_exclude_count_text.setText(Integer.toString(blueExcludeCount));
	}
	
	private void informParentForChange()
	{
		if (onDataChangedHandler != null)
		{				
			onDataChangedHandler.sendEmptyMessage(0);
		}
	}
}
