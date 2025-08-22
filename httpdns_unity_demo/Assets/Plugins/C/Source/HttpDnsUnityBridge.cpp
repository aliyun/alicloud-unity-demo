/*
 * HttpDns Unity Bridge Implementation
 * 
 * Simplified C API wrapper for Alibaba Cloud HttpDNS C SDK
 * Designed for Unity P/Invoke integration
 * 
 * Author: Unity HttpDNS Plugin
 * Version: 1.0.0
 */

// Only compile on desktop platforms (not on mobile platforms)
#if defined(UNITY_STANDALONE) || defined(UNITY_EDITOR) || (!defined(UNITY_ANDROID) && !defined(UNITY_IOS))

#include "HttpDnsUnityBridge.h"
#include <string.h>
#include <stdio.h>
#include <stdlib.h>

// Include Alibaba Cloud HttpDNS C SDK headers
#ifdef ENABLE_HTTPDNS_C_SDK
#include "hdns_api.h"
#endif

// SDK version
#define HTTPDNS_UNITY_BRIDGE_VERSION "1.0.0"

// Global variables
static bool g_sdk_initialized = false;
static void* g_client = nullptr;
static unity_debug_callback_t g_unity_debug_callback = nullptr;

// Debug logging macro
#define UNITY_LOG(fmt, ...) do { \
    if (g_unity_debug_callback) { \
        char buffer[512]; \
        snprintf(buffer, sizeof(buffer), "[C SDK Bridge] " fmt, ##__VA_ARGS__); \
        g_unity_debug_callback(buffer); \
    } \
} while(0)

// Set Unity debug callback
UNITY_EXPORT void hdns_unity_set_debug_callback(unity_debug_callback_t callback) {
    g_unity_debug_callback = callback;
    UNITY_LOG("Debug callback registered");
}

// Helper function to convert Unity query type to C SDK query type
#ifdef ENABLE_HTTPDNS_C_SDK
static hdns_query_type_t convert_unity_query_type(hdns_unity_query_type_t unity_type) {
    switch (unity_type) {
        case HDNS_UNITY_QUERY_AUTO: return HDNS_QUERY_AUTO;
        case HDNS_UNITY_QUERY_IPV4: return HDNS_QUERY_IPV4;
        case HDNS_UNITY_QUERY_IPV6: return HDNS_QUERY_IPV6;
        case HDNS_UNITY_QUERY_BOTH: return HDNS_QUERY_BOTH;
        default: return HDNS_QUERY_AUTO;
    }
}

// Helper function to extract IPs from C SDK result list
static int extract_ips_from_results(hdns_list_head_t* results, hdns_unity_result_t* unity_result) {
    if (!results || !unity_result) {
        return -1;
    }
    
    unity_result->ip_count = 0;
    unity_result->ttl = 0;
    unity_result->error_code = 0;
    memset(unity_result->error_message, 0, sizeof(unity_result->error_message));
    
    // Use the first available IP selection function
    char ip_buffer[HDNS_MAX_IP_LENGTH] = {0};
    if (hdns_select_first_ip(results, HDNS_QUERY_AUTO, ip_buffer) == 0) {
        strncpy(unity_result->ips[0], ip_buffer, HDNS_MAX_IP_LENGTH - 1);
        unity_result->ips[0][HDNS_MAX_IP_LENGTH - 1] = '\0';
        unity_result->ip_count = 1;
        
        // Try to get more IPs if available using random selection
        for (int i = 1; i < HDNS_MAX_IPS_PER_HOST; i++) {
            memset(ip_buffer, 0, sizeof(ip_buffer));
            if (hdns_select_ip_randomly(results, HDNS_QUERY_AUTO, ip_buffer) == 0) {
                // Check if this IP is different from the first one
                bool is_duplicate = false;
                for (int j = 0; j < unity_result->ip_count; j++) {
                    if (strcmp(unity_result->ips[j], ip_buffer) == 0) {
                        is_duplicate = true;
                        break;
                    }
                }
                if (!is_duplicate) {
                    strncpy(unity_result->ips[unity_result->ip_count], ip_buffer, HDNS_MAX_IP_LENGTH - 1);
                    unity_result->ips[unity_result->ip_count][HDNS_MAX_IP_LENGTH - 1] = '\0';
                    unity_result->ip_count++;
                }
            } else {
                break; // No more IPs available
            }
        }
    }
    
    return unity_result->ip_count > 0 ? 0 : -1;
}
#endif

// Implementation of Unity bridge functions

UNITY_EXPORT int hdns_unity_init(const char* account_id, const char* secret_key) {
    if (g_sdk_initialized) {
        UNITY_LOG("Already initialized, returning success");
        return 0; // Already initialized
    }
    
    if (!account_id) {
        UNITY_LOG("Error: Account ID is null");
        return -1;
    }
    
    UNITY_LOG("Initializing with Account ID: %s, Secret: %s", 
              account_id, secret_key ? "[PROVIDED]" : "[NULL]");
    
#ifdef ENABLE_HTTPDNS_C_SDK
    UNITY_LOG("Step 1: Calling hdns_sdk_init()");
    if (hdns_sdk_init() != 0) {
        UNITY_LOG("Error: hdns_sdk_init() failed");
        return -1;
    }
    UNITY_LOG("Step 1: hdns_sdk_init() success");
    
    UNITY_LOG("Step 2: Creating client with account_id=%s", account_id);
    g_client = hdns_client_create(account_id, secret_key);
    if (!g_client) {
        UNITY_LOG("Error: hdns_client_create() failed");
        hdns_sdk_cleanup();
        return -2;
    }
    UNITY_LOG("Step 2: hdns_client_create() success");
    
    UNITY_LOG("Step 3: Configuring client settings");
    hdns_client_set_timeout((hdns_client_t*)g_client, 3000);  // 3 seconds timeout
    hdns_client_set_using_cache((hdns_client_t*)g_client, true);  // Enable cache
    hdns_client_set_using_https((hdns_client_t*)g_client, true);  // Use HTTPS
    hdns_client_set_using_sign((hdns_client_t*)g_client, secret_key != nullptr);  // Enable signing if secret key provided
    hdns_client_set_retry_times((hdns_client_t*)g_client, 2);  // 2 retries
    hdns_client_set_region((hdns_client_t*)g_client, "cn");  // CN region
    hdns_client_set_schedule_center_region((hdns_client_t*)g_client, "cn");  // CN schedule center
    hdns_client_enable_update_cache_after_net_change((hdns_client_t*)g_client, true);  // Update cache after network change
    hdns_client_enable_expired_ip((hdns_client_t*)g_client, true);  // Allow expired IP
    hdns_client_enable_failover_localdns((hdns_client_t*)g_client, true);  // Auto fallback to local DNS
    UNITY_LOG("Step 3: Client configuration complete");
    
    UNITY_LOG("Step 4: Starting client with hdns_client_start()");
    hdns_status_t status = hdns_client_start((hdns_client_t*)g_client);
    UNITY_LOG("hdns_client_start() returned: code=%d, message='%s'", 
              status.code, status.error_msg);
    
    if (status.code != 0) {
        UNITY_LOG("Error: hdns_client_start() failed with code %d: %s", 
                  status.code, status.error_msg);
        
        // Don't cleanup immediately, keep client in a "partially initialized" state
        // This allows other functions to work in fallback mode
        g_sdk_initialized = true;  // Mark as initialized for fallback
        return 0;  // Return success to allow fallback mode
    }
    
    UNITY_LOG("Success: C SDK initialized successfully");
    g_sdk_initialized = true;
    return 0;
#else
    UNITY_LOG("Warning: C SDK not compiled, using fallback mode");
    g_sdk_initialized = true;
    return 0;
#endif
}

UNITY_EXPORT int hdns_unity_configure(int timeout_ms, int enable_https, int enable_cache) {
    if (!g_sdk_initialized) {
        return -1;
    }
    
#ifdef ENABLE_HTTPDNS_C_SDK
    if (g_client) {
        hdns_client_set_timeout((hdns_client_t*)g_client, timeout_ms);
        hdns_client_set_using_https((hdns_client_t*)g_client, enable_https != 0);
        hdns_client_set_using_cache((hdns_client_t*)g_client, enable_cache != 0);
    }
#endif
    
    return 0;
}

UNITY_EXPORT int hdns_unity_resolve_host_sync(const char* host, 
                                               hdns_unity_query_type_t query_type, 
                                               hdns_unity_result_t* result) {
    if (!g_sdk_initialized || !host || !result) {
        return -1;
    }
    
    // Initialize result
    memset(result, 0, sizeof(hdns_unity_result_t));
    strncpy(result->error_message, "Unknown error", sizeof(result->error_message) - 1);
    
#ifdef ENABLE_HTTPDNS_C_SDK
    if (g_client) {
        hdns_list_head_t* results = nullptr;
        hdns_query_type_t c_query_type = convert_unity_query_type(query_type);
        
        hdns_status_t status = hdns_get_result_for_host_sync_with_cache(
            (hdns_client_t*)g_client, host, c_query_type, nullptr, &results);
        
        if (status.code == 0 && results) {
            int extract_result = extract_ips_from_results(results, result);
            hdns_list_cleanup(results);
            
            if (extract_result == 0) {
                strncpy(result->error_message, "Success", sizeof(result->error_message) - 1);
                result->error_code = 0;
                return 0;
            }
        }
        
        if (results) {
            hdns_list_cleanup(results);
        }
        
        // Set error information
        result->error_code = status.code;
        if (status.error_msg[0] != '\0') {
            strncpy(result->error_message, status.error_msg, sizeof(result->error_message) - 1);
        }
        return -2;
    }
#endif
    
    // Fallback to system DNS
    result->error_code = -999;
    strncpy(result->error_message, "C SDK not available, falling back", sizeof(result->error_message) - 1);
    return -999;
}

UNITY_EXPORT int hdns_unity_resolve_host_async(const char* host, 
                                                hdns_unity_query_type_t query_type, 
                                                hdns_unity_result_t* result) {
    // For Unity integration, we'll use the sync version for simplicity
    // The async nature will be handled by Unity's threading
    return hdns_unity_resolve_host_sync(host, query_type, result);
}

UNITY_EXPORT int hdns_unity_add_pre_resolve_host(const char* host) {
    if (!g_sdk_initialized || !host) {
        return -1;
    }
    
#ifdef ENABLE_HTTPDNS_C_SDK
    if (g_client) {
        hdns_client_add_pre_resolve_host((hdns_client_t*)g_client, host);
    }
#endif
    
    return 0;
}

UNITY_EXPORT int hdns_unity_set_timeout(int timeout_ms) {
    if (!g_sdk_initialized) {
        return -1;
    }
    
#ifdef ENABLE_HTTPDNS_C_SDK
    if (g_client) {
        hdns_client_set_timeout((hdns_client_t*)g_client, timeout_ms);
    }
#endif
    
    return 0;
}

UNITY_EXPORT int hdns_unity_set_https_enabled(int enable) {
    if (!g_sdk_initialized) {
        return -1;
    }
    
#ifdef ENABLE_HTTPDNS_C_SDK
    if (g_client) {
        hdns_client_set_using_https((hdns_client_t*)g_client, enable != 0);
    }
#endif
    
    return 0;
}

UNITY_EXPORT int hdns_unity_clear_cache(void) {
    if (!g_sdk_initialized) {
        return -1;
    }
    
    // Note: The C SDK doesn't have a direct cache clear API
    // This would typically require recreating the client or waiting for TTL expiry
    return 0;
}

UNITY_EXPORT int hdns_unity_get_session_id(char* session_id) {
    if (!g_sdk_initialized || !session_id) {
        return -1;
    }
    
#ifdef ENABLE_HTTPDNS_C_SDK
    if (g_client) {
        return hdns_client_get_session_id((hdns_client_t*)g_client, session_id);
    }
#endif
    
    // Fallback
    strncpy(session_id, "fallback_session", HDNS_MAX_SESSION_ID_LENGTH - 1);
    session_id[HDNS_MAX_SESSION_ID_LENGTH - 1] = '\0';
    return 0;
}

UNITY_EXPORT int hdns_unity_cleanup(void) {
    if (!g_sdk_initialized) {
        return 0;
    }
    
#ifdef ENABLE_HTTPDNS_C_SDK
    if (g_client) {
        hdns_client_cleanup((hdns_client_t*)g_client);
        g_client = nullptr;
    }
    hdns_sdk_cleanup();
#endif
    
    g_sdk_initialized = false;
    return 0;
}

UNITY_EXPORT const char* hdns_unity_get_version(void) {
    return HTTPDNS_UNITY_BRIDGE_VERSION;
}

#else // Mobile platforms (Android/iOS) - provide stub implementations

#include "HttpDnsUnityBridge.h"
#include <string.h>

// Stub implementations for mobile platforms
UNITY_EXPORT void hdns_unity_set_debug_callback(unity_debug_callback_t callback) {}
UNITY_EXPORT int hdns_unity_init(const char* account_id, const char* secret_key) { return 0; }
UNITY_EXPORT int hdns_unity_configure(int timeout_ms, int enable_https, int enable_cache) { return 0; }
UNITY_EXPORT int hdns_unity_resolve_host_sync(const char* host, hdns_unity_query_type_t query_type, hdns_unity_result_t* result) { return -999; }
UNITY_EXPORT int hdns_unity_resolve_host_async(const char* host, hdns_unity_query_type_t query_type, hdns_unity_result_t* result) { return -999; }
UNITY_EXPORT int hdns_unity_add_pre_resolve_host(const char* host) { return 0; }
UNITY_EXPORT int hdns_unity_set_timeout(int timeout_ms) { return 0; }
UNITY_EXPORT int hdns_unity_set_https_enabled(int enable) { return 0; }
UNITY_EXPORT int hdns_unity_clear_cache(void) { return 0; }
UNITY_EXPORT int hdns_unity_get_session_id(char* session_id) { 
    if (session_id) {
        strncpy(session_id, "mobile_stub", 64);
    }
    return 0;
}
UNITY_EXPORT int hdns_unity_cleanup(void) { return 0; }
UNITY_EXPORT const char* hdns_unity_get_version(void) { return "1.0.0-mobile-stub"; }

#endif // Platform check