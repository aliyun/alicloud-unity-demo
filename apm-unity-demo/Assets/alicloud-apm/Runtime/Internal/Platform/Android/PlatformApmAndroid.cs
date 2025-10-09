using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alicloud.Apm
{
#if UNITY_ANDROID && !UNITY_EDITOR
    internal class PlatformApmAndroid : IPlatformApm
    {
        public static string APM = "com.aliyun.emas.apm.Apm";
        private static string APM_OPTIONS_BUILDER = "com.aliyun.emas.apm.ApmOptions$Builder";
        private static string APM_CRASH_ANALYSIS_COMPONENT =
            "com.aliyun.emas.apm.crash.ApmCrashAnalysisComponent";

        public void SetCustomKeysAndValues(IDictionary<string, object> keysAndValues)
        {
            if (keysAndValues == null || keysAndValues.Count == 0)
            {
                return;
            }

            foreach (var pair in keysAndValues)
            {
                SetCustomKeyValue(pair.Key, pair.Value);
            }
        }

        public void SetCustomKeyValue(string key, object value)
        {
            if (key == null || value == null)
            {
                return;
            }

            try
            {
                AndroidJavaClass apmClass = new AndroidJavaClass(APM);
                apmClass.CallStatic("setCustomKey", key, value.ToString());
            }
            catch
            {
                ApmLogger.Error($"Failed to set custom value: {key} = {value}");
            }
        }

        public void SetUserId(string userId)
        {
            try
            {
                AndroidJavaClass apmClass = new AndroidJavaClass(APM);
                apmClass.CallStatic("setUserId", userId);
            }
            catch
            {
                ApmLogger.Error($"Failed to set user ID: {userId}");
            }
        }

        public void SetUserNick(string userNick)
        {
            try
            {
                AndroidJavaClass apmClass = new AndroidJavaClass(APM);
                apmClass.CallStatic("setUserNick", userNick);
            }
            catch
            {
                ApmLogger.Error($"Failed to set user nick: {userNick}");
            }
        }

        public void Start(ApmOptions options)
        {
            try
            {
                AndroidJavaClass apmClass = new AndroidJavaClass(APM);

                AndroidJavaObject optionsBuilder = new AndroidJavaObject(APM_OPTIONS_BUILDER);
                optionsBuilder = optionsBuilder.Call<AndroidJavaObject>(
                    "setAppKey",
                    options.AppKey
                );
                optionsBuilder = optionsBuilder.Call<AndroidJavaObject>(
                    "setAppSecret",
                    options.AppSecret
                );
                optionsBuilder = optionsBuilder.Call<AndroidJavaObject>(
                    "setAppRsaSecret",
                    options.AppRsaSecret
                );
                optionsBuilder = optionsBuilder.Call<AndroidJavaObject>(
                    "setChannel",
                    options.Channel
                );
                optionsBuilder = optionsBuilder.Call<AndroidJavaObject>(
                    "setUserId",
                    options.UserId
                );
                optionsBuilder = optionsBuilder.Call<AndroidJavaObject>(
                    "setUserNick",
                    options.UserNick
                );

                if (options.SdkComponents.HasFlag(SDKComponents.CrashAnalysis))
                {
                    AndroidJavaClass crashAnalysisComponent = new AndroidJavaClass(
                        APM_CRASH_ANALYSIS_COMPONENT
                    );
                    optionsBuilder = optionsBuilder.Call<AndroidJavaObject>(
                        "addComponent",
                        crashAnalysisComponent
                    );
                }

                AndroidJavaClass unityPlayer = new AndroidJavaClass(
                    "com.unity3d.player.UnityPlayer"
                );
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>(
                    "currentActivity"
                );
                AndroidJavaObject applicationContext = currentActivity.Call<AndroidJavaObject>(
                    "getApplicationContext"
                );
                optionsBuilder = optionsBuilder.Call<AndroidJavaObject>(
                    "setApplication",
                    applicationContext
                );

                AndroidJavaObject androidOptions = optionsBuilder.Call<AndroidJavaObject>("build");

                apmClass.CallStatic("preStart", androidOptions);
                apmClass.CallStatic<bool>("start");
            }
            catch
            {
                ApmLogger.Error("Failed to start APM on Android platform");
            }
        }
    }
#endif
}
