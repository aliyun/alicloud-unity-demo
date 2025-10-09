#import "EAPMReportFilling.h"
#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMReportNetwork : EAPMJSONModel <NSCopying, EAPMReportFilling>
/**
 * 运营商
 */
@property (nonatomic, copy) NSString *carrier;

/**
 * 网络接入类型
 */
@property (nonatomic, copy) NSString *access;
@end

NS_ASSUME_NONNULL_END
