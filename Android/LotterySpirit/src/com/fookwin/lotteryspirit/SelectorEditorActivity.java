package com.fookwin.lotteryspirit;

import com.fookwin.lotterydata.data.DantuoSchemeSelector;
import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotterydata.data.RandomSchemeSelector;
import com.fookwin.lotterydata.data.SchemeSelector;
import com.fookwin.lotterydata.data.SchemeSelectorTypeEnum;
import com.fookwin.lotterydata.data.StandardSchemeSelector;
import com.fookwin.lotteryspirit.data.HelpCenter;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.fragment.DanTuoSelectorFragment;
import com.fookwin.lotteryspirit.fragment.RandomSelectorFragment;
import com.fookwin.lotteryspirit.fragment.StandardSelectorFragment;
import com.fookwin.lotteryspirit.view.NumberSelectorView.NumInfoEnum;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.AlertDialog;
import android.app.FragmentTransaction;
import android.app.Activity;
import android.app.AlertDialog.Builder;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.res.Resources;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

public class SelectorEditorActivity extends Activity 
{
	private int editingIndex = 0;
	private SchemeSelector editing_selector;
	private NumInfoEnum num_info_state = NumInfoEnum.NONE;
	private Purchase _editingPurchase;	
	
	private Button confirm_button;
	private Button cancel_button;	
	private MenuItem number_info_menu_item;
	private StandardSelectorFragment standard_mode;
	private DanTuoSelectorFragment dantuo_mode;
	private RandomSelectorFragment random_mode;
	
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
	private SchemeSelectorTypeEnum selectorMode;
	private boolean isSelectionEntry = false;

	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_scheme_editor);
		
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
				
				Intent intent = new Intent(SelectorEditorActivity.this, SelectorsActivity.class);
				startActivity(intent);
			}			
		});
		
		cancel_button = (Button) this.findViewById(R.id.cancel_btn);
		cancel_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0) 
			{
				exit();
			}			
		});
		
		Resources res = getResources();
		clr_error_text = res.getColor(R.color.red);
		clr_normal_text = res.getColor(R.color.dimgrey);
				
		// prepare data.
		//
		editing_selector = null;
		_editingPurchase = LBDataManager.GetInstance().getPendingPurchase();
		
		// read data from bundle.
		Bundle bundle = this.getIntent().getExtras();
		selectorMode = SchemeSelectorTypeEnum.forValue(bundle.getInt("mode")); 		
		editingIndex = bundle.getInt("selector"); 
		if (editingIndex >= 0)
		{
			// Make a clone of the target selector for modification.
			SchemeSelector selectedSelector = _editingPurchase.getSelectors().get(editingIndex);
			if (selectedSelector != null)
				editing_selector = selectedSelector.clone();
		}
		
		if (bundle.containsKey("isSelectionEntry"))
		{
			isSelectionEntry = bundle.getBoolean("isSelectionEntry");
		}
		
		// update controls.
		setSelectorMode(selectorMode);
		
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
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if (keyCode == KeyEvent.KEYCODE_BACK) {
			exit();
		}
		return super.onKeyDown(keyCode, event);
	}
	
	private void exit() {
		if (isSelectionEntry)
		{
			// if this page is the entry page of selection, confirm with users.
			Builder builder = new AlertDialog.Builder(SelectorEditorActivity.this);
			builder.setTitle("退出选号编辑");
			builder.setMessage("返回后将清空当前选号，确认要退出选号吗？");
			builder.setPositiveButton("确定", new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog, int which) {
					LBDataManager.GetInstance().resetPendingPurchase();
					finish();
				}
			});
			builder.setNegativeButton("不", null);
			builder.create().show();
		}
		else
		{
			// otherwise, close this page immediately.
			finish();
		}
	}
	
	private void commit()
	{
		if (editingIndex >= 0)
		{
			// if we are in editing, remove the old one and add the current in same place.
			_editingPurchase.getSelectors().remove(editingIndex);
			_editingPurchase.getSelectors().add(editingIndex, editing_selector);
			
			_editingPurchase.markSelectorsRecomputeRequired();
		}
		else
		{
			// we are working with a newly created selector, just add it.
			_editingPurchase.getSelectors().add(editing_selector);
			
			_editingPurchase.markSelectorsRecomputeRequired();
		}
	}
	
	private void updateStatusText()
	{
		String error = editing_selector.HasError();
		int count = editing_selector.GetSchemeCount();
		
		if (error != "")
		{
			status_text.setText(error);
			status_text.setTextColor(clr_error_text);
			
			confirm_button.setEnabled(false);
		}
		else
		{
			status_text.setText("共" + Integer.toString(count) + "注");
			status_text.setTextColor(clr_normal_text);
			
			confirm_button.setEnabled(true);
		}
	}

	@Override
	public void onBackPressed() 
	{
		exit();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) 
	{
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.scheme_editor, menu);		

		number_info_menu_item = menu.findItem(R.id.num_info);
		
		return true;
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
			Intent intent = new Intent( this , LotteryHistoryActivity.class );
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		    startActivity( intent );
		    
			return true;
		}
		case R.id.ref_chart:
		{
			Intent intent = new Intent( this , LotteryTrendChartActivity.class );
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		    startActivity( intent );
		    
			return true;
		}
		case R.id.ref_attributes:
		{
			Intent intent = new Intent( this , LotteryAttributeActivity.class );
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		    startActivity( intent );
		    
			return true;
		}
		case R.id.filllist_helper:
		{
			// set the selector as output source.
			ListFillingHelperActivity.setSource(editing_selector);
			
			Intent intent = new Intent( this , ListFillingHelperActivity.class );
			intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		    startActivity( intent );
		    
			return true;
		}
        case android.R.id.home:
        	exit();
            return true;
		}
		
		if (newState != num_info_state)
		{
			num_info_state = newState;
			notifyNumInfoTypeChanged(newState);
			number_info_menu_item.setTitle("号码信息 (" + item.getTitle() + ")");
		}
		
		return super.onOptionsItemSelected(item);
	}
	
	private void notifyNumInfoTypeChanged(NumInfoEnum state)
	{
		switch (selectorMode)
		{
		case DantuoSelectorType:
			dantuo_mode.updateNumbInfo(state);
			break;
		case RandomSelectorType:
			random_mode.updateNumbInfo(state);
			break;
		case StandardSelectorType:
			standard_mode.updateNumbInfo(state);
			break;
		case UploadSelectorType:
		default:
			break;		
		}
	}
	
	private void setSelectorMode(SchemeSelectorTypeEnum mode)
	{
		// hide all fragments first.
		FragmentTransaction transaction = getFragmentManager().beginTransaction(); 
		hideFragments(transaction);
		
		ActionBar actionBar = getActionBar(); 
		
		// select the corresponding button and show the content.
		switch (mode)
		{
		case StandardSelectorType:	
		{			
			if (standard_mode == null)				
			{
				if (editing_selector == null)
				{
					editing_selector = new StandardSchemeSelector();
				}

				standard_mode = new StandardSelectorFragment((StandardSchemeSelector) editing_selector);
				standard_mode.setDataChangedHandler(dataChangedhandler);
				transaction.add(R.id.content_panel, standard_mode);  
			}
			else
			{
				transaction.show(standard_mode);
			}
			
			helpID = 32;		
			standard_mode.updateNumbInfo(num_info_state);
			actionBar.setTitle("标准选号");
			
			break;
		}
		case DantuoSelectorType:
		{
			if (dantuo_mode == null)				
			{
				if (editing_selector == null)
				{
					editing_selector = new DantuoSchemeSelector();
				}
				
				dantuo_mode = new DanTuoSelectorFragment((DantuoSchemeSelector) editing_selector);
				dantuo_mode.setDataChangedHandler(dataChangedhandler);
				transaction.add(R.id.content_panel, dantuo_mode);  
			}
			else
			{
				transaction.show(dantuo_mode);
			}
			
			helpID = 33;
			dantuo_mode.updateNumbInfo(num_info_state);
			actionBar.setTitle("胆拖选号");
			break;
		}
		case RandomSelectorType:
		{
			if (random_mode == null)				
			{
				if (editing_selector == null)
				{
					editing_selector = new RandomSchemeSelector();
				}
				
				random_mode = new RandomSelectorFragment((RandomSchemeSelector) editing_selector);
				random_mode.setDataChangedHandler(dataChangedhandler);
				transaction.add(R.id.content_panel, random_mode);  
			}
			else
			{
				transaction.show(random_mode);
			}
			
			helpID = 34;
			random_mode.updateNumbInfo(num_info_state);
			actionBar.setTitle("智能随机");
			break;
		}
		default:
			break;
		}
		transaction.commit();
		
		updateStatusText();
	}
	
	private void hideFragments(FragmentTransaction transaction) 
	{
		if (standard_mode != null) 
		{
			transaction.hide(standard_mode);
		}
		
		if (dantuo_mode != null)
		{
			transaction.hide(dantuo_mode);
		}
		
		if (random_mode != null)
		{
			transaction.hide(random_mode);
		}	
	}
}
