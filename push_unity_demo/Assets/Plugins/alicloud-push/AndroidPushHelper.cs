#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using AOT;
using System.Threading;

namespace Aliyun.Push.Android {
    public class AndroidPushHelper {
        private static Dictionary<int, Action<bool, string>> CallbackPairs = new Dictionary<int, Action<bool, string>>();
        private static int ActionIndex = 0;
        private static AndroidJavaClass pushWrapper;
        private static Action<string, string, Dictionary<string, string>> customNotificationCallback;
        private static Action<string, string> customMessageReceivedCallback;
        private static Action<string, string, string> customNotificationOpenedCallback;
        private static Action<string, string, string> customNotificationClickedWithNoActionCallback;
        private static Action<string> customNotificationRemovedCallback;
        private static Action<string, string, Dictionary<string, string>, int, string, string> customNotificationReceivedInAppCallback;
        private static Action<string, string, Dictionary<string, string>> customThirdPushNotificationOpenedCallback;

        class PushCallback : AndroidJavaProxy {
            public PushCallback() : base("com.alibaba.sdk.android.pushwrapper.PushCallback") { }
            void callback(int actionId, bool success, string data) {
                Debug.Log(success + " " + data);
                Action<bool, string> callback = CallbackPairs[actionId];
                if (callback != null) {
                    callback(success, data);
                    CallbackPairs.Remove(actionId);
                }
            }
        }

        class NotificationCallback : AndroidJavaProxy {
            public NotificationCallback() : base("com.alibaba.sdk.android.pushwrapper.NotificationCallback") { }
            void onNotification(string title, string summary, AndroidJavaObject extraMap) {
                Dictionary<string, string> dict = JavaMapToDictionary(extraMap);
                Debug.Log("onNotification " + title + "\n summary " + summary);
                if (customNotificationCallback != null) {
                    customNotificationCallback(title, summary, dict);
                }
            }
        }

        class MessageReceivedCallback : AndroidJavaProxy {
            public MessageReceivedCallback() : base("com.alibaba.sdk.android.pushwrapper.MessageReceivedCallback") { }
            void receive(string title, string content) {
                Debug.Log("receive message " + title + "\n content " + content);
                if (customMessageReceivedCallback != null) {
                    customMessageReceivedCallback(title, content);
                }
            }
        }

        class NotificationOpenedCallback : AndroidJavaProxy {
            public NotificationOpenedCallback() : base("com.alibaba.sdk.android.pushwrapper.NotificationOpenedCallback") { }
            void onNotificationOpened(string title, string summary, string extraMap) {
                Debug.Log("onNotificationOpened " + title);
                if (customNotificationOpenedCallback != null) {
                    customNotificationOpenedCallback(title, summary, extraMap);
                }
            }
        }

        class NotificationClickedWithNoActionCallback : AndroidJavaProxy {
            public NotificationClickedWithNoActionCallback() : base("com.alibaba.sdk.android.pushwrapper.NotificationClickedWithNoActionCallback") { }
            void onNotificationClickedWithNoAction(string title, string summary, string extraMap) {
                Debug.Log("onNotificationClickedWithNoAction " + title);
                if (customNotificationClickedWithNoActionCallback != null) {
                    customNotificationClickedWithNoActionCallback(title, summary, extraMap);
                }
            }
        }

        class NotificationRemovedCallback : AndroidJavaProxy {
            public NotificationRemovedCallback() : base("com.alibaba.sdk.android.pushwrapper.NotificationRemovedCallback") { }
            void onNotificationRemoved(string messageId) {
                Debug.Log("onNotificationRemoved " + messageId);
                if (customNotificationRemovedCallback != null) {
                    customNotificationRemovedCallback(messageId);
                }
            }
        }

        class NotificationReceivedInAppCallback : AndroidJavaProxy {
            public NotificationReceivedInAppCallback() : base("com.alibaba.sdk.android.pushwrapper.NotificationReceivedInAppCallback") { }
            void onNotificationReceivedInApp(string title, string summary, AndroidJavaObject extraMap, 
                    int openType, string openActivity, string openUrl) {
                Dictionary<string, string> dict = JavaMapToDictionary(extraMap);
                Debug.Log("onNotificationReceivedInApp " + title);
                if (customNotificationReceivedInAppCallback != null) {
                    customNotificationReceivedInAppCallback(title, summary, dict, openType, openActivity, openUrl);
                }
            }
        }

        class ThirdPushNotificationOpenedCallback : AndroidJavaProxy {
            public ThirdPushNotificationOpenedCallback() : base("com.alibaba.sdk.android.pushwrapper.ThirdPushNotificationOpenedCallback") { }
            void onThirdPushNotificationOpened(string title, string summary, AndroidJavaObject extraMap) {
                Dictionary<string, string> dict = JavaMapToDictionary(extraMap);
                Debug.Log("onThirdPushNotificationOpened " + title);
                if (customThirdPushNotificationOpenedCallback != null) {
                    customThirdPushNotificationOpenedCallback(title, summary, dict);
                }
            }
        }

        private static Dictionary<string, string> JavaMapToDictionary(AndroidJavaObject javaMap) {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (javaMap == null) return dict;
            
            AndroidJavaObject entrySet = javaMap.Call<AndroidJavaObject>("entrySet");
            AndroidJavaObject iterator = entrySet.Call<AndroidJavaObject>("iterator");
            
            while (iterator.Call<bool>("hasNext")) {
                AndroidJavaObject entry = iterator.Call<AndroidJavaObject>("next");
                string key = entry.Call<string>("getKey");
                string value = entry.Call<string>("getValue");
                dict[key] = value;
            }
            
            return dict;
        }

        public static void SetNotificationCallback(Action<string, string, Dictionary<string, string>> notificationCallback) {
            customNotificationCallback = notificationCallback;
        }

        public static void SetMessageReceivedCallback(Action<string, string> messageReceivedCallback) {
            customMessageReceivedCallback = messageReceivedCallback;
        }

        public static void SetNotificationOpenedCallback(Action<string, string, string> notificationOpenedCallback) {
            customNotificationOpenedCallback = notificationOpenedCallback;
        }

        public static void SetNotificationClickedWithNoActionCallback(Action<string, string, string> callback) {
            customNotificationClickedWithNoActionCallback = callback;
        }

        public static void SetNotificationRemovedCallback(Action<string> notificationRemovedCallback) {
            customNotificationRemovedCallback = notificationRemovedCallback;
        }

        public static void SetNotificationReceivedInAppCallback(Action<string, string, Dictionary<string, string>, int, string, string> callback) {
            customNotificationReceivedInAppCallback = callback;
        }

        public static void SetThirdPushNotificationOpenedCallback(Action<string, string, Dictionary<string, string>> callback) {
            customThirdPushNotificationOpenedCallback = callback;
        }

        public static void Register(Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                pushWrapper.CallStatic("setPushCallback", new PushCallback());
                pushWrapper.CallStatic("setNotificationCallback", new NotificationCallback());
                pushWrapper.CallStatic("setMessageReceivedCallback", new MessageReceivedCallback());
                pushWrapper.CallStatic("setNotificationOpenedCallback", new NotificationOpenedCallback());
                pushWrapper.CallStatic("setNotificationClickedWithNoActionCallback", new NotificationClickedWithNoActionCallback());
                pushWrapper.CallStatic("setNotificationRemovedCallback", new NotificationRemovedCallback());
                pushWrapper.CallStatic("setNotificationReceivedInAppCallback", new NotificationReceivedInAppCallback());
                pushWrapper.CallStatic("setThirdPushNotificationOpenedCallback", new ThirdPushNotificationOpenedCallback());
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("register", actionId);
            } else {
                callback(false, "pushWrapper is null");
            }
        }

        public static void RegisterThirdPush() {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                pushWrapper.CallStatic("registerThirdPush");
            } else {
                Debug.Log("pushWrapper is null");
            }
        }

        public static void BindAccount(string account, Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("bindAccount", actionId, account);
            } else {
                callback(false, "pushWrapper is null");
            }
        }

        public static void UnBindAccount(Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("unbindAccount", actionId);
            } else {
                callback(false, "pushWrapper is null");
            }            
        }

        public static void BindTag(int target, string tag, string alias, Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("bindTag", actionId, target, tag, alias);
            } else {
                callback(false, "pushWrapper is null");
            }           
        }

        public static void UnBindTag(int target, string tag, string alias, Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("unbindTag", actionId, target, tag, alias);
            } else {
                callback(false, "pushWrapper is null");
            }            
        }

        public static void ListTag(int target, Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("listTag", actionId, target);
            } else {
                callback(false, "pushWrapper is null");
            }
        }

        public static void AddAlias(string alias, Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("addAlias", actionId, alias);
            } else {
                callback(false, "pushWrapper is null");
            }
        }

        public static void RemoveAlias(string alias, Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("removeAlias", actionId, alias);
            } else {
                callback(false, "pushWrapper is null");
            }
        }

        public static void ListAlias(Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("listAlias", actionId);
            } else {
                callback(false, "pushWrapper is null");
            }
        }

        public static void BindPhone(string phoneName, Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("bindPhone", actionId, phoneName);
            } else {
                callback(false, "pushWrapper is null");
            }
        }

        public static void UnBindPhone(Action<bool, string> callback) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                int actionId = Interlocked.Increment(ref ActionIndex);
                CallbackPairs.Add(actionId, callback);
                pushWrapper.CallStatic("unbindPhone", actionId);
            } else {
                callback(false, "pushWrapper is null");
            }
        }

        public static string DeviceId() {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                return pushWrapper.CallStatic<string>("deviceId");
            } else {
                Debug.Log("pushWrapper is null");
                return null;
            }
        }

        public static bool IsNotificationEnabled() {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                return pushWrapper.CallStatic<bool>("isNotificationEnabled");
            } else {
                Debug.Log("pushWrapper is null");
                return true;
            }
        }

        public static bool IsNotificationChannelEnabled(string channelId) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                return pushWrapper.CallStatic<bool>("isNotificationChannelEnabled", channelId);
            } else {
                Debug.Log("pushWrapper is null");
                return true;
            }
        }

        public static void JumpToNotificationSetting() {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                pushWrapper.CallStatic("jumpToNotificationSetting");
            } else {
                Debug.Log("pushWrapper is null");
            }
        }

        public static void JumpToNotificationChannelSetting(string channelId) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                pushWrapper.CallStatic("jumpToNotificationChannelSetting", channelId);
            } else {
                Debug.Log("pushWrapper is null");
            }
        }

        public static void SetLogLevel(string logLevel) {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                pushWrapper.CallStatic("setLogLevel", logLevel);
            } else {
                Debug.Log("pushWrapper is null");
            }
        }

        public static void CreateNotificationChannel(
            string id,
            string name,
            int importance,
            string desc = "",
            bool showBadge = true,
            bool enableLights = true,
            int lightColor = 0,
            bool enableVibration = true,
            string vibrationPattern = "") {
            AndroidJavaClass pushWrapper = getAndroidPushWrapper();
            if (pushWrapper != null) {
                pushWrapper.CallStatic("createNotificationChannel",
                    id, name, importance, desc, showBadge,
                    enableLights, lightColor, enableVibration, vibrationPattern);
            } else {
                Debug.Log("pushWrapper is null");
            }
        }

        private static AndroidJavaClass getAndroidPushWrapper() {
            if (pushWrapper == null) {
                pushWrapper = new AndroidJavaClass("com.alibaba.sdk.android.pushwrapper.PushWrapper");
                if (pushWrapper != null) {
                    return pushWrapper;
                } else {
                    Debug.Log("pushWrapper is null");
                    return null;
                }
            }
            return pushWrapper;
        }
    }
}
#endif
