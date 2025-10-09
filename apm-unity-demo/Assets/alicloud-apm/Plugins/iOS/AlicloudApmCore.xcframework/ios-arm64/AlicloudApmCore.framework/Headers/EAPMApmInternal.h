#ifndef EAPMCORE_EAPMAPMINTERNAL_H
#define EAPMCORE_EAPMAPMINTERNAL_H

#import <AlicloudApmCore/EAPMApm.h>

@class EAPMComponentContainer;
@protocol EAPMLibrary;
@class EAPMExecutionIdentifierModel;
@class EAPMUserAgent;
@class EAPMSetting;
@class EAPMTraceManager;
@class EAPMTransport;

/**
 * The internal interface to `EAPMApm`. This is meant for first-party integrators, who need to
 * receive `EAPMApm` notifications, log info about the success or failure of their
 * configuration, and access other internal functionality of `EAPMApm`.
 */
NS_ASSUME_NONNULL_BEGIN

FOUNDATION_EXPORT NSDate * _Nullable gEAPMAppStartTime;

typedef NS_ENUM(NSInteger, EAPMConfigType) {
    EAPMConfigTypeCore = 1,
    EAPMConfigTypeSDK = 2,
};

extern NSString *const kEAPMDefaultApmName;
extern NSString *const kEAPMApmReadyToConfigureSDKNotification;
extern NSString *const kEAPMApmDeleteNotification;
extern NSString *const kEAPMApmIsDefaultAppKey;
extern NSString *const kEAPMApmNameKey;
extern NSString *const kEAPMApmUserAgentKey;
extern NSString *const kAlicloudApmCoreErrorDomain;

/**
 * Keys for the Options
 */

extern NSString *const kEAPMOptionsKeyAppKey;
extern NSString *const kEAPMOptionsKeyAppSecret;
extern NSString *const kEAPMOptionsKeyAppVersion;
extern NSString *const kEAPMOptionsKeyChannel;

/**
 * The format string for the `UserDefaults` key used for storing the data collection enabled flag.
 * This includes formatting to append the `EAPMApm`'s name.
 */
extern NSString *const kEAPMGlobalApmDataCollectionEnabledDefaultsKeyFormat;

/**
 * The plist key used for storing the data collection enabled flag.
 */
extern NSString *const kEAPMGlobalApmDataCollectionEnabledPlistKey;

@protocol EAPMCustomKeyValueProvider <NSObject>
- (void)setCustomValue:(nullable id)value forKey:(NSString *)key;
- (void)setCustomKeysAndValues:(NSDictionary *)keysAndValues;
@end

@interface EAPMApm ()

/**
 * A flag indicating if this is the default apm (has the default apm name).
 */
@property (nonatomic, readonly) BOOL isDefaultApm;

/**
 * The container of interop SDKs for this apm.
 */
@property (nonatomic) EAPMComponentContainer *container;

@property (nonatomic, readonly) EAPMExecutionIdentifierModel *executionIdentifierModel;

@property (nonatomic, readonly) EAPMSetting *setting;

@property (nonatomic, readonly) EAPMTransport *transport;

@property (nonatomic, readonly) EAPMTraceManager *traceManager;

@property (nonatomic, weak, nullable) id<EAPMCustomKeyValueProvider> customKeyValueProvider;

/**
 * Checks if the default apm is configured without trying to configure it.
 */
+ (BOOL)isDefaultApmConfigured;

/**
 * Registers a given third-party library with the given version number to be reported for
 * analytics.
 *
 * @param name Name of the library.
 * @param version Version of the library.
 */
+ (void)registerLibrary:(nonnull NSString *)name withVersion:(nonnull NSString *)version;

/**
 * Registers a given internal library to be reported for analytics.
 *
 * @param library Optional parameter for component registration.
 * @param name Name of the library.
 */
+ (void)registerInternalLibrary:(nonnull Class<EAPMLibrary>)library withName:(nonnull NSString *)name;

/**
 * Registers a given internal library with the given version number to be reported for
 * analytics. This should only be used for non-EAPM libraries that have their own versioning
 * scheme.
 *
 * @param library Optional parameter for component registration.
 * @param name Name of the library.
 * @param version Version of the library.
 */
+ (void)registerInternalLibrary:(nonnull Class<EAPMLibrary>)library
                       withName:(nonnull NSString *)name
                    withVersion:(nonnull NSString *)version;

/**
 * A concatenated string representing all the third-party libraries and version numbers.
 */
+ (NSString *)apmUserAgent;

/**
 * EAPMUserAgent Model representing all the third-party libraries and version numbers.
 */
+ (EAPMUserAgent *)userAgent;

/**
 * Can be used by the unit tests in each SDK to reset `EAPMApm`. This method is thread unsafe.
 */
+ (void)resetApms;

/**
 * 应用启动早期尝试获取default apm实例，此时可能尚未实例化，调用者需要自行处理返回nil的情况
 * 目前用于UIView事件记录日志
 */
+ (EAPMApm *)tryDefaultApm;

+ (BOOL)isAppFirstLaunch;

/**
 * Can be used by the unit tests in each SDK to set customized options.
 */
- (instancetype)initInstanceWithName:(NSString *)name options:(EAPMOptions *)options;

- (EAPMOptions *)originOptions;

@end

NS_ASSUME_NONNULL_END

#endif // EAPMCORE_EAPMAPMINTERNAL_H
