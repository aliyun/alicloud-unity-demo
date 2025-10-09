#import <Foundation/Foundation.h>

@class EAPMSettingData;

NS_ASSUME_NONNULL_BEGIN

@interface EAPMSettingFileManager : NSObject

/**
 * 全局属性共享实例
 */
@property (class, nonatomic, readonly) EAPMSettingFileManager *sharedInstance;

- (NSString *)settingFilePath;

- (void)saveSettingData:(EAPMSettingData *)settingData;

- (void)deleteSettingData;

- (EAPMSettingData *)readSettingData;

@end

NS_ASSUME_NONNULL_END
