using System.Runtime.InteropServices;

namespace Alicloud.Apm
{
#if UNITY_IOS && !UNITY_EDITOR
    internal class PlatformApmConfigurationIos : IPlatformApmConfiguration
    {
        [DllImport(IosNativeLibrary.LibraryName)]
        private static extern void eapm_set_logger_level(int loggerLevel);

        [DllImport(IosNativeLibrary.LibraryName)]
        private static extern void eapm_set_environment(int environment);

        public void SetLoggerLevel(ApmLoggerLevel loggerLevel)
        {
            eapm_set_logger_level((int)loggerLevel);
        }

        public void SetEnvironment(ApmEnvironment environment)
        {
            eapm_set_environment((int)environment);
        }
    }
#endif
}
