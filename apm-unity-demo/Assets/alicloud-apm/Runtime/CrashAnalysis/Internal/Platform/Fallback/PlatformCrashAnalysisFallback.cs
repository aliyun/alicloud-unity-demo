namespace Alicloud.Apm.CrashAnalysis
{
    internal sealed class PlatformCrashAnalysisFallback : IPlatformCrashAnalysis
    {
        public void Log(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                CrashAnalysisLogger.Info($"[Fallback] Log: {message}");
            }
        }

        public void RecordExceptionModel(InternalExceptionModel exceptionModel)
        {
            if (exceptionModel == null)
            {
                return;
            }

            var logBuilder = new System.Text.StringBuilder();
            logBuilder.AppendLine($"[Fallback] Exception recorded: {exceptionModel.Name}");
            logBuilder.AppendLine($"  Reason: {exceptionModel.Reason}");
            logBuilder.AppendLine($"  Language: {exceptionModel.Language}");
            logBuilder.AppendLine(
                $"  Custom: {exceptionModel.Custom}, Urgent: {exceptionModel.Urgent}, QuitApp: {exceptionModel.QuitApp}"
            );
            logBuilder.AppendLine(
                $"  Timestamp: {exceptionModel.Timestamp:yyyy-MM-dd HH:mm:ss} UTC"
            );

            if (exceptionModel.StackTrace != null && exceptionModel.StackTrace.Count > 0)
            {
                logBuilder.AppendLine("  Stack trace:");
                for (int i = 0; i < exceptionModel.StackTrace.Count; i++)
                {
                    var frame = exceptionModel.StackTrace[i];
                    var frameInfo = $"    {i}: {frame.Symbol}";
                    if (!string.IsNullOrEmpty(frame.File))
                    {
                        frameInfo += $" ({frame.File}:{frame.Line})";
                    }
                    if (!string.IsNullOrEmpty(frame.Library))
                    {
                        frameInfo += $" [{frame.Library}]";
                    }
                    logBuilder.AppendLine(frameInfo);
                }
            }

            CrashAnalysisLogger.Error(logBuilder.ToString().TrimEnd());

            // If policy requires quitting the app, do it immediately in fallback as well
            if (exceptionModel.QuitApp)
            {
                CrashAnalysisLogger.Error(
                    "[Fallback] QuitApp=true, terminating application per policy"
                );
#if UNITY_EDITOR
                // Stop play mode in editor
                UnityEditor.EditorApplication.isPlaying = false;
#else
                try
                {
                    UnityEngine.Application.Quit();
                }
                catch { }
                try
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                catch { }
#endif
            }
        }
    }
}
