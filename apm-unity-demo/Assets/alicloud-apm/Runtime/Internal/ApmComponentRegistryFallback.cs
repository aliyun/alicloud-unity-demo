#nullable enable

using System;
using System.Collections.Generic;

namespace Alicloud.Apm
{
    /// <summary>
    /// 组件注册兜底：在正常注册缺失时尝试手动注册已知组件。
    /// </summary>
    internal static class ApmComponentRegistryFallback
    {
        private static readonly Dictionary<SDKComponents, Action> _registrars = new()
        {
            {
                SDKComponents.CrashAnalysis,
                () => CrashAnalysis.CrashAnalysisComponentRegistrar.EnsureRegistered()
            },
        };

        public static bool TryRegister(SDKComponents componentType)
        {
            if (!_registrars.TryGetValue(componentType, out var action))
            {
                ApmLogger.Warning(
                    $"Fallback registry unable to locate registrar for component {componentType}"
                );
                return false;
            }

            ApmLogger.Warning(
                $"Fallback registry auto-registering component {componentType}"
            );
            action();
            return true;
        }
    }
}
