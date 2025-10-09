#import <Foundation/Foundation.h>

/**
 * This is a convenience class to ease constructing NSURLs.
 */
@interface EAPMNetworkURLBuilder : NSObject

/**
 * Convenience method that returns a EAPMNetworkURLBuilder instance with the input base URL appended to
 * it.
 */
+ (instancetype)URLWithBase:(NSString *)base;
/**
 * Appends the component to the URL being built by EAPMNetworkURLBuilder instance
 */
- (void)appendComponent:(NSString *)component;
/**
 * Escapes and appends the component to the URL being built by EAPMNetworkURLBuilder instance
 */
- (void)escapeAndAppendComponent:(NSString *)component;
/**
 * Adds a query and value to the URL being built
 */
- (void)appendValue:(id)value forQueryParam:(NSString *)param;
/**
 * Returns the built URL
 */
- (NSURL *)URL;

/**
 * URL query string
 */
- (NSString *)query;

@end
