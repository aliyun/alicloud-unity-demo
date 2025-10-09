#import "EAPMReportFilling.h"
#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMReportDevice : EAPMJSONModel <NSCopying, EAPMReportFilling>
/**
 * 品牌
 */
@property (nonatomic, copy) NSString *brand;

/**
 * 型号
 */
@property (nonatomic, copy) NSString *model;
/**
 * 操作系统
 */
@property (nonatomic, copy) NSString *os;

/**
 * 操作系统版本，形如“16.4.0”
 */
@property (nonatomic, copy) NSString *version;

/**
 * 操作系统构建版本，形如“22D68”
 */
@property (nonatomic, copy) NSString *osBuildVersion;

/**
 * 语言
 */
@property (nonatomic, copy) NSString *language;

/**
 * 分辨率
 */
@property (nonatomic, copy) NSString *resolution;

/**
 * cpu架构
 */
@property (nonatomic, copy) NSString *cpu;

/**
 * 是否越狱
 */
@property (nonatomic, assign, getter=isJailbroken) BOOL jailbroken;

@end

NS_ASSUME_NONNULL_END
