#import <Foundation/Foundation.h>

#import "AliHAInternalLoggerLevel.h"

NS_ASSUME_NONNULL_BEGIN

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

/// Used for other logging.
extern NSString *const kAliHAInternalLogSubsystem;

/// Initialize AliHAInternalLogger.
extern void AliHAInternalLoggerInitialize(void);

/// Gets the current `AliHAInternalLoggerLevel`.
extern AliHAInternalLoggerLevel AliHAInternalLoggerGetLoggerLevel(void);

extern void AliHAInternalLoggerSetLoggerLevel(AliHAInternalLoggerLevel loggerLevel);

/**
 * Logs a message to the Xcode console and the device log. If running from AppStore, will
 * not log any messages with a level higher than AliHAInternalLoggerLevelNotice to avoid log spamming.
 * (required) log level (one of the AliHAInternalLoggerLevel enum values).
 * (required) service name of type NSString *.
 * (required) message code
 * (required) message string which can be a format string.
 * (optional) variable arguments list obtained from calling va_start, used when message is a format
 *            string.
 */
extern void AliHAInternalOSLogBasic(AliHAInternalLoggerLevel level,
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
extern void AliHAInternalOSLogError(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);
extern void AliHAInternalOSLogWarning(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);
extern void AliHAInternalOSLogNotice(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);
extern void AliHAInternalOSLogInfo(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);
extern void AliHAInternalOSLogDebug(NSString *subsystem, NSString *category, BOOL force, NSString *message, ...) NS_FORMAT_FUNCTION(4, 5);

#ifdef __cplusplus
} // extern "C"
#endif // __cplusplus

@interface AliHAInternalLoggerWrapper : NSObject

/// Objective-C wrapper for `AliHAInternalOSLogBasic` to allow weak linking to `AliHAInternalLogger`.
///
/// - Parameters:
///   - level: The log level (one of the `AliHAInternalLoggerLevel` enum values).
///   - subsystem: An identifier for the subsystem performing logging, e.g., `com.example.logger`.
///   - category: The category name within the `subsystem` to group related messages
///   - message: The message to log, which may be a format string.
///   - arguments: The variable arguments list obtained from calling va_start, used when message is
///     a format string; optional if `message` is not a format string.
+ (void)logWithLevel:(AliHAInternalLoggerLevel)level
           subsystem:(NSString *)subsystem
            category:(NSString *)category
             message:(NSString *)message
           arguments:(va_list)args;

@end

NS_ASSUME_NONNULL_END
