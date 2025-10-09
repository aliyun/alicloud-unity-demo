#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

NS_SWIFT_NAME(StackFrame)
@interface EAPMStackFrame : NSObject

/** :nodoc: */
- (instancetype)init NS_UNAVAILABLE;

/**
 * 初始化一个符号化的 `StackFrame`
 *
 * @param symbol - 函数或方法名
 * @param file - 异常发生的文件
 * @param line - 行号
 */
- (instancetype)initWithSymbol:(NSString *)symbol file:(NSString *)file line:(NSInteger)line;

/**
 * 根据地址创建一个符号化的 `StackFrame`，该地址将在崩溃分析平台进行符号化
 * @param address - 异常发生的地址
 */
+ (instancetype)stackFrameWithAddress:(NSUInteger)address;

/**
 * 创建一个符号化的 `StackFrame`
 *
 * @param symbol - 函数或方法名
 * @param file - 异常发生的文件
 * @param line - 行号
 */
+ (instancetype)stackFrameWithSymbol:(NSString *)symbol file:(NSString *)file line:(NSInteger)line NS_SWIFT_UNAVAILABLE("");

/**
 * 设置堆栈帧的地址
 * @param address - 堆栈帧地址
 */
- (void)setAddress:(uint64_t)address;

/**
 * 设置堆栈帧所属的库名称
 * @param library - 库名称
 */
- (void)setLibrary:(nullable NSString *)library;

@end

NS_ASSUME_NONNULL_END
