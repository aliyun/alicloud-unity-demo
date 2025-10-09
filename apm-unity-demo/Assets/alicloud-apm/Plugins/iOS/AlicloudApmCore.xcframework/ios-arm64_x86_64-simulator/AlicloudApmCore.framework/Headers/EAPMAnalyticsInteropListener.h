@protocol EAPMAnalyticsInteropListener <NSObject>

/// Triggers when an Analytics event happens for the registered origin with
/// EAPMAnalyticsInterop`s `registerAnalyticsListener(_:withOrigin:)`.
- (void)messageTriggered:(NSString *)name parameters:(NSDictionary *)parameters;

@end
