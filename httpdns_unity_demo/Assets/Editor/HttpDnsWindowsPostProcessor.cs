#if UNITY_STANDALONE_WIN
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

/// <summary>
/// Post-build processor for Windows HttpDNS integration
/// Automatically copies required DLL files to correct locations
/// Solves the issue where DLLs are not found at runtime
/// </summary>
public class HttpDnsWindowsPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.StandaloneWindows && buildTarget != BuildTarget.StandaloneWindows64)
            return;

        Debug.Log("[HttpDNS Windows PostProcessor] Configuring Windows project for HttpDNS...");
        
        // 检测当前运行环境
        bool isRunningOnWindows = System.Environment.OSVersion.Platform == System.PlatformID.Win32NT;
        Debug.Log($"[HttpDNS Windows PostProcessor] Running on Windows: {isRunningOnWindows}");

        try
        {
            CopyHttpDnsBridge(pathToBuiltProject);
            
            // 只在Windows环境下尝试复制VCPKG依赖
            if (isRunningOnWindows)
            {
                CopyVCPKGDependencies(pathToBuiltProject);
            }
            else
            {
                CreateVCPKGInstructionsForCrossPlatform(pathToBuiltProject);
            }
            
            Debug.Log("[HttpDNS Windows PostProcessor] Windows project configuration completed successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[HttpDNS Windows PostProcessor] Failed to configure Windows project: " + e.Message);
        }
    }

    private static void CopyHttpDnsBridge(string pathToBuiltProject)
    {
        string projectRoot = Path.GetDirectoryName(pathToBuiltProject);
        string bridgeDllSource = Path.Combine(Application.dataPath, "Plugins", "C", "x86_64", "HttpDnsUnityBridge.dll");
        
        if (!File.Exists(bridgeDllSource))
        {
            Debug.LogWarning($"[HttpDNS Windows PostProcessor] HttpDnsUnityBridge.dll not found at: {bridgeDllSource}");
            return;
        }

        Debug.Log($"[HttpDNS Windows PostProcessor] Build project root: {projectRoot}");
        
        // MonoBleedingEdge目录在项目根目录下 - HttpDnsUnityBridge.dll的正确位置
        string monoBleedingEdgeDir = Path.Combine(projectRoot, "MonoBleedingEdge");
        Directory.CreateDirectory(monoBleedingEdgeDir);
        
        // 复制HttpDnsUnityBridge.dll到根目录的MonoBleedingEdge目录
        string bridgeDllDest = Path.Combine(monoBleedingEdgeDir, "HttpDnsUnityBridge.dll");
        File.Copy(bridgeDllSource, bridgeDllDest, true);
        
        Debug.Log($"[HttpDNS Windows PostProcessor] Copied HttpDnsUnityBridge.dll to root MonoBleedingEdge: {bridgeDllDest}");
        
        // 验证文件是否成功复制
        if (File.Exists(bridgeDllDest))
        {
            FileInfo sourceInfo = new FileInfo(bridgeDllSource);
            FileInfo destInfo = new FileInfo(bridgeDllDest);
            
            Debug.Log($"[HttpDNS Windows PostProcessor] Verification - Source: {sourceInfo.Length}B, MonoBleedingEdge: {destInfo.Length}B");
        }
    }

    private static void CopyVCPKGDependencies(string pathToBuiltProject)
    {
        string projectRoot = Path.GetDirectoryName(pathToBuiltProject);
        
        // VCPKG依赖库列表
        List<string> vcpkgDependencies = new List<string>
        {
            "curl.dll", "libcurl.dll", "cjson.dll", "apr-1.dll", "aprutil-1.dll",
            "libapr-1.dll", "libaprutil-1.dll", "ssl-3.dll", "crypto-3.dll",
            "libssl-3.dll", "libcrypto-3.dll", "zlib1.dll", "libz.dll"
        };

        // 常见的VCPKG安装路径
        List<string> vcpkgSearchPaths = new List<string>();
        
        // 从环境变量获取VCPKG路径
        string vcpkgRoot = System.Environment.GetEnvironmentVariable("VCPKG_ROOT");
        if (!string.IsNullOrEmpty(vcpkgRoot))
        {
            string vcpkgBinPath = Path.Combine(vcpkgRoot, "installed", "x64-windows", "bin");
            vcpkgSearchPaths.Add(vcpkgBinPath);
        }
        
        // 添加常见路径
        vcpkgSearchPaths.AddRange(new string[]
        {
            @"C:\vcpkg\installed\x64-windows\bin",
            @"C:\Users\Administrator\Desktop\vcpkg\installed\x64-windows\bin"
        });

        Debug.Log($"[HttpDNS Windows PostProcessor] Searching for VCPKG dependencies...");
        
        int copiedCount = 0;
        foreach (string searchPath in vcpkgSearchPaths)
        {
            if (!Directory.Exists(searchPath)) continue;

            Debug.Log($"[HttpDNS Windows PostProcessor] Checking VCPKG path: {searchPath}");
            
            foreach (string dependency in vcpkgDependencies)
            {
                string sourcePath = Path.Combine(searchPath, dependency);
                if (File.Exists(sourcePath))
                {
                    // 复制到项目根目录 - 正确的VCPKG依赖库位置
                    string destPath = Path.Combine(projectRoot, dependency);
                    File.Copy(sourcePath, destPath, true);
                    
                    Debug.Log($"[HttpDNS Windows PostProcessor] Copied {dependency} to root directory");
                    copiedCount++;
                }
            }
            
            if (copiedCount > 0) break; // 找到有效的VCPKG路径就停止搜索
        }

        if (copiedCount == 0)
        {
            Debug.LogWarning("[HttpDNS Windows PostProcessor] No VCPKG dependencies found.");
            Debug.LogWarning("[HttpDNS Windows PostProcessor] Please install VCPKG and set VCPKG_ROOT environment variable.");
        }
        else
        {
            Debug.Log($"[HttpDNS Windows PostProcessor] Successfully copied {copiedCount} VCPKG dependencies to root directory");
        }
    }
    
    private static void CreateVCPKGInstructionsForCrossPlatform(string pathToBuiltProject)
    {
        string projectRoot = Path.GetDirectoryName(pathToBuiltProject);
        
        Debug.Log("[HttpDNS Windows PostProcessor] Cross-platform build detected. VCPKG dependencies will need to be copied manually on Windows.");
        
        // 创建简单的说明文件
        string instructionsPath = Path.Combine(projectRoot, "HttpDNS_Setup_Instructions.txt");
        string instructions = "HttpDNS Windows Setup Instructions\n" +
            "=================================\n\n" +
            "This project was built on a non-Windows platform.\n" +
            "To use HttpDNS functionality on Windows:\n\n" +
            "1. HttpDnsUnityBridge.dll should be in: MonoBleedingEdge/\n" +
            "2. VCPKG dependencies should be in project root directory\n" +
            "3. Install VCPKG: vcpkg install curl apr apr-util cjson --triplet x64-windows\n" +
            "4. Set VCPKG_ROOT environment variable\n" +
            "5. Rebuild on Windows for automatic dependency copying\n";
        
        File.WriteAllText(instructionsPath, instructions);
        Debug.Log($"[HttpDNS Windows PostProcessor] Created setup instructions: {instructionsPath}");
    }
}
#endif