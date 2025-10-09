#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMPromise <Value>(RaceAdditions)

/**
 Wait until any of the given promises are fulfilled.
 If one of the promises is rejected, then the returned promise is rejected with same error.
 If any other arbitrary value or `NSError` appears in the array instead of `EAPMPromise`,
 it's implicitly considered a pre-fulfilled or pre-rejected `EAPMPromise` correspondingly.

 @param promises Promises to wait for.
 @return A new pending promise to be resolved with the same resolution as the first promise, among
         the given ones, which was resolved.
 */
+ (instancetype)race:(NSArray *)promises NS_SWIFT_UNAVAILABLE("");

/**
 Wait until any of the given promises are fulfilled.
 If one of the promises is rejected, then the returned promise is rejected with same error.
 If any other arbitrary value or `NSError` appears in the array instead of `EAPMPromise`,
 it's implicitly considered a pre-fulfilled or pre-rejected `EAPMPromise` correspondingly.

 @param queue A queue to dispatch on.
 @param promises Promises to wait for.
 @return A new pending promise to be resolved with the same resolution as the first promise, among
         the given ones, which was resolved.
 */
+ (instancetype)onQueue:(dispatch_queue_t)queue race:(NSArray *)promises NS_REFINED_FOR_SWIFT;

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `race` operators.
 Usage: EAPMPromise.race(@[ ... ])
 */
@interface EAPMPromise <Value>(DotSyntax_RaceAdditions)

+ (EAPMPromise * (^)(NSArray *))race EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(dispatch_queue_t, NSArray *))raceOn EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
