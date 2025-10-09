#import "EAPMEventDataObject.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface EAPMEvent : NSObject <NSSecureCoding>

/** The unique ID of the event. */
@property (readonly, nonatomic) NSString *eventId;

/** Alicloud APM appKey */
@property (readonly, nonatomic) NSString *appKey;

/** Alicloud APM appSecret */
@property (readonly, nonatomic) NSString *appSecret;

@property (nullable, nonatomic) id<EAPMEventDataObject> dataObject;

/** The serialized bytes from calling [dataObject transportJson]. */
@property (nullable, nonatomic) NSData *serializedDataObjectJson;

/** The expiration date of the event. Default is 604800 seconds (7 days) from creation. */
@property (nonatomic) NSDate *expirationDate;

/** Bytes that can be used by an uploader later on. */
@property (nullable, nonatomic) NSData *customBytes;

/** Generates a unique event ID. */
+ (NSString *)nextEventId;

// Please use the designated initializer.
- (instancetype)init NS_UNAVAILABLE;

/** Initializes a new event
 *
 * @param appKey alicloud APM appKey
 * @param appSecret alicloud APM appSecret
 * @return A event
 */
- (nullable instancetype)initWithAppkey:(NSString *)appKey appSecret:(NSString *)appSecret NS_DESIGNATED_INITIALIZER;

@end

NS_ASSUME_NONNULL_END
