#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

NS_SWIFT_NAME(ShareStackFrame)
@interface EAPMShareStackFrame : NSObject

@property (nonatomic, copy, nullable) NSString *symbol;
@property (nonatomic, copy, nullable) NSString *rawSymbol;
@property (nonatomic, copy, nullable) NSString *library;
@property (nonatomic, copy, nullable) NSString *fileName;
@property (nonatomic, assign) uint32_t lineNumber;
@property (nonatomic, assign) uint64_t offset;
@property (nonatomic, assign) uint64_t address;

/** :nodoc: */
- (instancetype)init NS_UNAVAILABLE;

/**
 * 根据地址创建一个符号化的 `ShareStackFrame`，该地址将在崩溃分析平台进行符号化
 * @param address - 异常发生的地址
 */
+ (instancetype)stackFrameWithAddress:(NSUInteger)address;

@end

NS_ASSUME_NONNULL_END
