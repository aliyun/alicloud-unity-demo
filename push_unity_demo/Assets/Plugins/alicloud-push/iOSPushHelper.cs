#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;
using System.Threading;

namespace Aliyun.Push.IOS {
    public class iOSPushHelper {
        private static Dictionary<int, Action<bool, string>> CallbackPairs = new Dictionary<int, Action<bool, string>>();
        private static int ActionIndex = 0;
        
        // 消息回调
        private static Action<string, string> customMessageReceivedCallback;
        
        // APNS注册回调
        private static Action<string> customAPNSRegisterSuccessCallback;
        private static Action<string> customAPNSRegisterFailedCallback;
        
        // 通知回调
        private static Action<string, string, string, string> customNotificationReceivedCallback;
        private static Action<string, string, string, string> customNotificationOpenedCallback;
        private static Action<string> customNotificationRemovedCallback;
        private static Action<string, string, string, string> customRemoteNotificationReceived;

        public static void Init(string appKey, string appSecret, Action<bool, string> callback) {
            _setCloudPushCallback(CloudPushCallbackReceived);
            _setMessageCallback(MessageReceived);
            _setAPNSRegisterSuccessCallback(APNSRegisterSuccess);
            _setAPNSRegisterFailedCallback(APNSRegisterFailed);
            _setNotificationReceivedCallback(NotificationReceived);
            _setNotificationOpenedCallback(NotificationOpened);
            _setNotificationRemovedCallback(NotificationRemoved);
            _setRemoteNotificationReceivedCallback(RemoteNotificationReceived);
            
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _initCloudPush(appKey, appSecret, actionId);
        }

        public static void BindAccount(string account, Action<bool, string> callback) {
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _bindAccount(account, actionId);
        }

        public static void UnBindAccount(Action<bool, string> callback) {
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _unBindAccount(actionId);
        }

        public static void BindTag(int target, string tag, string alias, Action<bool, string> callback) {
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _bindTag(target, tag, alias, actionId);
        }

        public static void UnBindTag(int target, string tag, string alias, Action<bool, string> callback) {
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _unBindTag(target, tag, alias, actionId);
        }

        public static void ListTag(int target, Action<bool, string> callback) {
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _listTag(target, actionId);
        }

        public static void AddAlias(string alias, Action<bool, string> callback) {
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _addAlias(alias, actionId);
        }

        public static void RemoveAlias(string alias, Action<bool, string> callback) {
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _removeAlias(alias, actionId);
        }

        public static void ListAlias(Action<bool, string> callback) {
            int actionId = Interlocked.Increment(ref ActionIndex);
            CallbackPairs.Add(actionId, callback);
            _listAlias(actionId);
        }

        public static string DeviceId() {
            return _deviceId();
        }

        public static void SetLogLevel(string logLevel) {
            _setLogLevel(logLevel);
        }

        public static void SetForegroundNotificationMode(string mode) {
            _setForegroundNotificationMode(mode);
        }

        public static void SetMessageReceivedCallback(Action<string, string> messageReceivedCallback) {
            customMessageReceivedCallback = messageReceivedCallback;
        }

        public static void SetAPNSRegisterSuccessCallback(Action<string> callback) {
            customAPNSRegisterSuccessCallback = callback;
        }

        public static void SetAPNSRegisterFailedCallback(Action<string> callback) {
            customAPNSRegisterFailedCallback = callback;
        }

        public static void SetNotificationReceivedCallback(Action<string, string, string, string> callback) {
            customNotificationReceivedCallback = callback;
        }

        public static void SetNotificationOpenedCallback(Action<string, string, string, string> callback) {
            customNotificationOpenedCallback = callback;
        }

        public static void SetNotificationRemovedCallback(Action<string> callback) {
            customNotificationRemovedCallback = callback;
        }

        public static void SetRemoteNotificationReceivedCallback(Action<string, string, string, string> callback) {
            customRemoteNotificationReceived = callback;
        }

        [DllImport("__Internal")]
        private static extern void _setCloudPushCallback(CloudPushCallback callback);

        [DllImport("__Internal")]
        private static extern void _setMessageCallback(MessageReceivedCallback callback);

        [DllImport("__Internal")]
        private static extern void _setAPNSRegisterSuccessCallback(APNSRegisterSuccessCallback callback);

        [DllImport("__Internal")]
        private static extern void _setAPNSRegisterFailedCallback(APNSRegisterFailedCallback callback);

        [DllImport("__Internal")]
        private static extern void _setNotificationReceivedCallback(NotificationReceivedCallback callback);

        [DllImport("__Internal")]
        private static extern void _setNotificationOpenedCallback(NotificationOpenedCallback callback);

        [DllImport("__Internal")]
        private static extern void _setNotificationRemovedCallback(NotificationRemovedCallback callback);

        [DllImport("__Internal")]
        private static extern void _setRemoteNotificationReceivedCallback(NotificationReceivedCallback callback);

        [DllImport("__Internal")]
        private static extern void _initCloudPush(string appKey, string appSecret, int actionId);

        [DllImport("__Internal")]
        private static extern void _bindAccount(string account, int actionId);

        [DllImport("__Internal")]
        private static extern void _unBindAccount(int actionId);

        [DllImport("__Internal")]
        private static extern void _bindTag(int target, string tag, string alias, int actionId);

        [DllImport("__Internal")]
        private static extern void _unBindTag(int target, string tag, string alias, int actionId);

        [DllImport("__Internal")]
        private static extern void _listTag(int target, int actionId);

        [DllImport("__Internal")]
        private static extern void _addAlias(string alias, int actionId);

        [DllImport("__Internal")]
        private static extern void _removeAlias(string alias, int actionId);

        [DllImport("__Internal")]
        private static extern void _listAlias(int actionId);

        [DllImport("__Internal")]
        private static extern string _deviceId();

        [DllImport("__Internal")]
        private static extern void _setLogLevel(string logLevel);

        [DllImport("__Internal")]
        private static extern void _setForegroundNotificationMode(string mode);

        private delegate void CloudPushCallback(int actionId, bool success, string msg);
        private delegate void MessageReceivedCallback(string title, string content);
        private delegate void APNSRegisterSuccessCallback(string deviceToken);
        private delegate void APNSRegisterFailedCallback(string error);
        private delegate void NotificationReceivedCallback(string title, string body, string subtitle, string userInfo);
        private delegate void NotificationOpenedCallback(string title, string body, string subtitle, string userInfo);
        private delegate void NotificationRemovedCallback(string userInfo);

        [MonoPInvokeCallback(typeof(CloudPushCallback))]
        public static void CloudPushCallbackReceived(int actionId, bool success, string msg) {
            Debug.Log("CloudPush callback: " + success + " " + msg);
            // call cs code
            if (CallbackPairs.ContainsKey(actionId)) {
                Action<bool, string> callback = CallbackPairs[actionId];
                if (callback != null) {
                    callback(success, msg);
                }
                CallbackPairs.Remove(actionId);
            }
        }

        [MonoPInvokeCallback(typeof(MessageReceivedCallback))]
        public static void MessageReceived(string title, string content) {
            Debug.Log("Message received - Title: " + title + ", Content: " + content);
            if(customMessageReceivedCallback != null) {
                customMessageReceivedCallback(title, content);
            }
        }

        [MonoPInvokeCallback(typeof(APNSRegisterSuccessCallback))]
        public static void APNSRegisterSuccess(string deviceToken) {
            Debug.Log("APNS register success, deviceToken: " + deviceToken);
            if(customAPNSRegisterSuccessCallback != null) {
                customAPNSRegisterSuccessCallback(deviceToken);
            }
        }

        [MonoPInvokeCallback(typeof(APNSRegisterFailedCallback))]
        public static void APNSRegisterFailed(string error) {
            Debug.Log("APNS register failed: " + error);
            if(customAPNSRegisterFailedCallback != null) {
                customAPNSRegisterFailedCallback(error);
            }
        }

        [MonoPInvokeCallback(typeof(NotificationReceivedCallback))]
        public static void NotificationReceived(string title, string body, string subtitle, string userInfo) {
            Debug.Log("Notification received - Title: " + title + ", Body: " + body + ", Subtitle: " + subtitle);
            if(customNotificationReceivedCallback != null) {
                customNotificationReceivedCallback(title, body, subtitle, userInfo);
            }
        }

        [MonoPInvokeCallback(typeof(NotificationOpenedCallback))]
        public static void NotificationOpened(string title, string body, string subtitle, string userInfo) {
            Debug.Log("Notification opened - Title: " + title + ", Body: " + body + ", Subtitle: " + subtitle);
            if(customNotificationOpenedCallback != null) {
                customNotificationOpenedCallback(title, body, subtitle, userInfo);
            }
        }

        [MonoPInvokeCallback(typeof(NotificationRemovedCallback))]
        public static void NotificationRemoved(string userInfo) {
            Debug.Log("Notification removed");
            if(customNotificationRemovedCallback != null) {
                customNotificationRemovedCallback(userInfo);
            }
        }

        [MonoPInvokeCallback(typeof(NotificationReceivedCallback))]
        public static void RemoteNotificationReceived(string title, string body, string subtitle, string userInfo) {
            Debug.Log("Remote notification received - Title: " + title + ", Body: " + body);
            if(customRemoteNotificationReceived != null) {
                customRemoteNotificationReceived(title, body, subtitle, userInfo);
            }
        }
    }
}
#endif
