#import <Foundation/Foundation.h>

@interface EAPMAllocationTrackerSummary : NSObject

@property (nonatomic, readonly) NSUInteger allocations;
@property (nonatomic, readonly) NSUInteger deallocations;
@property (nonatomic, readonly) NSInteger aliveObjects;
@property (nonatomic, copy, readonly, nonnull) NSString *className;
@property (nonatomic, readonly) NSUInteger instanceSize;

- (nonnull instancetype)initWithAllocations:(NSUInteger)allocations
                              deallocations:(NSUInteger)deallocations
                               aliveObjects:(NSInteger)aliveObjects
                                  className:(nonnull NSString *)className
                               instanceSize:(NSUInteger)instanceSize;

@end
