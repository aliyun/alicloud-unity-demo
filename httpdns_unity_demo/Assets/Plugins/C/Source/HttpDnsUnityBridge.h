/*
 * HttpDns Unity Bridge Header
 * 
 * Simplified C API wrapper for Alibaba Cloud HttpDNS C SDK
 * Designed for Unity P/Invoke integration
 * 
 * Author: Unity HttpDNS Plugin
 * Version: 1.0.0
 */

#ifndef HTTPDNS_UNITY_BRIDGE_H
#define HTTPDNS_UNITY_BRIDGE_H

#ifdef __cplusplus
extern "C" {
#endif

// Platform-specific exports
#ifdef _WIN32
    #ifdef BUILDING_DLL
        #define UNITY_EXPORT __declspec(dllexport)
    #else
        #define UNITY_EXPORT __declspec(dllimport)
    #endif
#else
    #define UNITY_EXPORT __attribute__((visibility("default")))
#endif

// Unity debug callback type
typedef void (*unity_debug_callback_t)(const char* message);

// Set Unity debug callback
UNITY_EXPORT void hdns_unity_set_debug_callback(unity_debug_callback_t callback);

// Maximum string lengths
#define HDNS_MAX_HOST_LENGTH 256
#define HDNS_MAX_IP_LENGTH 64
#define HDNS_MAX_SESSION_ID_LENGTH 16
#define HDNS_MAX_IPS_PER_HOST 10

// Query types (matching Unity enum)
typedef enum {
    HDNS_UNITY_QUERY_AUTO = 0,
    HDNS_UNITY_QUERY_IPV4 = 1,
    HDNS_UNITY_QUERY_IPV6 = 2,
    HDNS_UNITY_QUERY_BOTH = 3
} hdns_unity_query_type_t;

// Result structure for passing back to Unity
typedef struct {
    char ips[HDNS_MAX_IPS_PER_HOST][HDNS_MAX_IP_LENGTH];
    int ip_count;
    int ttl;
    char session_id[HDNS_MAX_SESSION_ID_LENGTH];
    int error_code;
    char error_message[256];
} hdns_unity_result_t;

/*
 * Initialize HttpDNS SDK
 * @param account_id: HTTPDNS account ID
 * @param secret_key: Secret key for authentication (can be NULL)
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_init(const char* account_id, const char* secret_key);

/*
 * Configure HttpDNS settings
 * @param timeout_ms: Request timeout in milliseconds
 * @param enable_https: Whether to use HTTPS (0=false, 1=true)
 * @param enable_cache: Whether to use local cache (0=false, 1=true)
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_configure(int timeout_ms, int enable_https, int enable_cache);

/*
 * Resolve hostname to IP addresses (synchronous with cache)
 * @param host: Hostname to resolve
 * @param query_type: Query type (auto, ipv4, ipv6, both)
 * @param result: Output result structure
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_resolve_host_sync(const char* host, 
                                               hdns_unity_query_type_t query_type, 
                                               hdns_unity_result_t* result);

/*
 * Resolve hostname to IP addresses (non-blocking)
 * @param host: Hostname to resolve
 * @param query_type: Query type (auto, ipv4, ipv6, both)
 * @param result: Output result structure
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_resolve_host_async(const char* host, 
                                                hdns_unity_query_type_t query_type, 
                                                hdns_unity_result_t* result);

/*
 * Add hostname for pre-resolution
 * @param host: Hostname to pre-resolve
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_add_pre_resolve_host(const char* host);

/*
 * Set request timeout
 * @param timeout_ms: Timeout in milliseconds
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_set_timeout(int timeout_ms);

/*
 * Enable or disable HTTPS
 * @param enable: 1 to enable, 0 to disable
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_set_https_enabled(int enable);

/*
 * Clear DNS cache
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_clear_cache(void);

/*
 * Get SDK session ID for debugging
 * @param session_id: Buffer to store session ID (must be at least 16 bytes)
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_get_session_id(char* session_id);

/*
 * Cleanup HttpDNS SDK resources
 * @return: 0 on success, non-zero on error
 */
UNITY_EXPORT int hdns_unity_cleanup(void);

/*
 * Get SDK version string
 * @return: Version string pointer (static memory)
 */
UNITY_EXPORT const char* hdns_unity_get_version(void);

#ifdef __cplusplus
}
#endif

#endif // HTTPDNS_UNITY_BRIDGE_H