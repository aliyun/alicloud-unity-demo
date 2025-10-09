#import "EAPMReportFilling.h"
#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMReportApp : EAPMJSONModel <NSCopying, EAPMReportFilling>
/**
 * 包名
 */
@property (nonatomic, copy) NSString *name;

/**
 * 应用版
 */
@property (nonatomic, copy) NSString *version;
/**
 * 应用构建版本
 */
@property (nonatomic, copy) NSString *build;

/**
 * 应用分发渠道
 */
@property (nonatomic, copy) NSString *channel;

@property (nonatomic, copy) NSString *process;

@end

NS_ASSUME_NONNULL_END
