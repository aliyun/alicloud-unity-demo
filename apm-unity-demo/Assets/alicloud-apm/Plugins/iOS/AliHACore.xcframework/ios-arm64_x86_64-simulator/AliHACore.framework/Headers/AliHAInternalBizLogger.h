#ifndef ALiHA_INTERNALBIZLOGGER_H
#define ALiHA_INTERNALBIZLOGGER_H

#import <Foundation/Foundation.h>
#import "AliHAInternalLoggerLevel.h"

NS_ASSUME_NONNULL_BEGIN

extern NSString *const kAliHAInternalLoggerDefaultCategory;

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus


/**
 * Logs a message to the Xcode console and the device log. If running from AppStore, will
 * not log any messages with a level higher than AliHAInternalLoggerLevelNotice to avoid log spamming.
 * (required) log level (one of the AliHAInternalLoggerLevel enum values).
 * (required) service name of type AliHAInternalBizLoggerService.
 *            An example of the message code is @"I-COR000001".
 * (required) message string which can be a format string.
 * (optional) variable arguments list obtained from calling va_start, used when message is a format
 *            string.
 */
extern void AliHAInternalBizLogBasic(AliHAInternalLoggerLevel level,
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
 * (required) service name of type AliHAInternalBizLoggerService.
 * (required) message string which can be a format string.
 * (optional) the list of arguments to substitute into the format string.
 * Example usage:
 * AliHAInternalBizLogNotice(kAliHAInternalBizLoggerCore, @"Configuration of %@ failed.", apm.name);
 */
extern void AliHAInternalBizLogError(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);
extern void AliHAInternalBizLogWarning(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);
extern void AliHAInternalBizLogNotice(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);
extern void AliHAInternalBizLogInfo(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);
extern void AliHAInternalBizLogDebug(NSString *category, NSString *message, ...) NS_FORMAT_FUNCTION(2, 3);

/**
 * This function is similar to the one above, except it takes a `va_list` instead of the listed
 * variables.
 *
 * The following functions accept the following parameters in order: (required) service
 * name of type AliHAInternalBizLoggerService.
 *
 * (required) message string which can be a format string.
 * (optional) A va_list
 */
extern void AliHAInternalBizLogBasicError(NSString *category, NSString *message, va_list args_ptr);
extern void AliHAInternalBizLogBasicWarning(NSString *category, NSString *message, va_list args_ptr);
extern void AliHAInternalBizLogBasicNotice(NSString *category, NSString *message, va_list args_ptr);
extern void AliHAInternalBizLogBasicInfo(NSString *category, NSString *message, va_list args_ptr);
extern void AliHAInternalBizLogBasicDebug(NSString *category, NSString *message, va_list args_ptr);

#ifdef __cplusplus
} // extern "C"
#endif // __cplusplus

NS_SWIFT_NAME(AliHAInternalBizLogger)
@interface AliHAInternalBizLoggerWrapper : NSObject

/// Logs a given message at a given log level.
///
/// - Parameters:
///   - level: The log level to use (defined by `AliHAInternalLoggerLevel` enum values).
///   - category: The service name of type `AliHAInternalBizLoggerService`.
///   - message: Formatted string to be used as the log's message.
+ (void)logWithLevel:(AliHAInternalLoggerLevel)level
             service:(NSString *)category
             message:(NSString *)message __attribute__((__swift_name__("log(level:service:message:)")));

@end

NS_ASSUME_NONNULL_END

#endif // ALiHA_INTERNALBIZLOGGER_H
