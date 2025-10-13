using System.Collections.Generic;

namespace Alicloud.Apm
{
    internal sealed class PlatformApmFallback : IPlatformApm
    {
        public void Start(ApmOptions options)
        {
            if (options != null)
            {
                ApmLogger.Info(
                    $"[Fallback] Start: AppKey={options.AppKey}, Components={options.SdkComponents}"
                );
            }
        }

        public void SetUserNick(string userNick)
        {
            if (!string.IsNullOrEmpty(userNick))
            {
                ApmLogger.Info($"[Fallback] SetUserNick: {userNick}");
            }
        }

        public void SetUserId(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                ApmLogger.Info($"[Fallback] SetUserId: {userId}");
            }
        }

        public void SetCustomKeyValue(string key, object value)
        {
            if (key != null && value != null)
            {
                ApmLogger.Info($"[Fallback] SetCustomKeyValue: {key} = {value}");
            }
        }

        public void SetCustomKeysAndValues(IDictionary<string, object> keysAndValues)
        {
            if (keysAndValues != null && keysAndValues.Count > 0)
            {
                var logBuilder = new System.Text.StringBuilder();
                logBuilder.AppendLine(
                    $"[Fallback] SetCustomKeysAndValues: {keysAndValues.Count} pairs"
                );
                foreach (var pair in keysAndValues)
                {
                    logBuilder.AppendLine($"  {pair.Key} = {pair.Value}");
                }
                ApmLogger.Info(logBuilder.ToString().TrimEnd());
            }
        }
    }
}
