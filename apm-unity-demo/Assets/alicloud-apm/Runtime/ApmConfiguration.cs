#nullable enable

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

        internal static IPlatformApmConfiguration PlatformApmConfiguration
        {
            get
            {
                if (_platformApmConfiguration != null)
                {
                    return _platformApmConfiguration;
                }
                lock (_apmConfigurationLock)
                {
#if UNITY_IOS && !UNITY_EDITOR
                    return _platformApmConfiguration ??= new PlatformApmConfigurationIos();
#elif UNITY_ANDROID && !UNITY_EDITOR
                    return _platformApmConfiguration ??= new PlatformApmConfigurationAndroid();
#else
                    return _platformApmConfiguration ??= new PlatformApmConfigurationFallback();
#endif
                }
            }
        }

        private static IPlatformApmConfiguration? _platformApmConfiguration;
        private static readonly object _apmConfigurationLock = new object();
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
