#import <Foundation/Foundation.h>

//! Project version number for EAPMRetainCycleDetector.
FOUNDATION_EXPORT double EAPMRetainCycleDetectorVersionNumber;

//! Project version string for EAPMRetainCycleDetector.
FOUNDATION_EXPORT const unsigned char EAPMRetainCycleDetectorVersionString[];

#import "EAPMAssociationManager.h"
#import "EAPMObjectiveCBlock.h"
#import "EAPMObjectiveCGraphElement.h"
#import "EAPMObjectiveCNSCFTimer.h"
#import "EAPMObjectiveCObject.h"
#import "EAPMObjectGraphConfiguration.h"
#import "EAPMStandardGraphEdgeFilters.h"

/**
 Retain Cycle Detector is enabled by default in DEBUG builds, but you can also force it in other builds by
 uncommenting the line below. Beware, Retain Cycle Detector uses some private APIs that shouldn't be compiled in
 production builds.
 注意：当前已启用在生产环境，注释下一行可禁用
 */
#define RETAIN_CYCLE_DETECTOR_ENABLED 1

/**
 EAPMRetainCycleDetector

 The main class responsible for detecting retain cycles.

 Be cautious, the class is NOT thread safe.

 The process of detecting retain cycles is relatively slow and consumes a lot of CPU.
 */

@interface EAPMRetainCycleDetector : NSObject

/**
 Designated initializer

 @param configuration Configuration for detector. Can include specific filters and options.
 @see EAPMRetainCycleDetectorConfiguration
 */
- (nonnull instancetype)initWithConfiguration:(nonnull EAPMObjectGraphConfiguration *)configuration NS_DESIGNATED_INITIALIZER;

/**
 Adds candidate you are interested in getting retain cycles from.

 @param candidate Any Objective-C object you want to verify for cycles.
 */
- (void)addCandidate:(nonnull id)candidate;

/**
 Searches for all retain cycles for all candidates the detector has been
 provided with.

 @return NSSet with retain cycles. An element of this array will be
 an array representing retain cycle. That array will hold elements
 of type EAPMObjectiveCGraphElement.

 @discussion For given candidate, the detector will go through all object graph rooted in this candidate and return
 ALL retain cycles that this candidate references. It will also take care of removing duplicates. It will not look for
 cycles longer than 10 elements. If you want to look for longer ones use findRetainCyclesWithMaxCycleLenght:
 */
- (nonnull NSSet<NSArray<EAPMObjectiveCGraphElement *> *> *)findRetainCycles;

- (nonnull NSSet<NSArray<EAPMObjectiveCGraphElement *> *> *)findRetainCyclesWithMaxCycleLength:(NSUInteger)length;

/**
 This macro is used across EAPMRetainCycleDetector to compile out sensitive code.
 If you do not define it anywhere, Retain Cycle Detector will be available in DEBUG builds.
 */
#ifdef RETAIN_CYCLE_DETECTOR_ENABLED
#define _INTERNAL_RCD_ENABLED RETAIN_CYCLE_DETECTOR_ENABLED
#else
#define _INTERNAL_RCD_ENABLED DEBUG
#endif

@end
