#import "EAPMNetworkListener.h"
#import "EAPMReachability.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMNetworkInfo : NSObject

@property (strong, nonatomic) NSString *carrierInfo;

@property (strong, nonatomic) NSString *networkType;

@property (assign, nonatomic) NetworkStatus networkStatus;

+ (EAPMNetworkInfo *)sharedInstance;

- (void)addListener:(id<EAPMNetworkListener>)listener;

- (void)removeListener:(id<EAPMNetworkListener>)listener;

@end

NS_ASSUME_NONNULL_END
