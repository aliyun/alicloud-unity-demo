using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Aliyun.HttpDns
{
    /// <summary>
    /// C SDK helper for Unity HttpDNS integration
    /// Provides access to Alibaba Cloud HttpDNS C SDK functionality
    /// </summary>
    public static class CHttpDnsHelper
    {
        // Platform-specific library names
        private const string LIBRARY_NAME_WINDOWS = "HttpDnsUnityBridge";
        private const string LIBRARY_NAME_LINUX = "libHttpDnsUnityBridge.so";
        private const string LIBRARY_NAME_MACOS = "libHttpDnsUnityBridge.dylib";
        
        // Maximum constants (must match C header)
        private const int HDNS_MAX_IP_LENGTH = 64;
        private const int HDNS_MAX_SESSION_ID_LENGTH = 16;
        private const int HDNS_MAX_IPS_PER_HOST = 10;
        
        // Debug callback delegate
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DebugCallback(string message);
        
        // Static reference to keep callback alive
        private static DebugCallback s_debugCallback;
        
        // Query types (must match C enum)
        public enum QueryType
        {
            Auto = 0,
            IPv4 = 1,
            IPv6 = 2,
            Both = 3
        }
        
        // Result structure (must match C struct)
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct UnityResult
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HDNS_MAX_IPS_PER_HOST * HDNS_MAX_IP_LENGTH)]
            public byte[] ips;
            
            public int ip_count;
            public int ttl;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HDNS_MAX_SESSION_ID_LENGTH)]
            public byte[] session_id;
            
            public int error_code;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] error_message;
            
            // Helper methods to extract strings
            public List<string> GetIpList()
            {
                var result = new List<string>();
                if (ips == null || ip_count <= 0) return result;
                
                for (int i = 0; i < ip_count && i < HDNS_MAX_IPS_PER_HOST; i++)
                {
                    int startIndex = i * HDNS_MAX_IP_LENGTH;
                    int endIndex = startIndex;
                    
                    // Find the null terminator
                    while (endIndex < startIndex + HDNS_MAX_IP_LENGTH && 
                           endIndex < ips.Length && 
                           ips[endIndex] != 0)
                    {
                        endIndex++;
                    }
                    
                    if (endIndex > startIndex)
                    {
                        string ip = System.Text.Encoding.UTF8.GetString(ips, startIndex, endIndex - startIndex);
                        if (!string.IsNullOrEmpty(ip))
                        {
                            result.Add(ip);
                        }
                    }
                }
                
                return result;
            }
            
            public string GetSessionId()
            {
                if (session_id == null) return "";
                
                int length = 0;
                while (length < session_id.Length && session_id[length] != 0)
                {
                    length++;
                }
                
                return length > 0 ? System.Text.Encoding.UTF8.GetString(session_id, 0, length) : "";
            }
            
            public string GetErrorMessage()
            {
                if (error_message == null) return "";
                
                int length = 0;
                while (length < error_message.Length && error_message[length] != 0)
                {
                    length++;
                }
                
                return length > 0 ? System.Text.Encoding.UTF8.GetString(error_message, 0, length) : "";
            }
        }
        
        // Get the appropriate library name based on platform
        private static string GetLibraryName()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return LIBRARY_NAME_WINDOWS;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return LIBRARY_NAME_MACOS;
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            return LIBRARY_NAME_LINUX;
#else
            return LIBRARY_NAME_WINDOWS; // Default fallback
#endif
        }
        
        // P/Invoke declarations with platform-specific library names
        
#if UNITY_IOS && !UNITY_EDITOR
        // iOS uses __Internal
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_init(string account_id, string secret_key);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_configure(int timeout_ms, int enable_https, int enable_cache);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_resolve_host_sync(string host, QueryType query_type, ref UnityResult result);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_resolve_host_async(string host, QueryType query_type, ref UnityResult result);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_add_pre_resolve_host(string host);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_set_timeout(int timeout_ms);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_set_https_enabled(int enable);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_clear_cache();
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_get_session_id(byte[] session_id);
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_cleanup();
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr hdns_unity_get_version();
        
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern void hdns_unity_set_debug_callback(DebugCallback callback);
        
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        // Windows uses .dll
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_init(string account_id, string secret_key);
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_configure(int timeout_ms, int enable_https, int enable_cache);
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_resolve_host_sync(string host, QueryType query_type, ref UnityResult result);
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_resolve_host_async(string host, QueryType query_type, ref UnityResult result);
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_add_pre_resolve_host(string host);
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_set_timeout(int timeout_ms);
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_set_https_enabled(int enable);
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_clear_cache();
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_get_session_id(byte[] session_id);
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_cleanup();
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr hdns_unity_get_version();
        
        [DllImport("HttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern void hdns_unity_set_debug_callback(DebugCallback callback);
        
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        // macOS uses .dylib
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_init(string account_id, string secret_key);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_configure(int timeout_ms, int enable_https, int enable_cache);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_resolve_host_sync(string host, QueryType query_type, ref UnityResult result);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_resolve_host_async(string host, QueryType query_type, ref UnityResult result);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_add_pre_resolve_host(string host);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_set_timeout(int timeout_ms);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_set_https_enabled(int enable);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_clear_cache();
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_get_session_id(byte[] session_id);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_cleanup();
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr hdns_unity_get_version();
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern void hdns_unity_set_debug_callback(DebugCallback callback);
        
#else
        // Linux and other platforms use .so
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_init(string account_id, string secret_key);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_configure(int timeout_ms, int enable_https, int enable_cache);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_resolve_host_sync(string host, QueryType query_type, ref UnityResult result);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_resolve_host_async(string host, QueryType query_type, ref UnityResult result);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_add_pre_resolve_host(string host);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_set_timeout(int timeout_ms);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_set_https_enabled(int enable);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_clear_cache();
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_get_session_id(byte[] session_id);
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern int hdns_unity_cleanup();
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr hdns_unity_get_version();
        
        [DllImport("libHttpDnsUnityBridge", CallingConvention = CallingConvention.Cdecl)]
        private static extern void hdns_unity_set_debug_callback(DebugCallback callback);
#endif
        
        // High-level C# wrapper methods
        
        /// <summary>
        /// Initialize the HttpDNS C SDK
        /// </summary>
        /// <param name="accountId">Account ID from Alibaba Cloud HttpDNS console</param>
        /// <param name="secretKey">Secret key for authentication (can be null)</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool Initialize(string accountId, string secretKey)
        {
            try
            {
                Debug.Log($"[CHttpDnsHelper] Attempting to initialize with Account ID: '{accountId}', Secret Key: {(string.IsNullOrEmpty(secretKey) ? "null" : "provided")}");
                
                int result = hdns_unity_init(accountId, secretKey);
                Debug.Log($"[CHttpDnsHelper] hdns_unity_init returned: {result}");
                
                if (result == 0)
                {
                    Debug.Log($"[CHttpDnsHelper] C SDK initialized successfully with Account ID: {accountId}");
                    return true;
                }
                else
                {
                    string errorMsg = "Unknown error";
                    switch (result)
                    {
                        case -1: errorMsg = "SDK initialization failed or invalid account ID"; break;
                        case -2: errorMsg = "Client creation failed"; break;
                        case -3: errorMsg = "Client start failed - continuing in fallback mode"; break;
                    }
                    Debug.LogWarning($"[CHttpDnsHelper] C SDK initialization issue: {result} ({errorMsg}). System will use fallback DNS.");
                    return true; // Allow fallback mode
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception during initialization: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Configure HttpDNS settings
        /// </summary>
        /// <param name="timeoutMs">Request timeout in milliseconds</param>
        /// <param name="enableHttps">Whether to use HTTPS</param>
        /// <param name="enableCache">Whether to use local cache</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool Configure(int timeoutMs, bool enableHttps, bool enableCache)
        {
            try
            {
                int result = hdns_unity_configure(timeoutMs, enableHttps ? 1 : 0, enableCache ? 1 : 0);
                Debug.Log($"[CHttpDnsHelper] Configuration result: {result}");
                return result == 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception during configuration: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Resolve a hostname to IP addresses synchronously
        /// </summary>
        /// <param name="host">Hostname to resolve</param>
        /// <param name="queryType">Query type (Auto, IPv4, IPv6, Both)</param>
        /// <returns>List of IP addresses or null if failed</returns>
        public static List<string> ResolveHostSync(string host, QueryType queryType = QueryType.Auto)
        {
            if (string.IsNullOrEmpty(host))
            {
                Debug.LogWarning("[CHttpDnsHelper] Host is null or empty");
                return null;
            }
            
            try
            {
                var result = new UnityResult();
                int returnCode = hdns_unity_resolve_host_sync(host, queryType, ref result);
                
                if (returnCode == 0 && result.ip_count > 0)
                {
                    var ips = result.GetIpList();
                    Debug.Log($"[CHttpDnsHelper] Resolved {host} to {ips.Count} IPs: {string.Join(", ", ips)}");
                    return ips;
                }
                else if (returnCode == -999)
                {
                    // C SDK not available, return null to fall back to system DNS
                    Debug.LogWarning($"[CHttpDnsHelper] C SDK not available for {host}, falling back to system DNS");
                    return null;
                }
                else
                {
                    Debug.LogWarning($"[CHttpDnsHelper] Failed to resolve {host}, error code: {returnCode}, message: {result.GetErrorMessage()}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception during resolution of {host}: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Resolve a hostname to IP addresses asynchronously (non-blocking)
        /// </summary>
        /// <param name="host">Hostname to resolve</param>
        /// <param name="queryType">Query type (Auto, IPv4, IPv6, Both)</param>
        /// <returns>List of IP addresses or null if failed</returns>
        public static List<string> ResolveHostAsync(string host, QueryType queryType = QueryType.Auto)
        {
            if (string.IsNullOrEmpty(host))
            {
                Debug.LogWarning("[CHttpDnsHelper] Host is null or empty");
                return null;
            }
            
            try
            {
                var result = new UnityResult();
                int returnCode = hdns_unity_resolve_host_async(host, queryType, ref result);
                
                if (returnCode == 0 && result.ip_count > 0)
                {
                    var ips = result.GetIpList();
                    Debug.Log($"[CHttpDnsHelper] Async resolved {host} to {ips.Count} IPs: {string.Join(", ", ips)}");
                    return ips;
                }
                else if (returnCode == -999)
                {
                    // C SDK not available, return null to fall back to system DNS
                    Debug.LogWarning($"[CHttpDnsHelper] C SDK not available for {host}, falling back to system DNS");
                    return null;
                }
                else
                {
                    Debug.LogWarning($"[CHttpDnsHelper] Failed to async resolve {host}, error code: {returnCode}, message: {result.GetErrorMessage()}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception during async resolution of {host}: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Add a hostname for pre-resolution
        /// </summary>
        /// <param name="host">Hostname to pre-resolve</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool AddPreResolveHost(string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                return false;
            }
            
            try
            {
                int result = hdns_unity_add_pre_resolve_host(host);
                Debug.Log($"[CHttpDnsHelper] Added pre-resolve host {host}, result: {result}");
                return result == 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception adding pre-resolve host {host}: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Set request timeout
        /// </summary>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetTimeout(int timeoutMs)
        {
            try
            {
                int result = hdns_unity_set_timeout(timeoutMs);
                Debug.Log($"[CHttpDnsHelper] Set timeout to {timeoutMs}ms, result: {result}");
                return result == 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception setting timeout: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Enable or disable HTTPS
        /// </summary>
        /// <param name="enable">True to enable HTTPS, false to disable</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetHttpsEnabled(bool enable)
        {
            try
            {
                int result = hdns_unity_set_https_enabled(enable ? 1 : 0);
                Debug.Log($"[CHttpDnsHelper] Set HTTPS enabled: {enable}, result: {result}");
                return result == 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception setting HTTPS: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Clear DNS cache
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool ClearCache()
        {
            try
            {
                int result = hdns_unity_clear_cache();
                Debug.Log($"[CHttpDnsHelper] Clear cache result: {result}");
                return result == 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception clearing cache: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Get SDK session ID for debugging
        /// </summary>
        /// <returns>Session ID string</returns>
        public static string GetSessionId()
        {
            try
            {
                byte[] buffer = new byte[HDNS_MAX_SESSION_ID_LENGTH];
                int result = hdns_unity_get_session_id(buffer);
                
                if (result == 0)
                {
                    int length = 0;
                    while (length < buffer.Length && buffer[length] != 0)
                    {
                        length++;
                    }
                    
                    return length > 0 ? System.Text.Encoding.UTF8.GetString(buffer, 0, length) : "";
                }
                
                return "";
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception getting session ID: {e.Message}");
                return "";
            }
        }
        
        /// <summary>
        /// Cleanup HttpDNS SDK resources
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool Cleanup()
        {
            try
            {
                int result = hdns_unity_cleanup();
                Debug.Log($"[CHttpDnsHelper] Cleanup result: {result}");
                return result == 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception during cleanup: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Get SDK version
        /// </summary>
        /// <returns>Version string</returns>
        public static string GetVersion()
        {
            try
            {
                IntPtr ptr = hdns_unity_get_version();
                return Marshal.PtrToStringAnsi(ptr) ?? "Unknown";
            }
            catch (Exception e)
            {
                Debug.LogError($"[CHttpDnsHelper] Exception getting version: {e.Message}");
                return "Unknown";
            }
        }
        
        /// <summary>
        /// Check if native C SDK is available on this platform
        /// </summary>
        /// <returns>True if available, false otherwise</returns>
        public static bool IsAvailable()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return true; // Assume available on desktop platforms
#else
            return false; // Not available on mobile platforms (use platform-specific SDKs)
#endif
        }
    }
}