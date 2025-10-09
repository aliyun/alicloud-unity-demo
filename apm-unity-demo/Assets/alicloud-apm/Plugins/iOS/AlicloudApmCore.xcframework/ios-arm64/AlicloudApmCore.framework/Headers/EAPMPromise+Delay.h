#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(DelayAdditions)

/**
 Creates a new pending promise that fulfills with the same value as `self` after the `delay`, or
 rejects with the same error immediately.

 @param interval Time to wait in seconds.
 @return A new pending promise that fulfills at least `delay` seconds later than `self`, or rejects
         with the same error immediately.
 */
- (EAPMPromise *)delay:(NSTimeInterval)interval NS_SWIFT_UNAVAILABLE("");

/**
 Creates a new pending promise that fulfills with the same value as `self` after the `delay`, or
 rejects with the same error immediately.

 @param queue A queue to dispatch on.
 @param interval Time to wait in seconds.
 @return A new pending promise that fulfills at least `delay` seconds later than `self`, or rejects
         with the same error immediately.
 */
- (EAPMPromise *)onQueue:(dispatch_queue_t)queue delay:(NSTimeInterval)interval NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `delay` operators.
 Usage: promise.delay(...)
 */
@interface EAPMPromise <Value>(DotSyntax_DelayAdditions)

- (EAPMPromise * (^)(NSTimeInterval))delay EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
- (EAPMPromise * (^)(dispatch_queue_t, NSTimeInterval))delayOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
