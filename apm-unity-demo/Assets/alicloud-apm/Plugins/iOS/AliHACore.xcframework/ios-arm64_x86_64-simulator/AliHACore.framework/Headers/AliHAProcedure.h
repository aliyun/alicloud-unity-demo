//
//  Procedure.h
//  AliHADataHub4iOS
//
//  Created by hansong.lhs on 2018/5/12.
//  Copyright © 2018年 alibaba. All rights reserved.
//

#import <Foundation/Foundation.h>

#define kProcedureBeginStage            @"procedureStartTime"
#define kProcedureEndStage              @"procedureEndTime"

@interface AliHAProcedure : NSObject

@property (nonatomic, copy) NSString* uuid;             // unique identifier through procedure lifecycle
@property (nonatomic, copy) NSString* topic;            // procedure
@property (nonatomic, assign) uint64_t beginTime;       // start time stamp
@property (nonatomic, assign) uint64_t endTime;         // end time stamp
@property (nonatomic, assign) BOOL isSuccess;
@property (nonatomic, assign) BOOL isFail;

- (instancetype)init:(NSString*)uuid topic:(NSString*)topic properties:(NSDictionary*)properties;

- (void)addHeaders:(NSDictionary<NSString*, NSString*>*)headers;
- (void)addHeader:(NSString*)name value:(NSString*)value;
- (void)addProperties:(NSDictionary<NSString*, NSString*>*)properties;
- (void)addProperty:(NSString*)name value:(NSString*)value;
- (void)setBizProperties:(NSDictionary*)bizProperties;
- (void)addStatisticsValues:(NSDictionary*)statisticsValues;
- (void)addEvent:(NSString*)event timestamp:(uint64_t)timestamp;
- (void)addEvent:(NSString *)event properties:(NSDictionary *)properties timestamp:(uint64_t)timestamp;
- (void)addStage:(NSString*)stage timestamp:(uint64_t)timestamp;

// TODO: handle sub-procedure
- (void)addSubProcedureStage:(NSString*)procedureName stage:(NSString*)stage timestamp:(uint64_t)timestamp;
- (void)addSubProcedureProperties:(NSString*)procedureName properties:(NSDictionary*)properties;

- (NSString*)serialize;

@end
