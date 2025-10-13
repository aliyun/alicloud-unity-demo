using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Alicloud.Apm.Editor
{
    /// <summary>
    /// Injects IL2CPP arg --emit-source-mapping before build to enable C# stack symbolization.
    /// Restores the original value after build to avoid persistent project changes.
    /// </summary>
    internal sealed class Il2CppArgsInjector
        : IPreprocessBuildWithReport,
            IPostprocessBuildWithReport
    {
        private const string Flag = "--emit-source-mapping";
        private const string EditorPrefsKey = "AlicloudApm.PreviousAdditionalIl2CppArgs";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var targetGroup = BuildPipeline.GetBuildTargetGroup(report.summary.platform);

            // Only for IL2CPP builds
            if (PlayerSettings.GetScriptingBackend(targetGroup) != ScriptingImplementation.IL2CPP)
                return;

            // Read current args
            var currentArgs = ReadAdditionalIl2CppArgs();

            // Skip if already present
            if (!string.IsNullOrEmpty(currentArgs) && currentArgs.Contains(Flag))
                return;

            // Save original once per build session
            if (!EditorPrefs.HasKey(EditorPrefsKey))
            {
                EditorPrefs.SetString(EditorPrefsKey, currentArgs ?? string.Empty);
            }

            var newArgs = string.IsNullOrEmpty(currentArgs) ? Flag : (currentArgs + " " + Flag);
            WriteAdditionalIl2CppArgs(newArgs);
            ApmEditorLogger.Info("Injected IL2CPP arg: " + Flag);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (!EditorPrefs.HasKey(EditorPrefsKey))
                return;

            var original = EditorPrefs.GetString(EditorPrefsKey, string.Empty);
            EditorPrefs.DeleteKey(EditorPrefsKey);
            WriteAdditionalIl2CppArgs(original);
            ApmEditorLogger.Info("Restored IL2CPP additional args after build");
        }

        private static string ReadAdditionalIl2CppArgs()
        {
            // Try public API first (if available in this Unity version)
            var playerSettingsType = typeof(PlayerSettings);
            var getMethod = playerSettingsType.GetMethod(
                "GetAdditionalIl2CppArgs",
                System.Reflection.BindingFlags.Static
                    | System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.NonPublic
            );
            if (getMethod != null)
            {
                try
                {
                    var value = getMethod.Invoke(null, null) as string;
                    return value ?? string.Empty;
                }
                catch
                { /* fall back below */
                }
            }

            // Fallback: read from ProjectSettings asset via serialization
            var obj = AssetDatabase
                .LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")
                .FirstOrDefault();
            if (obj == null)
                return string.Empty;

            var so = new SerializedObject(obj);
            var prop = so.FindProperty("additionalIl2CppArgs");
            return prop != null ? prop.stringValue : string.Empty;
        }

        private static void WriteAdditionalIl2CppArgs(string value)
        {
            // Try public API first (if available in this Unity version)
            var playerSettingsType = typeof(PlayerSettings);
            var setMethod = playerSettingsType.GetMethod(
                "SetAdditionalIl2CppArgs",
                System.Reflection.BindingFlags.Static
                    | System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.NonPublic
            );
            if (setMethod != null)
            {
                try
                {
                    setMethod.Invoke(null, new object[] { value ?? string.Empty });
                    return;
                }
                catch
                { /* fall back below */
                }
            }

            // Fallback: write to ProjectSettings asset via serialization
            var obj = AssetDatabase
                .LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")
                .FirstOrDefault();
            if (obj == null)
                return;

            var so = new SerializedObject(obj);
            var prop = so.FindProperty("additionalIl2CppArgs");
            if (prop != null)
            {
                prop.stringValue = value ?? string.Empty;
                so.ApplyModifiedPropertiesWithoutUndo();
                AssetDatabase.SaveAssets();
            }
        }
    }

    /// <summary>
    /// Safety net: if a previous build failed and we didn't get a postprocess
    /// callback, restore the args on editor load.
    /// </summary>
    [InitializeOnLoad]
    internal static class Il2CppArgsInjectorGuard
    {
        static Il2CppArgsInjectorGuard()
        {
            EditorApplication.delayCall += RestoreIfNeeded;
        }

        private static void RestoreIfNeeded()
        {
            const string key = "AlicloudApm.PreviousAdditionalIl2CppArgs";
            if (!EditorPrefs.HasKey(key))
                return;

            var original = EditorPrefs.GetString(key, string.Empty);
            EditorPrefs.DeleteKey(key);

            // Reuse writer from injector
            var playerSettingsType = typeof(PlayerSettings);
            var setMethod = playerSettingsType.GetMethod(
                "SetAdditionalIl2CppArgs",
                System.Reflection.BindingFlags.Static
                    | System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.NonPublic
            );
            if (setMethod != null)
            {
                try
                {
                    setMethod.Invoke(null, new object[] { original });
                    return;
                }
                catch { }
            }

            var obj = AssetDatabase
                .LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")
                .FirstOrDefault();
            if (obj == null)
                return;
            var so = new SerializedObject(obj);
            var prop = so.FindProperty("additionalIl2CppArgs");
            if (prop != null)
            {
                prop.stringValue = original;
                so.ApplyModifiedPropertiesWithoutUndo();
                AssetDatabase.SaveAssets();
            }

            ApmEditorLogger.Info("Restored IL2CPP additional args on editor load");
        }
    }
}
