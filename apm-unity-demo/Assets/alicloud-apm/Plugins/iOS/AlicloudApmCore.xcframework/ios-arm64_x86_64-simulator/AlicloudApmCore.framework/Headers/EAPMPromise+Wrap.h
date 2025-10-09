#import "EAPMPromise.h"

NS_ASSUME_NONNULL_BEGIN

/**
 Different types of completion handlers available to be wrapped with promise.
 */
typedef void (^EAPMPromiseCompletion)(void) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseObjectCompletion)(id __nullable) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseErrorCompletion)(NSError *__nullable) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseObjectOrErrorCompletion)(id __nullable, NSError *__nullable) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseErrorOrObjectCompletion)(NSError *__nullable, id __nullable) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromise2ObjectsOrErrorCompletion)(id __nullable, id __nullable, NSError *__nullable) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseBoolCompletion)(BOOL) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseBoolOrErrorCompletion)(BOOL, NSError *__nullable) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseIntegerCompletion)(NSInteger) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseIntegerOrErrorCompletion)(NSInteger, NSError *__nullable) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseDoubleCompletion)(double) NS_SWIFT_UNAVAILABLE("");
typedef void (^EAPMPromiseDoubleOrErrorCompletion)(double, NSError *__nullable) NS_SWIFT_UNAVAILABLE("");

/**
 Provides an easy way to convert methods that use common callback patterns into promises.
 */
@interface EAPMPromise <Value>(WrapAdditions)

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with `nil` when completion handler is invoked.
 */
+ (instancetype)wrapCompletion:(void (^)(EAPMPromiseCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with `nil` when completion handler is invoked.
 */
+ (instancetype)onQueue:(dispatch_queue_t)queue
         wrapCompletion:(void (^)(EAPMPromiseCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an object provided by completion handler.
 */
+ (instancetype)wrapObjectCompletion:(void (^)(EAPMPromiseObjectCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an object provided by completion handler.
 */
+ (instancetype)onQueue:(dispatch_queue_t)queue
   wrapObjectCompletion:(void (^)(EAPMPromiseObjectCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an error provided by completion handler.
 If error is `nil`, fulfills with `nil`, otherwise rejects with the error.
 */
+ (instancetype)wrapErrorCompletion:(void (^)(EAPMPromiseErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an error provided by completion handler.
 If error is `nil`, fulfills with `nil`, otherwise rejects with the error.
 */
+ (instancetype)onQueue:(dispatch_queue_t)queue
    wrapErrorCompletion:(void (^)(EAPMPromiseErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an object provided by completion handler if error is `nil`.
 Otherwise, rejects with the error.
 */
+ (instancetype)wrapObjectOrErrorCompletion:(void (^)(EAPMPromiseObjectOrErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an object provided by completion handler if error is `nil`.
 Otherwise, rejects with the error.
 */
+ (instancetype)onQueue:(dispatch_queue_t)queue
wrapObjectOrErrorCompletion:(void (^)(EAPMPromiseObjectOrErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an error or object provided by completion handler. If error
 is not `nil`, rejects with the error.
 */
+ (instancetype)wrapErrorOrObjectCompletion:(void (^)(EAPMPromiseErrorOrObjectCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an error or object provided by completion handler. If error
 is not `nil`, rejects with the error.
 */
+ (instancetype)onQueue:(dispatch_queue_t)queue
wrapErrorOrObjectCompletion:(void (^)(EAPMPromiseErrorOrObjectCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an array of objects provided by completion handler in order
 if error is `nil`. Otherwise, rejects with the error.
 */
+ (EAPMPromise<NSArray *> *)wrap2ObjectsOrErrorCompletion:(void (^)(EAPMPromise2ObjectsOrErrorCompletion handler))work
NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an array of objects provided by completion handler in order
 if error is `nil`. Otherwise, rejects with the error.
 */
+ (EAPMPromise<NSArray *> *)onQueue:(dispatch_queue_t)queue
      wrap2ObjectsOrErrorCompletion:(void (^)(EAPMPromise2ObjectsOrErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping YES/NO.
 */
+ (EAPMPromise<NSNumber *> *)wrapBoolCompletion:(void (^)(EAPMPromiseBoolCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping YES/NO.
 */
+ (EAPMPromise<NSNumber *> *)onQueue:(dispatch_queue_t)queue
                  wrapBoolCompletion:(void (^)(EAPMPromiseBoolCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping YES/NO when error is `nil`.
 Otherwise rejects with the error.
 */
+ (EAPMPromise<NSNumber *> *)wrapBoolOrErrorCompletion:(void (^)(EAPMPromiseBoolOrErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping YES/NO when error is `nil`.
 Otherwise rejects with the error.
 */
+ (EAPMPromise<NSNumber *> *)onQueue:(dispatch_queue_t)queue
           wrapBoolOrErrorCompletion:(void (^)(EAPMPromiseBoolOrErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping an integer.
 */
+ (EAPMPromise<NSNumber *> *)wrapIntegerCompletion:(void (^)(EAPMPromiseIntegerCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping an integer.
 */
+ (EAPMPromise<NSNumber *> *)onQueue:(dispatch_queue_t)queue
               wrapIntegerCompletion:(void (^)(EAPMPromiseIntegerCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping an integer when error is `nil`.
 Otherwise rejects with the error.
 */
+ (EAPMPromise<NSNumber *> *)wrapIntegerOrErrorCompletion:(void (^)(EAPMPromiseIntegerOrErrorCompletion handler))work
NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping an integer when error is `nil`.
 Otherwise rejects with the error.
 */
+ (EAPMPromise<NSNumber *> *)onQueue:(dispatch_queue_t)queue
        wrapIntegerOrErrorCompletion:(void (^)(EAPMPromiseIntegerOrErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping a double.
 */
+ (EAPMPromise<NSNumber *> *)wrapDoubleCompletion:(void (^)(EAPMPromiseDoubleCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping a double.
 */
+ (EAPMPromise<NSNumber *> *)onQueue:(dispatch_queue_t)queue
                wrapDoubleCompletion:(void (^)(EAPMPromiseDoubleCompletion handler))work NS_SWIFT_UNAVAILABLE("");

/**
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping a double when error is `nil`.
 Otherwise rejects with the error.
 */
+ (EAPMPromise<NSNumber *> *)wrapDoubleOrErrorCompletion:(void (^)(EAPMPromiseDoubleOrErrorCompletion handler))work
NS_SWIFT_UNAVAILABLE("");

/**
 @param queue A queue to invoke the `work` block on.
 @param work A block to perform any operations needed to resolve the promise.
 @returns A promise that resolves with an `NSNumber` wrapping a double when error is `nil`.
 Otherwise rejects with the error.
 */
+ (EAPMPromise<NSNumber *> *)onQueue:(dispatch_queue_t)queue
         wrapDoubleOrErrorCompletion:(void (^)(EAPMPromiseDoubleOrErrorCompletion handler))work NS_SWIFT_UNAVAILABLE("");

@end

/**
 Convenience dot-syntax wrappers for `EAPMPromise` `wrap` operators.
 Usage: EAPMPromise.wrapCompletion(^(EAPMPromiseCompletion handler) {...})
 */
@interface EAPMPromise <Value>(DotSyntax_WrapAdditions)

+ (EAPMPromise * (^)(void (^)(EAPMPromiseCompletion)))wrapCompletion EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(dispatch_queue_t, void (^)(EAPMPromiseCompletion)))wrapCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(void (^)(EAPMPromiseObjectCompletion)))wrapObjectCompletion EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(dispatch_queue_t, void (^)(EAPMPromiseObjectCompletion)))wrapObjectCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(void (^)(EAPMPromiseErrorCompletion)))wrapErrorCompletion EAPM_PROMISES_DOT_SYNTAX NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(dispatch_queue_t, void (^)(EAPMPromiseErrorCompletion)))wrapErrorCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(void (^)(EAPMPromiseObjectOrErrorCompletion)))wrapObjectOrErrorCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(dispatch_queue_t, void (^)(EAPMPromiseObjectOrErrorCompletion)))wrapObjectOrErrorCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(void (^)(EAPMPromiseErrorOrObjectCompletion)))wrapErrorOrObjectCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise * (^)(dispatch_queue_t, void (^)(EAPMPromiseErrorOrObjectCompletion)))wrapErrorOrObjectCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSArray *> * (^)(void (^)(EAPMPromise2ObjectsOrErrorCompletion)))wrap2ObjectsOrErrorCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSArray *> * (^)(dispatch_queue_t, void (^)(EAPMPromise2ObjectsOrErrorCompletion)))wrap2ObjectsOrErrorCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(void (^)(EAPMPromiseBoolCompletion)))wrapBoolCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(dispatch_queue_t, void (^)(EAPMPromiseBoolCompletion)))wrapBoolCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(void (^)(EAPMPromiseBoolOrErrorCompletion)))wrapBoolOrErrorCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(dispatch_queue_t, void (^)(EAPMPromiseBoolOrErrorCompletion)))wrapBoolOrErrorCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(void (^)(EAPMPromiseIntegerCompletion)))wrapIntegerCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(dispatch_queue_t, void (^)(EAPMPromiseIntegerCompletion)))wrapIntegerCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(void (^)(EAPMPromiseIntegerOrErrorCompletion)))wrapIntegerOrErrorCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(dispatch_queue_t, void (^)(EAPMPromiseIntegerOrErrorCompletion)))wrapIntegerOrErrorCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(void (^)(EAPMPromiseDoubleCompletion)))wrapDoubleCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(dispatch_queue_t, void (^)(EAPMPromiseDoubleCompletion)))wrapDoubleCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(void (^)(EAPMPromiseDoubleOrErrorCompletion)))wrapDoubleOrErrorCompletion EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");
+ (EAPMPromise<NSNumber *> * (^)(dispatch_queue_t, void (^)(EAPMPromiseDoubleOrErrorCompletion)))wrapDoubleOrErrorCompletionOn EAPM_PROMISES_DOT_SYNTAX
NS_SWIFT_UNAVAILABLE("");

@end

NS_ASSUME_NONNULL_END
