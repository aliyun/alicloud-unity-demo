#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMCompressor <NSObject>

- (NSString *)getCompressorType;

- (NSData *)compressedDataWithData:(NSData *)data;

- (NSData *)decompressedDataWithData:(NSData *)data;

@end

NS_ASSUME_NONNULL_END
