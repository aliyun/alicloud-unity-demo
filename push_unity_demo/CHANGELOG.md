# 更新日志

## [1.0.0-beta] - 2025-12-11

### 重大更新

这是阿里云移动推送Unity SDK的首个正式版本，完成了全面重构，提供了更加统一、易用的API接口。

### 新增功能

#### 跨平台统一API
- ✨ 提供统一的`PushHelper`类，封装iOS和Android的推送功能
- ✨ 跨平台方法自动适配，开发者无需关心平台差异
- ✨ 统一的回调机制，简化错误处理

#### 核心功能
- ✅ 推送服务初始化（`InitPush`）
- ✅ 账号绑定/解绑（`BindAccount` / `UnbindAccount`）
- ✅ 标签管理（`BindTag` / `UnbindTag` / `ListTags`）
- ✅ 别名管理（`AddAlias` / `RemoveAlias` / `ListAliases`）
- ✅ 设备ID获取（`GetDeviceId`）
- ✅ 日志级别控制（`SetLogLevel`）

#### Android专属功能
- 📱 厂商通道初始化（`InitAndroidThirdPush`）
  - 支持小米、华为、OPPO、vivo、魅族等主流厂商
- 📱 手机号绑定/解绑（`BindPhoneNumber` / `UnbindPhoneNumber`）
- 📱 通知权限检查（`IsNotificationEnabled` / `IsNotificationChannelEnabled`）
- 📱 通知设置跳转（`JumpToNotificationSetting` / `JumpToNotificationChannelSetting`）
- 📱 通知渠道创建（`CreateNotificationChannel`）
  - 支持Android 8.0+的通知渠道管理
  - 可配置重要性级别、震动、指示灯等
- 📱 统一的消息接收器（`SetAndroidMessageReceiver`）
  - 透传消息回调
  - 通知到达回调
  - 通知点击回调
  - 通知移除回调
  - 应用内通知接收回调
  - 厂商通道通知点击回调

#### iOS专属功能
- 🍎 统一的消息接收器（`SetIOSMessageReceiver`）
  - 透传消息接收回调
  - APNS注册成功/失败回调
  - 前台通知接收回调
  - 通知点击回调
  - 通知移除回调
  - 静默通知回调
- 🍎 前台通知处理模式（`SetIOSForegroundNotificationMode`）
  - 展示通知且调用回调（`IOS_FOREGROUND_SHOW_AND_CALLBACK`）
  - 只展示通知（`IOS_FOREGROUND_SHOW_ONLY`）
  - 只调用回调（`IOS_FOREGROUND_CALLBACK_ONLY`）

### API改进

#### 统一的消息回调设置
```csharp
// Android
PushHelper.SetAndroidMessageReceiver(
    onMessage: (title, body) => { },
    onNotification: (title, body, extraMap) => { },
    onNotificationOpened: (title, body, extraMapString) => { }
);

// iOS
PushHelper.SetIOSMessageReceiver(
    onMessageReceived: (title, body) => { },
    onNotificationOpened: (title, body, subtitle, userInfo) => { }
);
```

#### 注册流程标准化

旧版注册方式：

```csharp
#if UNITY_IOS
PushHelper.Init("******", "********", (result, data) => Log("Init " + result + " " + data));
#elif UNITY_ANDROID
PushHelper.Init(null, null, (result, data) => Log("Init " + result + " " + data));
#endif
PushHelper.Register(this, (result, data) => Log("Register " + result + " " + data));
```

新版注册方式：

```csharp
// 推送注册
PushHelper.InitPush("您的AppKey", "您的AppSecret", (success, message) => {
    if (success) {
        Debug.Log("推送初始化成功: " + message);
        // 初始化成功后可以获取设备ID
        // string deviceId = PushHelper.GetDeviceId();
        // Debug.Log("设备ID: " + deviceId);
    } else {
        Debug.LogError("推送初始化失败: " + message);
    }
});
#if UNITY_ANDROID
// 安卓厂商通道注册
PushHelper.InitAndroidThirdPush();
#endif
```

#### 常量定义
- 📝 日志级别常量：`LOG_OFF`, `LOG_ERROR`, `LOG_WARN`, `LOG_INFO`, `LOG_DEBUG`
- 📝 通知重要性常量（Android）：`IMPORTANCE_NONE` ~ `IMPORTANCE_MAX`
- 📝 前台通知模式常量（iOS）：`IOS_FOREGROUND_*`

### 示例应用改进

- 🎨 更新`ClickHandler.cs`示例代码，展示所有API用法
- 🎨 添加输入对话框支持，方便测试账号、标签、别名等功能
- 🎨 改进日志显示，所有操作结果实时展示
- 🎨 添加通知渠道创建示例（Android）
- 🎨 添加前台通知处理示例（iOS）

### 文档更新

- 📚 全新的README文档
  - 详细的快速开始指南
  - 完整的API参考
  - 平台特定配置说明
  - 常见问题解答
- 📚 添加CHANGELOG文档
- 📚 改进代码注释，所有公开API都有详细说明

### 兼容性说明

- ✅ Unity 6000.2.10f1
- ✅ iOS 12.0+
- ✅ Android 5.0+ (API Level 21+)

### 升级指南

如果您之前使用过旧版本的SDK，请注意以下变更：

1. **API命名变更**
   - `Register` → `InitPush`
   - `UnBindAccount` → `UnbindAccount`
   - `UnBindTag` → `UnbindTag`
   - `ListTag` → `ListTags`
   - `ListAlias` → `ListAliases`
   - `BindPhone` → `BindPhoneNumber`
   - `UnBindPhone` → `UnbindPhoneNumber`
   - `DeviceId()` → `GetDeviceId()`

2. **回调设置方式变更**
   - 使用统一的`SetAndroidMessageReceiver`和`SetIOSMessageReceiver`
   - 支持一次性设置所有回调

3. **初始化方式变更**
   - 使用统一的`InitPush`方法
   - Android的AppKey/AppSecret仍需在AndroidManifest.xml中配置

### 致谢

感谢所有为本项目做出贡献的开发者和测试人员。