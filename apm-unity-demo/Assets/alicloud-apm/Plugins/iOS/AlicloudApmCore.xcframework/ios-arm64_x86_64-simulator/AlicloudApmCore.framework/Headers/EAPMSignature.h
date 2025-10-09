#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMSignature : NSObject

+ (NSString *)calcSignWithAppKey:(NSString *)appKey appSecret:(NSString *)appSecret data:(NSData *)data;

+ (NSString *)calcSignWithString:(NSString *)toSign appSecret:(NSString *)appSecret;

@end

NS_ASSUME_NONNULL_END
