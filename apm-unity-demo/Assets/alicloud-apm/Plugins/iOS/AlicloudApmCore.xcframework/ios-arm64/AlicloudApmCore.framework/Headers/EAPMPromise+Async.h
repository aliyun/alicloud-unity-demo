#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(AsyncAdditions)

typedef void (^EAPMPromiseFulfillBlock)(Value __nullable value) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseRejectBlock)(NSError *error) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseAsyncWorkBlock)(EAPMPromiseFulfillBlock fulfill, EAPMPromiseRejectBlock reject) NS_SWIFT_UNAVAILABLE("");

/**
 Creates a pending promise and executes `work` block asynchronously.

 @param work A block to perform any operations needed to resolve the promise.
 @return A new pending promise.
 */
+ (instancetype)async:(EAPMPromiseAsyncWorkBlock)work NS_SWIFT_UNAVAILABLE("");

/**
 Creates a pending promise and executes `work` block asynchronously on the given queue.

 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @return A new pending promise.
 */
+ (instancetype)onQueue:(dispatch_queue_t)queue async:(EAPMPromiseAsyncWorkBlock)work NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `async` operators.
 Usage: EAPMPromise.async(^(EAPMPromiseFulfillBlock fulfill, EAPMPromiseRejectBlock reject) { ... })
 */
@interface EAPMPromise <Value>(DotSyntax_AsyncAdditions)

+ (EAPMPromise * (^)(EAPMPromiseAsyncWorkBlock))async EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(dispatch_queue_t, EAPMPromiseAsyncWorkBlock))asyncOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
