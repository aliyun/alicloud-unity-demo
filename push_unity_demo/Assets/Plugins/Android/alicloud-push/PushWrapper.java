package com.alibaba.sdk.android.pushwrapper;

import java.lang.ref.WeakReference;
import java.util.ArrayList;

import com.alibaba.sdk.android.logger.ILog;
import com.alibaba.sdk.android.push.CloudPushService;
import com.alibaba.sdk.android.push.HonorRegister;
import com.alibaba.sdk.android.push.huawei.HuaWeiRegister;
import com.alibaba.sdk.android.push.noonesdk.PushInitConfig;
import com.alibaba.sdk.android.push.noonesdk.PushServiceFactory;
import com.alibaba.sdk.android.push.register.GcmRegister;
import com.alibaba.sdk.android.push.register.MeizuRegister;
import com.alibaba.sdk.android.push.register.MiPushRegister;
import com.alibaba.sdk.android.push.register.OppoRegister;
import com.alibaba.sdk.android.push.register.VivoRegister;

import android.app.Application;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.provider.Settings;
import androidx.core.app.NotificationManagerCompat;

import static android.content.Context.NOTIFICATION_SERVICE;

/**
 * sdk 对外提供的api
 */
public class PushWrapper implements Constant {

	private static final ILog LOGGER = PushWrapperLog.getLogger(PushWrapper.class);

	private static PushCallback sPushCallback;
	private static NotificationCallback sNotificationCallback;
	private static MessageReceivedCallback sMessageReceivedCallback;
	private static NotificationOpenedCallback sNotificationOpenedCallback;
	private static NotificationClickedWithNoActionCallback sNotificationClickedWithNoActionCallback;
	private static NotificationRemovedCallback sNotificationRemovedCallback;
	private static NotificationReceivedInAppCallback sNotificationReceivedInAppCallback;
	private static ThirdPushNotificationOpenedCallback sThirdPushNotificationOpenedCallback;

	private static Application mContext;

	private static final ArrayList<NotificationClickData> PENDING_CLICK_DATA_LIST = new ArrayList<>();

	public static void init(Application application) {
		LOGGER.d("pushWrapper init");
		mContext = application;
		PushInitConfig config = new PushInitConfig.Builder()
			.application(application)
			.build();
		PushServiceFactory.init(config);
	}

	/**
	 * 注册阿里云通道
	 */
	public static void register(int actionId) {
		LOGGER.d("push register " + actionId);
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		if (mContext != null) {
			pushService.register(mContext,
				new PushCommonCallback(actionId, sPushCallback));
		}
		if (PENDING_CLICK_DATA_LIST.size() > 0) {
			sendOldClickData();
		}
	}

	/**
	 * 注册厂商通道
	 */
	public static void registerThirdPush() {
		LOGGER.d("push registerThirdPush");

		CloudPushService pushService = PushServiceFactory.getCloudPushService();

		Bundle metaData = Utils.getAppMetaData(mContext);

		if (metaData != null && metaData.containsKey(HUAWEI_APP_ID)) {
			// 华为通道
			HuaWeiRegister.register(mContext);
		}

		if (metaData != null && metaData.containsKey(XIAO_MI_ID)
			&& metaData.containsKey(XIAO_MI_KEY)) {
			// 小米通道
			MiPushRegister.register(
				mContext,
				Utils.getPushData(metaData, XIAO_MI_ID),
				Utils.getPushData(metaData, XIAO_MI_KEY));
		}

		if (metaData != null && metaData.containsKey(OPPO_KEY)
			&& metaData.containsKey(OPPO_SECRET)) {
			// oppo通道
			OppoRegister.register(
				mContext,
				Utils.getPushData(metaData, OPPO_KEY),
				Utils.getPushData(metaData, OPPO_SECRET));
		}
		if (metaData != null && metaData.containsKey(VIVO_API_KEY)
			&& metaData.containsKey(VIVO_APP_ID)) {
			// vivo通道
			VivoRegister.register(mContext);//接入vivo辅助推送
		}

		if (metaData != null && metaData.containsKey(MEIZU_ID)
			&& metaData.containsKey(MEIZU_SECRET)) {
			// 魅族通道
			MeizuRegister.register(
				mContext,
				Utils.getPushData(metaData, MEIZU_ID),
				Utils.getPushData(metaData, MEIZU_SECRET));
		}

		if (metaData != null && metaData.containsKey(GCM_SEND_ID)
			&& metaData.containsKey(GCM_APPLICATION_ID) && metaData.containsKey(GCM_PROJECT_ID)
			&& metaData.containsKey(GCM_API_KEY)) {
			// FCM通道
			GcmRegister.register(
				mContext,
				Utils.getPushData(metaData, GCM_SEND_ID),
				Utils.getPushData(metaData, GCM_APPLICATION_ID),
				Utils.getPushData(metaData, GCM_PROJECT_ID),
				Utils.getPushData(metaData, GCM_API_KEY)
			);
		}

		if (metaData != null && metaData.containsKey(HONOR_APP_ID)) {
			HonorRegister.register(mContext);
		}
	}

	public static void setPushCallback(PushCallback pushCallback) {
		PushWrapper.sPushCallback = pushCallback;
	}

	public static void setMessageReceivedCallback(MessageReceivedCallback messageReceivedCallback) {
		PushWrapper.sMessageReceivedCallback = messageReceivedCallback;
	}

	public static void setNotificationCallback(NotificationCallback notificationCallback) {
		PushWrapper.sNotificationCallback = notificationCallback;
	}

	public static void setNotificationOpenedCallback(NotificationOpenedCallback notificationOpenedCallback) {
		PushWrapper.sNotificationOpenedCallback = notificationOpenedCallback;
	}

	public static void setNotificationClickedWithNoActionCallback(NotificationClickedWithNoActionCallback callback) {
		PushWrapper.sNotificationClickedWithNoActionCallback = callback;
	}

	public static void setNotificationRemovedCallback(NotificationRemovedCallback notificationRemovedCallback) {
		PushWrapper.sNotificationRemovedCallback = notificationRemovedCallback;
	}

	public static void setNotificationReceivedInAppCallback(NotificationReceivedInAppCallback callback) {
		PushWrapper.sNotificationReceivedInAppCallback = callback;
	}

	public static void setThirdPushNotificationOpenedCallback(ThirdPushNotificationOpenedCallback callback) {
		PushWrapper.sThirdPushNotificationOpenedCallback = callback;
	}

	public static void bindAccount(int actionId, String account) {
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.bindAccount(account, new PushCommonCallback(actionId, sPushCallback));
	}

	public static void unbindAccount(int actionId) {
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.unbindAccount(new PushCommonCallback(actionId, sPushCallback));
	}

	public static void bindTag(int actionId, int target, String tag, String alias) {
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.bindTag(target, new String[] {tag}, alias,
			new PushCommonCallback(actionId, sPushCallback));
	}

	public static void unbindTag(int actionId, int target, String tag, String alias) {
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.unbindTag(target, new String[] {tag}, alias,
			new PushCommonCallback(actionId, sPushCallback));
	}

	public static void listTag(int actionId, int target) {
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.listTags(target, new PushCommonCallback(actionId, sPushCallback));
	}

	/**
	 * 添加别名
	 */
	public static void addAlias(int actionId, final String alias) {
		LOGGER.d("add alias " + alias + " actionId " + actionId);
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.addAlias(alias, new PushCommonCallback(actionId, sPushCallback));
	}

	public static void removeAlias(int actionId, final String alias) {
		LOGGER.d("remove alias " + alias + " actionId " + actionId);
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.removeAlias(alias, new PushCommonCallback(actionId, sPushCallback));
	}

	public static void listAlias(int actionId) {
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.listAliases(new PushCommonCallback(actionId, sPushCallback));
	}

	public static void bindPhone(int actionId, final String phone) {
		LOGGER.d("bindPhone " + phone + " actionId " + actionId);
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.bindPhoneNumber(phone, new PushCommonCallback(actionId, sPushCallback));
	}

	public static void unbindPhone(int actionId) {
		LOGGER.d("unBindPhone actionId " + actionId);
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		pushService.unbindPhoneNumber(new PushCommonCallback(actionId, sPushCallback));
	}

	/**
	 * 获取设备ID
	 */
	public static String deviceId() {
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		return pushService.getDeviceId();
	}

	public static void receiveNotification(String title, String summary, java.util.Map<String, String> extraMap) {
		if (sNotificationCallback != null) {
			sNotificationCallback.onNotification(title, summary, extraMap);
		}
	}

	public static void receiveMessage(String title, String content) {
		if (sMessageReceivedCallback != null) {
			sMessageReceivedCallback.receive(title != null ? title : "",
				content != null ? content : "");
		}
	}

	public static void receiveNotificationOpened(String title, String summary, String extraMap) {
		if (sNotificationOpenedCallback != null) {
			sNotificationOpenedCallback.onNotificationOpened(title, summary, extraMap);
		} else {
			PENDING_CLICK_DATA_LIST.add(new NotificationClickData(title, summary, extraMap, 
				NotificationClickData.ClickType.NOTIFICATION_OPENED));
		}
	}

	public static void receiveNotificationClickedWithNoAction(String title, String summary, String extraMap) {
		if (sNotificationClickedWithNoActionCallback != null) {
			sNotificationClickedWithNoActionCallback.onNotificationClickedWithNoAction(title, summary, extraMap);
		} else {
			PENDING_CLICK_DATA_LIST.add(new NotificationClickData(title, summary, extraMap, 
				NotificationClickData.ClickType.NOTIFICATION_CLICKED_NO_ACTION));
		}
	}

	public static void receiveThirdPushNotificationOpened(String title, String summary, java.util.Map<String, String> extraMap) {
		if (sThirdPushNotificationOpenedCallback != null) {
			sThirdPushNotificationOpenedCallback.onThirdPushNotificationOpened(title, summary, extraMap);
		} else {
			PENDING_CLICK_DATA_LIST.add(new NotificationClickData(title, summary, extraMap, 
				NotificationClickData.ClickType.THIRD_PUSH_NOTIFICATION_OPENED));
		}
	}

	private synchronized static void sendOldClickData() {
		if (PENDING_CLICK_DATA_LIST.size() > 0) {
			ArrayList<NotificationClickData> tmp = new ArrayList<>(PENDING_CLICK_DATA_LIST);
			for (NotificationClickData data : tmp) {
				switch (data.getClickType()) {
					case NOTIFICATION_OPENED:
						if (sNotificationOpenedCallback != null) {
							sNotificationOpenedCallback.onNotificationOpened(
								data.getTitle(), data.getSummary(), data.getExtraMap());
						}
						break;
					case NOTIFICATION_CLICKED_NO_ACTION:
						if (sNotificationClickedWithNoActionCallback != null) {
							sNotificationClickedWithNoActionCallback.onNotificationClickedWithNoAction(
								data.getTitle(), data.getSummary(), data.getExtraMap());
						}
						break;
					case THIRD_PUSH_NOTIFICATION_OPENED:
						if (sThirdPushNotificationOpenedCallback != null) {
							sThirdPushNotificationOpenedCallback.onThirdPushNotificationOpened(
								data.getTitle(), data.getSummary(), data.getExtraMapObject());
						}
						break;
				}
			}
			PENDING_CLICK_DATA_LIST.removeAll(tmp);
		}
	}

	public static void receiveNotificationRemoved(String messageId) {
		if (sNotificationRemovedCallback != null) {
			sNotificationRemovedCallback.onNotificationRemoved(messageId);
		}
	}

	public static void receiveNotificationReceivedInApp(String title, String summary, java.util.Map<String, String> extraMap, 
			int openType, String openActivity, String openUrl) {
		if (sNotificationReceivedInAppCallback != null) {
			sNotificationReceivedInAppCallback.onNotificationReceivedInApp(title, summary, extraMap, openType, openActivity, openUrl);
		}
	}

	public static boolean isNotificationEnabled() {
		if (mContext != null) {
			NotificationManagerCompat manager = NotificationManagerCompat.from(mContext);
			// areNotificationsEnabled方法的有效性官方只最低支持到API 19，低于19的仍可调用此方法不过只会返回true，即默认为用户已经开启了通知。
			boolean isOpened = manager.areNotificationsEnabled();
			LOGGER.d("isNotificationEnabled：" + isOpened);
			return isOpened;
		}
		return false;
	}

	public static boolean isNotificationChannelEnabled(String channelId) {
		if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
			if (mContext != null) {
				NotificationManager manager = (NotificationManager)mContext.getSystemService(
					NOTIFICATION_SERVICE);
				NotificationChannel channel = manager.getNotificationChannel(channelId);
				boolean isOpened = !(channel.getImportance()
					== NotificationManager.IMPORTANCE_NONE);
				LOGGER.d("isNotificationChannelEnabled " + channelId + "：" + isOpened);
				return isOpened;
			}
			return false;
		} else {
			return true;
		}
	}

	public static void jumpToNotificationSetting() {
		if (mContext == null) {
			return;
		}
		try {
			Intent intent = new Intent();
			intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
				//这种方案适用于 API 26, 即8.0（含8.0）以上可以用
				intent.setAction(Settings.ACTION_APP_NOTIFICATION_SETTINGS);
				intent.putExtra(Settings.EXTRA_APP_PACKAGE, mContext.getPackageName());
				intent.putExtra(Settings.EXTRA_CHANNEL_ID, mContext.getApplicationInfo().uid);
			} else {
				//这种方案适用于 API21——25，即 5.0——7.1 之间的版本可以使用
				intent.putExtra("app_package", mContext.getPackageName());
				intent.putExtra("app_uid", mContext.getApplicationInfo().uid);
			}
			mContext.startActivity(intent);
		} catch (Exception e) {
			// 出现异常则跳转到应用设置界面
			if (mContext != null) {
				Intent intent = new Intent();
				intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
				//下面这种方案是直接跳转到当前应用的设置界面。
				intent.setAction(Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
				Uri uri = Uri.fromParts("package", mContext.getPackageName(), null);
				intent.setData(uri);
				mContext.startActivity(intent);
			};
		}
	}

	public static void jumpToNotificationChannelSetting(String channelId) {
		if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O && mContext != null) {
			Intent intent = new Intent(Settings.ACTION_CHANNEL_NOTIFICATION_SETTINGS);
			intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			intent.putExtra(Settings.EXTRA_APP_PACKAGE, mContext.getPackageName());
			intent.putExtra(Settings.EXTRA_CHANNEL_ID, channelId);
			mContext.startActivity(intent);
		}
	}

	/**
	 * 设置日志等级
	 * @param logLevel 日志等级："off", "error", "warn", "info", "debug"
	 */
	public static void setLogLevel(String logLevel) {
		CloudPushService pushService = PushServiceFactory.getCloudPushService();
		int level = CloudPushService.LOG_INFO; // 默认Info级别
		
		if ("off".equalsIgnoreCase(logLevel)) {
			level = CloudPushService.LOG_OFF;
		} else if ("error".equalsIgnoreCase(logLevel)) {
			level = CloudPushService.LOG_ERROR;
		} else if ("warn".equalsIgnoreCase(logLevel)) {
			// Android没有warn级别，使用error级别
			level = CloudPushService.LOG_ERROR;
		} else if ("info".equalsIgnoreCase(logLevel)) {
			level = CloudPushService.LOG_INFO;
		} else if ("debug".equalsIgnoreCase(logLevel)) {
			level = CloudPushService.LOG_DEBUG;
		}
		
		pushService.setLogLevel(level);
		LOGGER.d("setLogLevel: " + logLevel + " -> " + level);
	}

	/**
	 * 创建通知渠道（Android 8.0+）
	 * @param id 渠道ID
	 * @param name 渠道名称
	 * @param importance 重要性级别
	 * @param desc 渠道描述
	 * @param showBadge 是否显示角标
	 * @param enableLights 是否启用指示灯
	 * @param lightColor 指示灯颜色
	 * @param enableVibration 是否启用震动
	 * @param vibrationPattern 震动模式（逗号分隔的长整型字符串，如"0,250,250,250"）
	 */
	public static void createNotificationChannel(
			String id,
			String name,
			int importance,
			String desc,
			boolean showBadge,
			boolean enableLights,
			int lightColor,
			boolean enableVibration,
			String vibrationPattern) {
		if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
			if (mContext != null) {
				NotificationManager notificationManager = (NotificationManager)mContext.getSystemService(
					NOTIFICATION_SERVICE);
				
				NotificationChannel channel = new NotificationChannel(id, name, importance);
				
				// 设置描述
				if (desc != null && !desc.isEmpty()) {
					channel.setDescription(desc);
				}
				
				// 设置角标
				channel.setShowBadge(showBadge);
				
				// 设置指示灯
				channel.enableLights(enableLights);
				if (enableLights && lightColor != 0) {
					channel.setLightColor(lightColor);
				}
				
				// 设置震动
				channel.enableVibration(enableVibration);
				if (enableVibration && vibrationPattern != null && !vibrationPattern.isEmpty()) {
					String[] parts = vibrationPattern.split(",");
					long[] pattern = new long[parts.length];
					for (int i = 0; i < parts.length; i++) {
						try {
							pattern[i] = Long.parseLong(parts[i].trim());
						} catch (NumberFormatException e) {
							pattern[i] = 0;
						}
					}
					channel.setVibrationPattern(pattern);
				}
				
				notificationManager.createNotificationChannel(channel);
				LOGGER.d("createNotificationChannel: " + id + ", name: " + name);
			}
		}
	}
}
