#if UNITY_EDITOR
#nullable enable
using System;
using System.IO;
using UnityEditor.Android;

namespace Alicloud.Apm.Editor
{
    internal sealed class ApmAndroidGradleDependencies : IPostGenerateGradleAndroidProject
    {
        private const string DependencyIdentifier = "com.aliyun.ams:alicloud-apm-crash-analysis";
        private const string DependencyNotation = "    implementation '" + DependencyIdentifier + ":3.5.0'";
        private const string RepositoryUrl = "https://maven.aliyun.com/nexus/content/repositories/releases/";
        private const string ProguardRule = "-keep class com.aliyun.emas.apm.**{*;}";
        private const string ProguardComment = "# Alicloud APM crash analysis rules";

        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                ApmEditorLogger.Warning("ApmAndroidGradleDependencies invoked with empty project path.");
                return;
            }

            var projectRootPath = Path.GetFullPath(path);
            var unityLibraryPath = Path.Combine(projectRootPath, "unityLibrary");
            string? buildGradlePath;

            if (!File.Exists(Path.Combine(unityLibraryPath, "build.gradle")))
            {
                var normalizedPath = projectRootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var lastSegment = Path.GetFileName(normalizedPath);
                if (string.Equals(lastSegment, "unityLibrary", StringComparison.OrdinalIgnoreCase))
                {
                    unityLibraryPath = normalizedPath;
                    projectRootPath = Directory.GetParent(unityLibraryPath)?.FullName ?? string.Empty;
                }
            }

            buildGradlePath = Path.Combine(unityLibraryPath, "build.gradle");

            if (!File.Exists(buildGradlePath))
            {
                ApmEditorLogger.Warning($"unityLibrary/build.gradle not found. Checked path: {buildGradlePath}");
                return;
            }

            if (string.IsNullOrEmpty(projectRootPath) || !Directory.Exists(projectRootPath))
            {
                ApmEditorLogger.Warning("Unable to determine Android project root directory.");
                return;
            }

            var gradleContents = File.ReadAllText(buildGradlePath);
            if (gradleContents.IndexOf(DependencyIdentifier, StringComparison.Ordinal) < 0)
            {
                var updatedContents = EnsureDependency(gradleContents);
                if (updatedContents is not null)
                {
                    File.WriteAllText(buildGradlePath, updatedContents);
                    gradleContents = updatedContents;
                }
                else
                {
                    ApmEditorLogger.Warning("Failed to locate dependencies block in unityLibrary/build.gradle; dependency not injected.");
                }
            }
            else
            {
                ApmEditorLogger.Info("unityLibrary/build.gradle already contains the Alicloud APM dependency.");
            }

            EnsureConsumerProguardFile(buildGradlePath, gradleContents);

            EnsureSettingsRepository(projectRootPath);
            EnsureProguardRules(unityLibraryPath);

            ApmEditorLogger.Info("ApmAndroidGradleDependencies completed Android Gradle post-processing.");
        }

        private static string? EnsureDependency(string gradleContents)
        {
            const string dependenciesSection = "dependencies {";
            var dependenciesIndex = gradleContents.IndexOf(dependenciesSection, StringComparison.Ordinal);
            if (dependenciesIndex < 0)
            {
                return null;
            }

            var lineBreakIndex = gradleContents.IndexOf('\n', dependenciesIndex + dependenciesSection.Length);
            if (lineBreakIndex < 0)
            {
                return null;
            }

            return gradleContents.Insert(lineBreakIndex + 1, DependencyNotation + Environment.NewLine);
        }

        private static void EnsureSettingsRepository(string projectPath)
        {
            var settingsGradlePath = Path.Combine(projectPath, "settings.gradle");
            if (!File.Exists(settingsGradlePath))
            {
                ApmEditorLogger.Warning($"settings.gradle not found at path: {settingsGradlePath}");
                return;
            }

            var contents = File.ReadAllText(settingsGradlePath);
            if (contents.IndexOf(RepositoryUrl, StringComparison.Ordinal) >= 0)
            {
                return;
            }

            const string dependencyResolutionSection = "dependencyResolutionManagement";
            var dependencyIndex = contents.IndexOf(dependencyResolutionSection, StringComparison.Ordinal);
            if (dependencyIndex < 0)
            {
                ApmEditorLogger.Warning("dependencyResolutionManagement block not found in settings.gradle.");
                return;
            }

            var blockStart = contents.IndexOf('{', dependencyIndex);
            if (blockStart < 0)
            {
                ApmEditorLogger.Warning("No opening brace found for dependencyResolutionManagement block.");
                return;
            }

            var blockEnd = FindMatchingBrace(contents, blockStart);
            if (blockEnd < 0)
            {
                ApmEditorLogger.Warning("Could not match closing brace for dependencyResolutionManagement block.");
                return;
            }

            const string repositoriesSection = "repositories {";
            var repositoriesIndex = contents.IndexOf(repositoriesSection, blockStart, blockEnd - blockStart);
            if (repositoriesIndex < 0)
            {
                ApmEditorLogger.Warning("repositories block not found under dependencyResolutionManagement.");
                return;
            }

            var lineBreakIndex = contents.IndexOf('\n', repositoriesIndex + repositoriesSection.Length);
            if (lineBreakIndex < 0)
            {
                ApmEditorLogger.Warning("Could not determine insertion point inside repositories block.");
                return;
            }

            var lineIndent = GetLineIndentation(contents, repositoriesIndex);
            var childIndent = lineIndent + "    ";
            var newline = Environment.NewLine;
            var insertion = $"{childIndent}maven {{{newline}{childIndent}    url '{RepositoryUrl}'{newline}{childIndent}}}{newline}";

            contents = contents.Insert(lineBreakIndex + 1, insertion);
            File.WriteAllText(settingsGradlePath, contents);
        }

        private static void EnsureConsumerProguardFile(string buildGradlePath, string gradleContents)
        {
            if (gradleContents.IndexOf("proguard-user.txt", StringComparison.Ordinal) >= 0)
            {
                ApmEditorLogger.Info("unityLibrary/build.gradle already references proguard-user.txt.");
                return;
            }

            const string consumerKey = "consumerProguardFiles";
            var consumerIndex = gradleContents.IndexOf(consumerKey, StringComparison.Ordinal);
            if (consumerIndex < 0)
            {
                ApmEditorLogger.Warning("consumerProguardFiles entry not found in unityLibrary/build.gradle; cannot append custom rules.");
                return;
            }

            var lineEndIndex = gradleContents.IndexOf('\n', consumerIndex);
            if (lineEndIndex < 0)
            {
                lineEndIndex = gradleContents.Length;
            }

            var line = gradleContents.Substring(consumerIndex, lineEndIndex - consumerIndex);
            string updatedLine;
            if (line.IndexOf("proguard-user.txt", StringComparison.Ordinal) >= 0)
            {
                return;
            }

            if (line.IndexOf("proguard-unity.txt'", StringComparison.Ordinal) >= 0)
            {
                updatedLine = line.Replace("proguard-unity.txt'", "proguard-unity.txt', 'proguard-user.txt'");
            }
            else if (line.IndexOf("proguard-unity.txt\"", StringComparison.Ordinal) >= 0)
            {
                updatedLine = line.Replace("proguard-unity.txt\"", "proguard-unity.txt\", 'proguard-user.txt'");
            }
            else
            {
                updatedLine = line.TrimEnd() + ", 'proguard-user.txt'";
            }

            var updatedContents = gradleContents.Remove(consumerIndex, line.Length).Insert(consumerIndex, updatedLine);
            File.WriteAllText(buildGradlePath, updatedContents);
        }

        private static void EnsureProguardRules(string unityLibraryPath)
        {
            var proguardPath = Path.Combine(unityLibraryPath, "proguard-user.txt");
            if (!File.Exists(proguardPath))
            {
                var contents = $"{ProguardComment}{Environment.NewLine}{ProguardRule}{Environment.NewLine}";
                File.WriteAllText(proguardPath, contents);
                return;
            }

            var existingContents = File.ReadAllText(proguardPath);
            if (existingContents.IndexOf(ProguardRule, StringComparison.Ordinal) >= 0)
            {
                return;
            }

            var separator = existingContents.EndsWith(Environment.NewLine, StringComparison.Ordinal)
                ? string.Empty
                : Environment.NewLine;
            var updatedContents = $"{existingContents}{separator}{ProguardComment}{Environment.NewLine}{ProguardRule}{Environment.NewLine}";
            File.WriteAllText(proguardPath, updatedContents);
        }

        private static string GetLineIndentation(string contents, int position)
        {
            var lineStart = contents.LastIndexOf('\n', position);
            lineStart = lineStart < 0 ? 0 : lineStart + 1;

            var indentEnd = lineStart;
            while (indentEnd < contents.Length)
            {
                var c = contents[indentEnd];
                if (c == ' ' || c == '\t')
                {
                    indentEnd++;
                    continue;
                }

                break;
            }

            return contents.Substring(lineStart, indentEnd - lineStart);
        }

        private static int FindMatchingBrace(string contents, int openBraceIndex)
        {
            var depth = 0;
            for (var i = openBraceIndex; i < contents.Length; i++)
            {
                var c = contents[i];
                if (c == '{')
                {
                    depth++;
                }
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i + 1;
                    }
                }
            }

            return -1;
        }
    }
}
#nullable restore
#endif
