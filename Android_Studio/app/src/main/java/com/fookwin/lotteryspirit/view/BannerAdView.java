package com.fookwin.lotteryspirit.view;

import android.app.Activity;
import android.util.Log;
import android.view.ViewGroup;

import com.qq.e.ads.banner.ADSize;
import com.qq.e.ads.banner.AbstractBannerADListener;
import com.qq.e.ads.banner.BannerView;

public class BannerAdView {
    private Activity _activity;
    private String _adPosId;
    private ViewGroup _container;
    private BannerView bannerAdView;

    public BannerAdView(Activity activity, String posId, ViewGroup container) {
        _activity = activity;
        _adPosId = posId;
        _container = container;
    }

    public void loadAD() {
        if (bannerAdView == null) {
            bannerAdView = new BannerView(_activity, ADSize.BANNER, "1102491872", _adPosId);
            bannerAdView.setRefresh(30);
            bannerAdView.setADListener(new AbstractBannerADListener() {

                @Override
                public void onNoAD(int arg0) {
                    Log.i("AD_DEMO", "BannerNoAD，eCode=" + arg0);
                }

                @Override
                public void onADReceiv() {
                    Log.i("AD_DEMO", "ONBannerReceive");
                }
            });
            _container.addView(bannerAdView);
            bannerAdView.loadAD();
        }

        bannerAdView.loadAD();
    }
}
