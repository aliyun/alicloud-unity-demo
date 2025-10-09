using System;
using System.Collections.Generic;
using System.Threading;

namespace Alicloud.Apm.CrashAnalysis
{
    internal sealed class InternalExceptionModel
    {
        // 名称，通常为异常类型名
        public string Name { get; }

        // 原因
        public string Reason { get; }

        // 异常来源的语言
        public SourceLanguage Language { get; set; } = SourceLanguage.CSharp;

        // 堆栈列表：从顶部开始倒序（Top-Down）
        public IList<StackFrame> StackTrace { get; set; } = new List<StackFrame>();

        // 是否为自定义异常
        public bool Custom { get; set; } = false;

        // 是否立即上报
        public bool Urgent { get; set; } = false;

        // 是否退出应用
        public bool QuitApp { get; set; } = false;

        // 时间戳
        public DateTime Timestamp { get; }

        // 捕获时记录的托管线程 Id（原始触发线程）
        public int ThreadId { get; set; }

        public InternalExceptionModel(string name, string reason)
        {
            Name = name;
            Reason = reason;
            Timestamp = DateTime.UtcNow;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public InternalExceptionModel(
            string name,
            string reason,
            SourceLanguage language = SourceLanguage.CSharp
        )
        {
            Name = name;
            Reason = reason;
            Language = language;
            Timestamp = DateTime.UtcNow;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}
