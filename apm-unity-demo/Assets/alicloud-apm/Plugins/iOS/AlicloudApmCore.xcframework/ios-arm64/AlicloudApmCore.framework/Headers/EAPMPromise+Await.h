#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

/**
 Waits for promise resolution. The current thread blocks until the promise is resolved.

 @param promise Promise to wait for.
 @param error Error the promise was rejected with, or `nil` if the promise was fulfilled.
 @return Value the promise was fulfilled with. If the promise was rejected, the return value
         is always `nil`, but the error out arg is not.
 */
FOUNDATION_EXTERN id __nullable EAPMPromiseAwait(EAPMPromise *promise, NSError **error) NS_REFINED_FOR_SWIFT;

NS_ASSUME_NONNULL_END
