//
//  PubCenter.h
//  AliHADataHub4iOS
//
//  doc: https://lark.alipay.com/data-driven-all/apm/biz_protocols
//
//  Created by hansong.lhs on 2018/5/9.
//  Copyright © 2018年 alibaba. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface AliHAPubCenter : NSObject

/**
 * 发布业务消息(lock-free)
 * @param bizId 业务标识
 * @param properties 业务属性（flat map）
 * 协议详见：https://lark.alipay.com/data-driven-all/apm/biz_protocols
 */
+ (void)publish:(NSString*)bizId properties:(NSDictionary<NSString*, NSString*>*)properties;

/**
 * 发布AB测试标签(lock-free)
 * @param bizId 业务标识
 * @param properties AB业务属性（flat map）
 * 协议详见：https://lark.alipay.com/data-driven-all/apm/biz_protocols
 */
+ (void)publishABTest:(NSString*)bizId properties:(NSDictionary<NSString*, NSString*>*)properties;

@end
