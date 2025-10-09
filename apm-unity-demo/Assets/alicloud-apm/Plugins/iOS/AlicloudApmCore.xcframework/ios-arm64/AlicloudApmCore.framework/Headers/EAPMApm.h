#import <Foundation/Foundation.h>

@class EAPMOptions;

NS_ASSUME_NONNULL_BEGIN

typedef void (^EAPMApmVoidBoolCallback)(BOOL success) NS_SWIFT_UNAVAILABLE("Use Swift's closure syntax instead.");

/**
 * AlicloudApm SDK启动入口
 */
NS_SWIFT_NAME(EAPMApm)
@interface EAPMApm : NSObject

/**
 * 指定选项启动APM
 *
 * @param options 配置选项
 */
+ (void)startWithOptions:(EAPMOptions *)options NS_SWIFT_NAME(start(options:));

/**
 * 获取APM实例
 * 未启动将返回nil
 */
+ (nullable EAPMApm *)defaultApm NS_SWIFT_NAME(apm());

/**
 * 更新userNick
 * @param userNick 用户昵称
 */
- (void)setUserNick:(NSString *)userNick NS_SWIFT_NAME(setUserNick(userNick:));

/**
 * @brief 更新userId
 * @param userId 用户标识
 */
- (void)setUserId:(NSString *)userId NS_SWIFT_NAME(setUserId(userId:));

/**
 * 设置自定义维度
 *
 * @param value 值
 * @param key   维度
 */
- (void)setCustomValue:(nullable id)value forKey:(NSString *)key NS_SWIFT_NAME(setCustomValue(_:forKey:));

/**
 * 批量设置自定义维度
 *
 * @param keysAndValues 键值对
 */
- (void)setCustomKeysAndValues:(NSDictionary *)keysAndValues NS_SWIFT_NAME(setCustomKeysAndValues(keysAndValues:));

/**
 * 删除APM实例
 */
- (void)deleteApm:(void (^)(BOOL success))completion;

/**
 * 请勿直接调用，请用`[EAPMApm startWithOptions:options]`启动，并通过[EAPMApm defaultApm]获取实例
 */
- (instancetype)init NS_UNAVAILABLE;

/**
 * APM名称
 */
@property (nonatomic, copy, readonly) NSString *name;

/**
 * APM配置选项
 */
@property (nonatomic, copy, readonly) EAPMOptions *options;

@end

NS_ASSUME_NONNULL_END
