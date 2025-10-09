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
                AndroidJavaClass apmCrashAnalysisClass = new AndroidJavaClass(APM_CRASH_ANALYSIS);
                AndroidJavaObject apmCrashAnalysisInstance =
                    apmCrashAnalysisClass.CallStatic<AndroidJavaObject>("getInstance");

                apmCrashAnalysisInstance.Call("log", message);
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
                AndroidJavaObject optionsBuilder = new AndroidJavaObject(
                    "com.aliyun.emas.apm.crash.ExceptionModel$Builder"
                );

                optionsBuilder
                    .Call<AndroidJavaObject>("setName", exceptionModel.Name)
                    .Call<AndroidJavaObject>("setReason", exceptionModel.Reason)
                    .Call<AndroidJavaObject>("setCustom", exceptionModel.Custom)
                    .Call<AndroidJavaObject>("setUrgent", exceptionModel.Urgent)
                    .Call<AndroidJavaObject>("setQuitApp", exceptionModel.QuitApp);
                //build android language
                AndroidJavaObject androidLanguage = null;
                AndroidJavaClass languageClass = new AndroidJavaClass(
                    "com.aliyun.emas.apm.crash.SourceLanguage"
                );
                if (exceptionModel.Language == SourceLanguage.CSharp)
                {
                    androidLanguage = languageClass.GetStatic<AndroidJavaObject>("CSharp");
                }
                else if (exceptionModel.Language == SourceLanguage.Lua)
                {
                    androidLanguage = languageClass.GetStatic<AndroidJavaObject>("Lua");
                }
                else
                {
                    androidLanguage = languageClass.GetStatic<AndroidJavaObject>("Unknown");
                }
                optionsBuilder.Call<AndroidJavaObject>("setSourceLanguage", androidLanguage);

                if (exceptionModel.StackTrace != null && exceptionModel.StackTrace.Count > 0)
                {
                    //build android stack trace
                    AndroidJavaObject stackList = new AndroidJavaObject("java.util.ArrayList");
                    for (int i = 0; i < exceptionModel.StackTrace.Count; i++)
                    {
                        AndroidJavaObject androidStackFrameBuilder = new AndroidJavaObject(
                            "com.aliyun.emas.apm.crash.StackFrame$Builder"
                        );
                        androidStackFrameBuilder
                            .Call<AndroidJavaObject>(
                                "setSymbol",
                                exceptionModel.StackTrace[i].Symbol ?? string.Empty
                            )
                            .Call<AndroidJavaObject>(
                                "setFile",
                                exceptionModel.StackTrace[i].File ?? string.Empty
                            )
                            .Call<AndroidJavaObject>("setLine", exceptionModel.StackTrace[i].Line)
                            .Call<AndroidJavaObject>(
                                "setLibrary",
                                exceptionModel.StackTrace[i].Library ?? string.Empty
                            )
                            .Call<AndroidJavaObject>(
                                "setAddress",
                                ((long?)exceptionModel.StackTrace[i].Address) ?? 0
                            );

                        AndroidJavaObject androidStackFrame =
                            androidStackFrameBuilder.Call<AndroidJavaObject>("build");
                        stackList.Call<bool>("add", androidStackFrame);
                    }

                    optionsBuilder.Call<AndroidJavaObject>("setStackTrace", stackList);
                }
                AndroidJavaObject androidExceptionModel = optionsBuilder.Call<AndroidJavaObject>(
                    "build"
                );

                AndroidJavaClass apmCrashAnalysisClass = new AndroidJavaClass(APM_CRASH_ANALYSIS);
                AndroidJavaObject apmCrashAnalysisInstance =
                    apmCrashAnalysisClass.CallStatic<AndroidJavaObject>("getInstance");

                apmCrashAnalysisInstance.Call("recordException", androidExceptionModel);
            }
            catch (Exception ex)
            {
                CrashAnalysisLogger.Error(
                    $"Failed to record exception to android: {exceptionModel.Name} - {exceptionModel.Reason}. Exception: {ex}"
                );
            }
        }
    }
#endif
}
