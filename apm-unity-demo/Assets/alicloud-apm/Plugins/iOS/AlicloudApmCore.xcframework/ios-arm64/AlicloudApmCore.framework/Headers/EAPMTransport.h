#import <Foundation/Foundation.h>

@class EAPMApm;
@class EAPMEvent;

NS_ASSUME_NONNULL_BEGIN

@interface EAPMTransport : NSObject

// Please use the designated initializer.
- (instancetype)init NS_UNAVAILABLE;

/** Initializes a new transport that will send events to the given target backend.
 *
 * @param apm alicloud APM
 * @return A transport that will send events.
 */
- (nullable instancetype)initWithApm:(EAPMApm *)apm NS_DESIGNATED_INITIALIZER;

- (BOOL)sendEventWithPayload:(NSObject *)payload eventId:(NSString *)eventId;

- (BOOL)sendEventWithPayload:(NSObject *)payload eventId:(NSString *)eventId urgent:(BOOL)urgent;

- (BOOL)sendEventWithPayload:(NSObject *)payload eventId:(NSString *)eventId urgent:(BOOL)urgent onCompletion:(nullable void (^)(BOOL success, NSError *error))completion;

- (BOOL)sendEvent:(EAPMEvent *)event urgent:(BOOL)urgent onCompletion:(nullable void (^)(BOOL success, NSError *error))completion;

/** Creates an event for use by this transport.
 *
 * @return An event that is suited for use by this transport.
 */
- (EAPMEvent *)eventForTransport;

@end

NS_ASSUME_NONNULL_END
