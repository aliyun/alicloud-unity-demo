#import <Foundation/Foundation.h>

#import <AlicloudApmCore/AlicloudApmPromise.h>
#import <AlicloudApmCore/AlicloudApmReport.h>

@class EAPMSettingData;
@class EAPMSettingRule;

NS_ASSUME_NONNULL_BEGIN

typedef void (^EAPMSettingAvailableCallback)(void);

@interface EAPMSetting : NSObject

- (instancetype)init NS_UNAVAILABLE;
+ (instancetype)new NS_UNAVAILABLE;

- (instancetype)initWithAppKey:(nonnull NSString *)appKey options:(EAPMOptions *)options NS_DESIGNATED_INITIALIZER;

- (void)loadSettingData;

- (void)updateSettingData:(EAPMSettingData *)settingData;

- (void)fetchSetting;

- (NSDictionary *)getEventConfigs;

- (double)getEventSampleRateIfHit:(EAPMReport *)eventData;

- (EAPMSettingRule *)getEventRule:(NSString *)eventId;

- (NSDictionary *)getReportPolicy;

- (EAPMPromise *)waitForSettingAvailable;

- (void)registerCallback:(EAPMSettingAvailableCallback)callback;

- (void)unregisterCallback:(EAPMSettingAvailableCallback)callback;

@property (atomic, readonly) BOOL settingAvailable;

@property (atomic, readonly) long expirationTime;

@end

NS_ASSUME_NONNULL_END
