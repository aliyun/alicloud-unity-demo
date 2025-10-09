#ifndef EAPM_C_APM_H
#define EAPM_C_APM_H

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

// SDK组件位标志定义
#define EAPM_SDK_CRASH_ANALYSIS  1   // 0x01 - 崩溃分析
#define EAPM_SDK_PERFORMANCE     2   // 0x02 - 性能分析
#define EAPM_SDK_REMOTE_LOG      4   // 0x04 - 远程日志
#define EAPM_SDK_MEM_ALLOC       8   // 0x08 - 内存分配
#define EAPM_SDK_MEM_LEAK        16  // 0x10 - 内存泄漏

// 便利宏定义
#define EAPM_SDK_ALL_COMPONENTS  (EAPM_SDK_CRASH_ANALYSIS | EAPM_SDK_PERFORMANCE | EAPM_SDK_REMOTE_LOG | EAPM_SDK_MEM_ALLOC | EAPM_SDK_MEM_LEAK)  // 启用所有组件

// APM核心功能函数
EAPM_EXTERN EAPM_EXPORT void eapm_start(const char *appKey, 
                                        const char *appSecret, 
                                        const char *appRsaSecret, 
                                        const char *appGroupId, 
                                        const char *userId, 
                                        const char *userNick, 
                                        const char *channel, 
                                        int sdkComponents);

EAPM_EXTERN EAPM_EXPORT void eapm_set_user_id(const char *userId);
EAPM_EXTERN EAPM_EXPORT void eapm_set_user_nick(const char *userNick);
EAPM_EXTERN EAPM_EXPORT void eapm_set_custom_value(const char *key, const char *value);

#endif /* EAPM_C_APM_H */
