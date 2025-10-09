#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMReportUser : EAPMJSONModel <NSCopying>
/**
 * 用户ID
 */
@property (nonatomic, copy) NSString *id;

/**
 * 用户昵称
 */
@property (nonatomic, copy) NSString *nick;

@end

NS_ASSUME_NONNULL_END
