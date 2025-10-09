#import <Foundation/Foundation.h>

extern NSString *const EAPMJSONModelErrorDomain;

typedef NS_ENUM(int, EAPMJSONModelErrorCode) { EAPMJSONModelErrorCodeNilInput = 1, EAPMJSONModelErrorCodeDataInvalid = 2 };

@interface EAPMJSONModelError : NSError

+ (id)errorNilInput;
+ (id)errorDataInvalidWithDescription:(NSString *)description;

@end
