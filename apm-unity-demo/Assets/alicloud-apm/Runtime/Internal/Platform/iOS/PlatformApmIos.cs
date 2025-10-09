using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Alicloud.Apm
{
#if UNITY_IOS && !UNITY_EDITOR
    internal class PlatformApmIos : IPlatformApm
    {
        /// <summary>
        /// Start APM with complete configuration
        /// </summary>
        [DllImport(IosNativeLibrary.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void eapm_start(
            [MarshalAs(UnmanagedType.LPStr)] string appKey,
            [MarshalAs(UnmanagedType.LPStr)] string appSecret,
            [MarshalAs(UnmanagedType.LPStr)] string appRsaSecret,
            [MarshalAs(UnmanagedType.LPStr)] string appGroupId,
            [MarshalAs(UnmanagedType.LPStr)] string userId,
            [MarshalAs(UnmanagedType.LPStr)] string userNick,
            [MarshalAs(UnmanagedType.LPStr)] string channel,
            int sdkComponents
        );

        /// <summary>
        /// Set user ID
        /// </summary>
        [DllImport(IosNativeLibrary.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void eapm_set_user_id([MarshalAs(UnmanagedType.LPStr)] string userId);

        /// <summary>
        /// Set user nickname
        /// </summary>
        [DllImport(IosNativeLibrary.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void eapm_set_user_nick(
            [MarshalAs(UnmanagedType.LPStr)] string userNick
        );

        /// <summary>
        /// Set custom key-value pair
        /// </summary>
        [DllImport(IosNativeLibrary.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void eapm_set_custom_value(
            [MarshalAs(UnmanagedType.LPStr)] string key,
            [MarshalAs(UnmanagedType.LPStr)] string value
        );

        public void Start(ApmOptions options)
        {
            try
            {
                var components = (int)options.SdkComponents;
                eapm_start(
                    options.AppKey ?? string.Empty,
                    options.AppSecret ?? string.Empty,
                    options.AppRsaSecret ?? string.Empty,
                    options.AppGroupId ?? string.Empty,
                    options.UserId ?? string.Empty,
                    options.UserNick ?? string.Empty,
                    options.Channel ?? string.Empty,
                    components
                );
            }
            catch
            {
                ApmLogger.Error("Failed to start APM on iOS platform");
            }
        }

        public void SetUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            try
            {
                eapm_set_user_id(userId);
            }
            catch
            {
                ApmLogger.Error($"Failed to set user ID: {userId}");
            }
        }

        public void SetUserNick(string userNick)
        {
            if (string.IsNullOrEmpty(userNick))
            {
                return;
            }

            try
            {
                eapm_set_user_nick(userNick);
            }
            catch
            {
                ApmLogger.Error($"Failed to set user nick: {userNick}");
            }
        }

        public void SetCustomKeyValue(string key, object value)
        {
            if (key == null || value == null)
            {
                return;
            }

            try
            {
                eapm_set_custom_value(key, value.ToString());
            }
            catch
            {
                ApmLogger.Error($"Failed to set custom value: {key} = {value}");
            }
        }

        public void SetCustomKeysAndValues(IDictionary<string, object> keysAndValues)
        {
            if (keysAndValues == null || keysAndValues.Count == 0)
            {
                return;
            }

            foreach (var pair in keysAndValues)
            {
                SetCustomKeyValue(pair.Key, pair.Value);
            }
        }
    }
#endif
}
