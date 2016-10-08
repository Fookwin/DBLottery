package com.baidu.mobads.demo.main;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.widget.ImageView;

import com.baidu.mobads.RecommendAd;
import com.baidu.mobads.RecommendAd.RecmdEventListener;

public class RecommendAdActivity extends Activity{
	
	RecommendAd recommendAd;
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.recommend_layout);
		ImageView iconToBind = (ImageView)this.findViewById(R.id.iv_icon1);
		
		
		String appId = "your_appid";//your appid，需从union.baidu.com上申请
		String adPlaceId = "";//广告位ID, 目前为空
		RecommendAd.Builder builder = new RecommendAd.Builder(iconToBind, appId, adPlaceId);
		builder.setEventListener(new RecmdEventListener(){

			@Override
			public void onIconBindFailed(String reason) {
				Log.i("RecommendAd-DEMO ", "onIconBindFailed: " + reason);
			}

			@Override
			public void onIconClick() {
				Log.i("RecommendAd-DEMO ", "onIconClick");
			}

			@Override
			public void onIconShow() {
				Log.i("RecommendAd-DEMO ", "onIconShow");
			}
			
		});
		recommendAd = builder.build();
		recommendAd.load(this);
	}

	@Override
	protected void onDestroy() {
		recommendAd.destroy();
		
		super.onDestroy();
	}
	
}
