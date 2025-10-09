#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

@protocol EAPMFilter <NSObject>
@end

@interface EAPMFilter : EAPMJSONModel

/**
 * 操作符类型（如 "and", "or", "=", ">", "regex" 等）
 */
@property (nonatomic, copy) NSString *op;

/**
 * 递归子条件（当 op 为 "and"/"or" 时使用）
 */
@property (nonatomic, strong) NSArray<EAPMFilter *> *filters;

/**
 * 数据键路径（当 op 为 "=", ">", "regex" 等时使用）
 */
@property (nonatomic, copy) NSString *key;

/**
 * 比较值列表（支持多值匹配，如 "in" 操作符）
 */
@property (nonatomic, strong) NSArray<NSString *> *values;

@end
