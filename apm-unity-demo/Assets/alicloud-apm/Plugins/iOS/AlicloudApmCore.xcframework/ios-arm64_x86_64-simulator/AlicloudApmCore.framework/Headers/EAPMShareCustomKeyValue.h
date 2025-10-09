#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMShareCustomKeyValue <NSObject>

/**
 * 增量 Key-Value 文件路径
 */
- (NSString *)incrementalFilePath;

/**
 * 压缩 Key-Value 文件路径
 */
- (NSString *)compactedFilePath;

@end

NS_ASSUME_NONNULL_END
