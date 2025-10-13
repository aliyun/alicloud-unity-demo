namespace Alicloud.Apm.CrashAnalysis
{
    internal static class CrashAnalysisLogger
    {
        private const string Prefix = "[crashAnalysis] ";

        public static void Error(string message)
        {
            ApmLogger.Error(Prefix + message);
        }

        public static void Warning(string message)
        {
            ApmLogger.Warning(Prefix + message);
        }

        public static void Notice(string message)
        {
            ApmLogger.Notice(Prefix + message);
        }

        public static void Info(string message)
        {
            ApmLogger.Info(Prefix + message);
        }

        public static void DebugLog(string message)
        {
            ApmLogger.DebugLog(Prefix + message);
        }
    }
}
