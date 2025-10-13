namespace Alicloud.Apm.Editor
{
#if UNITY_EDITOR
    /// <summary>
    /// 默认的 CocoaPods 配置，如需自定义可直接修改常量值。
    /// </summary>
    static class ApmPodSettings
    {
        public const string BridgePodVersionConstraint = "~> 1.0.0";
        public const string PodExecutablePath = "pod";
        public const bool UseRepoUpdateOnInstall = false;
    }
#endif
}
