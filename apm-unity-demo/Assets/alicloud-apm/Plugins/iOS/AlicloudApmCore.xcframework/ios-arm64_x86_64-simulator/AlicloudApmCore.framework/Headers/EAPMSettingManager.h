#import <Foundation/Foundation.h>

@class EAPMSetting;
@class EAPMOptions;

NS_ASSUME_NONNULL_BEGIN

@interface EAPMSettingManager : NSObject

/**
 * Designated Initializer.
 */
- (instancetype)initWithSetting:(EAPMSetting *)setting options:(EAPMOptions *)options NS_DESIGNATED_INITIALIZER;
- (instancetype)init NS_UNAVAILABLE;
+ (instancetype)new NS_UNAVAILABLE;

- (void)fetchSetting;

@property (nonatomic, strong, readonly) NSString *settingFilePath;

@end

NS_ASSUME_NONNULL_END
