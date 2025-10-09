#ifndef EAPMAPPSTATEINFO_H
#define EAPMAPPSTATEINFO_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdbool.h>
#include <mach/mach_types.h>

__BEGIN_DECLS

typedef struct {
    /** Timestamp for when the app state was last changed */
    double appStateTransitionTime;

    /** If true, the application is currently active */
    bool applicationIsActive;

    /** If true, the application is currently in the foreground */
    bool applicationIsInForeground;

    thread_t mainThread;
} EAPMAppStateInfo;

extern EAPMAppStateInfo gEAPMAppStateInfo;

__END_DECLS

#ifdef __cplusplus
}
#endif

#endif // EAPMAPPSTATEINFO_H
