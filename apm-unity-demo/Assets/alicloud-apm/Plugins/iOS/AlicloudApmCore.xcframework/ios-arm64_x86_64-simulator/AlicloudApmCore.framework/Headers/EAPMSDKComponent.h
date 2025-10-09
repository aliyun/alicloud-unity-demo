#ifndef EAPMSDKComponent_h
#define EAPMSDKComponent_h

NS_SWIFT_NAME(EAPMSDKComponent)
@protocol EAPMSDKComponent <NSObject>

@required
+ (NSString *)getLibraryName;

+ (BOOL)reportActiveEvent;

@end

#endif /* EAPMSDKComponent_h */
