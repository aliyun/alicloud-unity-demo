//
//  JDYThreadTracer+Backtrace.h
//  Pods
//
//  Created by Jason Lee on 16/6/14.
//
//

#import "JDYThreadTracer.h"
#import <mach/mach.h>

@interface JDYThreadTracer (Backtrace)

/**
 * 回溯指定线程的调用栈
 *
 * @param thread 要进行回溯的线程
 * @return 线程调用栈，可能为nil，每个元素是虚拟地址
 */
+ (NSArray <NSNumber *>*)backtraceThread:(thread_t)thread;

/**
 * 符号化指定的调用栈
 *
 * @param stacks 要进行符号化的调用栈
 * @return 线程调用栈，可能为nil，每个元素是虚拟地址
 */
+ (NSArray <NSString *>*)symbolicateBacktrace:(NSArray <NSNumber *>*)stacks;

+ (NSArray <NSString *>*)symbolicateBacktrace:(NSArray <NSNumber *>*)stacks showAddress:(BOOL)showAddress;


/**
 * 回溯指定线程的调用栈，并符号化
 *
 * @param thread 要进行回溯的线程
 * @return 线程调用栈，可能为nil，每个元素是空格分隔的字符串
 */
+ (NSArray <NSString *>*)backtraceAndSymbolicateThread:(thread_t)thread;

+ (dispatch_queue_t)queueForThread:(thread_t)thread;

+ (NSString*)queueNameForThread:(thread_t)thread;

+ (NSString*)threadNameForThread:(thread_t)thread;

+ (void)disableSuspendMainThread;
+ (void)enableSuspendMainThread;
+ (void)setMainThreadId:(thread_t)mainThreadId;

@end
