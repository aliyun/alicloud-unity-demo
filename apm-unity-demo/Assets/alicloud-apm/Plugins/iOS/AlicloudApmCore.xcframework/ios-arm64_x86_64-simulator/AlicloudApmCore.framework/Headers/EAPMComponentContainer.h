#ifndef EAPMCORE_EAPMCOMPONENTCONTAINER_H
#define EAPMCORE_EAPMCOMPONENTCONTAINER_H

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/// A type-safe macro to retrieve a component from a container. This should be used to retrieve
/// components instead of using the container directly.
#define EAPM_COMPONENT(type, container) [EAPMComponentType<id<type>> instanceForProtocol:@protocol(type) inContainer:container]

@class EAPMApm;

/// A container that holds different components that are registered via the
/// `registerAsComponentRegistrant` call. These classes should conform to `ComponentRegistrant`
/// in order to properly register components for Core.
NS_SWIFT_NAME(EAPMComponentContainer)
@interface EAPMComponentContainer : NSObject

/// A weak reference to the app that an instance of the container belongs to.
@property (nonatomic, weak, readonly) EAPMApm *apm;

// TODO: See if we can get improved type safety here.
/// A Swift only API for fetching an instance since the top macro isn't available.
- (nullable id)__instanceForProtocol:(Protocol *)protocol NS_SWIFT_NAME(instance(for:));

/// Unavailable. Use the `container` property on `EAPMApm`.
- (instancetype)init NS_UNAVAILABLE;

@end

NS_ASSUME_NONNULL_END

#endif // EAPMCORE_EAPMCOMPONENTCONTAINER_H
