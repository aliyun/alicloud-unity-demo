#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 * EAPMTraceLogger 参数模型
 */
@interface EAPMTraceParameters : NSObject

@property (nonatomic, copy, nullable) NSString *data;
@property (nonatomic, copy, nullable) NSString *desc;
@property (nonatomic, strong, nullable) NSArray<NSNumber *> *params;

- (instancetype)initWithData:(nullable NSString *)data;

- (instancetype)initWithData:(nullable NSString *)data desc:(nullable NSString *)desc;

- (instancetype)initWithData:(nullable NSString *)data
                        desc:(nullable NSString *)desc
                      params:(nullable NSArray<NSNumber *> *)params;

@end

NS_ASSUME_NONNULL_END
