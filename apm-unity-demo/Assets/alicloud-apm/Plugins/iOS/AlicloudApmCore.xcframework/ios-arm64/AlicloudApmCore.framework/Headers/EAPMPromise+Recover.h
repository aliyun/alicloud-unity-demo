#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(RecoverAdditions)

typedef id __nullable (^EAPMPromiseRecoverWorkBlock)(NSError *error) NS_SWIFT_UNAVAILABLE("");

/**
 Provides a new promise to recover in case the receiver gets rejected.

 @param recovery A block to handle the error that the receiver was rejected with.
 @return A new pending promise to use instead of the rejected one that gets resolved with resolution
         returned from `recovery` block.
 */
- (EAPMPromise *)recover:(EAPMPromiseRecoverWorkBlock)recovery NS_SWIFT_UNAVAILABLE("");

/**
 Provides a new promise to recover in case the receiver gets rejected.

 @param queue A queue to dispatch on.
 @param recovery A block to handle the error that the receiver was rejected with.
 @return A new pending promise to use instead of the rejected one that gets resolved with resolution
         returned from `recovery` block.
 */
- (EAPMPromise *)onQueue:(dispatch_queue_t)queue recover:(EAPMPromiseRecoverWorkBlock)recovery NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `recover` operators.
 Usage: promise.recover(^id(NSError *error) {...})
 */
@interface EAPMPromise <Value>(DotSyntax_RecoverAdditions)

- (EAPMPromise * (^)(EAPMPromiseRecoverWorkBlock))recover EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
- (EAPMPromise * (^)(dispatch_queue_t, EAPMPromiseRecoverWorkBlock))recoverOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
