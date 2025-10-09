#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(TimeoutAdditions)

/**
 Waits for a promise with the specified `timeout`.

 @param interval Time to wait in seconds.
 @return A new pending promise that gets either resolved with same resolution as the receiver or
         rejected with `EAPMPromiseErrorCodeTimedOut` error code in `EAPMPromiseErrorDomain`.
 */
- (EAPMPromise *)timeout:(NSTimeInterval)interval NS_SWIFT_UNAVAILABLE("");

/**
 Waits for a promise with the specified `timeout`.

 @param queue A queue to dispatch on.
 @param interval Time to wait in seconds.
 @return A new pending promise that gets either resolved with same resolution as the receiver or
         rejected with `EAPMPromiseErrorCodeTimedOut` error code in `EAPMPromiseErrorDomain`.
 */
- (EAPMPromise *)onQueue:(dispatch_queue_t)queue timeout:(NSTimeInterval)interval NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `timeout` operators.
 Usage: promise.timeout(...)
 */
@interface EAPMPromise <Value>(DotSyntax_TimeoutAdditions)

- (EAPMPromise * (^)(NSTimeInterval))timeout EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
- (EAPMPromise * (^)(dispatch_queue_t, NSTimeInterval))timeoutOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
