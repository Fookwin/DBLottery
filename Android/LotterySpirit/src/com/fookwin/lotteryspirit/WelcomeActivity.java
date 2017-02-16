package com.fookwin.lotteryspirit;

import com.adsmogo.splash.AdsMogoSplash;
import com.adsmogo.splash.AdsMogoSplashListener;
import com.adsmogo.util.AdsMogoSplashMode;
import com.baidu.android.pushservice.PushConstants;
import com.baidu.android.pushservice.PushManager;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.util.BitmapUtil;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.DisplayMetrics;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.view.View.OnClickListener;
import android.widget.ImageView;
import android.widget.TextView;

public class WelcomeActivity extends Activity implements AdsMogoSplashListener
{
    private int screenHeight;
	private int screenWidth;	
	private ImageView welcome_imageview;
	private TextView step_progress;
	private ViewGroup adView;
	private View reload_button;
	private View skipad_button;
	
	private boolean dataLoaded = false;
	private boolean splashClosed = false;
	private boolean finished = false;
	
	@SuppressLint("HandlerLeak")
	Handler hannext = new Handler()
	{
		@Override
		public void handleMessage(Message msg) {
			super.handleMessage(msg);
			
			Bundle data = msg.getData();
			String message = data.getString("TITLE");
			int progress = data.getInt("PROGRESS");
			if (progress > 0)
				message += " (" + Integer.toString(progress) + " %)";
			
			if (progress == 100)
			{			
				step_progress.setVisibility(View.GONE);
				markDataLoaded(false);
			}
			else if (progress < 0)
			{
				step_progress.setVisibility(View.GONE);			
				markDataLoaded(true);
			}
			else
			{
				// Update the progress UI...
				step_progress.setText(message);
			}
		}
	};

	/** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState)
    {
    	requestWindowFeature(Window.FEATURE_NO_TITLE);
        
        // Display welcome screen in full screen.
		getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);
        
        super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_welcome_view);
        
        step_progress = (TextView)findViewById(R.id.step_progress);
        reload_button = (View) findViewById(R.id.reload_button);
        skipad_button = (View) findViewById(R.id.skipad_button);
        
        // Get the screen size.
        DisplayMetrics dm = new DisplayMetrics();  
		getWindowManager().getDefaultDisplay().getMetrics(dm); 
		screenHeight= dm.heightPixels;  
		screenWidth =dm.widthPixels;
		
		adView = (ViewGroup) findViewById(R.id.adsMogoView);
		
		init();
    }

	private void init()
	{
		int newWidth = screenWidth/2;
		if (newWidth > screenHeight/2)
			newWidth = screenHeight/2;
		
		welcome_imageview = (ImageView)findViewById(R.id.welcome_img);
		Bitmap srcpic = BitmapFactory.decodeResource(getResources(),R.drawable.image_welcome_screen);
		Bitmap newpic = BitmapUtil.GetNewBitmap(srcpic, screenWidth, screenHeight, newWidth, newWidth);
		welcome_imageview.setImageBitmap(newpic);
		
		AdsMogoSplash adsmogoSplash = new AdsMogoSplash(this, "d8c9d9b2ee8c4aa0bc2f861fc7d484a1",
				adView, screenWidth, screenHeight - 60, AdsMogoSplashMode.TOP);

		adsmogoSplash.setAdsMogoSplashListener(this);
		
		reload_button.setOnClickListener(new OnClickListener()
		{
			public void onClick(View v)
			{
				step_progress.setVisibility(View.VISIBLE);
				reload_button.setVisibility(View.GONE);
				
				loadData();
			}
		});
		
		skipad_button.setOnClickListener(new OnClickListener()
		{
			public void onClick(View v)
			{
				//gotoMainPage();
			}
		});
		
		PushManager.startWork(getApplicationContext(),
                PushConstants.LOGIN_TYPE_API_KEY,
                "mPVtHd5I5kOPbIZBfy76XWik");
		
		loadData();
	}
	
	private void markDataLoaded(boolean failed)
	{
		if (failed)
		{
			// show reload button.
			reload_button.setVisibility(View.VISIBLE);
		}
		else
		{
			// mark load succeeded and waiting for splash closed.
			dataLoaded = true;
			if (splashClosed)
			{
				// go main activity.
				gotoMainPage();
			}
			else
			{
				// show skip splash button.
				skipad_button.setVisibility(View.VISIBLE);
			}
		}
	}
	
	private void loadData()
	{
		new Thread(new Runnable()
		{
			public void run() 
			{
				LBDataManager.GetInstance().setInitializingHandler(hannext);
				LBDataManager.GetInstance().Initialize();
				LBDataManager.GetInstance().setInitializingHandler(null);
			}
		}).start();
	}
	
	private void gotoMainPage()
	{
		if (!finished)
		{
			finish();
			Intent intent = new Intent(WelcomeActivity.this, MainActivity.class);
			startActivity(intent);
			
			skipad_button.setEnabled(false);
			finished = true;
		}
	}
	
	@Override
	protected void onDestroy()
	{
		super.onDestroy();
	}

	@Override
	public void onSplashClickAd(String arg0) {
	}

	@Override
	public void onSplashClose() {
		splashClosed = true;
		if (dataLoaded)
		{
			// go main page if data has been loaded.
			gotoMainPage();
		}
	}

	@Override
	public void onSplashError(String arg0) {
	}

	@Override
	public void onSplashRealClickAd(String arg0) {
	}

	@Override
	public void onSplashSucceed() {
	}
}