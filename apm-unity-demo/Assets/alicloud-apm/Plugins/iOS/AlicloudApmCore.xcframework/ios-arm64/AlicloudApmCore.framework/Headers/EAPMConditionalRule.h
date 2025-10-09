#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>
#import "EAPMFilter.h"

@protocol EAPMConditionalRule <NSObject>
@end

@interface EAPMConditionalRule : EAPMJSONModel

@property (nonatomic, strong) EAPMFilter *filter; // 嵌套的过滤条件
@property (nonatomic, copy) NSString *modifyTime; // 修改时间
@property (nonatomic, copy) NSString *name; // 规则名称
@property (nonatomic, assign) double sampleRate; // 采样率
@property (nonatomic, copy) NSString *operatorName; // 操作者（注意：operator 是保留字，建议改名为 operatorName）

@end
