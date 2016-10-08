package com.fookwin.lotteryspirit.fragment;

import com.fookwin.lotterydata.data.SchemeSelectorTypeEnum;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.SelectorEditorActivity;
import com.fookwin.lotteryspirit.UserCenterActivity;

import android.os.Bundle;
import android.app.ActionBar;
import android.app.Fragment;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.LinearLayout;

public class SelectionFragment extends Fragment
{	
	@Override  
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) 
	{
		// update action bar
		updateActionBar();
		
		View contentLayout = inflater.inflate(R.layout.fragment_selection, container, false);

		LinearLayout createStandardSelector = (LinearLayout)contentLayout.findViewById(R.id.createStandardSelector);
		createStandardSelector.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				startSelector(SchemeSelectorTypeEnum.StandardSelectorType);
			}
		});

		LinearLayout createDantuoSelector = (LinearLayout)contentLayout.findViewById(R.id.createDantuoSelector);
		createDantuoSelector.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				startSelector(SchemeSelectorTypeEnum.DantuoSelectorType);
			}
		});
		
		LinearLayout createRandomSelector = (LinearLayout)contentLayout.findViewById(R.id.createRandomSelector);
		createRandomSelector.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				startSelector(SchemeSelectorTypeEnum.RandomSelectorType);
			}
		});
		
		LinearLayout reuseExistingSolution = (LinearLayout)contentLayout.findViewById(R.id.reuseExistingSolution);
		reuseExistingSolution.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				gotoUserPurchases();
			}
		});
		
		return contentLayout;
	}
	
	private void startSelector(SchemeSelectorTypeEnum mode)
	{
		Intent intent = new Intent(getActivity(), SelectorEditorActivity.class);
		Bundle bundle = new Bundle();

		bundle.putInt("mode", mode.getValue());
		bundle.putInt("selector", -1); // start from a brand new.
		bundle.putBoolean("isSelectionEntry", true);
		intent.putExtras(bundle);
        
		startActivityForResult(intent, 1);	
	}
	
	private void gotoUserPurchases()
	{
		Intent intent = new Intent( this.getActivity(), UserCenterActivity.class );
		intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
	    startActivity( intent );
	}
	
	public void updateActionBar()
	{
		ActionBar actionBar = getActivity().getActionBar();  
		actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);
		actionBar.setTitle("Ñ¡ºÅ¹ýÂË");
		actionBar.setIcon(R.drawable.icon_selection_grey);
	}
}
