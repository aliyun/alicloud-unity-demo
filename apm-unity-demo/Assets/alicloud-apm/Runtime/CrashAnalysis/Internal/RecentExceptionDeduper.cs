using System;
using System.Collections.Generic;
using System.Text;

namespace Alicloud.Apm.CrashAnalysis
{
    /// <summary>
    /// Deduplicates exceptions reported through multiple channels (Unity log + UnhandledException + TaskScheduler events)
    /// within a short time window to avoid double-reporting.
    /// </summary>
    internal static class RecentExceptionDeduper
    {
        private static readonly Dictionary<string, double> _recent =
            new Dictionary<string, double>();
        private static readonly Queue<(string key, double ts)> _order =
            new Queue<(string, double)>();
        private static readonly Dictionary<string, FrequencyWindow> _frequency =
            new Dictionary<string, FrequencyWindow>();
        private static readonly object _lock = new object();
        private const double TtlSeconds = 1.0; // time window to treat duplicates as the same
        private const int MaxEntries = 1024; // hard cap to prevent unbounded growth
        private const int SignatureFrameDepth = 4; // 超过4帧之后，Application.logMessageReceivedThreaded捕获的堆栈会自带Unity RegisterUECatcher 桥接帧，如"UnityEngine.<>c:<RegisterUECatcher>b__0_0|UnhandledExceptionHandler.bindings.cs|46"

        public static bool ShouldRecord(InternalExceptionModel model)
        {
            if (model == null)
            {
                return false;
            }
            var sig = BuildSignature(model);
            var now = DateTime.UtcNow;
            var ts = now.Subtract(DateTime.UnixEpoch).TotalSeconds;
            lock (_lock)
            {
                Cleanup(ts);
                var frequencyWindow = UpdateFrequency(sig, ts);

                if (_recent.TryGetValue(sig, out var lastTs))
                {
                    if (ts - lastTs <= TtlSeconds)
                    {
                        // duplicate
                        LogFrequency("dedupe-hit", sig, frequencyWindow);
                        return false;
                    }
                }
                _recent[sig] = ts;
                _order.Enqueue((sig, ts));
                // Enforce capacity after adding
                Cleanup(ts);
                return true;
            }
        }

        private static void Cleanup(double nowTs)
        {
            if (_recent.Count == 0 && _order.Count == 0)
            {
                return;
            }
            // First drop expired by TTL
            while (_order.Count > 0)
            {
                var (k, t0) = _order.Peek();
                if (nowTs - t0 > TtlSeconds)
                {
                    // Remove only if the timestamp matches current entry (ignore stale queue nodes)
                    if (
                        _recent.TryGetValue(k, out var curTs)
                        && Math.Abs(curTs - t0) < double.Epsilon
                    )
                    {
                        _recent.Remove(k);
                    }
                    _order.Dequeue();
                }
                else
                {
                    break;
                }
            }

            // Then enforce max capacity by evicting the oldest keys
            while (_recent.Count > MaxEntries && _order.Count > 0)
            {
                var (k, t0) = _order.Dequeue();
                if (_recent.TryGetValue(k, out var curTs) && Math.Abs(curTs - t0) < double.Epsilon)
                {
                    _recent.Remove(k);
                }
            }

            CleanupFrequency(nowTs);
        }

        private static string BuildSignature(InternalExceptionModel m)
        {
            var framesSig = BuildFramesSignature(m.StackTrace);
            var threadId = m.ThreadId.ToString();
            return string.Join(
                "|",
                m.Name ?? string.Empty,
                m.Reason ?? string.Empty,
                m.Language.ToString(),
                threadId,
                framesSig
            );
        }

        private static string NormalizeFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                return string.Empty;
            var f = file.Replace('\\', '/');
            var idx = f.LastIndexOf('/');
            return idx >= 0 ? f.Substring(idx + 1) : f;
        }

        private static string BuildFramesSignature(IList<StackFrame> frames)
        {
            if (frames == null || frames.Count == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            var normalized = new List<StackFrame>(frames.Count);
            foreach (var frame in frames)
            {
                if (ShouldIgnoreFrame(frame))
                {
                    continue;
                }

                normalized.Add(frame);
            }

            if (normalized.Count == 0)
            {
                return string.Empty;
            }

            var depth = Math.Min(normalized.Count, SignatureFrameDepth);
            for (var i = 0; i < depth; i++)
            {
                var frame = normalized[i];
                if (i > 0)
                {
                    builder.Append("||");
                }

                var symbol = (frame.Symbol ?? string.Empty).Trim();
                var fileOnly = NormalizeFile(frame.File ?? string.Empty);
                builder.Append(symbol).Append('|').Append(fileOnly).Append('|').Append(frame.Line);
            }

            return builder.ToString();
        }

        private static bool ShouldIgnoreFrame(StackFrame frame)
        {
            if (frame == null)
            {
                return false;
            }

            var symbol = frame.Symbol ?? string.Empty;
            var file = frame.File ?? string.Empty;

            if (
                file.Equals(
                    "UnhandledExceptionHandler.bindings.cs",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return true;
            }

            if (symbol.StartsWith("UnityEngine.<>c:<RegisterUECatcher>", StringComparison.Ordinal))
            {
                return true;
            }

            if (
                symbol.StartsWith(
                    "UnityEngine.UnhandledExceptionHandler:HandleUnhandledException",
                    StringComparison.Ordinal
                )
            )
            {
                return true;
            }

            return false;
        }

        private static FrequencyWindow UpdateFrequency(string key, double nowTs)
        {
            if (_frequency.TryGetValue(key, out var window))
            {
                if (nowTs - window.LastTs > TtlSeconds)
                {
                    window.FirstTs = nowTs;
                    window.Count = 1;
                }
                else
                {
                    window.Count += 1;
                }

                window.LastTs = nowTs;
                _frequency[key] = window;
                return window;
            }

            var newWindow = new FrequencyWindow
            {
                FirstTs = nowTs,
                LastTs = nowTs,
                Count = 1,
            };
            _frequency[key] = newWindow;
            return newWindow;
        }

        private static void CleanupFrequency(double nowTs)
        {
            if (_frequency.Count == 0)
            {
                return;
            }

            var keysToRemove = new List<string>();
            foreach (var pair in _frequency)
            {
                if (nowTs - pair.Value.LastTs > TtlSeconds)
                {
                    keysToRemove.Add(pair.Key);
                }
            }

            if (keysToRemove.Count == 0)
            {
                return;
            }

            foreach (var key in keysToRemove)
            {
                _frequency.Remove(key);
            }
        }

        public static ExceptionFrequencySnapshot GetFrequencySnapshot(InternalExceptionModel model)
        {
            if (model == null)
            {
                return ExceptionFrequencySnapshot.Empty(string.Empty);
            }

            var sig = BuildSignature(model);
            lock (_lock)
            {
                if (_frequency.TryGetValue(sig, out var window))
                {
                    return CreateSnapshot(sig, window);
                }
            }

            return ExceptionFrequencySnapshot.Empty(sig);
        }

        private static ExceptionFrequencySnapshot CreateSnapshot(
            string signature,
            FrequencyWindow window
        )
        {
            return new ExceptionFrequencySnapshot(
                signature,
                window.Count,
                DateTime.UnixEpoch.AddSeconds(window.FirstTs),
                DateTime.UnixEpoch.AddSeconds(window.LastTs)
            );
        }

        private struct FrequencyWindow
        {
            public double FirstTs;
            public double LastTs;
            public int Count;
        }

        private static void LogFrequency(string stage, string signature, FrequencyWindow window)
        {
            var snapshot = CreateSnapshot(signature, window);
            CrashAnalysisLogger.DebugLog(
                $"RecentExceptionDeduper {stage}: count={snapshot.Count} first={snapshot.FirstSeenUtc:o} last={snapshot.LastSeenUtc:o} signature={snapshot.Signature}"
            );
        }

        public readonly struct ExceptionFrequencySnapshot
        {
            public string Signature { get; }
            public int Count { get; }
            public DateTime FirstSeenUtc { get; }
            public DateTime LastSeenUtc { get; }

            public ExceptionFrequencySnapshot(
                string signature,
                int count,
                DateTime firstSeenUtc,
                DateTime lastSeenUtc
            )
            {
                Signature = signature;
                Count = count;
                FirstSeenUtc = firstSeenUtc;
                LastSeenUtc = lastSeenUtc;
            }

            public static ExceptionFrequencySnapshot Empty(string signature)
            {
                return new ExceptionFrequencySnapshot(
                    signature,
                    0,
                    DateTime.MinValue,
                    DateTime.MinValue
                );
            }
        }
    }
}
