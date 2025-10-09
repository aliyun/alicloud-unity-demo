#nullable enable

using Alicloud.Apm;
using UnityEngine;

namespace AlicloudApmDemo
{
    internal static class ApmDemoBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (Apm.IsStarted())
            {
                return;
            }
            try
            {
                var options = BuildOptions();
                if (options == null)
                {
                    Debug.Log(
                        "[ApmDemo] Alibaba Cloud APM SDK not supported on this platform; startup skipped."
                    );
                    return;
                }

                Apm.Start(options);
                Debug.Log("[ApmDemo] Alibaba Cloud APM SDK started.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ApmDemo] Failed to start APM SDK: {ex.Message}");
            }
        }

        private static ApmOptions? BuildOptions()
        {
#if UNITY_IOS
            const string appKey = "Your iOS AppKey";
            const string appSecret = "Your iOS AppSecret";
            const string appRsaSecret = "Your iOS AppRsaSecret";

            return new ApmOptions(appKey, appSecret, appRsaSecret)
            {
                // 可选配置项
                UserNick = "Your UserNick",
                UserId = "Your UserId",
            };
#elif UNITY_ANDROID
            const string appKey = "Your Android AppKey";
            const string appSecret = "Your Android AppSecret";
            const string appRsaSecret = "Your Android AppRsaSecret";

            return new ApmOptions(appKey, appSecret, appRsaSecret)
            {
                // 可选配置项
                UserNick = "Your UserNick",
                UserId = "Your UserId",
            };
#else
            return null;
#endif
        }
    }
}
