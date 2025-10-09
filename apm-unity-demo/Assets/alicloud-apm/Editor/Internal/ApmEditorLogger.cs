#if UNITY_EDITOR
using UnityEngine;

namespace Alicloud.Apm.Editor
{
    internal static class ApmEditorLogger
    {
        private const string Prefix = "[AlicloudApm.Editor] ";

        public static void Info(string message)
        {
            Debug.Log(Prefix + message);
        }

        public static void Warning(string message)
        {
            Debug.LogWarning(Prefix + message);
        }

        public static void Error(string message)
        {
            Debug.LogError(Prefix + message);
        }

        public static void Exception(System.Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
#endif
