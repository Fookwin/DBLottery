package com.fookwin.lotteryspirit;

import com.fookwin.lotterydata.util.DataUtil;
import com.fookwin.lotteryspirit.data.FilterOption;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.util.NotificationUtil;
import com.fookwin.lotteryspirit.view.NumberEditor;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;

public class AttributeFilterOptionActivity extends Activity 
{
	private FilterOption option;
	
	private NumberEditor happen_prop_editor;
	private NumberEditor current_omission_editor;
	private NumberEditor prop_energy_editor;
	private NumberEditor max_prop_energy_editor;
	private NumberEditor recommend_threshold_editor;

	private Button clean_cache_button;
	private Button commit_button;
	private Button asdefault_button;

	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState) 
	{
		option = LBDataManager.GetInstance().getFilterOption();
		
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_options);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		happen_prop_editor = (NumberEditor) this.findViewById(R.id.happen_prop_editor);
		happen_prop_editor.setTitle("出现概率大于：");
		happen_prop_editor.setRegion(0, NumberEditor.MAXVALUE);
		happen_prop_editor.setOnValueChangedHandler(new Handler()
		{
			@Override
			public void handleMessage(Message msg) {
				super.handleMessage(msg);	
				
				option.HitProbability_LowLimit = msg.what;
			}
		});
		
		current_omission_editor = (NumberEditor) this.findViewById(R.id.current_omission_editor);
		current_omission_editor.setTitle("当前遗漏大于：");
		current_omission_editor.setRegion(0, NumberEditor.MAXVALUE);
		current_omission_editor.setOnValueChangedHandler(new Handler()
		{
			@Override
			public void handleMessage(Message msg) {
				super.handleMessage(msg);	
				
				option.ImmediateOmission_LowLimit = msg.what;
			}
		});
		
		prop_energy_editor = (NumberEditor) this.findViewById(R.id.prop_energy_editor);
		prop_energy_editor.setTitle("偏离指数大于：");
		prop_energy_editor.setRegion(0, NumberEditor.MAXVALUE);
		prop_energy_editor.setOnValueChangedHandler(new Handler()
		{
			@Override
			public void handleMessage(Message msg) {
				super.handleMessage(msg);	
				
				option.ProtentialEnergy_LowLimit = msg.what;
			}
		});
		
		max_prop_energy_editor = (NumberEditor) this.findViewById(R.id.max_prop_energy_editor);
		max_prop_energy_editor.setTitle("最大偏离大于：");
		max_prop_energy_editor.setRegion(0, NumberEditor.MAXVALUE);
		max_prop_energy_editor.setOnValueChangedHandler(new Handler()
		{
			@Override
			public void handleMessage(Message msg) {
				super.handleMessage(msg);	
				
				option.MaxDeviation_LowLimit = msg.what;
			}
		});
		
		recommend_threshold_editor = (NumberEditor) this.findViewById(R.id.recommend_threshold_editor);
		recommend_threshold_editor.setTitle("标记偏离大于：");
		recommend_threshold_editor.setRegion(1, 10);
		recommend_threshold_editor.setOnValueChangedHandler(new Handler()
		{
			@Override
			public void handleMessage(Message msg) {
				super.handleMessage(msg);	
				
				option.thresoldToRecommend = msg.what;
			}
		});

		clean_cache_button = (Button) this.findViewById(R.id.clean_cache_button);
		clean_cache_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) {
				DataUtil.cleanLocalFiles();
				
				NotificationUtil.ShowMessage(AttributeFilterOptionActivity.this, "本地数据已清除， 下次启动会将重新加载最新数据。");
			}
		});
		
		commit_button = (Button) this.findViewById(R.id.commit_button);
		commit_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) {
				LBDataManager.GetInstance().SaveAttributeFilterOption();
				
				NotificationUtil.ShowMessage(AttributeFilterOptionActivity.this, "设置已保存，下次启动会使用当前设置");
			}
		});
		
		asdefault_button = (Button) this.findViewById(R.id.asdefault_button);
		asdefault_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) {
				option.asDefault();
				refresh();
			}
		});
		
		refresh();
	}
	
	private void refresh()
	{
		happen_prop_editor.setValue((int) option.HitProbability_LowLimit);
		current_omission_editor.setValue(option.ImmediateOmission_LowLimit);
		prop_energy_editor.setValue((int) option.ProtentialEnergy_LowLimit);
		max_prop_energy_editor.setValue((int) option.MaxDeviation_LowLimit);
		recommend_threshold_editor.setValue((int) option.thresoldToRecommend);
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	            finish();
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
}
