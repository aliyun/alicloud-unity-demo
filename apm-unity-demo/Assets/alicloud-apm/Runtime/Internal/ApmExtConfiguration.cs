using System;

namespace Alicloud.Apm
{
    /// <summary>
    /// APM组件注册器
    /// </summary>
    internal static class ApmExtConfiguration
    {
        /// <summary>
        /// 设置APM环境配置（SDK内部接口）
        /// </summary>
        /// <param name="environment">环境类型</param>
        public static void SetEnvironment(ApmEnvironment environment)
        {
            if (!Enum.IsDefined(typeof(ApmEnvironment), environment))
            {
                ApmLogger.Warning(
                    $"Invalid ApmEnvironment value: {environment}. Using default Production environment."
                );
                environment = ApmEnvironment.Production;
            }

            ApmConfiguration.PlatformApmConfiguration.SetEnvironment(environment);
        }
    }

    /// <summary>
    /// APM环境类型
    /// </summary>
    public enum ApmEnvironment : int
    {
        /// <summary>
        /// 预发环境
        /// </summary>
        PreProduction = 0,

        /// <summary>
        /// 生产环境
        /// </summary>
        Production = 1,
    }
}
