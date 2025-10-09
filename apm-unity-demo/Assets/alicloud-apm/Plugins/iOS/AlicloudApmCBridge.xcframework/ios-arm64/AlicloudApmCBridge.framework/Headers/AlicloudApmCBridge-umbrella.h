#ifdef __OBJC__
#import <UIKit/UIKit.h>
#else
#ifndef FOUNDATION_EXPORT
#if defined(__cplusplus)
#define FOUNDATION_EXPORT extern "C"
#else
#define FOUNDATION_EXPORT extern
#endif
#endif
#endif

#import "eapm_c_apm.h"
#import "eapm_c_apm_configuration.h"
#import "eapm_c_crash_analysis.h"

FOUNDATION_EXPORT double AlicloudApmCBridgeVersionNumber;
FOUNDATION_EXPORT const unsigned char AlicloudApmCBridgeVersionString[];

