#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMShareBinaryImages <NSObject>

/**
 * 获取二进制文件地址
 *
 */
- (NSString *)binaryImagesFilePath;

@end

NS_ASSUME_NONNULL_END
