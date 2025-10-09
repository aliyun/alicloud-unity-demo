namespace Alicloud.Apm
{
    /// <summary>
    /// APM组件接口
    /// </summary>
    internal interface IApmComponent
    {
        /// <summary>
        /// 组件类型
        /// </summary>
        SDKComponents ComponentType { get; }

        /// <summary>
        /// 初始化组件
        /// </summary>
        void Initialize();

        /// <summary>
        /// 组件是否已初始化
        /// </summary>
        bool IsInitialized { get; }
    }
}
