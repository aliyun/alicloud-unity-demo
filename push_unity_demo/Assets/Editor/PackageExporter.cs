using UnityEngine;
using UnityEditor;
using System.IO;

public class PackageExporter
{
    [MenuItem("Tools/将Plugins打包为unitypackage")]
    public static void ExportPackage()
    {
        // 定义要导出的路径 - 只打包Assets/Plugins
        string[] exportPaths = new string[]
        {
            "Assets/Plugins"
        };

        // 版本号（不带v前缀，符合Unity标准）
        string version = "1.0.0-beta";
        
        // 固定的文件名
        string packageName = "alicloud-push.unitypackage";
        
        // 输出目录：unity根目录/build/版本号/
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        string buildDir = Path.Combine(projectRoot, "build", version);
        string outputPath = Path.Combine(buildDir, packageName);

        // 确保输出目录存在
        if (!Directory.Exists(buildDir))
        {
            Directory.CreateDirectory(buildDir);
            Debug.Log($"Created build directory: {buildDir}");
        }

        // 导出package（移除IncludeDependencies避免包含Unity内置资源）
        AssetDatabase.ExportPackage(
            exportPaths, 
            outputPath, 
            ExportPackageOptions.Recurse
        );

        Debug.Log($"Package exported successfully!");
        Debug.Log($"Package name: {packageName}");
        Debug.Log($"Version: {version}");
        Debug.Log($"Output path: {outputPath}");
        
        // 在文件浏览器中显示文件
        EditorUtility.RevealInFinder(outputPath);
    }
}