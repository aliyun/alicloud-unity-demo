using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

#nullable enable

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
        private const string BridgePodName = "AlicloudApmCBridge";
        private const string UnityFrameworkTargetName = "UnityFramework";
        private const string UnityIPhoneTargetName = "Unity-iPhone";
        private const string AliyunPodSourceDirective =
            "source 'https://github.com/aliyun/aliyun-specs.git'";
        private const string DefaultPodSourceDirective = "source 'https://cdn.cocoapods.org/'";
        private const string Utf8Locale = "UTF-8";

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
                EnsureBridgePod(pathToBuiltProject);
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

        private static void EnsureBridgePod(string pathToBuiltProject)
        {
            string versionConstraint = ApmPodSettings.BridgePodVersionConstraint;
            string podfilePath = Path.Combine(pathToBuiltProject, "Podfile");

            try
            {
                if (!File.Exists(podfilePath))
                {
                    File.WriteAllText(
                        podfilePath,
                        GenerateDefaultPodfileContents(versionConstraint)
                    );
                    EnsurePodSources(podfilePath);
                    ApmEditorLogger.Info(
                        $"Created Podfile with {BridgePodName} ({versionConstraint}) dependency"
                    );
                }
                else if (!PodfileContainsBridge(podfilePath))
                {
                    AddBridgeDependency(podfilePath, versionConstraint);
                    EnsurePodSources(podfilePath);
                    ApmEditorLogger.Info(
                        $"Added {BridgePodName} ({versionConstraint}) dependency to existing Podfile"
                    );
                }
                else
                {
                    EnsurePodSources(podfilePath);
                    ApmEditorLogger.Info($"{BridgePodName} dependency already present in Podfile");
                }
            }
            catch (Exception ex)
            {
                ApmEditorLogger.Warning($"Failed to update Podfile automatically: {ex.Message}");
                return;
            }

            RunPodInstall(pathToBuiltProject);
        }

        private static string GenerateDefaultPodfileContents(string versionConstraint)
        {
            return $@"# Generated by Alicloud APM Unity SDK
{AliyunPodSourceDirective}
{DefaultPodSourceDirective}
platform :ios, '12.0'
use_frameworks! :linkage => :static

target '{UnityFrameworkTargetName}' do
  pod '{BridgePodName}', '{versionConstraint}'
end

target '{UnityIPhoneTargetName}' do
  use_frameworks! :linkage => :static
  inherit! :complete
end
";
        }

        private static bool PodfileContainsBridge(string podfilePath)
        {
            string contents = File.ReadAllText(podfilePath);
            return contents.Contains($"pod '{BridgePodName}'", StringComparison.Ordinal)
                || contents.Contains($"pod \"{BridgePodName}\"", StringComparison.Ordinal);
        }

        private static void AddBridgeDependency(string podfilePath, string versionConstraint)
        {
            var lines = new List<string>(File.ReadAllLines(podfilePath));
            string podLine = $"  pod '{BridgePodName}', '{versionConstraint}'";

            bool insideUnityFramework = false;
            int insertionIndex = -1;

            for (int i = 0; i < lines.Count; i++)
            {
                string trimmed = lines[i].TrimStart();
                if (
                    trimmed.StartsWith(
                        $"target '{UnityFrameworkTargetName}'",
                        StringComparison.Ordinal
                    )
                )
                {
                    insideUnityFramework = true;
                    continue;
                }

                if (insideUnityFramework && trimmed.Equals("end", StringComparison.Ordinal))
                {
                    insertionIndex = i;
                    break;
                }
            }

            if (insertionIndex >= 0)
            {
                lines.Insert(insertionIndex, podLine);
            }
            else if (insideUnityFramework)
            {
                lines.Add(podLine);
                lines.Add("end");
            }
            else
            {
                if (lines.Count > 0 && lines[^1].Length != 0)
                {
                    lines.Add(string.Empty);
                }

                lines.Add($"target '{UnityFrameworkTargetName}' do");
                lines.Add(podLine);
                lines.Add("end");
            }

            File.WriteAllLines(podfilePath, lines);
        }

        private static void EnsurePodSources(string podfilePath)
        {
            var lines = new List<string>(File.ReadAllLines(podfilePath));
            bool updated = false;

            if (EnsureSourceDirective(lines, AliyunPodSourceDirective))
            {
                updated = true;
            }

            if (EnsureSourceDirective(lines, DefaultPodSourceDirective))
            {
                updated = true;
            }

            if (updated)
            {
                File.WriteAllLines(podfilePath, lines);
                ApmEditorLogger.Info("Ensured Podfile contains required source directives");
            }
        }

        private static bool EnsureSourceDirective(List<string> lines, string directive)
        {
            foreach (string line in lines)
            {
                if (line.Trim().Equals(directive, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            int insertIndex = 0;
            while (
                insertIndex < lines.Count
                && (
                    string.IsNullOrWhiteSpace(lines[insertIndex])
                    || lines[insertIndex].TrimStart().StartsWith("#", StringComparison.Ordinal)
                    || lines[insertIndex]
                        .TrimStart()
                        .StartsWith("source ", StringComparison.Ordinal)
                )
            )
            {
                insertIndex++;
            }

            lines.Insert(insertIndex, directive);
            return true;
        }

        private static void RunPodInstall(string projectPath)
        {
            string podExecutable = ResolvePodExecutable();
            if (string.IsNullOrEmpty(podExecutable))
            {
                ApmEditorLogger.Warning(
                    "CocoaPods 命令 `pod` 未找到。请确认已安装并可在 PATH 中访问，或在 `Assets/alicloud-apm/Editor/ApmPodSettings.cs` 中指定绝对路径。"
                );
                return;
            }

            bool runViaShell = ShouldUseShell(podExecutable);
            string arguments = "install";
            if (ApmPodSettings.UseRepoUpdateOnInstall)
            {
#pragma warning disable CS0162 // 检测到无法访问的代码
                arguments += " --repo-update";
#pragma warning restore CS0162 // 检测到无法访问的代码
            }

            string commandDisplay = $"{podExecutable} {arguments}".Trim();

            using var process = new Process();
            process.StartInfo.WorkingDirectory = projectPath;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            if (runViaShell)
            {
                string shell = ResolveShellExecutable();
                string quoted = QuoteForShell($"{podExecutable} {arguments}");
                process.StartInfo.FileName = shell;
                process.StartInfo.Arguments = $"-lc {quoted}";
                ApmEditorLogger.Info($"Running `{commandDisplay}` via shell `{shell}`");
            }
            else
            {
                process.StartInfo.FileName = podExecutable;
                process.StartInfo.Arguments = arguments;
                ApmEditorLogger.Info($"Running `{commandDisplay}`");
            }

            EnsureUtf8Locale(process.StartInfo.EnvironmentVariables);

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                ApmEditorLogger.Error(
                    $"无法自动执行 `{commandDisplay}`。请在 iOS 工程目录手动运行 `{commandDisplay}`。详情: {ex.Message}"
                );
                return;
            }

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                ApmEditorLogger.Info($"`{commandDisplay}` completed successfully");
            }
            else
            {
                ApmEditorLogger.Error(
                    $"`{commandDisplay}` exited with code {process.ExitCode}. stderr: {stderr}"
                );
            }
        }

        private static string ResolvePodExecutable()
        {
            string configured = ApmPodSettings.PodExecutablePath;
            return string.IsNullOrWhiteSpace(configured) ? "pod" : configured.Trim();
        }

        private static bool ShouldUseShell(string executable)
        {
            if (string.IsNullOrEmpty(executable))
            {
                return false;
            }

            return !Path.IsPathRooted(executable)
                && executable.IndexOf(Path.DirectorySeparatorChar) < 0
                && executable.IndexOf(Path.AltDirectorySeparatorChar) < 0;
        }

        private static string ResolveShellExecutable()
        {
            string? shellEnv = Environment.GetEnvironmentVariable("SHELL");
            if (!string.IsNullOrEmpty(shellEnv) && File.Exists(shellEnv))
            {
                return shellEnv;
            }

            string[] fallbacks = { "/bin/zsh", "/bin/bash", "/bin/sh" };
            foreach (string candidate in fallbacks)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return "/bin/sh";
        }

        private static string QuoteForShell(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return "''";
            }

            return "'" + command.Replace("'", "'\"'\"'") + "'";
        }

        private static void EnsureUtf8Locale(StringDictionary environment)
        {
            if (environment == null)
            {
                return;
            }

            SetUtf8IfMissing(environment, "LANG");
            SetUtf8IfMissing(environment, "LC_ALL");
            SetUtf8IfMissing(environment, "LC_CTYPE");
        }

        private static void SetUtf8IfMissing(StringDictionary environment, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            string? current = environment[key];
            if (
                !string.IsNullOrEmpty(current)
                && current.Contains("UTF-8", StringComparison.OrdinalIgnoreCase)
            )
            {
                return;
            }

            environment[key] = Utf8Locale;
        }
#endif
    }
}
