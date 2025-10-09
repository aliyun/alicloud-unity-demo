#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 * EAPM内部环境标识
 */
typedef NS_ENUM(NSInteger, EAPMEnvironmentTag) {
    EAPMEnvironmentPre,       // 预发布环境
    EAPMEnvironmentProduction // 生产环境
} NS_SWIFT_NAME(EAPMEnvironmentTag);

NS_SWIFT_NAME(EAPMEnvironment)
@interface EAPMEnvironment : NSObject
/**
 * 全局属性共享实例
 */
@property (class, nonatomic, readonly) EAPMEnvironment *sharedInstance NS_SWIFT_NAME(shared);

/**
 * EAPM内部环境标识
 *
 * 默认级别是EAPMEnvironmentProduction
 */
@property (nonatomic, assign) EAPMEnvironmentTag envTag;

@end

NS_ASSUME_NONNULL_END
