package com.alibaba.sdk.android.pushwrapper;

import java.util.Map;

public interface NotificationCallback {
    void onNotification(String title, String summary, Map<String, String> extraMap);
}
