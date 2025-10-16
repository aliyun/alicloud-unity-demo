using System;
using UnityEngine;

namespace Alicloud.Apm.CrashAnalysis
{
#if UNITY_ANDROID && !UNITY_EDITOR
    internal sealed class PlatformCrashAnalysisAndroid : IPlatformCrashAnalysis
    {
        public static string APM_CRASH_ANALYSIS = "com.aliyun.emas.apm.crash.ApmCrashAnalysis";

        public PlatformCrashAnalysisAndroid() { }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                using (
                    AndroidJavaClass apmCrashAnalysisClass = new AndroidJavaClass(APM_CRASH_ANALYSIS)
                )
                {
                    using (
                        AndroidJavaObject apmCrashAnalysisInstance =
                            apmCrashAnalysisClass.CallStatic<AndroidJavaObject>("getInstance")
                    )
                    {
                        apmCrashAnalysisInstance.Call("log", message);
                    }
                }
            }
            catch (Exception ex)
            {
                CrashAnalysisLogger.Error(
                    $"Failed to log message to android: {message}. Exception: {ex}"
                );
            }
        }

        public void RecordExceptionModel(InternalExceptionModel exceptionModel)
        {
            if (exceptionModel == null)
            {
                return;
            }

            try
            {
                using (
                    AndroidJavaObject optionsBuilder = new AndroidJavaObject(
                        "com.aliyun.emas.apm.crash.ExceptionModel$Builder"
                    )
                )
                {
                    optionsBuilder
                        .Call<AndroidJavaObject>("setName", exceptionModel.Name)
                        .Call<AndroidJavaObject>("setReason", exceptionModel.Reason)
                        .Call<AndroidJavaObject>("setCustom", exceptionModel.Custom)
                        .Call<AndroidJavaObject>("setUrgent", exceptionModel.Urgent)
                        .Call<AndroidJavaObject>("setQuitApp", exceptionModel.QuitApp);
                    //build android language
                    using (
                        AndroidJavaClass languageClass = new AndroidJavaClass(
                            "com.aliyun.emas.apm.crash.SourceLanguage"
                        )
                    )
                    {
                        using (
                            AndroidJavaObject androidLanguage = GetAndroidLanguage(
                                languageClass,
                                exceptionModel.Language
                            )
                        )
                        {
                            optionsBuilder.Call<AndroidJavaObject>(
                                "setSourceLanguage",
                                androidLanguage
                            );
                        }
                    }

                    if (exceptionModel.StackTrace != null && exceptionModel.StackTrace.Count > 0)
                    {
                        //build android stack trace
                        using (
                            AndroidJavaObject stackList = new AndroidJavaObject("java.util.ArrayList")
                        )
                        {
                            for (int i = 0; i < exceptionModel.StackTrace.Count; i++)
                            {
                                var frame = exceptionModel.StackTrace[i];
                                using (
                                    AndroidJavaObject androidStackFrameBuilder = new AndroidJavaObject(
                                        "com.aliyun.emas.apm.crash.StackFrame$Builder"
                                    )
                                )
                                {
                                    androidStackFrameBuilder
                                        .Call<AndroidJavaObject>(
                                            "setSymbol",
                                            frame.Symbol ?? string.Empty
                                        )
                                        .Call<AndroidJavaObject>("setFile", frame.File ?? string.Empty)
                                        .Call<AndroidJavaObject>("setLine", frame.Line)
                                        .Call<AndroidJavaObject>(
                                            "setLibrary",
                                            frame.Library ?? string.Empty
                                        )
                                        .Call<AndroidJavaObject>("setAddress", ((long?)frame.Address) ?? 0);

                                    using (
                                        AndroidJavaObject androidStackFrame = androidStackFrameBuilder.Call<
                                            AndroidJavaObject
                                        >("build")
                                    )
                                    {
                                        stackList.Call<bool>("add", androidStackFrame);
                                    }
                                }
                            }

                            optionsBuilder.Call<AndroidJavaObject>("setStackTrace", stackList);
                        }
                    }

                    using (
                        AndroidJavaObject androidExceptionModel = optionsBuilder.Call<AndroidJavaObject>(
                            "build"
                        )
                    )
                    {
                        using (
                            AndroidJavaClass apmCrashAnalysisClass = new AndroidJavaClass(
                                APM_CRASH_ANALYSIS
                            )
                        )
                        {
                            using (
                                AndroidJavaObject apmCrashAnalysisInstance =
                                    apmCrashAnalysisClass.CallStatic<AndroidJavaObject>("getInstance")
                            )
                            {
                                apmCrashAnalysisInstance.Call("recordException", androidExceptionModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CrashAnalysisLogger.Error(
                    $"Failed to record exception to android: {exceptionModel.Name} - {exceptionModel.Reason}. Exception: {ex}"
                );
            }
        }

        private static AndroidJavaObject GetAndroidLanguage(
            AndroidJavaClass languageClass,
            SourceLanguage language
        )
        {
            return language switch
            {
                SourceLanguage.CSharp => languageClass.GetStatic<AndroidJavaObject>("CSharp"),
                SourceLanguage.Lua => languageClass.GetStatic<AndroidJavaObject>("Lua"),
                _ => languageClass.GetStatic<AndroidJavaObject>("Unknown"),
            };
        }
    }
#endif
}
