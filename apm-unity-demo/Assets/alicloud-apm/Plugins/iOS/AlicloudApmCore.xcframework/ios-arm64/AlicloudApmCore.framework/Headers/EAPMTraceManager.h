#import "EAPMTraceLogger.h"
#import "EAPMTraceProtocol.h"
#import <Foundation/Foundation.h>

extern NSString *_Nonnull const kEAPMTraceTypeStartupBegin;

NS_ASSUME_NONNULL_BEGIN

@interface EAPMTraceManager : NSObject <EAPMTraceAppLifeProtocol, EAPMTraceVCLifeProtocol, EAPMTraceRuntimeTaskProtocol>

/**
 * register app lifecycle listener
 */
- (void)registerAppLifeListener:(id<EAPMTraceAppLifeProtocol>)listener;

/**
 * register vc lifecycle listener
 */
- (void)registerVCLifeListener:(id<EAPMTraceVCLifeProtocol>)listener;

/**
 * register user event listener
 */
- (void)registerUserEventListener:(id<EAPMTraceUserEventProtocol>)listener;

/**
 * register customized task listener
 */
- (void)registerCustomizedTaskListener:(id<EAPMTraceRuntimeTaskProtocol>)listener;

/**
 * @return page resolver delegate
 */
- (id<EAPMTracePageResolverProtocol>)getPageResolver;

- (void)onUserEvent:(id)sender;

- (void)onPreLoadView:(UIViewController *)viewController;

- (void)onPostLoadView:(UIViewController *)viewController;

- (void)onPreViewDidLoad:(UIViewController *)viewController;

- (void)onPostViewDidLoad:(UIViewController *)viewController;

- (void)onPreViewWillExit:(UIViewController *)viewController;

- (void)onPostViewWillExit:(UIViewController *)viewController;

- (void)onPreViewWillAppear:(BOOL)animated viewController:(UIViewController *)viewController;

- (void)onPostViewWillAppear:(BOOL)animated viewController:(UIViewController *)viewController;

- (void)onPreViewDidAppear:(BOOL)animated viewController:(UIViewController *)viewController;

- (void)onPostViewDidAppear:(BOOL)animated viewController:(UIViewController *)viewController;

- (void)onPreViewWillDisappear:(BOOL)animated viewController:(UIViewController *)viewController;

- (void)onPostViewWillDisappear:(BOOL)animated viewController:(UIViewController *)viewController;

- (void)onPreViewDidDisappear:(BOOL)animated viewController:(UIViewController *)viewController;

- (void)onPostViewDidDisappear:(BOOL)animated viewController:(UIViewController *)viewController;

- (void)onPreViewWillLayoutSubviews:(UIViewController *)viewController;

- (void)onPostViewWillLayoutSubviews:(UIViewController *)viewController;

- (void)onPreViewDidLayoutSubviews:(UIViewController *)viewController;

- (void)onPostViewDidLayoutSubviews:(UIViewController *)viewController;

@property (nonatomic, strong) id<EAPMTraceLogger> traceLogger;

// app lifecycle listeners
@property (nonatomic, strong) NSMutableArray<id<EAPMTraceAppLifeProtocol>> *appLifeListeners;

// vc lifecycle listeners
@property (nonatomic, strong) NSMutableArray<id<EAPMTraceVCLifeProtocol>> *vcLifeListeners;

// user event callbacks
@property (nonatomic, strong) NSMutableArray<id<EAPMTraceUserEventProtocol>> *userEventListeners;

// customized task callbacks
@property (nonatomic, strong) NSMutableArray<id<EAPMTraceRuntimeTaskProtocol>> *customizedTaskListeners;

// page resolve delegate.
@property (nonatomic, strong) id<EAPMTracePageResolverProtocol> pageResolverDelegate;

@end

NS_ASSUME_NONNULL_END
