#import <Foundation/Foundation.h>
#import <mach/mach.h>

@interface EAPMThreadScene : NSObject

@property (nonatomic, strong) NSArray *threadScene;
@property (nonatomic, strong) NSString *sceneTitle;

+ (EAPMThreadScene *)captureSceneForThread:(thread_t)thread;

+ (EAPMThreadScene *)captureSceneForThreadIndex:(int)index;

@end
