using UnityEngine;

namespace Alicloud.Apm
{
    internal static class ApmLogger
    {
        public const string Prefix = "[alicloud-apm] ";

        public static void Error(string message)
        {
            if ((int)ApmConfiguration.LoggerLevel >= (int)ApmLoggerLevel.Error)
            {
                Debug.LogError(Prefix + message);
            }
        }

        public static void Warning(string message)
        {
            if ((int)ApmConfiguration.LoggerLevel >= (int)ApmLoggerLevel.Warning)
            {
                Debug.LogWarning(Prefix + message);
            }
        }

        public static void Notice(string message)
        {
            if ((int)ApmConfiguration.LoggerLevel >= (int)ApmLoggerLevel.Notice)
            {
                Debug.Log(Prefix + message);
            }
        }

        public static void Info(string message)
        {
            if ((int)ApmConfiguration.LoggerLevel >= (int)ApmLoggerLevel.Info)
            {
                Debug.Log(Prefix + message);
            }
        }

        public static void DebugLog(string message)
        {
            if ((int)ApmConfiguration.LoggerLevel >= (int)ApmLoggerLevel.Debug)
            {
                Debug.Log(Prefix + message);
            }
        }
    }
}



