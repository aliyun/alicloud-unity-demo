//
//  AliAPMServiceInterface.h
//  AliAPMServiceInterface
//
//  Created by hansong.lhs on 2018/5/15.
//  Copyright © 2018年 alibaba. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 * procedure monitor is meant to track the full trace of a certain procedure
 * (from the begining to the end, including events, stages and failure during the procedure)
 */
@protocol ProcedureTrackInteface <NSObject>

/**
 * data context begin when page exits
 */
- (void)onDataContextBegin;

/**
 * data context finishes when page exits
 */
- (void)onDataContextEnd;

/**
 * @param procedureName the module name, such like ImageLoad, networkRequest
 * @param uuid the unique identifier of a certain procedure(such as a image-load procedure for a single image)
 */
- (void)onProcedureBegin:(NSString*)procedureName properties:(NSDictionary<NSString*, NSString*>*)properties uuid:(NSString*)uuid;
- (void)onProcedureEvent:(NSString*)procedureName event:(NSString*)event uuid:(NSString*)uuid;
- (void)onProcedureStage:(NSString*)procedureName stage:(NSString*)stage uuid:(NSString*)uuid;
- (void)onProcedureFailure:(NSString*)procedureName errorInfo:(NSDictionary<NSString*, NSString*>*)errorInfo uuid:(NSString*)uuid;
- (void)onProcedureSuccess:(NSString*)procedureName properties:(NSDictionary<NSString*, NSString*>*)properties uuid:(NSString*)uuid;
- (void)onProcedurePause:(NSString*)procedureName uuid:(NSString*)uuid;
- (void)onProcedureResume:(NSString*)procedureName uuid:(NSString*)uuid;
- (void)addProcedureProperties:(NSString*)procedureName properties:(NSDictionary<NSString*, NSString*>*)properties uuid:(NSString*)uuid;
- (void)addProcedureStatistic:(NSString*)procedureName properties:(NSDictionary<NSString*, NSString*>*)properties uuid:(NSString*)uuid;

# pragma procedure with timestamp
- (void)onProcedureBegin:(NSString*)procedureName properties:(NSDictionary*)properties uuid:(NSString*)uuid timestamp:(NSTimeInterval)timestamp;
- (void)onProcedureEvent:(NSString*)procedureName event:(NSString*)event uuid:(NSString*)uuid timestamp:(NSTimeInterval)timestamp;
- (void)onProcedureStage:(NSString*)procedureName stage:(NSString*)stage uuid:(NSString*)uuid timestamp:(NSTimeInterval)timestamp;
- (void)onProcedureFailure:(NSString*)procedureName errorInfo:(NSDictionary<NSString*, NSString*>*)errorInfo uuid:(NSString*)uuid timestamp:(NSTimeInterval)timestamp;
- (void)onProcedureSuccess:(NSString*)procedureName properties:(NSDictionary<NSString*, NSString*>*)properties uuid:(NSString*)uuid timestamp:(NSTimeInterval)timestamp;
- (void)onProcedurePause:(NSString*)procedureName uuid:(NSString*)uuid timestamp:(NSTimeInterval)timestamp;
- (void)onProcedureResume:(NSString*)procedureName uuid:(NSString*)uuid timestamp:(NSTimeInterval)timestamp;

#pragma sub-procedures
- (void)onSubProcedureBegin:(NSString*)procedureName parentPrecedureName:(NSString*)parentPrecedureName parentUuid:(NSString*)parentUuid;
- (void)onSubProcedureFailure:(NSString*)procedureName parentPrecedureName:(NSString*)parentPrecedureName parentUuid:(NSString*)parentUuid;
- (void)onSubProcedureSuccess:(NSString*)procedureName parentPrecedureName:(NSString*)parentPrecedureName parentUuid:(NSString*)parentUuid;

- (void)onSubProcedureBegin:(NSString*)procedureName properties:(NSDictionary<NSString*, NSString*>*)properties parentPrecedureName:(NSString*)parentPrecedureName parentUuid:(NSString*)parentUuid;
- (void)onSubProcedureFailure:(NSString*)procedureName properties:(NSDictionary<NSString*, NSString*>*)properties parentPrecedureName:(NSString*)parentPrecedureName parentUuid:(NSString*)parentUuid;
- (void)onSubProcedureSuccess:(NSString*)procedureName properties:(NSDictionary<NSString*, NSString*>*)properties parentPrecedureName:(NSString*)parentPrecedureName parentUuid:(NSString*)parentUuid;

#pragma full-trace events
- (void)onFullTraceEvent:(NSString*)eventName extraDescription:(NSString*)extraDescription;
- (void)onFullTraceEvent:(NSString*)eventName extraDescription:(NSString*)extraDescription timestamp:(NSTimeInterval)timestamp;

@end


#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
    
    id<ProcedureTrackInteface> getProcedureTrackService(void);
    void setProcedureTrackService(id<ProcedureTrackInteface> service);

#ifdef __cplusplus
}
#endif // __cplusplus
