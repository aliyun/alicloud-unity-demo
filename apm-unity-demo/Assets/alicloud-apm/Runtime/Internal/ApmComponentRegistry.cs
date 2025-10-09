#nullable enable

using System;
using System.Collections.Generic;

namespace Alicloud.Apm
{
    /// <summary>
    /// APM组件注册器
    /// </summary>
    internal static class ApmComponentRegistry
    {
        private static readonly Dictionary<SDKComponents, Func<IApmComponent>> _componentFactories =
            new();
        private static readonly Dictionary<SDKComponents, IApmComponent> _componentInstances =
            new();
        private static readonly object _lock = new();

        /// <summary>
        /// 注册组件工厂
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <param name="factory">组件工厂方法</param>
        public static void RegisterComponent<T>(SDKComponents componentType, Func<T> factory)
            where T : IApmComponent
        {
            lock (_lock)
            {
                _componentFactories[componentType] = () => factory();
                ApmLogger.Info($"Registered component factory: {componentType}");
            }
        }

        /// <summary>
        /// 初始化指定的组件
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>是否成功初始化</returns>
        public static bool InitializeComponent(SDKComponents componentType)
        {
            lock (_lock)
            {
                // 检查是否已经初始化
                if (_componentInstances.TryGetValue(componentType, out var existingComponent))
                {
                    if (existingComponent.IsInitialized)
                    {
                        return true;
                    }
                }

                // 检查是否有注册的工厂
                if (!_componentFactories.TryGetValue(componentType, out var factory))
                {
                    if (
                        !ApmComponentRegistryFallback.TryRegister(componentType)
                        || !_componentFactories.TryGetValue(componentType, out factory)
                    )
                    {
                        ApmLogger.Warning($"No factory registered for component: {componentType}");
                        return false;
                    }
                }

                try
                {
                    // 创建组件实例
                    var component = factory();
                    if (component.ComponentType != componentType)
                    {
                        ApmLogger.Error(
                            $"Component type mismatch: expected {componentType}, got {component.ComponentType}"
                        );
                        return false;
                    }

                    // 初始化组件
                    component.Initialize();
                    _componentInstances[componentType] = component;

                    ApmLogger.Info($"Component {componentType} initialized successfully");
                    return true;
                }
                catch (Exception ex)
                {
                    ApmLogger.Error(
                        $"Failed to initialize component {componentType}: {ex.Message}"
                    );
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取组件实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="componentType">组件枚举</param>
        /// <returns>组件实例，如果未初始化则返回null</returns>
        public static T? GetComponent<T>(SDKComponents componentType)
            where T : class, IApmComponent
        {
            lock (_lock)
            {
                if (_componentInstances.TryGetValue(componentType, out var component))
                {
                    return component as T;
                }
                return null;
            }
        }

        /// <summary>
        /// 检查组件是否已注册
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>是否已注册</returns>
        public static bool IsComponentRegistered(SDKComponents componentType)
        {
            lock (_lock)
            {
                return _componentFactories.ContainsKey(componentType);
            }
        }
    }
}
