# Alicloud APM SDK

## 简介
Alicloud APM SDK 为 Unity 应用提供阿里云 EMAS 端监控能力，当前重点覆盖 Crash Analysis 组件，并随包提供 iOS/Android 原生依赖及必要的编辑器自动化。导入 `alicloud-apm` 目录即可获取运行时代码、平台桥接与构建期脚本。

## 目录结构
- `Runtime/`：SDK 运行时入口、配置、Crash Analysis 组件及平台适配代码
- `Plugins/`：iOS `.xcframework` 与 Android `.aar/.jar` 依赖，随包自动参与平台构建
- `Editor/`：Unity 编辑器自动化（IL2CPP 命令注入、Xcode 后处理、版本号生成）

## 环境要求
- iOS/Android 平台需启用 IL2CPP 脚本后端，并包含 ARM64 架构
- Android Gradle 构建工具需允许引入随包提供的 `.aar/.jar` 依赖
- iOS 构建需使用 Xcode 14 及以上版本

## 快速集成
1. 在 Unity 中导入发布的 `alicloud-apm.unitypackage`。
2. 尽早调用 `Apm.Start(options)` 完成 SDK 初始化，可以参考以下两种示例方式，并替换示例中的云端凭据：

### 场景加载前初始化示例（推荐）
```csharp
using Alicloud.Apm;
using UnityEngine;

internal static class ApmBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Apm.IsStarted())
        {
            return;
        }

        var options = new ApmOptions("<AppKey>", "<AppSecret>", "<AppRsaSecret>")
        {
            UserId = "<UserId>",
            UserNick = "<UserNick>"
        };

        Apm.Start(options);
    }
}
```

### 场景入口脚本示例
```csharp
using Alicloud.Apm;
using UnityEngine;

public class ApmBootstrapBehaviour : MonoBehaviour
{
    void Awake()
    {
        if (Apm.IsStarted())
        {
            return;
        }

        var options = new ApmOptions("<AppKey>", "<AppSecret>", "<AppRsaSecret>")
        {
            UserId = "<UserId>",
            UserNick = "<UserNick>"
        };

        Apm.Start(options);
    }
}
```

### 更新用户信息与自定义维度
```csharp
Apm.SetUserId(playerId);
Apm.SetUserNick(displayName);
Apm.SetCustomKeyValue("vip_level", currentVipLevel);
```
如需批量更新，可使用 `Apm.SetCustomKeysAndValues(IDictionary<string, object>)`。

## Crash Analysis 功能
- 组件默认随 `SDKComponents.CrashAnalysis` 启用，并通过 `CrashAnalysisComponentRegistrar` 自动注册。
- `CrashAnalysisConfiguration.ReportLogLevel` 默认值为 `LogType.Exception`，可在启动前按需调整：
- `CrashAnalysisConfiguration.ExceptionMode` 默认值为 `ExceptionPolicy.ReportImmediately`，即捕获到异常后立即上报，如需延迟或停用可调整为其他策略值。
```csharp
using UnityEngine;
using Alicloud.Apm.CrashAnalysis;

CrashAnalysisConfiguration.ReportLogLevel = LogType.Exception;
CrashAnalysisConfiguration.ExceptionMode = ExceptionPolicy.ReportImmediately;
```
- 记录自定义异常：
```csharp
try
{
    // ... game logic
}
catch (Exception ex)
{
    CrashAnalysis.RecordException(ex);
}

CrashAnalysis.RecordExceptionModel(new ExceptionModel
{
    Name = "LuaError",
    Reason = errorMessage,
    Language = "Lua"
});
```
- 使用 `CrashAnalysis.Log("message")` 向原生 SDK 写入调试日志，便于排查。

## 运行时配置
- `Apm.IsStarted()`：查询 SDK 是否已完成初始化。
- `Apm.Options`：读取当前生效的配置快照（只读）。
- `CrashAnalysisConfiguration.ResetToDefaults()`：恢复 Crash 配置默认值。

## 平台说明
### iOS
- SDK 打包内置 `AliHACore`, `AlicloudApmCore`, `AlicloudApmCrashAnalysis`, `AlicloudApmCBridge`, `UTDID` 等 `.xcframework`。
- 编辑器在 Xcode 导出后会自动添加 `-ObjC`、Swift 运行库路径等，通常无需手动配置。
- 如需符号还原，确保使用 Unity 生成的 `dSYM` 并上传至阿里云 EMAS 平台。

### Android
- `Plugins/Android` 目录包含所需 `.aar/.jar` 依赖，默认勾选在 Android 平台生效。
- 若自定义 Gradle 模板，请确认未过滤随包附带的依赖库和 proguard 配置。

## 编辑器自动化
- `Il2CppArgsInjector` 会在构建前自动注入 `--emit-source-mapping`，以便在 Crash 报告中还原 C# 方法符号，构建结束后会恢复原设置。
- `XcodeProjectPostProcessor` 负责配置 iOS 工程的运行/链接路径。
- `PackageVersionGenerator` 会在进入编辑器或构建前刷新 `Runtime/Internal/Generated/ApmVersionInfo.cs`，无需手动维护。

## 常见问题
- **未看到 Crash 上报？** 确认目标平台构建启用 IL2CPP、symbol 文件已上传，并检查 `CrashAnalysisConfiguration.ExceptionMode` 是否被设置为 `Disabled`。
- **重复初始化提示**：`Apm.Start` 只能调用一次，如需更新用户信息请使用 `SetUserId/SetUserNick` 等接口。
- **发生字符串长度警告**：用户 ID、昵称及自定义 key/value 最长 128 个字符，超出部分会被截断或忽略。

如需更多平台接入指南与后台配置说明，请联系阿里云 EMAS 技术支持。
