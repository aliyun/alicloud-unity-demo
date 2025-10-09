#import <Foundation/Foundation.h>

#import "AlicloudApmCore/AlicloudApmCore.h"
#import "EAPMCrashAnalysisReport.h"
#import "EAPMExceptionModel.h"

#if __has_include(<AlicloudApmCrashAnalysis/CrashAnalysis.h>)
#warning "AlicloudApmCrashAnalysis and CrashAnalysis are not compatible \
in the same app because including multiple crash reporters can \
cause problems when registering exception handlers."
#endif

NS_ASSUME_NONNULL_BEGIN

NS_SWIFT_NAME(CrashAnalysis)
@interface EAPMCrashAnalysis : NSObject <EAPMSDKComponent>

/** :nodoc: */
- (instancetype)init NS_UNAVAILABLE;

/**
 * 访问单例 CrashAnalysis 实例。
 *
 * @return 单例 CrashAnalysis 实例。
 */
+ (instancetype)crashAnalysis NS_SWIFT_NAME(crashAnalysis());

/**
 * 记录崩溃相关日志
 *
 * @param msg 要记录的消息
 */
- (void)log:(NSString *)msg;

/**
 * 记录崩溃相关日志
 *
 * @param format 字符串格式
 * @param ... 格式替换参数
 */
- (void)logWithFormat:(NSString *)format, ... NS_FORMAT_FUNCTION(1, 2);

/**
 * 记录崩溃相关日志
 *
 * @param format 字符串格式
 * @param args 格式替换参数
 */
- (void)logWithFormat:(NSString *)format arguments:(va_list)args __attribute__((__swift_name__("log(format:arguments:)")));

/**
 * 设置自定义键值对
 *
 * @param value 值
 * @param key   唯一键
 */
- (void)setCustomValue:(nullable id)value forKey:(NSString *)key;

/**
 * 批量设置自定义键值对
 *
 * @param keysAndValues 键值对
 */
- (void)setCustomKeysAndValues:(NSDictionary *)keysAndValues;

/**
 * 记录异常对象
 *
 * @param error 异常对象
 */
- (void)recordError:(NSError *)error NS_SWIFT_NAME(record(error:));

/**
 * 记录异常对象
 *
 * @param error 异常对象
 * @param userInfo 附加键值对
 */
- (void)recordError:(NSError *)error
           userInfo:(nullable NSDictionary<NSString *, id> *)userInfo NS_SWIFT_NAME(record(error:userInfo:));

/**
 * 记录自定义异常模型
 *
 * @param exceptionModel 异常模型
 */
- (void)recordExceptionModel:(EAPMExceptionModel *)exceptionModel;

@end

NS_ASSUME_NONNULL_END
