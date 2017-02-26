package com.fookwin.lotteryspirit;

import com.fookwin.lotterydata.data.Constraint;
import com.fookwin.lotterydata.data.ConstraintTypeEnum;
import com.fookwin.lotterydata.data.HistoryDuplicateConstraint;
import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotterydata.data.RedNumSetConstraint;
import com.fookwin.lotterydata.data.SchemeAttributeConstraint;
import com.fookwin.lotteryspirit.data.HelpCenter;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.fragment.AttributeConstraintFragment;
import com.fookwin.lotteryspirit.fragment.HistoryConstraintFragment;
import com.fookwin.lotteryspirit.fragment.NumSetConstraintFragment;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.app.FragmentTransaction;
import android.content.res.Resources;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

public class ConstraintEditorActivity extends Activity
{
	private int editingIndex = 0;
	private Constraint editing_constraint;
	private Purchase _editingPurchase;	
	
	private Button confirm_button;
	private Button cancel_button;

	private AttributeConstraintFragment attribute_mode;
	private NumSetConstraintFragment numset_mode;
	private HistoryConstraintFragment history_mode;
	
	private TextView status_text;
	private int clr_error_text;
	private int clr_normal_text;
	
	private boolean hasBeenModified = false;
	
	@SuppressLint("HandlerLeak")
	Handler dataChangedhandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) {
			super.handleMessage(msg);	
			
			// refresh the status.
			updateStatusText();
			
			hasBeenModified = true;
		}
	};
	private ImageView helpIcon;
	protected int helpID;
	private ConstraintTypeEnum filterMode;

	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_filter_editor);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		status_text = (TextView) this.findViewById(R.id.status_text);
		confirm_button = (Button) this.findViewById(R.id.confirm_btn);
		confirm_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0) 
			{
				if (hasBeenModified)
					commit(); // commit the changes.
				
				finish();
			}			
		});
		
		cancel_button = (Button) this.findViewById(R.id.cancel_btn);
		cancel_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0) 
			{
				finish();
			}			
		});
		
		Resources res = getResources();
		clr_error_text = res.getColor(R.color.red);
		clr_normal_text = res.getColor(R.color.dimgrey);
		
		// prepare data.
		//
		editing_constraint = null;
		_editingPurchase = LBDataManager.GetInstance().getPendingPurchase();
		
		// read data from bundle.
		Bundle bundle = this.getIntent().getExtras(); 
		filterMode = ConstraintTypeEnum.forValue(bundle.getInt("mode")); 
		editingIndex = bundle.getInt("constraint"); 
		if (editingIndex >= 0)
		{
			Constraint filter = _editingPurchase.getConstraints().get(editingIndex);
			editing_constraint = filter.clone();
		}
		
		// update controls.
		setConstraintMode(filterMode);
		
		helpIcon = (ImageView)findViewById(R.id.helpIcon);
		helpIcon.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0)
			{
				HelpCenter.Instance().Show(helpID);
			}			
		});
	}
	
	private void commit()
	{
		if (editingIndex >= 0)
		{
			// if we are in editing, remove the old one and add the current in same place.
			_editingPurchase.getConstraints().remove(editingIndex);
			_editingPurchase.getConstraints().add(editingIndex, editing_constraint);			
			_editingPurchase.markConstraintRecomputeRequired();
			
			// for any constraint change has to recompute the selection
			_editingPurchase.markSelectorsRecomputeRequired(); 
		}
		else
		{
			// we are working with a newly created constraint, just add it.
			_editingPurchase.getConstraints().add(editing_constraint);			
			_editingPurchase.markConstraintRecomputeRequired();;
		}
	}
	
	private void updateStatusText()
	{
		String error = editing_constraint.HasError();
		String status = editing_constraint.getDisplayExpression();
		
		if (error != "")
		{
			status_text.setText(error);
			status_text.setTextColor(clr_error_text);
			
			confirm_button.setEnabled(false);
		}
		else
		{
			status_text.setText(status);
			status_text.setTextColor(clr_normal_text);
			
			confirm_button.setEnabled(true);
		}
	}

	@Override
	public void onBackPressed() 
	{
		finish();
	}
	
	private void setConstraintMode(ConstraintTypeEnum modeIndex)
	{
		ActionBar actionBar = getActionBar();  
		
		// hide all fragments first.
		FragmentTransaction transaction = getFragmentManager().beginTransaction(); 
		hideFragments(transaction);
		
		// select the corresponding button and show the content.
		switch (modeIndex)
		{
		case SchemeAttributeConstraintType:	
		{			
			if (attribute_mode == null)				
			{
				if (editing_constraint == null)
				{
					editing_constraint = new SchemeAttributeConstraint();
				}

				attribute_mode = new AttributeConstraintFragment();
				attribute_mode.SetConstraint((SchemeAttributeConstraint)editing_constraint);
				attribute_mode.setDataChangedHandler(dataChangedhandler);
				transaction.add(R.id.content_panel, attribute_mode);  
			}
			else
			{
				transaction.show(attribute_mode);
			}
			
			helpID = 35;
			actionBar.setTitle("属性过滤");
			break;
		}
		case RedNumSetConstraintType:
		{
			if (numset_mode == null)				
			{
				if (editing_constraint == null)
				{
					editing_constraint = new RedNumSetConstraint();
				}

				numset_mode = new NumSetConstraintFragment();
				numset_mode.SetConstraint((RedNumSetConstraint) editing_constraint);
				numset_mode.setDataChangedHandler(dataChangedhandler);
				transaction.add(R.id.content_panel, numset_mode);  
			}
			else
			{
				transaction.show(numset_mode);
			}
			
			helpID = 36;
			actionBar.setTitle("号码组过滤");
			break;
		}
		case HistoryDuplicateConstraintType:
		{
			if (history_mode == null)				
			{
				if (editing_constraint == null)
				{
					editing_constraint = new HistoryDuplicateConstraint();
				}

				history_mode = new HistoryConstraintFragment();
				history_mode.SetConstraint((HistoryDuplicateConstraint)editing_constraint);
				history_mode.setDataChangedHandler(dataChangedhandler);
				transaction.add(R.id.content_panel, history_mode);  
			}
			else
			{
				transaction.show(history_mode);
			}
			
			helpID = 37;
			actionBar.setTitle("历史过滤");
			break;
		}
		}
		transaction.commit();
		
		updateStatusText();
	}
	
	private void hideFragments(FragmentTransaction transaction) 
	{
		if (attribute_mode != null) 
		{
			transaction.hide(attribute_mode);
		}
		
		if (numset_mode != null)
		{
			transaction.hide(numset_mode);
		}
		
		if (history_mode != null)
		{
			transaction.hide(history_mode);
		}	
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	        	onBackPressed();
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
}
