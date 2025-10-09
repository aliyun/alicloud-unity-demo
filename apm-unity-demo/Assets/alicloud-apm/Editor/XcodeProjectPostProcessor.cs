using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Alicloud.Apm.Editor
{
    /// <summary>
    /// Alicloud APM Xcode 工程后处理器
    /// 自动为 UnityFramework target 配置以下项目：
    /// - 添加 /usr/lib/swift 到 Runpath Search Paths
    /// - 添加 $(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME) 到 Library Search Paths
    /// - 添加 -ObjC 到 Other Linker Flags
    /// </summary>
    public class XcodeProjectPostProcessor
    {
        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
                return;

#if UNITY_IOS
            ApmEditorLogger.Info("Configuring iOS project...");

            string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

            // 获取 UnityFramework target
            string unityFrameworkTarget = project.GetUnityFrameworkTargetGuid();
            if (string.IsNullOrEmpty(unityFrameworkTarget))
            {
                unityFrameworkTarget = project.TargetGuidByName("UnityFramework");
            }

            if (!string.IsNullOrEmpty(unityFrameworkTarget))
            {
                // 添加 Runpath Search Paths
                AddSwiftRuntimePath(project, unityFrameworkTarget);

                // 添加 Library Search Paths
                AddLibrarySearchPaths(project, unityFrameworkTarget);

                // 添加 Other Linker Flags
                AddOtherLinkerFlags(project, unityFrameworkTarget);

                // 保存修改
                project.WriteToFile(projectPath);
                ApmEditorLogger.Info("iOS project configured successfully");
            }
            else
            {
                ApmEditorLogger.Warning("UnityFramework target not found");
            }
#endif
        }

#if UNITY_IOS
        private static void AddSwiftRuntimePath(PBXProject project, string targetGuid)
        {
            const string swiftPath = "/usr/lib/swift";

            // 检查是否已存在
            if (HasBuildProperty(project, targetGuid, "LD_RUNPATH_SEARCH_PATHS", swiftPath))
            {
                ApmEditorLogger.Info($"Swift runtime path already exists: {swiftPath}");
                return;
            }

            project.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", swiftPath);
            ApmEditorLogger.Info($"Added Swift runtime path: {swiftPath}");
        }

        private static void AddLibrarySearchPaths(PBXProject project, string targetGuid)
        {
            const string librarySearchPath = "$(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME)";

            // 检查是否已存在
            if (HasBuildProperty(project, targetGuid, "LIBRARY_SEARCH_PATHS", librarySearchPath))
            {
                ApmEditorLogger.Info($"Library search path already exists: {librarySearchPath}");
                return;
            }

            project.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", librarySearchPath);
            ApmEditorLogger.Info($"Added library search path: {librarySearchPath}");
        }

        private static void AddOtherLinkerFlags(PBXProject project, string targetGuid)
        {
            const string objcFlag = "-ObjC";

            // 检查是否已存在
            if (HasBuildProperty(project, targetGuid, "OTHER_LDFLAGS", objcFlag))
            {
                ApmEditorLogger.Info($"Linker flag already exists: {objcFlag}");
                return;
            }

            project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", objcFlag);
            ApmEditorLogger.Info($"Added linker flag: {objcFlag}");
        }

        /// <summary>
        /// 检查构建属性是否已包含指定值
        /// </summary>
        private static bool HasBuildProperty(
            PBXProject project,
            string targetGuid,
            string propertyName,
            string value
        )
        {
            try
            {
                // 获取所有构建配置
                var configGuids = project.BuildConfigByName(targetGuid, "Release");
                if (string.IsNullOrEmpty(configGuids))
                {
                    configGuids = project.BuildConfigByName(targetGuid, "Debug");
                }

                if (!string.IsNullOrEmpty(configGuids))
                {
                    var currentValue = project.GetBuildPropertyForConfig(configGuids, propertyName);
                    return !string.IsNullOrEmpty(currentValue) && currentValue.Contains(value);
                }

                return false;
            }
            catch
            {
                // 如果检查失败，假设不存在，继续添加
                return false;
            }
        }
#endif
    }
}
