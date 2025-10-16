using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alicloud.Apm
{
#if UNITY_ANDROID && !UNITY_EDITOR
    internal class PlatformApmConfigurationAndroid : IPlatformApmConfiguration
    {
        public void SetEnvironment(ApmEnvironment environment)
        {
            //android not support
        }

        public void SetLoggerLevel(ApmLoggerLevel loggerLevel)
        {
            try
            {
                using (AndroidJavaClass apmClass = new AndroidJavaClass(PlatformApmAndroid.APM))
                {
                    using (
                        AndroidJavaClass loggerLevelClass = new AndroidJavaClass(
                            "com.aliyun.emas.apm.logger.LoggerLevel"
                        )
                    )
                    {
                        using (
                            AndroidJavaObject apmLoggerLevel = GetAndroidLoggerLevel(
                                loggerLevelClass,
                                loggerLevel
                            )
                        )
                        {
                            apmClass.CallStatic("setLoggerLevel", apmLoggerLevel);
                        }
                    }
                }
            }
            catch
            {
                ApmLogger.Error($"Failed to set logger level: {loggerLevel.ToString()}");
            }
        }

        private static AndroidJavaObject GetAndroidLoggerLevel(
            AndroidJavaClass loggerLevelClass,
            ApmLoggerLevel loggerLevel
        )
        {
            string fieldName;
            if (loggerLevel == ApmLoggerLevel.Error)
            {
                fieldName = "ERROR";
            }
            else if (loggerLevel == ApmLoggerLevel.Info || loggerLevel == ApmLoggerLevel.Notice)
            {
                fieldName = "INFO";
            }
            else if (loggerLevel == ApmLoggerLevel.Warning)
            {
                fieldName = "WARN";
            }
            else
            {
                fieldName = "DEBUG";
            }

            return loggerLevelClass.GetStatic<AndroidJavaObject>(fieldName);
        }
    }
#endif
}
