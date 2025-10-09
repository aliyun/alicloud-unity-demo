#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

FOUNDATION_EXPORT NSString *const EAPMNetworkDeveloperToken;

// User Messages
FOUNDATION_EXPORT NSString *const EAPMNetworkMissingConsumerKeyMsg;
FOUNDATION_EXPORT NSString *const EAPMNetworkMissingConsumerSecretMsg;

// Exceptions
FOUNDATION_EXPORT NSString *const EAPMNetworkException;

// Network requests
FOUNDATION_EXPORT NSString *const EAPMNetworkAccept;
FOUNDATION_EXPORT NSString *const EAPMNetworkAcceptCharset;
FOUNDATION_EXPORT NSString *const EAPMNetworkApplicationJson;
FOUNDATION_EXPORT NSString *const EAPMNetworkApplicationOctetStream;
FOUNDATION_EXPORT NSString *const EAPMNetworkAcceptLanguage;
FOUNDATION_EXPORT NSString *const EAPMNetworkContentLanguage;
FOUNDATION_EXPORT NSString *const EAPMNetworkContentType;
FOUNDATION_EXPORT NSString *const EAPMNetworkAppKey;
FOUNDATION_EXPORT NSString *const EAPMNetworkSignature;
FOUNDATION_EXPORT NSString *const EAPMNetworkSignatureAlgorithm;
FOUNDATION_EXPORT NSString *const EAPMNetworkSignatureAlgorithmHmac;
FOUNDATION_EXPORT NSString *const EAPMNetworkCompression;
FOUNDATION_EXPORT NSString *const EAPMNetworkUserAgent;
FOUNDATION_EXPORT NSString *const EAPMNetworkUTF8;

FOUNDATION_EXPORT float const EAPMNetworkMainRunLoopBlockTime;
FOUNDATION_EXPORT BOOL const EAPMNetworkMainRunLoopIsCloseSampling;

NSString *EAPMNetworkSDKGeneratorName(void);

NSString *EAPMNetworkSDKVersion(void);

// Endpoints
NSString *EAPMNetworkReportsEndpoint(void);
NSString *EAPMNetworkSettingsEndpoint(void);

NS_ASSUME_NONNULL_END
