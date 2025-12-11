//
//  PushWrapper.m
//  Unity-iPhone
//
//  Created by 心定 on 2021/3/25.
//

#import <Foundation/Foundation.h>
#import <CloudPushSDK/CloudPushSDK.h>
#import <UserNotifications/UserNotifications.h>
#import "PushWrapper.h"

// 通知点击数据模型
@interface NotificationClickData : NSObject
@property (nonatomic, strong) NSString *title;
@property (nonatomic, strong) NSString *body;
@property (nonatomic, strong) NSString *subtitle;
@property (nonatomic, strong) NSString *userInfo;
@property (nonatomic, assign) NotificationClickType clickType;

- (instancetype)initWithTitle:(NSString *)title 
                         body:(NSString *)body 
                     subtitle:(NSString *)subtitle 
                     userInfo:(NSString *)userInfo 
                    clickType:(NotificationClickType)clickType;
@end

@implementation NotificationClickData

- (instancetype)initWithTitle:(NSString *)title 
                         body:(NSString *)body 
                     subtitle:(NSString *)subtitle 
                     userInfo:(NSString *)userInfo 
                    clickType:(NotificationClickType)clickType {
    self = [super init];
    if (self) {
        _title = title;
        _body = body;
        _subtitle = subtitle;
        _userInfo = userInfo;
        _clickType = clickType;
    }
    return self;
}

@end

@interface PushWrapper () {
    NSString *_foregroundNotificationMode;
}
@property (nonatomic, strong) NSMutableArray<NotificationClickData *> *pendingClickDataList;
@end

@implementation PushWrapper

+ (instancetype)sharedInstance
{
    static PushWrapper *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[PushWrapper alloc] init];
        // 设置默认前台通知处理模式
        sharedInstance->_foregroundNotificationMode = @"show_and_callback";
        // 初始化待处理通知列表
        sharedInstance.pendingClickDataList = [NSMutableArray array];
    });

    return sharedInstance;
}

- (void)invokeCallback:(CloudPushCallbackResult *)res actionId:(int)actionId {
    if(self.onPushActionCallback != NULL) {
        if(res.success) {
            if(res.data != NULL){
                self.onPushActionCallback(actionId, res.success, [[NSString stringWithFormat:@"%@", res.data] UTF8String]);
            } else {
                self.onPushActionCallback(actionId, res.success, nil);
            }
        } else {
            self.onPushActionCallback(actionId, res.success, [[NSString stringWithFormat:@"%@", res.error] UTF8String]);
        }
    }
}

- (NSString *)deviceId {
    return [CloudPushSDK getDeviceId];
}

- (void)setLogLevel:(NSString *)logLevel {
    MPLogLevel level = MPLogLevelInfo; // 默认Info级别
    
    if ([logLevel isEqualToString:@"off"]) {
        level = MPLogLevelNone;
    } else if ([logLevel isEqualToString:@"error"]) {
        level = MPLogLevelError;
    } else if ([logLevel isEqualToString:@"warn"]) {
        level = MPLogLevelWarn;
    } else if ([logLevel isEqualToString:@"info"]) {
        level = MPLogLevelInfo;
    } else if ([logLevel isEqualToString:@"debug"]) {
        level = MPLogLevelDebug;
    }
    
    [CloudPushSDK setLogLevel:level];
    NSLog(@"Set log level to: %@", logLevel);
}

// 手动实现 getter 和 setter
- (NSString *)foregroundNotificationMode {
    return _foregroundNotificationMode;
}

- (void)setForegroundNotificationMode:(NSString *)mode {
    _foregroundNotificationMode = mode;
    NSLog(@"Set foreground notification mode to: %@", mode);
}

#pragma mark - 通知点击缓存处理

/**
 * 处理通知打开事件（支持缓存）
 */
- (void)handleNotificationOpened:(NSString *)title 
                            body:(NSString *)body 
                        subtitle:(NSString *)subtitle 
                        userInfo:(NSString *)userInfo {
    if (self.onNotificationOpenedCallback != NULL) {
        // 回调已设置，直接调用
        self.onNotificationOpenedCallback([title UTF8String], [body UTF8String], [subtitle UTF8String], [userInfo UTF8String]);
    } else {
        // 回调未设置，缓存数据
        NSLog(@"[AliyunPush] Notification opened callback not set, caching data");
        NotificationClickData *data = [[NotificationClickData alloc] initWithTitle:title 
                                                                              body:body 
                                                                          subtitle:subtitle 
                                                                          userInfo:userInfo 
                                                                         clickType:NotificationClickTypeOpened];
        @synchronized (self.pendingClickDataList) {
            [self.pendingClickDataList addObject:data];
        }
    }
}

/**
 * 处理通知移除事件（支持缓存）
 */
- (void)handleNotificationRemoved:(NSString *)userInfo {
    if (self.onNotificationRemovedCallback != NULL) {
        // 回调已设置，直接调用
        self.onNotificationRemovedCallback([userInfo UTF8String]);
    } else {
        // 回调未设置，缓存数据
        NSLog(@"[AliyunPush] Notification removed callback not set, caching data");
        NotificationClickData *data = [[NotificationClickData alloc] initWithTitle:@"" 
                                                                              body:@"" 
                                                                          subtitle:@"" 
                                                                          userInfo:userInfo 
                                                                         clickType:NotificationClickTypeRemoved];
        @synchronized (self.pendingClickDataList) {
            [self.pendingClickDataList addObject:data];
        }
    }
}

/**
 * 处理远程通知接收事件（支持缓存）
 */
- (void)handleRemoteNotificationReceived:(NSString *)title 
                                    body:(NSString *)body 
                                subtitle:(NSString *)subtitle 
                                userInfo:(NSString *)userInfo {
    if (self.onRemoteNotificationReceived != NULL) {
        // 回调已设置，直接调用
        self.onRemoteNotificationReceived([title UTF8String], [body UTF8String], [subtitle UTF8String], [userInfo UTF8String]);
    } else {
        // 回调未设置，缓存数据
        NSLog(@"[AliyunPush] Remote notification callback not set, caching data");
        NotificationClickData *data = [[NotificationClickData alloc] initWithTitle:title 
                                                                              body:body 
                                                                          subtitle:subtitle 
                                                                          userInfo:userInfo 
                                                                         clickType:NotificationClickTypeRemote];
        @synchronized (self.pendingClickDataList) {
            [self.pendingClickDataList addObject:data];
        }
    }
}

/**
 * 发送缓存的通知数据
 */
- (void)sendPendingClickData {
    @synchronized (self.pendingClickDataList) {
        NSLog(@"[AliyunPush] sendPendingClickData count %lu", (unsigned long)self.pendingClickDataList.count);
        if (self.pendingClickDataList.count > 0) {
            NSLog(@"[AliyunPush] Sending %lu pending notification data", (unsigned long)self.pendingClickDataList.count);
            
            NSArray<NotificationClickData *> *tmpList = [self.pendingClickDataList copy];
            for (NotificationClickData *data in tmpList) {
                switch (data.clickType) {
                    case NotificationClickTypeOpened:
                        if (self.onNotificationOpenedCallback != NULL) {
                            self.onNotificationOpenedCallback([data.title UTF8String], 
                                                            [data.body UTF8String], 
                                                            [data.subtitle UTF8String], 
                                                            [data.userInfo UTF8String]);
                        }
                        break;
                        
                    case NotificationClickTypeRemoved:
                        if (self.onNotificationRemovedCallback != NULL) {
                            self.onNotificationRemovedCallback([data.userInfo UTF8String]);
                        }
                        break;
                        
                    case NotificationClickTypeRemote:
                        if (self.onRemoteNotificationReceived != NULL) {
                            self.onRemoteNotificationReceived([data.title UTF8String], 
                                                            [data.body UTF8String], 
                                                            [data.subtitle UTF8String], 
                                                            [data.userInfo UTF8String]);
                        }
                        break;
                }
            }
            
            [self.pendingClickDataList removeObjectsInArray:tmpList];
        }
    }
}

- (void)initCloudPush:(NSString *)appKey appSecret:(NSString *)appSecret actionId:(int)actionId {
    //APNS注册，获取deviceToken并上报
    [self registerAPNS];
    
    // SDK初始化，使用新版startWithAppkey方法
    [CloudPushSDK startWithAppkey:appKey appSecret:appSecret callback:^(CloudPushCallbackResult *res) {
        if (res.success) {
            NSLog(@"Push SDK init success, deviceId: %@.", [CloudPushSDK getDeviceId]);
            [self registerMessageReceive];
            [self registerChannelListener];
        } else {
            NSLog(@"Push SDK init failed, error: %@", res.error);
        }
        [self invokeCallback:res actionId:actionId];
    }];

    // 发送缓存的通知数据
    [self sendPendingClickData];
}

- (void)bindAccount:(NSString *)account actionId:(int)actionId {
    [CloudPushSDK bindAccount:account withCallback:^(CloudPushCallbackResult *res) {
        [self invokeCallback:res actionId:actionId];
    }];
}

- (void)unBindAccount:(int)actionId {
    [CloudPushSDK unbindAccount:^(CloudPushCallbackResult *res) {
        [self invokeCallback:res actionId:actionId];
    }];
}

- (void)bindTag:(int)target
       withTag:(NSString *)tag
      withAlias:(NSString *)alias
       actionId:(int)actionId{
    NSArray *tagArray = [[NSArray alloc] initWithObjects:tag, nil];
    [CloudPushSDK bindTag:target withTags:tagArray withAlias:alias withCallback:^(CloudPushCallbackResult *res) {
        [self invokeCallback:res actionId:actionId];
    }];
}

- (void)unBindTag:(int)target
         withTag:(NSString *)tag
        withAlias:(NSString *)alias
         actionId:(int)actionId {
    NSArray *tagArray = [[NSArray alloc] initWithObjects:tag, nil];
    [CloudPushSDK unbindTag:target withTags:tagArray withAlias:alias withCallback:^(CloudPushCallbackResult *res) {
        [self invokeCallback:res actionId:actionId];
    }];
}

- (void)listTags:(int)target
        actionId:(int)actionId {
    [CloudPushSDK listTags:target withCallback:^(CloudPushCallbackResult *res) {
        [self invokeCallback:res actionId:actionId];
    }];
}

- (void)addAlias:(NSString *)alias
        actionId:(int)actionId {
    [CloudPushSDK addAlias:alias withCallback:^(CloudPushCallbackResult *res) {
        [self invokeCallback:res actionId:actionId];
    }];
}

- (void)removeAlias:(NSString *)alias
           actionId:(int)actionId{
    [CloudPushSDK removeAlias:alias withCallback:^(CloudPushCallbackResult *res) {
        [self invokeCallback:res actionId:actionId];
    }];
}

- (void)listAliases:(int)actionId {
    [CloudPushSDK listAliases:^(CloudPushCallbackResult *res) {
        [self invokeCallback:res actionId:actionId];
    }];
}


/**
 *    @brief    注册推送消息到来监听
 */
- (void)registerMessageReceive {
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(onMessageReceived:)
                                                 name:@"CCPDidReceiveMessageNotification"
                                               object:nil];
}

/**
 *    @brief    注册通道状态监听
 */
- (void)registerChannelListener {
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(onChannelOpened:)
                                                 name:@"CCPDidChannelConnectedSuccess"
                                               object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(onChannelDisconnected:)
                                                 name:@"CCPDidChannelDisconnected"
                                               object:nil];
}

/**
 *    处理到来推送消息
 *
 *    @param     notification
 */
- (void)onMessageReceived:(NSNotification *)notification {
    NSLog(@"Receive one message!");
   
    NSDictionary *data = [notification object];
    NSString *title = data[@"title"];
    NSString *content = data[@"content"];
    NSLog(@"Receive message title: %@, content: %@.", title, content);
    
    if(self.onMessageReceivedCallback != NULL){
        self.onMessageReceivedCallback([title UTF8String], [content UTF8String]);
    }
}

/**
 *    通道连接成功
 */
- (void)onChannelOpened:(NSNotification *)notification {
    NSLog(@"Push channel opened.");
}

/**
 *    通道断开连接
 */
- (void)onChannelDisconnected:(NSNotification *)notification {
    NSLog(@"Push channel disconnected.");
}

/**
 *    创建并注册通知category (iOS 10+)
 */
- (void)createCustomNotificationCategory {
    // 创建默认打开action
    UNNotificationAction *openAction = [UNNotificationAction actionWithIdentifier:UNNotificationDefaultActionIdentifier 
                                                                            title:@"打开" 
                                                                          options:UNNotificationActionOptionForeground];
    
    // 创建删除action
    UNNotificationAction *dismissAction = [UNNotificationAction actionWithIdentifier:UNNotificationDismissActionIdentifier 
                                                                                title:@"删除" 
                                                                              options:UNNotificationActionOptionDestructive];
    
    // 创建category并注册actions
    UNNotificationCategory *category = [UNNotificationCategory categoryWithIdentifier:@"aliyun_push_category" 
                                                                              actions:@[openAction, dismissAction] 
                                                                    intentIdentifiers:@[] 
                                                                              options:UNNotificationCategoryOptionCustomDismissAction];
    
    // 注册category到通知中心
    [self.notificationCenter setNotificationCategories:[NSSet setWithObjects:category, nil]];
    NSLog(@"[AliyunPush] Notification category registered");
}

/**
 *    请求APNS授权并注册
 */
- (void)registerAPNS {
    // 确保通知中心实例存在（通常已在应用启动时设置）
    if (self.notificationCenter == nil) {
        self.notificationCenter = [UNUserNotificationCenter currentNotificationCenter];
        self.notificationCenter.delegate = self;
        NSLog(@"[AliyunPush] Set UNUserNotificationCenter delegate in registerAPNS (fallback)");
    } else {
        NSLog(@"[AliyunPush] UNUserNotificationCenter already configured");
    }
    
    // 创建并注册通知category
    [self createCustomNotificationCategory];
    
    // 请求推送权限
    [self.notificationCenter requestAuthorizationWithOptions:UNAuthorizationOptionAlert | UNAuthorizationOptionBadge | UNAuthorizationOptionSound 
                                           completionHandler:^(BOOL granted, NSError * _Nullable error) {
        if (granted) {
            NSLog(@"[AliyunPush] User authorized notification.");
            // 向APNs注册，获取deviceToken
            dispatch_async(dispatch_get_main_queue(), ^{
                [[UIApplication sharedApplication] registerForRemoteNotifications];
            });
        } else {
            NSLog(@"[AliyunPush] User denied notification, error: %@", error);
            if (self.onAPNSRegisterFailedCallback != NULL) {
                NSString *errorMsg = error ? [NSString stringWithFormat:@"%@", error] : @"User denied notification";
                self.onAPNSRegisterFailedCallback([errorMsg UTF8String]);
            }
        }
    }];
}

#pragma mark - UNUserNotificationCenterDelegate

/**
 *    APP处于前台时收到通知
 */
- (void)userNotificationCenter:(UNUserNotificationCenter *)center 
       willPresentNotification:(UNNotification *)notification 
         withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler {
    NSLog(@"Received notification in foreground");
    
    NSDictionary *userInfo = notification.request.content.userInfo;
    
    // 根据模式决定是否调用回调
    BOOL shouldCallback = ([_foregroundNotificationMode isEqualToString:@"show_and_callback"] || 
                          [_foregroundNotificationMode isEqualToString:@"callback_only"]);
    
    if (shouldCallback && self.onNotificationReceivedCallback != NULL) {
        NSString *title = notification.request.content.title ?: @"";
        NSString *body = notification.request.content.body ?: @"";
        NSString *subtitle = notification.request.content.subtitle ?: @"";
        
        // 将userInfo转换为JSON字符串
        NSError *error;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:userInfo options:0 error:&error];
        NSString *userInfoJson = @"{}";
        if (!error && jsonData) {
            userInfoJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        }
        
        self.onNotificationReceivedCallback([title UTF8String], [body UTF8String], [subtitle UTF8String], [userInfoJson UTF8String]);
    }
    
    // 根据模式决定是否展示通知
    BOOL shouldShow = ([_foregroundNotificationMode isEqualToString:@"show_and_callback"] || 
                      [_foregroundNotificationMode isEqualToString:@"show_only"]);
    
    if (shouldShow) {
        // 展示通知
        completionHandler(UNNotificationPresentationOptionAlert | UNNotificationPresentationOptionSound | UNNotificationPresentationOptionBadge);
    } else {
        // 不展示通知
        // 上报通知点击事件
        [CloudPushSDK sendNotificationAck:userInfo];
        completionHandler(UNNotificationPresentationOptionNone);
    }
}

/**
 *    点击通知时回调
 */
- (void)userNotificationCenter:(UNUserNotificationCenter *)center 
didReceiveNotificationResponse:(UNNotificationResponse *)response 
         withCompletionHandler:(void (^)(void))completionHandler {
    NSString *userAction = response.actionIdentifier;
    
    // 点击通知打开
    if ([userAction isEqualToString:UNNotificationDefaultActionIdentifier]) {
        NSLog(@"User tapped notification");
        
        NSDictionary *userInfo = response.notification.request.content.userInfo;
        
        // 上报通知点击事件
        [CloudPushSDK sendNotificationAck:userInfo];
        
        // 提取通知内容
        NSString *title = response.notification.request.content.title ?: @"";
        NSString *body = response.notification.request.content.body ?: @"";
        NSString *subtitle = response.notification.request.content.subtitle ?: @"";
        
        // 将userInfo转换为JSON字符串
        NSError *error;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:userInfo options:0 error:&error];
        NSString *userInfoJson = @"{}";
        if (!error && jsonData) {
            userInfoJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        }
        
        // 使用缓存处理方法
        [self handleNotificationOpened:title body:body subtitle:subtitle userInfo:userInfoJson];
    }
    
    // 通知清除
    if ([userAction isEqualToString:UNNotificationDismissActionIdentifier]) {
        NSLog(@"User dismissed notification");
        NSDictionary *userInfo = response.notification.request.content.userInfo;
        
        // 上报通知清除事件
        [CloudPushSDK sendDeleteNotificationAck:userInfo];
        
        // 将userInfo转换为JSON字符串
        NSError *error;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:userInfo options:0 error:&error];
        NSString *userInfoJson = @"{}";
        if (!error && jsonData) {
            userInfoJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        }
        
        // 使用缓存处理方法
        [self handleNotificationRemoved:userInfoJson];
    }
    
    completionHandler();
}

@end
