package com.baidu.mobads.demo.main;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import com.baidu.mobads.BaiduManager;
import com.baidu.mobads.demo.main.video.BaiduSDKVideoDemo;

public class BaiduSDKDemo extends Activity {
	
	Button simpleCoding;
	Button simpleVideo;
	Button simpleInter;
	Button simpleInterForVideoApp;
	Button simpleIcon;
	Button simpleRecomAd;
	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);		
		setContentView(R.layout.main);
		
		// 广告展现前先调用sdk初始化方法，可以有效缩短广告第一次展现所需时间
		BaiduManager.init(this);
		
		simpleCoding = (Button) findViewById(R.id.simple_coding);
		simpleCoding.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				Intent intent = new Intent(simpleCoding.getContext(), BannerAdActivity.class);
				startActivity(intent);
			}
		});
		
		simpleVideo = (Button) findViewById(R.id.simple_video);
		simpleVideo.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				Intent intent = new Intent(simpleVideo.getContext(), BaiduSDKVideoDemo.class);
				startActivity(intent);
			}
		});
		
		simpleInter = (Button) findViewById(R.id.simple_inters);
		simpleInter.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				Intent intent = new Intent(simpleInter.getContext(), InterstitialAdActivity.class);
				startActivity(intent);
			}
		});
		
		simpleInterForVideoApp = (Button) findViewById(R.id.simple_inters_for_videoapp);
		simpleInterForVideoApp.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				Intent intent = new Intent(simpleInterForVideoApp.getContext(), InterstitialAdForVideoAppActivity.class);
				startActivity(intent);
			}
		});
		
		simpleIcon = (Button) findViewById(R.id.simple_icon);
		simpleIcon.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				Intent intent = new Intent(simpleIcon.getContext(), IconsAdActivity.class);
				startActivity(intent);
			}
		});
		simpleRecomAd = (Button) findViewById(R.id.simple_recmad);
		simpleRecomAd.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				Intent intent = new Intent(simpleRecomAd.getContext(), RecommendAdActivity.class);
				startActivity(intent);
			}
		});
		
	}
}

//      /:~