namespace Alicloud.Apm.CrashAnalysis
{
    /// <summary>
    /// 异常策略处理器，负责处理异常上报策略相关的逻辑
    /// </summary>
    internal static class ExceptionPolicyHandler
    {
        /// <summary>
        /// 判断是否为紧急上报模式
        /// </summary>
        /// <param name="policy">异常策略</param>
        /// <returns>是否需要立即上报</returns>
        public static bool IsUrgentMode(ExceptionPolicy policy)
        {
            return policy
                is ExceptionPolicy.ReportImmediately
                    or ExceptionPolicy.ReportImmediatelyAndQuitApplication;
        }

        /// <summary>
        /// 判断是否应该退出应用程序
        /// </summary>
        /// <param name="policy">异常策略</param>
        /// <returns>是否应该退出应用程序</returns>
        public static bool ShouldQuitApp(ExceptionPolicy policy)
        {
            return policy == ExceptionPolicy.ReportImmediatelyAndQuitApplication;
        }

        /// <summary>
        /// 应用上报策略到异常模型
        /// </summary>
        /// <param name="model">内部异常模型</param>
        /// <param name="policy">异常策略</param>
        public static void ApplyReportingStrategy(
            InternalExceptionModel model,
            ExceptionPolicy policy
        )
        {
            if (model == null)
            {
                return;
            }

            model.Urgent = IsUrgentMode(policy);
            model.QuitApp = ShouldQuitApp(policy);
        }
    }
}
