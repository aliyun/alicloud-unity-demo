#ifndef EAPM_C_APM_CONFIGURATION_H
#define EAPM_C_APM_CONFIGURATION_H

#ifdef __cplusplus
#define EAPM_EXTERN extern "C"
#else
#define EAPM_EXTERN extern
#endif

#ifndef EAPM_EXPORT
#if defined(__GNUC__) || defined(__clang__)
#define EAPM_EXPORT __attribute__((visibility("default")))
#else
#define EAPM_EXPORT
#endif
#endif

// 环境配置常量
#define EAPM_ENVIRONMENT_PRE        0  // 预发环境
#define EAPM_ENVIRONMENT_PRODUCTION 1  // 生产环境

// APM配置相关函数
EAPM_EXTERN EAPM_EXPORT void eapm_set_logger_level(int loggerLevel);

// 环境配置函数
EAPM_EXTERN EAPM_EXPORT void eapm_set_environment(int environment);

#endif /* EAPM_C_APM_CONFIGURATION_H */