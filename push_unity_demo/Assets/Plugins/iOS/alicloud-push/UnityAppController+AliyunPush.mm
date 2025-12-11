//
//  UnityAppController+AliyunPush.mm
//  Unity-iPhone
//
//  阿里云移动推送 - Unity AppController扩展
//  用于接收UIApplicationDelegate回调
//

#import <Foundation/Foundation.h>
#import <CloudPushSDK/CloudPushSDK.h>
#import <UserNotifications/UserNotifications.h>
#import "UnityAppController.h"
#import "PushWrapper.h"

@interface UnityAppController (AliyunPush)
@end

@implementation UnityAppController (AliyunPush)

/**
 * 应用启动完成回调
 * 在这里提前设置 UNUserNotificationCenter 的 delegate，确保能接收到冷启动的通知点击事件
 */
- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    NSLog(@"[AliyunPush] Application did finish launching - setting up notification center");
    
    // 提前设置通知中心的 delegate
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    PushWrapper *wrapper = [PushWrapper sharedInstance];
    center.delegate = wrapper;
    wrapper.notificationCenter = center;
    
    NSLog(@"[AliyunPush] UNUserNotificationCenter delegate set at app launch");
    
    // 返回 YES 表示应用正常启动
    return YES;
}

/**
 * APNs注册成功回调
 * 系统会在用户授权推送权限后调用此方法，返回deviceToken
 */
- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken {
    NSLog(@"[AliyunPush] APNs注册成功，收到deviceToken");
    
    // 将deviceToken上传到阿里云推送服务器
    [CloudPushSDK registerDevice:deviceToken withCallback:^(CloudPushCallbackResult *res) {
        PushWrapper *wrapper = [PushWrapper sharedInstance];
        
        if (res.success) {
            NSLog(@"[AliyunPush] DeviceToken上传成功: %@", [CloudPushSDK getApnsDeviceToken]);
            
            // 回调Unity层
            if (wrapper.onAPNSRegisterSuccessCallback != NULL) {
                NSString *token = [CloudPushSDK getApnsDeviceToken];
                wrapper.onAPNSRegisterSuccessCallback([token UTF8String]);
            }
        } else {
            NSLog(@"[AliyunPush] DeviceToken上传失败: %@", res.error);
            
            // 回调Unity层
            if (wrapper.onAPNSRegisterFailedCallback != NULL) {
                NSString *errorMsg = [NSString stringWithFormat:@"%@", res.error];
                wrapper.onAPNSRegisterFailedCallback([errorMsg UTF8String]);
            }
        }
    }];
}

/**
 * APNs注册失败回调
 * 当设备无法注册推送服务时调用（例如模拟器、网络问题等）
 */
- (void)application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error {
    NSLog(@"[AliyunPush] APNs注册失败: %@", error);
    
    PushWrapper *wrapper = [PushWrapper sharedInstance];
    if (wrapper.onAPNSRegisterFailedCallback != NULL) {
        NSString *errorMsg = [NSString stringWithFormat:@"%@", error];
        wrapper.onAPNSRegisterFailedCallback([errorMsg UTF8String]);
    }
}

/**
 * 静默通知回调
 * 当收到静默推送（content-available=1）或应用在后台收到通知时调用
 * 这个方法可以在应用处于后台时被唤醒执行
 */
- (void)application:(UIApplication *)application 
didReceiveRemoteNotification:(NSDictionary *)userInfo 
fetchCompletionHandler:(void (^)(UIBackgroundFetchResult))completionHandler {
    NSLog(@"[AliyunPush] 收到远程通知（静默或后台）");
    
    // 上报通知回执
    [CloudPushSDK sendNotificationAck:userInfo];
    
    // 提取通知内容
    NSString *title = @"";
    NSString *body = @"";
    NSString *subtitle = @"";
    
    // 尝试解析通知内容
    NSDictionary *aps = userInfo[@"aps"];
    if (aps) {
        id alert = aps[@"alert"];
        if ([alert isKindOfClass:[NSDictionary class]]) {
            NSDictionary *alertDict = (NSDictionary *)alert;
            title = alertDict[@"title"] ?: @"";
            body = alertDict[@"body"] ?: @"";
            subtitle = alertDict[@"subtitle"] ?: @"";
        } else if ([alert isKindOfClass:[NSString class]]) {
            body = (NSString *)alert;
        }
    }
    
    // 将userInfo转换为JSON字符串
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:userInfo options:0 error:&error];
    NSString *userInfoJson = @"{}";
    if (!error && jsonData) {
        userInfoJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    // 使用缓存处理方法
    PushWrapper *wrapper = [PushWrapper sharedInstance];
    [wrapper handleRemoteNotificationReceived:title body:body subtitle:subtitle userInfo:userInfoJson];
    
    // 告诉系统已处理完成
    completionHandler(UIBackgroundFetchResultNewData);
}

@end
