#import <Foundation/Foundation.h>

#import "EAPMAllocationTrackerDefines.h"

#ifdef __cplusplus
extern "C" {
#endif

BOOL EAPMIsEAPMATEnabledInThisBuild(void);

#ifdef __cplusplus
}
#endif

@class EAPMAllocationTrackerSummary;

/**
 EAPMAllocationTrackerManager is a wrapper around C++ Allocation Tracker API.
 This will let you interact with EAPMAllocationTracker using just Objective-C.
 Because EAPMAllocationTracker is guarded by compilation flag, all calls in this
 API are nullable.
 */
@interface EAPMAllocationTrackerManager : NSObject

+ (nullable instancetype)sharedManager;

/**
 Enable tracking allocations.
 */
- (void)startTrackingAllocations;

/**
 Disable tracking allocations. It will clear all data gathered so far.
 */
- (void)stopTrackingAllocations;

- (BOOL)isAllocationTrackerEnabled;

/**
 Grab summary snapshot for current moment. Every object in this array will have a quick
 snapshot containing how many instances were allocated/deallocated and few more.
 Check EAPMAllocationTrackerSummary.h.
 */
- (nullable NSArray<EAPMAllocationTrackerSummary *> *)currentAllocationSummary;

/**
 Enable generations. If generations were already enabled, it won't do anything.
 
 The number of enables and disables must match.
 */
- (void)enableGenerations;

/**
 Disable generations. It will remove all generations data.
 */
- (void)disableGenerations;

/**
 Marks generation. Every allocation is attributed to just one generation. After you mark
 generation - all new allocations will be dropped to the new generation bucket.
 */
- (void)markGeneration;

/**
 Creates summary for all generations. Every element will be a summary for one generation.
 In every generation the summary for given class will be described inside EAPMAllocationTrackerSummary
 object.
 Returns nil if generations are not enabled.
 */
- (nullable NSArray<NSArray<EAPMAllocationTrackerSummary *> *> *)currentSummaryForGenerations;

/**
 Accessor to browse objects inside Allocation Tracker. You can browse objects only if generations
 are enabled (otherwise you are just tracking counters).
 Returns nil if generations are not enabled.
 */
- (nullable NSArray *)instancesForClass:(nonnull __unsafe_unretained Class)aCls
                           inGeneration:(NSInteger)generation;


/**
 Grab all instances of given classes across all generations.
 Returns nil if generations are not enabled.
 */
- (nullable NSArray *)instancesOfClasses:(nonnull NSArray *)classes;

/**
 Grab all instances of given classes and their subclasses across all generations.
 This method will find instances of the specified classes as well as any subclasses
 that inherit from them.
 
 For example, if you pass @[[UIViewController class]], it will return instances of:
 - UIViewController itself
 - UINavigationController (subclass of UIViewController)
 - UITableViewController (subclass of UIViewController) 
 - Any custom UIViewController subclasses
 
 @param classes Array of Class objects to search for (including their subclasses)
 @return NSArray of all instances found, or nil if generations are not enabled
 */
- (nullable NSArray *)instancesOfClassesAndSubclasses:(nonnull NSArray *)classes;

/**
 Gets all classes that were used in allocation tracker. Basically if any instance of given
 class was created when Allocation Tracker was enabled - this class will be there.
 */
- (nullable NSSet<Class> *)trackedClasses;

@end
