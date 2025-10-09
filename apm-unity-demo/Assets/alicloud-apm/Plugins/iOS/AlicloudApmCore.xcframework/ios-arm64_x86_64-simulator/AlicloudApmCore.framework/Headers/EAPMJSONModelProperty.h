#import <Foundation/Foundation.h>
/*
 相关知识请参见Runtime文档
 Type Encodings https://developer.apple.com/library/mac/documentation/Cocoa/Conceptual/ObjCRuntimeGuide/Articles/ocrtTypeEncodings.html#//apple_ref/doc/uid/TP40008048-CH100-SW1
 Property Type String https://developer.apple.com/library/mac/documentation/Cocoa/Conceptual/ObjCRuntimeGuide/Articles/ocrtPropertyIntrospection.html#//apple_ref/doc/uid/TP40008048-CH101-SW6
 */
typedef NS_ENUM(NSInteger, EAPMJSONModelPropertyValueType) {
    EAPMClassPropertyValueTypeNone = 0,
    EAPMClassPropertyTypeChar,
    EAPMClassPropertyTypeInt,
    EAPMClassPropertyTypeShort,
    EAPMClassPropertyTypeLong,
    EAPMClassPropertyTypeLongLong,
    EAPMClassPropertyTypeUnsignedChar,
    EAPMClassPropertyTypeUnsignedInt,
    EAPMClassPropertyTypeUnsignedShort,
    EAPMClassPropertyTypeUnsignedLong,
    EAPMClassPropertyTypeUnsignedLongLong,
    EAPMClassPropertyTypeFloat,
    EAPMClassPropertyTypeDouble,
    EAPMClassPropertyTypeBool,
    EAPMClassPropertyTypeVoid,
    EAPMClassPropertyTypeCharString,
    EAPMClassPropertyTypeObject,
    EAPMClassPropertyTypeClassObject,
    EAPMClassPropertyTypeSelector,
    EAPMClassPropertyTypeArray,
    EAPMClassPropertyTypeStruct,
    EAPMClassPropertyTypeUnion,
    EAPMClassPropertyTypeBitField,
    EAPMClassPropertyTypePointer,
    EAPMClassPropertyTypeUnknow
};

@interface EAPMJSONModelProperty : NSObject {
    @public
    NSString *_name;
    EAPMJSONModelPropertyValueType _valueType;
    NSString *_typeName;
    Class _objectClass;
    NSArray *_objectProtocols;
    Class _containerElementClass;
    BOOL _isReadonly;
}

+ (instancetype)propertyWithName:(NSString *)name typeString:(NSString *)typeString;

@end
