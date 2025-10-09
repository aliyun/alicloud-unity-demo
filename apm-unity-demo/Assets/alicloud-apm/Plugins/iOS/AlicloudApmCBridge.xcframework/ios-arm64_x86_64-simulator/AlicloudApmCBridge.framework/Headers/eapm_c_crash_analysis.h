#ifndef EAPM_C_CRASH_ANALYSIS_H
#define EAPM_C_CRASH_ANALYSIS_H

#include <stdbool.h>
#include <stdint.h>

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

// 堆栈帧结构体（与Unity中的NativeStackFrame对应）
typedef struct {
    const char *symbol;
    const char *file;
    int line;
    const char *library;
    uint64_t address;
} NativeStackFrame;

// 崩溃分析功能函数
EAPM_EXTERN EAPM_EXPORT void eapm_ca_record_exception(const char *name,
                                                      const char *reason,
                                                      NativeStackFrame *stackFrames,
                                                      int frameCount,
                                                      int language,
                                                      bool isCustom,
                                                      bool isUrgent,
                                                      bool quitApp);

EAPM_EXTERN EAPM_EXPORT void eapm_ca_log(const char *message);
EAPM_EXTERN EAPM_EXPORT void eapm_ca_set_internal_key_value(const char *key, const char *value);

#endif /* EAPM_C_CRASH_ANALYSIS_H */