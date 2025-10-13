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
                AndroidJavaClass apmClass = new AndroidJavaClass(PlatformApmAndroid.APM);
                AndroidJavaClass loggerLevelClass = new AndroidJavaClass(
                    "com.aliyun.emas.apm.logger.LoggerLevel"
                );

                AndroidJavaObject apmLoggerLevel = null;
                if (loggerLevel == ApmLoggerLevel.Error)
                {
                    apmLoggerLevel = loggerLevelClass.GetStatic<AndroidJavaObject>("ERROR");
                }
                else if (loggerLevel == ApmLoggerLevel.Info || loggerLevel == ApmLoggerLevel.Notice)
                {
                    apmLoggerLevel = loggerLevelClass.GetStatic<AndroidJavaObject>("INFO");
                }
                else if (loggerLevel == ApmLoggerLevel.Warning)
                {
                    apmLoggerLevel = loggerLevelClass.GetStatic<AndroidJavaObject>("WARN");
                }
                else
                {
                    apmLoggerLevel = loggerLevelClass.GetStatic<AndroidJavaObject>("DEBUG");
                }

                apmClass.CallStatic("setLoggerLevel", apmLoggerLevel);
            }
            catch
            {
                ApmLogger.Error($"Failed to set logger level: {loggerLevel.ToString()}");
            }
        }
    }
#endif
}
