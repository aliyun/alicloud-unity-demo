#import <Foundation/Foundation.h>
#import "EAPMShareBinaryImages.h"
#import "EAPMShareMemAllocStack.h"
#import "EAPMShareRuntime.h"
#import "EAPMShareCustomKeyValue.h"
#import "EAPMShareSymbolResolver.h"

NS_ASSUME_NONNULL_BEGIN

@interface EAPMShareManager : NSObject

@property (nonatomic, strong) id<EAPMShareBinaryImages> binaryImages;
@property (nonatomic, strong) id<EAPMShareMemAllocStack> memAllocStack;
@property (nonatomic, strong) id<EAPMShareRuntime> runtime;
@property (nonatomic, strong) id<EAPMShareCustomKeyValue> customKeyValue;
@property (nonatomic, strong) id<EAPMShareSymbolResolver> symbolResolver;

+ (nonnull instancetype)sharedInstance;

@end

NS_ASSUME_NONNULL_END
