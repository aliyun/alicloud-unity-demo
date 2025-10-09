#import <Foundation/Foundation.h>

@class EAPMShareStackFrame;

@protocol EAPMShareSymbolResolver <NSObject>

- (EAPMShareStackFrame *)frameForAddress:(uint64_t)address;

@end
