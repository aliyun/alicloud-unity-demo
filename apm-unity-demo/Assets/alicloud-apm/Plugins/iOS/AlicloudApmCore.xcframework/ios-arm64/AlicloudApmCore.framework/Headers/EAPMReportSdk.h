#import "EAPMReportFilling.h"
#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMReportSdk : EAPMJSONModel <NSCopying, EAPMReportFilling>
/**
 * sdk名称
 */
@property (nonatomic, copy) NSString *name;

/**
 * sdk版本
 */
@property (nonatomic, copy) NSString *version;

@end

NS_ASSUME_NONNULL_END
