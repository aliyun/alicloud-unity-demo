#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMNetworkRequestBuilder : NSObject

- (instancetype)init NS_UNAVAILABLE;
+ (instancetype)new NS_UNAVAILABLE;

/**
 * Designated initializer. All parameters are mandatory and must not be nil.
 */
- (instancetype)initWithAppkey:(NSString *)appkey appSecret:(NSString *)appSecret NS_DESIGNATED_INITIALIZER;

/**
 * Creates a mutable request for posting to backend with a default timeout.
 */
- (NSMutableURLRequest *)mutableRequestWithDefaultHTTPHeaderFieldsAndTimeoutForURL:(NSURL *)url;

/**
 * Creates a mutable request for posting to backend with given timeout.
 */
- (NSMutableURLRequest *)mutableRequestWithDefaultHTTPHeadersForURL:(NSURL *)url timeout:(NSTimeInterval)timeout;

@end

NS_ASSUME_NONNULL_END
