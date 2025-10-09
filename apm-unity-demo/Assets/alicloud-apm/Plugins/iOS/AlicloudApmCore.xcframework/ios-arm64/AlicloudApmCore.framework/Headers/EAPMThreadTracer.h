#import "EAPMThreadScene.h"
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <mach/mach_types.h>

typedef enum : NSUInteger {
    EAPMThreadTraceReportDeviceInfo = 1UL << 0,
    EAPMThreadTraceReportCpuAndMem = 1UL << 1,
    EAPMThreadTraceReportMainThread = 1UL << 2,
    EAPMThreadTraceReportOtherThread = 1UL << 3,
    EAPMThreadTraceReportRegisters = 1UL << 4, // 目前该标志位无作用
    EAPMThreadTraceReportBinaryImages = 1UL << 5,

    EAPMThreadTraceReportMain = 0x27U, // 不包含OtherThread和Registers标志位
    EAPMThreadTraceReportAll = 0xFFU
} EAPMThreadTraceReportMask;

@interface EAPMThreadTracer : NSObject

+ (NSString *)generateTraceReport:(EAPMThreadTraceReportMask)reportMask;

/**
 * 传入指定的thread，dump指定线程的堆栈数据，如果要获取当前线程，内部会调用 generateTraceReportForCurrentThread 方法
 */
+ (NSString *)generateTraceReportWithThread:(thread_t)thread;

+ (NSString *)generateMainThreadTraceReportWithTrace:(NSArray<NSNumber *> *)trace
                                        customReason:(NSString *)customReason
                                          customInfo:(NSString *)customInfo;

+ (NSString *)getThreadNameWithThread:(thread_t)thread;
+ (NSArray *)getThreadNameListForAllThreads;

/**
 * dump当前线程的堆栈数据
 */
+ (NSString *)generateTraceReportForCurrentThread;

/// dump当前线程的堆栈数据
/// @param skipCout 过滤前几行堆栈
+ (NSString *)generateTraceReportForCurrentThreadWithSkip:(NSUInteger)skipCout;

/**
 * needSymbolicated 标识是否需要开启本地符号化
 */
+ (NSString *)generateTraceReport:(EAPMThreadTraceReportMask)reportMask needSymbolicated:(BOOL)needSymbolicated;

/**
 * 很多应用真实的version不等于bundle version，这个时候需要应用自己传入version
 */
+ (NSString *)generateTraceReportWithAppVersion:(NSString *)appVersion ReportMask:(EAPMThreadTraceReportMask)reportMask;

/**
 * 获取主线程的堆栈，单纯的堆栈
 */
+ (NSString *)getMainStackTrace;

/**
 * 获取crash时内存信息
 */
+ (NSString *)getCurrentMemoryInfo;

/**
 * 从线程场景中生成可上传的报告，reason可用来区分报告
 */
+ (NSString *)generateTraceReportForThreadScenes:(NSArray *)scenes needSymbolicated:(BOOL)sym reason:(NSString *)reason;

@end
