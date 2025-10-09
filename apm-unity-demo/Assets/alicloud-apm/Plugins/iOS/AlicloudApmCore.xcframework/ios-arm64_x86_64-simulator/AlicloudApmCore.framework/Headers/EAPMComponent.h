#ifndef EAPMCORE_EAPMCOMPONENT_H
#define EAPMCORE_EAPMCOMPONENT_H

#import <Foundation/Foundation.h>

@class EAPMApm;
@class EAPMComponentContainer;

NS_ASSUME_NONNULL_BEGIN

/// Provides a system to clean up cached instances returned from the component system.
NS_SWIFT_NAME(ComponentLifecycleMaintainer)
@protocol EAPMComponentLifecycleMaintainer
/// The associated apm will be deleted, clean up any resources as they are about to be deallocated.
- (void)apmWillBeDeleted:(EAPMApm *)apm;
@end

typedef _Nullable id (^EAPMComponentCreationBlock)(EAPMComponentContainer *container, BOOL *isCacheable)
NS_SWIFT_NAME(ComponentCreationBlock);

/// Describes the timing of instantiation. Note: new components should default to lazy unless there
/// is a strong reason to be eager.
typedef NS_ENUM(NSInteger, EAPMInstantiationTiming) {
    EAPMInstantiationTimingLazy,
    EAPMInstantiationTimingAlwaysEager,
    EAPMInstantiationTimingEagerInDefaultApm
} NS_SWIFT_NAME(InstantiationTiming);

/// A component that can be used from other EAPM SDKs.
NS_SWIFT_NAME(Component)
@interface EAPMComponent : NSObject

/// The protocol describing functionality provided from the `Component`.
@property (nonatomic, strong, readonly) Protocol *protocol;

/// The timing of instantiation.
@property (nonatomic, readonly) EAPMInstantiationTiming instantiationTiming;

/// A block to instantiate an instance of the component with the appropriate dependencies.
@property (nonatomic, copy, readonly) EAPMComponentCreationBlock creationBlock;

// There's an issue with long NS_SWIFT_NAMES that causes compilation to fail, disable clang-format
// for the next two methods.
// clang-format off

/// Creates a component with no dependencies that will be lazily initialized.
+ (instancetype)componentWithProtocol:(Protocol *)protocol
                        creationBlock:(EAPMComponentCreationBlock)creationBlock
NS_SWIFT_NAME(init(_:creationBlock:));

/// Creates a component to be registered with the component container.
///
/// @param protocol - The protocol describing functionality provided by the component.
/// @param instantiationTiming - When the component should be initialized. Use .lazy unless there's
///                              a good reason to be instantiated earlier.
/// @param creationBlock - A block to instantiate the component with a container, and if
/// @return A component that can be registered with the component container.
+ (instancetype)componentWithProtocol:(Protocol *)protocol
                  instantiationTiming:(EAPMInstantiationTiming)instantiationTiming
                        creationBlock:(EAPMComponentCreationBlock)creationBlock
NS_SWIFT_NAME(init(_:instantiationTiming:creationBlock:));

// clang-format on

/// Unavailable.
- (instancetype)init NS_UNAVAILABLE;

@end

NS_ASSUME_NONNULL_END

#endif // EAPMCORE_EAPMCOMPONENT_H
