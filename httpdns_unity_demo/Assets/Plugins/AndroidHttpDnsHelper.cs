using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aliyun.HttpDns
{
    /// <summary>
    /// Android-specific implementation of HTTP DNS service using Alibaba Cloud SDK
    /// This class uses JNI to communicate with the native Android HTTP DNS SDK
    /// Implements staged initialization pattern
    /// </summary>
    public class AndroidHttpDnsService : IHttpDnsService
    {
        private AndroidJavaClass httpDnsClass;
        private AndroidJavaObject httpDnsService;
        private AndroidJavaObject currentActivity;
        private AndroidJavaObject initConfigBuilder;
        private string pendingAccountId;
        private string pendingSecretKey;
        private string pendingAesSecretKey;
        private bool isBuilt = false;
        
        // Pending configuration states (collected before build)
        private bool pendingEnableHttps = true;
        private bool pendingEnableCacheIp = true;
        private bool pendingEnableExpiredIp = true;
        private bool pendingPreResolveAfterNetworkChanged = true;
        private int pendingTimeout = 2000;

        private const string HTTP_DNS_CLASS = "com.alibaba.sdk.android.httpdns.HttpDns";
        private const string INIT_CONFIG_CLASS = "com.alibaba.sdk.android.httpdns.InitConfig";
        private const string REGION_CLASS = "com.alibaba.sdk.android.httpdns.Region";
        private const string REQUEST_IP_TYPE_CLASS = "com.alibaba.sdk.android.httpdns.RequestIpType";

        public void Initialize(string accountId, string secretKey)
        {
            Initialize(accountId, secretKey, null);
        }
        
        public void Initialize(string accountId, string secretKey, string aesSecretKey)
        {
            pendingAccountId = accountId;
            pendingSecretKey = secretKey;
            pendingAesSecretKey = aesSecretKey;
            
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }

                if (currentActivity != null)
                {
                    try
                    {
                        httpDnsClass = new AndroidJavaClass(HTTP_DNS_CLASS);
                    }
                    catch
                    {
                        httpDnsClass = null;
                    }
                }
            }
            catch { }
        }
        
        public bool Build()
        {
            if (isBuilt) return true;
            if (httpDnsClass == null) return false;
            
            try
            {
                CreateInitConfigWithPendingSettings();
                
                httpDnsClass.CallStatic("init", pendingAccountId, initConfigBuilder.Call<AndroidJavaObject>("build"));

                if (!string.IsNullOrEmpty(pendingSecretKey))
                {
                    httpDnsService = httpDnsClass.CallStatic<AndroidJavaObject>("getService", currentActivity, pendingAccountId, pendingSecretKey);
                }
                else
                {
                    httpDnsService = httpDnsClass.CallStatic<AndroidJavaObject>("getService", currentActivity, pendingAccountId);
                }

                if (httpDnsService != null)
                {
                    isBuilt = true;
                    return true;
                }
            }
            catch { }
            
            return false;
        }

        private void CreateInitConfigWithPendingSettings()
        {
            initConfigBuilder = new AndroidJavaObject(INIT_CONFIG_CLASS + "$Builder");

            using (AndroidJavaClass regionClass = new AndroidJavaClass(REGION_CLASS))
            {
                AndroidJavaObject defaultRegion = regionClass.GetStatic<AndroidJavaObject>("DEFAULT");
                initConfigBuilder = initConfigBuilder.Call<AndroidJavaObject>("setRegion", defaultRegion);
            }

            initConfigBuilder = initConfigBuilder.Call<AndroidJavaObject>("setContext", currentActivity);

            if (!string.IsNullOrEmpty(pendingSecretKey))
            {
                initConfigBuilder = initConfigBuilder.Call<AndroidJavaObject>("setSecretKey", pendingSecretKey);
            }
            
            if (!string.IsNullOrEmpty(pendingAesSecretKey))
            {
                initConfigBuilder = initConfigBuilder.Call<AndroidJavaObject>("setAesSecretKey", pendingAesSecretKey);
            }

            try { initConfigBuilder = initConfigBuilder.Call<AndroidJavaObject>("setEnableHttps", pendingEnableHttps); } catch { }
            try { initConfigBuilder = initConfigBuilder.Call<AndroidJavaObject>("setPreResolveAfterNetworkChanged", pendingPreResolveAfterNetworkChanged); } catch { }
        }

        public void SetHttpsEnabled(bool enable)
        {
            pendingEnableHttps = enable;
        }

        public List<string> GetIpsByHost(string host)
        {
            if (!EnsureBuild()) return null;

            try
            {
                using (AndroidJavaClass requestIpTypeClass = new AndroidJavaClass(REQUEST_IP_TYPE_CLASS))
                {
                    AndroidJavaObject autoType = requestIpTypeClass.GetStatic<AndroidJavaObject>("auto");
                    AndroidJavaObject result = httpDnsService.Call<AndroidJavaObject>("getHttpDnsResultForHostSyncNonBlocking", host, autoType);

                    if (result != null)
                    {
                        List<string> ips = new List<string>();

                        string[] ipv4s = result.Call<string[]>("getIps");
                        if (ipv4s != null)
                        {
                            foreach (string ip in ipv4s)
                            {
                                if (!string.IsNullOrEmpty(ip)) ips.Add(ip);
                            }
                        }

                        string[] ipv6s = result.Call<string[]>("getIpv6s");
                        if (ipv6s != null)
                        {
                            foreach (string ip in ipv6s)
                            {
                                if (!string.IsNullOrEmpty(ip)) ips.Add(ip);
                            }
                        }

                        return ips.Count > 0 ? ips : null;
                    }
                }
            }
            catch { }

            return null;
        }
        
        public ResolveResult ResolveHostSyncNonBlocking(string host, string ipType, Dictionary<string, string> sdnsParams, string cacheKey)
        {
            if (!EnsureBuild()) return null;

            try
            {
                using (AndroidJavaClass requestIpTypeClass = new AndroidJavaClass(REQUEST_IP_TYPE_CLASS))
                {
                    AndroidJavaObject requestType = GetRequestIpType(ipType, requestIpTypeClass);
                    AndroidJavaObject result;
                    
                    if (sdnsParams != null && !string.IsNullOrEmpty(cacheKey))
                    {
                        var javaParams = ConvertToJavaMap(sdnsParams);
                        result = httpDnsService.Call<AndroidJavaObject>("getHttpDnsResultForHostSyncNonBlocking", host, requestType, javaParams, cacheKey);
                    }
                    else
                    {
                        result = httpDnsService.Call<AndroidJavaObject>("getHttpDnsResultForHostSyncNonBlocking", host, requestType);
                    }

                    if (result != null)
                    {
                        var enhancedResult = new ResolveResult();

                        string[] ipv4s = result.Call<string[]>("getIps");
                        if (ipv4s != null)
                        {
                            enhancedResult.IPv4.AddRange(ipv4s.Where(ip => !string.IsNullOrEmpty(ip)));
                        }

                        string[] ipv6s = result.Call<string[]>("getIpv6s");
                        if (ipv6s != null)
                        {
                            enhancedResult.IPv6.AddRange(ipv6s.Where(ip => !string.IsNullOrEmpty(ip)));
                        }

                        try { enhancedResult.TTL = result.Call<int>("getTtl"); } catch { }

                        return enhancedResult;
                    }
                }
            }
            catch { }

            return null;
        }

        public void PreResolveHost(string host)
        {
            if (!EnsureBuild()) return;

            try
            {
                using (AndroidJavaClass requestIpTypeClass = new AndroidJavaClass(REQUEST_IP_TYPE_CLASS))
                {
                    AndroidJavaObject autoType = requestIpTypeClass.GetStatic<AndroidJavaObject>("auto");
                    httpDnsService.Call("asyncResolve", host, autoType);
                }
            }
            catch { }
        }
        
        public void PreResolveHosts(List<string> hosts, string ipType)
        {
            if (!EnsureBuild() || hosts == null) return;
            
            try
            {
                using (AndroidJavaClass requestIpTypeClass = new AndroidJavaClass(REQUEST_IP_TYPE_CLASS))
                {
                    AndroidJavaObject requestTypeObj = GetRequestIpType(ipType, requestIpTypeClass);
                    var javaList = ConvertToJavaList(hosts);
                    httpDnsService.Call("setPreResolveHosts", javaList, requestTypeObj);
                }
            }
            catch { }
        }

        public void SetTimeout(int timeoutMs)
        {
            pendingTimeout = timeoutMs;
        }
        
        public void SetPersistentCacheIPEnabled(bool enable, int discardAfterSeconds)
        {
            pendingEnableCacheIp = enable;
        }
        
        public void SetReuseExpiredIPEnabled(bool enable)
        {
            pendingEnableExpiredIp = enable;
        }
        
        public void SetPreResolveAfterNetworkChanged(bool enable)
        {
            pendingPreResolveAfterNetworkChanged = enable;
        }
        
        public string GetSessionId()
        {
            if (!EnsureBuild()) return null;
            
            try
            {
                return httpDnsService.Call<string>("getSessionId");
            }
            catch
            {
                return null;
            }
        }

        public void ClearCache()
        {
            if (!EnsureBuild()) return;

            try
            {
                var emptyList = new AndroidJavaObject("java.util.ArrayList");
                httpDnsService.Call("cleanHostCache", emptyList);
            }
            catch { }
        }
        
        private AndroidJavaObject GetRequestIpType(string ipType, AndroidJavaClass requestIpTypeClass)
        {
            switch (ipType?.ToLower())
            {
                case "ipv4":
                case "v4":
                    return requestIpTypeClass.GetStatic<AndroidJavaObject>("v4");
                case "ipv6":
                case "v6":
                    return requestIpTypeClass.GetStatic<AndroidJavaObject>("v6");
                case "both":
                case "64":
                    return requestIpTypeClass.GetStatic<AndroidJavaObject>("both");
                default:
                    return requestIpTypeClass.GetStatic<AndroidJavaObject>("auto");
            }
        }
        
        private AndroidJavaObject ConvertToJavaMap(Dictionary<string, string> dict)
        {
            var hashMap = new AndroidJavaObject("java.util.HashMap");
            foreach (var kvp in dict)
            {
                hashMap.Call<AndroidJavaObject>("put", kvp.Key, kvp.Value);
            }
            return hashMap;
        }
        
        private AndroidJavaObject ConvertToJavaList(List<string> list)
        {
            var arrayList = new AndroidJavaObject("java.util.ArrayList");
            foreach (var item in list)
            {
                arrayList.Call<bool>("add", item);
            }
            return arrayList;
        }

        private bool EnsureBuild()
        {
            if (!isBuilt || httpDnsService == null)
            {
                Debug.LogError("[AndroidHttpDnsService] Service not built. Call build() first.");
                return false;
            }
            return true;
        }

        ~AndroidHttpDnsService()
        {
            try
            {
                httpDnsClass?.Dispose();
                httpDnsService?.Dispose();
                currentActivity?.Dispose();
                initConfigBuilder?.Dispose();
            }
            catch { }
        }
    }
}