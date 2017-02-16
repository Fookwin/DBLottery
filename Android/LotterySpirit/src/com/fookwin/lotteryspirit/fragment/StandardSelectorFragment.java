package com.fookwin.lotteryspirit.fragment;

import com.fookwin.lotterydata.data.Region;
import com.fookwin.lotterydata.data.Set;
import com.fookwin.lotterydata.data.StandardSchemeSelector;
import com.fookwin.lotterydata.data.StandardSchemeSelector.RedBlueConnectionTypeEnum;
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
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.Spinner;
import android.widget.TextView;

public class StandardSelectorFragment extends Fragment
{
	private StandardSchemeSelector selector;
	private NumInfoEnum num_info_state = NumInfoEnum.NONE;
	
	private int screenWidth;
	private boolean refreshingFromData = false;
	
	private View red_select_pan;
	private View blue_select_pan;
	private TextView red_count_text;
	private TextView blue_count_text;
	private Spinner random_red_spinner;
	private Spinner random_blue_spinner;
	private View random_red_button;
	private View random_blue_button;
	private Spinner blue_connection_spinner;
	private CheckBox enable_matrix_filter;
	
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
			refreshStatusAndOptions();
			
			// inform the parent updating.
			informParentForChange();
		}
	};
	
	private Handler onDataChangedHandler;
	private View blue_connection_view;
	
	public StandardSelectorFragment(StandardSchemeSelector starndard_selector)
	{
		selector = starndard_selector;
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
		
		View contentLayout = inflater.inflate(R.layout.fragment_standard_selector, container, false);		
		red_select_pan = contentLayout.findViewById(R.id.red_select_pan);
		blue_select_pan = contentLayout.findViewById(R.id.blue_select_pan);
		
		red_count_text = (TextView) contentLayout.findViewById(R.id.red_count);
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
		
		blue_connection_view = contentLayout.findViewById(R.id.blue_connection_view);
		blue_connection_view.setVisibility(View.GONE); // TODO: disable this option.
		
		blue_connection_spinner = (Spinner) contentLayout.findViewById(R.id.blue_connection_type);
		blue_connection_spinner.setOnItemSelectedListener(new OnItemSelectedListener()
		{	
			private boolean firstTime = true;
			@Override
			public void onItemSelected(AdapterView<?> arg0, View arg1,
					int position, long arg3)
			{
				if (!firstTime && !refreshingFromData)
				{
					switch (position)
					{
					case 0:
						selector.setBlueConnectionType(RedBlueConnectionTypeEnum.Duplicate);
						break;
					case 1:
						selector.setBlueConnectionType(RedBlueConnectionTypeEnum.OneToOneInOrder);
						break;
					case 2:
						selector.setBlueConnectionType(RedBlueConnectionTypeEnum.OneToOneInRandom);
						break;
					}
					
					// inform the parent updating.
					informParentForChange();
				}
				firstTime = false;
			}

			@Override
			public void onNothingSelected(AdapterView<?> arg0) {				

			}			
		});
		
		enable_matrix_filter = (CheckBox) contentLayout.findViewById(R.id.enable_matrix_filter);
		enable_matrix_filter.setOnCheckedChangeListener(new OnCheckedChangeListener()
		{			
			@Override
			public void onCheckedChanged(CompoundButton arg0, boolean arg1)
			{
				if (!refreshingFromData)
				{
					selector.setApplyMatrixFilter(enable_matrix_filter.isChecked());
					
					// inform the parent updating.
					informParentForChange();
				}
			}			
		});
		
        // Get the screen size.
        DisplayMetrics dm = new DisplayMetrics();  
		getActivity().getWindowManager().getDefaultDisplay().getMetrics(dm); 
		screenWidth = dm.widthPixels;
		
		red_select_view = new NumberSelectorView();
		red_select_view.setNumSet(selector.getSelectedReds(), null);
		red_select_view.setNumInfoMode(num_info_state);
		red_select_view.setDataChangedHandler(numSelectionChangedhandler);
		red_select_view.addToContainer(red_select_pan, SelectModeEnum.STANDARD_RED, screenWidth - 30);
	
		blue_select_view = new NumberSelectorView();
		blue_select_view.setNumSet(selector.getSelectedBlues(), null);
		blue_select_view.setNumInfoMode(num_info_state);
		blue_select_view.setDataChangedHandler(numSelectionChangedhandler);
		blue_select_view.addToContainer(blue_select_pan, SelectModeEnum.STANDARD_BLUE, screenWidth - 30);
		
		// refresh the status.
		refreshStatusAndOptions();
		
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
		Set selected = SelectUtil.RandomReds(new Set(new Region(1, 33)), toSelectCount);
		
		// refresh the selector.
		selector.setSelectedReds(selected);
		red_select_view.setNumSet(selector.getSelectedReds(), null);
		red_select_view.refresh();
		
		refreshStatusAndOptions();
		
		// inform the parent updating.
		informParentForChange();
	}
	
	private void selectRandomBlues()
	{
		int toSelectCount = random_blue_spinner.getSelectedItemPosition() + 1;
		Set selected = SelectUtil.RandomBlues(new Set(new Region(1, 16)), toSelectCount);
		
		// refresh the selector.
		selector.setSelectedBlues(selected);
		blue_select_view.setNumSet(selector.getSelectedBlues(), null);
		blue_select_view.refresh();
		
		refreshStatusAndOptions();
		
		// inform the parent updating.
		informParentForChange();
	}
	
	private void refreshStatusAndOptions()
	{
		refreshingFromData = true;
		
		int redCount = selector.getSelectedReds().getCount();
		int blueCount = selector.getSelectedBlues().getCount();
		red_count_text.setText(Integer.toString(redCount));
		blue_count_text.setText(Integer.toString(blueCount));
		
		// update options status.
		int blueConnectionIndex = 0;
		if (blueCount > 1)
		{
			switch (selector.getBlueConnectionType())
			{
			case Duplicate:
				blueConnectionIndex = 0;
				break;
			case OneToOneInOrder:
				blueConnectionIndex = 1;
				break;
			case OneToOneInRandom:
				blueConnectionIndex = 2;
				break;
			default:
				break;		
			}
		}
		blue_connection_spinner.setSelection(blueConnectionIndex);
		blue_connection_spinner.setEnabled(blueCount > 1);	
		blue_connection_view.setEnabled(blueCount > 1);
		
		boolean checkMF = selector.getApplyMatrixFilter() && redCount > 6;
		enable_matrix_filter.setEnabled(redCount > 6);
		enable_matrix_filter.setChecked(checkMF);
		
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
