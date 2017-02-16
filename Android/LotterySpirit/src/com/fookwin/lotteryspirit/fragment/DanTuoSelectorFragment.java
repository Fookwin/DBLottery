package com.fookwin.lotteryspirit.fragment;

import com.fookwin.lotterydata.data.DantuoSchemeSelector;
import com.fookwin.lotterydata.data.Region;
import com.fookwin.lotterydata.data.Set;
import com.fookwin.lotterydata.util.SelectUtil;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.view.NumberSelectorView;
import com.fookwin.lotteryspirit.view.NumberSelectorView.NumInfoEnum;
import com.fookwin.lotteryspirit.view.NumberSelectorView.SelectModeEnum;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.annotation.SuppressLint;
import android.app.Fragment;
import android.util.DisplayMetrics;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.Spinner;
import android.widget.TextView;

public class DanTuoSelectorFragment extends Fragment
{
	private DantuoSchemeSelector selector;
	private NumInfoEnum num_info_state = NumInfoEnum.NONE;
	
	private int screenWidth;
	private boolean refreshingFromData = false;
	
	private View red_select_pan;
	private View blue_select_pan;
	private TextView red_count_text;
	private TextView dan_count_text;
	private TextView blue_count_text;
	private Spinner random_red_spinner;
	private Spinner random_blue_spinner;
	private View random_red_button;
	private View random_blue_button;
	
	private NumberSelectorView red_select_view;
	private NumberSelectorView blue_select_view;	
	
	
	@SuppressLint("HandlerLeak")
	Handler numSelectionChangedhandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) 
		{
			super.handleMessage(msg);		

			// refresh the status.
			refreshStatus();
			
			// inform the parent updating.
			informParentForChange();
		}
	};
	
	private Handler onDataChangedHandler;
	
	public DanTuoSelectorFragment(DantuoSchemeSelector _selector)
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
		refreshingFromData = true;
		
		View contentLayout = inflater.inflate(R.layout.fragment_dantuo_selector, container, false);		
		red_select_pan = contentLayout.findViewById(R.id.red_select_pan);
		blue_select_pan = contentLayout.findViewById(R.id.blue_select_pan);
		
		red_count_text = (TextView) contentLayout.findViewById(R.id.red_count);
		dan_count_text = (TextView) contentLayout.findViewById(R.id.dan_count);
		blue_count_text = (TextView) contentLayout.findViewById(R.id.blue_count);
		
		random_red_spinner = (Spinner) contentLayout.findViewById(R.id.random_red_count);
		random_red_spinner.setOnItemSelectedListener(new OnItemSelectedListener()
		{
			private boolean firstTime = true;
			
			@Override
			public void onItemSelected(AdapterView<?> arg0, View arg1,
					int arg2, long arg3)
			{
				if (!firstTime && !refreshingFromData)
				{
					selectRandomReds();
				}
				firstTime = false;
			}

			@Override
			public void onNothingSelected(AdapterView<?> arg0) {				
				
			}			
		});
		
		random_blue_spinner = (Spinner) contentLayout.findViewById(R.id.random_blue_count);
		random_blue_spinner.setOnItemSelectedListener(new OnItemSelectedListener()
		{		
			private boolean firstTime = true;
			
			@Override
			public void onItemSelected(AdapterView<?> arg0, View arg1,
					int arg2, long arg3) 
			{
				if (!firstTime && !refreshingFromData)
				{
					selectRandomBlues();
				}
				firstTime = false;
			}

			@Override
			public void onNothingSelected(AdapterView<?> arg0) {				

			}			
		});
		
		random_red_button = contentLayout.findViewById(R.id.random_red_btn);
		random_red_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v)
			{
				selectRandomReds();
			}			
		});
		
		random_blue_button = contentLayout.findViewById(R.id.random_blue_btn);
		random_blue_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				selectRandomBlues();
			}			
		});	
		
        // Get the screen size.
        DisplayMetrics dm = new DisplayMetrics();  
		getActivity().getWindowManager().getDefaultDisplay().getMetrics(dm); 
		screenWidth = dm.widthPixels;
		
		red_select_view = new NumberSelectorView();
		red_select_view.setNumSet(selector.getSelectedDans(), selector.getSelectedTuos());
		red_select_view.setNumInfoMode(num_info_state);
		red_select_view.setDataChangedHandler(numSelectionChangedhandler);
		red_select_view.addToContainer(red_select_pan, SelectModeEnum.DANTUO_RED, screenWidth - 30);
	
		blue_select_view = new NumberSelectorView();
		blue_select_view.setNumSet(selector.getSelectedBlues(), null);
		blue_select_view.setNumInfoMode(num_info_state);
		blue_select_view.setDataChangedHandler(numSelectionChangedhandler);
		blue_select_view.addToContainer(blue_select_pan, SelectModeEnum.STANDARD_BLUE, screenWidth - 30);
		
		// refresh the status.
		refreshStatus();
		
		refreshingFromData = false;
		
		return contentLayout;
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
	
	private void selectRandomReds()
	{
		int toSelectCount = random_red_spinner.getSelectedItemPosition() + 6;		
		int[] danNums = selector.getSelectedDans().getNumbers();
		if (toSelectCount > danNums.length)
		{		
			Set candidates = new Set(new Region(1, 33));
			for (int i = 0; i < danNums.length; ++ i)
			{
				candidates.Remove(danNums[i]);
			}
			
			Set selected = SelectUtil.RandomReds(candidates, toSelectCount - danNums.length);
			
			// refresh the selector.
			selector.setSelectedTuos(selected);
			red_select_view.setNumSet(selector.getSelectedDans(), selector.getSelectedTuos());
			red_select_view.refresh();
			
			refreshStatus();
			
			// inform the parent updating.
			informParentForChange();
		}
	}
	
	private void selectRandomBlues()
	{
		int toSelectCount = random_blue_spinner.getSelectedItemPosition() + 1;
		Set selected = SelectUtil.RandomBlues(new Set(new Region(1, 16)), toSelectCount);
		
		// refresh the selector.
		selector.setSelectedBlues(selected);
		blue_select_view.setNumSet(selector.getSelectedBlues(), null);
		blue_select_view.refresh();
		
		refreshStatus();
		
		// inform the parent updating.
		informParentForChange();
	}
	
	private void refreshStatus()
	{
		refreshingFromData = true;
		
		int danCount = selector.getSelectedDans().getCount();
		int tuoCount = selector.getSelectedTuos().getCount();
		int blueCount = selector.getSelectedBlues().getCount();
		red_count_text.setText(Integer.toString(danCount + tuoCount));
		dan_count_text.setText(Integer.toString(danCount));
		blue_count_text.setText(Integer.toString(blueCount));
		
		refreshingFromData = false;
	}
	
	private void informParentForChange()
	{
		if (onDataChangedHandler != null)
		{				
			onDataChangedHandler.sendEmptyMessage(0);
		}
	}
}
