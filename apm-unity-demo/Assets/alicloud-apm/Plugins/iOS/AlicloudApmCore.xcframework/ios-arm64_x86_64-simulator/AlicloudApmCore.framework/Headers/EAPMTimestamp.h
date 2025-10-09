#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 * EAPMTimestamp独立于任何时区或日历的时间点，以UTC时间中的秒和纳秒分数表示，分辨率为纳秒。
 *
 */
NS_SWIFT_NAME(Timestamp)
@interface EAPMTimestamp : NSObject <NSCopying>

/** :nodoc: */
- (instancetype)init NS_UNAVAILABLE;

/**
 * 创建时间戳
 *
 * @param seconds 自纪元以来的秒数
 * @param nanoseconds 剩余纳秒数
 */
- (instancetype)initWithSeconds:(int64_t)seconds nanoseconds:(int32_t)nanoseconds NS_DESIGNATED_INITIALIZER;

/**
 * 创建时间戳
 *
 * @param seconds 自纪元以来的秒数
 * @param nanoseconds 剩余纳秒数
 */
+ (instancetype)timestampWithSeconds:(int64_t)seconds nanoseconds:(int32_t)nanoseconds;

/**
 * 根据日期创建时间戳
 *
 * @param date 日期
 */
+ (instancetype)timestampWithDate:(NSDate *)date;

/**
 * 创建当前时间的时间戳
 */
+ (instancetype)timestamp;

/**
 * 返回时间戳对应的日期
 */
- (NSDate *)dateValue;

/**
 * 时间戳比较
 * @param other 另一个时间戳
 * @return 如果 `other` 在时间上晚于自身，则返回 `orderedAscending`，
 *     如果 `other` 在时间上早于自身，则返回 `orderedDescending`，
 *     否则返回 `orderedSame`。
 */
- (NSComparisonResult)compare:(EAPMTimestamp *)other;

/**
 * 表示自1970-01-01T00:00:00Z 起的 UTC 时间的秒数
 */
@property (nonatomic, assign, readonly) int64_t seconds;

/**
 * 非负秒的纳秒小数部分
 */
@property (nonatomic, assign, readonly) int32_t nanoseconds;

@end

NS_ASSUME_NONNULL_END
