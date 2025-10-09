#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

@class EAPMSettingData;

NS_ASSUME_NONNULL_BEGIN

@interface EAPMSettingResult : EAPMJSONModel

@property (nonatomic) BOOL success;

@property (nonatomic, copy) NSString *errorCode;

@property (nonatomic, copy) NSString *errorMsg;

@property (nonatomic, copy) EAPMSettingData *data;

@end

NS_ASSUME_NONNULL_END
