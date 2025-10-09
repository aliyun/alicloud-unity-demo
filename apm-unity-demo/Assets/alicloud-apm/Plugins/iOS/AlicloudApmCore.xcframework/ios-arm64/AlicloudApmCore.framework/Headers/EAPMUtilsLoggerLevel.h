#import <Foundation/Foundation.h>

#import "EAPMUtilsLoggerLevel.h"

NS_ASSUME_NONNULL_BEGIN

/// The log levels used by internal logging.
typedef NS_ENUM(NSInteger, EAPMUtilsLoggerLevel) {
    /// Error level, corresponding to `OS_LOG_TYPE_ERROR`.
    EAPMUtilsLoggerLevelError = 3, // For backwards compatibility, the enum value matches `ASL_LEVEL_ERR`.

    /// Warning level, corresponding to `OS_LOG_TYPE_DEFAULT`.
    ///
    /// > Note: Since OSLog doesn't have a WARNING type, this is equivalent to `EAPMUtilsLoggerLevelNotice`.
    EAPMUtilsLoggerLevelWarning = 4, // For backwards compatibility, the value matches `ASL_LEVEL_WARNING`.

    /// Notice level, corresponding to `OS_LOG_TYPE_DEFAULT`.
    EAPMUtilsLoggerLevelNotice = 5, // For backwards compatibility, the value matches `ASL_LEVEL_NOTICE`.

    /// Info level, corresponding to `OS_LOG_TYPE_INFO`.
    EAPMUtilsLoggerLevelInfo = 6, // For backwards compatibility, the enum value matches `ASL_LEVEL_INFO`.

    /// Debug level, corresponding to `OS_LOG_TYPE_DEBUG`.
    EAPMUtilsLoggerLevelDebug = 7, // For backwards compatibility, the value matches `ASL_LEVEL_DEBUG`.

    /// The minimum (most severe) supported logging level.
    EAPMUtilsLoggerLevelMin = EAPMUtilsLoggerLevelError,

    /// The maximum (least severe) supported logging level.
    EAPMUtilsLoggerLevelMax = EAPMUtilsLoggerLevelDebug
} NS_SWIFT_NAME(UtilsLoggerLevel);

NS_ASSUME_NONNULL_END
