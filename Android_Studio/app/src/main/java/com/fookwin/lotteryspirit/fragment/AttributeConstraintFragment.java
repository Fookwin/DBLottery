package com.fookwin.lotteryspirit.fragment;

import com.fookwin.lotterydata.data.SchemeAttribute;
import com.fookwin.lotterydata.data.SchemeAttributeConstraint;
import com.fookwin.lotterydata.data.SchemeAttributeValueStatus;
import com.fookwin.lotterydata.data.Set;
import com.fookwin.lotteryspirit.AttributeDetailActivity;
import com.fookwin.lotteryspirit.AttributeFilterOptionActivity;
import com.fookwin.lotteryspirit.LotteryHistoryActivity;
import com.fookwin.lotteryspirit.LotteryTrendChartActivity;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.view.ConstraintListView;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.Fragment;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ExpandableListView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;

public class AttributeConstraintFragment extends Fragment
{	
	private SchemeAttributeConstraint constraint;
	private ConstraintListView constraintListView;
	
	private LayoutInflater layout_inflater;
	
	private ListView recommended_filter_list;
	private ExpandableListView full_filter_list;
	
	private Handler onDataChangedHandler;
	private MenuItem constraint_filter_menu;
	private LinearLayout selected_attribute_panel;
	private TextView selected_attribute;
	private MenuItem filter_option_menu;
	private ImageView goto_detail_icon;
	private boolean bShowAllAttributes = false;
		
	public void setDataChangedHandler(Handler handler)
	{
		onDataChangedHandler = handler;
	}

	public AttributeConstraintFragment()
	{
	}

	public void SetConstraint(SchemeAttributeConstraint attribute_constraint)
	{
		constraint = attribute_constraint;
	}

	@SuppressLint("HandlerLeak")
	@Override  
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) 
	{		
		setHasOptionsMenu(true);
		
		layout_inflater = inflater;		
	
		View view = inflater.inflate(R.layout.fragment_attribute_constraint, container, false);
		
		selected_attribute_panel = (LinearLayout)view.findViewById(R.id.selected_attribute_panel);
		selected_attribute_panel.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{	
				String selKey = constraint.getAttributeKey();
				if (!selKey.isEmpty())
					gotoConstraintDetail(selKey);
			}			
		});
		
		selected_attribute = (TextView)view.findViewById(R.id.selected_attribute);
		goto_detail_icon = (ImageView)view.findViewById(R.id.goto_detail_icon);
		
        // initialize the tab content.
        recommended_filter_list = (ListView)view.findViewById(R.id.recommend_filter_list); 
        full_filter_list = (ExpandableListView)view.findViewById(R.id.filter_category_list);
        
        constraintListView = new ConstraintListView();
        constraintListView.setOnItemClickedHandler(new Handler()
        {
    		@Override
    		public void handleMessage(Message msg) 
    		{
    			super.handleMessage(msg);
    			
    			Bundle data = msg.getData();
    			String attributeKey = data.getString("key");
    			if (attributeKey != null)
    			{
    				gotoConstraintDetail(attributeKey);
    			}
    		}
        });
        
        String expression = constraint.getDisplayExpression();
        if (expression.isEmpty())
        {
        	expression = "请选择一个属性过滤条件";
        	goto_detail_icon.setVisibility(View.GONE);
        }
        
        selected_attribute.setText(expression);
       
        switchConstraintView(true);

		return view;
	}	
	
	private void gotoConstraintDetail(String attributeKey)
	{
		Intent intent = new Intent(getActivity(), AttributeDetailActivity.class);
		intent.putExtra("key", attributeKey);
		
		if (constraint.getAttributeKey() == attributeKey)
		{
			SchemeAttribute attribute = LBDataManager.GetInstance().getLastAttributes().Attribute(attributeKey);
			
			Set selectedIndices = new Set();
			Set selectedValues = constraint.getValues();
			int index = 1;
			for (SchemeAttributeValueStatus state : attribute.getValueStates())
			{
				if (selectedValues.Contains(state.getValueRegion()))
					selectedIndices.Add(index);
				
				index ++;
			}
			intent.putExtra("selected", selectedIndices.toString());
		}
		else
			intent.putExtra("selected", "");
        
        startActivityForResult(intent, 2);
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) 
	{
		super.onActivityResult(requestCode, resultCode, data);
		
		if (requestCode == 1) // for option change.
		{
			// update the recommended list in case the filter option has been just changed.
			constraintListView.refreshRecommendedList();
		}		
		else if (requestCode == 2) // for attribute filter change.
		{
			if (resultCode == Activity.RESULT_OK)
			{
				// Get the selection from intent data.
				String key = data.getStringExtra("key");
				SchemeAttribute attribute = LBDataManager.GetInstance().getLastAttributes().Attribute(key);
				
				Set selectedIndices = new Set(data.getStringExtra("selected"));
				constraint.setAttribute(key, attribute.getDisplayName());
				
				Set selectedValues = new Set();
				int index = 1;
				for (SchemeAttributeValueStatus state : attribute.getValueStates())
				{
					if (selectedIndices.Contains(index))
						selectedValues.Add(state.getValueRegion());
					
					index ++;
				}				
				constraint.setValues(selectedValues);
	
				// update the selected attribute in case it has been changed just.
				selected_attribute.setText(constraint.getDisplayExpression());
				
				goto_detail_icon.setVisibility(View.VISIBLE);
				
				// inform the parent for any change.
				if (onDataChangedHandler != null)
				{
					onDataChangedHandler.sendEmptyMessage(0);
				}
			}
		}
	}

	@Override
	public void onCreateOptionsMenu(Menu menu, MenuInflater inflater)
	{
		inflater.inflate(R.menu.constraint_display_filters, menu);
		
		constraint_filter_menu = menu.findItem(R.id.constraint_filter);
		filter_option_menu = menu.findItem(R.id.edit_filter_option);
		
		super.onCreateOptionsMenu(menu, inflater);
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item)
	{
		switch (item.getItemId())
		{
		case R.id.constraint_filter:
		{
			if (bShowAllAttributes)
			{
				switchConstraintView(true);			
				filter_option_menu.setVisible(true);
				bShowAllAttributes = false;
				constraint_filter_menu.setTitle("显示所有");
			}
			else
			{
				switchConstraintView(false);
				filter_option_menu.setVisible(false);
				bShowAllAttributes = true;
				constraint_filter_menu.setTitle("显示推荐");
			}
			break;
		}
		case R.id.edit_filter_option:
		{
			// goto the filter option setting.
			Intent intent = new Intent(getActivity(), AttributeFilterOptionActivity.class);			
            //intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);           
            startActivityForResult(intent, 1);
            
			return false;
		}
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
		default:
			return false;
		}
		
		constraint_filter_menu.setTitle(item.getTitle());
		return false;
	}
	
	public void switchConstraintView(boolean showRecommended) 
	{
		if (showRecommended)
		{
			recommended_filter_list.setVisibility(View.VISIBLE);
			full_filter_list.setVisibility(View.INVISIBLE);
			
			constraintListView.initRecommendedFilterList(layout_inflater, recommended_filter_list);
		}
		else 
		{
			recommended_filter_list.setVisibility(View.INVISIBLE);
			full_filter_list.setVisibility(View.VISIBLE);
			
			constraintListView.initFullFilterList(layout_inflater, full_filter_list);
		}
	}
}
