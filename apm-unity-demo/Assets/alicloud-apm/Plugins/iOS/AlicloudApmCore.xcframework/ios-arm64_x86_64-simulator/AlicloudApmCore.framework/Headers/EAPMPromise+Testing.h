#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

/**
 Waits for all scheduled promises blocks.

 @param timeout Maximum time to wait.
 @return YES if all promises blocks have completed before the timeout and NO otherwise.
 */
FOUNDATION_EXTERN BOOL EAPMWaitForPromisesWithTimeout(NSTimeInterval timeout) NS_REFINED_FOR_SWIFT;

@interface EAPMPromise <Value>(TestingAdditions)

/**
 Dispatch group for promises that is typically used to wait for all scheduled blocks.
 */
@property (class, nonatomic, readonly) dispatch_group_t dispatchGroup NS_REFINED_FOR_SWIFT;

/**
 Properties to get the current state of the promise.
 */
@property (nonatomic, readonly) BOOL isPending NS_REFINED_FOR_SWIFT;
@property (nonatomic, readonly) BOOL isFulfilled NS_REFINED_FOR_SWIFT;
@property (nonatomic, readonly) BOOL isRejected NS_REFINED_FOR_SWIFT;

/**
 Value the promise was fulfilled with.
 Can be nil if the promise is still pending, was resolved with nil or after it has been rejected.
 */
@property (nonatomic, readonly, nullable) Value value NS_REFINED_FOR_SWIFT;

/**
 Error the promise was rejected with.
 Can be nil if the promise is still pending or after it has been fulfilled.
 */
@property (nonatomic, readonly, nullable) NSError *error NS_REFINED_FOR_SWIFT;

@end

NS_ASSUME_NONNULL_END
