#import <Foundation/Foundation.h>

#import "EAPMObjectGraphConfiguration.h"

#ifdef __cplusplus
extern "C" {
#endif

/**
 Standard filters mostly filters excluding some UIKit references we have caught during testing on some apps.
 */
NSArray<EAPMGraphEdgeFilterBlock> *_Nonnull EAPMGetStandardGraphEdgeFilters(void);

/**
 Helper functions for some typical patterns.
 */
EAPMGraphEdgeFilterBlock _Nonnull EAPMFilterBlockWithObjectIvarRelation(Class _Nonnull aCls,
                                                                    NSString *_Nonnull ivarName);
EAPMGraphEdgeFilterBlock _Nonnull EAPMFilterBlockWithObjectToManyIvarsRelation(Class _Nonnull aCls,
                                                                           NSSet<NSString *> *_Nonnull ivarNames);
EAPMGraphEdgeFilterBlock _Nonnull EAPMFilterBlockWithObjectIvarObjectRelation(Class _Nonnull fromClass,
                                                                          NSString *_Nonnull ivarName,
                                                                          Class _Nonnull toClass);

#ifdef __cplusplus
}
#endif
