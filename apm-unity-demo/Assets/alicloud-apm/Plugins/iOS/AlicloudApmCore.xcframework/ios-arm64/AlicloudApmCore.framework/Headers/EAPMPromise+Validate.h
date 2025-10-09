#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(ValidateAdditions)

typedef BOOL (^EAPMPromiseValidateWorkBlock)(Value __nullable value) NS_SWIFT_UNAVAILABLE("");

/**
 Validates a fulfilled value or rejects the value if it can not be validated.

 @param predicate An expression to validate.
 @return A new pending promise that gets either resolved with same resolution as the receiver or
         rejected with `EAPMPromiseErrorCodeValidationFailure` error code in `EAPMPromiseErrorDomain`.
 */
- (EAPMPromise *)validate:(EAPMPromiseValidateWorkBlock)predicate NS_SWIFT_UNAVAILABLE("");

/**
 Validates a fulfilled value or rejects the value if it can not be validated.

 @param queue A queue to dispatch on.
 @param predicate An expression to validate.
 @return A new pending promise that gets either resolved with same resolution as the receiver or
         rejected with `EAPMPromiseErrorCodeValidationFailure` error code in `EAPMPromiseErrorDomain`.
 */
- (EAPMPromise *)onQueue:(dispatch_queue_t)queue validate:(EAPMPromiseValidateWorkBlock)predicate NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `validate` operators.
 Usage: promise.validate(^BOOL(id value) { ... })
 */
@interface EAPMPromise <Value>(DotSyntax_ValidateAdditions)

- (EAPMPromise * (^)(EAPMPromiseValidateWorkBlock))validate EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
- (EAPMPromise * (^)(dispatch_queue_t, EAPMPromiseValidateWorkBlock))validateOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
