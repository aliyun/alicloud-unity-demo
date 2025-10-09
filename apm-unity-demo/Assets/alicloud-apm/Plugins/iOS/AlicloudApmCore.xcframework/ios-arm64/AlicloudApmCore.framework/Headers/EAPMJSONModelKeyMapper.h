#import <Foundation/Foundation.h>

@interface EAPMJSONModelKeyMapper : NSObject

// 映射字典自身的key为json的字段名，value为model的属性名
- (id)initWithDictionary:(NSDictionary *)dict;

- (NSString *)modelKeyMappedFromJsonKey:(NSString *)jsonKey;
- (NSString *)jsonKeyMappedFromModelKey:(NSString *)modelKey;

@end
