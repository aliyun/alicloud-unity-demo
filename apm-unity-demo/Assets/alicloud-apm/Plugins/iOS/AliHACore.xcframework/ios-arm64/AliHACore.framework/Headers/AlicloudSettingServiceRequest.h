//
//  AlicloudSettingServiceRequest.h
//  AlicloudSettingService
//
//  Created by sky on 2021/6/15.
//  Copyright © 2021 aliyun. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface AlicloudSettingServiceRequest : NSObject

+ (void)requestWithURLPrefix:(NSString *)urlPrefix params:(NSDictionary *)params completionBlock:(void (^)(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error))block;

@end

NS_ASSUME_NONNULL_END
