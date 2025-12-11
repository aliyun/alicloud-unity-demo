package com.alibaba.sdk.android.pushwrapper;

import java.util.Map;

import android.content.Intent;
import com.alibaba.sdk.android.push.AndroidPopupActivity;

public class ThirdPushActivity extends AndroidPopupActivity {

	@Override
	protected void onSysNoticeOpened(String title, String summary, Map<String, String> extMap) {
		PushWrapper.receiveThirdPushNotificationOpened(title, summary, extMap);
		startActivity(getPackageManager().getLaunchIntentForPackage(getPackageName()));
		finish();
	}

	@Override
    public void onNotPushData(Intent intent) {
        startActivity(getPackageManager().getLaunchIntentForPackage(getPackageName()));
		finish();
    }

	@Override
    public void onParseFailed(Intent intent) {
		startActivity(getPackageManager().getLaunchIntentForPackage(getPackageName()));
		finish();
    }
}
