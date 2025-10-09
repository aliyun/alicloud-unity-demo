#import "EAPMReachability.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMNetworkListener <NSObject>

@optional
- (void)onCarrierDidChange:(NSString *)carrierInfo;
- (void)onNetworkTypeDidChange:(NSString *)networkType;
- (void)onNetworkStatusDidChange:(NetworkStatus)networkStatus;

@end

NS_ASSUME_NONNULL_END
