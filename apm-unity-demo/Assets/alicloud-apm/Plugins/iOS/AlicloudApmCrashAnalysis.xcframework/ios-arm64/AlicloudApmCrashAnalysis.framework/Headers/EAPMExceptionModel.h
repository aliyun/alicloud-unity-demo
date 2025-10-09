#import <Foundation/Foundation.h>

#import "EAPMStackFrame.h"

NS_ASSUME_NONNULL_BEGIN

/**
 * 异常来源的编程语言枚举
 */
typedef NS_ENUM(NSInteger, EAPMSourceLanguage) {
    EAPMSourceLanguageUnknown = 0,
    EAPMSourceLanguageCSharp = 1,
    EAPMSourceLanguageLua = 2,
    EAPMSourceLanguageObjectiveC = 3,
    EAPMSourceLanguageSwift = 4
} NS_SWIFT_NAME(SourceLanguage);

NS_SWIFT_NAME(ExceptionModel)
@interface EAPMExceptionModel : NSObject

/** :nodoc: */
- (instancetype)init NS_UNAVAILABLE;

/**
 * 初始化ExceptionModel实例
 *
 * @param name - 名称，通常为Exception类的类型
 * @param reason - 问题发生的可读原因
 */
- (instancetype)initWithName:(NSString *)name reason:(NSString *)reason;

/**
 * 初始化ExceptionModel实例
 *
 * @param name - 名称，通常为Exception类的类型
 * @param reason - 问题发生的可读原因
 * @param language - 异常来源的编程语言
 * @param custom - 是否为自定义异常
 * @param urgent - 是否为紧急
 * @param quitApp - 是否退出应用
 */
- (instancetype)initWithName:(NSString *)name 
                      reason:(NSString *)reason
                    language:(EAPMSourceLanguage)language
                      custom:(BOOL)custom
                      urgent:(BOOL)urgent
                     quitApp:(BOOL)quitApp;

/**
 * 创建ExceptionModel实例
 *
 * @param name - 名称，通常为Exception类的类型
 * @param reason - 问题发生的可读原因
 */
+ (instancetype)exceptionModelWithName:(NSString *)name reason:(NSString *)reason NS_SWIFT_UNAVAILABLE("");

/**
 * 堆栈列表，从顶部开始倒序
 */
@property (nonatomic, copy) NSArray<EAPMStackFrame *> *stackTrace;

/**
 * 异常来源的编程语言
 */
@property (nonatomic, assign) EAPMSourceLanguage language;

/**
 * 是否为自定义异常
 */
@property (nonatomic, assign) BOOL custom;

/**
 * 是否紧急
 */
@property (nonatomic, assign) BOOL urgent;

/**
 * 是否退出应用
 */
@property (nonatomic, assign) BOOL quitApp;

@end

NS_ASSUME_NONNULL_END
