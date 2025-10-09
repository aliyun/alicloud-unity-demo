#import "EAPMSettingRule.h"
#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMSettingData : EAPMJSONModel

@property (nonatomic, copy) NSString *appKey;

@property (nonatomic, copy) NSArray<NSString *> *fullSampleUsers;

@property (nonatomic) double sampleRate;

@property (nonatomic, copy) NSString *sampleMethod;

@property (nonatomic, copy) NSArray<EAPMSettingRule> *rules;

+ (EAPMSettingData *)defaultSettingDataWithAppKey:(NSString *)appKey;

@end

NS_ASSUME_NONNULL_END
