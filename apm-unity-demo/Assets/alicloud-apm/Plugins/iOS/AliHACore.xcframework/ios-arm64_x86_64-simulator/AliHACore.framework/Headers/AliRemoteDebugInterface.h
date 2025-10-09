//
//  AliRemoteDebugServiceInterface.h
//  AliInfrastructureServiceInterface
//
//  Created by 晨燕 on 2018/1/8.
//

#import <Foundation/Foundation.h>


typedef NS_ENUM(NSUInteger, AliRemoteDebugLogLevel) {
    AliRemoteDebugLogLevel_Off,
    AliRemoteDebugLogLevel_Error,
    AliRemoteDebugLogLevel_Warn,
    AliRemoteDebugLogLevel_Info,
    AliRemoteDebugLogLevel_Debug
};

@protocol AliRemoteDebugInterface <NSObject>

- (void)debug:(NSString *)message module:(NSString*)module exception:(NSException *)exception;

- (void)info:(NSString *)message module:(NSString*)module exception:(NSException *)exception;

- (void)warn:(NSString *)message module:(NSString*)module exception:(NSException *)exception;

- (void)error:(NSString *)message module:(NSString*)module exception:(NSException *)exception;

 /**
 *  在某些场景或异常流程下，业务方主动调用，能够直接上传TLog日志，默认上传当天的日志，如果当时客户端的网站环境是非wifi,
 *  会把上传指令存成离线任务。等待切换到wifi再上传。
 *
 *  @param extraInfo 上传成功后附带的业务参数，由调用方自定义传入: @{@"反馈标题":,@"反馈内容":,@"分类名称":,@"昵称":,@"utdid":}
 */
- (void)uploadLogFile:(NSDictionary*)extraInfo;

/**
 * current log level
 */
- (AliRemoteDebugLogLevel)currentLogLevel;

@end


#define AliRemoteDebugService getAliRemoteDebugService()

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
    
    // default get [AliRemoteDebugImp sharedInstance]接口来获取，如果没有的话，就返回nil
    id<AliRemoteDebugInterface> getAliRemoteDebugService(void);
    void setAliRemoteDebugService(id<AliRemoteDebugInterface>);

#ifdef __cplusplus
}
#endif // __cplusplus


