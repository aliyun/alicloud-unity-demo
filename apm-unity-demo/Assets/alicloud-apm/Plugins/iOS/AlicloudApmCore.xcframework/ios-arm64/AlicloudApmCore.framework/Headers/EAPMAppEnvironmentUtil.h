#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMAppEnvironmentUtil : NSObject

/// Indicates whether the app is from Apple Store or not. Returns NO if the app is on simulator,
/// development environment or sideloaded.
+ (BOOL)isFromAppStore;

/// Indicates whether the app is a Testflight app. Returns YES if the app has sandbox receipt.
/// Returns NO otherwise.
+ (BOOL)isAppStoreReceiptSandbox;

/// Indicates whether the app is on simulator or not at runtime depending on the device
/// architecture.
+ (BOOL)isSimulator;

/// The current device model. Returns an empty string if device model cannot be retrieved.
+ (nullable NSString *)deviceModel;

/// The current device model, with simulator-specific values. Returns an empty string if device
/// model cannot be retrieved.
+ (nullable NSString *)deviceSimulatorModel;

/// The current operating system version. Returns an empty string if the system version cannot be
/// retrieved.
+ (NSString *)systemVersion;

/// Indicates whether it is running inside an extension or an app.
+ (BOOL)isAppExtension;

/// @return An Apple platform. Possible values "ios", "tvos", "macos", "watchos", "maccatalyst", and
/// "visionos".
+ (NSString *)applePlatform;

/// @return An Apple Device platform. Same possible values as `applePlatform`, with the addition of
/// "ipados".
+ (NSString *)appleDevicePlatform;

/// @return The way the library was added to the app, e.g. "swiftpm", "cocoapods", etc.
+ (NSString *)deploymentType;

@end

NS_ASSUME_NONNULL_END
