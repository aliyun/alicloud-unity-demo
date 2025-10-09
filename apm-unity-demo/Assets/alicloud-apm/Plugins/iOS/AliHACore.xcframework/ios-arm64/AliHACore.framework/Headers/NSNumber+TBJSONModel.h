//
//  NSNumber+TSMantleAddition.h
//  MantleExtension
//
//  Created by Hilen on 3/29/16.
//  Copyright © 2016 Hilen. All rights reserved.
//

//stolen from : https://github.com/ibireme/YYCategories/blob/master/YYCategories/Foundation/NSNumber%2BYYAdd.h

#define NS_ASSUME_NONNULL_BEGIN _Pragma("clang assume_nonnull begin")
#define NS_ASSUME_NONNULL_END   _Pragma("clang assume_nonnull end")


#import <Foundation/Foundation.h>
NS_ASSUME_NONNULL_BEGIN
/**
 Provide a method to parse `NSString` for `NSNumber`.
 */
@interface NSNumber (TBJSONModel)

/**
 Creates and returns an NSNumber object from a string.
 Valid format: @"12", @"12.345", @" -0xFF", @" .23e99 "...
 
 @param string  The string described an number.
 
 @return an NSNumber when parse succeed, or nil if an error occurs.
 */
+ (nullable NSNumber *)tbjsonmodel_numberWithString:(NSString *)string;

@end

NS_ASSUME_NONNULL_END




