using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Alicloud.Apm.CrashAnalysis
{
    internal static class ExceptionHandler
    {
        private static bool _registered;
        private static readonly object _registrationLock = new object();
        private static readonly object _unhandledExceptionLock = new object();

        // Prevent log-driven re-entry into handlers
        private static readonly ThreadLocal<int> _callbackReentrancyDepth = new ThreadLocal<int>(
            () =>
                0
        );

        public static void Register()
        {
            lock (_registrationLock)
            {
                if (_registered)
                {
                    return;
                }
                _registered = true;
                CrashAnalysisLogger.DebugLog("Registering CrashAnalysis exception handlers");

                try
                {
#if UNITY_5 || UNITY_5_3_OR_NEWER
                    Application.logMessageReceivedThreaded += OnThreadedLogCallback;
#else
                    if (
                        Application.unityVersion.StartsWith(
                            "4.",
                            System.StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        Application.RegisterLogCallbackThreaded(OnThreadedLogCallback);
                    }
                    else
                    {
                        Application.logMessageReceivedThreaded += OnThreadedLogCallback;
                    }
#endif
                    // .Net CLR
                    AppDomain.CurrentDomain.UnhandledException += OnUncaughtExceptionCallback;
                    TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

                    CrashAnalysisLogger.DebugLog(
                        "Successfully registered CrashAnalysis exception handlers"
                    );
                }
                catch (Exception ex)
                {
                    CrashAnalysisLogger.Error(
                        $"Failed to register exception handlers: {ex.Message}"
                    );
                    _registered = false; // Reset registration state on failure
                }
            }
        }

        private static void OnThreadedLogCallback(string condition, string stackTrace, LogType type)
        {
            var originatingThreadId = Thread.CurrentThread.ManagedThreadId;
            if (MainThreadHelper.IsMainThread)
            {
                OnMainThreadLogCallback(condition, stackTrace, type, originatingThreadId);
            }
            else
            {
                MainThreadDispatchBehaviour.Enqueue(() =>
                {
                    OnMainThreadLogCallback(condition, stackTrace, type, originatingThreadId);
                });
            }
        }

        private static void OnMainThreadLogCallback(
            string condition,
            string stackTrace,
            LogType type,
            int originatingThreadId
        )
        {
            if (!TryEnterCallbackProcessing())
            {
                return;
            }

            try
            {
                if (!ShouldReport(condition, type))
                {
                    return;
                }

                var model = ExceptionConverter.FromUnityLog(
                    condition,
                    stackTrace,
                    type,
                    originatingThreadId
                );
                ExceptionPolicyHandler.ApplyReportingStrategy(
                    model,
                    CrashAnalysisConfiguration.ExceptionMode
                );
                if (RecentExceptionDeduper.ShouldRecord(model))
                {
                    CrashAnalysis.PlatformCrashAnalysis.RecordExceptionModel(model);
                }
            }
            catch (Exception ex)
            {
                CrashAnalysisLogger.Error($"Error processing Unity log callback: {ex.Message}");
            }
            finally
            {
                ExitCallbackProcessing();
            }
        }

        private static void OnUncaughtExceptionCallback(
            object sender,
            UnhandledExceptionEventArgs e
        )
        {
            if (CrashAnalysisConfiguration.ExceptionMode == ExceptionPolicy.Disabled)
            {
                return;
            }
            if (e.ExceptionObject is Exception exObj)
            {
                var originatingThreadId = Thread.CurrentThread.ManagedThreadId;
                MainThreadDispatchBehaviour.Enqueue(() =>
                {
                    if (!TryEnterCallbackProcessing())
                    {
                        return;
                    }

                    try
                    {
                        var model = ExceptionConverter.FromException(exObj, originatingThreadId);
                        model.Custom = false;
                        ExceptionPolicyHandler.ApplyReportingStrategy(
                            model,
                            CrashAnalysisConfiguration.ExceptionMode
                        );
                        if (RecentExceptionDeduper.ShouldRecord(model))
                        {
                            CrashAnalysis.PlatformCrashAnalysis.RecordExceptionModel(model);
                        }
                    }
                    catch (Exception ex)
                    {
                        CrashAnalysisLogger.Error(
                            $"Error processing unhandled exception: {ex.Message}"
                        );
                    }
                    finally
                    {
                        ExitCallbackProcessing();
                    }
                });
            }
            else if (e.ExceptionObject != null)
            {
                var reason = e.ExceptionObject.ToString();
                var originatingThreadId = Thread.CurrentThread.ManagedThreadId;
                MainThreadDispatchBehaviour.Enqueue(() =>
                {
                    if (!TryEnterCallbackProcessing())
                    {
                        return;
                    }

                    try
                    {
                        var model = new InternalExceptionModel(
                            "UnhandledException",
                            reason,
                            SourceLanguage.CSharp
                        )
                        {
                            Custom = false,
                        };
                        model.ThreadId = originatingThreadId;
                        ExceptionPolicyHandler.ApplyReportingStrategy(
                            model,
                            CrashAnalysisConfiguration.ExceptionMode
                        );
                        if (RecentExceptionDeduper.ShouldRecord(model))
                        {
                            CrashAnalysis.PlatformCrashAnalysis.RecordExceptionModel(model);
                        }
                    }
                    catch (Exception ex)
                    {
                        CrashAnalysisLogger.Error(
                            $"Error processing unhandled exception string: {ex.Message}"
                        );
                    }
                    finally
                    {
                        ExitCallbackProcessing();
                    }
                });
            }
        }

        /// <summary>
        /// Unregister exception handlers for proper cleanup
        /// </summary>
        public static void Unregister()
        {
            lock (_registrationLock)
            {
                if (!_registered)
                {
                    return;
                }

                try
                {
#if UNITY_5 || UNITY_5_3_OR_NEWER
                    Application.logMessageReceivedThreaded -= OnThreadedLogCallback;
#else
                    Application.RegisterLogCallbackThreaded(null);
#endif
                    AppDomain.CurrentDomain.UnhandledException -= OnUncaughtExceptionCallback;
                    TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

                    _registered = false;
                    CrashAnalysisLogger.DebugLog(
                        "Successfully unregistered CrashAnalysis exception handlers"
                    );
                }
                catch (Exception ex)
                {
                    CrashAnalysisLogger.Error(
                        $"Failed to unregister exception handlers: {ex.Message}"
                    );
                }
            }
        }

        private static bool ShouldReport(string condition, LogType type)
        {
            if (CrashAnalysisConfiguration.ExceptionMode == ExceptionPolicy.Disabled)
            {
                return false;
            }

            if (!ShouldReport(type))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(condition) && condition.StartsWith(ApmLogger.Prefix))
            {
                return false;
            }

            return true;
        }

        private static bool ShouldReport(LogType type)
        {
            // Treat ReportLogLevel as the minimum level to report
            // Define a consistent severity ordering independent of enum numeric values
            int Sev(LogType t) =>
                t switch
                {
                    LogType.Log => 0,
                    LogType.Warning => 1,
                    LogType.Assert => 2,
                    LogType.Error => 3,
                    LogType.Exception => 4,
                    _ => 3,
                };

            var minLevel = CrashAnalysisConfiguration.ReportLogLevel;
            return Sev(type) >= Sev(minLevel);
        }

        private static void OnUnobservedTaskException(
            object sender,
            UnobservedTaskExceptionEventArgs e
        )
        {
            if (CrashAnalysisConfiguration.ExceptionMode == ExceptionPolicy.Disabled)
            {
                return;
            }

            if (e?.Exception == null)
            {
                return;
            }

            var originatingThreadId = Thread.CurrentThread.ManagedThreadId;
            var exceptions = e.Exception.Flatten().InnerExceptions;

            MainThreadDispatchBehaviour.Enqueue(() =>
            {
                if (!TryEnterCallbackProcessing())
                {
                    return;
                }

                try
                {
                    foreach (var ex in exceptions)
                    {
                        var model = ExceptionConverter.FromException(ex, originatingThreadId);
                        model.Custom = false;
                        ExceptionPolicyHandler.ApplyReportingStrategy(
                            model,
                            CrashAnalysisConfiguration.ExceptionMode
                        );
                        if (RecentExceptionDeduper.ShouldRecord(model))
                        {
                            CrashAnalysis.PlatformCrashAnalysis.RecordExceptionModel(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CrashAnalysisLogger.Error($"Error processing task exception: {ex.Message}");
                }
                finally
                {
                    ExitCallbackProcessing();
                }
            });

            // Prevent escalation to the runtime default handler
            e.SetObserved();
        }

        private static bool TryEnterCallbackProcessing()
        {
            if (_callbackReentrancyDepth.Value > 0)
            {
                return false;
            }

            _callbackReentrancyDepth.Value = 1;
            return true;
        }

        private static void ExitCallbackProcessing()
        {
            if (_callbackReentrancyDepth.Value > 0)
            {
                _callbackReentrancyDepth.Value = 0;
            }
        }
    }
}
