#ifndef EAPMCORE_EAPMCOMPONENTTYPE_H
#define EAPMCORE_EAPMCOMPONENTTYPE_H

#import <Foundation/Foundation.h>

@class EAPMComponentContainer;

NS_ASSUME_NONNULL_BEGIN

/// Do not use directly. A placeholder type in order to provide a macro that will warn users of
/// mis-matched protocols.
NS_SWIFT_NAME(ComponentType)
@interface EAPMComponentType<__covariant T> : NSObject

/// Do not use directly. A factory method to retrieve an instance that provides a specific
/// functionality.
+ (nullable T)instanceForProtocol:(Protocol *)protocol inContainer:(EAPMComponentContainer *)container;

@end

NS_ASSUME_NONNULL_END

#endif // EAPMCORE_EAPMCOMPONENTTYPE_H
