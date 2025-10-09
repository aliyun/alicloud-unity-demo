#import <Foundation/Foundation.h>

@protocol EAPMAnalyticsInteropListener;

NS_ASSUME_NONNULL_BEGIN

/// Block typedef callback parameter to `getUserProperties(with:)`.
typedef void (^EAPMAInteropUserPropertiesCallback)(NSDictionary<NSString *, id> *userProperties)
NS_SWIFT_UNAVAILABLE("Use Swift's closure syntax instead.");

/// Connector for bridging communication between EAPM SDKs and Analytics APIs.
@protocol EAPMAnalyticsInterop

/// Sets user property when trigger event is logged. This API is only available in the SDK.
- (void)setConditionalUserProperty:(NSDictionary<NSString *, id> *)conditionalUserProperty;

/// Clears user property if set.
- (void)clearConditionalUserProperty:(NSString *)userPropertyName
                           forOrigin:(NSString *)origin
                      clearEventName:(NSString *)clearEventName
                clearEventParameters:(NSDictionary<NSString *, NSString *> *)clearEventParameters;

/// Returns currently set user properties.
- (NSArray<NSDictionary<NSString *, NSString *> *> *)conditionalUserProperties:(NSString *)origin
                                                            propertyNamePrefix:(NSString *)propertyNamePrefix;

/// Returns the maximum number of user properties.
- (NSInteger)maxUserProperties:(NSString *)origin;

/// Returns the user properties to a callback function.
- (void)getUserPropertiesWithCallback:(void (^)(NSDictionary<NSString *, id> *userProperties))callback;

/// Logs events.
- (void)logEventWithOrigin:(NSString *)origin
                      name:(NSString *)name
                parameters:(nullable NSDictionary<NSString *, id> *)parameters;

/// Sets user property.
- (void)setUserPropertyWithOrigin:(NSString *)origin name:(NSString *)name value:(id)value;

/// Registers an Analytics listener for the given origin.
- (void)registerAnalyticsListener:(id<EAPMAnalyticsInteropListener>)listener withOrigin:(NSString *)origin;

/// Unregisters an Analytics listener for the given origin.
- (void)unregisterAnalyticsListenerWithOrigin:(NSString *)origin;

@end

NS_ASSUME_NONNULL_END
