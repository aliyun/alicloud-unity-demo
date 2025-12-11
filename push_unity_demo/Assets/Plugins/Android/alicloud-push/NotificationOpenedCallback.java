package com.alibaba.sdk.android.pushwrapper;

public interface NotificationOpenedCallback {
    void onNotificationOpened(String title, String summary, String extraMap);
}
