//
//  HttpDnsUnityBridge.mm
//  Unity iOS HttpDNS Bridge
//
//  Implementation following Alibaba Cloud HTTPDNS iOS SDK documentation
//  https://help.aliyun.com/document_detail/2868038.html
//

#import <Foundation/Foundation.h>
#import <AlicloudHTTPDNS/HttpDnsService.h>

// Constants for region settings - matching iOS Helper interface
#define ALICLOUD_HTTPDNS_DEFAULT_REGION_KEY 0
#define ALICLOUD_HTTPDNS_SINGAPORE_REGION_KEY 1
#define ALICLOUD_HTTPDNS_HK_REGION_KEY 2

// Global HttpDNS service instance
static HttpDnsService *g_httpdnsService = nil;

// Helper function to convert NSString to C string
char* _createCString(NSString* string) {
    if (string == nil) {
        return NULL;
    }
    
    const char* utf8String = [string UTF8String];
    if (utf8String == NULL) {
        return NULL;
    }
    
    char* cString = (char*)malloc(strlen(utf8String) + 1);
    strcpy(cString, utf8String);
    return cString;
}

// Helper function to convert HttpdnsResult to JSON string
NSString* _httpdnsResultToJson(HttpdnsResult* result) {
    if (!result) {
        return nil;
    }
    
    NSMutableDictionary* jsonDict = [NSMutableDictionary dictionary];
    
    // Add IPv4 addresses
    if ([result hasIpv4Address]) {
        NSArray<NSString*>* ips = [result ips];
        if (ips && ips.count > 0) {
            jsonDict[@"ips"] = ips;
        }
    }
    
    // Add IPv6 addresses
    if ([result hasIpv6Address]) {
        NSArray<NSString*>* ipv6s = [result ipv6s];
        if (ipv6s && ipv6s.count > 0) {
            jsonDict[@"ipv6s"] = ipv6s;
        }
    }
    
    // Convert to JSON string
    NSError* error;
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:jsonDict options:0 error:&error];
    if (error) {
        NSLog(@"[HttpDNS] JSON serialization error: %@", error.localizedDescription);
        return nil;
    }
    
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

extern "C" {
    
    // Initialize HttpDNS service
    void _httpdns_init(const char* accountId, const char* secretKey) {
        NSString* nsAccountId = accountId ? [NSString stringWithUTF8String:accountId] : @"";
        NSString* nsSecretKey = secretKey ? [NSString stringWithUTF8String:secretKey] : @"";
        
        // Convert accountId to NSInteger for API compatibility
        NSInteger accountIdInteger = [nsAccountId integerValue];
        
        // Create HttpDNS service instance
        g_httpdnsService = [[HttpDnsService alloc] initWithAccountID:accountIdInteger secretKey:nsSecretKey];
        
        if (g_httpdnsService) {
            // Configure default settings according to documentation
            [g_httpdnsService setLogEnabled:NO];
            [g_httpdnsService setHTTPSRequestEnabled:YES];
            [g_httpdnsService setRegion:@"cn-hangzhou"]; // Default region
            [g_httpdnsService setPersistentCacheIPEnabled:YES];
            [g_httpdnsService setReuseExpiredIPEnabled:YES];
            [g_httpdnsService setIPv6Enabled:YES];
            [g_httpdnsService setNetworkingTimeoutInterval:3];
            
            NSLog(@"[HttpDNS] Initialized with AccountID: %@", nsAccountId);
        } else {
            NSLog(@"[HttpDNS] Init failed: HttpDnsService creation failed");
        }
    }
    
    // Enable/disable debug logging
    void _httpdns_setLogEnabled(bool enabled) {
        if (g_httpdnsService) {
            [g_httpdnsService setLogEnabled:enabled];
            NSLog(@"[HttpDNS] Log enabled: %@", enabled ? @"YES" : @"NO");
        }
    }
    
    // Enable/disable HTTPS requests
    void _httpdns_setHTTPSRequestEnabled(bool enabled) {
        if (g_httpdnsService) {
            [g_httpdnsService setHTTPSRequestEnabled:enabled];
            NSLog(@"[HttpDNS] HTTPS request enabled: %@", enabled ? @"YES" : @"NO");
        }
    }
    
    // Enable/disable persistent cache
    void _httpdns_setPersistentCacheIPEnabled(bool enabled) {
        if (g_httpdnsService) {
            [g_httpdnsService setPersistentCacheIPEnabled:enabled];
            NSLog(@"[HttpDNS] Persistent cache enabled: %@", enabled ? @"YES" : @"NO");
        }
    }
    
    // Enable/disable expired IP reuse
    void _httpdns_setReuseExpiredIPEnabled(bool enabled) {
        if (g_httpdnsService) {
            [g_httpdnsService setReuseExpiredIPEnabled:enabled];
            NSLog(@"[HttpDNS] Reuse expired IP enabled: %@", enabled ? @"YES" : @"NO");
        }
    }
    
    // Enable/disable IPv6 support
    void _httpdns_setIPv6Enabled(bool enabled) {
        if (g_httpdnsService) {
            [g_httpdnsService setIPv6Enabled:enabled];
            NSLog(@"[HttpDNS] IPv6 enabled: %@", enabled ? @"YES" : @"NO");
        }
    }
    
    // Set network timeout
    void _httpdns_setNetworkingTimeoutInterval(int timeoutSeconds) {
        if (g_httpdnsService) {
            [g_httpdnsService setNetworkingTimeoutInterval:timeoutSeconds];
            NSLog(@"[HttpDNS] Timeout set to: %d seconds", timeoutSeconds);
        }
    }
    
    // Set service region
    void _httpdns_setRegion(int region) {
        if (g_httpdnsService) {
            NSString *regionString;
            switch (region) {
                case ALICLOUD_HTTPDNS_SINGAPORE_REGION_KEY:
                    regionString = @"ap-southeast-1";
                    break;
                case ALICLOUD_HTTPDNS_HK_REGION_KEY:
                    regionString = @"ap-east-1";
                    break;
                case ALICLOUD_HTTPDNS_DEFAULT_REGION_KEY:
                default:
                    regionString = @"cn-hangzhou";
                    break;
            }
            [g_httpdnsService setRegion:regionString];
            NSLog(@"[HttpDNS] Region set to: %@ (code: %d)", regionString, region);
        }
    }
    
    // Resolve hostname synchronously (non-blocking)
    const char* _httpdns_resolveHostSyncNonBlocking(const char* hostname) {
        if (!g_httpdnsService || !hostname) {
            return NULL;
        }
        
        NSString* nsHostname = [NSString stringWithUTF8String:hostname];
        HttpdnsResult* result = [g_httpdnsService resolveHostSyncNonBlocking:nsHostname byIpType:HttpdnsQueryIPTypeAuto];
        
        if (!result) {
            NSLog(@"[HttpDNS] No cached result for %@, background request started", nsHostname);
            return NULL;
        }
        
        NSString* jsonResult = _httpdnsResultToJson(result);
        if (jsonResult) {
            NSLog(@"[HttpDNS] Resolved %@ from cache", nsHostname);
            return _createCString(jsonResult);
        }
        
        return NULL;
    }
    
    // Resolve hostname synchronously (blocking)
    const char* _httpdns_resolveHostSync(const char* hostname) {
        if (!g_httpdnsService || !hostname) {
            return NULL;
        }
        
        NSString* nsHostname = [NSString stringWithUTF8String:hostname];
        HttpdnsResult* result = [g_httpdnsService resolveHostSync:nsHostname byIpType:HttpdnsQueryIPTypeAuto];
        
        if (!result) {
            NSLog(@"[HttpDNS] Failed to resolve %@", nsHostname);
            return NULL;
        }
        
        NSString* jsonResult = _httpdnsResultToJson(result);
        if (jsonResult) {
            NSLog(@"[HttpDNS] Resolved %@ synchronously", nsHostname);
            return _createCString(jsonResult);
        }
        
        return NULL;
    }
    
    // Pre-resolve hostnames
    void _httpdns_preResolveHosts(const char* hostname) {
        if (!g_httpdnsService || !hostname) {
            return;
        }
        
        NSString* nsHostname = [NSString stringWithUTF8String:hostname];
        [g_httpdnsService setPreResolveHosts:@[nsHostname]];
        NSLog(@"[HttpDNS] Pre-resolving: %@", nsHostname);
    }
    
    // Clear DNS cache
    void _httpdns_clearCache() {
        if (g_httpdnsService) {
            [g_httpdnsService cleanAllHostCache];
            NSLog(@"[HttpDNS] Cache cleared");
        }
    }
    
    // Check if service is initialized
    bool _httpdns_isInitialized() {
        return g_httpdnsService != nil;
    }
}