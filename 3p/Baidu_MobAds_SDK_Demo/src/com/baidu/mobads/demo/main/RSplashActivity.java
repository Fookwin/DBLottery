package com.baidu.mobads.demo.main;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.widget.RelativeLayout;

import com.baidu.mobads.SplashAd;
import com.baidu.mobads.SplashAd.SplashType;
import com.baidu.mobads.SplashAdListener;

/**
 *	实时开屏，广告实时请求并且立即展现
 */
public class RSplashActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.splash);
		RelativeLayout adsParent = (RelativeLayout) this
				.findViewById(R.id.adsRl);
		SplashAdListener listener=new SplashAdListener() {
			@Override
			public void onAdDismissed() {
				Log.i("RSplashActivity", "onAdDismissed");
				jumpWhenCanClick();// 跳转至您的应用主界面
			}

			@Override
			public void onAdFailed(String arg0) {
				Log.i("RSplashActivity", "onAdFailed");
				jump();
			}

			@Override
			public void onAdPresent() {
				Log.i("RSplashActivity", "onAdPresent");
			}

			@Override
			public void onAdClick() {
				Log.i("RSplashActivity", "onAdClick");
				//设置开屏可接受点击时，该回调可用
			}
		};
		/**
		 * 默认开屏构造函数：
		 * new SplashAd(Context context, ViewGroup adsParent,
		 * 				SplashAdListener listener, SplashType splashType);
		 * 实时开屏默认接受点击，使用样例中的jumpWhenCanClick方法来跳转；
		 */
//		new SplashAd(this, adsParent, listener, SplashType.REAL_TIME);
		/**
		 * 实时开屏默认接受点击。如果想让开屏不接受点击，使用以下构造函数：
		 * new SplashAd(Context context, ViewGroup adsParent,
		 * 				SplashAdListener listener,String posId, boolean canClick, SplashType splashType);
		 * 因当前posId（广告位ID）需设置为空，故可使用如下代码进行创建：
		 */ 
		new SplashAd(this, adsParent, listener, "", true, SplashType.REAL_TIME);
		
	}
	
	
	/**
	 * 当设置开屏可点击时，需要等待跳转页面关闭后，再切换至您的主窗口。故此时需要增加waitingOnRestart判断。
	 * 另外，点击开屏还需要在onRestart中调用jumpWhenCanClick接口。
	 */
	public boolean waitingOnRestart=false;
	private void jumpWhenCanClick() {
		Log.d("test", "this.hasWindowFocus():"+this.hasWindowFocus());
		if(this.hasWindowFocus()||waitingOnRestart){
			this.startActivity(new Intent(RSplashActivity.this, BaiduSDKDemo.class));
			this.finish();
		}else{
			waitingOnRestart=true;
		}
		
	}
	
	/**
	 * 不可点击的开屏，使用该jump方法，而不是用jumpWhenCanClick
	 */
	private void jump() {
		this.startActivity(new Intent(RSplashActivity.this, BaiduSDKDemo.class));
		this.finish();
	}
	
	@Override
	protected void onRestart() {
		super.onRestart();
		if(waitingOnRestart){
			jumpWhenCanClick();
		}
	}
	
}
