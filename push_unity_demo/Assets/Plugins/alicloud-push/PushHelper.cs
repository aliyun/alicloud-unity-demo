using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using AOT;
#if UNITY_IOS
using Aliyun.Push.IOS;
#elif UNITY_ANDROID
using Aliyun.Push.Android;
#endif

namespace Aliyun.Push {
    public class PushHelper {

        // ============================================
        // 第一部分：跨平台方法（Android 和 iOS 都支持）
        // ============================================

        /// <summary>
        /// 初始化推送服务
        /// </summary>
        /// <param name="appKey">应用的 AppKey</param>
        /// <param name="appSecret">应用的 AppSecret</param>
        /// <param name="callback">初始化结果回调，参数为 (是否成功, 结果信息)</param>
        public static void InitPush(string appKey, string appSecret, Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.Init(appKey, appSecret, callback);
#elif UNITY_ANDROID
            AndroidPushHelper.Register(callback);
#endif
        }

        /// <summary>
        /// 绑定账号
        /// </summary>
        /// <param name="account">账号名称</param>
        /// <param name="callback">绑定结果回调，参数为 (是否成功, 结果信息)</param>
        public static void BindAccount(string account, Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.BindAccount(account, callback);
#elif UNITY_ANDROID
            AndroidPushHelper.BindAccount(account, callback);
#endif
        }

        /// <summary>
        /// 解绑账号
        /// </summary>
        /// <param name="callback">解绑结果回调，参数为 (是否成功, 结果信息)</param>
        public static void UnbindAccount(Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.UnBindAccount(callback);
#elif UNITY_ANDROID
            AndroidPushHelper.UnBindAccount(callback);
#endif
        }

        /// <summary>
        /// 绑定标签
        /// </summary>
        /// <param name="target">目标类型（1=设备, 2=账号, 3=别名）</param>
        /// <param name="tag">标签名称</param>
        /// <param name="alias">别名（当 target=3 时使用）</param>
        /// <param name="callback">绑定结果回调，参数为 (是否成功, 结果信息)</param>
        public static void BindTag(int target, string tag, string alias, Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.BindTag(target, tag, alias, callback);
#elif UNITY_ANDROID
            AndroidPushHelper.BindTag(target, tag, alias, callback);
#endif
        }

        /// <summary>
        /// 解绑标签
        /// </summary>
        /// <param name="target">目标类型（1=设备, 2=账号, 3=别名）</param>
        /// <param name="tag">标签名称</param>
        /// <param name="alias">别名（当 target=3 时使用）</param>
        /// <param name="callback">解绑结果回调，参数为 (是否成功, 结果信息)</param>
        public static void UnbindTag(int target, string tag, string alias, Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.UnBindTag(target, tag, alias, callback);
#elif UNITY_ANDROID
            AndroidPushHelper.UnBindTag(target, tag, alias, callback);
#endif
        }

        /// <summary>
        /// 查询标签列表
        /// </summary>
        /// <param name="target">目标类型（1=设备, 2=账号, 3=别名）</param>
        /// <param name="callback">查询结果回调，参数为 (是否成功, 标签列表JSON字符串)</param>
        public static void ListTags(int target, Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.ListTag(target, callback);
#elif UNITY_ANDROID
            AndroidPushHelper.ListTag(target, callback);
#endif
        }

        /// <summary>
        /// 添加别名
        /// </summary>
        /// <param name="alias">别名名称</param>
        /// <param name="callback">添加结果回调，参数为 (是否成功, 结果信息)</param>
        public static void AddAlias(string alias, Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.AddAlias(alias, callback);
#elif UNITY_ANDROID
            AndroidPushHelper.AddAlias(alias, callback);
#endif
        }

        /// <summary>
        /// 移除别名
        /// </summary>
        /// <param name="alias">别名名称</param>
        /// <param name="callback">移除结果回调，参数为 (是否成功, 结果信息)</param>
        public static void RemoveAlias(string alias, Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.RemoveAlias(alias, callback);
#elif UNITY_ANDROID
            AndroidPushHelper.RemoveAlias(alias, callback);
#endif
        }

        /// <summary>
        /// 查询别名列表
        /// </summary>
        /// <param name="callback">查询结果回调，参数为 (是否成功, 别名列表JSON字符串)</param>
        public static void ListAliases(Action<bool, string> callback) {
#if UNITY_IOS
            iOSPushHelper.ListAlias(callback);
#elif UNITY_ANDROID
            AndroidPushHelper.ListAlias(callback);
#endif
        }

        /// <summary>
        /// 获取设备唯一标识符
        /// </summary>
        /// <returns>设备ID字符串</returns>
        public static string GetDeviceId() {
#if UNITY_IOS
            return iOSPushHelper.DeviceId();
#elif UNITY_ANDROID
            return AndroidPushHelper.DeviceId();
#endif
        return "";
        }

        /// <summary>
        /// 设置日志等级
        /// </summary>
        /// <param name="logLevel">日志等级："off", "error", "warn", "info", "debug"</param>
        public static void SetLogLevel(string logLevel) {
#if UNITY_IOS
            iOSPushHelper.SetLogLevel(logLevel);
#elif UNITY_ANDROID
            AndroidPushHelper.SetLogLevel(logLevel);
#endif
        }

        // 日志等级常量
        public const string LOG_OFF = "off";
        public const string LOG_ERROR = "error";
        public const string LOG_WARN = "warn";
        public const string LOG_INFO = "info";
        public const string LOG_DEBUG = "debug";

        // ============================================
        // 第二部分：Android 特有方法
        // ============================================

#if UNITY_ANDROID
        /// <summary>
        /// 初始化 Android 第三方推送通道（小米、华为、OPPO、vivo 等厂商通道）
        /// </summary>
        public static void InitAndroidThirdPush() {
            AndroidPushHelper.RegisterThirdPush();
        }

        /// <summary>
        /// 绑定手机号码
        /// </summary>
        /// <param name="phoneName">手机号码</param>
        /// <param name="callback">绑定结果回调，参数为 (是否成功, 结果信息)</param>
        public static void BindPhoneNumber(string phoneName, Action<bool, string> callback) {
            AndroidPushHelper.BindPhone(phoneName, callback);
        }

        /// <summary>
        /// 解绑手机号码
        /// </summary>
        /// <param name="callback">解绑结果回调，参数为 (是否成功, 结果信息)</param>
        public static void UnbindPhoneNumber(Action<bool, string> callback) {
            AndroidPushHelper.UnBindPhone(callback);
        }

        /// <summary>
        /// 设置 Android 消息接收器（统一设置所有回调）
        /// </summary>
        /// <param name="onMessage">阿里云通道通消息到达回调</param>
        /// <param name="onNotification">阿里云通道通知到达回调</param>
        /// <param name="onNotificationOpened">阿里云通道通通知点击回调</param>
        /// <param name="onNotificationClickedWithNoAction">阿里云通道通通知点击无动作回调（推送时设置AndroidOpenType为NONE才会调用）</param>
        /// <param name="onNotificationRemoved">阿里云通道通通知移除回调</param>
        /// <param name="onNotificationReceivedInApp">阿里云通道通应用内通知接收回调（PushMessageReceiver中覆写showNotificationNow方法返回false才会调用）</param>
        /// <param name="onSysNoticeOpened">厂商通道通知点击回调</param>
        public static void SetAndroidMessageReceiver(
            Action<string, string> onMessage = null,
            Action<string, string, Dictionary<string, string>> onNotification = null,
            Action<string, string, string> onNotificationOpened = null,
            Action<string, string, string> onNotificationClickedWithNoAction = null,
            Action<string> onNotificationRemoved = null,
            Action<string, string, Dictionary<string, string>, int, string, string> onNotificationReceivedInApp = null,
            Action<string, string, Dictionary<string, string>> onSysNoticeOpened = null) {
            
            if (onNotification != null) {
                AndroidPushHelper.SetNotificationCallback(onNotification);
            }
            if (onMessage != null) {
                AndroidPushHelper.SetMessageReceivedCallback(onMessage);
            }
            if (onNotificationOpened != null) {
                AndroidPushHelper.SetNotificationOpenedCallback(onNotificationOpened);
            }
            if (onNotificationClickedWithNoAction != null) {
                AndroidPushHelper.SetNotificationClickedWithNoActionCallback(onNotificationClickedWithNoAction);
            }
            if (onNotificationRemoved != null) {
                AndroidPushHelper.SetNotificationRemovedCallback(onNotificationRemoved);
            }
            if (onNotificationReceivedInApp != null) {
                AndroidPushHelper.SetNotificationReceivedInAppCallback(onNotificationReceivedInApp);
            }
            if (onSysNoticeOpened != null) {
                AndroidPushHelper.SetThirdPushNotificationOpenedCallback(onSysNoticeOpened);
            }
        }

        /// <summary>
        /// 检查通知权限是否已开启
        /// </summary>
        /// <returns>true=已开启, false=未开启</returns>
        public static bool IsNotificationEnabled() {
            return AndroidPushHelper.IsNotificationEnabled();
        }

        /// <summary>
        /// 检查指定通知渠道是否已开启
        /// </summary>
        /// <param name="channelId">通知渠道ID</param>
        /// <returns>true=已开启, false=未开启</returns>
        public static bool IsNotificationChannelEnabled(string channelId) {
            return AndroidPushHelper.IsNotificationChannelEnabled(channelId);
        }

        /// <summary>
        /// 跳转到应用通知设置页面
        /// </summary>
        public static void JumpToNotificationSetting() {
            AndroidPushHelper.JumpToNotificationSetting();
        }

        /// <summary>
        /// 跳转到指定通知渠道设置页面
        /// </summary>
        /// <param name="channelId">通知渠道ID</param>
        public static void JumpToNotificationChannelSetting(string channelId) {
            AndroidPushHelper.JumpToNotificationChannelSetting(channelId);
        }

        /// <summary>
        /// 创建通知渠道（Android 8.0+）
        /// </summary>
        /// <param name="id">渠道ID</param>
        /// <param name="name">渠道名称</param>
        /// <param name="importance">重要性级别（0-5）</param>
        /// <param name="desc">渠道描述</param>
        /// <param name="showBadge">是否显示角标</param>
        /// <param name="enableLights">是否启用指示灯</param>
        /// <param name="lightColor">指示灯颜色（ARGB格式，如0xFFFF0000表示红色）</param>
        /// <param name="enableVibration">是否启用震动</param>
        /// <param name="vibrationPattern">震动模式（逗号分隔，如"0,250,250,250"）</param>
        public static void CreateNotificationChannel(
            string id,
            string name,
            int importance = 3,
            string desc = "",
            bool showBadge = true,
            bool enableLights = true,
            int lightColor = 0,
            bool enableVibration = true,
            string vibrationPattern = "") {
            AndroidPushHelper.CreateNotificationChannel(
                id, name, importance, desc, showBadge,
                enableLights, lightColor, enableVibration, vibrationPattern);
        }

        // 通知重要性级别常量（Android）
        public const int IMPORTANCE_NONE = 0;      // 不显示通知
        public const int IMPORTANCE_MIN = 1;       // 最低级别，不发出声音
        public const int IMPORTANCE_LOW = 2;       // 低级别，不发出声音
        public const int IMPORTANCE_DEFAULT = 3;   // 默认级别，发出声音
        public const int IMPORTANCE_HIGH = 4;      // 高级别，发出声音并弹出
        public const int IMPORTANCE_MAX = 5;       // 最高级别
#endif

        // ============================================
        // 第三部分：iOS 特有方法
        // ============================================

#if UNITY_IOS
        /// <summary>
        /// 设置 iOS 消息接收器（统一设置所有回调）
        /// </summary>
        /// <param name="onMessageReceived">阿里云自有通道消息接收回调</param>
        /// <param name="onAPNSRegisterSuccess">APNS注册成功回调</param>
        /// <param name="onAPNSRegisterFailed">APNS注册失败回调</param>
        /// <param name="onForegroundNotificationReceived">前台通知接收回调</param>
        /// <param name="onNotificationOpened">通知点击回调</param>
        /// <param name="onNotificationRemoved">通知移除回调</param>
        /// <param name="onRemoteNotificationReceived">静默通知接收回调</param>
        public static void SetIOSMessageReceiver(
            Action<string, string> onMessageReceived = null,
            Action<string> onAPNSRegisterSuccess = null,
            Action<string> onAPNSRegisterFailed = null,
            Action<string, string, string, string> onForegroundNotificationReceived = null,
            Action<string, string, string, string> onNotificationOpened = null,
            Action<string> onNotificationRemoved = null,
            Action<string, string, string, string> onRemoteNotificationReceived = null) {
            
            if (onMessageReceived != null) {
                iOSPushHelper.SetMessageReceivedCallback(onMessageReceived);
            }
            if (onAPNSRegisterSuccess != null) {
                iOSPushHelper.SetAPNSRegisterSuccessCallback(onAPNSRegisterSuccess);
            }
            if (onAPNSRegisterFailed != null) {
                iOSPushHelper.SetAPNSRegisterFailedCallback(onAPNSRegisterFailed);
            }
            if (onForegroundNotificationReceived != null) {
                iOSPushHelper.SetNotificationReceivedCallback(onForegroundNotificationReceived);
            }
            if (onNotificationOpened != null) {
                iOSPushHelper.SetNotificationOpenedCallback(onNotificationOpened);
            }
            if (onNotificationRemoved != null) {
                iOSPushHelper.SetNotificationRemovedCallback(onNotificationRemoved);
            }
            if (onRemoteNotificationReceived != null) {
                iOSPushHelper.SetRemoteNotificationReceivedCallback(onRemoteNotificationReceived);
            }
        }

        /// <summary>
        /// 设置 iOS 前台通知处理模式
        /// </summary>
        /// <param name="mode">处理模式：使用常量 IOS_FOREGROUND_SHOW_AND_CALLBACK / IOS_FOREGROUND_SHOW_ONLY / IOS_FOREGROUND_CALLBACK_ONLY</param>
        public static void SetIOSForegroundNotificationMode(string mode) {
            iOSPushHelper.SetForegroundNotificationMode(mode);
        }

        // iOS 前台通知处理模式常量
        public const string IOS_FOREGROUND_SHOW_AND_CALLBACK = "show_and_callback";  // 展示通知且调用回调
        public const string IOS_FOREGROUND_SHOW_ONLY = "show_only";                  // 只展示通知，不调用回调
        public const string IOS_FOREGROUND_CALLBACK_ONLY = "callback_only";          // 只调用回调，不展示通知
#endif
    }
}
