#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/**
 * This class is a model to identify a single execution of the app
 */
@interface EAPMExecutionIdentifierModel : NSObject

/**
 * Returns the launch identifier. This is a unique id that will remain constant until this process
 * is relaunched. This value is useful for correlating events across kits and/or across reports at
 * the process-lifecycle level.
 */
@property (nonatomic, readonly) NSString *executionID;

@end

NS_ASSUME_NONNULL_END
