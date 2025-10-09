#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(ThenAdditions)

typedef id __nullable (^EAPMPromiseThenWorkBlock)(Value __nullable value) NS_SWIFT_UNAVAILABLE("");

/**
 Creates a pending promise which eventually gets resolved with resolution returned from `work`
 block: either value, error or another promise. The `work` block is executed asynchronously only
 when the receiver is fulfilled. If receiver is rejected, the returned promise is also rejected with
 the same error.

 @param work A block to handle the value that receiver was fulfilled with.
 @return A new pending promise to be resolved with resolution returned from the `work` block.
 */
- (EAPMPromise *)then:(EAPMPromiseThenWorkBlock)work NS_SWIFT_UNAVAILABLE("");

/**
 Creates a pending promise which eventually gets resolved with resolution returned from `work`
 block: either value, error or another promise. The `work` block is executed asynchronously when the
 receiver is fulfilled. If receiver is rejected, the returned promise is also rejected with the same
 error.

 @param queue A queue to invoke the `work` block on.
 @param work A block to handle the value that receiver was fulfilled with.
 @return A new pending promise to be resolved with resolution returned from the `work` block.
 */
- (EAPMPromise *)onQueue:(dispatch_queue_t)queue then:(EAPMPromiseThenWorkBlock)work NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `then` operators.
 Usage: promise.then(^id(id value) { ... })
 */
@interface EAPMPromise <Value>(DotSyntax_ThenAdditions)

- (EAPMPromise * (^)(EAPMPromiseThenWorkBlock))then EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
- (EAPMPromise * (^)(dispatch_queue_t, EAPMPromiseThenWorkBlock))thenOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
