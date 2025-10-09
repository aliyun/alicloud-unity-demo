#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

NS_SWIFT_NAME(CrashAnalysisReport)
@interface EAPMCrashAnalysisReport : NSObject

/** :nodoc: */
- (instancetype)init NS_UNAVAILABLE;

/**
 * 崩溃报告ID
 */
@property (nonatomic, readonly) NSString *reportID;

/**
 * 报告产生时间
 */
@property (nonatomic, readonly) NSDate *dateCreated;

/**
 * 报告是否含有崩溃
 */
@property (nonatomic, readonly) BOOL hasCrash;

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
- (void)logWithFormat:(NSString *)format
            arguments:(va_list)args __attribute__((__swift_name__("log(format:arguments:)"))); // Avoid `NS_SWIFT_NAME` (#9331).

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
 * 设置关联的用户ID
 *
 * @param userId 用户标识符
 */
- (void)setUserId:(nullable NSString *)userId;

@end

NS_ASSUME_NONNULL_END
