#nullable enable

using System;
using System.Threading;

namespace Alicloud.Apm
{
    /// <summary>
    /// 全局属性配置
    /// </summary>
    public static class ApmConfiguration
    {
        /// <summary>
        /// 获取或设置 EAPM 内部的日志级别。
        /// <para>默认级别是 <see cref="ApmLoggerLevel.Notice"/>。</para>
        /// </summary>
        public static ApmLoggerLevel LoggerLevel
        {
            get => _loggerLevel;
            set
            {
                _loggerLevel = value;
                PlatformApmConfiguration.SetLoggerLevel(value);
            }
        }

        private static ApmLoggerLevel _loggerLevel = ApmLoggerLevel.Notice;

        internal static IPlatformApmConfiguration PlatformApmConfiguration =>
            _platformApmConfiguration.Value;

        private static IPlatformApmConfiguration CreatePlatformApmConfiguration()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new PlatformApmConfigurationIos();
#elif UNITY_ANDROID && !UNITY_EDITOR
            return new PlatformApmConfigurationAndroid();
#else
            return new PlatformApmConfigurationFallback();
#endif
        }

        private static readonly Lazy<IPlatformApmConfiguration> _platformApmConfiguration = new(
            CreatePlatformApmConfiguration,
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    public enum ApmLoggerLevel : int
    {
        Error = 3,
        Warning = 4,
        Notice = 5,
        Info = 6,
        Debug = 7,
    }
}
