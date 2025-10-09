#ifndef EAPMCORE_EAPMLOGGER_H
#define EAPMCORE_EAPMLOGGER_H

#import <Foundation/Foundation.h>

typedef NS_ENUM(NSInteger, EAPMLoggerLevel);

NS_ASSUME_NONNULL_BEGIN

/**
 * The EAPM services used in EAPM logger.
 */
typedef NSString *const EAPMLoggerService;

extern NSString *const kEAPMLoggerCore;

/**
 * The key used to store the logger's error count.
 */
extern NSString *const kEAPMLoggerErrorCountKey;

/**
 * The key used to store the logger's warning count.
 */
extern NSString *const kEAPMLoggerWarningCountKey;

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

/**
 * Enables or disables Analytics debug mode.
 * If set to true, the logging level for Analytics will be set to EAPMLoggerLevelDebug.
 * Enabling the debug mode has no effect if the app is running from App Store.
 * (required) analytics debug mode flag.
 */
void EAPMSetAnalyticsDebugMode(BOOL analyticsDebugMode);

/**
 * Gets the current EAPMLoggerLevel.
 */
EAPMLoggerLevel EAPMGetLoggerLevel(void);

/**
 * Changes the default logging level of EAPMLoggerLevelNotice to a user-specified level.
 * The default level cannot be set above EAPMLoggerLevelNotice if the app is running from App
 * Store. (required) log level (one of the EAPMLoggerLevel enum values).
 */
void EAPMSetLoggerLevel(EAPMLoggerLevel loggerLevel);

void EAPMSetLoggerLevelNotice(void);
void EAPMSetLoggerLevelWarning(void);
void EAPMSetLoggerLevelError(void);
void EAPMSetLoggerLevelDebug(void);

/**
 * Checks if the specified logger level is loggable given the current settings.
 * (required) log level (one of the EAPMLoggerLevel enum values).
 * (required) whether or not this function is called from the Analytics component.
 */
BOOL EAPMIsLoggableLevel(EAPMLoggerLevel loggerLevel, BOOL analyticsComponent);

BOOL EAPMIsLoggableLevelNotice(void);
BOOL EAPMIsLoggableLevelWarning(void);
BOOL EAPMIsLoggableLevelError(void);
BOOL EAPMIsLoggableLevelDebug(void);

/**
 * Logs a message to the Xcode console and the device log. If running from AppStore, will
 * not log any messages with a level higher than EAPMLoggerLevelNotice to avoid log spamming.
 * (required) log level (one of the EAPMLoggerLevel enum values).
 * (required) service name of type EAPMLoggerService.
 *            An example of the message code is @"I-COR000001".
 * (required) message string which can be a format string.
 * (optional) variable arguments list obtained from calling va_start, used when message is a format
 *            string.
 */
extern void EAPMLogBasic(EAPMLoggerLevel level,
                         NSString *category,
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
 * (required) service name of type EAPMLoggerService.
 * (required) message string which can be a format string.
 * (optional) the list of arguments to substitute into the format string.
 * Example usage:
 * EAPMLogError(kEAPMLoggerCore, @"Configuration of %@ failed.", apm.name);
 */
extern void EAPMLogError(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);
extern void EAPMLogWarning(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);
extern void EAPMLogNotice(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);
extern void EAPMLogInfo(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);
extern void EAPMLogDebug(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);

/**
 * This function is similar to the one above, except it takes a `va_list` instead of the listed
 * variables.
 *
 * The following functions accept the following parameters in order: (required) service
 * name of type EAPMLoggerService.
 *
 * (required) message string which can be a format string.
 * (optional) A va_list
 */
extern void EAPMLogBasicError(NSString *category, NSString *message, va_list args_ptr);
extern void EAPMLogBasicWarning(NSString *category, NSString *message, va_list args_ptr);
extern void EAPMLogBasicNotice(NSString *category, NSString *message, va_list args_ptr);
extern void EAPMLogBasicInfo(NSString *category, NSString *message, va_list args_ptr);
extern void EAPMLogBasicDebug(NSString *category, NSString *message, va_list args_ptr);

#ifdef __cplusplus
} // extern "C"
#endif // __cplusplus

NS_SWIFT_NAME(EAPMLogger)
@interface EAPMLoggerWrapper : NSObject

/// Logs a given message at a given log level.
///
/// - Parameters:
///   - level: The log level to use (defined by `EAPMLoggerLevel` enum values).
///   - category: The service name of type `EAPMLoggerService`.
///   - message: Formatted string to be used as the log's message.
+ (void)logWithLevel:(EAPMLoggerLevel)level
             service:(NSString *)category
             message:(NSString *)message __attribute__((__swift_name__("log(level:service:message:)")));

@end

NS_ASSUME_NONNULL_END

#endif // EAPMCORE_EAPMLOGGER_H
