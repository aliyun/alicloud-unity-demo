#import <Foundation/Foundation.h>
#include "AlicloudApmCore/EAPMAppStateInfo.h"
#include "EAPMShareProcessTypes.h"

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMShareRuntime <NSObject>

/**
 * 应用状态
 */
- (EAPMAppStateInfo)appStateInfo;

/**
 * 进程状态
 */
- (EAPMShareProcessStats)processStats;

/**
 * 进程信息
 */
- (EAPMShareProcessInfo)processInfo;

/**
 * 组装应用状态、进程状态、进程信息
 */
- (NSDictionary *)getCommonInfo;

@end

NS_ASSUME_NONNULL_END

