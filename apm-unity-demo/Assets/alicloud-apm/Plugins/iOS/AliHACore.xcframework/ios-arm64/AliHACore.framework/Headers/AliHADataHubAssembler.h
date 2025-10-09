//
//  AliHADataHubImp.h
//  AliHACore
//
//  Created by hansong.lhs on 2018/5/22.
//

#import <Foundation/Foundation.h>

#import "AliAPMInterface.h"
#import "AliHADataHub.h"
#import "AliHAProtocol.h"

#pragma data context lifecycle protocol

@interface AliHADataHubAssembler : NSObject <ProcedureTrackInteface, AliHADataHubSubscriber, AliHAPluginProtocol>

+ (instancetype)sharedInstance;

@end

