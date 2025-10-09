#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(ReduceAdditions)

typedef id __nullable (^EAPMPromiseReducerBlock)(Value __nullable partial, id next) NS_SWIFT_UNAVAILABLE("");

/**
 Sequentially reduces a collection of values to a single promise using a given combining block
 and the value `self` resolves with as initial value.

 @param items An array of values to process in order.
 @param reducer A block to combine an accumulating value and an element of the sequence into
                the new accumulating value or a promise resolved with it, to be used in the next
                call of the `reducer` or returned to the caller.
 @return A new pending promise returned from the last `reducer` invocation.
         Or `self` if `items` is empty.
 */
- (EAPMPromise *)reduce:(NSArray *)items combine:(EAPMPromiseReducerBlock)reducer NS_SWIFT_UNAVAILABLE("");

/**
 Sequentially reduces a collection of values to a single promise using a given combining block
 and the value `self` resolves with as initial value.

 @param queue A queue to dispatch on.
 @param items An array of values to process in order.
 @param reducer A block to combine an accumulating value and an element of the sequence into
                the new accumulating value or a promise resolved with it, to be used in the next
                call of the `reducer` or returned to the caller.
 @return A new pending promise returned from the last `reducer` invocation.
         Or `self` if `items` is empty.
 */
- (EAPMPromise *)onQueue:(dispatch_queue_t)queue
                  reduce:(NSArray *)items
                 combine:(EAPMPromiseReducerBlock)reducer NS_SWIFT_UNAVAILABLE("");

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `reduce` operators.
 Usage: promise.reduce(values, ^id(id partial, id next) { ... })
 */
@interface EAPMPromise <Value>(DotSyntax_ReduceAdditions)

- (EAPMPromise * (^)(NSArray *, EAPMPromiseReducerBlock))reduce EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
- (EAPMPromise * (^)(dispatch_queue_t, NSArray *, EAPMPromiseReducerBlock))reduceOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
