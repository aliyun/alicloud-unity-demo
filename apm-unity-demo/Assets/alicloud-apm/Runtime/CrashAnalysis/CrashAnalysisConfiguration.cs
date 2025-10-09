using UnityEngine;

namespace Alicloud.Apm.CrashAnalysis
{
    public static class CrashAnalysisConfiguration
    {
        private static LogType _reportLogLevel = LogType.Exception;
        private static ExceptionPolicy _exceptionMode = ExceptionPolicy.ReportImmediately;

        /// <summary>
        /// 获取或设置异常上报的最低日志级别
        /// </summary>
        public static LogType ReportLogLevel
        {
            get => _reportLogLevel;
            set
            {
                if (_reportLogLevel != value)
                {
                    var oldValue = _reportLogLevel;
                    _reportLogLevel = value;
                    CrashAnalysisLogger.Info($"ReportLogLevel changed from {oldValue} to {value}");
                }
            }
        }

        /// <summary>
        /// 获取或设置异常处理策略
        /// </summary>
        public static ExceptionPolicy ExceptionMode
        {
            get => _exceptionMode;
            set
            {
                if (_exceptionMode != value)
                {
                    var oldValue = _exceptionMode;
                    _exceptionMode = value;
                    CrashAnalysisLogger.Info($"ExceptionMode changed from {oldValue} to {value}");
                }
            }
        }

        /// <summary>
        /// 重置配置为默认值
        /// </summary>
        public static void ResetToDefaults()
        {
            ReportLogLevel = LogType.Exception;
            ExceptionMode = ExceptionPolicy.ReportImmediately;
            CrashAnalysisLogger.Info("CrashAnalysis configuration reset to defaults");
        }
    }

    public enum ExceptionPolicy
    {
        Disabled = 0,
        Record = 1,
        ReportImmediately = 2,
        ReportImmediatelyAndQuitApplication = 3,
    }
}
