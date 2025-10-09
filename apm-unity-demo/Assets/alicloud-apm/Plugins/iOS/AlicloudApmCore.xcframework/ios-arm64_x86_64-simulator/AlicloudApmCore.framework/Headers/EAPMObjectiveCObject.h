#import <Foundation/Foundation.h>

#import "EAPMObjectiveCGraphElement.h"

@class EAPMGraphEdgeFilterProvider;

/**
 EAPMObjectiveCGraphElement specialization that can gather all references kept in ivars, as part of collection
 etc.
 */
@interface EAPMObjectiveCObject : EAPMObjectiveCGraphElement
@end
