#import "EAPMPromiseError.h"

NS_ASSUME_NONNULL_BEGIN

/**
 Promises synchronization construct in Objective-C.
 */
@interface EAPMPromise<__covariant Value> : NSObject

/**
 Default dispatch queue used for `EAPMPromise`, which is `main` if a queue is not specified.
 */
@property (class) dispatch_queue_t defaultDispatchQueue NS_REFINED_FOR_SWIFT;

/**
 Creates a pending promise.
 */
+ (instancetype)pendingPromise NS_REFINED_FOR_SWIFT;

/**
 Creates a resolved promise.

 @param resolution An object to resolve the promise with: either a value or an error.
 @return A new resolved promise.
 */
+ (instancetype)resolvedWith:(nullable id)resolution NS_REFINED_FOR_SWIFT;

/**
 Synchronously fulfills the promise with a value.

 @param value An arbitrary value to fulfill the promise with, including `nil`.
 */
- (void)fulfill:(nullable Value)value NS_REFINED_FOR_SWIFT;

/**
 Synchronously rejects the promise with an error.

 @param error An error to reject the promise with.
 */
- (void)reject:(NSError *)error NS_REFINED_FOR_SWIFT;

+ (instancetype)new NS_UNAVAILABLE;
- (instancetype)init NS_UNAVAILABLE;
@end

@interface EAPMPromise <Value>()

/**
 Adds an object to the set of pending objects to keep strongly while the promise is pending.
 Used by the Swift wrappers to keep them alive until the underlying ObjC promise is resolved.

 @param object An object to add.
 */
- (void)addPendingObject:(id)object NS_REFINED_FOR_SWIFT;

@end

#ifdef EAPM_PROMISES_DOT_SYNTAX_IS_DEPRECATED
#define EAPM_PROMISES_DOT_SYNTAX __attribute__((deprecated))
#else
#define EAPM_PROMISES_DOT_SYNTAX
#endif

@interface EAPMPromise <Value>(DotSyntaxAdditions)

/**
 Convenience dot-syntax wrappers for EAPMPromise.
 Usage: EAPMPromise.pending()
        EAPMPromise.resolved(value)

 */
+ (EAPMPromise * (^)(void))pending EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(id __nullable))resolved EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
