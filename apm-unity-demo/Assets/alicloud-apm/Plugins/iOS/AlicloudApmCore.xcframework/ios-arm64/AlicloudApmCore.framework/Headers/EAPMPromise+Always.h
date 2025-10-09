#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(AlwaysAdditions)

typedef void (^EAPMPromiseAlwaysWorkBlock)(void) NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block that always executes, no matter if the receiver is rejected or fulfilled.
 @return A new pending promise to be resolved with same resolution as the receiver.
 */
- (EAPMPromise *)always:(EAPMPromiseAlwaysWorkBlock)work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to dispatch on.
 @param work A block that always executes, no matter if the receiver is rejected or fulfilled.
 @return A new pending promise to be resolved with same resolution as the receiver.
 */
- (EAPMPromise *)onQueue:(dispatch_queue_t)queue always:(EAPMPromiseAlwaysWorkBlock)work NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `always` operators.
 Usage: promise.always(^{...})
 */
@interface EAPMPromise <Value>(DotSyntax_AlwaysAdditions)

- (EAPMPromise * (^)(EAPMPromiseAlwaysWorkBlock))always EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
- (EAPMPromise * (^)(dispatch_queue_t, EAPMPromiseAlwaysWorkBlock))alwaysOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
