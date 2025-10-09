#import "EAPMSDKComponent.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

FOUNDATION_EXPORT NSString *const kEAPMOptionsKeyUserId;
FOUNDATION_EXPORT NSString *const kEAPMOptionsKeyUserNick;
FOUNDATION_EXPORT NSString *const kEAPMOptionsKeyChannel;

/**
 * EAPM配置选项
 */
NS_SWIFT_NAME(EAPMOptions)
@interface EAPMOptions : NSObject <NSCopying>

/**
 * EAPM appKey
 */
@property (nonatomic, copy) NSString *appKey;

/**
 * EAPM appSecret
 */
@property (nonatomic, copy) NSString *appSecret;

/**
 * 标识application and application extensions
 */
@property (nonatomic, copy, nullable) NSString *appGroupId;

/**
 * 渠道
 */
@property (nonatomic, copy) NSString *channel;

/**
 * 用户昵称
 */
@property (nonatomic, copy) NSString *userNick;

/**
 * 用户ID
 */
@property (nonatomic, copy) NSString *userId;

/**
 * EAPM appRsaSecret
 */
@property (nonatomic, copy) NSString *appRsaSecret;

/**
 * SDK组件列表
 */
@property (nonatomic, copy) NSArray<Class<EAPMSDKComponent>> *sdkComponents;

- (instancetype)init;

- (instancetype)initWithAppKey:(NSString *)appKey appSecret:(NSString *)appSecret;

- (instancetype)initWithAppKey:(NSString *)appKey
                     appSecret:(NSString *)appSecret
                 sdkComponents:(NSArray<Class<EAPMSDKComponent>> *)sdkComponents;

@end

NS_ASSUME_NONNULL_END
