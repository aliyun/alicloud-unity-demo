#nullable enable

using System;
using System.Collections.Generic;

namespace Alicloud.Apm
{
    ///
    /// SDK 入口
    ///
    public sealed class Apm
    {
        private static volatile IPlatformApm? _platformApm;
        private static volatile bool _started;
        private static readonly object _apmLock = new();
        private const int MaxCustomStringLength = 128;

        /// <summary>
        /// 指定选项启动 APM
        /// </summary>
        /// <param name="options">配置选项</param>
        public static void Start(ApmOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            ValidateOptions(options);

            lock (_apmLock)
            {
                if (_started)
                {
                    ApmLogger.Warning(
                        "Apm is already running and this call to Start() will be ignored"
                    );
                    return;
                }
                _platformApm = CreatePlatformApm();
                _platformApm.Start(options);
                Options = (ApmOptions)options.Clone();
                _started = true;
                InitComponents();
                var version = PackageUtility.ReadPackageVersion();
                ApmLogger.Notice($"Apm started. version={version}");
            }
        }

        /// <summary>
        /// 是否已启动
        /// </summary>
        public static bool IsStarted()
        {
            return _started;
        }

        /// <summary>
        /// 更新 userNick
        /// </summary>
        /// <param name="userNick">用户昵称</param>
        public static void SetUserNick(string userNick)
        {
            if (!TryGetPlatform(out var platform))
            {
                return;
            }
            if (!IsLengthValid("userNick", userNick))
            {
                return;
            }
            Options.UserNick = userNick;
            platform.SetUserNick(userNick);
        }

        /// <summary>
        /// 更新 userId
        /// </summary>
        /// <param name="userId">用户标识</param>
        public static void SetUserId(string userId)
        {
            if (!TryGetPlatform(out var platform))
            {
                return;
            }
            if (!IsLengthValid("userId", userId))
            {
                return;
            }
            Options.UserId = userId;
            platform.SetUserId(userId);
        }

        /// <summary>
        /// 设置自定义维度
        /// </summary>
        /// <param name="key">维度</param>
        /// <param name="value">值</param>
        public static void SetCustomKeyValue(string? key, object value)
        {
            if (!TryGetPlatform(out var platform))
            {
                return;
            }
            if (key == null)
            {
                return;
            }
            if (!IsLengthValid("key", key))
            {
                return;
            }
            platform.SetCustomKeyValue(key, value);
        }

        /// <summary>
        /// 批量设置自定义维度
        /// </summary>
        /// <param name="keysAndValues">键值对</param>
        public static void SetCustomKeysAndValues(IDictionary<string, object>? keysAndValues)
        {
            if (!TryGetPlatform(out var platform))
            {
                return;
            }
            if (keysAndValues == null || keysAndValues.Count == 0)
            {
                return;
            }
            foreach (var pair in keysAndValues)
            {
                var key = pair.Key;
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                if (!IsLengthValid("key", key))
                {
                    continue;
                }
                platform.SetCustomKeyValue(key, pair.Value);
            }
        }

        /// <summary>
        /// APM 配置选项
        /// </summary>
        public static ApmOptions Options { get; private set; } = new ApmOptions();

        private static IPlatformApm CreatePlatformApm()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new PlatformApmIos();
#elif UNITY_ANDROID && !UNITY_EDITOR
            return new PlatformApmAndroid();
#else
            return new PlatformApmFallback();
#endif
        }

        private static void ValidateOptions(ApmOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.AppKey))
            {
                throw new ArgumentException(
                    "AppKey cannot be null or empty.",
                    nameof(options.AppKey)
                );
            }
            if (string.IsNullOrWhiteSpace(options.AppSecret))
            {
                throw new ArgumentException(
                    "AppSecret cannot be null or empty.",
                    nameof(options.AppSecret)
                );
            }
            if (string.IsNullOrWhiteSpace(options.AppRsaSecret))
            {
                throw new ArgumentException(
                    "AppRsaSecret cannot be null or empty.",
                    nameof(options.AppRsaSecret)
                );
            }
            options.UserNick = EnsureMaxLength("userNick", options.UserNick);
            options.UserId = EnsureMaxLength("userId", options.UserId);
        }

        private static void InitComponents()
        {
            if (Options.SdkComponents == 0)
            {
                ApmLogger.Warning("ApmOptions has no SdkComponents.");
                return;
            }

            // 初始化通过组件注册表注册的组件
            InitializeRegisteredComponents();
        }

        /// <summary>
        /// 初始化通过组件注册表注册的组件
        /// </summary>
        private static void InitializeRegisteredComponents()
        {
            var processedComponents = new HashSet<SDKComponents>();

            foreach (SDKComponents component in Enum.GetValues(typeof(SDKComponents)))
            {
                if (!processedComponents.Add(component))
                {
                    continue;
                }
                if (component == SDKComponents.None)
                {
                    continue;
                }
                if (!HasSingleFlag(component))
                {
                    continue;
                }
                if ((Options.SdkComponents & component) == 0)
                {
                    continue;
                }

                if (!ApmComponentRegistry.InitializeComponent(component))
                {
                    ApmLogger.Warning($"Component {component} failed to initialize");
                }
            }
        }

        private static bool HasSingleFlag(SDKComponents component)
        {
            var value = (int)component;
            return value != 0 && (value & (value - 1)) == 0;
        }

        private static bool TryGetPlatform(out IPlatformApm platform)
        {
            if (_started && _platformApm != null)
            {
                platform = _platformApm;
                return true;
            }

            ApmLogger.Warning("Apm not started. Call Apm.Start() first.");
            platform = null!;
            return false;
        }

        private static bool IsLengthValid(string fieldName, string? value)
        {
            if (value != null && value.Length > MaxCustomStringLength)
            {
                ApmLogger.Error(
                    $"{fieldName}={value} (length={value.Length}), maximum allowed length is {MaxCustomStringLength}"
                );
                return false;
            }
            return true;
        }

        private static string EnsureMaxLength(string fieldName, string? value)
        {
            var v = value ?? string.Empty;
            if (v.Length > MaxCustomStringLength)
            {
                ApmLogger.Error(
                    $"{fieldName}={v} (length={v.Length}), maximum allowed length is {MaxCustomStringLength}"
                );
                return v[..MaxCustomStringLength];
            }
            return v;
        }
    }
}
