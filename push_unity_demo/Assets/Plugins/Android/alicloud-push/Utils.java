package com.alibaba.sdk.android.pushwrapper;

import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;

public class Utils {

	public static Bundle getAppMetaData(Context context) {
		try {
			final PackageManager packageManager = context.getPackageManager();
			final String packageName = context.getPackageName();
			ApplicationInfo info = packageManager.getApplicationInfo(packageName,
				PackageManager.GET_META_DATA);
			if (info != null && info.metaData != null) {
				return info.metaData;
			}
		} catch (PackageManager.NameNotFoundException e) {
			Log.e("AMS", "Meta data not found!");
		}
		return null;
	}

	public static String getPushData(Bundle bundle, String key) {
		if (bundle != null && bundle.containsKey(key)) {
			Object obj = bundle.get(key);
			String value = null;
			
			// 处理不同类型的值
			if (obj instanceof String) {
				value = (String) obj;
			} else if (obj instanceof Integer) {
				value = String.valueOf(obj);
			} else if (obj != null) {
				value = obj.toString();
			}
			
			// 处理特殊格式
			if (value != null && value.startsWith("id=")) {
				value = value.replaceFirst("id=", "");
			}
			return value;
		} else {
			return null;
		}
	}
}
