package com.alibaba.sdk.android.pushwrapper;

import java.util.Map;

public interface ThirdPushNotificationOpenedCallback {
	void onThirdPushNotificationOpened(String title, String summary, Map<String, String> extraMap);
}
