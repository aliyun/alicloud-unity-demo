#import <Foundation/Foundation.h>

OBJC_EXTERN const NSUInteger EAPMNetworkMaximumRetryCount;

NS_ASSUME_NONNULL_BEGIN

typedef void (^EAPMNetworkDataTaskCompletionHandlerBlock)(NSData *__nullable data, NSURLResponse *__nullable response, NSError *__nullable error);
typedef void (^EAPMNetworkDownloadTaskCompletionHandlerBlock)(NSURL *__nullable location,
                                                              NSURLResponse *__nullable response,
                                                              NSError *__nullable error);

@interface EAPMNetworkClient : NSObject

@property (nonatomic, readonly) NSOperationQueue *operationQueue;

- (instancetype)init;
- (instancetype)initWithQueue:(nullable NSOperationQueue *)operationQueue;
- (instancetype)initWithSessionConfiguration:(NSURLSessionConfiguration *)config
                                       queue:(nullable NSOperationQueue *)operationQueue NS_DESIGNATED_INITIALIZER;

- (void)startDataTaskWithRequest:(NSURLRequest *)request
                      retryLimit:(NSUInteger)retryLimit
               completionHandler:(EAPMNetworkDataTaskCompletionHandlerBlock)completionHandler;
- (void)startDownloadTaskWithRequest:(NSURLRequest *)request
                          retryLimit:(NSUInteger)retryLimit
                   completionHandler:(EAPMNetworkDownloadTaskCompletionHandlerBlock)completionHandler;

- (void)invalidateAndCancel;

- (void)finishTasksAndInvalidate;

@end

NS_ASSUME_NONNULL_END
