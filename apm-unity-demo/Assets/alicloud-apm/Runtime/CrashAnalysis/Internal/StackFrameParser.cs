using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Alicloud.Apm.CrashAnalysis
{
    /// <summary>
    /// Utility class for parsing stack traces from different sources
    /// </summary>
    internal static class StackFrameParser
    {
        // Regex patterns for improved stack trace parsing

        // Matches e.g. "Method(string arg1, int arg2)" or "Method (string arg1, int arg2)"
        private static readonly string FrameArgsRegex = @"\s?\(.*\)";

        // Matches e.g. "at Assets.Src.Gui.Menus.GameMenuController.ShowSkilltreeScreen (string arg1)"
        //      or e.g. "UnityEngine.Debug:Log(Object)"
        private static readonly string FrameRegexWithoutFileInfo =
            @"(?<class>[^\s]+)\.(?<method>[^\s\.]+)" + FrameArgsRegex;

        // Matches e.g.:
        //   "at Assets.Src.Gui.Menus.GameMenuController.ShowSkilltreeScreen () (at C:/Game_Project/Assets/Src/Gui/GameMenuController.cs:154)"
        //   "at SampleApp.Buttons.SampleClass.CauseDivByZero () [0x00000] in /Some/Dir/Program.cs:567"
        private static readonly string FrameRegexWithFileInfo =
            FrameRegexWithoutFileInfo + @" .*[/|\\](?<file>.+):(?<line>\d+)";

        // Mono's hardcoded unknown file string (never localized)
        private static readonly string MonoFilenameUnknownString = "<filename unknown>";

        private static readonly string[] StringDelimiters = new string[] { Environment.NewLine };

#if UNITY_IOS && !UNITY_EDITOR
        /// <summary>
        /// Convert StackFrame list to NativeStackFrame array for native interop
        /// </summary>
        /// <param name="frames">List of StackFrame objects</param>
        /// <returns>Array of NativeStackFrame for native calls</returns>
        public static NativeStackFrame[] ConvertToNativeFrames(IList<StackFrame> frames)
        {
            if (frames == null || frames.Count == 0)
            {
                return new NativeStackFrame[0];
            }

            var nativeFrames = new NativeStackFrame[frames.Count];
            for (int i = 0; i < frames.Count; i++)
            {
                nativeFrames[i] = new NativeStackFrame
                {
                    symbol = frames[i].Symbol ?? string.Empty,
                    file = frames[i].File ?? string.Empty,
                    line = frames[i].Line,
                    library = frames[i].Library ?? string.Empty,
                    address = frames[i].Address ?? 0,
                };
            }

            return nativeFrames;
        }
#endif

        /// <summary>
        /// Parse stack trace from different sources based on language
        /// </summary>
        /// <param name="stackTrace">Stack trace string</param>
        /// <param name="language">Source language</param>
        /// <returns>List of parsed stack frames</returns>
        public static IList<StackFrame> ParseStackTrace(string stackTrace, SourceLanguage language)
        {
            switch (language)
            {
                case SourceLanguage.CSharp:
                    return ParseUnityStackTrace(stackTrace);
                case SourceLanguage.Lua:
                    return ParseLuaStackTrace(stackTrace);
                default:
                    return ParseUnityStackTrace(stackTrace);
            }
        }

        /// <summary>
        /// Parse Unity/C# stack trace using regex parsing for improved accuracy
        /// </summary>
        /// <param name="stackTrace">Stack trace string</param>
        /// <returns>List of parsed stack frames</returns>
        public static IList<StackFrame> ParseUnityStackTrace(string stackTrace)
        {
            var frames = new List<StackFrame>();

            if (string.IsNullOrEmpty(stackTrace))
            {
                return frames;
            }

            string[] splitStackTrace = stackTrace.Split(StringDelimiters, StringSplitOptions.None);
            if (splitStackTrace.Length < 1)
            {
                return frames;
            }

            foreach (var frameString in splitStackTrace)
            {
                if (string.IsNullOrEmpty(frameString?.Trim()))
                {
                    continue;
                }

                string regex = null;

                // Try to match with file info first (more specific)
                if (Regex.Matches(frameString, FrameRegexWithFileInfo).Count == 1)
                {
                    regex = FrameRegexWithFileInfo;
                }
                // Fall back to without file info
                else if (Regex.Matches(frameString, FrameRegexWithoutFileInfo).Count == 1)
                {
                    regex = FrameRegexWithoutFileInfo;
                }
                else
                {
                    // If no regex matches, create a basic frame with the line as symbol
                    var basicFrame = StackFrame.FromSymbol(
                        frameString.Trim(),
                        string.Empty,
                        0,
                        string.Empty
                    );
                    frames.Add(basicFrame);
                    continue;
                }

                var parsedFrame = ParseFrameString(regex, frameString);
                if (parsedFrame != null)
                {
                    frames.Add(parsedFrame);
                }
            }

            return frames;
        }

        /// <summary>
        /// Parse a single frame string using regex parsing
        /// </summary>
        /// <param name="regex">Regex pattern to use</param>
        /// <param name="frameString">Frame string to parse</param>
        /// <returns>Parsed stack frame or null if parsing failed</returns>
        private static StackFrame ParseFrameString(string regex, string frameString)
        {
            var matches = Regex.Matches(frameString, regex);
            if (matches.Count < 1)
            {
                return null;
            }

            var match = matches[0];
            if (!match.Groups["class"].Success || !match.Groups["method"].Success)
            {
                return null;
            }

            string className = match.Groups["class"].Value;
            string methodName = match.Groups["method"].Value;
            string file = match.Groups["file"].Success ? match.Groups["file"].Value : string.Empty;
            string lineStr = match.Groups["line"].Success ? match.Groups["line"].Value : "0";

            // Handle Mono's unknown filename
            if (file == MonoFilenameUnknownString)
            {
                file = string.Empty;
                lineStr = "0";
            }

            int.TryParse(lineStr, out int lineNumber);

            // Create symbol from class and method
            string symbol = $"{className}.{methodName}";

            return StackFrame.FromSymbol(symbol, file, lineNumber, string.Empty);
        }

        /// <summary>
        /// Parse Lua stack trace string into stack frames
        /// </summary>
        /// <param name="stackTrace">Lua stack trace string</param>
        /// <returns>List of parsed stack frames</returns>
        public static IList<StackFrame> ParseLuaStackTrace(string stackTrace)
        {
            var frames = new List<StackFrame>();

            if (string.IsNullOrEmpty(stackTrace))
            {
                return frames;
            }

            var lines = stackTrace.Split(
                new[] { '\n', '\r' },
                System.StringSplitOptions.RemoveEmptyEntries
            );
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    continue;
                }

                var frame = ParseLuaStackLine(trimmed);
                if (frame != null)
                {
                    frames.Add(frame);
                }
            }

            return frames;
        }

        /// <summary>
        /// Parse a single Lua stack trace line
        /// </summary>
        /// <param name="line">Lua stack trace line</param>
        /// <returns>Parsed stack frame or null if parsing failed</returns>
        private static StackFrame ParseLuaStackLine(string line)
        {
            try
            {
                // Lua stack trace format typically: "[string \"filename\"]:line: in function 'functionName'"
                // or: "filename:line: in function 'functionName'"
                // or: "filename:line: in main chunk"

                string symbol = string.Empty;
                string file = string.Empty;
                int lineNumber = 0;

                // Look for file:line pattern
                var match = System.Text.RegularExpressions.Regex.Match(
                    line,
                    @"(?:\[string\s+""([^""]+)""\]|([^:]+)):(\d+):\s*(.*)"
                );
                if (match.Success)
                {
                    file = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                    if (int.TryParse(match.Groups[3].Value, out lineNumber))
                    {
                        symbol = match.Groups[4].Value.Trim();

                        // Clean up symbol
                        if (symbol.StartsWith("in function '") && symbol.EndsWith("'"))
                        {
                            symbol = symbol.Substring(13, symbol.Length - 14);
                        }
                        else if (symbol.StartsWith("in "))
                        {
                            symbol = symbol.Substring(3);
                        }
                    }
                }
                else
                {
                    // If no pattern match, use the whole line as symbol
                    symbol = line;
                }

                return StackFrame.FromSymbol(symbol, file, lineNumber, "Lua");
            }
            catch
            {
                // If parsing fails, create a frame with the raw line as symbol
                return StackFrame.FromSymbol(line, string.Empty, 0, "Lua");
            }
        }
    }
}
