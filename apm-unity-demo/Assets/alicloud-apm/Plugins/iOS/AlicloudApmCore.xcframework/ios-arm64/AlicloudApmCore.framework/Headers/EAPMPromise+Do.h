#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(DoAdditions)

typedef id __nullable (^EAPMPromiseDoWorkBlock)(void) NS_SWIFT_UNAVAILABLE("");

/**
 Creates a pending promise and executes `work` block asynchronously.

 @param work A block that returns a value or an error used to resolve the promise.
 @return A new pending promise.
 */
+ (instancetype)do:(EAPMPromiseDoWorkBlock)work NS_SWIFT_UNAVAILABLE("");

/**
 Creates a pending promise and executes `work` block asynchronously on the given queue.

 @param queue A queue to invoke the `work` block on.
 @param work A block that returns a value or an error used to resolve the promise.
 @return A new pending promise.
 */
+ (instancetype)onQueue:(dispatch_queue_t)queue do:(EAPMPromiseDoWorkBlock)work NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `do` operators.
 Usage: EAPMPromise.doOn(queue, ^(NSError *error) { ... })
 */
@interface EAPMPromise <Value>(DotSyntax_DoAdditions)

+ (EAPMPromise * (^)(dispatch_queue_t, EAPMPromiseDoWorkBlock))doOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
