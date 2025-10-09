#ifndef EAPMCORE_EAPMLIBRARY_H
#define EAPMCORE_EAPMLIBRARY_H

#ifndef EAPMLibrary_h
#define EAPMLibrary_h

#import <Foundation/Foundation.h>

@class EAPMApm;
@class EAPMComponent;

NS_ASSUME_NONNULL_BEGIN

/// Provide an interface to register a library for userAgent logging and availability to others.
NS_SWIFT_NAME(Library)
@protocol EAPMLibrary

/// Returns one or more Components that will be registered in
/// EAPMApm and participate in dependency resolution and injection.
+ (NSArray<EAPMComponent *> *)componentsToRegister;

@end

NS_ASSUME_NONNULL_END

#endif /* EAPMLibrary_h */

#endif // EAPMCORE_EAPMLIBRARY_H
