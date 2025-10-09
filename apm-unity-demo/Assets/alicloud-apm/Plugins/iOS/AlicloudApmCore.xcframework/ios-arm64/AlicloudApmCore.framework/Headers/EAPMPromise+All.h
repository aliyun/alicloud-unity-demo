#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(AllAdditions)

/**
 Wait until all of the given promises are fulfilled.
 If one of the given promises is rejected, then the returned promise is rejected with same error.
 If any other arbitrary value or `NSError` appears in the array instead of `EAPMPromise`,
 it's implicitly considered a pre-fulfilled or pre-rejected `EAPMPromise` correspondingly.
 Promises resolved with `nil` become `NSNull` instances in the resulting array.

 @param promises Promises to wait for.
 @return Promise of an array containing the values of input promises in the same order.
 */
+ (EAPMPromise<NSArray *> *)all:(NSArray *)promises NS_SWIFT_UNAVAILABLE("");

/**
 Wait until all of the given promises are fulfilled.
 If one of the given promises is rejected, then the returned promise is rejected with same error.
 If any other arbitrary value or `NSError` appears in the array instead of `EAPMPromise`,
 it's implicitly considered a pre-fulfilled or pre-rejected EAPMPromise correspondingly.
 Promises resolved with `nil` become `NSNull` instances in the resulting array.

 @param queue A queue to dispatch on.
 @param promises Promises to wait for.
 @return Promise of an array containing the values of input promises in the same order.
 */
+ (EAPMPromise<NSArray *> *)onQueue:(dispatch_queue_t)queue all:(NSArray *)promises NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `all` operators.
 Usage: EAPMPromise.all(@[ ... ])
 */
@interface EAPMPromise <Value>(DotSyntax_AllAdditions)

+ (EAPMPromise<NSArray *> * (^)(NSArray *))all EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSArray *> * (^)(dispatch_queue_t, NSArray *))allOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
