using System.Collections.Generic;
using UnityEngine;

namespace Aliyun.HttpDns
{
    /// <summary>
    /// HTTP DNS Helper class for Unity cross-platform support
    /// Provides DNS resolution functionality using Alibaba Cloud HTTP DNS service
    /// </summary>
    public static class HttpDnsHelper
    {
        private static IHttpDnsService httpDnsService;
        private static bool isInitialized = false;
        private static bool debugEnabled = false;

        /// <summary>
        /// Initialize HTTP DNS service with account credentials (stage 1: save parameters)
        /// </summary>
        /// <param name="accountId">Account ID from Alibaba Cloud HTTP DNS console</param>
        /// <param name="secretKey">Secret key for authentication (optional)</param>
        /// <param name="aesSecretKey">AES secret key for encryption (optional)</param>
        public static void init(string accountId, string secretKey = null, string aesSecretKey = null)
        {
            if (isInitialized)
            {
                LogDebug("HttpDnsHelper already initialized");
                return;
            }

            LogDebug($"Initializing HttpDnsHelper with accountId: {accountId}");

#if UNITY_ANDROID && !UNITY_EDITOR
            httpDnsService = new AndroidHttpDnsService();
#elif UNITY_IOS && !UNITY_EDITOR
            httpDnsService = new iOSHttpDnsService();
#elif (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_EDITOR)
            httpDnsService = new CHttpDnsService();
#else
            httpDnsService = new DefaultHttpDnsService();
#endif

            httpDnsService.Initialize(accountId, secretKey, aesSecretKey);
            isInitialized = true;
            LogDebug("HttpDnsHelper initialization completed");
        }

        /// <summary>
        /// Enable or disable debug logging
        /// </summary>
        /// <param name="enable">True to enable debug logs</param>
        public static void debug(bool enable)
        {
            debugEnabled = enable;
            LogDebug($"Debug logging {(enable ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Enable or disable HTTPS requests for HTTP DNS resolution
        /// </summary>
        /// <param name="enable">True to enable HTTPS</param>
        public static void setHttpsRequestEnabled(bool enable)
        {
            if (!EnsureInitialized()) return;
            
            httpDnsService.SetHttpsEnabled(enable);
            LogDebug($"HTTPS requests {(enable ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Get IP addresses for a host using HTTP DNS (async)
        /// </summary>
        /// <param name="host">The hostname to resolve</param>
        /// <returns>List of IP addresses or null if resolution fails</returns>
        public static List<string> getIpsByHostAsync(string host)
        {
            if (!EnsureInitialized()) return null;
            
            LogDebug($"Resolving host: {host}");
            var result = httpDnsService.GetIpsByHost(host);
            LogDebug($"Resolved {host} to {result?.Count ?? 0} IPs");
            return result;
        }

        /// <summary>
        /// Get IP addresses for a host using HTTP DNS (sync)
        /// </summary>
        /// <param name="host">The hostname to resolve</param>
        /// <returns>List of IP addresses or null if resolution fails</returns>
        public static List<string> getIpsByHost(string host)
        {
            return getIpsByHostAsync(host);
        }

        /// <summary>
        /// Preload DNS resolution for a host
        /// </summary>
        /// <param name="host">The hostname to preload</param>
        public static void setPreResolveHosts(string host)
        {
            if (!EnsureInitialized()) return;
            
            LogDebug($"Pre-resolving host: {host}");
            httpDnsService.PreResolveHost(host);
        }

        /// <summary>
        /// Set timeout for HTTP DNS requests
        /// </summary>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        public static void setTimeout(int timeoutMs)
        {
            if (!EnsureInitialized()) return;
            
            httpDnsService.SetTimeout(timeoutMs);
            LogDebug($"Timeout set to {timeoutMs}ms");
        }

        /// <summary>
        /// Clear DNS cache
        /// </summary>
        public static void clearCache()
        {
            if (!EnsureInitialized()) return;
            
            httpDnsService.ClearCache();
            LogDebug("DNS cache cleared");
        }
        
        /// <summary>
        /// Clean all host cache
        /// </summary>
        public static void cleanAllHostCache()
        {
            clearCache(); // Delegate to existing method
        }

        /// <summary>
        /// Build the HTTP DNS service (stage 2: actually create service instances)
        /// </summary>
        /// <returns>True if build succeeded, false otherwise</returns>
        public static bool build()
        {
            if (!EnsureInitialized()) return false;
            
            bool result = httpDnsService.Build();
            LogDebug($"Service build {(result ? "succeeded" : "failed")}");
            return result;
        }

        /// <summary>
        /// Enable or disable persistent IP cache
        /// </summary>
        /// <param name="enable">True to enable persistent cache</param>
        /// <param name="discardAfterSeconds">Discard expired records after specified seconds (-1 for default)</param>
        public static void setPersistentCacheIPEnabled(bool enable, int discardAfterSeconds = -1)
        {
            if (!EnsureInitialized()) return;
            
            httpDnsService.SetPersistentCacheIPEnabled(enable, discardAfterSeconds);
            LogDebug($"Persistent cache IP enabled: {enable}, discard after: {discardAfterSeconds}s");
        }

        /// <summary>
        /// Enable or disable reuse of expired IPs
        /// </summary>
        /// <param name="enable">True to enable expired IP reuse</param>
        public static void setReuseExpiredIPEnabled(bool enable)
        {
            if (!EnsureInitialized()) return;
            
            httpDnsService.SetReuseExpiredIPEnabled(enable);
            LogDebug($"Reuse expired IP enabled: {enable}");
        }

        /// <summary>
        /// Enable or disable automatic pre-resolution after network changes
        /// </summary>
        /// <param name="enable">True to enable auto pre-resolution</param>
        public static void setPreResolveAfterNetworkChanged(bool enable)
        {
            if (!EnsureInitialized()) return;
            
            httpDnsService.SetPreResolveAfterNetworkChanged(enable);
            LogDebug($"Pre-resolve after network changed: {enable}");
        }

        /// <summary>
        /// Get session ID for debugging and logging
        /// </summary>
        /// <returns>Session ID string or null if not available</returns>
        public static string getSessionId()
        {
            if (!EnsureInitialized()) return null;
            
            string sessionId = httpDnsService.GetSessionId();
            LogDebug($"Session ID: {sessionId}");
            return sessionId;
        }

        /// <summary>
        /// Enhanced pre-resolve hosts supporting multiple hosts and IP types
        /// </summary>
        /// <param name="hosts">List of hostnames to pre-resolve</param>
        /// <param name="ipType">IP type: auto, ipv4, ipv6, both</param>
        public static void setPreResolveHosts(List<string> hosts, string ipType = "auto")
        {
            if (!EnsureInitialized()) return;
            
            httpDnsService.PreResolveHosts(hosts, ipType);
            LogDebug($"Pre-resolving {hosts?.Count ?? 0} hosts with type: {ipType}");
        }

        /// <summary>
        /// Enhanced DNS resolution with additional parameters
        /// </summary>
        /// <param name="host">Hostname to resolve</param>
        /// <param name="ipType">IP type: auto, ipv4, ipv6, both</param>
        /// <param name="sdnsParams">SDNS parameters (optional)</param>
        /// <param name="cacheKey">Custom cache key (optional)</param>
        /// <returns>Enhanced resolve result with IPv4/IPv6 lists</returns>
        public static ResolveResult resolveHostSyncNonBlocking(string host, string ipType = "auto", 
            Dictionary<string, string> sdnsParams = null, string cacheKey = null)
        {
            if (!EnsureInitialized()) return null;
            
            LogDebug($"Enhanced resolving host: {host}, type: {ipType}");
            var result = httpDnsService.ResolveHostSyncNonBlocking(host, ipType, sdnsParams, cacheKey);
            LogDebug($"Enhanced resolved {host} to {result?.IPv4?.Count ?? 0} IPv4 + {result?.IPv6?.Count ?? 0} IPv6 IPs");
            return result;
        }

        private static bool EnsureInitialized()
        {
            if (!isInitialized)
            {
                Debug.LogError("HttpDnsHelper not initialized. Call init() first.");
                return false;
            }
            return true;
        }

        private static void LogDebug(string message)
        {
            if (debugEnabled)
            {
                Debug.Log($"[HttpDnsHelper] {message}");
            }
        }
    }

    /// <summary>
    /// Interface for HTTP DNS service implementations
    /// </summary>
    public interface IHttpDnsService
    {
        void Initialize(string accountId, string secretKey);
        void Initialize(string accountId, string secretKey, string aesSecretKey);
        bool Build();
        void SetHttpsEnabled(bool enable);
        List<string> GetIpsByHost(string host);
        void PreResolveHost(string host);
        void PreResolveHosts(List<string> hosts, string ipType);
        void SetTimeout(int timeoutMs);
        void ClearCache();
        void SetPersistentCacheIPEnabled(bool enable, int discardAfterSeconds);
        void SetReuseExpiredIPEnabled(bool enable);
        void SetPreResolveAfterNetworkChanged(bool enable);
        string GetSessionId();
        ResolveResult ResolveHostSyncNonBlocking(string host, string ipType, Dictionary<string, string> sdnsParams, string cacheKey);
    }

    /// <summary>
    /// Enhanced DNS resolution result
    /// </summary>
    public class ResolveResult
    {
        public List<string> IPv4 { get; set; } = new List<string>();
        public List<string> IPv6 { get; set; } = new List<string>();
        public int TTL { get; set; }
        public string SessionId { get; set; }
        public string Extra { get; set; }
    }

    /// <summary>
    /// iOS implementation using Alibaba Cloud HTTPDNS iOS SDK
    /// </summary>
    public class iOSHttpDnsService : IHttpDnsService
    {
        private string pendingAccountId;
        private string pendingSecretKey;
        private string pendingAesSecretKey;
        private bool isBuilt = false;
        
        public void Initialize(string accountId, string secretKey)
        {
            Initialize(accountId, secretKey, null);
        }
        
        public void Initialize(string accountId, string secretKey, string aesSecretKey)
        {
            pendingAccountId = accountId;
            pendingSecretKey = secretKey;
            pendingAesSecretKey = aesSecretKey;
            Debug.Log("iOS HttpDNS service parameters saved, call build() to initialize");
        }
        
        public bool Build()
        {
            try
            {
                iOSHttpDnsHelper.Init(pendingAccountId, pendingSecretKey);
                isBuilt = true;
                Debug.Log("iOS HttpDNS service built successfully");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"iOS HttpDNS service build failed: {e.Message}");
                return false;
            }
        }

        public void SetHttpsEnabled(bool enable)
        {
            if (isBuilt)
            {
                iOSHttpDnsHelper.SetHTTPSRequestEnabled(enable);
            }
        }

        public List<string> GetIpsByHost(string host)
        {
            if (!isBuilt) return null;
            
            // Use sync non-blocking
            var result = iOSHttpDnsHelper.GetIpsByHostAsync(host);
            if (result != null && result.Count > 0)
            {
                return result;
            }
            
            // If no cached result, fallback to system DNS
            return FallbackToSystemDns(host);
        }
        
        public ResolveResult ResolveHostSyncNonBlocking(string host, string ipType, Dictionary<string, string> sdnsParams, string cacheKey)
        {
            if (!isBuilt) return null;
            
            // Use sync non-blocking with enhanced result
            var result = iOSHttpDnsHelper.GetIpsByHostAsync(host);
            var enhancedResult = new ResolveResult();
            
            if (result != null && result.Count > 0)
            {
                // Parse IPs based on format (IPv4 or IPv6)
                foreach (var ip in result)
                {
                    if (ip.Contains(":"))
                    {
                        enhancedResult.IPv6.Add(ip);
                    }
                    else
                    {
                        enhancedResult.IPv4.Add(ip);
                    }
                }
            }
            
            return enhancedResult;
        }

        public void PreResolveHost(string host)
        {
            if (isBuilt)
            {
                iOSHttpDnsHelper.SetPreResolveHosts(host);
            }
        }
        
        public void PreResolveHosts(List<string> hosts, string ipType)
        {
            if (!isBuilt || hosts == null) return;
            
            // iOS supports individual pre-resolve, iterate through list
            foreach (var host in hosts)
            {
                iOSHttpDnsHelper.SetPreResolveHosts(host);
            }
        }

        public void SetTimeout(int timeoutMs)
        {
            if (isBuilt)
            {
                iOSHttpDnsHelper.SetTimeout(timeoutMs);
            }
        }
        
        public void SetPersistentCacheIPEnabled(bool enable, int discardAfterSeconds)
        {
            if (isBuilt)
            {
                if (discardAfterSeconds > 0)
                {
                    iOSHttpDnsHelper.SetPersistentCacheIPEnabled(enable, discardAfterSeconds);
                }
                else
                {
                    iOSHttpDnsHelper.SetPersistentCacheIPEnabled(enable);
                }
            }
        }
        
        public void SetReuseExpiredIPEnabled(bool enable)
        {
            if (isBuilt)
            {
                iOSHttpDnsHelper.SetReuseExpiredIPEnabled(enable);
            }
        }
        
        public void SetPreResolveAfterNetworkChanged(bool enable)
        {
            if (isBuilt)
            {
                iOSHttpDnsHelper.SetPreResolveAfterNetworkChanged(enable);
            }
        }
        
        public string GetSessionId()
        {
            if (!isBuilt) return null;
            return iOSHttpDnsHelper.GetSessionId();
        }

        public void ClearCache()
        {
            if (isBuilt)
            {
                iOSHttpDnsHelper.ClearCache();
            }
        }
        
        private List<string> FallbackToSystemDns(string host)
        {
            try
            {
                var addresses = System.Net.Dns.GetHostAddresses(host);
                var ips = new List<string>();
                foreach (var addr in addresses)
                {
                    ips.Add(addr.ToString());
                }
                return ips.Count > 0 ? ips : null;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"System DNS fallback failed for {host}: {e.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// C SDK implementation for desktop platforms using HttpDNS C SDK
    /// </summary>
    public class CHttpDnsService : IHttpDnsService
    {
        private string pendingAccountId;
        private string pendingSecretKey;
        private string pendingAesSecretKey;
        private bool isBuilt = false;
        
        public void Initialize(string accountId, string secretKey)
        {
            Initialize(accountId, secretKey, null);
        }
        
        public void Initialize(string accountId, string secretKey, string aesSecretKey)
        {
            pendingAccountId = accountId;
            pendingSecretKey = secretKey;
            pendingAesSecretKey = aesSecretKey;
            Debug.Log("C HttpDNS service parameters saved, call build() to initialize");
        }
        
        public bool Build()
        {
            try
            {
                if (CHttpDnsHelper.Initialize(pendingAccountId, pendingSecretKey))
                {
                    isBuilt = true;
                    Debug.Log("C HttpDNS service built successfully");
                    return true;
                }
                else
                {
                    Debug.LogWarning("C HttpDNS service build failed, will use fallback");
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"C HttpDNS service build exception: {e.Message}");
                return false;
            }
        }

        public void SetHttpsEnabled(bool enable)
        {
            if (isBuilt)
            {
                CHttpDnsHelper.SetHttpsEnabled(enable);
            }
        }

        public List<string> GetIpsByHost(string host)
        {
            if (!isBuilt) return FallbackToSystemDns(host);
            
            // Use async (non-blocking)
            var result = CHttpDnsHelper.ResolveHostAsync(host, CHttpDnsHelper.QueryType.Auto);
            if (result != null && result.Count > 0)
            {
                return result;
            }
            
            // Fallback to system DNS
            return FallbackToSystemDns(host);
        }
        
        public ResolveResult ResolveHostSyncNonBlocking(string host, string ipType, Dictionary<string, string> sdnsParams, string cacheKey)
        {
            if (!isBuilt) return null;
            
            var queryType = ParseQueryType(ipType);
            var result = CHttpDnsHelper.ResolveHostAsync(host, queryType);
            
            var enhancedResult = new ResolveResult();
            if (result != null && result.Count > 0)
            {
                // Parse IPs based on format (IPv4 or IPv6)
                foreach (var ip in result)
                {
                    if (ip.Contains(":"))
                    {
                        enhancedResult.IPv6.Add(ip);
                    }
                    else
                    {
                        enhancedResult.IPv4.Add(ip);
                    }
                }
            }
            
            return enhancedResult;
        }

        public void PreResolveHost(string host)
        {
            if (isBuilt)
            {
                CHttpDnsHelper.AddPreResolveHost(host);
            }
        }
        
        public void PreResolveHosts(List<string> hosts, string ipType)
        {
            if (!isBuilt || hosts == null) return;
            
            // C SDK supports individual pre-resolve, iterate through list
            foreach (var host in hosts)
            {
                CHttpDnsHelper.AddPreResolveHost(host);
            }
        }

        public void SetTimeout(int timeoutMs)
        {
            if (isBuilt)
            {
                CHttpDnsHelper.SetTimeout(timeoutMs);
            }
        }
        
        public void SetPersistentCacheIPEnabled(bool enable, int discardAfterSeconds)
        {
            // C SDK doesn't have separate persistent cache setting, uses general cache
            if (isBuilt)
            {
                Debug.Log($"C HttpDNS persistent cache (using general cache): {enable}");
            }
        }
        
        public void SetReuseExpiredIPEnabled(bool enable)
        {
            // C SDK has this functionality, but not exposed through current bridge
            if (isBuilt)
            {
                Debug.Log($"C HttpDNS reuse expired IP: {enable} (not directly supported)");
            }
        }
        
        public void SetPreResolveAfterNetworkChanged(bool enable)
        {
            // C SDK has this functionality, but not exposed through current bridge
            if (isBuilt)
            {
                Debug.Log($"C HttpDNS pre-resolve after network change: {enable} (not directly supported)");
            }
        }
        
        public string GetSessionId()
        {
            if (!isBuilt) return null;
            return CHttpDnsHelper.GetSessionId();
        }

        public void ClearCache()
        {
            if (isBuilt)
            {
                CHttpDnsHelper.ClearCache();
            }
        }
        
        private CHttpDnsHelper.QueryType ParseQueryType(string ipType)
        {
            switch (ipType?.ToLower())
            {
                case "ipv4":
                case "v4":
                    return CHttpDnsHelper.QueryType.IPv4;
                case "ipv6":
                case "v6":
                    return CHttpDnsHelper.QueryType.IPv6;
                case "both":
                case "64":
                    return CHttpDnsHelper.QueryType.Both;
                default:
                    return CHttpDnsHelper.QueryType.Auto;
            }
        }
        
        private List<string> FallbackToSystemDns(string host)
        {
            try
            {
                var addresses = System.Net.Dns.GetHostAddresses(host);
                var ips = new List<string>();
                foreach (var addr in addresses)
                {
                    ips.Add(addr.ToString());
                }
                return ips.Count > 0 ? ips : null;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"System DNS fallback failed for {host}: {e.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// Default implementation for non-Android/iOS platforms (fallback to system DNS)
    /// </summary>
    public class DefaultHttpDnsService : IHttpDnsService
    {
        public void Initialize(string accountId, string secretKey)
        {
            Debug.Log("Using default DNS service (system DNS)");
        }
        
        public void Initialize(string accountId, string secretKey, string aesSecretKey)
        {
            Debug.Log("Using default DNS service (system DNS)");
        }
        
        public bool Build()
        {
            return true; // Always successful for system DNS
        }

        public void SetHttpsEnabled(bool enable) { }

        public List<string> GetIpsByHost(string host)
        {
            try
            {
                var addresses = System.Net.Dns.GetHostAddresses(host);
                var ips = new List<string>();
                foreach (var addr in addresses)
                {
                    ips.Add(addr.ToString());
                }
                return ips.Count > 0 ? ips : null;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"System DNS resolution failed for {host}: {e.Message}");
                return null;
            }
        }
        
        public ResolveResult ResolveHostSyncNonBlocking(string host, string ipType, Dictionary<string, string> sdnsParams, string cacheKey)
        {
            var ips = GetIpsByHost(host);
            if (ips == null) return null;
            
            var result = new ResolveResult();
            foreach (var ip in ips)
            {
                if (ip.Contains(":"))
                {
                    result.IPv6.Add(ip);
                }
                else
                {
                    result.IPv4.Add(ip);
                }
            }
            return result;
        }

        public void PreResolveHost(string host) { }
        public void PreResolveHosts(List<string> hosts, string ipType) { }
        public void SetTimeout(int timeoutMs) { }
        public void ClearCache() { }
        public void SetPersistentCacheIPEnabled(bool enable, int discardAfterSeconds) { }
        public void SetReuseExpiredIPEnabled(bool enable) { }
        public void SetPreResolveAfterNetworkChanged(bool enable) { }
        public string GetSessionId() { return null; }
    }
}