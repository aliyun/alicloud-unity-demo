#import <Foundation/Foundation.h>

@class EAPMApm;

NS_ASSUME_NONNULL_BEGIN

@interface EAPMLauncherEventManager : NSObject

// Please use the designated initializer.
- (instancetype)init NS_UNAVAILABLE;

/**
 *
 * @param apm APM instance
 * @return EAPMLauncherEventManager instance
 */
- (nullable instancetype)initWithApm:(EAPMApm *)apm NS_DESIGNATED_INITIALIZER;

// SDK启动打点
- (void)submitLauncherEvent;

@end

NS_ASSUME_NONNULL_END
