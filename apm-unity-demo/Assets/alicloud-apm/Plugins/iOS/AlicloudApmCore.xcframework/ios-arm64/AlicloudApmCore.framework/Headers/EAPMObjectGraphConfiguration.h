#import <Foundation/Foundation.h>

#import "EAPMObjectiveCGraphElement.h"

typedef NS_ENUM(NSUInteger, EAPMGraphEdgeType) {
  EAPMGraphEdgeValid,
  EAPMGraphEdgeInvalid,
};

@protocol EAPMObjectReference;

/**
 Every filter has to be of type EAPMGraphEdgeFilterBlock. Filter, given two object graph elements, it should decide,
 whether a reference between them should be filtered out or not.
 @see EAPMGetStandardGraphEdgeFilters()
 */
typedef EAPMGraphEdgeType (^EAPMGraphEdgeFilterBlock)(EAPMObjectiveCGraphElement *_Nullable fromObject,
                                                  NSString *_Nullable byIvar,
                                                  Class _Nullable toObjectOfClass);

typedef EAPMObjectiveCGraphElement *_Nullable(^EAPMObjectiveCGraphElementTransformerBlock)(EAPMObjectiveCGraphElement *_Nonnull fromObject);

/**
 EAPMObjectGraphConfiguration represents a configuration for object graph walking.
 It can hold filters and detector specific options.
 */
@interface EAPMObjectGraphConfiguration : NSObject

/**
 Every block represents a filter that every reference must pass in order to be inspected.
 Reference will be described as relation from one object to another object. See definition of
 EAPMGraphEdgeFilterBlock above.

 Invalid relations would be the relations that we are guaranteed are going to be broken at some point.
 Be careful though, it's not so straightforward to tell if the relation will be broken *with 100%
 certainty*, and if you'll filter out something that could otherwise show retain cycle that leaks -
 it would never be caught by detector.

 For examples of what are the relations that will be broken at some point check EAPMStandardGraphEdgeFilters.mm
 */
@property (nonatomic, readonly, copy, nullable) NSArray<EAPMGraphEdgeFilterBlock> *filterBlocks;

@property (nonatomic, readonly, copy, nullable) EAPMObjectiveCGraphElementTransformerBlock transformerBlock;

/**
 Decides if object graph walker should look for retain cycles inside NSTimers.
 */
@property (nonatomic, readonly) BOOL shouldInspectTimers;

/**
 Decides if block objects should include their invocation address (the code part of the block) in the report.
 If set to YES, then it will change from: `MallocBlock` to `<<MallocBlock:0xADDR>>`.
 You can then symbolicate the address to retrieve a symbol name which will look like:
 `__FOO_block_invoke` where FOO is replaced by the function creating the block.
 This will allow easier understanding of the code involved in the cycle when blocks are involved.
 */
@property (nonatomic, readonly) BOOL shouldIncludeBlockAddress;

/**
 To enabled the inclution of swift Objects in the object graph.
 */
@property (nonatomic, readonly) BOOL shouldIncludeSwiftObjects;

/**
 Will cache layout
 */
@property (nonatomic, readonly, nullable) NSMutableDictionary<NSString*, NSArray<id<EAPMObjectReference>> *> *layoutCache;
@property (nonatomic, readonly) BOOL shouldCacheLayouts;

- (nonnull instancetype)initWithFilterBlocks:(nonnull NSArray<EAPMGraphEdgeFilterBlock> *)filterBlocks
                         shouldInspectTimers:(BOOL)shouldInspectTimers
                         transformerBlock:(nullable EAPMObjectiveCGraphElementTransformerBlock)transformerBlock
                         shouldIncludeBlockAddress:(BOOL)shouldIncludeBlockAddress
                         shouldIncludeSwiftObjects:(BOOL) shouldIncludeSwiftObjects NS_DESIGNATED_INITIALIZER;

- (nonnull instancetype)initWithFilterBlocks:(nonnull NSArray<EAPMGraphEdgeFilterBlock> *)filterBlocks
                         shouldInspectTimers:(BOOL)shouldInspectTimers
                         transformerBlock:(nullable EAPMObjectiveCGraphElementTransformerBlock)transformerBlock
                         shouldIncludeBlockAddress:(BOOL)shouldIncludeBlockAddress;

- (nonnull instancetype)initWithFilterBlocks:(nonnull NSArray<EAPMGraphEdgeFilterBlock> *)filterBlocks
                         shouldInspectTimers:(BOOL)shouldInspectTimers
                         transformerBlock:(nullable EAPMObjectiveCGraphElementTransformerBlock)transformerBlock;

- (nonnull instancetype)initWithFilterBlocks:(nonnull NSArray<EAPMGraphEdgeFilterBlock> *)filterBlocks
                         shouldInspectTimers:(BOOL)shouldInspectTimers;

@end
