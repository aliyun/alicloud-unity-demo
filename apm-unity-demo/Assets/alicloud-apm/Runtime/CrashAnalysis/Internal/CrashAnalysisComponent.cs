namespace Alicloud.Apm.CrashAnalysis
{
    /// <summary>
    /// CrashAnalysis组件实现
    /// </summary>
    internal class CrashAnalysisComponent : IApmComponent
    {
        private bool _initialized;
        private readonly object _lock = new();

        public SDKComponents ComponentType => SDKComponents.CrashAnalysis;
        public bool IsInitialized => _initialized;

        public void Initialize()
        {
            lock (_lock)
            {
                if (_initialized)
                {
                    return;
                }

                _initialized = true;
                ExceptionHandler.Register();
            }

            CrashAnalysisLogger.Notice("CrashAnalysis Started");
        }
    }
}
