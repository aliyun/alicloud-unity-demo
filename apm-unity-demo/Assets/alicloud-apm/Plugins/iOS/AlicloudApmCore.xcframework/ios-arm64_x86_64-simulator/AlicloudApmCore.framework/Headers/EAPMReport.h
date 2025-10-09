#import <AlicloudApmCore/AlicloudApmJson.h>
#import <Foundation/Foundation.h>

@class EAPMReportSdk;
@class EAPMReportApp;
@class EAPMReportDevice;
@class EAPMReportNetwork;
@class EAPMReportUser;
@class EAPMOptions;
@class EAPMCustomAttributes;

NS_ASSUME_NONNULL_BEGIN

@interface EAPMReport : EAPMJSONModel

/**
 * 协议版本
 */
@property (nonatomic, copy) NSString *protocolVersion;

/**
 * 平台
 */
@property (nonatomic, copy) NSString *platform;

/**
 * 事件类型
 */
@property (nonatomic, copy) NSString *eventId;

/**
 * 设备ID
 */
@property (nonatomic, copy) NSString *utdid;

/**
 * 事件关联会话ID
 */
@property (nonatomic, copy) NSString *sessionId;

/**
 * 事件唯一ID
 */
@property (nonatomic, copy) NSString *uuid;

/**
 * sdk信息
 */
@property (nonatomic, strong) EAPMReportSdk *sdk;

/**
 * 应用信息
 */
@property (nonatomic, strong) EAPMReportApp *app;

/**
 * 设备信息
 */
@property (nonatomic, strong) EAPMReportDevice *device;

/**
 * 网络信息
 */
@property (nonatomic, strong) EAPMReportNetwork *network;

/**
 * 用户信息
 */
@property (nonatomic, strong) EAPMReportUser *user;

/**
 * 事件发生时时间戳，UNIX毫秒数
 */
@property (nonatomic, assign) int64_t eventTime;

/**
 * 上报时间戳，UNIX毫秒数
 */
@property (nonatomic, assign) int64_t clientTime;

/**
 * 事件发生时URI
 */
@property (nonatomic, copy) NSString *uri;

/**
 * 命中采样率的值
 */
@property (nonatomic, assign) double sampleRate;

/**
 * 事件数据，按照事件类型区分
 */
@property (nonatomic, strong) NSObject *payload;

/**
 * 自定义数据
 */
@property (nonatomic, strong) NSArray<NSDictionary<NSString *, id> *> *customAttributes;

- (void)fillWithOptions:(EAPMOptions *)options andPayload:(NSObject *)payload;

@end

NS_ASSUME_NONNULL_END
