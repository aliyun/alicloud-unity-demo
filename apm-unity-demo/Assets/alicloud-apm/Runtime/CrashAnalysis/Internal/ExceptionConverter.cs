using System;
using UnityEngine;

namespace Alicloud.Apm.CrashAnalysis
{
    internal static class ExceptionConverter
    {
        public static InternalExceptionModel FromException(
            Exception exception,
            int? threadId = null
        )
        {
            var model = new InternalExceptionModel(
                exception.GetType().Name,
                exception.Message,
                SourceLanguage.CSharp
            );

            var frames = new System.Diagnostics.StackTrace(exception, true).GetFrames();
            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    var symbol =
                        method != null ? method.DeclaringType?.FullName + "." + method.Name : null;
                    var file = frame.GetFileName() ?? string.Empty;
                    var line = frame.GetFileLineNumber();
                    model.StackTrace.Add(
                        StackFrame.FromSymbol(symbol ?? string.Empty, file, line, string.Empty)
                    );
                }
            }

            if (threadId.HasValue)
            {
                model.ThreadId = threadId.Value;
            }

            return model;
        }

        public static InternalExceptionModel FromPublicModel(ExceptionModel exceptionModel)
        {
            var model = new InternalExceptionModel(
                exceptionModel.Name,
                exceptionModel.Reason,
                exceptionModel.Language
            );
            foreach (var sf in exceptionModel.StackTrace)
            {
                model.StackTrace.Add(
                    StackFrame.FromSymbol(
                        sf.Symbol ?? string.Empty,
                        sf.File ?? string.Empty,
                        sf.Line,
                        sf.Library ?? string.Empty
                    )
                );
            }
            return model;
        }

        public static InternalExceptionModel FromUnityLog(
            string condition,
            string stackTrace,
            LogType type,
            int? threadId = null
        )
        {
            // Normalize Unity condition like "NullReferenceException: message" so it matches .NET exception shape
            NormalizeUnityCondition(condition, type, out var name, out var reason);
            var model = new InternalExceptionModel(name, reason, SourceLanguage.CSharp)
            {
                Custom = false, // SDK internal automatic capture
            };

            // Use the improved stack frame parser
            var frames = StackFrameParser.ParseUnityStackTrace(stackTrace);
            foreach (var frame in frames)
            {
                model.StackTrace.Add(frame);
            }

            if (threadId.HasValue)
            {
                model.ThreadId = threadId.Value;
            }

            return model;
        }

        /// <summary>
        /// Unity passes condition strings that often contain a type prefix, e.g.
        /// "NullReferenceException: ..." or "Unhandled Exception: System.InvalidOperationException: ...".
        /// This method extracts the exception type and message so signatures match AppDomain exceptions.
        /// </summary>
        private static void NormalizeUnityCondition(
            string condition,
            LogType type,
            out string name,
            out string reason
        )
        {
            name = type.ToString();
            reason = condition ?? string.Empty;
            if (string.IsNullOrEmpty(reason))
            {
                return;
            }

            var text = reason.Trim();
            const string unhandledPrefix = "Unhandled Exception:";
            if (text.StartsWith(unhandledPrefix, StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(unhandledPrefix.Length).TrimStart();
            }

            int idx = text.IndexOf(':');
            if (idx <= 0)
            {
                return;
            }

            var typeCandidate = text.Substring(0, idx).Trim();
            var msg = text.Substring(idx + 1).Trim();

            // Heuristics: real exception types usually end with "Exception" or "Error" or contain dots/namespaces.
            bool looksLikeType =
                typeCandidate.EndsWith("Exception", StringComparison.Ordinal)
                || typeCandidate.EndsWith("Error", StringComparison.Ordinal)
                || typeCandidate.Contains(".");
            if (looksLikeType)
            {
                name = typeCandidate;
                reason = msg;
            }
        }
    }
}
