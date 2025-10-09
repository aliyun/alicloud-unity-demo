#import <Foundation/Foundation.h>
#import <AlicloudApmCore/AlicloudApmReport.h>

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMEventDataObject <NSObject>

@property (nonatomic, readonly) EAPMReport *report;

@required

- (NSData *)transportJson;
- (NSString *)eventId;

@end

NS_ASSUME_NONNULL_END
