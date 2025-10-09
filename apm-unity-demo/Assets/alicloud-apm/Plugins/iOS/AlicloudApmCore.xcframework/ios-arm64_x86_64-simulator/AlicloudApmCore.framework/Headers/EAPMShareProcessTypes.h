#ifndef EAPM_SHARE_PROCESS_TYPES_H
#define EAPM_SHARE_PROCESS_TYPES_H

#include <stdint.h>
#include <sys/types.h>

#ifdef __cplusplus
extern "C" {
#endif

#ifndef EAPM_PROCESS_NAME_MAX_LENGTH
#define EAPM_PROCESS_NAME_MAX_LENGTH 32
#endif

// Process memory and CPU statistics
typedef struct {
    uint64_t active;
    uint64_t inactive;
    uint64_t wired;
    uint64_t freeMem;
    uint64_t virtualSize;
    uint64_t residentSize;
    uint64_t userTime;
    uint64_t systemTime;
    uint64_t totalMem;
    uint64_t footprint;
    uint64_t internal;
    uint64_t compressed;
} EAPMShareProcessStats;

// Basic process info
typedef struct {
    pid_t pid;
    pid_t ppid;
    char processName[EAPM_PROCESS_NAME_MAX_LENGTH];
    char parentProcessName[EAPM_PROCESS_NAME_MAX_LENGTH];
    uint64_t launchTime;
} EAPMShareProcessInfo;

#ifdef __cplusplus
}
#endif

#endif /* EAPM_SHARE_PROCESS_TYPES_H */ 
