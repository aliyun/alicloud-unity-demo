#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(AnyAdditions)

/**
 Waits until all of the given promises are either fulfilled or rejected.
 If all promises are rejected, then the returned promise is rejected with same error
 as the last one rejected.
 If at least one of the promises is fulfilled, the resulting promise is fulfilled with an array of
 values or `NSErrors`, matching the original order of fulfilled or rejected promises respectively.
 If any other arbitrary value or `NSError` appears in the array instead of `EAPMPromise`,
 it's implicitly considered a pre-fulfilled or pre-rejected `EAPMPromise` correspondingly.
 Promises resolved with `nil` become `NSNull` instances in the resulting array.

 @param promises Promises to wait for.
 @return Promise of array containing the values or `NSError`s of input promises in the same order.
 */
+ (EAPMPromise<NSArray *> *)any:(NSArray *)promises NS_SWIFT_UNAVAILABLE("");

/**
 Waits until all of the given promises are either fulfilled or rejected.
 If all promises are rejected, then the returned promise is rejected with same error
 as the last one rejected.
 If at least one of the promises is fulfilled, the resulting promise is fulfilled with an array of
 values or `NSError`s, matching the original order of fulfilled or rejected promises respectively.
 If any other arbitrary value or `NSError` appears in the array instead of `EAPMPromise`,
 it's implicitly considered a pre-fulfilled or pre-rejected `EAPMPromise` correspondingly.
 Promises resolved with `nil` become `NSNull` instances in the resulting array.

 @param queue A queue to dispatch on.
 @param promises Promises to wait for.
 @return Promise of array containing the values or `NSError`s of input promises in the same order.
 */
+ (EAPMPromise<NSArray *> *)onQueue:(dispatch_queue_t)queue any:(NSArray *)promises NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `any` operators.
 Usage: EAPMPromise.any(@[ ... ])
 */
@interface EAPMPromise <Value>(DotSyntax_AnyAdditions)

+ (EAPMPromise<NSArray *> * (^)(NSArray *))any EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSArray *> * (^)(dispatch_queue_t, NSArray *))anyOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
