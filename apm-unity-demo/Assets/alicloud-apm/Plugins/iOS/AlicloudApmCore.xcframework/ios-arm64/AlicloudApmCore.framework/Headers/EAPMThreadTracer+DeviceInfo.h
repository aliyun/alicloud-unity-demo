#import "EAPMThreadTracer.h"
#import <mach/mach.h>

@interface EAPMThreadTracer (DeviceInfo)

/// @brief 格式化的设备信息，但是不包括cpu和内存的用量
+ (NSString *)formattedDeviceInfo;
+ (NSString *)formattedDeviceInfoWithCustomizedReason:(NSString *)reason;

+ (NSString *)formattedDeviceInfoWithCustomizedReason:(NSString *)reason reportVersion:(NSString *)reportVersion;

+ (float)cpuUsageOfThread:(thread_t)thread;

+ (vm_size_t)memoryUsageOfTask:(task_t)task;

// 是否为64位cpu type
+ (boolean_t)islp64;

@end
