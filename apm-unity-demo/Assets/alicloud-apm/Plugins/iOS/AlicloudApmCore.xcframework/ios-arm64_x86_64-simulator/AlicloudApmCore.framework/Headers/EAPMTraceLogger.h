#import <Foundation/Foundation.h>

@class EAPMTraceParameters;

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMTraceLogger <NSObject>

/**
 * 记录跟踪信息
 *
 * @param type 跟踪类型
 */
- (void)appendWithType:(NSString *)type;

/**
 * 记录跟踪信息
 *
 * @param type 跟踪类型
 * @param time 时间戳
 */
- (void)appendWithType:(NSString *)type time:(NSDate *)time;

/**
 * 记录跟踪信息
 *
 * @param type 跟踪类型
 * @param parameters 可选参数，包括 data, desc 和 params
 */
- (void)appendWithType:(NSString *)type parameters:(nullable EAPMTraceParameters *)parameters;

/**
 * 记录跟踪信息
 *
 * @param type 跟踪类型
 * @param time 时间戳
 * @param parameters 可选参数，包括 data, desc 和 params
 */
- (void)appendWithType:(NSString *)type time:(NSDate *)time parameters:(nullable EAPMTraceParameters *)parameters;

/**
 * trace 日志 A 文件路径
 */
- (nullable NSString *)traceAFilePath;

/**
 * trace 日志 B 文件路径
 */
- (nullable NSString *)traceBFilePath;

@end

NS_ASSUME_NONNULL_END
