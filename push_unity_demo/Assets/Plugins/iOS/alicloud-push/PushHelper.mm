#import "PushHelper.h"
#import "PushWrapper.h"

@implementation PushHelper


@end

#if defined(__cplusplus)
extern "C" {
#endif
    
    extern NSString* toNSString (const char* string);
#if defined(__cplusplus)
}
#endif


#if defined(__cplusplus)
extern "C" {
#endif

   
    /// 将NSString转换为C#可以使用的 char *
    /// @param source 目标串
    static char *toCString(const char *source) {
        if (!source)return NULL;
        char *dest = static_cast<char*>(malloc(strlen(source) + 1));
        if (dest)strcpy(dest, source);
        return dest;
    }
    
    NSString *toNSString (const char *string) {
        return [NSString stringWithUTF8String:(string ? string : "")];
    }

const char * _deviceId() {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    return toCString([[wrapper deviceId] UTF8String]);
}

void _setLogLevel(const char* logLevel) {
    NSString *levelStr = toNSString(logLevel);
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper setLogLevel:levelStr];
}

void _setForegroundNotificationMode(const char* mode) {
    NSString *modeStr = toNSString(mode);
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper setForegroundNotificationMode:modeStr];
}

void _setCloudPushCallback(CloudPushCallback callback) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    wrapper.onPushActionCallback = callback;
}

void _setMessageCallback(MessageReceivedCallback callback) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    wrapper.onMessageReceivedCallback = callback;
}

void _setAPNSRegisterSuccessCallback(APNSRegisterSuccessCallback callback) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    wrapper.onAPNSRegisterSuccessCallback = callback;
}

void _setAPNSRegisterFailedCallback(APNSRegisterFailedCallback callback) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    wrapper.onAPNSRegisterFailedCallback = callback;
}

void _setNotificationReceivedCallback(NotificationReceivedCallback callback) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    wrapper.onNotificationReceivedCallback = callback;
}

void _setNotificationOpenedCallback(NotificationOpenedCallback callback) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    wrapper.onNotificationOpenedCallback = callback;
}

void _setNotificationRemovedCallback(NotificationRemovedCallback callback) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    wrapper.onNotificationRemovedCallback = callback;
}

void _setRemoteNotificationReceivedCallback(NotificationReceivedCallback callback) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    wrapper.onRemoteNotificationReceived = callback;
}

void _initCloudPush(const char * appKey, const char * appSecret, int actionId) {
    NSString *appKeyTmp = toNSString(appKey);
    NSString *appSecretTmp = toNSString(appSecret);
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper initCloudPush:appKeyTmp appSecret:appSecretTmp actionId:actionId];
}

void _bindAccount(const char* account, int actionId){
    NSString *accountTmp = toNSString(account);
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper bindAccount:accountTmp actionId:actionId];
}

void _unBindAccount(int actionId){
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper unBindAccount:actionId];
}

void _bindTag(int target, const char* tag, const char* alias, int actionId) {
    NSString *tagTmp = toNSString(tag);
    NSString *aliasTmp = toNSString(alias);
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper bindTag:target withTag:tagTmp withAlias:aliasTmp actionId:actionId];
}

void _unBindTag(int target, const char* tag, const char* alias, int actionId) {
    NSString *tagTmp = toNSString(tag);
    NSString *aliasTmp = toNSString(alias);
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper unBindTag:target withTag:tagTmp withAlias:aliasTmp actionId:actionId];
}

void _listTag(int target, int actionId) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper listTags:target actionId:actionId];
}

void _addAlias(const char* alias, int actionId) {
    NSString *aliasTmp = toNSString(alias);
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper addAlias:aliasTmp actionId:actionId];
}

void _removeAlias(const char* alias, int actionId) {
    NSString *aliasTmp = toNSString(alias);
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper removeAlias:aliasTmp actionId:actionId];
}

void _listAlias(int actionId) {
    PushWrapper* wrapper = [PushWrapper sharedInstance];
    [wrapper listAliases:actionId];
}

#if defined(__cplusplus)
}
#endif
