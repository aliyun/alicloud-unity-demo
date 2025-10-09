#import <Foundation/Foundation.h>

#import "EAPMUtilsLoggerLevel.h"

NS_ASSUME_NONNULL_BEGIN

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

/// Used for other logging.
extern NSString *const kEAPMLogSubsystem;

/// Initialize EAPMUtilsLogger.
extern void EAPMUtilsLoggerInitialize(void);

/// Override log level to Debug.
void EAPMUtilsLoggerForceDebug(void);

/// Gets the current `EAPMUtilsLoggerLevel`.
extern EAPMUtilsLoggerLevel EAPMUtilsGetLoggerLevel(void);

/**
 * Changes the default logging level of EAPMUtilsLoggerLevelNotice to a user-specified level.
 * The default level cannot be set above EAPMUtilsLoggerLevelNotice if the app is running from App Store.
 * (required) log level (one of the EAPMUtilsLoggerLevel enum values).
 */
extern void EAPMUtilsSetLoggerLevel(EAPMUtilsLoggerLevel loggerLevel);

/**
 * Checks if the specified logger level is loggable given the current settings.
 * (required) log level (one of the EAPMUtilsLoggerLevel enum values).
 */
extern BOOL EAPMUtilsIsLoggableLevel(EAPMUtilsLoggerLevel loggerLevel);

/**
 * Register version to include in logs.
 * (required) version
 */
extern void EAPMUtilsLoggerRegisterVersion(NSString *version);

/**
 * Logs a message to the Xcode console and the device log. If running from AppStore, will
 * not log any messages with a level higher than EAPMUtilsLoggerLevelNotice to avoid log spamming.
 * (required) log level (one of the EAPMUtilsLoggerLevel enum values).
 * (required) service name of type NSString *.
 * (required) message code
 * (required) message string which can be a format string.
 * (optional) variable arguments list obtained from calling va_start, used when message is a format
 *            string.
 */
extern void EAPMOSLogBasic(EAPMUtilsLoggerLevel level,
                           NSString *subsystem,
                           NSString *category,
                           BOOL forceLog,
                           NSString *message,
// On 64-bit simulators, va_list is not a pointer, so cannot be marked nullable
// See: http://stackoverflow.com/q/29095469
#if __LP64__ && TARGET_OS_SIMULATOR || TARGET_OS_OSX
                           va_list args_ptr
#else
                           va_list _Nullable args_ptr
#endif
);

/**
 * The following functions accept the following parameters in order:
 * (required) service name of type NSString *.
 * (required) message string which can be a format string.
 * (optional) the list of arguments to substitute into the format string.
 */
extern void EAPMOSLogError(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);
extern void EAPMOSLogWarning(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);
extern void EAPMOSLogNotice(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);
extern void EAPMOSLogInfo(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);
extern void EAPMOSLogDebug(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);

#ifdef __cplusplus
} // extern "C"
#endif // __cplusplus

@interface EAPMUtilsLoggerWrapper : NSObject

/// Objective-C wrapper for `EAPMOSLogBasic` to allow weak linking to `EAPMUtilsLogger`.
///
/// - Parameters:
///   - level: The log level (one of the `EAPMUtilsLoggerLevel` enum values).
///   - subsystem: An identifier for the subsystem performing logging, e.g., `com.example.logger`.
///   - category: The category name within the `subsystem` to group related messages
///   - message: The message to log, which may be a format string.
///   - arguments: The variable arguments list obtained from calling va_start, used when message is
///     a format string; optional if `message` is not a format string.
+ (void)logWithLevel:(EAPMUtilsLoggerLevel)level
           subsystem:(NSString *)subsystem
            category:(NSString *)category
             message:(NSString *)message
           arguments:(va_list)args;

@end

NS_ASSUME_NONNULL_END
