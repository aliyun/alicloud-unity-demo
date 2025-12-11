//
//  PushWrapper.h
//  Unity-iPhone
//
//  Created by 心定 on 2021/3/25.
//

#ifndef PushWrapper_h
#define PushWrapper_h

#import "PushHelper.h"
#import <UserNotifications/UserNotifications.h>

// 通知点击类型枚举
typedef NS_ENUM(NSInteger, NotificationClickType) {
    NotificationClickTypeOpened = 0,        // 通知被点击打开
    NotificationClickTypeRemoved = 1,       // 通知被移除
    NotificationClickTypeRemote = 2         // 远程/后台通知
};

@interface PushWrapper : NSObject <UNUserNotificationCenterDelegate>

// 原有回调
@property MessageReceivedCallback onMessageReceivedCallback;
@property CloudPushCallback onPushActionCallback;

// 新增APNS和通知相关回调
@property APNSRegisterSuccessCallback onAPNSRegisterSuccessCallback;
@property APNSRegisterFailedCallback onAPNSRegisterFailedCallback;
@property NotificationReceivedCallback onNotificationReceivedCallback;
@property NotificationOpenedCallback onNotificationOpenedCallback;
@property NotificationRemovedCallback onNotificationRemovedCallback;
@property NotificationReceivedCallback onRemoteNotificationReceived;

// 前台通知处理模式
@property (nonatomic, strong) NSString *foregroundNotificationMode;

// 通知中心实例
@property (nonatomic, strong) UNUserNotificationCenter *notificationCenter;

+ (instancetype)sharedInstance;

- (void)initCloudPush:(NSString *)appKey appSecret:(NSString *)appSecret actionId:(int)actionId;
- (void)bindAccount:(NSString *)account actionId:(int)actionId;
- (void)unBindAccount:(int)actionId;
- (void)bindTag:(int)target
       withTag:(NSString *)tag
      withAlias:(NSString *)alias
       actionId:(int)actionId;
- (void)unBindTag:(int)target
         withTag:(NSString *)tag
        withAlias:(NSString *)alias
         actionId:(int)actionId;
- (void)listTags:(int)target
        actionId:(int)actionId;
- (void)addAlias:(NSString *)alias
        actionId:(int)actionId;
- (void)removeAlias:(NSString *)alias
           actionId:(int)actionId;
- (void)listAliases:(int)actionId;
- (NSString *)deviceId;
- (void)setLogLevel:(NSString *)logLevel;
- (void)setForegroundNotificationMode:(NSString *)mode;

// 通知点击缓存处理方法
- (void)handleNotificationOpened:(NSString *)title 
                            body:(NSString *)body 
                        subtitle:(NSString *)subtitle 
                        userInfo:(NSString *)userInfo;
- (void)handleNotificationRemoved:(NSString *)userInfo;
- (void)handleRemoteNotificationReceived:(NSString *)title 
                                    body:(NSString *)body 
                                subtitle:(NSString *)subtitle 
                                userInfo:(NSString *)userInfo;

@end

#endif /* PushWrapper_h */
