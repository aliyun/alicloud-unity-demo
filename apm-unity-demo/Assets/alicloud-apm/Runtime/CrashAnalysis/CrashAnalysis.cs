#nullable enable

using System;
using System.Threading;

namespace Alicloud.Apm.CrashAnalysis
{
    public static class CrashAnalysis
    {
        private static readonly object _recordExceptionLock = new();

        public static void Log(string message)
        {
            PlatformCrashAnalysis.Log(message);
        }

        /// <summary>
        /// 记录自定义异常
        /// </summary>
        /// <param name="exception">要记录的异常对象</param>
        public static void RecordException(Exception exception)
        {
            lock (_recordExceptionLock)
            {
                if (exception == null)
                {
                    CrashAnalysisLogger.Warning(
                        "RecordException called with null exception, ignoring"
                    );
                    return;
                }

                if (CrashAnalysisConfiguration.ExceptionMode == ExceptionPolicy.Disabled)
                {
                    CrashAnalysisLogger.DebugLog(
                        "Exception recording is disabled, ignoring exception: "
                            + exception.GetType().Name
                    );
                    return;
                }

                try
                {
                    var model = ExceptionConverter.FromException(exception);
                    model.Custom = true;
                    ExceptionPolicyHandler.ApplyReportingStrategy(
                        model,
                        CrashAnalysisConfiguration.ExceptionMode
                    );
                    PlatformCrashAnalysis.RecordExceptionModel(model);

                    CrashAnalysisLogger.DebugLog(
                        $"Successfully recorded custom exception: {exception.GetType().Name}"
                    );
                }
                catch (Exception ex)
                {
                    CrashAnalysisLogger.Error(
                        $"Failed to record exception {exception.GetType().Name}: {ex.Message}"
                    );
                }
            }
        }

        /// <summary>
        /// 记录自定义异常模型，支持多语言异常
        /// </summary>
        /// <param name="exceptionModel">要记录的异常模型</param>
        public static void RecordExceptionModel(ExceptionModel exceptionModel)
        {
            lock (_recordExceptionLock)
            {
                if (exceptionModel == null)
                {
                    CrashAnalysisLogger.Warning(
                        "RecordExceptionModel called with null exceptionModel, ignoring"
                    );
                    return;
                }

                if (string.IsNullOrEmpty(exceptionModel.Name))
                {
                    CrashAnalysisLogger.Warning(
                        "RecordExceptionModel called with empty exception name, ignoring"
                    );
                    return;
                }

                if (string.IsNullOrEmpty(exceptionModel.Reason))
                {
                    CrashAnalysisLogger.Warning(
                        "RecordExceptionModel called with empty exception reason, ignoring"
                    );
                    return;
                }

                if (CrashAnalysisConfiguration.ExceptionMode == ExceptionPolicy.Disabled)
                {
                    CrashAnalysisLogger.DebugLog(
                        $"Exception recording is disabled, ignoring exception: {exceptionModel.Name}"
                    );
                    return;
                }

                try
                {
                    var model = ExceptionConverter.FromPublicModel(exceptionModel);
                    model.Custom = true;
                    ExceptionPolicyHandler.ApplyReportingStrategy(
                        model,
                        CrashAnalysisConfiguration.ExceptionMode
                    );
                    PlatformCrashAnalysis.RecordExceptionModel(model);

                    CrashAnalysisLogger.DebugLog(
                        $"Successfully recorded custom exception model: {exceptionModel.Name} (Language: {exceptionModel.Language})"
                    );
                }
                catch (Exception ex)
                {
                    CrashAnalysisLogger.Error(
                        $"Failed to record exception model {exceptionModel.Name}: {ex.Message}"
                    );
                }
            }
        }

        internal static IPlatformCrashAnalysis PlatformCrashAnalysis =>
            _platformCrashAnalysis.Value;

        private static IPlatformCrashAnalysis CreatePlatformCrashAnalysis()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new PlatformCrashAnalysisIos();
#elif UNITY_ANDROID && !UNITY_EDITOR
            return new PlatformCrashAnalysisAndroid();
#else
            return new PlatformCrashAnalysisFallback();
#endif
        }

        private static readonly Lazy<IPlatformCrashAnalysis> _platformCrashAnalysis = new(
            CreatePlatformCrashAnalysis,
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }
}
