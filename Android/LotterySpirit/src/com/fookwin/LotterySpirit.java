package com.fookwin;

import android.app.Application;

public class LotterySpirit extends Application {
    private static LotterySpirit instance;

    public static LotterySpirit getInstance() {
        return instance;
    }

    @Override
    public void onCreate() {
        // TODO Auto-generated method stub
        super.onCreate();
        instance = this;
    }
}
