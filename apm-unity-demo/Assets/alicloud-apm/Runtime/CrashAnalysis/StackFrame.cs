#nullable enable

namespace Alicloud.Apm.CrashAnalysis
{
    public sealed class StackFrame
    {
        // 符号（函数/方法名）
        public string? Symbol { get; private set; }

        // 文件名
        public string? File { get; private set; }

        // 行号
        public int Line { get; private set; }

        // 库
        public string? Library { get; private set; }

        // 地址
        public ulong? Address { get; private set; }

        private StackFrame() { }

        public static StackFrame FromSymbol(string symbol, string file, int line, string library)
        {
            return new StackFrame
            {
                Symbol = symbol,
                File = file,
                Line = line,
                Library = library,
            };
        }

        public static StackFrame FromAddress(ulong address)
        {
            return new StackFrame { Address = address };
        }
    }
}
