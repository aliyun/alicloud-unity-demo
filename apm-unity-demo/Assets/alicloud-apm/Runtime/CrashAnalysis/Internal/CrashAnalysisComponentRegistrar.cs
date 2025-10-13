namespace Alicloud.Apm.CrashAnalysis
{
    /// <summary>
    /// CrashAnalysis组件注册器
    /// </summary>
    internal static class CrashAnalysisComponentRegistrar
    {
        private static bool _registered;

        /// <summary>
        /// 注册CrashAnalysis组件到APM注册表
        /// </summary>
#if UNITY_2019_2_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(
            UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration
        )]
#elif UNITY_2017_1_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(
            UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen
        )]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod(
            UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad
        )]
#endif
        public static void RegisterComponent()
        {
            EnsureRegistered();
        }

        internal static void EnsureRegistered()
        {
            if (_registered)
            {
                return;
            }

            _registered = true;
            ApmComponentRegistry.RegisterComponent(
                SDKComponents.CrashAnalysis,
                () => new CrashAnalysisComponent()
            );
        }
    }
}
