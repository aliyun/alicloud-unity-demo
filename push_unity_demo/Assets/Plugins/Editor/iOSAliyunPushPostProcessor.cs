#if UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

/// <summary>
/// Post-build processor for iOS Aliyun Push integration
/// Automatically configures the Xcode project with required Podfile
/// Following Alibaba Cloud Push iOS SDK documentation
/// </summary>
public class iOSAliyunPushPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        Debug.Log("[Aliyun Push PostProcessor] Configuring iOS project for Aliyun Push...");

        try
        {
            ConfigurePodfile(pathToBuiltProject);
            Debug.Log("[Aliyun Push PostProcessor] iOS project configuration completed successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Aliyun Push PostProcessor] Failed to configure iOS project: " + e.Message);
        }
    }

    private static void ConfigurePodfile(string pathToBuiltProject)
    {
        string podfilePath = Path.Combine(pathToBuiltProject, "Podfile");

        // Check if Podfile already exists and contains AlicloudPush
        if (File.Exists(podfilePath))
        {
            string existingContent = File.ReadAllText(podfilePath);
            if (existingContent.Contains("AlicloudPush"))
            {
                Debug.Log("[Aliyun Push PostProcessor] Podfile already contains AlicloudPush dependency");
                return;
            }
        }

        // Create Podfile content according to documentation
        string podfileContent = string.Empty;
        podfileContent += "# Podfile for Unity iOS Aliyun Push integration\n";
        podfileContent += "# Following Alibaba Cloud Push iOS SDK documentation\n\n";
        podfileContent += "source 'https://github.com/CocoaPods/Specs.git'\n";
        podfileContent += "source 'https://github.com/aliyun/aliyun-specs.git'\n\n";
        podfileContent += "platform :ios, '12.0'\n\n";
        podfileContent += "target 'Unity-iPhone' do\n";
        podfileContent += "  use_frameworks!\n";
        podfileContent += "  \n";
        podfileContent += "  # Alibaba Cloud Push iOS SDK\n";
        podfileContent += "  pod 'AlicloudPush', '~> 3.0'\n";
        podfileContent += "end\n\n";
        podfileContent += "target 'UnityFramework' do\n";
        podfileContent += "  use_frameworks!\n";
        podfileContent += "  \n";
        podfileContent += "  # Alibaba Cloud Push iOS SDK\n";
        podfileContent += "  pod 'AlicloudPush', '~> 3.0'\n";
        podfileContent += "end\n";

        try
        {
            File.WriteAllText(podfilePath, podfileContent);
            Debug.Log("[Aliyun Push PostProcessor] Podfile created successfully");

            // Create installation instructions file
            string instructionsPath = Path.Combine(pathToBuiltProject, "ALIYUN_PUSH_SETUP_INSTRUCTIONS.txt");
            string instructions = "Aliyun Push iOS Setup Instructions\n" +
                "====================================\n\n" +
                "After Unity build completes, please follow these steps:\n\n" +
                "1. Open Terminal and navigate to the iOS build directory:\n" +
                "   cd \"" + pathToBuiltProject + "\"\n\n" +
                "2. Install CocoaPods dependencies:\n" +
                "   pod install --repo-update\n\n" +
                "3. Open the .xcworkspace file (NOT .xcodeproj):\n" +
                "   open Unity-iPhone.xcworkspace\n\n" +
                "4. Configure Push Notifications in Xcode:\n" +
                "   a. Select Unity-iPhone target (not UnityFramework)\n" +
                "   b. Go to 'Signing & Capabilities' tab\n" +
                "   c. Click '+ Capability' and add:\n" +
                "      - Push Notifications\n" +
                "      - Background Modes (enable 'Remote notifications')\n" +
                "   d. Configure your Team and Signing Certificate\n\n" +
                "5. Build and run your project from Xcode\n\n" +
                "Important Notes:\n" +
                "- Always use the .xcworkspace file after running 'pod install'\n" +
                "- Make sure you have CocoaPods installed: sudo gem install cocoapods\n" +
                "- If you encounter issues, try: pod repo update\n" +
                "- Push Notifications require a valid provisioning profile with push capability\n" +
                "- Test on a real device (push notifications don't work on simulator)\n\n" +
                "For more information, see:\n" +
                "- iOS SDK Documentation: https://help.aliyun.com/document_detail/434660.html\n" +
                "- Push Certificate Setup: https://help.aliyun.com/document_detail/434661.html\n";

            File.WriteAllText(instructionsPath, instructions);
            Debug.Log("[Aliyun Push PostProcessor] Setup instructions created");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[Aliyun Push PostProcessor] Failed to create Podfile: " + e.Message);
        }
    }
}
#endif
