using System.Collections.Generic;

namespace Alicloud.Apm.CrashAnalysis
{
    public sealed class ExceptionModel
    {
        // 名称，通常为异常类型名
        public string Name { get; }

        // 原因
        public string Reason { get; }

        // 异常来源的语言
        public SourceLanguage Language { get; set; } = SourceLanguage.CSharp;

        // 堆栈列表：从顶部开始倒序（Top-Down）
        public IList<StackFrame> StackTrace { get; set; } = new List<StackFrame>();

        public ExceptionModel(string name, string reason)
        {
            Name = name;
            Reason = reason;
        }

        public ExceptionModel(
            string name,
            string reason,
            SourceLanguage language = SourceLanguage.CSharp
        )
        {
            Name = name;
            Reason = reason;
            Language = language;
        }
    }

    /// <summary>
    /// 表示异常来源的编程语言
    /// </summary>
    public enum SourceLanguage
    {
        Unknown = 0,
        CSharp,
        Lua,
    }
}
