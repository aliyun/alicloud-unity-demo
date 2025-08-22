using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// iOS-specific HttpDNS implementation using Alibaba Cloud HTTPDNS iOS SDK
/// Following official documentation: https://help.aliyun.com/document_detail/2868038.html
/// </summary>
public class iOSHttpDnsHelper 
{
#if UNITY_IOS && !UNITY_EDITOR
    
    // Native iOS method imports
    [DllImport("__Internal")]
    private static extern void _httpdns_init(string accountId, string secretKey);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_setLogEnabled(bool enabled);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_setHTTPSRequestEnabled(bool enabled);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_setPersistentCacheIPEnabled(bool enabled);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_setReuseExpiredIPEnabled(bool enabled);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_setIPv6Enabled(bool enabled);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_setNetworkingTimeoutInterval(int timeoutSeconds);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_setRegion(int region);
    
    [DllImport("__Internal")]
    private static extern string _httpdns_resolveHostSyncNonBlocking(string hostname);
    
    [DllImport("__Internal")]
    private static extern string _httpdns_resolveHostSync(string hostname);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_preResolveHosts(string hostname);
    
    [DllImport("__Internal")]
    private static extern void _httpdns_clearCache();
    
    [DllImport("__Internal")]
    private static extern bool _httpdns_isInitialized();

    // Constants for region settings
    private const int ALICLOUD_HTTPDNS_DEFAULT_REGION_KEY = 0;
    private const int ALICLOUD_HTTPDNS_SINGAPORE_REGION_KEY = 1;
    private const int ALICLOUD_HTTPDNS_HK_REGION_KEY = 2;
    
#endif

    public static void Init(string accountId, string secretKey = "")
    {
#if UNITY_IOS && !UNITY_EDITOR
        _httpdns_init(accountId, secretKey);
#endif
    }

    public static void SetLogEnabled(bool enabled)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _httpdns_setLogEnabled(enabled);
#endif
    }

    public static void SetHTTPSRequestEnabled(bool enabled)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _httpdns_setHTTPSRequestEnabled(enabled);
#endif
    }

    public static void SetPersistentCacheIPEnabled(bool enabled)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _httpdns_setPersistentCacheIPEnabled(enabled);
#endif
    }
    
    public static void SetPersistentCacheIPEnabled(bool enabled, int discardAfterSeconds)
    {
        SetPersistentCacheIPEnabled(enabled);
    }

    public static void SetReuseExpiredIPEnabled(bool enabled)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _httpdns_setReuseExpiredIPEnabled(enabled);
#endif
    }

    public static void SetIPv6Enabled(bool enabled)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _httpdns_setIPv6Enabled(enabled);
#endif
    }

    public static void SetTimeout(int timeoutMs)
    {
#if UNITY_IOS && !UNITY_EDITOR
        int timeoutSeconds = Mathf.Max(1, timeoutMs / 1000);
        _httpdns_setNetworkingTimeoutInterval(timeoutSeconds);
#endif
    }

    public static void SetRegion(int region = 0)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _httpdns_setRegion(region);
#endif
    }

    public static List<string> GetIpsByHostAsync(string hostname)
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (string.IsNullOrEmpty(hostname)) return null;

        string result = _httpdns_resolveHostSyncNonBlocking(hostname);
        if (string.IsNullOrEmpty(result)) return null;

        return ParseResolveResult(result);
#else
        return null;
#endif
    }

    public static List<string> GetIpsByHostSync(string hostname)
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (string.IsNullOrEmpty(hostname)) return null;

        string result = _httpdns_resolveHostSync(hostname);
        if (string.IsNullOrEmpty(result)) return null;

        return ParseResolveResult(result);
#else
        return null;
#endif
    }

    public static void SetPreResolveHosts(string hostname)
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (!string.IsNullOrEmpty(hostname))
        {
            _httpdns_preResolveHosts(hostname);
        }
#endif
    }

    public static void ClearCache()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _httpdns_clearCache();
#endif
    }

    public static bool IsInitialized()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _httpdns_isInitialized();
#else
        return false;
#endif
    }
    
    public static void SetPreResolveAfterNetworkChanged(bool enabled)
    {
        // iOS SDK handles this internally
    }
    
    public static string GetSessionId()
    {
        return $"ios_session_{DateTime.Now.Ticks % 100000}";
    }

    private static List<string> ParseResolveResult(string jsonResult)
    {
        var ips = new List<string>();
        
        try
        {
            if (string.IsNullOrEmpty(jsonResult)) return ips;

            var ipsMatch = System.Text.RegularExpressions.Regex.Match(jsonResult, @"""ips"":\[(.*?)\]");
            if (ipsMatch.Success)
            {
                string ipsString = ipsMatch.Groups[1].Value;
                var matches = System.Text.RegularExpressions.Regex.Matches(ipsString, @"""([^""]+)""");
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    ips.Add(match.Groups[1].Value);
                }
            }

            var ipv6sMatch = System.Text.RegularExpressions.Regex.Match(jsonResult, @"""ipv6s"":\[(.*?)\]");
            if (ipv6sMatch.Success)
            {
                string ipv6sString = ipv6sMatch.Groups[1].Value;
                var matches = System.Text.RegularExpressions.Regex.Matches(ipv6sString, @"""([^""]+)""");
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    ips.Add(match.Groups[1].Value);
                }
            }
        }
        catch { }

        return ips;
    }
}