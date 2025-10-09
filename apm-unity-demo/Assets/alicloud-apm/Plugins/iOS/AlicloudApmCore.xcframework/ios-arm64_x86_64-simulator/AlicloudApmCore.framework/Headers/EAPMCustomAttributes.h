#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMCustomAttributes : NSObject

/**
 * 获取自定义属性数组，从 Share 模块的 customKeyValue 文件中读取
 * @return 自定义属性的数组，包含key-value字典
 */
+ (NSArray<NSDictionary<NSString *, id> *> *)getCustomAttributes;

@end

NS_ASSUME_NONNULL_END
