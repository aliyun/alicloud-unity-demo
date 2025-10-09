#ifndef EAPMTraceProtocol_h
#define EAPMTraceProtocol_h

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#pragma app lifecycle call back protocol
@protocol EAPMTraceAppLifeProtocol <NSObject>

@required

/**
 * application enter foreground callback
 */
- (void)onApplicationEnterForeground;

/**
 * application enter background callback
 */
- (void)onApplicationEnterBackground;

/**
 * application become active callback
 */
- (void)onApplicationBecomeActive;

/**
 * application resign active callback
 */
- (void)onApplicationResignActive;

@optional

/**
 * open application from external url callback
 */
- (void)onApplicationOpenFromURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication;

/**
 * application receive memory warning
 */
- (void)onReceiveMemoryWarning;

/**
 * application terminate callback
 */
- (void)onApplicationTerminate;

@end

/**
 * page change type
 */
typedef NS_ENUM(NSInteger, PageChangeType) {
    PageChangeTypePush = 0,
    PageChangeTypePop,
    PageChangeTypeTab,
};

#pragma Trace VC lifecycle protocol
@protocol EAPMTraceVCLifeProtocol <NSObject>

@required

/**
 * page change callback
 */
- (void)onPageChange:(PageChangeType)pageChangeType
              fromVC:(UIViewController *)fromVC
                toVC:(UIViewController *)toVC
                args:(NSDictionary *)args;

/**
 * viewDidAppear callback
 */
- (void)onViewDidAppear:(BOOL)animated viewController:(UIViewController *)viewController;

/**
 * viewDidLayoutSubviews callback
 */
- (void)onViewDidLayoutSubviews:(UIViewController *)viewController;

/**
 * viewDidDisappear callback
 */
- (void)onViewDidDisappear:(BOOL)animated viewController:(UIViewController *)viewController;

@optional

- (void)onViewDidLoad:(UIViewController *)viewController;
- (void)onViewWillExit:(UIViewController *)viewController;
- (void)onViewWillAppear:(BOOL)animated viewController:(UIViewController *)viewController;
- (void)onViewWillDisappear:(BOOL)animated viewController:(UIViewController *)viewController;

/**
 * NavigationDidEndTransitionFromView callback
 */
- (void)onNavigationDidEndTransitionFromView:(UIView *)view toView:(UIView *)toView;

/**
 * container view(UITableViewCellContentView, UITableViewCell, UITableView) layout subview callback
 */
- (void)onUIViewLayoutSubviews;

@end

/**
 * user event protocol
 */
@protocol EAPMTraceUserEventProtocol <NSObject>

/**
 * on general user event
 */
- (void)onUserEvent;

/**
 * on user tap event
 */
- (void)onUserTap;

/**
 * on user swipe event
 */
- (void)onUserSwipe;

@end

#pragma runtime task protocol
@protocol EAPMTraceRuntimeTaskProtocol <NSObject>

/**
 * on customized task begin
 */
- (void)onTaskBegin:(NSString *)taskName thread:(NSString *)thread;

/**
 * on customized task end
 */
- (void)onTaskEnd:(NSString *)taskName;

@end

#pragma Trace util protocol
@protocol EAPMTracePageResolverProtocol <NSObject>

/**
 * 获取VC真实的地址，webview -> url, weex - > url
 */
- (NSString *)getRealPageNameByVC:(UIViewController *)toVC;

/**
 * get page params from vc, such as product id, shop id .etc
 * @return page params kv pair
 */
- (NSDictionary *)getPageParamsByVC:(UIViewController *)toVC;

/**
 * 排除UITabBarController、UINavigationController等类型
 */
- (BOOL)isVaildViewController:(UIViewController *)viewController;

/**
 * 排除UITabBarController、UINavigationController等类型
 */
- (UIViewController *)getRealUIViewController:(UIViewController *)viewController;

@end

#endif /* EAPMTraceProtocol_h */
