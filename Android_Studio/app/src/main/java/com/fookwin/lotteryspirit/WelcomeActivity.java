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
import com.qq.e.ads.splash.SplashAD;
import com.qq.e.ads.splash.SplashADListener;
import com.qq.e.comm.constants.Constants;

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
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.view.View.OnClickListener;
import android.widget.ImageView;
import android.widget.TextView;

public class WelcomeActivity extends Activity implements SplashADListener {
	private int screenHeight;
	private int screenWidth;
	private ImageView welcome_imageview;
	private TextView step_progress;
	private View reload_button;
	private View skipad_button;

	private boolean dataLoaded = false;
	private boolean adClosed = false;

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
	private TextView skipView;
	private SplashAD splashAD;

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

		ViewGroup container = (ViewGroup) this.findViewById(R.id.ad_view);
		skipView = (TextView) findViewById(R.id.skip_ad_button);
		skipView.setVisibility(View.INVISIBLE);
		fetchSplashAD(this, container, skipView, "1102491872", "7000604514947293", this, 0);

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
		if (adClosed && dataLoaded) {
			finish();
			Intent intent = new Intent(WelcomeActivity.this, MainActivity.class);
			startActivity(intent);

			skipad_button.setEnabled(false);
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

	/**
	 * 拉取开屏广告，开屏广告的构造方法有3种，详细说明请参考开发者文档。
	 *
	 * @param activity        展示广告的activity
	 * @param adContainer     展示广告的大容器
	 * @param skipContainer   自定义的跳过按钮：传入该view给SDK后，SDK会自动给它绑定点击跳过事件。SkipView的样式可以由开发者自由定制，其尺寸限制请参考activity_splash.xml或下面的注意事项。
	 * @param appId           应用ID
	 * @param posId           广告位ID
	 * @param adListener      广告状态监听器
	 * @param fetchDelay      拉取广告的超时时长：取值范围[3000, 5000]，设为0表示使用广点通SDK默认的超时时长。
	 */
	private void fetchSplashAD(Activity activity, ViewGroup adContainer, View skipContainer,
							   String appId, String posId, SplashADListener adListener, int fetchDelay) {
		splashAD = new SplashAD(activity, adContainer, skipContainer, appId, posId, adListener, fetchDelay);
	}

	@Override
	public void onADPresent() {
		Log.i("AD_DEMO", "SplashADPresent");
		welcome_imageview.setVisibility(View.INVISIBLE); // 广告展示后一定要把预设的开屏图片隐藏起来
		skipView.setVisibility(View.VISIBLE);
	}

	@Override
	public void onADDismissed() {
		Log.i("AD_DEMO", "SplashADDismissed");
		next();
	}

	private void next() {
		adClosed = true;

		gotoMainPage();
	}

	@Override
	public void onNoAD(int arg0) {
		Log.i("AD_DEMO", "LoadSplashADFail,ecode=" + arg0);
		next();
	}


	@Override
	public void onADClicked() {
		Log.i("AD_DEMO", "SplashADClicked");
	}

	/**
	 * 倒计时回调，返回广告还将被展示的剩余时间。
	 * 通过这个接口，开发者可以自行决定是否显示倒计时提示，或者还剩几秒的时候显示倒计时
	 *
	 * @param millisUntilFinished 剩余毫秒数
	 */
	@Override
	public void onADTick(long millisUntilFinished) {
		Log.i("AD_DEMO", "SplashADTick " + millisUntilFinished + "ms");
		skipView.setText(String.format("跳过　%d秒", Math.round(millisUntilFinished / 1000f)));
	}

	//防止用户返回键退出APP
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if (keyCode == KeyEvent.KEYCODE_BACK || keyCode == KeyEvent.KEYCODE_HOME) {
			return true;
		}
		return super.onKeyDown(keyCode, event);
	}
}