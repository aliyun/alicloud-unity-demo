#import "EAPMCompressor.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, EAPMCompressorName) {
    EAPMCompressorNameGzip = 1,
    // EAPMCompressorNameBrotli = 2,
} NS_SWIFT_NAME(CompressorName);

@interface EAPMCompressorFactory : NSObject

+ (id<EAPMCompressor>)getCompressorWithName:(EAPMCompressorName)compressorName;

+ (id<EAPMCompressor>)getDefaultCompressor;

@end

NS_ASSUME_NONNULL_END
