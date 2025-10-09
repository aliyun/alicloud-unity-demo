#import <Foundation/Foundation.h>

#import "EAPMObjectiveCObject.h"

/**
 Specialization of EAPMObjectiveCObject for NSTimer.
 Standard methods that EAPMObjectiveCObject uses will not fetch us all objects retained by NSTimer.
 One good example is target of NSTimer.
 */
@interface EAPMObjectiveCNSCFTimer : EAPMObjectiveCObject
@end
