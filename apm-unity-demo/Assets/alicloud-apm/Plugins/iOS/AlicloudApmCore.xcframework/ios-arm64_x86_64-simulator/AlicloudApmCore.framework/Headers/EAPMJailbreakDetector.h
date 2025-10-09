#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMJailbreakDetector : NSObject

/// 判断当前设备是否越狱
+ (BOOL)isDeviceJailbroken;

@end

NS_ASSUME_NONNULL_END
