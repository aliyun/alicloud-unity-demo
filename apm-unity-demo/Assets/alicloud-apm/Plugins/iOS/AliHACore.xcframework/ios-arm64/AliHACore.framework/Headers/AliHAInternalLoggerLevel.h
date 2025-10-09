#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/// The log levels used by internal logging.
typedef NS_ENUM(NSInteger, AliHAInternalLoggerLevel) {
    /// Error level, corresponding to `OS_LOG_TYPE_ERROR`.
    AliHAInternalLoggerLevelError = 3, // For backwards compatibility, the enum value matches `ASL_LEVEL_ERR`.

    /// Warning level, corresponding to `OS_LOG_TYPE_DEFAULT`.
    ///
    /// > Note: Since OSLog doesn't have a WARNING type, this is equivalent to `AliHAInternalLoggerLevelNotice`.
    AliHAInternalLoggerLevelWarning = 4, // For backwards compatibility, the value matches `ASL_LEVEL_WARNING`.

    /// Notice level, corresponding to `OS_LOG_TYPE_DEFAULT`.
    AliHAInternalLoggerLevelNotice = 5, // For backwards compatibility, the value matches `ASL_LEVEL_NOTICE`.

    /// Info level, corresponding to `OS_LOG_TYPE_INFO`.
    AliHAInternalLoggerLevelInfo = 6, // For backwards compatibility, the enum value matches `ASL_LEVEL_INFO`.

    /// Debug level, corresponding to `OS_LOG_TYPE_DEBUG`.
    AliHAInternalLoggerLevelDebug = 7, // For backwards compatibility, the value matches `ASL_LEVEL_DEBUG`.

    /// The minimum (most severe) supported logging level.
    AliHAInternalLoggerLevelMin = AliHAInternalLoggerLevelError,

    /// The maximum (least severe) supported logging level.
    AliHAInternalLoggerLevelMax = AliHAInternalLoggerLevelDebug
} NS_SWIFT_NAME(AliHAInternalLoggerLevel);

NS_ASSUME_NONNULL_END
