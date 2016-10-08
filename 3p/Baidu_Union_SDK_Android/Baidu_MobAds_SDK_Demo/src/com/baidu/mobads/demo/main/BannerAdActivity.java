package com.baidu.mobads.demo.main;

import org.json.JSONObject;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.ViewGroup;
import android.widget.RelativeLayout;

import com.baidu.mobads.AdSettings;
import com.baidu.mobads.AdView;
import com.baidu.mobads.AdViewListener;

public class BannerAdActivity extends Activity {

	AdView adView;
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		RelativeLayout your_originnal_layout = new RelativeLayout(this);
		setContentView(your_originnal_layout);
		
		// 代码设置AppSid和Appsec，此函数必须在AdView实例化前调用
		// AdView.setAppSid("debug");
		// AdView.setAppSec("debug");
		
		// 人群属性
		AdSettings.setKey(new String[] { "baidu", "中 国 " });
		// AdSettings.setSex(AdSettings.Sex.FEMALE);
		// AdSettings.setBirthday(Calendar.getInstance());
		// AdSettings.setCity("上海");
		// AdSettings.setZip("123456");
		// AdSettings.setJob("工程师");
		// AdSettings.setEducation(AdSettings.Education.BACHELOR);
		// AdSettings.setSalary(AdSettings.Salary.F10kT15k);
		// AdSettings.setHob(new String[]{"羽毛球", "足球", "baseball"});
		// AdSettings.setUserAttr("k1","v1");
		// AdSettings.setUserAttr("k2","v2");
		
		// 创建广告View
		adView = new AdView(this);
		// 设置监听器
		adView.setListener(new AdViewListener() {
			public void onAdSwitch() {
				Log.w("", "onAdSwitch");
			}
			public void onAdShow(JSONObject info) {
				// 广告已经渲染出来
				Log.w("", "onAdShow " + info.toString());
			}
			public void onAdReady(AdView adView) {
				// 资源已经缓存完毕，还没有渲染出来
				Log.w("", "onAdReady " + adView);
			}
			public void onAdFailed(String reason) {
				Log.w("", "onAdFailed " + reason);
			}
			public void onAdClick(JSONObject info) {
				Log.w("", "onAdClick " + info.toString());
			}
			public void onVideoStart() {
				Log.w("", "onVideoStart");
			}
			public void onVideoFinish() {
				Log.w("", "onVideoFinish");
			}
			@Override
			public void onVideoClickAd() {
				Log.w("", "onVideoFinish");
			}
			@Override
			public void onVideoClickClose() {
				Log.w("", "onVideoFinish");
			}
			@Override
			public void onVideoClickReplay() {
				Log.w("", "onVideoFinish");
			}
			@Override
			public void onVideoError() {
				Log.w("", "onVideoFinish");
			}
		});
		
		//将adView添加到父控件中(注：该父控件不一定为您的根控件，只要该控件能通过addView能添加广告视图即可)
		RelativeLayout.LayoutParams rllp = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.FILL_PARENT, RelativeLayout.LayoutParams.WRAP_CONTENT);
		rllp.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM);
		your_originnal_layout.addView(adView, rllp);

	}
	/**
	 * Activity销毁时，销毁adView
	 */
	@Override
	protected void onDestroy() {
		adView.destroy();
		super.onDestroy();
	}
	
	
}