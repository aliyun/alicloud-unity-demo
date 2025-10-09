#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

FOUNDATION_EXTERN NSErrorDomain const EAPMPromiseErrorDomain NS_REFINED_FOR_SWIFT;

/**
 Possible error codes in `EAPMPromiseErrorDomain`.
 */
typedef NS_ENUM(NSInteger, EAPMPromiseErrorCode) {
    /** Promise failed to resolve in time. */
    EAPMPromiseErrorCodeTimedOut = 1,
    /** Validation predicate returned false. */
    EAPMPromiseErrorCodeValidationFailure = 2,
} NS_REFINED_FOR_SWIFT;

NS_INLINE BOOL EAPMPromiseErrorIsTimedOut(NSError *error) NS_SWIFT_UNAVAILABLE("") {
    return error.domain == EAPMPromiseErrorDomain && error.code == EAPMPromiseErrorCodeTimedOut;
}

NS_INLINE BOOL EAPMPromiseErrorIsValidationFailure(NSError *error) NS_SWIFT_UNAVAILABLE("") {
    return error.domain == EAPMPromiseErrorDomain && error.code == EAPMPromiseErrorCodeValidationFailure;
}

NS_ASSUME_NONNULL_END
