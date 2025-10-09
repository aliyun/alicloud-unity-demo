/**
 * EAPM内部日志级别
 */
typedef NS_ENUM(NSInteger, EAPMLoggerLevel) {
    /** Error level, matches ASL_LEVEL_ERR. */
    EAPMLoggerLevelError = 3,
    /** Warning level, matches ASL_LEVEL_WARNING. */
    EAPMLoggerLevelWarning = 4,
    /** Notice level, matches ASL_LEVEL_NOTICE. */
    EAPMLoggerLevelNotice = 5,
    /** Info level, matches ASL_LEVEL_INFO. */
    EAPMLoggerLevelInfo = 6,
    /** Debug level, matches ASL_LEVEL_DEBUG. */
    EAPMLoggerLevelDebug = 7,
    /** Minimum log level. */
    EAPMLoggerLevelMin = EAPMLoggerLevelError,
    /** Maximum log level. */
    EAPMLoggerLevelMax = EAPMLoggerLevelDebug
} NS_SWIFT_NAME(EAPMLoggerLevel);
