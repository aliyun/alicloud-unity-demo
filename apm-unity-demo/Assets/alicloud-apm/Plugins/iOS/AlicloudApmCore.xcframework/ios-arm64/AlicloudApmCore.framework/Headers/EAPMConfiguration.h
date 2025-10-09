#import <Foundation/Foundation.h>

#import "EAPMLoggerLevel.h"

NS_ASSUME_NONNULL_BEGIN

/**
 * 全局属性配置
 */
NS_SWIFT_NAME(EAPMConfiguration)
@interface EAPMConfiguration : NSObject

/**
 * 全局属性共享实例
 */
@property (class, nonatomic, readonly) EAPMConfiguration *sharedInstance NS_SWIFT_NAME(shared);

/**
 * 设置EAPM内部日志级别
 *
 * @param loggerLevel 日志级别，默认级别是EAPMLoggerLevelNotice.
 */
- (void)setLoggerLevel:(EAPMLoggerLevel)loggerLevel;

/**
 * 返回EAPM内部日志级别
 */
- (EAPMLoggerLevel)loggerLevel;

@end

NS_ASSUME_NONNULL_END
