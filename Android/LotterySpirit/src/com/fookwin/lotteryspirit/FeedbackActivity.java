package com.fookwin.lotteryspirit;

import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.util.NotificationUtil;

import android.app.ActionBar;
import android.app.Activity;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;

public class FeedbackActivity extends Activity {
	
	private EditText name_edit;
	private EditText email_edit;
	private EditText phone_edit;
	private EditText feedback_edit;
	private Button commit_button;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_feedback);
		
		this.getActionBar().setDisplayHomeAsUpEnabled(true);
		this.getActionBar().setDisplayOptions(0, ActionBar.DISPLAY_SHOW_HOME);
		
		name_edit = (EditText) findViewById(R.id.name_edit);
		email_edit = (EditText) findViewById(R.id.email_edit);
		phone_edit = (EditText) findViewById(R.id.phone_edit);
		feedback_edit = (EditText) findViewById(R.id.feedback_edit);
		
		commit_button = (Button) findViewById(R.id.commit_button);
		commit_button.setOnClickListener(new OnClickListener()
		{
			@Override
			public void onClick(View arg0) {
				
				final String name = name_edit.getText().toString();
				if (name == null || name.isEmpty())
				{
					NotificationUtil.ShowMessage(FeedbackActivity.this, "如何称呼您？");
					name_edit.requestFocus();
					return;
				}
				
				final String email = email_edit.getText().toString();
				if (email == null || email.isEmpty())
				{
					NotificationUtil.ShowMessage(FeedbackActivity.this, "如何联系您？");
					email_edit.requestFocus();
					return;
				}
				
				final String phone = phone_edit.getText().toString(); // optional
				
				final String content = feedback_edit.getText().toString();
				if (content == null || content.isEmpty())
				{
					NotificationUtil.ShowMessage(FeedbackActivity.this, "您的问题是？");
					feedback_edit.requestFocus();
					return;
				}
				
				new Thread(new Runnable()
				{
					public void run() 
					{
						try 
						{					
							LBDataManager.GetInstance().PostFeedback(name, email, phone, content);
						} catch (Exception e) {
							e.printStackTrace();
						}
					}
				}).start();
				
				NotificationUtil.ShowMessage(FeedbackActivity.this, "感谢您的支持，我们会认真考虑您的宝贵意见！");
			}
		});
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
