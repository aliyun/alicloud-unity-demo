package com.alibaba.sdk.android.pushwrapper;

import java.util.Map;

public class NotificationClickData {
	public enum ClickType {
		NOTIFICATION_OPENED,           // 普通通知点击
		NOTIFICATION_CLICKED_NO_ACTION, // 无动作通知点击
		THIRD_PUSH_NOTIFICATION_OPENED  // 厂商通知点击
	}

	private final String title;
	private final String summary;
	private final String extraMap;
	private final Map<String, String> extraMapObject;
	private final ClickType clickType;

	public NotificationClickData(String title, String summary, String extraMap, ClickType clickType) {
		this.title = title;
		this.summary = summary;
		this.extraMap = extraMap;
		this.extraMapObject = null;
		this.clickType = clickType;
	}

	public NotificationClickData(String title, String summary, Map<String, String> extraMapObject, ClickType clickType) {
		this.title = title;
		this.summary = summary;
		this.extraMap = null;
		this.extraMapObject = extraMapObject;
		this.clickType = clickType;
	}

	public String getTitle() {
		return title != null ? title : "";
	}

	public String getSummary() {
		return summary != null ? summary : "";
	}

	public String getExtraMap() {
		return extraMap != null ? extraMap : "";
	}

	public Map<String, String> getExtraMapObject() {
		return extraMapObject;
	}

	public ClickType getClickType() {
		return clickType;
	}
}
