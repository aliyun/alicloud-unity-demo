#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(CatchAdditions)

typedef void (^EAPMPromiseCatchWorkBlock)(NSError *error) NS_SWIFT_UNAVAILABLE("");

/**
 Creates a pending promise which eventually gets resolved with same resolution as the receiver.
 If receiver is rejected, then `reject` block is executed asynchronously.

 @param reject A block to handle the error that receiver was rejected with.
 @return A new pending promise.
 */
- (EAPMPromise *)catch:(EAPMPromiseCatchWorkBlock)reject NS_SWIFT_UNAVAILABLE("");

/**
 Creates a pending promise which eventually gets resolved with same resolution as the receiver.
 If receiver is rejected, then `reject` block is executed asynchronously on the given queue.

 @param queue A queue to invoke the `reject` block on.
 @param reject A block to handle the error that receiver was rejected with.
 @return A new pending promise.
 */
- (EAPMPromise *)onQueue:(dispatch_queue_t)queue catch:(EAPMPromiseCatchWorkBlock)reject NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `catch` operators.
 Usage: promise.catch(^(NSError *error) { ... })
 */
@interface EAPMPromise <Value>(DotSyntax_CatchAdditions)

- (EAPMPromise * (^)(EAPMPromiseCatchWorkBlock))catch EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
- (EAPMPromise * (^)(dispatch_queue_t, EAPMPromiseCatchWorkBlock))catchOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
