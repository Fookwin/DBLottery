package com.fookwin.lotteryspirit.view;

import android.app.Activity;
import android.util.Log;
import android.view.View;

import com.androidquery.AQuery;
import com.fookwin.lotteryspirit.R;
import com.qq.e.ads.nativ.NativeAD;
import com.qq.e.ads.nativ.NativeADDataRef;

import java.util.List;

public class NativeADView implements NativeAD.NativeAdListener{
    private Activity _activity;
    private String _adPosId;
    private View _container;

    private NativeADDataRef adItem;
    private NativeAD nativeAD;
    protected AQuery $;

    public NativeADView(Activity activity, String posId, View container) {
        _activity = activity;
        _adPosId = posId;
        _container = container;

        $ = new AQuery(_container);
    }

    public void loadAD() {
        if (nativeAD == null) {
            this.nativeAD = new NativeAD(_activity, "1102491872", _adPosId, this);
        }
        int count = 1; // 一次拉取的广告条数：范围1-10
        nativeAD.loadAD(count);
    }

    /**
     * 展示原生广告时，一定要先调用onExposured接口曝光广告，否则将无法调用onClicked点击接口
     */
    private void showAD() {

        String iconUrl = adItem.getIconUrl();
        String imgUrl = adItem.getImgUrl();
        if (iconUrl.isEmpty())
            iconUrl = imgUrl;

        String title = adItem.getTitle();
        String desc = adItem.getDesc();

        $.id(R.id.img_logo).image(iconUrl, false, true);
        $.id(R.id.img_poster).image(imgUrl, false, true);
        $.id(R.id.text_name).text(title);
        $.id(R.id.text_desc).text(desc);
        $.id(R.id.btn_download).text(getADButtonText());
        adItem.onExposured(_container); // 需要先调用曝光接口
        $.id(R.id.btn_download).clicked(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                adItem.onClicked(view); // 点击接口
            }
        });
    }

    /**
     * App类广告安装、下载状态的更新（普链广告没有此状态，其值为-1） 返回的AppStatus含义如下： 0：未下载 1：已安装 2：已安装旧版本 4：下载中（可获取下载进度“0-100”）
     * 8：下载完成 16：下载失败
     */
    private String getADButtonText() {
        if (adItem == null) {
            return "……";
        }
        if (!adItem.isAPP()) {
            return "查看详情";
        }
        switch (adItem.getAPPStatus()) {
            case 0:
                return "点击下载";
            case 1:
                return "点击启动";
            case 2:
                return "点击更新";
            case 4:
                return adItem.getProgress() > 0 ? "下载中" + adItem.getProgress()+ "%" : "下载中"; // 特别注意：当进度小于0时，不要使用进度来渲染界面
            case 8:
                return "下载完成";
            case 16:
                return "下载失败,点击重试";
            default:
                return "查看详情";
        }
    }

    @Override
    public void onADLoaded(List<NativeADDataRef> arg0) {
        if (arg0.size() > 0) {
            adItem = arg0.get(0);
            showAD();
        } else {
            Log.i("AD_DEMO", "NOADReturn");
        }
    }

    @Override
    public void onADStatusChanged(NativeADDataRef arg0) {
    }

    @Override
    public void onADError(NativeADDataRef adData, int errorCode) {
        Log.i("AD_DEMO", "onADError:" + errorCode);
    }

    @Override
    public void onNoAD(int arg0) {
        Log.i("AD_DEMO", "ONNoAD:" + arg0);
    }
}
