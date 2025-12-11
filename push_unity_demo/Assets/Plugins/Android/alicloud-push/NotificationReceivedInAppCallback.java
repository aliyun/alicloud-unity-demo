package com.alibaba.sdk.android.pushwrapper;

import java.util.Map;

public interface NotificationReceivedInAppCallback {
    void onNotificationReceivedInApp(String title, String summary, Map<String, String> extraMap, int openType, String openActivity, String openUrl);
}
