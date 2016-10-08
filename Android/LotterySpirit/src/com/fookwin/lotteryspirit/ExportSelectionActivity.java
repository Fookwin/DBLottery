package com.fookwin.lotteryspirit;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.ComponentName;
import android.content.Intent;
import android.content.pm.PackageManager;

import com.fookwin.lotterydata.data.Purchase;
import com.fookwin.lotterydata.data.Scheme;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.util.NotificationUtil;
import com.fookwin.lotteryspirit.view.SchemeOutputFormatSetting;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class ExportSelectionActivity extends Activity 
{
	private Purchase _editingPurchase;
	
	private String outputFormat = "";
	private final String outputFormatForNetease = " :";
	private final Scheme sampleScheme = new Scheme(1,2,3,4,5,6,7);

	private SchemeOutputFormatSetting format_setting;
	private TextView clipboard_format_example;
	private TextView easenet_format_example;
	private View export_to_easenet;
	private View copy_to_clipboard;
	
	@SuppressLint("HandlerLeak")
	Handler onPurchaseSavedHandler = new Handler()
	{
		@Override
		public void handleMessage(Message msg) {
			super.handleMessage(msg);
			
			NotificationUtil.ShowMessage(getApplicationContext(), "当前选号已保存");
		}
	};

	private View fill_form_assistent;
	
	@SuppressLint("HandlerLeak")
	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		_editingPurchase = LBDataManager.GetInstance().getPendingPurchase();
		
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_export_selection);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
        // initialize the tab content.
		clipboard_format_example = (TextView) findViewById(R.id.clipboard_format_example);		
		easenet_format_example = (TextView) findViewById(R.id.easenet_format_example);
		
		export_to_easenet = (View) findViewById(R.id.export_to_easenet);
		export_to_easenet.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				exportToNetEase();
			}			
		});
		
		fill_form_assistent = (View) findViewById(R.id.fill_form_assistent);
		fill_form_assistent.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				exportToFillFormAssistent();
			}			
		});
		
		copy_to_clipboard = (View) findViewById(R.id.copy_to_clipboard);
		copy_to_clipboard.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				exportToClipBoard();
			}			
		});
		
		format_setting = (SchemeOutputFormatSetting) findViewById(R.id.format_setting);
		format_setting.setOnValueChangedHandler(new Handler()
		{
			@Override
			public void handleMessage(Message msg) 
			{
				super.handleMessage(msg);
				
				outputFormat = format_setting.getOutputFormatFromUI();
				updateFormatSample();
			}
		});
		
		View edit_output_format = (View)findViewById(R.id.edit_output_format);
		edit_output_format.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				if (format_setting.getVisibility() == View.GONE)
					format_setting.setVisibility(View.VISIBLE);
				else
					format_setting.setVisibility(View.GONE);
			}			
		});
		
		easenet_format_example.setText("输出格式：" + sampleScheme.toString(outputFormatForNetease));
		
		updateFormatSample();
		
		Button save_btn = (Button) findViewById(R.id.save_btn);
		save_btn.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View v) 
			{
				// Save purchase.
				savePurchase();
				
				// make a new pending purchase and go to home page.
				LBDataManager.GetInstance().resetPendingPurchase();
				Intent intent = new Intent(ExportSelectionActivity.this, MainActivity.class);
				intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
				startActivity(intent);
			}			
		});
	}
	
	private void savePurchase()
	{
		if (_editingPurchase != null && _editingPurchase.isDirty())
		{
			new Thread(new Runnable()
			{
				public void run() 
				{					
					LBDataManager.GetInstance().getPurchaseManager().commit(_editingPurchase);
					onPurchaseSavedHandler.sendEmptyMessage(-1);
				}
			}).start();
		}
	}
	
	private void exportToNetEase()
	{
		// check if netease lottery has been installed.
		PackageManager manager = getPackageManager();
	    Intent neteaseIntent = manager.getLaunchIntentForPackage("com.netease.caipiao");
	    if (neteaseIntent == null)
	    {
	    	NotificationUtil.ShowDialog(this, "无法导出", "没有找到网易彩票客户端", "知道了", "", false, null, null);
	    	return;
	    }
		    	
	    Intent intent = new Intent("android.intent.action.MAIN");
	    intent.setComponent(ComponentName.unflattenFromString("com.netease.caipiao/com.netease.caipiao.common.activities.MainActivity"));
	    intent.addCategory("android.intent.category.LAUNCHER");
	    
	    Bundle bdl = new Bundle();			    
	    bdl.putString("startBy", "open");
	    bdl.putString("g", "ssq");
	    bdl.putInt("p", 1);
	    bdl.putInt("t", 1);
	    bdl.putInt("a", 1);
	    bdl.putInt("r", 0);
	    bdl.putInt("s", 1);
	    
	    bdl.putString("b", GetOutputForNetease());
	    
	    intent.putExtras(bdl);
	    
	    startActivity(intent);
	}
	
	private void exportToFillFormAssistent()
	{
		// set the selection as output source.
		ListFillingHelperActivity.setSource(_editingPurchase.getSelection());
		
		Intent intent = new Intent( this , ListFillingHelperActivity.class );
		intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
	    startActivity( intent );
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
	
	private void exportToClipBoard()
	{
		ClipboardManager clipboard = (ClipboardManager) getSystemService(CLIPBOARD_SERVICE);
		clipboard.setPrimaryClip(ClipData.newPlainText("福盈选号", GetOutput()));

		NotificationUtil.ShowMessage(ExportSelectionActivity.this, "选号已经复制到剪贴板");
	}
	
    private String GetOutputForNetease()
    {
        String stream = "";
        for (Scheme sm : _editingPurchase.getSelection())
        {
            stream += sm.toString(outputFormatForNetease) + ",SINGLE;"; // MULTIPLE
        }

        return stream;
    }
	
    private String GetOutput()
    {
        String stream = "";
        for (Scheme sm : _editingPurchase.getSelection())
        {
            stream += sm.toString(outputFormat) + "\r\n";
        }

        return stream;
    }
	
	private void updateFormatSample()
	{
		clipboard_format_example.setText("输出格式：" + sampleScheme.toString(outputFormat));
	}
}
