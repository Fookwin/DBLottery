package com.fookwin.lotteryspirit.fragment;

import com.fookwin.lotterydata.data.RedNumSetConstraint;
import com.fookwin.lotteryspirit.LotteryAttributeActivity;
import com.fookwin.lotteryspirit.LotteryHistoryActivity;
import com.fookwin.lotteryspirit.LotteryTrendChartActivity;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.view.NumberSelectorView;
import com.fookwin.lotteryspirit.view.NumberSelectorView.NumInfoEnum;
import com.fookwin.lotteryspirit.view.NumberSelectorView.SelectModeEnum;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.annotation.SuppressLint;
import android.app.Fragment;
import android.content.Intent;
import android.util.DisplayMetrics;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;

public class NumSetConstraintFragment extends Fragment implements OnClickListener
{	
	private RedNumSetConstraint constraint;
	private NumInfoEnum num_info_state = NumInfoEnum.NONE;
	
	private int screenWidth;
	
	private View red_select_pan;
	private TextView candidate_count_text;
	private TextView hit_count_text;
	
	private NumberSelectorView red_select_view;	
	
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
	private MenuItem number_info_menu_item;
	private Button hit_count_0;
	private Button hit_count_1;
	private Button hit_count_2;
	private Button hit_count_3;
	private Button hit_count_4;
	private Button hit_count_5;
	private Button hit_count_6;	

	public void setDataChangedHandler(Handler handler)
	{
		onDataChangedHandler = handler;
	}

	public NumSetConstraintFragment()
	{
	}

	public void SetConstraint(RedNumSetConstraint numset_constraint)
	{
		constraint = numset_constraint;
	}
	
	@Override  
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) 
	{	
		setHasOptionsMenu(true);
		
		View contentLayout = inflater.inflate(R.layout.fragment_numset_constraint, container, false);		
		red_select_pan = contentLayout.findViewById(R.id.red_select_pan);
		
		candidate_count_text = (TextView) contentLayout.findViewById(R.id.candidate_count);
		hit_count_text= (TextView) contentLayout.findViewById(R.id.hit_count);
	
        // Get the screen size.
        DisplayMetrics dm = new DisplayMetrics();  
		getActivity().getWindowManager().getDefaultDisplay().getMetrics(dm); 
		screenWidth = dm.widthPixels;
		
		red_select_view = new NumberSelectorView();
		red_select_view.setNumSet(constraint.getSelectSet(), null);
		red_select_view.setNumInfoMode(num_info_state);
		red_select_view.setDataChangedHandler(numSelectionChangedhandler);
		red_select_view.addToContainer(red_select_pan, SelectModeEnum.STANDARD_RED, screenWidth - 30);	
		
		initHitCountPan(contentLayout);
				
		// refresh the status.
		refreshStatusAndOptions();
		
		return contentLayout;
	}
	
	private void initHitCountPan(View contentLayout)
	{
		hit_count_0 = (Button) contentLayout.findViewById(R.id.hit_count_0);
		hit_count_0.setSelected(constraint.getHitLimits().Contains(0));
		hit_count_0.setOnClickListener(this);
		hit_count_1 = (Button) contentLayout.findViewById(R.id.hit_count_1);
		hit_count_1.setSelected(constraint.getHitLimits().Contains(1));
		hit_count_1.setOnClickListener(this);
		hit_count_2 = (Button) contentLayout.findViewById(R.id.hit_count_2);
		hit_count_2.setSelected(constraint.getHitLimits().Contains(2));
		hit_count_2.setOnClickListener(this);
		hit_count_3 = (Button) contentLayout.findViewById(R.id.hit_count_3);
		hit_count_3.setSelected(constraint.getHitLimits().Contains(3));
		hit_count_3.setOnClickListener(this);
		hit_count_4 = (Button) contentLayout.findViewById(R.id.hit_count_4);
		hit_count_4.setSelected(constraint.getHitLimits().Contains(4));
		hit_count_4.setOnClickListener(this);
		hit_count_5 = (Button) contentLayout.findViewById(R.id.hit_count_5);
		hit_count_5.setSelected(constraint.getHitLimits().Contains(5));
		hit_count_5.setOnClickListener(this);
		hit_count_6 = (Button) contentLayout.findViewById(R.id.hit_count_6);
		hit_count_6.setSelected(constraint.getHitLimits().Contains(6));
		hit_count_6.setOnClickListener(this);
	}
	
	@Override
	public void onCreateOptionsMenu(Menu menu, MenuInflater inflater)
	{
		// Inflate the menu; this adds items to the action bar if it is present.
		inflater.inflate(R.menu.scheme_editor, menu);		

		number_info_menu_item = menu.findItem(R.id.num_info);
		MenuItem fillform_item = menu.findItem(R.id.assistants).getSubMenu().findItem(R.id.filllist_helper);
		fillform_item.setVisible(false); // no form filling helper available.
		
		super.onCreateOptionsMenu(menu, inflater);
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item)
	{
		NumInfoEnum newState = num_info_state;
		switch (item.getItemId())
		{
		case R.id.none_info:
			newState = NumInfoEnum.NONE;
			break;
		case R.id.omission_info:
			newState = NumInfoEnum.OMISSION;
			break;
		case R.id.dansha_info:
			newState = NumInfoEnum.DANSHA;
			break;
		case R.id.temperature_info:
			newState = NumInfoEnum.TEMPERATURE;	
			break;
		case R.id.mark_info:
			newState = NumInfoEnum.MARKUP;
			break;
		case R.id.ref_history:
		{
			Intent intent = new Intent( this.getActivity() , LotteryHistoryActivity.class );
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		    startActivity( intent );
		    
			return true;
		}
		case R.id.ref_chart:
		{
			Intent intent = new Intent( this.getActivity() , LotteryTrendChartActivity.class );
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		    startActivity( intent );
		    
			return true;
		}
		case R.id.ref_attributes:
		{
			Intent intent = new Intent( this.getActivity() , LotteryAttributeActivity.class );
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		    startActivity( intent );
		    
			return true;
		}
		}
		
		if (newState != num_info_state)
		{
			num_info_state = newState;
			updateNumbInfo(newState);
			number_info_menu_item.setTitle("号码信息 (" + item.getTitle() + ")");
		}
		
		return super.onOptionsItemSelected(item);
	}
	
	public void updateNumbInfo(NumInfoEnum type)
	{
		num_info_state = type;
		
		if (red_select_view != null)
		{
			red_select_view.setNumInfoMode(type);
			red_select_view.refresh();
		}
	}
	
	private void refreshStatusAndOptions()
	{
		candidate_count_text.setText(Integer.toString(constraint.getSelectSet().getCount()));
		hit_count_text.setText(constraint.getHitLimits().getDisplayExpression());
	}
	
	private void informParentForChange()
	{
		if (onDataChangedHandler != null)
		{				
			onDataChangedHandler.sendEmptyMessage(0);
		}
	}

	@Override
	public void onClick(View v)
	{
		switch (v.getId())
		{
		case R.id.hit_count_0:
		{
			hit_count_0.setSelected(!hit_count_0.isSelected());
			
			if (hit_count_0.isSelected())
				constraint.getHitLimits().Add(0);
			else
				constraint.getHitLimits().Remove(0);
			break;
		}
		case R.id.hit_count_1:
		{
			hit_count_1.setSelected(!hit_count_1.isSelected());
			
			if (hit_count_1.isSelected())
				constraint.getHitLimits().Add(1);
			else
				constraint.getHitLimits().Remove(1);
			break;
		}
		case R.id.hit_count_2:
		{
			hit_count_2.setSelected(!hit_count_2.isSelected());
			
			if (hit_count_2.isSelected())
				constraint.getHitLimits().Add(2);
			else
				constraint.getHitLimits().Remove(2);
			break;
		}
		case R.id.hit_count_3:
		{
			hit_count_3.setSelected(!hit_count_3.isSelected());
			
			if (hit_count_3.isSelected())
				constraint.getHitLimits().Add(3);
			else
				constraint.getHitLimits().Remove(3);
			break;
		}
		case R.id.hit_count_4:
		{
			hit_count_4.setSelected(!hit_count_4.isSelected());
			
			if (hit_count_4.isSelected())
				constraint.getHitLimits().Add(4);
			else
				constraint.getHitLimits().Remove(4);
			break;
		}
		case R.id.hit_count_5:
		{
			hit_count_5.setSelected(!hit_count_5.isSelected());
			
			if (hit_count_5.isSelected())
				constraint.getHitLimits().Add(5);
			else
				constraint.getHitLimits().Remove(5);
			break;
		}
		case R.id.hit_count_6:
		{
			hit_count_6.setSelected(!hit_count_6.isSelected());
			
			if (hit_count_6.isSelected())
				constraint.getHitLimits().Add(6);
			else
				constraint.getHitLimits().Remove(6);
			break;
		}
		default:
			return;
		}
		
		// notify the changes.
		numSelectionChangedhandler.sendEmptyMessage(0);
	}
}
