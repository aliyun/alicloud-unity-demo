#import <Foundation/Foundation.h>

#ifndef PushHelper_h
#define PushHelper_h

// 原有回调类型
typedef void (*CloudPushCallback)(int id, bool success, const char* msg);
typedef void (*MessageReceivedCallback)(const char* title, const char* content);

// 新增APNS和通知相关回调类型
typedef void (*APNSRegisterSuccessCallback)(const char* deviceToken);
typedef void (*APNSRegisterFailedCallback)(const char* error);
typedef void (*NotificationReceivedCallback)(const char* title, const char* body, const char* subtitle, const char* userInfo);
typedef void (*NotificationOpenedCallback)(const char* title, const char* body, const char* subtitle, const char* userInfo);
typedef void (*NotificationRemovedCallback)(const char* userInfo);

@interface PushHelper : NSObject

@end
#endif
