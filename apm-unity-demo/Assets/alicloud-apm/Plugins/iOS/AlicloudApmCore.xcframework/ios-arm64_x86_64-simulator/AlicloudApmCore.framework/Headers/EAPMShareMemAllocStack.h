#ifndef EAPMShareMemAllocStack_h
#define EAPMShareMemAllocStack_h

#define EAPM_SHARE_MEM_ALLOC_STACK_MAX_DEPTH 64
#define EAPM_SHARE_MEM_ALLOC_TYPE_MAX_LENGTH 36
#define EAPM_SHARE_MEM_ALLOC_STACK_TOMBSTONE UINT32_MAX

#define EAPM_SHARE_MEM_ALLOC_VM_STACK_FILE_NAME "vm_stack.mmap"

#import <Foundation/Foundation.h>
#import <mach/vm_types.h>
#import <stdint.h>

typedef struct {
    uint32_t count;
    uint32_t size;
    uint32_t type;       // 0:malloc 1:vm
    uint32_t stackDepth;
    uint64_t digest;
    uint64_t timestamp;
    char memType[EAPM_SHARE_MEM_ALLOC_TYPE_MAX_LENGTH];
    vm_address_t stacks[EAPM_SHARE_MEM_ALLOC_STACK_MAX_DEPTH];
} EAPMShareMemAllocCacheStack;

NS_ASSUME_NONNULL_BEGIN

@protocol EAPMShareMemAllocStack <NSObject>

/**
 * 获取虚拟内存分配堆栈文件地址
 *
 */
- (NSString *)vmStackFilePath;

@end

NS_ASSUME_NONNULL_END

#endif /* EAPMShareMemAllocStack_h */
