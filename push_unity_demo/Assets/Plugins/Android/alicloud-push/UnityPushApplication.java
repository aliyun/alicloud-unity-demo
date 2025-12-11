package com.unity3d.player;

import android.app.Application;
import com.alibaba.sdk.android.pushwrapper.PushWrapper;

/**
 * Unity Push Application
 * 用于初始化阿里云推送服务
 */
public class UnityPushApplication extends Application {
    @Override
    public void onCreate() {
        super.onCreate();
        // 初始化推送服务
        PushWrapper.init(this);
    }
}
