package com.fookwin.lotteryspirit.fragment;

import com.fookwin.lotterydata.data.HistoryDuplicateConstraint;
import com.fookwin.lotteryspirit.R;

import android.os.Bundle;
import android.os.Handler;
import android.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.Spinner;

public class HistoryConstraintFragment extends Fragment
{	
	private HistoryDuplicateConstraint constraint;
	private Handler onDataChangedHandler;
	private Spinner ref_issue_count;
	private boolean refreshingFromData;
	private Spinner filter_match_count;
	
	public void setDataChangedHandler(Handler handler)
	{
		onDataChangedHandler = handler;
	}
	
	public HistoryConstraintFragment(HistoryDuplicateConstraint history_constraint) 
	{
		constraint = history_constraint;
	}

	@Override  
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) 
	{		
		View contentLayout = inflater.inflate(R.layout.fragment_history_constraint, container, false);

		ref_issue_count = (Spinner) contentLayout.findViewById(R.id.ref_issue_count);
		ref_issue_count.setOnItemSelectedListener(new OnItemSelectedListener()
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
						constraint.setReferenceCount(-1); // all issues
						break;
					case 1:
						constraint.setReferenceCount(10); // ref latest 10 issues
						break;
					case 2:
						constraint.setReferenceCount(30); // ref latest 30 issues
						break;
					case 3:
						constraint.setReferenceCount(50); // ref latest 50 issues
						break;
					case 4:
						constraint.setReferenceCount(100); // ref latest 100 issues
						break;
					case 5:
						constraint.setReferenceCount(200); // ref latest 200 issues
						break;
					case 6:
						constraint.setReferenceCount(500); // ref latest 500 issues
						break;
					case 7:
						constraint.setReferenceCount(1000); // ref latest 1000 issues
						break;
					}
					if (onDataChangedHandler != null)
						onDataChangedHandler.sendEmptyMessage(0);
				}
				firstTime = false;
			}

			@Override
			public void onNothingSelected(AdapterView<?> arg0) {				
				
			}			
		});
		
		filter_match_count = (Spinner) contentLayout.findViewById(R.id.filter_match_count);
		filter_match_count.setOnItemSelectedListener(new OnItemSelectedListener()
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
						constraint.setExcludeCondition(6); 
						break;
					case 1:
						constraint.setExcludeCondition(5);
						break;
					case 2:
						constraint.setExcludeCondition(4);
						break;
					case 3:
						constraint.setExcludeCondition(3); 
						break;
					case 4:
						constraint.setExcludeCondition(2); 
						break;
					}
					
					if (onDataChangedHandler != null)
						onDataChangedHandler.sendEmptyMessage(0);
				}
				firstTime = false;
			}

			@Override
			public void onNothingSelected(AdapterView<?> arg0) {				
				
			}			
		});
		
		initFromConstraint();
		
		return contentLayout;
	}
	
	private void initFromConstraint()
	{
		refreshingFromData = true;
		
		switch (constraint.getReferenceCount())
		{
		case -1:
			ref_issue_count.setSelection(0);
			break;
		case 10:
			ref_issue_count.setSelection(1); // ref latest 10 issues
			break;
		case 30:
			ref_issue_count.setSelection(2); // ref latest 30 issues
			break;
		case 50:
			ref_issue_count.setSelection(3); // ref latest 50 issues
			break;
		case 100:
			ref_issue_count.setSelection(4); // ref latest 100 issues
			break;
		case 200:
			ref_issue_count.setSelection(5); // ref latest 200 issues
			break;
		case 500:
			ref_issue_count.setSelection(6); // ref latest 500 issues
			break;
		case 1000:
			ref_issue_count.setSelection(7); // ref latest 1000 issues
			break;
		}
		
		switch (constraint.getExcludeCondition())
		{
		case 6:
			filter_match_count.setSelection(0);
			break;
		case 5:
			filter_match_count.setSelection(1);
			break;
		case 4:
			filter_match_count.setSelection(2);
			break;
		case 3:
			filter_match_count.setSelection(3); 
			break;
		case 2:
			filter_match_count.setSelection(4); 
			break;
		}
		
		refreshingFromData = false;
	}
}
