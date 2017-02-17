package com.fookwin.lotteryspirit;

import com.baidu.android.pushservice.PushConstants;
import com.baidu.android.pushservice.PushManager;
import com.baidu.mobstat.StatService;
import com.fookwin.lotteryspirit.R;
import com.fookwin.lotteryspirit.data.LBDataManager;
import com.fookwin.lotteryspirit.util.BitmapUtil;
import com.google.android.gms.appindexing.Action;
import com.google.android.gms.appindexing.AppIndex;
import com.google.android.gms.appindexing.Thing;
import com.google.android.gms.common.api.GoogleApiClient;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
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

public class WelcomeActivity extends Activity {
	private int screenHeight;
	private int screenWidth;
	private ImageView welcome_imageview;
	private TextView step_progress;
	private View reload_button;
	private View skipad_button;

	private boolean dataLoaded = false;
	private boolean finished = false;

	@SuppressLint("HandlerLeak")
	Handler hannext = new Handler() {
		@Override
		public void handleMessage(Message msg) {
			super.handleMessage(msg);

			Bundle data = msg.getData();
			String message = data.getString("TITLE");
			int progress = data.getInt("PROGRESS");
			if (progress > 0)
				message += " (" + Integer.toString(progress) + " %)";

			if (progress == 100) {
				step_progress.setVisibility(View.GONE);
				markDataLoaded(false);
			} else if (progress < 0) {
				step_progress.setVisibility(View.GONE);
				markDataLoaded(true);
			} else {
				// Update the progress UI...
				step_progress.setText(message);
			}
		}
	};
	/**
	 * ATTENTION: This was auto-generated to implement the App Indexing API.
	 * See https://g.co/AppIndexing/AndroidStudio for more information.
	 */
	private GoogleApiClient client;

	/**
	 * Called when the activity is first created.
	 */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		requestWindowFeature(Window.FEATURE_NO_TITLE);

		// Display welcome screen in full screen.
		getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_welcome_view);

		step_progress = (TextView) findViewById(R.id.step_progress);
		reload_button = (View) findViewById(R.id.reload_button);
		skipad_button = (View) findViewById(R.id.skipad_button);

		// Get the screen size.
		DisplayMetrics dm = new DisplayMetrics();
		getWindowManager().getDefaultDisplay().getMetrics(dm);
		screenHeight = dm.heightPixels;
		screenWidth = dm.widthPixels;

		init();
		// ATTENTION: This was auto-generated to implement the App Indexing API.
		// See https://g.co/AppIndexing/AndroidStudio for more information.
		client = new GoogleApiClient.Builder(this).addApi(AppIndex.API).build();
	}

	private void init() {
		int newWidth = screenWidth / 2;
		if (newWidth > screenHeight / 2)
			newWidth = screenHeight / 2;

		welcome_imageview = (ImageView) findViewById(R.id.welcome_img);
		Bitmap srcpic = BitmapFactory.decodeResource(getResources(), R.drawable.image_welcome_screen);
		Bitmap newpic = BitmapUtil.GetNewBitmap(srcpic, screenWidth, screenHeight, newWidth, newWidth);
		welcome_imageview.setImageBitmap(newpic);

		reload_button.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				step_progress.setVisibility(View.VISIBLE);
				reload_button.setVisibility(View.GONE);

				loadData();
			}
		});

		skipad_button.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				//gotoMainPage();
			}
		});

		PushManager.startWork(getApplicationContext(),
				PushConstants.LOGIN_TYPE_API_KEY,
				"mPVtHd5I5kOPbIZBfy76XWik");

		loadData();

		// setSendLogStrategy已经@deprecated，建议使用新的start接口
		// 如果没有页面和自定义事件统计埋点，此代码一定要设置，否则无法完成统计
		// 进程第一次执行此代码，会导致发送上次缓存的统计数据；若无上次缓存数据，则发送空启动日志
		// 由于多进程等可能造成Application多次执行，建议此代码不要埋点在Application中，否则可能造成启动次数偏高
		// 建议此代码埋点在统计路径触发的第一个页面中，若可能存在多个则建议都埋点
		StatService.start(this);
	}

	private void markDataLoaded(boolean failed) {
		if (failed) {
			// show reload button.
			reload_button.setVisibility(View.VISIBLE);
		} else {
			// mark load succeeded and waiting for splash closed.
			dataLoaded = true;

			// go main activity.
			gotoMainPage();
		}
	}

	private void loadData() {
		new Thread(new Runnable() {
			public void run() {
				LBDataManager.GetInstance().setInitializingHandler(hannext);
				LBDataManager.GetInstance().Initialize();
				LBDataManager.GetInstance().setInitializingHandler(null);
			}
		}).start();
	}

	private void gotoMainPage() {
		if (!finished) {
			finish();
			Intent intent = new Intent(WelcomeActivity.this, MainActivity.class);
			startActivity(intent);

			skipad_button.setEnabled(false);
			finished = true;
		}
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();
	}

	/**
	 * ATTENTION: This was auto-generated to implement the App Indexing API.
	 * See https://g.co/AppIndexing/AndroidStudio for more information.
	 */
	public Action getIndexApiAction() {
		Thing object = new Thing.Builder()
				.setName("Welcome Page") // TODO: Define a title for the content shown.
				// TODO: Make sure this auto-generated URL is correct.
				.setUrl(Uri.parse("http://[ENTER-YOUR-URL-HERE]"))
				.build();
		return new Action.Builder(Action.TYPE_VIEW)
				.setObject(object)
				.setActionStatus(Action.STATUS_TYPE_COMPLETED)
				.build();
	}

	@Override
	public void onStart() {
		super.onStart();

		// ATTENTION: This was auto-generated to implement the App Indexing API.
		// See https://g.co/AppIndexing/AndroidStudio for more information.
		client.connect();
		AppIndex.AppIndexApi.start(client, getIndexApiAction());
	}

	@Override
	public void onStop() {
		super.onStop();

		// ATTENTION: This was auto-generated to implement the App Indexing API.
		// See https://g.co/AppIndexing/AndroidStudio for more information.
		AppIndex.AppIndexApi.end(client, getIndexApiAction());
		client.disconnect();
	}
}