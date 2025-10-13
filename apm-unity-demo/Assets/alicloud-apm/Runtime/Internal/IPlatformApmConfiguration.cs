namespace Alicloud.Apm
{
    internal interface IPlatformApmConfiguration
    {
        /// <summary>
        /// 设置Native SDK日志级别
        /// </summary>
        /// <param name="loggerLevel">日志级别</param>
        public void SetLoggerLevel(ApmLoggerLevel loggerLevel);

        /// <summary>
        /// 设置APM环境配置
        /// </summary>
        /// <param name="environment">环境类型</param>
        public void SetEnvironment(ApmEnvironment environment);
    }
}
