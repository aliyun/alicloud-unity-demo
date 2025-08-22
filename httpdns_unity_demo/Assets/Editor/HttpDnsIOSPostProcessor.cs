#if UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

/// <summary>
/// Post-build processor for iOS HttpDNS integration
/// Automatically configures the Xcode project with required dependencies and settings
/// Following Alibaba Cloud HTTPDNS iOS SDK documentation
/// </summary>
public class HttpDnsIOSPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        Debug.Log("[HttpDNS PostProcessor] Configuring iOS project for HttpDNS...");

        try
        {
            ConfigureXcodeProject(pathToBuiltProject);
            ConfigurePodfile(pathToBuiltProject);
            Debug.Log("[HttpDNS PostProcessor] iOS project configuration completed successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[HttpDNS PostProcessor] Failed to configure iOS project: " + e.Message);
        }
    }

    private static void ConfigureXcodeProject(string pathToBuiltProject)
    {
        string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

        // Get the Unity-iPhone target GUID
        string targetGuid = project.GetUnityMainTargetGuid();
        string unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

        // Configure both targets
        string[] targetGuids = { targetGuid, unityFrameworkTargetGuid };
        
        foreach (string guid in targetGuids)
        {
            if (string.IsNullOrEmpty(guid)) continue;
            
            // Add required system frameworks and libraries according to documentation
            project.AddFrameworkToProject(guid, "SystemConfiguration.framework", false);
            project.AddFrameworkToProject(guid, "CoreTelephony.framework", false);
            project.AddFileToBuild(guid, project.AddFile("usr/lib/libsqlite3.0.tbd", "libsqlite3.0.tbd", PBXSourceTree.Sdk));
            project.AddFileToBuild(guid, project.AddFile("usr/lib/libresolv.tbd", "libresolv.tbd", PBXSourceTree.Sdk));

            // Add -ObjC linker flag as required by documentation
            project.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");

            // Set minimum iOS deployment target
            project.SetBuildProperty(guid, "IPHONEOS_DEPLOYMENT_TARGET", "12.0");

            // Enable ARC (Automatic Reference Counting)
            project.SetBuildProperty(guid, "CLANG_ENABLE_OBJC_ARC", "YES");
        }

        // Write changes back to project
        project.WriteToFile(projectPath);

        Debug.Log("[HttpDNS PostProcessor] Xcode project frameworks and build settings configured");
    }

    private static void ConfigurePodfile(string pathToBuiltProject)
    {
        string podfilePath = Path.Combine(pathToBuiltProject, "Podfile");
        
        // Create Podfile content according to documentation
        string podfileContent = string.Empty;
        podfileContent += "# Podfile for Unity iOS HttpDNS integration\n";
        podfileContent += "# Following Alibaba Cloud HTTPDNS iOS SDK documentation\n\n";
        podfileContent += "source 'https://github.com/CocoaPods/Specs.git'\n";
        podfileContent += "source 'https://github.com/aliyun/aliyun-specs.git'\n\n";
        podfileContent += "platform :ios, '12.0'\n";
        podfileContent += "use_frameworks!\n\n";
        podfileContent += "target 'Unity-iPhone' do\n";
        podfileContent += "  # Alibaba Cloud HTTPDNS iOS SDK\n";
        podfileContent += "  pod 'AlicloudHTTPDNS', '~> 3.2.0'\n";
        podfileContent += "end\n\n";
        podfileContent += "target 'UnityFramework' do\n";
        podfileContent += "  # Alibaba Cloud HTTPDNS iOS SDK\n";
        podfileContent += "  pod 'AlicloudHTTPDNS', '~> 3.2.0'\n";
        podfileContent += "end\n\n";
        podfileContent += "post_install do |installer|\n";
        podfileContent += "  installer.pods_project.targets.each do |target|\n";
        podfileContent += "    target.build_configurations.each do |config|\n";
        podfileContent += "      config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '12.0'\n";
        podfileContent += "      config.build_settings['ENABLE_BITCODE'] = 'NO'\n";
        podfileContent += "      config.build_settings['ALWAYS_SEARCH_USER_PATHS'] = 'YES'\n";
        podfileContent += "    end\n";
        podfileContent += "  end\n";
        podfileContent += "end\n";

        try
        {
            File.WriteAllText(podfilePath, podfileContent);
            Debug.Log("[HttpDNS PostProcessor] Podfile created successfully");
            
            // Create installation instructions file
            string instructionsPath = Path.Combine(pathToBuiltProject, "HTTPDNS_SETUP_INSTRUCTIONS.txt");
            string instructions = "HttpDNS iOS Setup Instructions\n" +
                "============================\n\n" +
                "After Unity build completes, please follow these steps:\n\n" +
                "1. Open Terminal and navigate to the iOS build directory:\n" +
                "   cd \"" + pathToBuiltProject + "\"\n\n" +
                "2. Install CocoaPods dependencies:\n" +
                "   pod install --repo-update\n\n" +
                "3. Open the .xcworkspace file (NOT .xcodeproj):\n" +
                "   open Unity-iPhone.xcworkspace\n\n" +
                "4. Build and run your project from Xcode\n\n" +
                "Important Notes:\n" +
                "- Always use the .xcworkspace file after running 'pod install'\n" +
                "- Make sure you have CocoaPods installed: sudo gem install cocoapods\n" +
                "- If you encounter issues, try: pod repo update\n\n" +
                "For more information, see: https://help.aliyun.com/document_detail/2868038.html\n";
            
            File.WriteAllText(instructionsPath, instructions);
            Debug.Log("[HttpDNS PostProcessor] Setup instructions created");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[HttpDNS PostProcessor] Failed to create Podfile: " + e.Message);
        }
    }
}
#endif