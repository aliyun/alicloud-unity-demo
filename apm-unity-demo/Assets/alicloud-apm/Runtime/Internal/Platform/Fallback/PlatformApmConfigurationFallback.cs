namespace Alicloud.Apm
{
    internal class PlatformApmConfigurationFallback : IPlatformApmConfiguration
    {
        public void SetLoggerLevel(ApmLoggerLevel loggerLevel)
        {
            ApmLogger.Info($"[Fallback] SetLoggerLevel: {loggerLevel}");
        }

        public void SetEnvironment(ApmEnvironment environment)
        {
            ApmLogger.Info($"[Fallback] SetEnvironment: {environment}");
        }
    }
}
