#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>
#import "EAPMConditionalRule.h"

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMSettingRule <NSObject>
@end

@interface EAPMSettingRule : EAPMJSONModel

@property (nonatomic) BOOL enable;

@property (nonatomic, copy) NSString *eventId;

@property (nonatomic) double sampleRate;

@property (nonatomic, copy) NSDictionary<NSString *, id> *options;

@property (nonatomic, strong) NSArray<EAPMConditionalRule *> *conditional;

@end

NS_ASSUME_NONNULL_END
