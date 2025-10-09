//
//  DataHub.h
//  AliHADataHub4iOS
//
//  Created by hansong.lhs on 2018/5/12.
//  Copyright © 2018年 alibaba. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 * subscriber protocol
 */
@protocol AliHADataHubSubscriber <NSObject>

- (void)handleBizData:(NSString*)bizId data:(NSDictionary*)data;
- (void)handleABTestData:(NSString*)bizId data:(NSDictionary*)data;
- (void)handleCurrentBiz:(NSString*)currentBiz;
- (void)onBizDataReadyStage;
- (void)handleStage:(NSString*)bizId stage:(NSString*)stage;
- (void)handleStage:(NSString *)bizId stage:(NSString *)stage timestamp:(NSTimeInterval)timestamp;

@end

@interface AliHADataHub : NSObject

/**
 * 数据处理器初始化；数据处理器只能初始化一次，不能动态添加以保证消息处理的lock-free
 */
+ (void)initWithDataHandler:(id<AliHADataHubSubscriber>)dataHandler;

#pragma biz messages

/**
 * 设置页面的主业务id(一个页面只能设置一次)
 */
+ (void)setCurrentBiz:(NSString*)bizId;
+ (void)setCurrentBiz:(NSString*)bizId subBiz:(NSString*)subBizId;

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


/**
 * 业务数据准备完毕阶段，为了统计真正的数据返回时间点。
 */
+ (void)onBizDataReadyStage;


/**
 * 记录业务阶段信息
 * @param bizId 业务标识
 * @param stage 业务阶段名
 */
+ (void)onStage:(NSString*)bizId stage:(NSString*)stage;


/**
 * 记录业务阶段信息
 * @param bizId 业务标识
 * @param stage 业务阶段名
 */
+ (void)onStage:(NSString*)bizId stage:(NSString*)stage timestamp:(NSTimeInterval)timestamp;

@end
