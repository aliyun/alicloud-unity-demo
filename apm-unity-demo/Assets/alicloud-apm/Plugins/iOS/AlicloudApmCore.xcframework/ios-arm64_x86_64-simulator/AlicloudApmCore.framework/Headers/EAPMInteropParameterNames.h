#import <Foundation/Foundation.h>

static NSString *const kEAPMIParameterSource NS_SWIFT_NAME(AnalyticsParameterSource) = @"source";

/// The advertising or marketing medium, for example: cpc, banner, email, push. Highly recommended
/// (String).
/// <pre>
///     let params = [
///       kEAPMParameterMedium : "email",
///       // ...
///     ]
/// </pre>
static NSString *const kEAPMIParameterMedium NS_SWIFT_NAME(AnalyticsParameterMedium) = @"medium";

/// The individual campaign name, slogan, promo code, etc. Some networks have pre-defined macro to
/// capture campaign information, otherwise can be populated by developer. Highly Recommended
/// (String).
/// <pre>
///     let params = [
///       kEAPMParameterCampaign : "winter_promotion",
///       // ...
///     ]
/// </pre>
static NSString *const kEAPMIParameterCampaign NS_SWIFT_NAME(AnalyticsParameterCampaign) = @"campaign";

/// Message identifier.
static NSString *const kEAPMIParameterMessageIdentifier = @"_nmid";

/// Message name.
static NSString *const kEAPMIParameterMessageName = @"_nmn";

/// Message send time.
static NSString *const kEAPMIParameterMessageTime = @"_nmt";

/// Message device time.
static NSString *const kEAPMIParameterMessageDeviceTime = @"_ndt";

/// Topic message.
static NSString *const kEAPMIParameterTopic = @"_nt";

/// Stores the message_id of the last notification opened by the app.
static NSString *const kEAPMIUserPropertyLastNotification = @"_ln";
